using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Vastbase;

namespace SqlSugar
{
    public class VastbaseV5FastBuilder : PostgreSQLFastBuilder, IFastBuilder
    {
        private readonly EntityInfo _entityInfo;

        // 由 FastestProvider 装配点注入：AS() 目标名（null = 未指定）。实体路径
        // identity 剥列的门禁条件之一，与 IsOffIdentity 同源。
        internal string AsName;

        public VastbaseV5FastBuilder(EntityInfo entityInfo) : base(entityInfo)
        {
            _entityInfo = entityInfo;
        }

        // COPY 二进制批量导入，Babelfish 兼容模式(TDS) 与 PG 模式同一路径。
        // 失败直抛、不重试：TDS 层存在「出错后会话残留协议消息」的引擎缺陷，池化回收后
        // 首条 COPY 可能失步抛错但数据已落库，任何重试都会重复插入。
        public override async Task<int> ExecuteBulkCopyAsync(DataTable dt)
        {
            var mode = VastbaseV5Mode.Require(this.Context);
            // 列编码门与 rowversion 剥列共用一次目标列元数据查询，且须在 COPY 开始前查：
            // COPY 进行中同连接不能再执行命令，AutoClose 下连接打开后查元数据还会关掉连接。
            // 解析名优先实际 COPY 目标（.AS() 时与实体表名不同，按实体名解析会漏）；temp 表
            //（# 前缀）解析不到时回落实体表名；均取不到 → 无门。
            var dbCols = ResolveTargetColumns(dt == null ? null : dt.TableName);
            if (dbCols == null || dbCols.Count == 0)
                dbCols = ResolveTargetColumns(_entityInfo == null ? null : _entityInfo.DbTableName);
            // identity 剥列（PG/Babelfish 双模式，实体/DataTable 全路径同源承接）：主库不再为
            // VastbaseV5 特判（零主码），dt 会带 identity 列及默认值（如 int 的 0）到达此处；
            // PG 语义 identity 值不随 COPY 显式写入（serial/identity 由服务端生成）。两源剥除：
            // ①实体声明列（DbColumnName 空回退 PropertyName，EqualCase）；
            // ②实体未标 IsIdentity 但 .AS() 目标表有（手工 identity 表）时按目标表元数据剥
            //  （原主库 ToDdateTable 第二路径与 Babelfish 实体路径合并推广为双模式——TDS COPY
            //   无 identity 防护会静默落库，PG 模式对齐主库旧行为）。temp 链（# 前缀）不剥：
            // BulkUpdate/Merge 的 JOIN 键列需要真实值（RestoreWhereColumns 补回的键列保留）；
            // IsOffIdentity=true（显式要求写入 identity 值）不剥，与主库旧门一致。
            if (dt != null
                && !dt.TableName.StartsWith("#")
                && dt.ExtendedProperties["VastbaseV5TempChain"] == null
                && (this.DbFastestProperties == null || this.DbFastestProperties.IsOffIdentity == false))
            {
                if (_entityInfo != null && _entityInfo.Columns.Any(c => c.IsIdentity))
                {
                    var identityNames = _entityInfo.Columns.Where(c => c.IsIdentity)
                        .Select(c => c.DbColumnName ?? c.PropertyName).ToList();
                    foreach (var name in identityNames)
                    {
                        var hit = dt.Columns.Cast<DataColumn>()
                            .FirstOrDefault(dc => dc.ColumnName.EqualCase(name));
                        if (hit != null)
                        {
                            dt.Columns.Remove(hit);
                        }
                    }
                }
                else if (AsName.HasValue() && dbCols != null)
                {
                    var dbIdentity = dbCols.Find(c => c.IsIdentity == true);
                    if (dbIdentity != null)
                    {
                        var hit = dt.Columns.Cast<DataColumn>()
                            .FirstOrDefault(dc => dc.ColumnName.EqualCase(dbIdentity.DbColumnName));
                        if (hit != null)
                        {
                            dt.Columns.Remove(hit);
                        }
                    }
                }
            }
            // sql_variant 列直拒：binary COPY 不按插件变体格式编码，服务端把裸基类型字节误读
            // 为变体元数据头，批次假成功且静默损坏（读侧可报 pglz corrupt）；驱动的逐行类型
            // 校验不足以拦截。temp 链（BulkUpdate/BulkMerge）同在此拦截；INSERT 路径不受影响，
            // 拒 COPY 时指引改用 Insertable。
            var variantCols = dbCols == null ? null
                : dbCols.Where(c => c.DataType != null && c.DataType.EqualCase("sql_variant")).ToList();
            if (variantCols != null && variantCols.Count > 0)
                throw new SqlSugarException("BulkCopy 不支持 sql_variant 列（COPY 路径会静默损坏数据），请改用 Insertable 插入。涉及列：" + string.Join(",", variantCols.Select(c => c.DbColumnName)));

            // 实体路径列裁剪（Babelfish）：dt 列来自 SELECT * 模板，「表有实体无」的列值恒为
            // DBNull——COPY 带这些列时 rowversion 被拒（ROWVERSION/TIMESTAMP column）、
            // NOT NULL DEFAULT 列被显式 NULL 顶掉默认值（515）。裁到实体映射列 ∪ discriminator；
            // PG 模式不裁；dt 列与映射列零交集时不裁（防误删业务列）。
            if (mode.IsBabelfish && dt != null && _entityInfo != null && _entityInfo.Columns.Count > 0)
            {
                var mappedNames = _entityInfo.Columns
                    .Where(c => !c.IsIgnore)
                    .Select(c => c.DbColumnName ?? c.PropertyName)
                    .ToList();
                if (_entityInfo.Discrimator.HasValue())
                {
                    foreach (var disItem in _entityInfo.Discrimator.Split(','))
                        mappedNames.Add(disItem.Split(':').First());
                }
                if (dt.Columns.Cast<DataColumn>().Any(dc => mappedNames.Any(m => m.EqualCase(dc.ColumnName))))
                {
                    foreach (var dc in dt.Columns.Cast<DataColumn>().ToList())
                    {
                        if (!mappedNames.Any(m => m.EqualCase(dc.ColumnName)))
                            dt.Columns.Remove(dc);
                    }
                }
            }
            // TDS COPY 一律拒绝显式写 rowversion（ROWVERSION/TIMESTAMP column），SqlBulkCopy
            // 语义是跳过该列由服务器生成。用户 DataTable 路径（BulkCopy(DataTable) 各重载）
            // 无从识别意图，在此按元数据剥除；实体路径上游已裁剪，此处幂等。Babelfish 元数据
            // 下 rowversion 显示为 timestamp，判别无歧义。
            if (mode.IsBabelfish && dbCols != null && dt != null)
            {
                foreach (var col in dt.Columns.Cast<DataColumn>().ToList())
                {
                    if (dbCols.Any(c => c.DbColumnName.EqualCase(col.ColumnName)
                        && c.DataType != null && c.DataType.EqualCase("timestamp")))
                    {
                        dt.Columns.Remove(col);
                    }
                }
            }
            // 列名小写化：PG 物理列名小写、COPY 带引号标识符精确匹配物理名——上游为
            // identity 实体手工补回的 whereColumns 是实体声明 case，不转小写即 42703。
            // Babelfish 的 TDS 层对 COPY 内标识符大小写不敏感，小写化无影响。
            var colNames = new List<string>();
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                colNames.Add("\"" + dt.Columns[i].ColumnName.ToLowerInvariant() + "\"");
            }
            string copyString = $"COPY  {BuildCopyTableName(dt, mode)} ( {string.Join(",", colNames)} ) FROM STDIN (FORMAT BINARY)";
            var gates = ResolveCopyColumnGates(dbCols, mode);
            var conn = (VastbaseConnection)this.Context.Ado.Connection;
            try
            {
                BulkCopy(dt, copyString, gates, conn);
            }
            catch
            {
                // 引擎缺陷缓解：COPY 半途失败（约束违反等）后，服务端会话残留 COPY 协议
                // 消息，本连接已不可用——直接归池会让下一个借到它的请求随机中招（报
                // invalid frontend message type 99/100）。失败点当场发一次读吸收残留：
                // 驱动遇残留错误即废弃该物理连接，后续请求从池中拿到新连接。排水异常
                // 吞掉，原始 COPY 错误保持直抛；引擎修复后此读直接成功，零成本。
                DrainResidualCopyState(conn);
                throw;
            }
            finally
            {
                // 连接生命周期对齐 PG/SqlServer 模板：AutoClose 且无事务时关闭，
                // 不论连接是否本方法打开。
                base.CloseDb();
            }
            return await Task.FromResult(dt.Rows.Count);
        }

