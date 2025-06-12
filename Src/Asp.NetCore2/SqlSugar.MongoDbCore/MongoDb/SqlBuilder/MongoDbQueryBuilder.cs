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
                if (IsFieldNameJson(trimmed))
                {
                    var outerDoc = BsonDocument.Parse(trimmed);
                    trimmed = outerDoc["fieldName"].AsString;
                    operations.Add(trimmed);
                }
                else
                {
                    // item 是 JSON 格式字符串，直接包进 $match
                    operations.Add($"{{ \"$match\": {trimmed} }}");
                }
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
            if (orderByString == "ORDER BY NOW()")
                orderByString = null;
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
             
            #region GroupBy 
            if (this.GroupByValue.HasValue())
            {
                var regex = new Regex($@"\(\{UtilConstants.ReplaceCommaKey}\((.*?)\)\{UtilConstants.ReplaceCommaKey}\)",
                      RegexOptions.Compiled);

                var matches = regex.Matches(this.GroupByValue);
                var selectItems = new List<string>();
                foreach (Match match in matches)
                {
                    var selectItem = match.Groups[1].Value;
                    selectItems.Add(selectItem);
                }
                var jsonPart = "["+Regex.Split(this.GroupByValue, UtilConstants.ReplaceCommaKey)
                    .First()
                    .TrimEnd('(')
                    .Replace("GROUP BY ", "")+"]";
                var fieldNames = new List<string>();
                var bsonArray = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonArray>(jsonPart);
                foreach (BsonDocument bson in bsonArray)
                { 
                    if (bson.Contains("fieldName"))
                    {
                       var field = bson["fieldName"].AsString;
                       operations[operations.Count - 1] = operations[operations.Count - 1].Replace($"\"${field}\"", $"\"$_id.{field}\"");
                       fieldNames.Add(field);
                    }
                }
                // 构造 _id 部分：支持多字段形式
                var groupId = new BsonDocument();
                foreach (var field in fieldNames)
                {
                    groupId.Add(field, $"${field}");
                }

                var groupDoc = new BsonDocument("$group", new BsonDocument
                {
                    { "_id", groupId }
                });

                // 解析 selectJsonItems（每个是 BsonDocument 字符串）
                var groupFields = groupDoc["$group"].AsBsonDocument;
                foreach (var json in selectItems)
                {
                    var doc = BsonDocument.Parse(json);
                    foreach (var element in doc)
                    {
                        groupFields.Add(element.Name, element.Value);
                    }
                }
                operations.Insert(operations.Count-1, groupDoc.ToJson(UtilMethods.GetJsonWriterSettings()));
            }
            #endregion

            sb.Append($"aggregate {this.GetTableNameString} ");
            sb.Append("[");
            sb.Append(string.Join(", ", operations));
            sb.Append("]");

            return sb.ToString();
        }

        private bool IsFieldNameJson(string trimmed)
        {
            return trimmed.StartsWith("{ \"fieldName\" : ");
        }

        public override string ToCountSql(string sql)
        {
            sql=sql.TrimEnd(']');
            sql += "{ \"$count\": \"TotalCount\" }]";
            return sql;
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
