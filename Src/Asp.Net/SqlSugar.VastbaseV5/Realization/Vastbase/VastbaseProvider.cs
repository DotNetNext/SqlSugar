using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Xml.Linq;
using Vastbase;
using VastbaseTypes;

namespace SqlSugar
{
    public partial class VastbaseV5Provider : AdoProvider
    {
        private VastbaseV5Mode Mode => VastbaseV5Mode.Require(this.Context);

        public VastbaseV5Provider()
        {
            if (StaticConfig.AppContext_ConvertInfinityDateTime == false)
            {
                AppContext.SetSwitch("Vastbase.EnableLegacyTimestampBehavior", true);
                AppContext.SetSwitch("Vastbase.DisableDateTimeInfinityConversions", true);
            }
        }

        public override IDbConnection Connection
        {
            get
            {
                if (base._DbConnection == null)
                {
                    try
                    {
                        var connectionString = base.Context.CurrentConnectionConfig.ConnectionString;
                        base._DbConnection = new VastbaseConnection(connectionString);
                    }
                    catch (Exception ex)
                    {
                        Check.Exception(true, ErrorMessage.ConnnectionOpen, ex.Message);
                    }
                }
                return base._DbConnection;
            }
            set
            {
                base._DbConnection = value;
            }
        }

        public override void Open()
        {
            base.Open();
            var mode = Mode;
            if (mode != null && mode.IsBabelfish)
            {
                var config = this.Context.CurrentConnectionConfig;
                if (config.MoreSettings == null)
                    config.MoreSettings = new ConnMoreSettings();
                try
                {
                    using (var cmd = this.Connection.CreateCommand())
                    {
                        cmd.CommandText = "SET babelfishpg_tsql.escape_hatch_rowversion = 'ignore'";
                        cmd.ExecuteNonQuery();
                    }
                }
                catch { /* escape hatch may already be set or not permitted */ }
            }
        }

        public override int ExecuteCommand(string sql, params SugarParameter[] parameters)
        {
            var mode = Mode;
            if (mode != null && mode.IsBabelfish)
            {
                sql = sql.Replace("timestamp without time zone", mode.NormalizeTypeName("timestamp without time zone"))
                         .Replace("time without time zone", mode.NormalizeTypeName("time without time zone"));
            }
            return base.ExecuteCommand(sql, parameters);
        }

        public override async System.Threading.Tasks.Task<int> ExecuteCommandAsync(string sql, params SugarParameter[] parameters)
        {
            var mode = Mode;
            if (mode != null && mode.IsBabelfish)
            {
                sql = sql.Replace("timestamp without time zone", mode.NormalizeTypeName("timestamp without time zone"))
                         .Replace("time without time zone", mode.NormalizeTypeName("time without time zone"));
            }
            return await base.ExecuteCommandAsync(sql, parameters);
        }

        public override void BeginTran(string transactionName)
        {
            this.BeginTran();
        }

        public override void BeginTran(IsolationLevel iso, string transactionName)
        {
            this.BeginTran(iso);
        }

        public override void RollbackTran()
        {
            if (this.Transaction != null)
            {
                if (!IsTransactionCompleted(this.Transaction))
                    this.Transaction.Rollback();
                this.Transaction = null;
                if (this.Context.CurrentConnectionConfig.IsAutoCloseConnection) this.Close();
            }
        }

        public override async System.Threading.Tasks.Task RollbackTranAsync()
        {
            if (this.Transaction != null)
            {
                if (!IsTransactionCompleted(this.Transaction))
                    await (this.Transaction as System.Data.Common.DbTransaction).RollbackAsync();
                this.Transaction = null;
                if (this.Context.CurrentConnectionConfig.IsAutoCloseConnection) await this.CloseAsync();
            }
        }

        public override void CommitTran()
        {
            if (this.Transaction != null)
            {
                if (!IsTransactionCompleted(this.Transaction))
                    this.Transaction.Commit();
                this.Transaction = null;
                if (this.Context.CurrentConnectionConfig.IsAutoCloseConnection) this.Close();
            }
        }

        public override async System.Threading.Tasks.Task CommitTranAsync()
        {
            if (this.Transaction != null)
            {
                if (!IsTransactionCompleted(this.Transaction))
                    await (this.Transaction as System.Data.Common.DbTransaction).CommitAsync();
                this.Transaction = null;
                if (this.Context.CurrentConnectionConfig.IsAutoCloseConnection) await this.CloseAsync();
            }
        }

