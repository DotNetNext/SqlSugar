using System;
using System.Collections.Generic;
using System.Linq;

namespace SqlSugar
{
    public class VastbaseV5DbMaintenance : PostgreSQLDbMaintenance
    {
        private VastbaseV5Mode Mode => VastbaseV5Mode.Require(this.Context);

        private static string TrimTrailingSpace(string name) => name?.TrimEnd();

        // Parse "[schema].[table]" / "schema.table" / "table" → schema (default dbo) + table.
        // Babelfish user tables may live in non-dbo schemas (e.g. SugarTable("abp.Foo")); the schema must be
        // preserved for sp_*extendedproperty + OBJECT_ID, which otherwise look in dbo and miss the table.
        private static void SplitRemarkName(string name, out string schema, out string table)
        {
            schema = "dbo";
            table = name;
            if (name == null) return;
            var n = name.Trim().TrimStart('[').TrimEnd(']');
            var dot = n.IndexOf('.');
            if (dot < 0) { table = n; return; }
            var s = n.Substring(0, dot).Trim().TrimStart('[').TrimEnd(']');
            var t = n.Substring(dot + 1).Trim().TrimStart('[').TrimEnd(']');
            if (!string.IsNullOrEmpty(s)) schema = s;
            table = t;
        }

        public override bool IsAnySystemTablePermissions()
        {
            var mode = Mode;
            if (mode != null && mode.IsBabelfish) return mode.MSSQLDbMaintenance.IsAnySystemTablePermissions();
            return base.IsAnySystemTablePermissions();
        }

        public override List<DbColumnInfo> GetColumnInfosByTableName(string tableName, bool isCache = true)
        {
            var mode = Mode;
            if (mode != null && mode.IsBabelfish)
            {
                var cleanName = tableName.Replace("[", "").Replace("]", "").TrimEnd();
                if (cleanName.Contains("."))
                    cleanName = cleanName.Substring(cleanName.LastIndexOf('.') + 1);
                // Babelfish 的 sys.columns/sys.tables 视图对任何谓词（name/object_id）都不能下推，每次查询全量物化，耗时随库内表/列总数线性增长；BulkCopy 每页 COPY 前都调本方法，不缓存则每页重复全库扫描。两点对策：
                //   1. 定位改 OBJECT_ID(name,'U')（函数内部走索引）替代 JOIN sys.tables + WHERE t.name；TableName 由已知表名回填，不取自 sys.tables；
                //   2. isCache=true 走进程级缓存，对齐 base 语义；空结果不落缓存，防「建表前查空被缓存，建表后同进程仍取到空」的陈旧毒化。
                // decimal/numeric 的 max_length 是字节数（decimal(18,4)=9）非精度，拼显式精度须用 c.precision（对齐 PG 模式 information_schema 语义），批量 UPDATE CAST 才能拼 numeric(p,s)。
                string sql = @"SELECT
                    CAST(N'" + cleanName + @"' as nvarchar(128)) as TableName,
                    0 as TableId,
                    CAST(c.name as nvarchar(128)) as DbColumnName,
                    CAST(TYPE_NAME(c.user_type_id) as nvarchar(128)) as DataType,
                    CASE WHEN TYPE_NAME(c.user_type_id) IN ('decimal','numeric') THEN c.precision ELSE c.max_length END as Length,
                    NULL as DefaultValue,
                    c.scale as Scale,
                    c.scale as DecimalDigits,
                    CAST(cep.value as nvarchar(2000)) as ColumnDescription,
                    c.is_identity as IsIdentity,
                    c.is_nullable as IsNullable,
                    CAST(CASE WHEN ic.column_id IS NOT NULL THEN 1 ELSE 0 END AS BIT) as IsPrimarykey
                FROM sys.columns c
                LEFT JOIN sys.indexes i ON c.object_id = i.object_id AND i.is_primary_key = 1
                LEFT JOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id AND ic.column_id = c.column_id
                LEFT JOIN sys.extended_properties cep ON cep.major_id = c.object_id AND cep.minor_id = c.column_id AND cep.name = N'MS_Description'
                WHERE c.object_id = OBJECT_ID(N'" + cleanName + @"', N'U')
                ORDER BY c.column_id";
                string cacheKey = null;
                ICacheService metaCache = null;
                if (isCache)
                {
                    cacheKey = GetCacheKey("DbMaintenanceProvider.GetColumnInfosByTableName."
                        + cleanName.ToLower() + this.Context.CurrentConnectionConfig.ConfigId);
                    metaCache = this.Context.Utilities.GetReflectionInoCacheInstance();
                    if (metaCache.ContainsKey<List<DbColumnInfo>>(cacheKey))
                        return metaCache.Get<List<DbColumnInfo>>(cacheKey);
                }
                var dt = this.Context.Ado.GetDataTable(sql);
                List<DbColumnInfo> result = new List<DbColumnInfo>();
                foreach (System.Data.DataRow dr in dt.Rows)
                {
                    DbColumnInfo column = new DbColumnInfo();
                    column.TableName = (dr["TableName"] + "").TrimEnd();
                    column.DbColumnName = dr["DbColumnName"] + "";
                    column.DataType = dr["DataType"] + "";
                    // 无长度 LOB 类型：sys.columns.max_length 返指针伪值（xml=-1 / text=16 / ntext,image=16），与实体 Length=0 语义不符 → CodeFirst re-init 误判"需要变更"，误发 ALTER COLUMN xml/text——Babelfish 报 syntax error at or near "xml"/"text"。归一 0 对齐 PG 模式语义。⚠ nvarchar(max)（-1）不可归一：base CodeFirst 的 bigStringArray 特判依赖 Length==-1 判断 isMaxString；varchar(max) 亦不归一（其 ALTER 受支持，幂等无害）。
                    if (column.DataType.EqualCase("xml") || column.DataType.EqualCase("text")
                        || column.DataType.EqualCase("ntext") || column.DataType.EqualCase("image"))
                    {
                        column.Length = 0;
                    }
                    else
                    {
                        column.Length = dr["Length"].ObjToInt();
                    }
                    column.DefaultValue = dr["DefaultValue"] + "";
                    column.Scale = dr["Scale"].ObjToInt();
                    column.DecimalDigits = dr["DecimalDigits"].ObjToInt();
                    column.ColumnDescription = dr["ColumnDescription"] + "";
                    column.IsPrimarykey = dr["IsPrimarykey"].ObjToBool();
                    column.IsIdentity = dr["IsIdentity"].ObjToBool();
                    column.IsNullable = dr["IsNullable"].ObjToBool();
                    result.Add(column);
                }
                // 仅非空结果入缓存（空 = 表不存在等场景，入缓存会毒化建表后的同进程查询）
                if (isCache && result.Count > 0)
                    metaCache.Add(cacheKey, result);
                return result;
            }
            return base.GetColumnInfosByTableName(tableName, isCache);
        }

