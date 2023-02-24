using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SqlSugar.Odbc
{
    public class OdbcQueryBuilder: QueryBuilder
    {
        #region Sql Template
        public override string PageTempalte
        {
            get
            {
                /*
                 SELECT * FROM TABLE WHERE CONDITION ORDER BY ID DESC LIMIT 0,10
                 */
                var template = "SELECT {0} FROM {1} {2} {3} {4} LIMIT {5},{6}";
                return template;
            }
        }
        public override string DefaultOrderByTemplate
        {
            get
            {
                return "ORDER BY SysDate ";
            }
        }

        #endregion

        #region Common Methods
        public override bool IsComplexModel(string sql)
        {
            return Regex.IsMatch(sql, @"AS \`\w+\.\w+\`") || Regex.IsMatch(sql, @"AS \`\w+\.\w+\.\w+\`");
        }
        public override string ToSqlString()
        {
            base.AppendFilter();
            string oldOrderValue = this.OrderByValue;
            string result = null;
            sql = new StringBuilder();
            sql.AppendFormat(SqlTemplate, GetSelectValue, GetTableNameString, GetWhereValueString, GetGroupByString + HavingInfos, (Skip != null || Take != null) ? null : GetOrderByString);
            if (IsCount) { return sql.ToString(); }
            if (Skip != null && Take == null)
            {
                if (this.OrderByValue == "ORDER BY ") this.OrderByValue += GetSelectValue.Split(',')[0];
                result = string.Format(PageTempalte, GetSelectValue, GetTableNameString, GetWhereValueString, GetGroupByString + HavingInfos, (Skip != null || Take != null) ? null : GetOrderByString, Skip.ObjToInt(), long.MaxValue);
            }
            else if (Skip == null && Take != null)
            {
                if (this.OrderByValue == "ORDER BY ") this.OrderByValue += GetSelectValue.Split(',')[0];
                result = string.Format(PageTempalte, GetSelectValue, GetTableNameString, GetWhereValueString, GetGroupByString + HavingInfos, GetOrderByString, 0, Take.ObjToInt());
            }
            else if (Skip != null && Take != null)
            {
                if (Skip == 0 && Take == 1 && this.OrderByValue == "ORDER BY SysDate ")
                {
                    this.OrderByValue = null;
                }
                if (this.OrderByValue == "ORDER BY ") this.OrderByValue += GetSelectValue.Split(',')[0];
                result = string.Format(PageTempalte, GetSelectValue, GetTableNameString, GetWhereValueString, GetGroupByString + HavingInfos, GetOrderByString, Skip.ObjToInt() > 0 ? Skip.ObjToInt() : 0, Take);
            }
            else
            {
                result = sql.ToString();
            }
            this.OrderByValue = oldOrderValue;
            result = GetSqlQuerySql(result);
            if (result.IndexOf("-- No table") > 0)
            {
                return "-- No table";
            }
            if (TranLock != null)
            {
                result = result + TranLock;
            }
            return result;
        }
        private string ToCountSqlString()
        {
            //base.AppendFilter();
            string oldOrderValue = this.OrderByValue;
            string result = null;
            sql = new StringBuilder();
            sql.AppendFormat(SqlTemplate, "Count(*)", GetTableNameString, GetWhereValueString, GetGroupByString + HavingInfos, (Skip != null || Take != null) ? null : GetOrderByString);
            if (IsCount)
            {
                if (sql.ToString().Contains("-- No table"))
                {
                    return "-- No table";
                }
                return sql.ToString();
            }
            if (Skip != null && Take == null)
            {
                if (this.OrderByValue == "ORDER BY ") this.OrderByValue += GetSelectValue.Split(',')[0];
                result = string.Format(PageTempalte, GetSelectValue, GetTableNameString, GetWhereValueString, GetGroupByString + HavingInfos, (Skip != null || Take != null) ? null : GetOrderByString, Skip.ObjToInt(), long.MaxValue);
            }
            else if (Skip == null && Take != null)
            {
                if (this.OrderByValue == "ORDER BY ") this.OrderByValue += GetSelectValue.Split(',')[0];
                result = string.Format(PageTempalte, GetSelectValue, GetTableNameString, GetWhereValueString, GetGroupByString + HavingInfos, GetOrderByString, 0, Take.ObjToInt());
            }
            else if (Skip != null && Take != null)
            {
                if (this.OrderByValue == "ORDER BY ") this.OrderByValue += GetSelectValue.Split(',')[0];
                result = string.Format(PageTempalte, GetSelectValue, GetTableNameString, GetWhereValueString, GetGroupByString + HavingInfos, GetOrderByString, Skip.ObjToInt() > 0 ? Skip.ObjToInt() : 0, Take);
            }
            else
            {
                result = sql.ToString();
            }
            this.OrderByValue = oldOrderValue;
            return result;
        }
        public override string ToCountSql(string sql)
        {
            if (this.GroupByValue.HasValue() || this.IsDistinct)
            {
                return base.ToCountSql(sql);
            }
            else
            {
                return ToCountSqlString();
            }
        }
        #endregion

        #region Get SQL Partial
        public override string GetSelectValue
        {
            get
            {
                string result = string.Empty;
                if (this.SelectValue == null || this.SelectValue is string)
                {
                    result = GetSelectValueByString();
                }
                else
                {
                    result = GetSelectValueByExpression();
                }
                if (this.SelectType == ResolveExpressType.SelectMultiple)
                {
                    this.SelectCacheKey = this.SelectCacheKey + string.Join("-", this.JoinQueryInfos.Select(it => it.TableName));
                }
                if (IsDistinct)
                {
                    result = " DISTINCT " + result;
                }
                if (this.SubToListParameters != null && this.SubToListParameters.Any())
                {
                    result = SubToListMethod(result);
                }
                return result;
            }
        }

        #endregion
    }
}
