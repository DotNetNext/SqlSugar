using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SqlSugar
{
    public class OracleQueryBuilder : QueryBuilder
    {
        public override bool IsSelectNoAll { get; set; } = true;
        public override bool IsComplexModel(string sql)
        {
            return Regex.IsMatch(sql, @"AS ""\w+\.\w+""")|| Regex.IsMatch(sql, @"AS ""\w+\.\w+\.\w+""");
        }
        public override string SqlTemplate
        {
            get
            {
                return "SELECT {0}{"+UtilConstants.ReplaceKey+"} FROM {1}{2}{3}{4}";
            }
        }
        public override string ToSqlString()
        {
            if (this.Offset == "true") 
            {
                return OffsetPage();
            }
            var oldTake = Take;
            var oldSkip = Skip;
            var isDistinctPage = IsDistinct && (Take > 1 || Skip > 1);
            if (isDistinctPage)
            {
                return OffsetPage();
            }
            var result = _ToSqlString();
            //if (isDistinctPage)
            //{
            //    if (this.OrderByValue.HasValue())
            //    {
            //        Take = int.MaxValue;
            //        result = result.Replace("DISTINCT", $" DISTINCT TOP {int.MaxValue} ");
            //    }
            //    Take = oldTake;
            //    Skip = oldSkip;
            //    result = this.Context.SqlQueryable<object>(result).Skip(Skip??0).Take(Take??0).ToSql().Key;


            //}
            if (TranLock != null)
            {
                result = result + TranLock;
            }
            return result;
        }

        private string OffsetPage()
        {
            var skip = this.Skip??1;
            var take = this.Take;
            this.Skip = null;
            this.Take = null;
            this.Offset = null;
            var pageSql = $"SELECT * FROM ( SELECT PAGETABLE1.*,ROWNUM PAGEINDEX FROM( { this.ToSqlString() }) PAGETABLE1 WHERE ROWNUM<={skip+take}) WHERE PAGEINDEX>={(skip==0?skip:(skip+1))}";
            return pageSql;
        }

        public  string _ToSqlString()
        {
            string oldOrderBy = this.OrderByValue;
            string externalOrderBy = oldOrderBy;
            var isIgnoreOrderBy = this.IsCount && this.PartitionByValue.IsNullOrEmpty();
            AppendFilter();
            sql = new StringBuilder();
            if (this.OrderByValue == null && (Skip != null || Take != null)) this.OrderByValue = " ORDER BY "+ this.Builder.SqlDateNow + " ";
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
            if (this.GetGroupByString==null&&this.Take == 1 && this.Skip == 0&&oldOrderBy== "ORDER BY sysdate ") 
            {
                result = $" {sql.ToString()} {(this.WhereInfos.Any()?"AND":"WHERE")}   ROWNUM = 1 ";
                result = result.Replace(rowNumberString, " ");
            }
            if (ExternalPageIndex > 0)
            {
                if (externalOrderBy.IsNullOrEmpty())
                {
                    externalOrderBy = " ORDER BY "+ this.Builder.SqlDateNow + " ";
                }
                result = string.Format("SELECT ExternalTable.*,ROW_NUMBER() OVER({0}) AS RowIndex2 FROM ({1}) ExternalTable ", GetExternalOrderBy(externalOrderBy), result);
                result = ToPageSql2(result, ExternalPageIndex, ExternalPageSize, true);
            }
            this.OrderByValue = oldOrderBy;
            result = GetSqlQuerySql(result);
            if (result.IndexOf("-- No table") > 0)
            {
                return "-- No table";
            }
            return result;
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