        public override List<DbTableInfo> GetTableInfoList(bool isCache = true)
        {
            var mode = Mode;
            if (mode != null && mode.IsBabelfish)
            {
                // sys.tables 视图谓词不下推、每次全量物化（大 catalog 库秒级）。对齐 base 范式（模板 SQL + GetListOrCache）：isCache=true 进程级缓存、false 直发不读不写（库内 post-DDL 内部序列均显式传 false）。空列表不落缓存（同 GetColumnInfosByTableName），防「空库查列表→建表→同进程仍空」毒化；非空列表的陈旧与 base 全族一致（无 DDL 失效钩子，isCache=false 是逃生门）。
                string sql = "SELECT CAST(t.name as nvarchar(128)) as Name, CAST(ep.value as nvarchar(2000)) as Description " +
                    "FROM sys.tables t " +
                    "LEFT JOIN sys.extended_properties ep ON ep.major_id = t.object_id AND ep.minor_id = 0 AND ep.name = N'MS_Description' " +
                    "ORDER BY t.name";
                string cacheKey = null;
                ICacheService metaCache = null;
                if (isCache)
                {
                    cacheKey = GetCacheKey("DbMaintenanceProvider.GetTableInfoList" + this.Context.CurrentConnectionConfig.ConfigId);
                    metaCache = this.Context.Utilities.GetReflectionInoCacheInstance();
                    if (metaCache.ContainsKey<List<DbTableInfo>>(cacheKey))
                        return metaCache.Get<List<DbTableInfo>>(cacheKey);
                }
                var result = new List<DbTableInfo>();
                foreach (System.Data.DataRow dr in this.Context.Ado.GetDataTable(sql).Rows)
                    result.Add(new DbTableInfo { Name = dr["Name"] + "", Description = dr["Description"] + "" });
                foreach (var item in result)
                    item.DbObjectType = DbObjectType.Table;
                if (isCache && result.Count > 0)
                    metaCache.Add(cacheKey, result);
                return result;
            }
            return base.GetTableInfoList(isCache);
        }

