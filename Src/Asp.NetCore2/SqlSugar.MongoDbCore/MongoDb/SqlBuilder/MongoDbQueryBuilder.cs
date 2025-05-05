using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace SqlSugar.MongoDb
{
    public partial class MongoDbQueryBuilder : QueryBuilder
    {
        #region Sql Template
        public override string PageTempalte
        {
            get
            {
                /*
                 SELECT * FROM TABLE WHERE CONDITION ORDER BY ID DESC LIMIT 10 offset 0
                 */
                var template = "SELECT {0} FROM {1} {2} {3} {4} LIMIT {6} offset {5}";
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

        #endregion

        #region Common Methods
        public override string GetTableNameString
        {
            get
            {
                if (this.TableShortName != null&&this.Context.CurrentConnectionConfig?.MoreSettings?.PgSqlIsAutoToLower==false) 
                {
                    this.TableShortName = Builder.GetTranslationColumnName(this.TableShortName);
                }
                return base.GetTableNameString;
            }
        }
        public override bool IsComplexModel(string sql)
        {
            return Regex.IsMatch(sql, @"AS ""\w+\.\w+""")|| Regex.IsMatch(sql, @"AS ""\w+\.\w+\.\w+""");
        }
        public override string ToSqlString()
        {
            List<string> operations = new List<string>();
            var sb = new StringBuilder();
            sb.Append($"aggregate {this.GetTableNameString} ");
            sb.Append("[");
            sb.Append(string.Join(", ", operations));
            sb.Append("]");

            return sb.ToString();
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
                    result = "distinct "+result;
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