        // Npgsql 8 语句失败即自动回滚并把事务标记为 completed；IsCompleted 为驱动 internal 属性，用缓存反射读取（NonPublic|Public 绑定，驱动调整可见性亦兼容）。
        private static readonly System.Reflection.PropertyInfo _s_isCompletedProp =
            typeof(VastbaseTransaction).GetProperty("IsCompleted",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

        private static bool IsTransactionCompleted(IDbTransaction tran)
        {
            if (tran is VastbaseTransaction vt && _s_isCompletedProp != null)
                return (bool)_s_isCompletedProp.GetValue(vt);
            return false;
        }

        public override Action<SqlSugarException> ErrorEvent => it =>
        {
            if (base.ErrorEvent != null)
            {
                base.ErrorEvent(it);
            }
            if (it.Message != null && it.Message.StartsWith("42883: function uuid_generate_v4() does not exist"))
            {
                Check.ExceptionEasy(it.Message, $"使用uuid_generate_v4()函数需要创建 CREATE EXTENSION IF NOT EXISTS pgcrypto;CREATE EXTENSION IF NOT EXISTS \"uuid-ossp\" ");
            }
        };

        public override IDataAdapter GetAdapter()
        {
            return new VastbaseDataAdapterWrapper();
        }

        public override DbCommand GetCommand(string sql, SugarParameter[] parameters)
        {
            return GetCommandCore(sql, parameters, out _);
        }

        private DbCommand GetCommandCore(string sql, SugarParameter[] parameters, out SpOutRewriteState spOutState)
        {
            spOutState = null;
            var mode = Mode;
            if (mode != null && mode.IsBabelfish)
            {
                sql = sql.Replace("timestamp without time zone", mode.NormalizeTypeName("timestamp without time zone"))
                         .Replace("time without time zone", mode.NormalizeTypeName("time without time zone"));
                // Babelfish 服务端对参数化 NULL 按 DbType 严格类型检查，目标列不匹配即拒（33557097 "expression is of type text"）；SQL Server 语义中 NULL 无类型可赋任意列。故 NULL 值参数内联为 NULL 字面量、不参与绑定，非 NULL 参数保持参数化。仅限文本 SQL：SP 路径 SQL 是过程名、无占位符可替换，剔除参数反致参数缺失。
                if (parameters.HasValue() && this.CommandType == System.Data.CommandType.Text)
                {
                    var nullPars = parameters
                        .Where(p => p.Direction != ParameterDirection.Output
                                 && p.Direction != ParameterDirection.ReturnValue
                                 && (p.Value == null || p.Value == DBNull.Value))
                        .ToArray();
                    if (nullPars.Length > 0)
                    {
                        // 只剔除确实在 SQL 中替换成功的参数（占位符不存在即保留——多余参数
                        // 驱动可容忍）。T-SQL 参数名大小写不敏感，替换用 IgnoreCase。
                        var replacedNames = new HashSet<string>();
                        foreach (var p in nullPars.OrderByDescending(x => (x.ParameterName ?? "").Length))
                        {
                            var pname = p.ParameterName;
                            if (string.IsNullOrEmpty(pname)) continue;
                            if (!pname.StartsWith("@")) pname = "@" + pname;
                            var newSql = System.Text.RegularExpressions.Regex.Replace(
                                sql, @"@?" + System.Text.RegularExpressions.Regex.Escape(pname) + @"\b", "NULL",
                                System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                            if (newSql != sql)
                            {
                                sql = newSql;
                                replacedNames.Add(p.ParameterName);
                            }
                        }
                        parameters = parameters.Where(p => p.Direction == ParameterDirection.Output
                            || p.Direction == ParameterDirection.ReturnValue
                            || !replacedNames.Contains(p.ParameterName)).ToArray();
                    }
                }
            }
            // Babelfish 兼容模式(TDS)：PG 协议绑定参数不可作 OUTPUT 目标，驱动 StoredProcedure 路径生成的 EXEC ... OUTPUT 被服务端拒（33557097 "argument is not writable"）。改写：输入参数保持参数化，输出参数改用 T-SQL 变量 + EXEC OUTPUT + SELECT 回读，由 GetDataSetAll 消费。
            var isSpOutRewrite = mode != null && mode.IsBabelfish
                && this.CommandType == System.Data.CommandType.StoredProcedure
                && parameters.HasValue()
                && System.Array.Exists(parameters, it => it.Direction.IsIn(ParameterDirection.Output, ParameterDirection.InputOutput));
            var binds = new List<VastbaseParameter>();
            var map = new List<KeyValuePair<string, SugarParameter>>();
            if (isSpOutRewrite)
            {
                var declares = new System.Text.StringBuilder();
                var execArgs = new System.Text.StringBuilder();
                var outSelects = new List<string>();
                bool first = true;
                int pos = 0, outIdx = 0;
                foreach (var p in parameters)
                {
                    if (p.Direction.IsIn(ParameterDirection.Output, ParameterDirection.InputOutput))
                    {
                        var varName = "@__spout" + outIdx;
                        var pname = "@" + p.ParameterName.TrimStart('@');
                        declares.Append("DECLARE ").Append(varName).Append(' ').Append(ToSpOutTsqlType(p)).AppendLine(";");
                        if (!first) execArgs.Append(", ");
                        execArgs.Append(pname).Append(" = ").Append(varName).Append(" OUTPUT");
                        outSelects.Add(varName + " as \"" + pname + "\"");
                        map.Add(new KeyValuePair<string, SugarParameter>(pname, p));
                        outIdx++;
                    }
                    else if (p.Direction != ParameterDirection.ReturnValue)
                    {
                        if (!first) execArgs.Append(", ");
                        execArgs.Append("@" + p.ParameterName.TrimStart('@')).Append(" = @").Append(pos);
                        var pp = (VastbaseParameter)ToIDbDataParameter(new[] { p })[0];
                        pp.ParameterName = "@" + pos;
                        binds.Add(pp);
                        pos++;
                    }
                    first = false;
                }
                if (outSelects.Count > 0)
                {
                    var originalSql = sql;
                    sql = declares.ToString() + "EXEC " + originalSql + " " + execArgs + ";\nSELECT " + string.Join(", ", outSelects) + ";";
                    spOutState = new SpOutRewriteState(originalSql, map, parameters);
                }
                else
                {
                    isSpOutRewrite = false;
                }
            }
            var sqlCommand = new VastbaseCommand(sql, (VastbaseConnection)this.Connection);
            sqlCommand.CommandType = isSpOutRewrite ? System.Data.CommandType.Text : this.CommandType;
            sqlCommand.CommandTimeout = this.CommandTimeOut;
            if (this.Transaction != null)
            {
                sqlCommand.Transaction = (VastbaseTransaction)this.Transaction;
            }
            if (isSpOutRewrite)
            {
                sqlCommand.Parameters.AddRange(binds.ToArray());
                // 输出参数不进绑定（T-SQL 变量），但填 OutputParameters 防 ExecuteAfter 空引用，
                // 其 DBNull 值随后被 GetDataSetAll 回读覆盖（无害）。
                if (this.OutputParameters == null) this.OutputParameters = new List<IDataParameter>();
                foreach (var p in parameters)
                {
                    if (p.Direction.IsIn(ParameterDirection.Output, ParameterDirection.InputOutput))
                    {
                        this.OutputParameters.RemoveAll(it => it.ParameterName == p.ParameterName);
                        this.OutputParameters.Add(new VastbaseParameter
                        {
                            ParameterName = p.ParameterName,
                            Direction = p.Direction,
                            Value = DBNull.Value
                        });
                    }
                }
            }
            else if (parameters.HasValue())
            {
                IDataParameter[] ipars = ToIDbDataParameter(parameters);
                sqlCommand.Parameters.AddRange((VastbaseParameter[])ipars);
            }
            CheckConnection();
            return sqlCommand;
        }

        private string ToSpOutTsqlType(SugarParameter p)
        {
            var size = p.Size;
            switch (p.DbType)
            {
                case System.Data.DbType.AnsiString:
                case System.Data.DbType.AnsiStringFixedLength:
                    return size > 8000 ? "varchar(max)" : (size > 0 ? "varchar(" + size + ")" : "varchar(4000)");
                case System.Data.DbType.String:
                case System.Data.DbType.StringFixedLength:
                    return size > 8000 ? "nvarchar(max)" : (size > 0 ? "nvarchar(" + size + ")" : "nvarchar(4000)");
                case System.Data.DbType.Int32: return "int";
                case System.Data.DbType.Int64: return "bigint";
                case System.Data.DbType.Int16: return "smallint";
                case System.Data.DbType.Byte: return "tinyint";
                case System.Data.DbType.Boolean: return "bit";
                case System.Data.DbType.Decimal: return "decimal(38,10)";
                case System.Data.DbType.Double: return "float";
                case System.Data.DbType.Single: return "real";
                case System.Data.DbType.DateTime: return "datetime";
                case System.Data.DbType.DateTime2: return "datetime2";
                case System.Data.DbType.DateTimeOffset: return "datetimeoffset";
                case System.Data.DbType.Time: return "time";
                case System.Data.DbType.Guid: return "uniqueidentifier";
                case System.Data.DbType.Binary: return "varbinary(max)";
                default: return "varchar(4000)";
            }
        }

        public override DataSet GetDataSetAll(string sql, SugarParameter[] parameters)
        {
            // 复刻 AdoProvider.GetDataSetAll 主流程，唯一差异：GetCommandCore 产出 SpOutRewriteState 值对象并在本帧消费；base 流程若变更，此处须同步修改。
            try
            {
                InitParameters(ref sql, parameters);
                if (FormatSql != null && parameters != null && parameters.Length > 0)
                    sql = FormatSql(sql);
                if (this.Context.CurrentConnectionConfig?.SqlMiddle?.IsSqlMiddle == true)
                    return this.Context.CurrentConnectionConfig.SqlMiddle.GetDataSetAll(sql, parameters);
                SetConnectionStart(sql);
                if (this.ProcessingEventStartingSQL != null)
                    ExecuteProcessingSQL(ref sql, ref parameters);
                ExecuteBefore(sql, parameters);
                IDataAdapter dataAdapter = this.GetAdapter();
                DbCommand sqlCommand = GetCommandCore(sql, parameters, out var spOutState);
                this.SetCommandToAdapter(dataAdapter, sqlCommand);
                DataSet ds = new DataSet();
                dataAdapter.Fill(ds);
                if (this.IsClearParameters)
                    sqlCommand.Parameters.Clear();
                ExecuteAfter(sql, parameters);
                sqlCommand.Dispose();
                SpOutRewriteState.ApplySpOutReadBack(ds, sql, parameters, spOutState);
                return ds;
            }
            catch (Exception ex)
            {
                SugarCatch(ex, sql, parameters);
                if (ErrorEvent != null)
                    ExecuteErrorEvent(sql, parameters, ex);
                throw;
            }
            finally
            {
                CommandType = CommandType.Text;
                if (this.IsAutoClose()) this.Close();
                SetConnectionEnd(sql);
            }
        }

        public override void SetCommandToAdapter(IDataAdapter dataAdapter, DbCommand command)
        {
            ((VastbaseDataAdapterWrapper)dataAdapter).SelectCommand = (VastbaseCommand)command;
        }

        public override IDataParameter[] ToIDbDataParameter(params SugarParameter[] parameters)
        {
            if (parameters == null || parameters.Length == 0) return null;
            var mode = Mode;
            var result = new VastbaseParameter[parameters.Length];
            int index = 0;
            var isVarchar = this.Context.IsVarchar();
            foreach (var parameter in parameters)
            {
                if (parameter.DbType == System.Data.DbType.Int64 && parameter.Value?.Equals("Result%") == true)
                {
                    parameter.DbType = System.Data.DbType.AnsiString;
                }
                UNumber(parameter);
                if (parameter.Value == null) parameter.Value = DBNull.Value;
                if (parameter.Value is System.Data.SqlTypes.SqlDateTime && parameter.DbType == System.Data.DbType.AnsiString)
                {
                    parameter.DbType = System.Data.DbType.DateTime;
                    parameter.Value = DBNull.Value;
                }
                var isSqlVariantString = mode != null && mode.IsBabelfish
                    && parameter.CustomDbType is System.Data.SqlDbType
                    && parameter.Value is string;
                // IsJson + PG 模式：不拷贝 DbType（保持驱动未设态）→ 驱动按
                // VastbaseDbType=Json 绑定。已实测：DbType 未设时 Json 生效；
                // 任一显式值（String/AnsiString/Object）都覆盖 Json → 42804 text→json。
                // 拷贝跳过仅限 PG 模式——Babelfish 依赖 DbType=String 的 nvarchar 语义。
                var isJsonPg = parameter.IsJson && (mode == null || !mode.IsBabelfish);
                // Babelfish TDS：驱动 ToSqlDbType 对 Object/SByte 无映射，绑定期抛
                // ArgumentOutOfRangeException（参数名 dbType）。跳过 DbType 拷贝 →
                // 参数保持驱动未设态，按 CLR 值类型推断（未设态路径已实证可用）。
                var isUnmappableDbType = mode != null && mode.IsBabelfish
                    && (parameter.DbType == System.Data.DbType.Object || parameter.DbType == System.Data.DbType.SByte);
                var xe = parameter.Value as XElement;
                // Babelfish 下普通 string 值参数不拷贝 DbType（保持驱动未设态，按 CLR 值推断）：DbType.String/AnsiString 在 TDS 绑定映射为 pg text，而 text→sys.datetime 等 T-SQL 类型族无 cast 路径，插 datetime/smalldatetime 列即拒（33557097 "expression is of type text"）；未设态走 sys 变体类型，T-SQL 隐式转换可用（空串落 1900-01-01，与 SQL Server 一致）。IsJson/sql_variant 有专属 DbType 分支，不在其列。
                var isBabelfishPlainString = mode != null && mode.IsBabelfish
                    && parameter.Value is string && !parameter.IsJson;
                // PG 模式未设类型的 NULL 参数不拷贝 DbType（未设态 → unknown OID，服务端按目标列推断）。SugarParameter.DbType 默认值即 AnsiString（枚举零值），「未设」与「显式 AnsiString」不可区分，本豁免两者都按未设态处理——若改 SugarParameter 默认值须同步复核本豁免。若拷贝，NULL 按 text OID 绑定，numeric/timestamp 等非文本列拒收（42804 "expression is of type text"）。仅限非输出方向（输出参数 DbType 承载返回类型语义）；显式非 AnsiString 类型（如 Decimal）的 NULL 保持拷贝。Babelfish 分支不受影响（NULL 走 GetCommandCore 字面量内联）。
                var isPgNullValue = (mode == null || !mode.IsBabelfish)
                    && parameter.DbType == System.Data.DbType.AnsiString
                    && parameter.Value is DBNull
                    && parameter.Direction != ParameterDirection.Output
                    && parameter.Direction != ParameterDirection.ReturnValue;
                var sqlParameter = new VastbaseParameter();
                sqlParameter.ParameterName = parameter.ParameterName;
                sqlParameter.Size = parameter.Size;
                sqlParameter.Value = xe != null ? xe.ToString() : parameter.Value;
                if (!isSqlVariantString && xe == null && !isJsonPg && !isUnmappableDbType && !isBabelfishPlainString && !isPgNullValue)
                    sqlParameter.DbType = parameter.DbType;
                if (parameter.Value is byte[] bytes && sqlParameter.Size <= 0)
                    sqlParameter.Size = bytes.Length;
                if (mode != null && mode.IsBabelfish)
                {
                    if (sqlParameter.Value is DateTime dt)
                    {
                        if (dt < System.Data.SqlTypes.SqlDateTime.MinValue.Value)
                            sqlParameter.Value = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
                        else if (dt > System.Data.SqlTypes.SqlDateTime.MaxValue.Value)
                            sqlParameter.Value = System.Data.SqlTypes.SqlDateTime.MaxValue.Value;
                        else
                            sqlParameter.Value = DateTime.SpecifyKind(dt, DateTimeKind.Unspecified);
                    }
                    else if (sqlParameter.Value is DateTimeOffset dto)
                    {
                        var dt2 = dto.DateTime;
                        if (dt2 < System.Data.SqlTypes.SqlDateTime.MinValue.Value)
                            sqlParameter.Value = new DateTimeOffset(System.Data.SqlTypes.SqlDateTime.MinValue.Value, dto.Offset);
                        else if (dt2 > System.Data.SqlTypes.SqlDateTime.MaxValue.Value)
                            sqlParameter.Value = new DateTimeOffset(System.Data.SqlTypes.SqlDateTime.MaxValue.Value, dto.Offset);
                        else
                            sqlParameter.Value = new DateTimeOffset(DateTime.SpecifyKind(dt2, DateTimeKind.Unspecified), dto.Offset);
                    }
                    else if (sqlParameter.Value is Array arr && !(sqlParameter.Value is byte[]))
                    {
                        // Babelfish TDS 无数组类型（DDL 与读写均拒）：数组参数按 PG 数组字面量文本序列化（"{1,2}"，配 DDL 降级 text 列）并以字符串参数发送。原样发送时驱动 TDS 序列化为无花括号 CSV，多元素报 "INSERT has more expressions than target columns"；IsArray 分支设 DbType=Object 又因 ToSqlDbType 无映射抛 ArgumentOutOfRangeException。
                        sqlParameter.Value = FormatPgArrayLiteral(arr);
                        sqlParameter.DbType = System.Data.DbType.String;
                    }
                }
                sqlParameter.Direction = parameter.Direction;
                if (sqlParameter.Direction == 0)
                {
                    sqlParameter.Direction = ParameterDirection.Input;
                }
                result[index] = sqlParameter;
                if (sqlParameter.Direction.IsIn(ParameterDirection.Output, ParameterDirection.InputOutput, ParameterDirection.ReturnValue))
                {
                    if (this.OutputParameters == null) this.OutputParameters = new List<IDataParameter>();
                    this.OutputParameters.RemoveAll(it => it.ParameterName == sqlParameter.ParameterName);
                    this.OutputParameters.Add(sqlParameter);
                }
                if (isVarchar && sqlParameter.DbType == System.Data.DbType.String)
                {
                    sqlParameter.DbType = System.Data.DbType.AnsiString;
                }
                else if (sqlParameter.Value is DateTime && sqlParameter.DbType == System.Data.DbType.AnsiString)
                {
                    sqlParameter.DbType = System.Data.DbType.DateTime;
                }
                ++index;
                if (parameter.IsJson)
                {
                    if (mode != null && mode.IsBabelfish)
                    {
                        // Babelfish T-SQL mode: JSON stored as nvarchar(max), no native JSON type
                        sqlParameter.DbType = System.Data.DbType.String;
                    }
                    else
                    {
                        // PG 模式：VastbaseDbType=Json 必须绑定 json 类型——DbType 任一显式值（String/AnsiString/Object）都会覆盖 VastbaseDbType → 42804 text→json，仅 DbType 未设时 Json 生效。故 IsJson 参数不拷贝 DbType，令驱动按 VastbaseDbType=Json 绑定，否则 json 列全部写入路径 42804。
                        sqlParameter.VastbaseDbType = VastbaseTypes.VastbaseDbType.Json;
                    }
                }
                if (parameter.IsArray && !parameter.IsJson && (mode == null || !mode.IsBabelfish))
                {
                    sqlParameter.DbType = System.Data.DbType.Object;
                    Array(parameter, sqlParameter);
                }
                if (mode == null || !mode.IsBabelfish)
                {
                    // PG 模式 DateTime 归一化：驱动 netstandard2.0 构建把 DbType.DateTime 解析为 timestamptz（要求 Kind=Utc），DateTime 参数被拒："Cannot write DateTime with Kind=X to PostgreSQL type 'timestamp with time zone'"。归一化：值归 Unspecified + 显式 VastbaseDbType.Timestamp + DbType 清零（DbType 优先于 VastbaseDbType，残留会按 timestamptz 覆盖显式声明）。netstandard2.0 下 Timestamp 无 handler，参数按 unknown 发送、服务端按目标列推断（42P18 仅裸 SELECT 触发）；net8.0 走显式 timestamp。DateTimeOffset（timestamptz 列）路径不受影响。
                    if (sqlParameter.Value is DateTime pgDateTime)
                    {
                        if (pgDateTime < System.Data.SqlTypes.SqlDateTime.MinValue.Value)
                            sqlParameter.Value = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
                        else if (pgDateTime > System.Data.SqlTypes.SqlDateTime.MaxValue.Value)
                            sqlParameter.Value = System.Data.SqlTypes.SqlDateTime.MaxValue.Value;
                        else
                            sqlParameter.Value = DateTime.SpecifyKind(pgDateTime, DateTimeKind.Unspecified);
                        sqlParameter.DbType = System.Data.DbType.Object;
                        sqlParameter.VastbaseDbType = VastbaseTypes.VastbaseDbType.Timestamp;
                    }
                    else if (sqlParameter.Value is DateTimeOffset pgDateTimeOffset)
                    {
                        // Npgsql 8 语义：timestamptz 参数只接受 UTC（offset 0），非零 offset 直接抛
                        // "Cannot write DateTimeOffset with Offset=+08:00 ... only offset 0 (UTC) is supported"。
                        // 归一化：ToUniversalTime() 保瞬时语义（timestamptz 存储瞬时，offset 不落库），
                        // 任意 offset 可写，读回恒为 UTC（timestamptz 固有）。DbType=DateTimeOffset 保留。
                        sqlParameter.Value = pgDateTimeOffset.ToUniversalTime();
                    }
                    if (parameter.CustomDbType != null && parameter.CustomDbType is VastbaseTypes.VastbaseDbType)
                    {
                        sqlParameter.VastbaseDbType = ((VastbaseTypes.VastbaseDbType)parameter.CustomDbType);
                    }
                    else
                    {
                        switch (parameter.Value)
                        {
                            case VastbaseTypes.VastbaseBox b: sqlParameter.VastbaseDbType = VastbaseTypes.VastbaseDbType.Box; break;
                            case VastbaseTypes.VastbaseCircle c: sqlParameter.VastbaseDbType = VastbaseTypes.VastbaseDbType.Circle; break;
                            case VastbaseTypes.VastbaseLine l: sqlParameter.VastbaseDbType = VastbaseTypes.VastbaseDbType.Line; break;
                            case VastbaseTypes.VastbaseLSeg s: sqlParameter.VastbaseDbType = VastbaseTypes.VastbaseDbType.LSeg; break;
                            case VastbaseTypes.VastbasePath p: sqlParameter.VastbaseDbType = VastbaseTypes.VastbaseDbType.Path; break;
                            case VastbaseTypes.VastbasePoint p: sqlParameter.VastbaseDbType = VastbaseTypes.VastbaseDbType.Point; break;
                            case VastbaseTypes.VastbasePolygon p: sqlParameter.VastbaseDbType = VastbaseTypes.VastbaseDbType.Polygon; break;
                            default: break;
                        }
                    }
                }
                if (xe != null)
                {
                    // XElement → ToString() + VastbaseDbType.Xml（对齐 SqlServerProvider 模式）：驱动对 Xml 类型走 string handler，XElement 值本身无 handler。不设 DbType——DbType=Object 在 Babelfish 下 ToSqlDbType 无映射会抛异常，且 DbType 与 VastbaseDbType 互斥：PG 模式残留 AnsiString 会覆盖 Xml（42804 text→xml）。
                    sqlParameter.VastbaseDbType = VastbaseTypes.VastbaseDbType.Xml;
                }
            }
            return result;
        }

        // 数组值 → PG 数组字面量文本（"{1,2}"，字符串元素引号+反斜杠转义，NULL 元素，
        // 数值 InvariantCulture）。配 VastbaseV5PgArrayParser.Parse 读回。byte[] 不经过此路径
        // （varbinary 专用参数类型，见 ToIDbDataParameter 的 byte[] 分支）。
        private static string FormatPgArrayLiteral(Array arr)
        {
            string Quote(string s) => '"' + s.Replace("\\", "\\\\").Replace("\"", "\\\"") + '"';
            var sb = new System.Text.StringBuilder();
            sb.Append('{');
            for (int i = 0; i < arr.Length; i++)
            {
                if (i > 0) sb.Append(',');
                var v = arr.GetValue(i);
                if (v == null)
                {
                    sb.Append("NULL");
                    continue;
                }
                if (v is string s)
                {
                    sb.Append(Quote(s));
                }
                else if (v is bool || v is byte || v is sbyte || v is short || v is ushort || v is int || v is uint
                    || v is long || v is ulong || v is float || v is double || v is decimal)
                {
                    sb.Append(Convert.ToString(v, System.Globalization.CultureInfo.InvariantCulture));
                }
                else
                {
                    // DateTime/DateTimeOffset/Guid/TimeSpan/char 等：引号包裹字符串化
                    sb.Append(Quote(Convert.ToString(v, System.Globalization.CultureInfo.InvariantCulture)));
                }
            }
            sb.Append('}');
            return sb.ToString();
        }

        private static void UNumber(SugarParameter parameter)
        {
            if (parameter.DbType == System.Data.DbType.UInt16)
            {
                parameter.DbType = System.Data.DbType.Int16;
                parameter.Value = Convert.ToInt16(parameter.Value);
            }
            else if (parameter.DbType == System.Data.DbType.UInt32)
            {
                parameter.DbType = System.Data.DbType.Int32;
                parameter.Value = Convert.ToInt32(parameter.Value);
            }
            else if (parameter.DbType == System.Data.DbType.UInt64)
            {
                parameter.DbType = System.Data.DbType.Int64;
                parameter.Value = Convert.ToInt64(parameter.Value);
            }
            if (parameter.Value is uint)
            {
                parameter.Value = Convert.ToInt32(parameter.Value);
            }
            else if (parameter.Value is ulong)
            {
                parameter.Value = Convert.ToInt64(parameter.Value);
            }
        }

        private static void Array(SugarParameter parameter, VastbaseParameter sqlParameter)
        {
            var type = sqlParameter.Value.GetType();
            if (ArrayMapping.ContainsKey(type))
            {
                sqlParameter.VastbaseDbType = ArrayMapping[type] | VastbaseTypes.VastbaseDbType.Array;
            }
            else if (type == DBNull.Value.GetType())
            {
                DbNullParametrerArray(parameter, sqlParameter);
            }
            else
            {
                Check.Exception(true, sqlParameter.Value.GetType().Name + " No Support");
            }
        }

        private static void DbNullParametrerArray(SugarParameter parameter, VastbaseParameter sqlParameter)
        {
            if (parameter.DbType.IsIn(System.Data.DbType.Int32))
            {
                sqlParameter.VastbaseDbType = VastbaseTypes.VastbaseDbType.Integer | VastbaseTypes.VastbaseDbType.Array;
            }
            else if (parameter.DbType.IsIn(System.Data.DbType.Int16))
            {
                sqlParameter.VastbaseDbType = VastbaseTypes.VastbaseDbType.Smallint | VastbaseTypes.VastbaseDbType.Array;
            }
            else if (parameter.DbType.IsIn(System.Data.DbType.Int64))
            {
                sqlParameter.VastbaseDbType = VastbaseTypes.VastbaseDbType.Bigint | VastbaseTypes.VastbaseDbType.Array;
            }
            else if (parameter.DbType.IsIn(System.Data.DbType.Guid))
            {
                sqlParameter.VastbaseDbType = VastbaseTypes.VastbaseDbType.Uuid | VastbaseTypes.VastbaseDbType.Array;
            }
            else
            {
                sqlParameter.VastbaseDbType = VastbaseTypes.VastbaseDbType.Text | VastbaseTypes.VastbaseDbType.Array;
            }
        }

        static readonly Dictionary<Type, VastbaseTypes.VastbaseDbType> ArrayMapping = new Dictionary<Type, VastbaseTypes.VastbaseDbType>()
        {
            { typeof(int[]), VastbaseTypes.VastbaseDbType.Integer },
            { typeof(short[]), VastbaseTypes.VastbaseDbType.Smallint },
            { typeof(long[]), VastbaseTypes.VastbaseDbType.Bigint },
            { typeof(decimal[]), VastbaseTypes.VastbaseDbType.Numeric },
            { typeof(double[]), VastbaseTypes.VastbaseDbType.Double },
            { typeof(char[]), VastbaseTypes.VastbaseDbType.Text },
            { typeof(byte[]), VastbaseTypes.VastbaseDbType.Bytea },
            { typeof(bool[]), VastbaseTypes.VastbaseDbType.Boolean },
            { typeof(DateTime[]), VastbaseTypes.VastbaseDbType.Date },
            { typeof(float[]), VastbaseTypes.VastbaseDbType.Real },
            { typeof(Guid[]), VastbaseTypes.VastbaseDbType.Uuid },

            { typeof(int?[]), VastbaseTypes.VastbaseDbType.Integer },
            { typeof(short?[]), VastbaseTypes.VastbaseDbType.Smallint },
            { typeof(long?[]), VastbaseTypes.VastbaseDbType.Bigint },
            { typeof(decimal?[]), VastbaseTypes.VastbaseDbType.Numeric },
            { typeof(double?[]), VastbaseTypes.VastbaseDbType.Double },
            { typeof(char?[]), VastbaseTypes.VastbaseDbType.Text },
            { typeof(byte?[]), VastbaseTypes.VastbaseDbType.Bytea },
            { typeof(bool?[]), VastbaseTypes.VastbaseDbType.Boolean },
            { typeof(DateTime?[]), VastbaseTypes.VastbaseDbType.Date },
            { typeof(float?[]), VastbaseTypes.VastbaseDbType.Real },
            { typeof(Guid?[]), VastbaseTypes.VastbaseDbType.Uuid },

            { typeof(string[]), VastbaseTypes.VastbaseDbType.Text },
        };
    }

    // Babelfish SP output 改写状态值对象：GetCommandCore 产出（out 传参），
    // GetDataSetAll 显式接收消费。不落 provider 实例字段 → SqlSugarScope 重入/并发复用下
    // 无「先 GetCommand 后 GetDataSetAll」跨调用覆盖丢（改写与回读严格同帧配对）。
    // 改写本体（DECLARE / EXEC OUTPUT / 回读 SELECT / 输入参数绑定）在 GetCommandCore 内完成
    // 并随命令执行；本对象只携带跨方法消费所需：回读映射 Map 与引用配对守卫（OriginalSql/Params）。
    internal sealed class SpOutRewriteState
    {
        internal readonly string OriginalSql;
        internal readonly List<KeyValuePair<string, SugarParameter>> Map;
        internal readonly SugarParameter[] Params;

        internal SpOutRewriteState(string originalSql, List<KeyValuePair<string, SugarParameter>> map, SugarParameter[] parameters)
        {
            OriginalSql = originalSql;
            Map = map;
            Params = parameters;
        }

        // 消费（GetDataSetAll 帧内调用）：回读 SELECT 结果表 → 输出参数回填，随后移除该表。
        // 引用配对守卫（sql == OriginalSql && parameters == Params，引用相等）保证回读只作用于
        // 产出它的那次改写；守卫任一不成立即整体跳过（不消费、不留痕）。离线可测：给定
        // 值对象 + 构造 DataSet 即可断言回填，不依赖真实 DB。
        internal static void ApplySpOutReadBack(DataSet ds, string sql, SugarParameter[] parameters, SpOutRewriteState state)
        {
            if (state == null) return;
            var map = state.Map;
            if (map == null || map.Count == 0 || ds == null || ds.Tables.Count == 0) return;
            if (sql != state.OriginalSql || parameters != state.Params) return;
            var last = ds.Tables[ds.Tables.Count - 1];
            if (last.Columns.Count == 0 || !last.Columns[0].ColumnName.StartsWith("@")) return;
            if (last.Rows.Count > 0)
            {
                foreach (var kv in map)
                {
                    if (last.Columns.Contains(kv.Key) && last.Rows[0][kv.Key] != DBNull.Value)
                    {
                        kv.Value.Value = last.Rows[0][kv.Key];
                    }
                }
            }
            ds.Tables.Remove(last);
        }
    }

    internal class VastbaseDataAdapterWrapper : IDataAdapter
    {
        private readonly VastbaseDataAdapter _adapter = new VastbaseDataAdapter();

        public VastbaseCommand SelectCommand
        {
            get { return _adapter.SelectCommand as VastbaseCommand; }
            set { _adapter.SelectCommand = value; }
        }

        public int Fill(DataSet ds)
        {
            return _adapter.Fill(ds);
        }

        public DataTable[] FillSchema(DataSet dataSet, SchemaType schemaType)
        {
            return _adapter.FillSchema(dataSet, schemaType);
        }

        public IDataParameter[] GetFillParameters()
        {
            return _adapter.GetFillParameters();
        }

        public int Update(DataSet dataSet)
        {
            return _adapter.Update(dataSet);
        }

        public MissingMappingAction MissingMappingAction
        {
            get { return _adapter.MissingMappingAction; }
            set { _adapter.MissingMappingAction = value; }
        }

        public MissingSchemaAction MissingSchemaAction
        {
            get { return _adapter.MissingSchemaAction; }
            set { _adapter.MissingSchemaAction = value; }
        }

        public ITableMappingCollection TableMappings
        {
            get { return _adapter.TableMappings; }
        }
    }
}
