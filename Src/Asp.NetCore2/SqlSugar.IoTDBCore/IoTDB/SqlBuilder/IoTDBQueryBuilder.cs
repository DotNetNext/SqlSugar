using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlSugar;

namespace SqlSugar.IoTDB
{
    public sealed class IoTDBQueryBuilder : QueryBuilder
    {
        public override string PageTempalte => "SELECT {0} FROM {1} {2} {3} {4} LIMIT {6} OFFSET {5}";

        public override string GetSelectValue
        {
            get
            {
                var value = base.GetSelectValue;
                var projection = value.Trim();
                var modifier = string.Empty;
                if (projection.StartsWith("DISTINCT ", StringComparison.OrdinalIgnoreCase))
                {
                    modifier = projection.Substring(0, "DISTINCT ".Length);
                    projection = projection.Substring(modifier.Length);
                }
                if (projection == "*") return value;

                var columns = SplitSelectList(projection);
                var retained = columns.Where(column => !IsImplicitTimeProjection(column)).ToList();
                if (retained.Count == columns.Count) return value;
                if (retained.Count == 0)
                    throw new NotSupportedException("IoTDB returns time implicitly; select at least one measurement when querying timestamps.");
                return modifier + string.Join(",", retained);
            }
        }

        public override string ToSqlString()
        {
            if (JoinQueryInfos.Any())
                throw new NotSupportedException("IoTDB tree model queries do not support relational joins.");

            AppendFilter();
            var statement = new StringBuilder();
            var orderBy = this.OrderByValue;
            statement.AppendFormat("SELECT {0} FROM {1} {2} {3} {4}",
                GetSelectValue,
                GetTableNameString,
                GetWhereValueString,
                GetGroupByString + HavingInfos,
                (Skip.HasValue || Take.HasValue) ? string.Empty : GetOrderByString);
            var sql = statement.ToString();
            if (IsCount || (!Skip.HasValue && !Take.HasValue)) return GetSqlQuerySql(sql);

            var limit = Take ?? int.MaxValue;
            var offset = Skip.GetValueOrDefault();
            if (limit < 0 || offset < 0) throw new ArgumentOutOfRangeException("IoTDB LIMIT/OFFSET values must be non-negative.");
            if (string.IsNullOrWhiteSpace(orderBy) || orderBy.Trim().Equals("ORDER BY now()", StringComparison.OrdinalIgnoreCase))
                orderBy = "ORDER BY time";
            sql = string.Format("SELECT {0} FROM {1} {2} {3} {4} {5} LIMIT {6} OFFSET {7}",
                GetSelectValue, GetTableNameString, GetWhereValueString, GetGroupByString + HavingInfos,
                orderBy, string.Empty, limit, offset);
            return GetSqlQuerySql(sql);
        }

        private static bool IsImplicitTimeProjection(string projection)
        {
            var expression = projection.Trim();
            var aliasIndex = expression.LastIndexOf(" AS ", StringComparison.OrdinalIgnoreCase);
            if (aliasIndex >= 0) expression = expression.Substring(0, aliasIndex).Trim();
            expression = expression.Trim('`', '"', '[', ']');
            var finalNode = expression.Substring(expression.LastIndexOf('.') + 1).Trim('`', '"', '[', ']');
            return finalNode.Equals("time", StringComparison.OrdinalIgnoreCase) ||
                finalNode.Equals("timestamp", StringComparison.OrdinalIgnoreCase);
        }

        private static List<string> SplitSelectList(string projection)
        {
            var result = new List<string>();
            var start = 0;
            var depth = 0;
            var quote = '\0';
            for (var index = 0; index < projection.Length; index++)
            {
                var current = projection[index];
                if (quote != '\0')
                {
                    if (current == quote)
                    {
                        if (index + 1 < projection.Length && projection[index + 1] == quote) index++;
                        else quote = '\0';
                    }
                    else if (quote == ']' && current == ']')
                    {
                        quote = '\0';
                    }
                    continue;
                }

                if (current == '\'' || current == '"' || current == '`') quote = current;
                else if (current == '[') quote = ']';
                else if (current == '(') depth++;
                else if (current == ')') depth--;
                else if (current == ',' && depth == 0)
                {
                    result.Add(projection.Substring(start, index - start).Trim());
                    start = index + 1;
                }
            }
            result.Add(projection.Substring(start).Trim());
            return result;
        }
    }
}
