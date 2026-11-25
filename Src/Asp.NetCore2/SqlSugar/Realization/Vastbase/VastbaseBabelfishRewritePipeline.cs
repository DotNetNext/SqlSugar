using System;
using System.Text.RegularExpressions;

namespace SqlSugar
{
    // Babelfish 兼容模式(TDS) 对 base PG SQL 的 7 步事后重写链；调用方（QueryBuilder/
    // UpdateBuilder 的 ToSqlString）只调一次 Apply，步骤顺序在 Steps 数组显式声明。
    // 第 7 步归一透传 SQL 内标识符尾空格（SQL Server 词法层自动 strip，Babelfish 不 strip）。
    //
    // 形状漂移守卫：每步前置断言——输入含目标特征但转换模式匹配数为 0 即抛
    // ShapeDriftException。base 输出形状变化（CAST 拼写、LIMIT 大小写等）时静默失配
    // 会把 PG 形态 SQL 直送 TDS，守卫把「静默不转换」改为生成期报错；无目标特征自检跳过。
    internal static class VastbaseV5BabelfishRewritePipeline
    {
        // 形状漂移异常：重写链失配即抛（匹配数=0 且输入含目标特征），不再静默放行。
        internal sealed class ShapeDriftException : Exception
        {
            internal ShapeDriftException(string step, string message)
                : base("Vastbase Babelfish 重写链形状漂移（步骤 " + step + "）：" + message)
            {
            }
        }

        // 调用方执行上下文：QueryBuilder 与 UpdateBuilder 各自 ToSqlString 提供的执行参数。
        internal sealed class RewriteContext
        {
            internal bool HasGroupBy;
            internal int Take;
            internal int Skip;
            internal string OrderBy;
        }

        // 显式步骤顺序。每步自检：无目标特征即原样跳过；有特征但转换失配即抛（见各步断言）。
        private static readonly Func<string, RewriteContext, string>[] Steps =
        {
            SyncGroupByParams,
            FixGroupBy,
            NormalizeExistsSelect,
            ConvertPagination,
            NormalizeCastTypes,
            NormalizeDatePart,
            NormalizeTrailingSpaceIdentifiers,
        };

        internal static string Apply(string sql, RewriteContext ctx)
        {
            foreach (var step in Steps)
            {
                sql = step(sql, ctx);
            }
            return sql;
        }

        private static ShapeDriftException Drift(string step, string reason, string sql)
        {
            var snippet = sql.Length <= 200 ? sql : sql.Substring(0, 200) + "…";
            return new ShapeDriftException(step,
                reason + " SQL: " + snippet);
        }

        // ========== Step 1: SyncGroupByParams ==========

        private static string SyncGroupByParams(string sql, RewriteContext ctx)
        {
            if (!ctx.HasGroupBy) return sql;
            // 形状断言：调用方断言有 GROUP BY，但 SQL 无 ' GROUP BY ' 精确形状
            // （base GetGroupByString 空格/换行拼法变化即漂移，Index 匹配将静默失配）。
            if (sql.IndexOf(" GROUP BY ") < 0)
                throw Drift(nameof(SyncGroupByParams),
                    "存在 GROUP BY（调用方断言）但 SQL 无 ' GROUP BY ' 形状，重写失配。", sql);

            var groupIdx = sql.IndexOf(" GROUP BY ");
            var fromIdx = sql.IndexOf(" FROM ");
            var selectPart = sql.Substring(0, fromIdx > 0 ? fromIdx : sql.Length);
            var orderIdx = sql.IndexOf(" ORDER BY ");
            var groupEnd = orderIdx > groupIdx ? orderIdx : sql.Length;
            var groupPart = sql.Substring(groupIdx, groupEnd - groupIdx);
            var prefix = sql.Substring(0, groupIdx);
            var suffix = sql.Substring(groupEnd);

            var replaced = groupPart;

            var caseRx = new Regex(
                @"CASE\s+WHEN\s+.+?END",
                RegexOptions.IgnoreCase);
            var funcRx = new Regex(
                @"\b(?:ISNULL|COALESCE|CAST|CONVERT|IIF)\s*\([^)]*@[\w\d]+[^)]*\)",
                RegexOptions.IgnoreCase);
            // 形状断言 2：group 部分含参数（@\w+\d+）且含同步关键字（CASE/ISNULL/COALESCE/
            // CAST/CONVERT/IIF），但可识别形态匹配数为 0 → 表达式形状漂移（如简单 CASE
            // 无 WHEN、函数括号拼写变化），参数同步将静默失配 → 抛。
            // 裸参数算术（GROUP BY [t].[a] + @p1，无同步关键字）是正常形状，不误报。
            if (Regex.IsMatch(groupPart, @"@[\w\d]+")
                && Regex.IsMatch(groupPart, @"\b(?:CASE|ISNULL|COALESCE|CAST|CONVERT|IIF)\b", RegexOptions.IgnoreCase)
                && !caseRx.IsMatch(groupPart)
                && !funcRx.IsMatch(groupPart))
                throw Drift(nameof(SyncGroupByParams),
                    "GROUP BY 部分含参数与同步关键字但无可识别 CASE/函数形态，参数同步失配。", sql);

            foreach (Match gm in caseRx.Matches(groupPart))
            {
                var gbExpr = gm.Value;
                var gbNorm = NormParams(gbExpr);
                foreach (Match sm in caseRx.Matches(selectPart))
                {
                    var selExpr = sm.Value;
                    if (NormParams(selExpr) == gbNorm && selExpr != gbExpr)
                    {
                        replaced = replaced.Replace(gbExpr, selExpr);
                        break;
                    }
                }
            }

            foreach (Match gm in funcRx.Matches(replaced))
            {
                var gbExpr = gm.Value;
                var gbNorm = NormParams(gbExpr);
                foreach (Match sm in funcRx.Matches(selectPart))
                {
                    var selExpr = sm.Value;
                    if (NormParams(selExpr) == gbNorm && selExpr != gbExpr)
                    {
                        replaced = replaced.Replace(gbExpr, selExpr);
                        break;
                    }
                }
            }

            return prefix + replaced + suffix;
        }