        // 目标名：Babelfish 下已带括号/引号的形态原样（TDS 层解析）；裸名逐段补括号——
        // 保留字（如 Order）裸发即语法错。PG 模式原样：不带引号的名按 PG 规则折叠到
        // 小写物理名，带引号的名保持精确匹配。
        private static string BuildCopyTableName(DataTable dt, VastbaseV5Mode mode)
        {
            if (mode.IsBabelfish
                && dt.TableName.IndexOf('[') < 0
                && dt.TableName.IndexOf('"') < 0)
            {
                return string.Join(".", SplitOuter(dt.TableName).Select(s => "[" + s.Trim() + "]"));
            }
            return dt.TableName;
        }

        // 三个编码门（按目标列元数据分类），writer 经 dynamic 分派：
        //  ① jsonb 列须按 jsonb 二进制格式写（版本字节 + JSON 文本），否则报 XX000
        //     unsupported jsonb version number；json 列服务端容错 raw text。
        //  ② date/time/money 列驱动默认按 CLR 类型编码，与 COPY 二进制期望格式错配
        //     （22P03 incorrect binary data format），须显式 VastbaseDbType。money 分模式：
        //     PG 显式 Money；Babelfish 底层为 int64 定标 4，decimal 预定标 ×10⁴ 走 Bigint。
        //  ③ 其余列 untyped：dynamic 按运行期值类型选 handler（静态 object 会绑成 Write<object>）。
        private static void BulkCopy(DataTable dt, string copyString, CopyColumnGates gates, VastbaseConnection conn)
        {
            if (conn.State == ConnectionState.Closed)
                conn.Open();
            dynamic writer = conn.BeginBinaryImport(copyString);
            Exception pending = null;
            try
            {
                foreach (DataRow row in dt.Rows)
                {
                    writer.StartRow();
                    foreach (DataColumn col in dt.Columns)
                    {
                        var val = row[col.ColumnName];
                        if (val is string sv && gates.JsonbCols != null && gates.JsonbCols.Contains(col.ColumnName))
                            writer.Write(sv, VastbaseTypes.VastbaseDbType.Jsonb);
                        else if (gates.TypedCols != null
                            && val != null && val != DBNull.Value
                            && gates.TypedCols.TryGetValue(col.ColumnName, out var explicitType)
                            && TryPrepareExplicitCopyValue(val, explicitType, out var prepared))
                            writer.Write((dynamic)prepared, explicitType);
                        else
                            writer.Write(val ?? DBNull.Value);
                    }
                }
                writer.Complete();
            }
            catch (Exception ex) { pending = ex; throw; }
            finally
            {
                // COPY 未 Complete 即 Dispose 时，驱动发 CopyFail 中止，部分引擎版本还会回一帧
                // 空错误使 Dispose 抛 PostgresException——finally 里抛新异常会替换正在传播的原始
                // 异常（C# 语义）。已有异常在传播时吞掉 Dispose 异常以保住根因；正常路径照常上抛。
                try { ((IDisposable)writer).Dispose(); }
                catch when (pending != null) { }
            }
        }

