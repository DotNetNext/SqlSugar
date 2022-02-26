using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar.Access
{
    public class AccessQueryBuilder : QueryBuilder
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
            string oldOrderBy = this.OrderByValue;
            string externalOrderBy = oldOrderBy;
            var isIgnoreOrderBy = this.IsCount && this.PartitionByValue.IsNullOrEmpty();
            AppendFilter();
            sql = new StringBuilder();
            if (this.OrderByValue == null && (Skip != null || Take != null)) this.OrderByValue = " ORDER BY now() ";
            if (this.PartitionByValue.HasValue())
            {
                throw new Exception("sqlite no support partition by");
            }
            var isFirst = (Skip == 0 || Skip == null) && Take == 1 && DisableTop == false;
            var isRowNumber = (Skip != null || Take != null) && !isFirst;
            var isPage = isRowNumber;
            isRowNumber = false;
            var oldSkip = Skip;
            var oldTake = Take;
            Skip = null;
            Take = null;
            var rowNumberString = string.Format(",ROW_NUMBER() OVER({0}) AS RowIndex ", GetOrderByString);
            string groupByValue = GetGroupByString + HavingInfos;
            string orderByValue = (!isRowNumber && this.OrderByValue.HasValue()) ? GetOrderByString : null;
            if (isIgnoreOrderBy) { orderByValue = null; }
            sql.AppendFormat(SqlTemplate, isFirst ? (" TOP 1 " + GetSelectValue) : GetSelectValue, GetTableNameString, GetWhereValueString, groupByValue, orderByValue);
            sql.Replace(UtilConstants.ReplaceKey, isRowNumber ? (isIgnoreOrderBy ? null : rowNumberString) : null);
            if (isIgnoreOrderBy) { this.OrderByValue = oldOrderBy; return sql.ToString(); }
            var result = isFirst ? sql.ToString() : ToPageSql(sql.ToString(), this.Take, this.Skip);
            if (ExternalPageIndex > 0)
            {
                throw new Exception("sqlite no support partition by");
            }
            this.OrderByValue = oldOrderBy;
            if (!string.IsNullOrEmpty(this.Offset))
            {
                if (this.OrderByValue.IsNullOrEmpty())
                {
                    result += " ORDER BY now() ";
                    this.OrderByValue = " ORDER BY now() ";
                }
                result += this.Offset;
            }
            result = GetSqlQuerySql(result);
            if (isPage) 
            {
                var colums=this.Context.EntityMaintenance.GetEntityInfo(this.EntityType).Columns;
                if (!colums.Any(x => x.IsPrimarykey)) 
                {
                    throw new Exception("sqlite page need primary key , entity name: "+ this.EntityName);
                }
                var pkName =this.Builder.GetTranslationColumnName(colums.Where(x => x.IsPrimarykey).First().DbColumnName);
                var takeSql = $@" SELECT   { (oldTake == null ? "" : ("Top " + oldTake)) }
                *FROM({ result}) ACCTABLE1";
                var skipSql = $@"  
                      WHERE {pkName} NOT IN   
                    (SELECT    {  (oldSkip == null ? "" : ("Top " + oldSkip))} {pkName} FROM ({result}) ACCTABLE2  { this.OrderByValue })";
                if (oldSkip == 0)
                {
                    result = takeSql;
                }
                else 
                {
                    result = takeSql+skipSql;
                }

            }
            return result;
        }
    }
}
