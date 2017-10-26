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
        public override bool IsComplexModel(string sql)
        {
            return Regex.IsMatch(sql, @"AS ""\w+\.\w+""");
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
            if (ExternalPageIndex > 0)
            {
                if (externalOrderBy.IsNullOrEmpty())
                {
                    externalOrderBy = " ORDER BY "+ this.Builder.SqlDateNow + " ";
                }
                result = string.Format("SELECT *,ROW_NUMBER() OVER({0}) AS RowIndex2 FROM ({1}) ExternalTable ", GetExternalOrderBy(externalOrderBy), result);
                result = ToPageSql2(result, ExternalPageIndex, ExternalPageSize, true);
            }
            this.OrderByValue = oldOrderBy;
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