        public override List<DbTableInfo> GetViewInfoList(bool isCache = true)
        {
            var mode = Mode;
            if (mode != null && mode.IsBabelfish)
            {
                // 不能委托 MSSQLDbMaintenance：其 GetViewInfoListSql 的 sysobjects.Name 是 sysname 内建别名类型列，Vastbase TDS 驱动（Npgsql 8 派生）读列抛 "Reading as 'System.Object' is not supported for fields having DataTypeName 'sys.sysname'"。CAST 归一 nvarchar 规避；isCache 语义同 GetTableInfoList；空列表不落缓存（防空库→建视图→同进程重查毒化）。
                string sql = "SELECT CAST(s.name as nvarchar(128)) as Name, Convert(varchar(max), tbp.value) as Description " +
                    "FROM sysobjects s " +
                    "LEFT JOIN sys.extended_properties tbp ON s.id = tbp.major_id AND tbp.minor_id = 0 " +
                    "AND (tbp.Name = N'MS_Description' OR tbp.Name IS NULL) " +
                    "WHERE s.xtype IN('V')";
                string cacheKey = null;
                ICacheService metaCache = null;
                if (isCache)
                {
                    cacheKey = GetCacheKey("DbMaintenanceProvider.GetViewInfoList" + this.Context.CurrentConnectionConfig.ConfigId);
                    metaCache = this.Context.Utilities.GetReflectionInoCacheInstance();
                    if (metaCache.ContainsKey<List<DbTableInfo>>(cacheKey))
                        return metaCache.Get<List<DbTableInfo>>(cacheKey);
                }
                var result = new List<DbTableInfo>();
                foreach (System.Data.DataRow dr in this.Context.Ado.GetDataTable(sql).Rows)
                {
                    result.Add(new DbTableInfo { Name = dr["Name"] + "", Description = dr["Description"] + "" });
                }
                foreach (var item in result)
                {
                    item.DbObjectType = DbObjectType.View;
                }
                if (isCache && result.Count > 0)
                    metaCache.Add(cacheKey, result);
                return result;
            }
            return base.GetViewInfoList(isCache);
        }

        public override bool TruncateTable(string tableName)
        {
            var mode = Mode;
            if (mode != null && mode.IsBabelfish) return mode.MSSQLDbMaintenance.TruncateTable(TrimTrailingSpace(tableName));
            return base.TruncateTable(tableName);
        }

        public override bool UpdateColumn(string tableName, DbColumnInfo column)
        {
            var mode = Mode;
            if (mode != null && mode.IsBabelfish)
            {
                mode.NormalizeColumn(column);
                return mode.MSSQLDbMaintenance.UpdateColumn(TrimTrailingSpace(tableName), column);
            }
            return base.UpdateColumn(tableName, column);
        }

        public override bool AddColumn(string tableName, DbColumnInfo column)
        {
            var mode = Mode;
            if (mode != null && mode.IsBabelfish)
            {
                mode.NormalizeColumn(column);
                if (column.IsNullable == false && string.IsNullOrEmpty(column.DefaultValue))
                {
                    column.DefaultValue = GetDefaultForType(column);
                }
                return mode.MSSQLDbMaintenance.AddColumn(TrimTrailingSpace(tableName), column);
            }
            return base.AddColumn(tableName, column);
        }

        private static string GetDefaultForType(DbColumnInfo column)
        {
            var dt = column.DataType?.ToLower() ?? "";
            if (dt == "datetime" || dt == "datetime2" || dt == "date" || dt == "smalldatetime")
                return "Convert(datetime, '1900-01-01')";
            if (dt == "int" || dt == "bigint" || dt == "smallint" || dt == "tinyint" || dt == "money" || dt == "smallmoney" || dt == "decimal" || dt == "numeric" || dt == "float" || dt == "real")
                return "0";
            if (dt == "bit")
                return "0";
            if (dt == "uniqueidentifier")
                return "Convert(uniqueidentifier, '00000000-0000-0000-0000-000000000000')";
            if (dt == "char" || dt == "varchar" || dt == "nvarchar" || dt == "nchar" || dt == "text" || dt == "ntext")
                return "''";
            return null;
        }

