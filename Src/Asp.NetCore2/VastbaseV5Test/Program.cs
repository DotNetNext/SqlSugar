using System;
using System.Collections.Generic;
using SqlSugar;

// VastbaseV5 冒烟：验证 DbType.VastbaseV5 入口的 PG 与 Babelfish 兼容模式(TDS) 两种模式。
// 用例：CodeFirst + CRUD + 分页、事务提交/回滚、BulkCopy、方言标识符分派。
// 默认占位连接串：跳过数据库访问并返回 0（只验证可构建、入口可运行）；同时设置
// VB_V5TEST_PG 与 VB_V5TEST_BF 才执行真实例（任一缺失或仍含 __V5TEST__ 标记即跳过）。
// 调用:dotnet run --project Src/Asp.NetCore2/VastbaseV5Test/VastbaseV5Test.csproj
internal static class Program
{
    private static int _fail;

    private static int Main()
    {
        if (Config.IsPlaceholder)
        {
            Console.WriteLine("VastbaseV5Test: 占位连接串形态(可编译验证)——跳过执行。");
            Console.WriteLine("真实例形态:设置 VB_V5TEST_PG 与 VB_V5TEST_BF 两环境变量后重跑。");
            return 0;
        }

        Run("PG", Config.Pg, expectBf: false);
        Run("Babelfish", Config.Bf, expectBf: true);
        Console.WriteLine(_fail == 0
            ? "VastbaseV5Test PASS (PG + Babelfish)"
            : "VastbaseV5Test FAIL (" + _fail + " checks)");
        return _fail == 0 ? 0 : 1;
    }

    private static void Run(string label, string conn, bool expectBf)
    {
        try
        {
            using (var db = new SqlSugarClient(new ConnectionConfig
            {
                DbType = DbType.VastbaseV5,
                ConnectionString = conn,
                IsAutoCloseConnection = true,
            }))
            {
                // 能开连接发 SQL 即证明 VastbaseV5 → VastbaseV5Provider 反射链通(独立驱动,非 Npgsql 路由)

                // —— ① CodeFirst + CRUD + 分页 ——
                db.CodeFirst.InitTables<V5Order>();
                db.DbMaintenance.DropTable<V5Order>();              // 自清(幂等重跑)
                db.CodeFirst.InitTables<V5Order>();
                var rows = new List<V5Order>();
                for (var i = 1; i <= 20; i++)
                    rows.Add(new V5Order { Id = i, Name = "v5-" + i, Price = i * 1.5m });
                Check(label, "① insert 20", db.Insertable(rows).ExecuteCommand() == 20);
                var page = db.Queryable<V5Order>().OrderBy(x => x.Id).ToPageList(2, 5);
                Check(label, "① page(2,5) 5 行且首行 id=6", page.Count == 5 && page[0].Id == 6);
                db.Updateable<V5Order>().SetColumns(x => x.Name == "updated").Where(x => x.Id == 1).ExecuteCommand();
                Check(label, "① update id=1", db.Queryable<V5Order>().Where(x => x.Id == 1).First().Name == "updated");

                // —— ② 事务:提交持久 + 回滚撤销 ——
                db.Ado.BeginTran();
                db.Insertable(new V5Order { Id = 501, Name = "tran-commit", Price = 1m }).ExecuteCommand();
                db.Ado.CommitTran();
                db.Ado.BeginTran();
                db.Insertable(new V5Order { Id = 502, Name = "tran-rollback", Price = 1m }).ExecuteCommand();
                db.Ado.RollbackTran();
                Check(label, "② commit 持久", db.Queryable<V5Order>().Count(x => x.Id == 501) == 1);
                Check(label, "② rollback 撤销", db.Queryable<V5Order>().Count(x => x.Id == 502) == 0);

                // —— ③ BulkCopy(Fastest 链 → VastbaseV5FastBuilder COPY)——
                var bulk = new List<V5Order>();
                for (var i = 101; i <= 120; i++)
                    bulk.Add(new V5Order { Id = i, Name = "bulk-" + i, Price = i * 0.5m });
                db.Fastest<V5Order>().BulkCopy(bulk);
                Check(label, "③ bulkcopy +20 → count=41", db.Queryable<V5Order>().Count() == 41);

                // —— ④ 模式分派:两串各自命中的方言引号形态 ——
                var sql = db.Queryable<V5Order>().Where(x => x.Id == 1).ToSqlString();
                if (expectBf)
                    Check(label, "④ Babelfish 串 → 方括号方言", sql.IndexOf('[') >= 0);
                else
                    Check(label, "④ PG 串 → 双引号方言", sql.IndexOf('"') >= 0);

                db.DbMaintenance.DropTable<V5Order>();
                Console.WriteLine("[" + label + "] done");
            }
        }
        catch (Exception ex)
        {
            _fail++;
            Console.WriteLine("[" + label + "] EXCEPTION: " + ex.Message);
            Cleanup(conn);
        }
    }

    private static void Check(string label, string what, bool ok)
    {
        if (ok) return;
        _fail++;
        Console.WriteLine("[" + label + "] CHECK FAIL: " + what);
    }

    private static void Cleanup(string conn)
    {
        try
        {
            using (var db = new SqlSugarClient(new ConnectionConfig
            {
                DbType = DbType.VastbaseV5,
                ConnectionString = conn,
                IsAutoCloseConnection = true,
            }))
            {
                if (db.DbMaintenance.IsAnyTable("V5Order")) db.DbMaintenance.DropTable("V5Order");
            }
        }
        catch { /* 清理尽力而为 */ }
    }
}

internal static class Config
{
    // 占位判据:连接串含 __V5TEST__ 标记(默认占位串全含;环境变量注入的真实串不含)。
    // 两串须同为真实形态才执行(需同时覆盖两种模式,仅设其一视为配置不全,按占位跳过)。
    private const string Mark = "__V5TEST__";
    private const string PgPlaceholder =
        "PORT=__V5TEST__;DATABASE=__V5TEST__;HOST=__V5TEST__;PASSWORD=__V5TEST__;USER ID=__V5TEST__";
    private const string BfPlaceholder =
        PgPlaceholder + ";Server Compatibility Mode=Babelfish";

    public static readonly string Pg = Env("VB_V5TEST_PG", PgPlaceholder);
    public static readonly string Bf = Env("VB_V5TEST_BF", BfPlaceholder);

    public static bool IsPlaceholder
    {
        get
        {
            return Pg.IndexOf(Mark, StringComparison.Ordinal) >= 0
                || Bf.IndexOf(Mark, StringComparison.Ordinal) >= 0;
        }
    }

    private static string Env(string key, string fallback)
    {
        var v = Environment.GetEnvironmentVariable(key);
        return string.IsNullOrWhiteSpace(v) ? fallback : v;
    }
}

[SugarTable("V5Order")]
public class V5Order
{
    [SugarColumn(IsPrimaryKey = true)]
    public int Id { get; set; }

    public string Name { get; set; }

    public decimal Price { get; set; }
}
