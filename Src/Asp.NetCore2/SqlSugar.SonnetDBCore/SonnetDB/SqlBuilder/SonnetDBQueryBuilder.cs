using System;
using System.Text;
using System.Text.RegularExpressions;

namespace SqlSugar.SonnetDB
{
    public class SonnetDBQueryBuilder : QueryBuilder
    {
        public override string PageTempalte => "SELECT {0} FROM {1} {2} {3} {4} LIMIT {6} OFFSET {5}";
        public override string DefaultOrderByTemplate => "ORDER BY CURRENT_UTC_DATETIME() ";

        public override bool IsComplexModel(string sql)
        {
            return Regex.IsMatch(sql, @"AS ""\w+\.\w+(?:\.\w+)?""");
        }

        public override string ToSqlString()
        {
            AppendFilter();
            var select = GetSelectValue;
            var orderBy = GetOrderByString;
            var isPartitionPage = !string.IsNullOrEmpty(PartitionByValue) && (Skip != null || Take != null);
            if (isPartitionPage)
            {
                select += ",ROW_NUMBER() OVER(" + PartitionByValue + " " + (orderBy ?? DefaultOrderByTemplate) + ") AS RowIndex";
                orderBy = null;
            }

            sql = new StringBuilder();
            sql.AppendFormat(SqlTemplate, select, GetTableNameString, GetWhereValueString, GetGroupByString + HavingInfos, orderBy);
            var result = sql.ToString();
            if (IsCount && string.IsNullOrEmpty(PartitionByValue))
            {
                return result;
            }

            if (isPartitionPage)
            {
                result = string.Format(base.PageTempalte, result, (Skip ?? 0) + 1, Take == null ? long.MaxValue : (long)(Skip ?? 0) + Take.Value);
            }
            else
            {
                result = ToPageSql(result, Take, Skip);
            }

            if (ExternalPageIndex > 0)
            {
                result = ToPageSql("SELECT * FROM (" + result + ") ExternalTable " + GetExternalOrderBy(GetOrderByString ?? DefaultOrderByTemplate), ExternalPageSize, (ExternalPageIndex - 1) * ExternalPageSize);
            }

            result = GetSqlQuerySql(result);
            return result.Contains("-- No table", StringComparison.Ordinal) ? "-- No table" : result + TranLock;
        }

        public override string ToPageSql(string sql, int? take, int? skip, bool isExternal = false)
        {
            if (take != null)
            {
                sql += " LIMIT " + take.Value;
            }
            if (skip != null)
            {
                sql += " OFFSET " + skip.Value;
            }
            return sql;
        }
    }
}
