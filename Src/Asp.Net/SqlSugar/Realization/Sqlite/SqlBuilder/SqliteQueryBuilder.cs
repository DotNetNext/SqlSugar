using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SqlSugar
{
    public partial class SqliteQueryBuilder : QueryBuilder
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
                return "ORDER BY DATETIME('now')   ";
            }
        }

        #endregion

        #region Common Methods
        public override bool IsComplexModel(string sql)
        {
            return Regex.IsMatch(sql, @"AS \`\w+\.\w+\`");
        }
        public override string ToSqlString()
        {
            base.AppendFilter();
            string result = null;
            string oldOrderBy = this.OrderByValue;
            sql = new StringBuilder();
            sql.AppendFormat(SqlTemplate, GetSelectValue, GetTableNameString, GetWhereValueString, GetGroupByString + HavingInfos, (Skip != null || Take != null) ? null : GetOrderByString);
            if (IsCount) { return sql.ToString(); }
            if (Skip != null && Take == null)
            {
                if (this.OrderByValue == "ORDER BY ") this.OrderByValue += GetSelectValue.Split(',')[0];
                result= string.Format(PageTempalte, GetSelectValue, GetTableNameString, GetWhereValueString, GetGroupByString + HavingInfos, (Skip != null || Take != null) ? null : GetOrderByString, Skip.ObjToInt(), long.MaxValue);
            }
            else if (Skip == null && Take != null)
            {
                if (this.OrderByValue == "ORDER BY ") this.OrderByValue += GetSelectValue.Split(',')[0];
                result= string.Format(PageTempalte, GetSelectValue, GetTableNameString, GetWhereValueString, GetGroupByString + HavingInfos, GetOrderByString, 0, Take.ObjToInt());
            }
            else if (Skip != null && Take != null)
            {
                if (this.OrderByValue == "ORDER BY ") this.OrderByValue += GetSelectValue.Split(',')[0];
                result= string.Format(PageTempalte, GetSelectValue, GetTableNameString, GetWhereValueString, GetGroupByString + HavingInfos, GetOrderByString, Skip.ObjToInt() > 0 ? Skip.ObjToInt(): 0, Take);
            }
            else
            {
                result= sql.ToString();
            }
            this.OrderByValue = oldOrderBy;
            return result;
        }
        
        #endregion

        #region Get SQL Partial
        public override string GetSelectValue
        {
            get
            {
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
                    this.SelectCacheKey = this.SelectCacheKey + string.Join("-", this.JoinQueryInfos.Select(it => it.TableName));
                }
                if (IsDistinct)
                {
                    reval = " DISTINCT " + reval;
                }
                return reval;
            }
        }

        #endregion
    }
}
