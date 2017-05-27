using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;

namespace SqlSugar
{
    public partial class MySqlQueryBuilder : QueryBuilder
    {
        #region Sql Template
        public override string SqlTemplate
        {
            get
            {
                return "SELECT {0} FROM {1}{2}{3}{4} ";
            }
        }
        public override string JoinTemplate
        {
            get
            {
                return "{0}JOIN {1}{2}ON {3} ";
            }
        }
        public override string PageTempalte
        {
            get
            {
                /*
                 @"WITH PageTable AS(
                          {0}
                  )
                  SELECT * FROM (SELECT *,ROW_NUMBER() OVER({1}) AS RowIndex FROM PageTable ) T WHERE RowIndex BETWEEN {2} AND {3}"
                 */
                var template = "SELECT {0} FROM {1} {2} {3} {4} LIMIT {5},{6}";
                return template;
            }
        }
        public override string DefaultOrderByTemplate
        {
            get
            {
                return "ORDER BY NOW() ";
            }
        }
        public override string OrderByTemplate
        {
            get
            {
                return "ORDER BY ";
            }
        }
        public override string GroupByTemplate
        {
            get
            {
                return "GROUP BY ";
            }
        }

        public override string MaxTemplate
        {
            get
            {
                return "MAX({0})";
            }
        }
        public override string MinTemplate
        {
            get
            {
                return "MIN({0})";
            }
        }
        public override string SumTemplate
        {
            get
            {
                return "SUM({0})";
            }
        }
        public override string AvgTemplate
        {
            get
            {
                return "AVG({0})";
            }
        }
        public override string InTemplate
        {
            get
            {
                return "{0} IN ({1}) ";
            }
        }
        #endregion

        #region Common Methods
        public override bool IsSingle()
        {
            var isSingle = Builder.QueryBuilder.JoinQueryInfos.IsNullOrEmpty();
            return isSingle;
        }
        public override ExpressionResult GetExpressionValue(Expression expression, ResolveExpressType resolveType)
        {
            ILambdaExpressions resolveExpress = this.LambdaExpressions;
            this.LambdaExpressions.Clear();
            resolveExpress.JoinQueryInfos = Builder.QueryBuilder.JoinQueryInfos;
            resolveExpress.MappingColumns = Context.MappingColumns;
            resolveExpress.MappingTables = Context.MappingTables;
            resolveExpress.IgnoreComumnList = Context.IgnoreColumns;
            resolveExpress.Resolve(expression, resolveType);
            this.Parameters = new List<SugarParameter>();
            this.Parameters.AddRange(resolveExpress.Parameters);
            var reval = resolveExpress.Result;
            return reval;
        }
        public override string ToSqlString()
        {
            sql = new StringBuilder();
            sql.AppendFormat(SqlTemplate, GetSelectValue, GetTableNameString, GetWhereValueString, GetGroupByString + HavingInfos, (Skip != null || Take != null) ? null : GetOrderByString);
            if (IsCount) { return sql.ToString(); }
            if (Skip != null && Take == null)
            {
                if (this.OrderByValue == "ORDER BY ") this.OrderByValue += GetSelectValue.Split(',')[0];
                return string.Format(PageTempalte, GetSelectValue, GetTableNameString, GetWhereValueString, GetGroupByString + HavingInfos, (Skip != null || Take != null) ? null : GetOrderByString, Skip.ObjToInt() + 1, long.MaxValue);
            }
            else if (Skip == null && Take != null)
            {
                if (this.OrderByValue == "ORDER BY ") this.OrderByValue += GetSelectValue.Split(',')[0];
                return string.Format(PageTempalte, GetSelectValue, GetTableNameString, GetWhereValueString, GetGroupByString + HavingInfos, GetOrderByString, 1, Take.ObjToInt());
            }
            else if (Skip != null && Take != null)
            {
                if (this.OrderByValue == "ORDER BY ") this.OrderByValue += GetSelectValue.Split(',')[0];
                return string.Format(PageTempalte, GetSelectValue, GetTableNameString, GetWhereValueString, GetGroupByString + HavingInfos, GetOrderByString, Skip.ObjToInt() > 0 ? Skip.ObjToInt() + 1 : 0, Take);
            }
            else
            {
                return sql.ToString();
            }

        }
        public override string ToJoinString(JoinQueryInfo joinInfo)
        {
            return string.Format(
                this.JoinTemplate,
                joinInfo.JoinType.ToString() + PubConst.Space,
                joinInfo.TableName + PubConst.Space,
                joinInfo.ShortName + PubConst.Space + joinInfo.TableWithString,
                joinInfo.JoinWhere);
        }
        public override void Clear()
        {
            this.Skip = 0;
            this.Take = 0;
            this.sql = null;
            this.WhereIndex = 0;
            this.Parameters = null;
            this.GroupByValue = null;
            this.WhereInfos = null;
            this._TableNameString = null;
            this.JoinQueryInfos = null;
        }
        public override bool IsComplexModel(string sql)
        {
            return Regex.IsMatch(sql, @"AS \[\w+\.\w+\]");
        }

        
        #endregion