        private static string NormParams(string expr) =>
            Regex.Replace(expr, @"@\w+\d+", "@PN");

        // ========== Step 2: FixGroupBy ==========

        private static string FixGroupBy(string sql, RewriteContext ctx)
        {
            if (!ctx.HasGroupBy) return sql;
            // 形状断言 1：无 ' FROM ' 精确形状 → 旧实现会 ArgumentOutOfRangeException，
            // 改为明确漂移报错（select 部分切分失配）。
            var fromIdx = sql.IndexOf(" FROM ");
            if (fromIdx < 0)
                throw Drift(nameof(FixGroupBy),
                    "SQL 无 ' FROM ' 形状，select 部分切分失配。", sql);

            var selectPart = sql.Substring(0, fromIdx);
            // 形状断言 2：select 部分存在宽松形态的列别名（'[col] AS ['，忽略大小写），
            // 但可识别形态匹配数为 0（AS 大小写/别名引号格式漂移）→ 静默跳过即 GROUP BY
            // 缺列，Babelfish 下运行期报错；改为生成期抛错。
            if (Regex.IsMatch(selectPart, @"\[[\w]+\]\s+AS\s+\[", RegexOptions.IgnoreCase)
                && !Regex.IsMatch(selectPart, @"((?:\[[\w]+\]\.)?\[[\w]+\])\s+AS\s+\[(\w+)\]"))
                throw Drift(nameof(FixGroupBy),
                    "select 部分存在 '[col] AS [' 形态列别名但无可识别形态（大小写/引号漂移），GROUP BY 补列将失配。", sql);

            var colAliasMatches = Regex.Matches(
                selectPart, @"((?:\[[\w]+\]\.)?\[[\w]+\])\s+AS\s+\[(\w+)\]");

            var groupIdx = sql.IndexOf(" GROUP BY ");
            var orderIdx = sql.IndexOf(" ORDER BY ");
            var groupPart = orderIdx > groupIdx
                ? sql.Substring(groupIdx + 10, orderIdx - groupIdx - 10)
                : sql.Substring(groupIdx + 10);
            var groupLower = groupPart.ToLower();

            var missingExprs = new System.Collections.Generic.HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (Match m in colAliasMatches)
            {
                var expr = m.Groups[1].Value;
                var alias = m.Groups[2].Value;
                if (groupLower.Contains(expr.ToLower()))
                    continue;
                if (groupLower.Contains(alias.ToLower()))
                    continue;
                missingExprs.Add(expr);
            }

            if (missingExprs.Count > 0)
            {
                var appendCols = string.Join(", ", missingExprs);
                if (orderIdx > groupIdx)
                    sql = sql.Insert(orderIdx, ", " + appendCols + " ");
                else
                    sql = sql.TrimEnd() + ", " + appendCols;
            }
            return sql;
        }

        // ========== Step 3: NormalizeExistsSelect ==========

