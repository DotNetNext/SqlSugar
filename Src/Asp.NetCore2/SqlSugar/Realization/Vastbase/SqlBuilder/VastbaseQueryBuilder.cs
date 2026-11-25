using System.Linq;
using System.Text.RegularExpressions;

namespace SqlSugar
{
    public class VastbaseV5QueryBuilder : PostgreSQLQueryBuilder
    {
        private VastbaseV5Mode Mode => VastbaseV5Mode.Require(this.Context);

        public override string ToJoinString(JoinQueryInfo joinInfo)
        {
            // 引擎拒收派生表别名后的表提示（WITH(NOLOCK) → 42601 syntax error near 'with'；
            // 表后/子查询内的提示均可接受）。基类 ToJoinString 的 isSubQuery 守卫用
            // StartsWith("(") 判定，而 AsTables 包装与 GetPackTable 名带前导空格，守卫被绕过、
            // 提示滑到别名后。此处按 Trim 复判（复现基类 AsTables 名字替换），子查询源抑制
            // TableWithString 再调 base；提示保留在包装内，本引擎 hint 无脏读语义（MVCC）。
            var mode = VastbaseV5Mode.Require(this.Context);
            if (mode != null && mode.IsBabelfish
                && this.TableWithString != null && this.TableWithString != SqlWith.Null
                && IsSubqueryJoinSource(joinInfo))
            {
                var saved = this.TableWithString;
                this.TableWithString = SqlWith.Null;
                try { return base.ToJoinString(joinInfo); }
                finally { this.TableWithString = saved; }
            }
            return base.ToJoinString(joinInfo);
        }

        // 复演基类 ToJoinString 的 AsTables/MappingTables 名字替换后，按 Trim 判定
        // join 源是否子查询（基类 StartsWith("(") 无 Trim，见 QueryBuilder.ToJoinString isSubQuery）。
        // 只判左括号：GetPackTable 的成员 queryable 包（" (SELECT …) [别名] "）右端是别名
        // 不是右括号，双端判定会漏掉它——提示落在派生表别名后（42601）。普通表名
        // （括号/引号包裹的标识符）永不以左括号开头，单端判定无误报面。
        private bool IsSubqueryJoinSource(JoinQueryInfo joinInfo)
        {
            var name = joinInfo.TableName;
            if (this.AsTables.Any())
            {
                if (this.Context.MappingTables != null && this.Context.MappingTables.Any(it => it.DbTableName == name))
                    name = this.Context.MappingTables.First(it => it.DbTableName == name).EntityName;
                if (this.AsTables.Any(it => it.Key == name))
                    name = this.AsTables.First(it => it.Key == name).Value;
            }
            var trimmed = name == null ? null : name.Trim();
            return trimmed != null && trimmed.StartsWith("(");
        }

        // 基类 Union/_UnionAll 产出裸 unionTable 别名；Babelfish 下在此统一引号化（与标识符
        // 翻译一致）。PG 模式保持裸：整链裸别名自洽（大小写折叠匹配），只引号化表名会与
        // SELECT 投影失配。
        public override string GetTableNameString
        {
            get
            {
                var result = base.GetTableNameString;
                if (result != null && Mode != null && Mode.IsBabelfish)
                {
                    result = result.Replace(") unionTable ", ") [unionTable] ");
                }
                return result;
            }
        }

        public override string GetGroupByString
        {
            get
            {
                var result = base.GetGroupByString;
                if (result != null && !result.StartsWith(" "))
                    result = " " + result;
                return result;
            }
        }

        public override bool IsComplexModel(string sql)
        {
            var mode = Mode;
            if (mode != null && mode.IsBabelfish)
                return Regex.IsMatch(sql, @"AS \[\w+\.\w+\]") ||
                       Regex.IsMatch(sql, @"AS \[\w+\.\w+\.\w+\]");
            return base.IsComplexModel(sql);
        }

        private sealed class ToSqlSelectSnapshot
        {
            public bool HasValue;
            public string Value;
        }

        private ToSqlSelectSnapshot _toSqlSelectSnapshot;

        // PostgreSQL pagination evaluates GetSelectValue repeatedly. Reuse one value only
        // inside the current ToSqlString frame so subquery resolution cannot drift between
        // evaluations; the previous frame is restored for nested calls and no value survives
        // the outermost call.
        public override string GetSelectValue
        {
            get
            {
                var snapshot = _toSqlSelectSnapshot;
                if (snapshot == null)
                    return base.GetSelectValue;
                if (!snapshot.HasValue)
                {
                    snapshot.Value = base.GetSelectValue;
                    snapshot.HasValue = true;
                }
                return snapshot.Value;
            }
        }

        public override string ToSqlString()
        {
            var previousSnapshot = _toSqlSelectSnapshot;
            _toSqlSelectSnapshot = new ToSqlSelectSnapshot();
            try
            {
                var mode = Mode;
                if (mode != null && mode.IsBabelfish)
                {
                    // QueryableHelper only applies NOLOCK when AsTables.Count==1 (single table).
                    // For joins (Count>1), apply here before base generates SQL.
                    if (this.Context.CurrentConnectionConfig?.MoreSettings?.IsWithNoLockQuery == true
                        && this.TableWithString != SqlWith.Null
                        && this.AsTables != null && this.AsTables.Count >= 1)
                    {
                        foreach (var kv in this.AsTables.ToList())
                        {
                            if (!kv.Value.Contains(SqlWith.NoLock))
                            {
                                this.AsTables[kv.Key] = " (SELECT * FROM " + kv.Value + " " + SqlWith.NoLock + ")";
                            }
                        }
                    }
                }
                var sql = base.ToSqlString();
                if (mode != null && mode.IsBabelfish)
                {
                    // Babelfish 事后重写统一走 VastbaseV5BabelfishRewritePipeline.Apply 单一入口，
                    // 步骤顺序与形状漂移断言均在 pipeline 内声明。
                    sql = VastbaseV5BabelfishRewritePipeline.Apply(sql, new VastbaseV5BabelfishRewritePipeline.RewriteContext
                    {
                        HasGroupBy = this.GroupByValue.HasValue(),
                        Take = this.Take ?? 0,
                        Skip = this.Skip ?? 0,
                        OrderBy = this.OrderByValue,
                    });
                }
                return sql.TrimEnd();
            }
            finally
            {
                _toSqlSelectSnapshot = previousSnapshot;
            }
        }
    }
}