        public override bool CreateTable(string tableName, List<DbColumnInfo> columns, bool isCreatePrimaryKey = true)
        {
            var mode = Mode;
            if (mode != null && mode.IsBabelfish)
            {
                foreach (var col in columns)
                    mode.NormalizeColumn(col);
                return mode.MSSQLDbMaintenance.CreateTable(TrimTrailingSpace(tableName), columns, isCreatePrimaryKey);
            }
            return base.CreateTable(tableName, columns, isCreatePrimaryKey);
        }

        public override bool DropColumn(string tableName, string columnName)
        {
            var mode = Mode;
            if (mode != null && mode.IsBabelfish) return mode.MSSQLDbMaintenance.DropColumn(TrimTrailingSpace(tableName), columnName);
            return base.DropColumn(tableName, columnName);
        }

        public override bool AddPrimaryKey(string tableName, string columnName)
        {
            var mode = Mode;
            if (mode != null && mode.IsBabelfish) return mode.MSSQLDbMaintenance.AddPrimaryKey(TrimTrailingSpace(tableName), columnName);
            return base.AddPrimaryKey(tableName, columnName);
        }

        public override bool CreateIndex(string tableName, string[] columnNames, bool isUnique = false)
        {
            var mode = Mode;
            if (mode != null && mode.IsBabelfish) return mode.MSSQLDbMaintenance.CreateIndex(TrimTrailingSpace(tableName), columnNames, isUnique);
            return base.CreateIndex(tableName, columnNames, isUnique);
        }

        public override bool IsAnyIndex(string indexName)
        {
            var mode = Mode;
            if (mode != null && mode.IsBabelfish) return mode.MSSQLDbMaintenance.IsAnyIndex(indexName);
            return base.IsAnyIndex(indexName);
        }

        public override bool RenameTable(string tableName, string newTableName)
        {
            var mode = Mode;
            if (mode != null && mode.IsBabelfish) return mode.MSSQLDbMaintenance.RenameTable(TrimTrailingSpace(tableName), TrimTrailingSpace(newTableName));
            return base.RenameTable(tableName, newTableName);
        }

        public override bool RenameColumn(string tableName, string columnName, string newColumnName)
        {
            var mode = Mode;
            if (mode != null && mode.IsBabelfish) return mode.MSSQLDbMaintenance.RenameColumn(TrimTrailingSpace(tableName), columnName, newColumnName);
            return base.RenameColumn(tableName, columnName, newColumnName);
        }

        public override bool AddDefaultValue(string tableName, string columnName, string defaultValue)
        {
            var mode = Mode;
            if (mode != null && mode.IsBabelfish)
            {
                if (defaultValue != null && defaultValue.StartsWith("'") && defaultValue.EndsWith("'"))
                    defaultValue = defaultValue.Substring(1, defaultValue.Length - 2);
                // current_timestamp / current_date 去引号直发裸函数（对齐 PG 侧 PostgreSQLDbMaintenance.AddDefaultValue 特例语义）：SqlServer 模板恒加引号（除非值含 "()"），current_timestamp 无括号会被包成 ADD DEFAULT 'current_timestamp' → Babelfish（PG 内核）抛 invalid input syntax for type datetime。共享 SqlServer provider 不改动，此处内联其去引号形态，上游模板变化需同步。返回值语义同委托路径（ExecuteCommand 抛错即失败，成功恒 true）。
                if (defaultValue.EqualCase("current_timestamp") || defaultValue.EqualCase("current_date"))
                {
                    string sql = string.Format("alter table {0} ADD DEFAULT {2} FOR {1}",
                        this.SqlBuilder.GetTranslationTableName(TrimTrailingSpace(tableName)),
                        this.SqlBuilder.GetTranslationColumnName(columnName),
                        defaultValue);
                    this.Context.Ado.ExecuteCommand(sql);
                    return true;
                }
                return mode.MSSQLDbMaintenance.AddDefaultValue(TrimTrailingSpace(tableName), columnName, defaultValue);
            }
            return base.AddDefaultValue(tableName, columnName, defaultValue);
        }

