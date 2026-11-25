using System;
using System.Collections.Generic;

namespace SqlSugar
{
    internal class VastbaseV5Mode
    {
        private readonly bool _isBabelfish;
        private readonly SqlSugarProvider _context;

        private ISqlBuilder _mssqlBuilder;
        private SqlSugarProvider _mssqlBuilderContext;
        private IDbBind _mssqlDbBind;
        private SqlSugarProvider _mssqlDbBindContext;
        private IDbMaintenance _mssqlDbMaintenance;
        private SqlSugarProvider _mssqlDbMaintenanceContext;

        private static readonly Dictionary<string, string> _typeMap = new Dictionary<string, string>
        {
            ["timestamp without time zone"] = "datetime",
            ["time without time zone"] = "time",
            ["character varying"] = "varchar",
            ["integer"] = "int",
            ["boolean"] = "bit",
            ["bytea"] = "varbinary(max)",
        };

        private VastbaseV5Mode(SqlSugarProvider context)
        {
            _context = context;
            _isBabelfish = VastbaseV5Helper.IsBabelfish(context);
        }

        public static VastbaseV5Mode From(SqlSugarProvider context)
        {
            if (context == null) return null;
            var temps = context.TempItems;
            if (temps != null && temps.TryGetValue("VastbaseV5Mode", out var cached))
                return (VastbaseV5Mode)cached;
            var mode = new VastbaseV5Mode(context);
            if (temps != null) temps["VastbaseV5Mode"] = mode;
            return mode;
        }

        // 模式敏感访问入口：Context 未挂载（null）即抛，消灭「未解析 → 静默 PG 回退」。From 保持宽容（供 ExpressionContext 等合法 null 路径）；各 Vastbase* 类的 Mode 属性一律走 Require——Context 应先于模式使用完成赋值，触发即调用时序错误，应响亮失败。
        public static VastbaseV5Mode Require(SqlSugarProvider context)
        {
            var mode = From(context);
            if (mode == null)
                Check.Exception(true, "Vastbase 模式未解析：Context 未挂载，无法判定 PG/Babelfish 模式。"
                    + "模式应在 Context/SugarContext 赋值后解析，未挂 Context 即触达模式敏感路径是调用时序错误。");
            return mode;
        }

        // ========== Mode flag ==========

        public bool IsBabelfish => _isBabelfish;

        // ========== Quote Style ==========

        public string SqlTranslationLeft => _isBabelfish ? "[" : "\"";
        public string SqlTranslationRight => _isBabelfish ? "]" : "\"";

        // ========== Flags ==========

        public bool AutoToLower => !_isBabelfish;

        // ========== Type Mapping ==========

        public string NormalizeTypeName(string pgType)
        {
            if (!_isBabelfish) return pgType;
            // Babelfish TDS 无数组类型：DDL 建表与读写均被拒（SELECT int8[] 列报 "data type bigint[] is not supported yet"）。数组实体列物理降级为 text（值按 PG 数组字面量文本存取，见 VastbaseV5Provider）；匹配含 "*[]" 与带空格的 "int8 []" 变体。
            if (pgType != null && pgType.TrimEnd().EndsWith("[]"))
                return "text";
            return _typeMap.TryGetValue(pgType, out var mapped) ? mapped : pgType;
        }

        public void NormalizeColumn(DbColumnInfo column)
        {
            if (!_isBabelfish || column == null) return;
            column.DataType = NormalizeTypeName(column.DataType);
        }

        // ========== SQL Transforms ==========
        // 6 步事后重写链（SyncGroupByParams/FixGroupBy/NormalizeExistsSelect/ConvertPagination/
        // NormalizeCastTypes/NormalizeDatePart）已收拢为 VastbaseV5BabelfishRewritePipeline
        //（步骤顺序与形状漂移断言在 pipeline 内声明）。此处仅保留列元数据
        // 感知的 CAST 精度拼装——字符串级重写拿不到列精度，故 GetCastDbType 不走 pipeline。

        // 批量 UPDATE VALUES 的 CAST 目标类型（列元数据感知，pipeline 字符串级正则无此信息）。Babelfish 下 CAST(x AS numeric/decimal) 无精度按 T-SQL 默认 decimal(18,0) 解析，小数位静默舍入（18.7545 会落库成 19.0000）。列精度可得时拼显式 numeric(p,s)；PG 裸 numeric 列（Length=0）无精度可拼，保持原样。
        public string GetCastDbType(DbColumnInfo columnInfo, string dbType)
        {
            if (!_isBabelfish || columnInfo == null || columnInfo.Length <= 0)
                return dbType;
            if (string.Equals(dbType, "numeric", StringComparison.OrdinalIgnoreCase)
                || string.Equals(dbType, "decimal", StringComparison.OrdinalIgnoreCase))
                return string.Format("{0}({1},{2})", dbType, columnInfo.Length, columnInfo.DecimalDigits);
            return dbType;
        }