        private static string NormalizeExistsSelect(string sql, RewriteContext ctx)
        {
            // 形状断言：EXISTS 子查询出现限定星号（SELECT t.*）——base 若开始限定星号，
            // 主模式（SELECT * FROM）失配即静默放行 PG 形态（T-SQL EXISTS 需 SELECT 1）。
            if (Regex.IsMatch(sql, @"EXISTS\s*\(\s*SELECT\s+\w+\.\*", RegexOptions.IgnoreCase)
                && !Regex.IsMatch(sql, @"EXISTS\s*\(\s*SELECT\s+\*\s+FROM", RegexOptions.IgnoreCase))
                throw Drift(nameof(NormalizeExistsSelect),
                    "EXISTS 子查询为 'SELECT t.*' 形态（主模式仅收 'SELECT * FROM'），改写失配。", sql);
            return Regex.Replace(sql, @"EXISTS\s*\(\s*SELECT\s+\*\s+FROM", "EXISTS (SELECT 1 FROM");
        }

        // ========== Step 4: ConvertPagination ==========

        private static string ConvertPagination(string sql, RewriteContext ctx)
        {
            // 查询语句（SELECT/WITH…SELECT）才可能有分页形态；非查询语句（UPDATE 等，
            // 如 SET [limit] 列）整体跳过，避免剥离误伤（旧实现仅查询链调用，
            // 收拢后 UpdateBuilder 也走此入口，须在此显式排除）。
            var trimmed = sql.TrimStart();
            if (!trimmed.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase)
                && !trimmed.StartsWith("WITH", StringComparison.OrdinalIgnoreCase))
                return sql;

            var hasLimit = sql.Contains("LIMIT");
            if (!hasLimit)
            {
                // 形状断言：存在 LIMIT 数字形态（忽略大小写）但非 'LIMIT' 精确形态
                // （base 若改小写/大小写混写，Contains 匹配即静默失配 → PG 形态 LIMIT
                // 直送 TDS）。断言模式锚定语句尾（builder 输出 'LIMIT {take} offset {skip}'/
                // 'LIMIT {take}'），避免命中字符串字面量（如 LIKE '% limit 5%'）。
                if (Regex.IsMatch(sql, @"\slimit\s+\d+(\s+offset\s+\d+)?\s*$", RegexOptions.IgnoreCase))
                    throw Drift(nameof(ConvertPagination),
                        "SQL 含 LIMIT 数字形态（忽略大小写）但非 'LIMIT' 精确形态，分页改写失配。", sql);
                return sql;
            }
            sql = sql.Substring(0, sql.LastIndexOf("LIMIT")).TrimEnd();
            if (ctx.Skip == 0 && ctx.Take > 0)
                sql = InsertTopAfterDistinct(sql, ctx.Take);
            else if (ctx.Skip > 0)
            {
                var orderBy = ctx.OrderBy;
                if (string.IsNullOrEmpty(orderBy) || orderBy.Trim() == "ORDER BY" || orderBy == "ORDER BY ")
                    sql += " ORDER BY GetDate() ";
                else if (ctx.Take == 0)
                    sql += " " + orderBy.Trim();
                sql += string.Format(" OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", ctx.Skip, ctx.Take > 0 ? ctx.Take : int.MaxValue);
            }
            return sql;
        }

        private static string InsertTopAfterDistinct(string sql, int topCount)
        {
            var afterSelect = sql.Substring("SELECT".Length);
            if (afterSelect.TrimStart().StartsWith("distinct ", StringComparison.OrdinalIgnoreCase))
            {
                var trimStart = afterSelect.TrimStart();
                var distinctLen = trimStart.IndexOf(' ') + 1;
                var afterDistinct = trimStart.Substring(distinctLen);
                return "SELECT distinct TOP " + topCount + " " + afterDistinct;
            }
            return "SELECT TOP " + topCount + afterSelect;
        }

        // ========== Step 5: NormalizeCastTypes ==========

        private static string NormalizeCastTypes(string sql, RewriteContext ctx)
        {
            // 形状断言：CAST 目标类型特征存在但转换模式失配 → 漂移
            // （如 AS timestamp(6)/timestamp without time zone、varchar 与 ) 之间插入空白、
            // COLLATE 等——旧实现静默放行，Babelfish 下截断/类型错误运行期才爆）。
            AssertCastShape(sql, "timestamp", @"\s+AS\s+timestamp\)", @"\bAS\s+timestamp\b");
            AssertCastShape(sql, "uuid", @"\s+AS\s+uuid\)", @"\bAS\s+uuid\b");
            // varchar 检测模式需紧跟右括号（可容括号前空白）：varchar(50) 是显式长度合法形态
            // （无需改写），不得误报；varchar 后跟空白/COLLATE 等才是漂移形态。
            AssertCastShape(sql, "varchar", @"\s+AS\s+varchar\)", @"\bAS\s+varchar\s*\)");

            sql = Regex.Replace(sql, @"\s+AS\s+timestamp\)", " AS datetime)", RegexOptions.IgnoreCase);
            sql = Regex.Replace(sql, @"\s+AS\s+uuid\)", " AS uniqueidentifier)", RegexOptions.IgnoreCase);
            // Babelfish 下 CAST(x AS varchar) 无长度按 T-SQL 默认 30 截断（长值如 GUID 会被截断
            // 致 WHERE 静默失配），显式补 varchar(MAX)。
            sql = Regex.Replace(sql, @"\s+AS\s+varchar\)", " AS varchar(MAX))", RegexOptions.IgnoreCase);
            return sql;
        }

