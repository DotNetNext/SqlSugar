using Dm.util;
using MongoDB.Bson;
using MongoDB.Driver;
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

        #region ToSqlString Items 
        private void ProcessSelectConditions(List<string> operations)
        {
            if (this.SelectValue is Expression expression)
            {
                var dos = MongoNestedTranslator.Translate(expression, new MongoNestedTranslatorContext()
                {
                    context = this.Context,
                    resolveType = ResolveExpressType.SelectSingle,
                    queryBuilder = this
                });
                if (MongoDbExpTools.IsFieldNameJson(dos))
                {
                    dos[UtilConstants.FieldName] = "$" + dos[UtilConstants.FieldName];
                    dos.Add(new BsonElement("_id", "0"));
                }
                else if (dos.ElementCount > 0 && dos.GetElement(0).Name.StartsWith("$"))
                {
                    // 如果第一个key带有$，说明是个函数，外面套一层fieldName
                    var funcDoc = new BsonDocument(dos); // 复制一份
                    dos.Clear();
                    dos.Add(UtilConstants.FieldName, funcDoc);
                    dos.Add(new BsonElement("_id", "0"));
                }
                var json = dos.ToJson(UtilMethods.GetJsonWriterSettings());
                operations.Add($"{{\"$project\": {json} }}");
            }
        }
        private void ProcessGroupByConditions(List<string> operations)
        {
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
                var jsonPart = "[" + Regex.Split(this.GroupByValue, UtilConstants.ReplaceCommaKey)
                    .First()
                    .TrimEnd('(')
                    .Replace("GROUP BY ", "") + "]";
                var fieldNames = new List<string>();
                var bsonArray = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonArray>(jsonPart);
                foreach (BsonDocument bson in bsonArray)
                {
                    if (bson.Contains(UtilConstants.FieldName))
                    {
                        var field = bson[UtilConstants.FieldName].AsString;
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
                operations.Insert(operations.Count - 1, groupDoc.ToJson(UtilMethods.GetJsonWriterSettings()));
            }
        }
        private void ProcessOrderByConditions(List<string> operations)
        {

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
                    if (bson.Contains(UtilConstants.FieldName))
                    {
                        var field = bson[UtilConstants.FieldName].AsString;
                        var direction = directionPart == "DESC" ? -1 : 1;
                        sortDoc[field] = direction;
                    }
                }
                if (sortDoc.ElementCount > 0)
                {
                    operations.Add($"{{ \"$sort\": {sortDoc.ToJson()} }}");
                }
            }
        }
        private void ProcessPagination(List<string> operations)
        {
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
        }
        private void ProcessWhereConditions(List<string> operations)
        {
            foreach (var item in this.WhereInfos)
            {
                // 去除开头的 WHERE 或 AND（忽略大小写和空格）
                string trimmed = item.TrimStart();
                if (trimmed.StartsWith("WHERE", StringComparison.OrdinalIgnoreCase))
                    trimmed = trimmed.Substring(5).TrimStart();
                else if (trimmed.StartsWith("AND", StringComparison.OrdinalIgnoreCase))
                    trimmed = trimmed.Substring(3).TrimStart();
                if (MongoDbExpTools.IsFieldNameJson(trimmed))
                {
                    var outerDoc = BsonDocument.Parse(trimmed);
                    var fieldName = outerDoc[UtilConstants.FieldName].AsString;
                    // 这里假定该字段为bool类型，生成 { fieldName: true }
                    var boolDoc = new BsonDocument(fieldName, true);
                    operations.Add($"{{ \"$match\": {boolDoc.ToJson(UtilMethods.GetJsonWriterSettings())} }}");
                }
                else
                {
                    // item 是 JSON 格式字符串，直接包进 $match
                    operations.Add($"{{ \"$match\": {trimmed} }}");
                }
            }
        }
        private void ProcessJoinInfoConditions(List<string> operations)
        {
            foreach (var item in this.JoinQueryInfos)
            {
                var joinWhereDoc = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(item.JoinWhere);
                var isEasyJoin = !(this.JoinQueryInfoLets?.Any(s => s.Key.EqualCase(item.ShortName)) == true);
                var isExp = !isEasyJoin;
                var localField = isExp ? string.Empty : joinWhereDoc.GetElement(0).Value["$eq"][0]?.toString()?.TrimStart('$'); 
                var foreignField = isExp ?string.Empty: joinWhereDoc.GetElement(0).Value["$eq"][1]?.toString()?.TrimStart($"${item.ShortName}.".toCharArray());

                string from = item.TableName ?? item.ShortName ?? "Unknown";
                string asName = item.ShortName;
                var asNamePrefix = $"{asName}.";
                var isValueKey = !isExp && foreignField.StartsWith(asNamePrefix) && !localField.StartsWith(asNamePrefix);
                var isKeyValue = !isExp && !foreignField.StartsWith(asNamePrefix) && localField.StartsWith(asNamePrefix);
              

                if (isKeyValue)
                {
                    localField = localField.TrimStart(asNamePrefix.ToCharArray());
                    var oldLocalField = localField;
                    localField = foreignField;
                    foreignField = oldLocalField;
                }
                else if (isValueKey)
                {
                    foreignField = foreignField.TrimStart(asNamePrefix.ToCharArray());
                }

                if (isEasyJoin)
                {
                    // $lookup 简单等值连接
                    var lookupDoc = new BsonDocument("$lookup", new BsonDocument
                    {
                        { "from", from },
                        { "localField", localField },
                        { "foreignField", foreignField },
                        { "as", asName }
                    });
                    operations.Add(lookupDoc.ToJson(UtilMethods.GetJsonWriterSettings()));
                }
                else
                { 
                    // 解析$expr表达式
                    var exprDoc = joinWhereDoc.Contains("$expr") ? joinWhereDoc["$expr"] : joinWhereDoc;
                     
                    // 构造let变量（可扩展，当前假设无变量）
                    var letDoc = new BsonDocument();
                    if (this.JoinQueryInfoLets != null && this.JoinQueryInfoLets.TryGetValue(asName, out var letFields) && letFields != null)
                    {
                        foreach (var kv in letFields)
                        {
                            letDoc.Add(kv.Key, $"{kv.Value}");
                        }
                    } 
                    // 构造pipeline
                    var pipelineArray = new BsonArray
                    {
                        new BsonDocument("$match", new BsonDocument("$expr", exprDoc))
                    };

                    var lookupDoc = new BsonDocument("$lookup", new BsonDocument
                    {
                        { "from", from },
                        { "let", letDoc },
                        { "pipeline", pipelineArray },
                        { "as", asName }
                    });
                    operations.Add(lookupDoc.ToJson(UtilMethods.GetJsonWriterSettings()));
                }

                // $unwind
                BsonValue unwindDoc = null;
                if (item.JoinType == JoinType.Left&&isEasyJoin)
                {
                    unwindDoc = new BsonDocument("$unwind", new BsonDocument
                    {
                        { "path", $"${asName}" },
                        { "preserveNullAndEmptyArrays", true }
                    });
                }
                else if (item.JoinType == JoinType.Inner && isEasyJoin)
                {
                    unwindDoc = new BsonDocument("$unwind", new BsonDocument
                    {
                        { "path", $"${asName}" }
                    });
                }
                else if (isExp)
                {
                    unwindDoc = new BsonDocument("$unwind", new BsonDocument
                    {
                        { "path", $"${asName}" }
                    });
                }
                else
                {
                    throw new Exception(" No Support " + item.JoinType);
                }
                operations.Add(unwindDoc.ToJson(UtilMethods.GetJsonWriterSettings()));
            }
        }

        #endregion

        #region Get SQL Partial 
        public override string GetTableNameString
        {
            get
            {
                if (this.AsTables.Any())
                    return this.AsTables.FirstOrDefault().Value;
                else
                    return this.Context.EntityMaintenance.GetEntityInfo(this.EntityType).DbTableName;
            }
        }
        public override bool IsComplexModel(string sql)
        {
            return Regex.IsMatch(sql, @"AS ""\w+\.\w+""") || Regex.IsMatch(sql, @"AS ""\w+\.\w+\.\w+""");
        }
        public override string ToSqlString()
        {
            List<string> operations = new List<string>();
            var sb = new StringBuilder();

            ProcessJoinInfoConditions(operations);

            ProcessWhereConditions(operations); 

            ProcessOrderByConditions(operations); 

            ProcessPagination(operations);

            ProcessSelectConditions(operations);

            ProcessGroupByConditions(operations);

            sb.Append($"aggregate {this.GetTableNameString} ");
            sb.Append("[");
            sb.Append(string.Join(", ", operations));
            sb.Append("]");

            return sb.ToString();
        }

        public override string ToCountSql(string sql)
        {
            sql = sql.TrimEnd(']');
            sql += "{ \"$count\": \"TotalCount\" }]";
            return sql;
        }
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
                    result = "distinct " + result;
                }
                if (this.SubToListParameters != null && this.SubToListParameters.Any())
                {
                    result = SubToListMethod(result);
                }
                return result;
            }
        }

        #endregion
         
        #region Exp to sql
        public bool? EasyJoin { get; internal set; }
        public string FirstParameter { get; internal set; }
        public string LastParameter { get; internal set; }
        public Dictionary<string,string> lets { get; internal set; } 
        public Dictionary<string, Dictionary<string, string>> JoinQueryInfoLets { get; set; }
        #endregion
    }
}