        // ========== Insert Template ==========

        public string InsertSqlTemplate => _isBabelfish && true
            ? @"INSERT INTO {0}
           ({1})
     VALUES
           ({2}) ;SELECT SCOPE_IDENTITY();"
            : null;

        public Func<string, string, string> ConvertInsertReturnIdFunc
        {
            get
            {
                if (!_isBabelfish) return null;
                return (name, sql) =>
                {
                    sql = sql.Replace("select SCOPE_IDENTITY();", "")
                             .Replace("SELECT SCOPE_IDENTITY();", "");
                    if (sql.Contains(")\r\n SELECT"))
                        sql = sql.Replace(")\r\n SELECT", string.Format(")\r\n OUTPUT INSERTED.{0} as {0}  \r\nSELECT", name));
                    else if (sql.Contains(") VALUES "))
                        sql = sql.Replace(") VALUES ", string.Format(") OUTPUT INSERTED.{0} as {0}  \r\n VALUES ", name));
                    else if (sql.Contains(")\r\n VALUES"))
                        sql = sql.Replace(")\r\n VALUES", string.Format(")\r\n OUTPUT INSERTED.{0} as {0}  \r\n VALUES", name));
                    return sql;
                };
            }
        }

        // ========== SqlServer Delegates ==========

        public ISqlBuilder MSSQLBuilder
        {
            get
            {
                if (_mssqlBuilder == null || !ReferenceEquals(_mssqlBuilderContext, _context))
                {
                    _mssqlBuilder = new SqlServerBuilder();
                    _mssqlBuilder.Context = _context;
                    _mssqlBuilderContext = _context;
                }
                return _mssqlBuilder;
            }
        }

        public IDbBind MSSQLDbBind
        {
            get
            {
                if (_mssqlDbBind == null || !ReferenceEquals(_mssqlDbBindContext, _context))
                {
                    _mssqlDbBind = new SqlServerDbBind();
                    _mssqlDbBind.Context = _context;
                    _mssqlDbBindContext = _context;
                }
                return _mssqlDbBind;
            }
        }

        public IDbMaintenance MSSQLDbMaintenance
        {
            get
            {
                if (_mssqlDbMaintenance == null || !ReferenceEquals(_mssqlDbMaintenanceContext, _context))
                {
                    _mssqlDbMaintenance = new VastbaseV5BabelfishDbMaintenance();
                    _mssqlDbMaintenance.Context = _context;
                    _mssqlDbMaintenanceContext = _context;
                }
                return _mssqlDbMaintenance;
            }
        }
    }

    internal static class VastbaseV5Helper
    {
        public static bool IsBabelfish(SqlSugarProvider context)
        {
            return IsBabelfish(context?.CurrentConnectionConfig);
        }

        // ConnectionConfig 直查版：供静态工具路径（UtilMethods.GetSqlString 等无 Provider 上下文）复用，
        // 判定逻辑与 Provider 版保持同一来源（MoreSettings.VastbaseCompatibility / 连接串标记）。
        public static bool IsBabelfish(ConnectionConfig config)
        {
            if (config?.MoreSettings?.VastbaseCompatibility
                == VastbaseCompatibilityMode.Babelfish)
                return true;

            return IsBabelfishConnectionString(config?.ConnectionString);
        }

        // 驱动端连接串解析（DbConnectionStringBuilder 家族）对大小写、等号两侧空白、引号宽容——
        // 实测 SERVER COMPATIBILITY MODE=BABELFISH、"Server Compatibility Mode = Babelfish" 均正常
        // 建立 TDS 连接。判定必须与驱动同口径：精确子串匹配会把变体写法误判为 PG，DateTime 参数
        // 随即走 PG 归一化被打成 DbType.Object，TDS 绑定期驱动无该映射抛
        // ArgumentOutOfRangeException（参数名 dbType）。连接串解析失败（驱动同样无法连接）按 PG 处理。
        internal static bool IsBabelfishConnectionString(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString)) return false;
            try
            {
                var builder = new System.Data.Common.DbConnectionStringBuilder();
                builder.ConnectionString = connectionString;
                foreach (string key in builder.Keys)
                    if (string.Equals(key, "Server Compatibility Mode", System.StringComparison.OrdinalIgnoreCase)
                        && string.Equals(System.Convert.ToString(builder[key]), "Babelfish", System.StringComparison.OrdinalIgnoreCase))
                        return true;
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}