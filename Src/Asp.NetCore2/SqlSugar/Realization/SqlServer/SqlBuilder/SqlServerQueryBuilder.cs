using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public class SqlServerQueryBuilder: QueryBuilder
    {
        public override string SqlTemplate
        {
            get
            {
                return "SELECT {0}{"+UtilConstants.ReplaceKey+"} FROM {1}{2}{3}{4}";
            }
        }
        public override string ToSqlString() 
        {
            var oldTake = Take;
            var oldSkip = Skip;
            var isDistinctPage = IsDistinct && (Take > 1 || Skip > 1);
            if (isDistinctPage) 
            {
                Take = null;
                Skip = null;
            }
            var result = _ToSqlString();
            if (isDistinctPage) 
            {
                if (this.OrderByValue.HasValue())
                {
                    Take = int.MaxValue;
                    result = result.Replace("DISTINCT", $" DISTINCT TOP {int.MaxValue} ");
                }
                Take = oldTake;
                Skip = oldSkip;
                result =this.Context.SqlQueryable<object>(result).Skip(Skip??0).Take(Take??0).ToSql().Key;
                

            }
            if (IsDistinct && oldTake == 1 && oldSkip == null) 
            {
                result = result.Replace(" TOP 1  DISTINCT", " TOP 1 ");
            }
            if (result.IndexOf("-- No table") > 0) 
            {
                return  "-- No table";
            }
            return result;
        }
        public string _ToSqlString()
        {
            string oldOrderBy = this.OrderByValue;
            string externalOrderBy = oldOrderBy;
            var isIgnoreOrderBy = this.IsCount && this.PartitionByValue.IsNullOrEmpty();
            AppendFilter();
            sql = new StringBuilder();
            var oldOrderByValue = this.OrderByValue;
            if (this.OrderByValue == null && (Skip != null || Take != null)) this.OrderByValue = " ORDER BY GetDate() ";
            if (this.PartitionByValue.HasValue())
            {
                this.OrderByValue = this.PartitionByValue + this.OrderByValue;
            }
            var isFirst = (Skip == 0 || Skip == null) && Take == 1 && DisableTop == false;
            var isTop = (Skip == null && Take != null && DisableTop == false);
            var isRowNumber = (Skip != null || Take != null) && !isFirst && !isTop;
            if (!isRowNumber && oldOrderByValue == null) { this.OrderByValue = null; }
            if (isFirst && oldOrderByValue == "ORDER BY GETDATE() ") { this.OrderByValue = null; }
            var rowNumberString = string.Format(",ROW_NUMBER() OVER({0}) AS RowIndex ", GetOrderByString);
            string groupByValue = GetGroupByString + HavingInfos;
            string orderByValue = (!isRowNumber && this.OrderByValue.HasValue()) ? GetOrderByString : null;
            if (isIgnoreOrderBy) { orderByValue = null; }
            sql.AppendFormat(SqlTemplate, GetSelect(isFirst,isTop), base.GetTableNameString, base.GetWhereValueString, groupByValue, orderByValue);
            sql.Replace(UtilConstants.ReplaceKey, isRowNumber ? (isIgnoreOrderBy ? null : rowNumberString) : null);
            if (isIgnoreOrderBy) { this.OrderByValue = oldOrderBy; return sql.ToString(); }
            var result = (isFirst || isTop) ? sql.ToString() : ToPageSql(sql.ToString(), this.Take, this.Skip);
            if (ExternalPageIndex > 0)
            {
                if (externalOrderBy.IsNullOrEmpty())
                {
                    externalOrderBy = " ORDER BY GetDate() ";
                }
                result = string.Format("SELECT *,ROW_NUMBER() OVER({0}) AS RowIndex2 FROM ({1}) ExternalTable ", GetExternalOrderBy(externalOrderBy), result);
                result = ToPageSql2(result, ExternalPageIndex, ExternalPageSize, true);
            }
            this.OrderByValue = oldOrderBy;
            if (!string.IsNullOrEmpty(this.Offset))
            {
                if (this.OrderByValue.IsNullOrEmpty())
                {
                    result += " ORDER BY GETDATE() ";
                    if (this.OldSql.HasValue())
                        this.OldSql += " ORDER BY GETDATE() ";
                }
                else
                {
                    if (this.OldSql.HasValue())
                        this.OldSql += (" " + this.GetOrderByString);
                }
                result += this.Offset;

                if (this.OldSql.HasValue())
                    this.OldSql += this.Offset;
            }
            result = GetSqlQuerySql(result);
            if (isFirst && IsDistinct) 
            {
                result = result.Replace("TOP 1  DISTINCT", "TOP 1 ");
            }
            return result;
        }

        private string GetSelect(bool isFirst,bool isTop)
        {
            if (isFirst) 
            {
                return (" TOP 1 " + GetSelectValue);
            }
            else if(isTop)
            {
                return ($" TOP {this.Take} " + GetSelectValue);
            }
            else 
            { 
                return GetSelectValue;
            }
        }
    }
}