        public override bool CreateDatabase(string databaseName, string databaseDirectory = null)
        {
            var mode = Mode;
            if (mode != null && mode.IsBabelfish) return mode.MSSQLDbMaintenance.CreateDatabase(databaseName, databaseDirectory);
            return base.CreateDatabase(databaseName, databaseDirectory);
        }

        // ========== Remarks ==========
        // Babelfish：sp_add/dropextendedproperty 须用 level0type=N'SCHEMA'（N'user' 被 Babelfish 拒）；不能委托 MSSQLDbMaintenance（其 SQL 硬编码 N'user',N'dbo'）。PG 模式走 base（PostgreSQL COMMENT ON）。实体可能在非 dbo schema（如 SugarTable("abp.Foo")），schema 须传入 sp_*extendedproperty 与 OBJECT_ID，否则只在 dbo 找而漏表。

        public override bool IsAnyTableRemark(string tableName)
        {
            var mode = Mode;
            if (mode != null && mode.IsBabelfish)
            {
                SplitRemarkName(tableName, out var schema, out var table);
                return this.Context.Ado.GetInt(
                    $"SELECT COUNT(*) FROM sys.extended_properties WHERE major_id = OBJECT_ID(N'{schema}.{table}') AND minor_id = 0 AND name = N'MS_Description'") > 0;
            }
            return base.IsAnyTableRemark(tableName);
        }

