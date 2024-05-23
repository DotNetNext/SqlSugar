using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SqlSugar
{
    public partial class DmQueryBuilder : QueryBuilder
    {
        public override bool IsComplexModel(string sql)
        {
            return Regex.IsMatch(sql, @"AS ""\w+\.\w+""")|| Regex.IsMatch(sql, @"AS ""\w+\.\w+\.\w+""");
        }
        public override string SqlTemplate
        {
            get
            {
                return "SELECT {0}{" + UtilConstants.ReplaceKey + "} FROM {1}{2}{3}{4}";
            }
        }
        public override string ToSqlString()
        {
            //Support MySql Model
            if (this.Context.CurrentConnectionConfig.MoreSettings?.DatabaseModel == DbType.MySql) 
            {
                return MySqlToSqlString();
            }
            var isDistinctPage = IsDistinct && (Take > 1 || Skip > 1);
            if (isDistinctPage)
            {
                return OffsetPage();
            }

            string oldOrderBy = this.OrderByValue;
            string externalOrderBy = oldOrderBy;
            var isIgnoreOrderBy = this.IsCount && this.PartitionByValue.IsNullOrEmpty();
            AppendFilter();
            sql = new StringBuilder();
            if (this.OrderByValue == null && (Skip != null || Take != null)) this.OrderByValue = " ORDER BY " + this.Builder.SqlDateNow + " ";
            if (this.PartitionByValue.HasValue())
            {
                this.OrderByValue = this.PartitionByValue + this.OrderByValue;
            }
            var isRowNumber = Skip != null || Take != null;
            var rowNumberString = string.Format(",ROW_NUMBER() OVER({0}) AS RowIndex ", GetOrderByString);
            string groupByValue = GetGroupByString + HavingInfos;
            string orderByValue = (!isRowNumber && this.OrderByValue.HasValue()) ? GetOrderByString : null;
            if (isIgnoreOrderBy) { orderByValue = null; }
            sql.AppendFormat(SqlTemplate, GetSelectValue, GetTableNameString, GetWhereValueString, groupByValue, orderByValue);
            sql.Replace(UtilConstants.ReplaceKey, isRowNumber ? (isIgnoreOrderBy ? null : rowNumberString) : null);
            if (isIgnoreOrderBy) { this.OrderByValue = oldOrderBy; return sql.ToString(); }
            var result = ToPageSql(sql.ToString(), this.Take, this.Skip);
            if (ExternalPageIndex > 0)
            {
                if (externalOrderBy.IsNullOrEmpty())
                {
                    externalOrderBy = " ORDER BY " + this.Builder.SqlDateNow + " ";
                }
                result = string.Format("SELECT *,ROW_NUMBER() OVER({0}) AS RowIndex2 FROM ({1}) ExternalTable ", GetExternalOrderBy(externalOrderBy), result);
                result = ToPageSql2(result, ExternalPageIndex, ExternalPageSize, true);
            }
            this.OrderByValue = oldOrderBy;
            result = GetSqlQuerySql(result);
            if (result.Contains("-- No table") )
            {
                return "select * from (select 1 as id) where id=0 -- No table";
            }
            return result;
        }

        public string MySqlToSqlString()
        {
            var PageTempalte = "SELECT {0} FROM {1} {2} {3} {4} LIMIT {5},{6}";
            var SqlTemplate = MySqlTemplate;
            base.AppendFilter();
            string result = null;
            string oldOrderBy = this.OrderByValue;
            sql = new StringBuilder();
            sql.AppendFormat(SqlTemplate, GetMySelectValue, GetTableNameString, GetWhereValueString, GetGroupByString + HavingInfos, (Skip != null || Take != null) ? null : GetOrderByString);
            if (IsCount) { return sql.ToString(); }
            if (Skip != null && Take == null)
            {
                if (this.OrderByValue == "ORDER BY ") this.OrderByValue += GetMySelectValue.Split(',')[0];
                result = string.Format(PageTempalte, GetMySelectValue, GetTableNameString, GetWhereValueString, GetGroupByString + HavingInfos, (Skip != null || Take != null) ? null : GetOrderByString, Skip.ObjToInt(), long.MaxValue);
            }
            else if (Skip == null && Take != null)
            {
                if (this.OrderByValue == "ORDER BY ") this.OrderByValue += GetMySelectValue.Split(',')[0];
                result = string.Format(PageTempalte, GetMySelectValue, GetTableNameString, GetWhereValueString, GetGroupByString + HavingInfos, GetOrderByString, 0, Take.ObjToInt());
            }
            else if (Skip != null && Take != null)
            {
                if (this.OrderByValue == "ORDER BY ") this.OrderByValue += GetMySelectValue.Split(',')[0];
                result = string.Format(PageTempalte, GetMySelectValue, GetTableNameString, GetWhereValueString, GetGroupByString + HavingInfos, GetOrderByString, Skip.ObjToInt() > 0 ? Skip.ObjToInt() : 0, Take);
            }
            else
            {
                result = sql.ToString();
            }
            this.OrderByValue = oldOrderBy;
            result = GetSqlQuerySql(result);
            result = result.Replace(UtilConstants.ReplaceCommaKey, "");
            if (result.IndexOf("-- No table") > 0)
            {
                return "-- No table";
            }
            return result;
        }
        public string GetMySelectValue
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
                if (this.SubToListParameters != null && this.SubToListParameters.Any())
                {
                    reval = SubToListMethod(reval);
                }
                return reval;
            }
        }
        public  string MySqlTemplate
        {
            get
            {
                if (this.SampleBy.HasValue())
                {
                    return "SELECT {0} FROM {1}{2} " + this.SampleBy + " {3}{4}";
                }
                return "SELECT {0} FROM {1}{2}{3}{4} ";
            }
        }
        private string OffsetPage()
        {
            var skip = this.Skip ?? 1;
            var take = this.Take;
            this.Skip = null;
            this.Take = null;
            this.Offset = null;
            var pageSql = $"SELECT * FROM ( SELECT PAGETABLE1.*,ROWNUM PAGEINDEX FROM( {this.ToSqlString()}) PAGETABLE1 WHERE ROWNUM<={skip + take}) WHERE PAGEINDEX>={(skip == 0 ? skip : (skip + 1))}";
            return pageSql;
        }
        public override string ToPageSql(string sql, int? take, int? skip, bool isExternal = false)
        {
            string temp = isExternal ? ExternalPageTempalte : PageTempalte;
            if (skip != null && take == null)
            {
                return string.Format(temp, sql.ToString(), skip.ObjToInt() + 1, long.MaxValue);
            }
            else if (skip == null && take != null)
            {
                return string.Format(temp, sql.ToString(), 1, take.ObjToInt());
            }
            else if (skip != null && take != null)
            {
                return string.Format(temp, sql.ToString(), skip.ObjToInt() + 1, skip.ObjToInt() + take.ObjToInt());
            }
            else
            {
                return sql.ToString();
            }
        }

        public override string ToPageSql2(string sql, int? pageIndex, int? pageSize, bool isExternal = false)
        {
            string temp = isExternal ? ExternalPageTempalte : PageTempalte;
            return string.Format(temp, sql.ToString(), (pageIndex - 1) * pageSize + 1, pageIndex * pageSize);
        }
    }
}
