using System.Data;
using System.Diagnostics;
using System.Reflection;
using System.Security.Cryptography;
using SqlSugar;

Console.OutputEncoding = new System.Text.UTF8Encoding(false);
foreach (var name in new[] { "SonnetDB.Core", "SonnetDB.Data", "SonnetDB.Protocol", "SqlSugar", "SqlSugar.SonnetDBCore" })
{
    var assembly = Assembly.Load(name);
    Console.WriteLine($"ASSEMBLY|{name}|{assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion}|{Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(assembly.Location)))}");
}
var databasePath = Path.Combine(Path.GetTempPath(), "sqlsugar-sonnetdb-" + Guid.NewGuid().ToString("N"));
var failures = 0;
var statements = new List<string>();
using var db = new SqlSugarClient(new ConnectionConfig
{
    ConnectionString = Environment.GetEnvironmentVariable("SQLSUGAR_SONNETDB_CONNECTION") ?? "Data Source=" + databasePath,
    DbType = SqlSugar.DbType.SonnetDB,
    InitKeyType = InitKeyType.Attribute,
    IsAutoCloseConnection = true
});
db.Aop.OnExecutingChangeSql = (sql, parameters) =>
{
    statements.Add(sql);
    return new KeyValuePair<string, SugarParameter[]>(sql, parameters);
};