        // 显式编码门只对匹配的 CLR 值类型生效并完成值变换（Babelfish money/smallmoney
        // 在此预定标 ×10⁴——SQL Server money 语义 scale 4，四舍五入对齐服务端行为），
        // 不匹配/DBNull 回落 untyped（不引入新失败面）
        private static bool TryPrepareExplicitCopyValue(object val, VastbaseTypes.VastbaseDbType dbType, out object prepared)
        {
            switch (dbType)
            {
                case VastbaseTypes.VastbaseDbType.Date:
                    prepared = val is DateTime ? val : null; return prepared != null;
                case VastbaseTypes.VastbaseDbType.Time:
                    prepared = val is TimeSpan ? val : null; return prepared != null;
                case VastbaseTypes.VastbaseDbType.Money:
                    prepared = val is decimal ? val : null; return prepared != null;
                case VastbaseTypes.VastbaseDbType.Bigint: // Babelfish money/smallmoney 定标腿
                    if (val is decimal d)
                    {
                        prepared = (long)(Math.Round(d, 4, MidpointRounding.AwayFromZero) * 10000m);
                        return true;
                    }
                    prepared = null; return false;
                default:
                    prepared = null; return false;
            }
        }

        private class CopyColumnGates
        {
            // null = 无元数据（实体/表名均解析不到），全部列 untyped
            public HashSet<string> JsonbCols;
            public Dictionary<string, VastbaseTypes.VastbaseDbType> TypedCols;
        }

        private CopyColumnGates ResolveCopyColumnGates(List<DbColumnInfo> dbCols, VastbaseV5Mode mode)
        {
            var gates = new CopyColumnGates();
            if (dbCols == null || dbCols.Count == 0)
                return gates;
            gates.JsonbCols = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            gates.TypedCols = new Dictionary<string, VastbaseTypes.VastbaseDbType>(StringComparer.OrdinalIgnoreCase);
            foreach (var c in dbCols)
            {
                if (c.DataType == null)
                    continue;
                if (c.DataType.EqualCase("jsonb"))
                    gates.JsonbCols.Add(c.DbColumnName);
                else if (c.DataType.EqualCase("date"))
                    gates.TypedCols[c.DbColumnName] = VastbaseTypes.VastbaseDbType.Date;
                else if (c.DataType.EqualCase("time"))
                    gates.TypedCols[c.DbColumnName] = VastbaseTypes.VastbaseDbType.Time;
                else if (c.DataType.EqualCase("money") || c.DataType.EqualCase("smallmoney"))
                    // PG 模式 money 走驱动原生编码；Babelfish 模式 money/smallmoney 均为
                    // int64 定标 4 底层，走 Bigint 定标腿（值变换在 TryPrepareExplicitCopyValue）
                    gates.TypedCols[c.DbColumnName] = mode.IsBabelfish
                        ? VastbaseTypes.VastbaseDbType.Bigint
                        : VastbaseTypes.VastbaseDbType.Money;
            }
            return gates;
        }