        #region Get SQL Partial
        public override string GetSelectValue
        {
            get
            {
                if (this.IsCount) return "COUNT(1) AS `Count` ";
                string reval = string.Empty;
                if (this.SelectValue == null || this.SelectValue is string)
                {
                    reval = GetSelectValueByString();
                }
                else
                {
                    reval = GetSelectValueByExpression();
                }
                if (this.SelectType == ResolveExpressType.SelectMultiple)
                {
                    this.SelectCacheKey = this.SelectCacheKey + string.Join("-", this._JoinQueryInfos.Select(it => it.TableName));
                }
                return reval;
            }
        }
        public override string GetSelectValueByExpression()
        {
            var expression = this.SelectValue as Expression;
            var reval = GetExpressionValue(expression, this.SelectType).GetResultString();
            this.SelectCacheKey = reval;
            return reval;
        }
        public override string GetSelectValueByString()
        {
            string reval;
            if (this.SelectValue.IsNullOrEmpty())
            {
                string pre = null;
                if (this.JoinQueryInfos.IsValuable() && this.JoinQueryInfos.Any(it => TableShortName.IsValuable()))
                {
                    pre = Builder.GetTranslationColumnName(TableShortName) + ".";
                }
                reval = string.Join(",", this.Context.EntityProvider.GetEntityInfo(this.EntityType).Columns.Where(it => !it.IsIgnore).Select(it => pre + Builder.GetTranslationColumnName(it.EnitytName, it.PropertyName)));
            }
            else
            {
                reval = this.SelectValue.ObjToString();
                this.SelectCacheKey = reval;
            }

            return reval;
        }
        public override string GetWhereValueString
        {
            get
            {
                if (this.WhereInfos == null) return null;
                else
                {
                    return string.Join(PubConst.Space, this.WhereInfos);
                }
            }
        }
        public override string GetJoinValueString
        {
            get
            {
                if (this.JoinQueryInfos.IsNullOrEmpty()) return null;
                else
                {
                    return string.Join(PubConst.Space, this.JoinQueryInfos.Select(it => this.ToJoinString(it)));
                }
            }
        }
        public override string GetTableNameString
        {
            get
            {
                var result = Builder.GetTranslationTableName(EntityName);
                result += PubConst.Space;
                if (this.TableWithString.IsValuable())
                {
                    result += TableWithString + PubConst.Space;
                }
                if (this.TableShortName.IsValuable())
                {
                    result += (TableShortName + PubConst.Space);
                }
                if (!this.IsSingle())
                {
                    result += GetJoinValueString + PubConst.Space;
                }
                return result;
            }
        }
        public override string GetOrderByString
        {
            get
            {
                return this.OrderByValue;
            }
        }
        public override string GetGroupByString
        {
            get
            {
                return this.GroupByValue;
            }
        }
        #endregion
    }
}