try
{
    Check("维护接口建表与元数据", () =>
    {
        foreach (var name in new[] { "bulk_records", "fast_records" })
        {
            db.DbMaintenance.CreateTable(name, new List<DbColumnInfo>
            {
                new() { DbColumnName = "Id", DataType = "INT", IsPrimarykey = true },
                new() { DbColumnName = "Name", DataType = "STRING" },
                new() { DbColumnName = "Amount", DataType = "DECIMAL", Length = 18, DecimalDigits = 4 },
                new() { DbColumnName = "Enabled", DataType = "BOOL" }
            });
        }
        Assert(db.DbMaintenance.IsAnyTable("bulk_records", false), "表元数据缺失");
        Assert(db.DbMaintenance.GetColumnInfosByTableName("bulk_records", false).Count == 4, "列元数据缺失");
        db.Ado.ExecuteCommand("CREATE TABLE identity_records (Id INT AUTO_INCREMENT NOT NULL, Name STRING NOT NULL, PRIMARY KEY (Id))");
    });

    Check("同步与异步自增键回写", () =>
    {
        var record = new IdentityRecord { Name = "sync" };
        Assert(db.Insertable(record).ExecuteCommandIdentityIntoEntity() && record.Id == 1, "同步自增键回写错误");
        record = new IdentityRecord { Name = "async" };
        Assert(db.Insertable(record).ExecuteCommandIdentityIntoEntityAsync().GetAwaiter().GetResult() && record.Id == 2, "异步自增键回写错误");
    });

    var records = Enumerable.Range(1, 10000).Select(id => new BulkRecord
    {
        Id = id, Name = "设备-" + id, Amount = id + 0.1234m, Enabled = id % 2 == 0
    }).ToList();

    Check("Feature10：一万行 Insertable 多值插入", () =>
    {
        statements.Clear();
        Assert(db.Insertable(records).PageSize(1000).ExecuteCommand() == records.Count, "批量插入影响行数错误");
        Assert(CountCommands("INSERT") == 10, "一万行应分为十个多值 INSERT");
        Assert(db.Queryable<BulkRecord>().Count() == records.Count, "批量插入总行数错误");
        Assert(db.Queryable<BulkRecord>().InSingle(9999).Amount == 9999.1234m, "decimal 精度丢失");
    });

    Check("Feature10：一万行 Updateable CASE 集合更新", () =>
    {
        foreach (var record in records) { record.Name = "更新-" + record.Id; record.Amount += 1m; }
        statements.Clear();
        Assert(db.Updateable(records).PageSize(1000).ExecuteCommand() == records.Count, "集合更新影响行数错误");
        Assert(CountCommands("UPDATE") == 10, "一万行应分为十个集合 UPDATE");
        var stored = db.Queryable<BulkRecord>().InSingle(9999);
        Assert(stored.Name == "更新-9999" && stored.Amount == 10000.1234m, "集合更新值错误");
    });

    Check("Feature10：一万行 Fastest 多值插入", () =>
    {
        statements.Clear();
        Assert(db.Fastest<BulkRecord>().AS("fast_records").BulkCopy(records) == records.Count, "Fastest 插入影响行数错误");
        Assert(CountCommands("INSERT") == 10, "Fastest 应每批执行一个多值 INSERT");
        Assert(db.Queryable<BulkRecord>().AS("fast_records").Count() == records.Count, "Fastest 插入总行数错误");
    });

    Check("Feature10：一万行 Fastest 关联集合更新", () =>
    {
        foreach (var record in records) record.Amount += 2m;
        statements.Clear();
        Assert(db.Fastest<BulkRecord>().AS("fast_records").BulkUpdate(records) == records.Count, "Fastest 更新影响行数错误");
        Assert(CountCommands("UPDATE") == 1, "Fastest 应执行一次关联集合 UPDATE");
        Assert(CountCommands("INSERT") == 10, "staging 应批量导入");
        Assert(db.Queryable<BulkRecord>().AS("fast_records").InSingle(9999).Amount == 10002.1234m, "Fastest 集合更新错误");
        Assert(!db.DbMaintenance.GetTableInfoList(false).Any(table => table.Name.StartsWith("sonnet_bulk_")), "staging 表没有清理");
    });

    Check("Fastest 批量合并", () =>
    {
        var merged = new List<BulkRecord>
        {
            new() { Id = 1, Name = "合并更新", Amount = 0.1234m },
            new() { Id = 10001, Name = "合并插入", Amount = 0.5678m }
        };
        statements.Clear();
        Assert(db.Fastest<BulkRecord>().AS("fast_records").BulkMerge(merged) == 2, "批量合并影响行数错误");
        Assert(CountCommands("UPDATE") == 1 && CountCommands("INSERT") == 2, "合并应通过集合语句执行");
        Assert(db.Queryable<BulkRecord>().AS("fast_records").InSingle(1).Name == "合并更新", "合并更新错误");
        Assert(db.Queryable<BulkRecord>().AS("fast_records").InSingle(10001).Amount == 0.5678m, "合并插入错误");
    });

    Check("Fastest 部分列 DataTable 更新", () =>
    {
        var changes = new DataTable();
        changes.Columns.Add("Id", typeof(int));
        changes.Columns.Add("Amount", typeof(decimal));
        changes.Rows.Add(2, 0.4321m);
        changes.Rows.Add(3, 0.8765m);
        Assert(db.Fastest<BulkRecord>().BulkUpdate("fast_records", changes, ["Id"], ["Amount"]) == 2,
            "部分列集合更新影响行数错误");
        Assert(db.Queryable<BulkRecord>().AS("fast_records").InSingle(3).Amount == 0.8765m, "部分列更新错误");
    });

    Check("Fastest 多条新自增记录合并与 OffIdentity", () =>
    {
        Assert(db.Fastest<IdentityRecord>().BulkMerge(new List<IdentityRecord>
        {
            new() { Name = "新自增-A" }, new() { Name = "新自增-B" }
        }, ["Name"], ["Name"]) == 2, "重复默认自增值妨碍合并");
        Assert(db.Fastest<IdentityRecord>().OffIdentity().BulkCopy(new List<IdentityRecord>
        {
            new() { Id = 100, Name = "显式自增值" }
        }) == 1 && db.Queryable<IdentityRecord>().InSingle(100).Name == "显式自增值", "OffIdentity 未传递");
    });

    Check("Fastest BatchSize 与重复键忽略", () =>
    {
        var batch = Enumerable.Range(20001, 25).Select(id => new BulkRecord { Id = id, Name = "batch" }).ToList();
        statements.Clear();
        Assert(db.Fastest<BulkRecord>().AS("fast_records").BatchSize(10).BulkCopy(batch) == 25, "小批次插入失败");
        Assert(CountCommands("INSERT") == 3, "BatchSize 未传递");
        Assert(db.Fastest<BulkRecord>().AS("fast_records").IgnoreInsertError().BulkCopy(batch) == 0,
            "重复键忽略影响行数错误");
    });

    Check("分页、字符串、类型转换与集合查询", () =>
    {
        var page = db.Queryable<BulkRecord>().OrderBy(it => it.Id).Skip(20).Take(10).ToList();
        Assert(page.Count == 10 && page[0].Id == 21, "分页错误");
        var projection = db.Queryable<BulkRecord>().Where(it => it.Id == 1).Select(it => new
        {
            Length = it.Name.Length, Name = it.Name.Replace("更新", "设备"), Number = SqlFunc.ToInt32(it.Id)
        }).Single();
        Assert(projection.Length > 0 && projection.Name == "设备-1" && projection.Number == 1, "原生函数映射错误");
        Assert(db.UnionAll(db.Queryable<BulkRecord>().Where(it => it.Id == 1),
            db.Queryable<BulkRecord>().Where(it => it.Id == 1)).Count() == 2, "UNION ALL 丢失重复行");
        Assert(db.Ado.GetInt("SELECT COUNT(DISTINCT Id) FROM bulk_records") == 10000, "聚合 DISTINCT 错误");
    });

    Check("精确 decimal、TIME 与数据填充", () =>
    {
        db.Ado.ExecuteCommand("CREATE TABLE typed_records (Id INT NOT NULL, Amount DECIMAL(18,4), At TIME, PRIMARY KEY (Id))");
        db.Ado.ExecuteCommand("INSERT INTO typed_records (Id,Amount,At) VALUES (@id,@amount,@time)",
            new SugarParameter("@id", 1), new SugarParameter("@amount", 0.1234m),
            new SugarParameter("@time", new TimeOnly(8, 30)));
        var table = db.Ado.GetDataTable("SELECT Amount,At FROM typed_records");
        Assert((decimal)table.Rows[0]["Amount"] == 0.1234m, "decimal 数据填充错误");
        Assert((TimeOnly)table.Rows[0]["At"] == new TimeOnly(8, 30), "TIME 数据填充错误");
    });

    Check("窗口、日期、JSON、复合键与关联更新", () => QueryChecks.Run(db));
    Check("DbFirst 默认值编译与 TIME 实体读回", () => MetadataChecks.Run(db));

    Check("集合 DML 事务回滚", () =>
    {
        db.Ado.BeginTran();
        try
        {
            db.Updateable(new List<BulkRecord>
            {
                new() { Id = 1, Name = "回滚", Amount = -1 },
                new() { Id = 2, Name = "回滚", Amount = -2 }
            }).ExecuteCommand();
            Assert(db.Queryable<BulkRecord>().InSingle(1).Amount == -1, "事务内读己之写失败");
        }
        finally { db.Ado.RollbackTran(); }
        Assert(db.Queryable<BulkRecord>().InSingle(1).Amount == 2.1234m, "集合更新未回滚");
    });

    Check("删除", () => Assert(db.Deleteable<BulkRecord>().In(10000).ExecuteCommand() == 1, "删除错误"));

    if (args.Contains("--check-capabilities"))
    {
        Check("完整性：基类 CodeFirst 的事务 DDL", () => db.CodeFirst.InitTables<CodeFirstRecord>());
        Check("完整性：ADO 多语句和多个结果集", () =>
            Assert(db.Ado.GetDataSetAll("SELECT 1 AS value; SELECT 2 AS value").Tables.Count == 2, "缺少结果集"));
        Check("完整性：原生临时表", () => db.Ado.ExecuteCommand(
            "CREATE TEMP TABLE capability_temp AS SELECT Id,Name FROM bulk_records WHERE FALSE"));
        Check("完整性：事务行锁", () =>
        {
            db.Ado.BeginTran();
            try { db.Queryable<BulkRecord>().Where(it => it.Id == 1).TranLock().ToList(); }
            finally { db.Ado.RollbackTran(); }
        });
        Check("完整性：高有效位 decimal 参数保真", () =>
        {
            db.Ado.ExecuteCommand("CREATE TABLE precision_records (Id INT, Amount DECIMAL(38,28), At DATETIME2, PRIMARY KEY(Id))");
            const decimal amount = 123456789.1234567890123456789m;
            db.Ado.ExecuteCommand("INSERT INTO precision_records(Id,Amount) VALUES(1,@amount)", new SugarParameter("@amount", amount));
            Assert(db.Ado.GetDecimal("SELECT Amount FROM precision_records WHERE Id=1") == amount, "decimal 参数有效位丢失（SonnetDB #235）");
        });
        Check("完整性：显式 DATETIME2 亚毫秒参数保真", () =>
        {
            var instant = new DateTime(2026, 10, 8, 1, 2, 3, DateTimeKind.Utc).AddTicks(4567890);
            db.Ado.ExecuteCommand("UPDATE precision_records SET At=@instant WHERE Id=1", new SugarParameter("@instant", instant) { DbType = System.Data.DbType.DateTime2 });
            Assert(Convert.ToDateTime(db.Ado.GetScalar("SELECT At FROM precision_records WHERE Id=1")).Ticks == instant.Ticks,
                "DATETIME2 参数亚毫秒精度丢失（SonnetDB #236）");
        });
        Check("完整性：旧 DATETIME 不静默截断亚毫秒参数", () =>
        {
            db.Ado.ExecuteCommand("CREATE TABLE legacy_time_precision (Id INT, At DATETIME, PRIMARY KEY(Id))");
            var rejected = false;
            try
            {
                db.Ado.ExecuteCommand("INSERT INTO legacy_time_precision(Id,At) VALUES(1,@instant)",
                    new SugarParameter("@instant", new DateTime(2026, 10, 8, 1, 2, 3, DateTimeKind.Utc).AddTicks(1)));
            }
            catch (ArgumentException) { rejected = true; }
            Assert(rejected && db.Ado.GetInt("SELECT COUNT(*) FROM legacy_time_precision") == 0,
                "旧 DATETIME 必须拒绝不兼容的精度且不写入数据。");
        });
        Check("完整性：ALTER DECIMAL 精度元数据", () =>
        {
            db.DbMaintenance.AddColumn("precision_records", new DbColumnInfo
            {
                DbColumnName = "Fee", DataType = "DECIMAL", Length = 12, DecimalDigits = 2, IsNullable = true
            });
            var column = db.DbMaintenance.GetColumnInfosByTableName("precision_records", false).Single(it => it.DbColumnName == "Fee");
            Assert(column.Length == 12 && column.DecimalDigits == 2, "ALTER DECIMAL(p,s) 丢失精度声明（SonnetDB #241）");
        });
    }
}
finally
{
    db.Dispose();
    var resolvedPath = Path.GetFullPath(databasePath);
    var tempRoot = Path.GetFullPath(Path.GetTempPath());
    if (resolvedPath.StartsWith(tempRoot, StringComparison.OrdinalIgnoreCase) && Directory.Exists(resolvedPath))
        Directory.Delete(resolvedPath, recursive: true);
}
Console.WriteLine(failures == 0 ? "所有已执行验证通过。" : $"验证失败：{failures} 项。");
return failures == 0 ? 0 : 1;

int CountCommands(string prefix) => statements.Count(sql => sql.TrimStart().StartsWith(prefix + " ", StringComparison.OrdinalIgnoreCase));

void Check(string name, Action action)
{
    var timer = Stopwatch.StartNew();
    try { action(); Console.WriteLine($"PASS {name} ({timer.ElapsedMilliseconds} ms)"); }
    catch (Exception exception) { failures++; Console.Error.WriteLine($"FAIL {name}: {exception.Message}"); }
}

static void Assert(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}

[SugarTable("bulk_records")]
public class BulkRecord
{
    [SugarColumn(IsPrimaryKey = true)]
    public int Id { get; set; }
    public string Name { get; set; } = "";
    [SugarColumn(ColumnDataType = "DECIMAL(18,4)")]
    public decimal Amount { get; set; }
    public bool Enabled { get; set; }
}

[SugarTable("identity_records")]
public class IdentityRecord
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }
    public string Name { get; set; } = "";
}

[SugarTable("codefirst_records")]
public class CodeFirstRecord
{
    [SugarColumn(IsPrimaryKey = true)]
    public int Id { get; set; }
    public string Name { get; set; } = "";
}