        private List<DbColumnInfo> ResolveTargetColumns(string tableName)
        {
            if (!tableName.HasValue() || tableName.StartsWith("#"))
                return null;
            try
            {
                var clean = tableName.Replace("[", "").Replace("]", "").Replace("\"", "");
                return this.Context.DbMaintenance.GetColumnInfosByTableName(clean);
            }
            catch
            {
                return null;
            }
        }

        // COPY 失败后的一次牺牲读：把服务端残留的协议错误吸收在本调用内（驱动会随之
        // 废弃该物理连接，池不再复用它）。任何异常都吞掉——只为清场，不产生新的
        // 用户可见行为；无残留时此读直接成功。
        private static void DrainResidualCopyState(VastbaseConnection conn)
        {
            try
            {
                if (conn.State != ConnectionState.Open)
                    return;
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT 1";
                cmd.ExecuteScalar();
            }
            catch { }
        }

        // 按顶层点拆段（段文本原样保留）。双引号成对（开=闭），按切换处理深度；
        // 方括号按进出计数。SqlSugar 翻译名各段统一 quote，无混合嵌套。
        private static List<string> SplitOuter(string name)
        {
            var parts = new List<string>();
            var depth = 0;
            var start = 0;
            for (var i = 0; i < name.Length; i++)
            {
                var ch = name[i];
                if (ch == '[') depth++;
                else if (ch == ']') { if (depth > 0) depth--; }
                else if (ch == '"') depth = depth == 0 ? 1 : 0;
                else if (ch == '.' && depth == 0)
                {
                    parts.Add(name.Substring(start, i - start));
                    start = i + 1;
                }
            }
            parts.Add(name.Substring(start));
            return parts;
        }

        // Babelfish（T-SQL）temp 表：结构用 SELECT INTO 从目标表复制，列类型随之保真；
        // #temp 是会话级对象，先清上次残留。PG 模式走基类（into temp）。
        public override async Task CreateTempAsync<T>(DataTable dt)
        {
            var mode = VastbaseV5Mode.Require(this.Context);
            if (mode.IsBabelfish)
            {
                try { await this.Context.Ado.ExecuteCommandAsync("DROP TABLE IF EXISTS [#mytemptable]"); }
                catch { }
                await this.Context.Queryable<T>().Where(it => false).AS(dt.TableName)
                    .Select("  * into  #mytemptable").ToListAsync();
                dt.TableName = "#mytemptable";
            }
            else
            {
                await base.CreateTempAsync<T>(dt);
            }
        }

        // Babelfish 的 UPDATE...FROM INNER JOIN（T-SQL 形态，与抽象基类默认模板同形）。
        // 不能直接用 base：继承链上一级是 PostgreSQLFastBuilder 的 PG 形态实现
        // （UPDATE t SET c=TE.c FROM temp AS TE），TDS 方言下不成立，故按基类模板复刻。
        public override async Task<int> UpdateByTempAsync(string tableName, string tempName, string[] updateColumns, string[] whereColumns)
        {
            var mode = VastbaseV5Mode.Require(this.Context);
            if (mode.IsBabelfish)
            {
                Check.ArgumentNullException(!updateColumns.Any(), "update columns count is 0");
                Check.ArgumentNullException(!whereColumns.Any(), "where columns count is 0");
                var sqlquerybulder = this.Context.Queryable<object>().SqlBuilder;
                var sets = string.Join(",", updateColumns.Select(it => $"TM.{sqlquerybulder.GetTranslationColumnName(it)}=TE.{sqlquerybulder.GetTranslationColumnName(it)}"));
                var wheres = string.Join(" AND ", whereColumns.Select(it => $"TM.{sqlquerybulder.GetTranslationColumnName(it)}=TE.{sqlquerybulder.GetTranslationColumnName(it)}"));
                var sql = string.Format(@"UPDATE TM SET {0} FROM {1} TM INNER JOIN {2} TE ON {3}", sets, tableName, tempName, wheres);
                return await this.Context.Ado.ExecuteCommandAsync(sql);
            }
            return await base.UpdateByTempAsync(tableName, tempName, updateColumns, whereColumns);
        }
    }
}
