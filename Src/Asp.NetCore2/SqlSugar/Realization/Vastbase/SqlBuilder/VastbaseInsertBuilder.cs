using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar
{
    public class VastbaseV5InsertBuilder : PostgreSQLInsertBuilder
    {
        private VastbaseV5Mode Mode => VastbaseV5Mode.Require(this.Context);

        // 基类 InsertBuilder.GetExpressionValue 未设 SugarContext（QueryBuilder/UpdateBuilder/
        // DeleteBuilder 均已设），Babelfish 兼容模式(TDS) 探测不到 → 列名带 PG 引号 →
        // InsertColumns(expr)/IgnoreColumns(expr) 过滤后无列可插 → 回退零列 INSERT
        //（NOT NULL 违例）。此处补设后按模式走对应引号。
        public override ExpressionResult GetExpressionValue(Expression expression, ResolveExpressType resolveType)
        {
            if (this.LambdaExpressions != null)
            {
                this.LambdaExpressions.SugarContext = new ExpressionOutParameter() { Context = this.Context };
            }
            return base.GetExpressionValue(expression, resolveType);
        }

        public override string SqlTemplate
        {
            get
            {
                var mode = Mode;
                if (mode != null && mode.IsBabelfish && IsReturnIdentity)
                    return mode.InsertSqlTemplate;
                return base.SqlTemplate;
            }
        }

        public override Func<string, string, string> ConvertInsertReturnIdFunc
        {
            get
            {
                var mode = Mode;
                if (mode != null && mode.IsBabelfish)
                    return mode.ConvertInsertReturnIdFunc;
                return base.ConvertInsertReturnIdFunc;
            }
            set => base.ConvertInsertReturnIdFunc = value;
        }

        public override string ToSqlString()
        {
            var mode = Mode;
            if (mode == null || !mode.IsBabelfish)
                return base.ToSqlString();

            if (IsNoInsertNull)
            {
                DbColumnInfoList = DbColumnInfoList.Where(it => it.Value != null).ToList();
            }
            var groupList = DbColumnInfoList.GroupBy(it => it.TableId).ToList();
            if (groupList.Count == 0)
            {
                ActionMinDate();
                string cols = Builder.GetTranslationColumnName("Id");
                string col_params = "DEFAULT";
                return FixConflictSql(string.Format(SqlTemplate, GetTableNameString, cols, col_params));
            }
            var isSingle = groupList.Count() == 1;
            string columnsString = string.Join(",", groupList.First().Select(it => Builder.GetTranslationColumnName(it.DbColumnName)));
            if (isSingle)
            {
                string columnParametersString = string.Join(",", this.DbColumnInfoList.Select(it => GetDbColumn(it, Builder.SqlParameterKeyWord + it.DbColumnName)));
                ActionMinDate();
                return FixConflictSql(string.Format(SqlTemplate, GetTableNameString, columnsString, columnParametersString));
            }
            else
            {
                StringBuilder batchInsetrSql = new StringBuilder();
                int pageSize = 200;
                int pageIndex = 1;
                if (IsNoPage && IsReturnPkList)
                {
                    pageSize = groupList.Count;
                }
                int totalRecord = groupList.Count;
                int pageCount = (totalRecord + pageSize - 1) / pageSize;
                while (pageCount >= pageIndex)
                {
                    batchInsetrSql.AppendFormat(SqlTemplateBatch, GetTableNameString, columnsString);
                    int i = 0;
                    foreach (var columns in groupList.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList())
                    {
                        var isFirst = i == 0;
                        if (isFirst)
                        {
                            batchInsetrSql.Append(SqlTemplateBatchUnion);
                        }
                        batchInsetrSql.Append("\r\n ( " + string.Join(",", columns.Select(it =>
                        {
                            if (it.InsertServerTime || it.InsertSql.HasValue() || it.SqlParameterDbType is Type || it?.PropertyType?.Name == "DateOnly" || it?.PropertyType?.Name == "TimeOnly")
                            {
                                return GetDbColumn(it, null);
                            }
                            object value = null;
                            if (it.Value is DateTime)
                            {
                                var date = ((DateTime)it.Value);
                                if (date < System.Data.SqlTypes.SqlDateTime.MinValue.Value)
                                    date = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
                                else if (date == DateTime.MaxValue)
                                    date = System.Data.SqlTypes.SqlDateTime.MaxValue.Value;
                                value = date.ToString("yyyy-MM-dd HH:mm:ss.fff");
                            }
                            else if (it.Value is DateTimeOffset)
                            {
                                return FormatDateTimeOffset(it.Value);
                            }
                            else if (it.Value is decimal v)
                            {
                                return v.ToString(CultureInfo.InvariantCulture);
                            }
                            else if (it.IsArray && it.Value != null)
                            {
                                return FormatValue(it.Value, it.PropertyName, i, it);
                            }
                            else if (it.Value is byte[])
                            {
                                return FormatValue(it.Value, it.PropertyName, i, it);
                            }
                            else
                            {
                                value = it.Value;
                            }
                            if (value == null || value == DBNull.Value)
                            {
                                return string.Format(SqlTemplateBatchSelect, "NULL");
                            }
                            return string.Format(SqlTemplateBatchSelect, "'" + value.ObjToStringNoTrim().ToSqlFilter() + "'");
                        })) + "),");
                        ++i;
                    }
                    pageIndex++;
                    batchInsetrSql.Remove(batchInsetrSql.Length - 1, 1).Append("\r\n;\r\n");
                }
                return FixConflictSql(batchInsetrSql.ToString());
            }
        }

        public override string GetDbColumn(DbColumnInfo columnInfo, object name)
        {
            // InsertServerTime 显式分派方言：plain-entity 插入不触发表达式解析，base GetDate()
            // 按构造默认 PG 方言生成 NOW()，对 Babelfish 是错误方言——
            // 故 PG 用 NOW()、Babelfish 用 GETDATE()。
            if (columnInfo.InsertServerTime)
                return Mode.IsBabelfish ? "GETDATE()" : "NOW()";
            if (columnInfo.PropertyType == typeof(TimeSpan) || columnInfo.PropertyType == typeof(TimeSpan?))
            {
                if (columnInfo.Value == null || columnInfo.Value == DBNull.Value)
                    return "NULL";
                if (columnInfo.Value is TimeSpan ts)
                    return "'" + ts.ToString() + "'";
            }
            return base.GetDbColumn(columnInfo, name);
        }

        private string FixConflictSql(string sql)
        {
            if (this.ConflictNothing?.Any() == true)
                sql = sql.Replace(";", string.Format("  ON CONFLICT ({0}) DO NOTHING;", string.Join(",", this.ConflictNothing)));
            return sql;
        }
    }
}