        public override bool IsAnyColumnRemark(string columnName, string tableName)
        {
            var mode = Mode;
            if (mode != null && mode.IsBabelfish)
            {
                SplitRemarkName(tableName, out var schema, out var table);
                var c = columnName.Trim().TrimStart('[').TrimEnd(']');
                // 列名 → minor_id 定位用 COLUMNPROPERTY 函数（内部走索引），不再 INNER JOIN sys.columns 按 col.name 过滤——sys.columns 视图无谓词下推、全量物化，大库下 JOIN 形态秒级。COLUMNPROPERTY 对不存在列返 NULL → 无行命中，与旧形态等价。
                return this.Context.Ado.GetInt(
                    $@"SELECT COUNT(*) FROM sys.extended_properties ep
                    WHERE ep.major_id = OBJECT_ID(N'{schema}.{table}')
                    AND ep.minor_id = COLUMNPROPERTY(OBJECT_ID(N'{schema}.{table}'), N'{c}', 'ColumnId')
                    AND ep.name = N'MS_Description'") > 0;
            }
            return base.IsAnyColumnRemark(columnName, tableName);
        }

        public override bool DeleteTableRemark(string tableName)
        {
            var mode = Mode;
            if (mode != null && mode.IsBabelfish)
            {
                SplitRemarkName(tableName, out var schema, out var table);
                this.Context.Ado.ExecuteCommand(
                    $"EXEC sp_dropextendedproperty N'MS_Description', N'SCHEMA', N'{schema}', N'table', N'{table}'");
                return true;
            }
            return base.DeleteTableRemark(tableName);
        }

        public override bool DeleteColumnRemark(string columnName, string tableName)
        {
            var mode = Mode;
            if (mode != null && mode.IsBabelfish)
            {
                SplitRemarkName(tableName, out var schema, out var table);
                var c = columnName.Trim().TrimStart('[').TrimEnd(']');
                this.Context.Ado.ExecuteCommand(
                    $"EXEC sp_dropextendedproperty N'MS_Description', N'SCHEMA', N'{schema}', N'table', N'{table}', N'column', N'{c}'");
                return true;
            }
            return base.DeleteColumnRemark(columnName, tableName);
        }

        public override bool AddTableRemark(string tableName, string tableDescription)
        {
            var mode = Mode;
            if (mode != null && mode.IsBabelfish)
            {
                SplitRemarkName(tableName, out var schema, out var table);
                // Plain sp_addextendedproperty — matches SqlServer provider contract (Add = add; IsAny/Delete are
                // separate). Base AddRemark orchestrates IsAny→delete→add; not compensated here. ⚠ Known Babelfish
                // limitation: CodeFirst's UseTran makes re-init (remark exists) raise "Property already exists"
                // (sp_drop not visible to sp_add across statements in one tran); this known limitation is surfaced rather than hidden.
                this.Context.Ado.ExecuteCommand(
                    $"EXEC sp_addextendedproperty N'MS_Description', N'{tableDescription}', N'SCHEMA', N'{schema}', N'table', N'{table}', NULL, NULL");
                return true;
            }
            return base.AddTableRemark(tableName, tableDescription);
        }

        public override bool AddColumnRemark(string columnName, string tableName, string columnDescription)
        {
            var mode = Mode;
            if (mode != null && mode.IsBabelfish)
            {
                SplitRemarkName(tableName, out var schema, out var table);
                var c = columnName.Trim().TrimStart('[').TrimEnd(']');
                this.Context.Ado.ExecuteCommand(
                    $"EXEC sp_addextendedproperty N'MS_Description', N'{columnDescription}', N'SCHEMA', N'{schema}', N'table', N'{table}', N'column', N'{c}'");
                return true;
            }
            return base.AddColumnRemark(columnName, tableName, columnDescription);
        }

        public override bool AddRemark(EntityInfo entity)
        {
            var mode = Mode;
            if (mode != null && mode.IsBabelfish)
            {
                // base（PostgreSQLDbMaintenance.AddRemark）是纯 add 编排（无 IsAny/diff 检查）：对 PG 的 COMMENT ON（天然幂等）成立，但对 Babelfish 的 sp_addextendedproperty（非幂等，已存在即抛 "Property already exists"）即 double-add，同表 re-init 必炸。故委托 MSSQLDbMaintenance：其编排为 IsAny→(diff? delete→add | skip)，且 IsAny/Delete/Add 经 Context.DbMaintenance 仍解析回本类的 sp_*extendedproperty 实现。
                return mode.MSSQLDbMaintenance.AddRemark(entity);
            }
            return base.AddRemark(entity);
        }

        public override bool IsAnyTable(string tableName, bool isCache = true)
        {
            var mode = Mode;
            if (mode != null && mode.IsBabelfish)
            {
                Check.Exception(string.IsNullOrEmpty(tableName), "IsAnyTable tableName is not null");
                // 委托 SqlServer 实现：schema-qualified 名走 OBJECT_ID、isCache=true 走 base 表清单缓存，schema-aware。唯一拦截 plain 名 + isCache=false：sys.objects WHERE name 谓词在 Babelfish 不下推，大库秒级且 CodeFirst InitTables 每表一次，改走索引的 OBJECT_ID(name,'U')。语义收窄：OBJECT_ID 按默认 schema 解析，Babelfish 用户表在 dbo，实际等价，且与 dotted 分支（本就 OBJECT_ID）一致化。
                if (!tableName.Contains(".") && !isCache)
                {
                    if (tableName.Contains(SqlBuilder.SqlTranslationLeft))
                        tableName = SqlBuilder.GetNoTranslationColumnName(tableName);
                    return this.Context.Ado.GetInt(
                        "SELECT CASE WHEN OBJECT_ID(N'" + tableName.ToSqlFilter() + "', N'U') IS NULL THEN 0 ELSE 1 END AS res") > 0;
                }
                return mode.MSSQLDbMaintenance.IsAnyTable(tableName, isCache);
            }
            return base.IsAnyTable(tableName, isCache);
        }


        public override bool DropTable(string tableName)
        {
            var mode = Mode;
            if (mode != null && mode.IsBabelfish) return mode.MSSQLDbMaintenance.DropTable(TrimTrailingSpace(tableName));
            return base.DropTable(tableName);
        }
    }

    // Babelfish DDL 经 MSSQLDbMaintenance 委托到本类。byte[] 由 CodeFirst 用
    // Length=-1 表示 max；仅 varbinary 覆写尺寸渲染，避免上游空尺寸落成
    // varbinary(1) 并静默截断，其余类型仍走 SqlServerDbMaintenance 原实现。
    internal sealed class VastbaseV5BabelfishDbMaintenance : SqlServerDbMaintenance
    {
        protected override string GetSize(DbColumnInfo item)
        {
            if (item != null && item.Length == -1
                && item.DataType != null && item.DataType.EqualCase("varbinary"))
                return "(max)";
            return base.GetSize(item);
        }
    }
}