        private static void AssertCastShape(string sql, string type, string mainPattern, string probePattern)
        {
            if (Regex.IsMatch(sql, probePattern, RegexOptions.IgnoreCase)
                && !Regex.IsMatch(sql, mainPattern, RegexOptions.IgnoreCase))
                throw Drift(nameof(NormalizeCastTypes),
                    "CAST(x AS " + type + ") 目标特征存在但转换模式失配，类型改写将静默跳过。", sql);
        }

        // ========== Step 6: NormalizeDatePart ==========

        private static string NormalizeDatePart(string sql, RewriteContext ctx)
        {
            // 形状断言：DATE_PART/EXTRACT 特征存在但四个转换模式全失配 → 漂移
            // （如 DATE_PART 字段不带引号、小写 date_part（主模式大小写敏感）、
            // extract 后无 FROM——旧实现静默放行，TDS 下 'function date_part does not exist'）。
            if ((Regex.IsMatch(sql, @"\bDATE_PART\b", RegexOptions.IgnoreCase)
                 || Regex.IsMatch(sql, @"\bextract\s*\(", RegexOptions.IgnoreCase))
                && !Regex.IsMatch(sql, @"DATE_PART\('(\w+)',\s*")
                && !Regex.IsMatch(sql, @"extract\s*\(\s*DOW\s+FROM\s+", RegexOptions.IgnoreCase)
                && !Regex.IsMatch(sql, @"extract\s*\(\s*DOY\s+FROM\s+", RegexOptions.IgnoreCase)
                && !Regex.IsMatch(sql, @"extract\s*\(\s*(\w+)\s+FROM\s+", RegexOptions.IgnoreCase))
                throw Drift(nameof(NormalizeDatePart),
                    "DATE_PART/EXTRACT 特征存在但四个转换模式全失配，日期函数改写将静默跳过。", sql);

            sql = Regex.Replace(sql, @"DATE_PART\('(\w+)',\s*", @"DATEPART($1, ");
            // PostgreSQL EXTRACT(field FROM expr) → T-SQL DATEPART(field, expr).
            // DOW/DOY are PostgreSQL-only names; map to T-SQL weekday/dayofyear.
            sql = Regex.Replace(sql, @"extract\s*\(\s*DOW\s+FROM\s+", "DATEPART(weekday, ", RegexOptions.IgnoreCase);
            sql = Regex.Replace(sql, @"extract\s*\(\s*DOY\s+FROM\s+", "DATEPART(dayofyear, ", RegexOptions.IgnoreCase);
            sql = Regex.Replace(sql, @"extract\s*\(\s*(\w+)\s+FROM\s+", "DATEPART($1, ", RegexOptions.IgnoreCase);
            return sql;
        }

        // ========== Step 7: NormalizeTrailingSpaceIdentifiers ==========

        // 透传 SQL（Select/JoinWhere/Where 字符串）内标识符与 . 之间的尾空格归一：SQL Server
        // 词法层自动 strip 而 Babelfish 不 strip，带空格标识符会解析失败（relation/column
        // does not exist）；框括号名路径已 TrimEnd，此处是唯一残留。先按单引号字面量（含
        // '' 转义）拆分只处理字面量外片段；[ 或 " 括起内容不受影响，标识符起始须词边界。
        // 纯正则替换无可失配形状，故无漂移断言。
        private static readonly Regex SqlLiteralRx = new Regex(
            @"('(?:[^']|'')*')",
            RegexOptions.Compiled);

        private static readonly Regex TrailingSpaceIdentRx = new Regex(
            @"(?<!\[)(?<!"")\b(?<id>[A-Za-z_][A-Za-z0-9_$#]*)[ \t]+\.(?=\w|\*|\[)",
            RegexOptions.Compiled);

        private static string NormalizeTrailingSpaceIdentifiers(string sql, RewriteContext ctx)
        {
            // Regex.Split 单捕获组：奇数索引 = 字面量原文（含引号），偶数索引 = 字面量外片段
            var parts = SqlLiteralRx.Split(sql);
            for (var i = 0; i < parts.Length; i += 2)
                parts[i] = TrailingSpaceIdentRx.Replace(parts[i], "${id}.");
            return string.Concat(parts);
        }
    }
}
