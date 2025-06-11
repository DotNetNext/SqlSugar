using MongoDB.Bson;
using SqlSugar.MongoDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

            #region Where
            foreach (var item in this.WhereInfos)
            {
                // 去除开头的 WHERE 或 AND（忽略大小写和空格）
                string trimmed = item.TrimStart();
                if (trimmed.StartsWith("WHERE", StringComparison.OrdinalIgnoreCase))
                    trimmed = trimmed.Substring(5).TrimStart();
                else if (trimmed.StartsWith("AND", StringComparison.OrdinalIgnoreCase))
                    trimmed = trimmed.Substring(3).TrimStart();
                // item 是 JSON 格式字符串，直接包进 $match
                operations.Add($"{{ \"$match\": {trimmed} }}");
            }
            #endregion

            #region Page
            var skip = this.Skip;
            var take = this.Take;
            // 处理 skip 和 take
            if (this.Skip.HasValue)
            {
                operations.Add($"{{ \"$skip\": {this.Skip.Value} }}");
            }
            if (this.Take.HasValue)
            {
                operations.Add($"{{ \"$limit\": {this.Take.Value} }}");
            }
            #endregion

            #region OrderBy
            var order = this.GetOrderByString;
            var orderByString = this.GetOrderByString?.Trim();
            if (!string.IsNullOrEmpty(orderByString) && orderByString.StartsWith("ORDER BY ", StringComparison.OrdinalIgnoreCase))
            {
                order = order.Substring("ORDER BY ".Length).Trim();

                var sortDoc = new BsonDocument();
                foreach (var str in order.Split(","))
                { 
                    int lastSpace = str.LastIndexOf(' ');
                    string jsonPart = str.Substring(0, lastSpace).Trim();
                    string directionPart = str.Substring(lastSpace + 1).Trim().ToUpper();
                    if (str.EndsWith("}")) 
                    {
                        jsonPart = str;
                        directionPart = "ASC";
                    }
                    var bson = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(jsonPart);
                    if (bson.Contains("fieldName"))
                    {
                        var field = bson["fieldName"].AsString;
                        var direction = directionPart == "DESC" ? -1 : 1;
                        sortDoc[field] = direction;
                    }
                }
                if (sortDoc.ElementCount > 0)
                {
                    operations.Add($"{{ \"$sort\": {sortDoc.ToJson()} }}");
                }
            }
            #endregion

            #region Select
            if (this.SelectValue is Expression expression) 
            {
                var dos=MongoNestedTranslator.Translate(expression, new MongoNestedTranslatorContext() { 
                    context = this.Context,
                    resolveType=ResolveExpressType.SelectSingle,
                    queryBuilder=this
                });
                var json = dos.ToJson(UtilMethods.GetJsonWriterSettings());
                operations.Add($"{{\"$project\": {json} }}"); 
            }
            #endregion

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
