using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;

namespace SqlSugar.MongoDb
{
    public class MongoDbUpdateBuilder : UpdateBuilder
    {
        public override string SqlTemplateBatch
        {
            get
            {
                return @"UPDATE  {1} {2} SET {0} FROM  ${{0}}  ";
            }
        }
        public override string SqlTemplateJoin
        {
            get
            {
                return @"            (VALUES
              {0}

            ) AS T ({2}) WHERE {1}
                 ";
            }
        }

        public override string SqlTemplateBatchUnion
        {
            get
            {
                return ",";
            }
        }
        protected override string ToSingleSqlString(List<IGrouping<int, DbColumnInfo>> groupList)
        {
            var result = BuildUpdateMany(groupList, this.TableName);
            return result;
        }
        protected override string TomultipleSqlString(List<IGrouping<int, DbColumnInfo>> groupList)
        {
            var result = BuildUpdateMany(groupList, this.TableName);
            return result;
        }
        public string BuildUpdateMany(List<IGrouping<int, DbColumnInfo>> groupList, string tableName)
        {
            var operations = new List<string>();
            List<string> pks = this.PrimaryKeys;
            if (this.SetValues.Any() && this.SetValues.Any(it => it.Key == "$set"))
            {
                BsonArray filterArray = GetFilterArray();
                var filter = new BsonDocument("$and", filterArray);
                operations.Add($"{{ filter: {filter.ToJson(UtilMethods.GetJsonWriterSettings())} , update: {SetValues.FirstOrDefault().Value}}}");
            }
            else if (this.SetValues.Any())
            {
                var setOperation = new BsonDocument();
                foreach (var item in this.SetValues)
                {
                    var bson = BsonSerializer.Deserialize<BsonDocument>(item.Value);
                    foreach (var element in bson)
                    {
                        setOperation[element.Name] = element.Value;
                    }
                }
                var filterArray = GetFilterArray();
                var filter = new BsonDocument("$and", filterArray);
                operations.Add($"{{ filter: {filter.ToJson(UtilMethods.GetJsonWriterSettings())} , update: {{ $set: {setOperation.ToJson(UtilMethods.GetJsonWriterSettings())} }} }}");
            }
            else
            {
                UpdateByObject(groupList, operations, pks);
            }
            var sb = new StringBuilder();
            sb.Append($"BulkWrite {tableName} [ ");
            sb.Append(string.Join(", ", operations));
            sb.Append(" ]");
            this.Parameters = new List<SugarParameter>();
            return sb.ToString();
        }

        private BsonArray GetFilterArray()
        {
            var filterArray = new BsonArray();
            foreach (var item in this.WhereValues)
            {
                var bson = BsonDocument.Parse(item); // 直接解析 JSON 为 BsonDocument
                filterArray.Add(bson); // 将每个条件添加到数组
            }

            return filterArray;
        }

        private static void UpdateByObject(List<IGrouping<int, DbColumnInfo>> groupList, List<string> operations, List<string> pks)
        {
            foreach (var group in groupList)
            {
                var filter = new BsonDocument();
                var setDoc = new BsonDocument();

                foreach (var col in group)
                { 
                    if (pks.Any(s=>s.EqualCase(col.DbColumnName)))
                    {
                        filter[col.DbColumnName] = UtilMethods.MyCreate(col.Value,col); 
                    }
                    else if (col.IsJson)
                    {
                        var bsonValue = UtilMethods.ParseJsonObject(col.Value?.ToString());
                        setDoc[col.DbColumnName] = bsonValue;
                    }
                    else if(col.IsPrimarykey==false)
                    {
                        var bsonValue = UtilMethods.MyCreate(col.Value, col);
                        setDoc[col.DbColumnName] = bsonValue;
                    }
                }

                var update = new BsonDocument
        {
            { "$set", setDoc }
        };

                var entry = new BsonDocument
        {
            { "filter", filter },
            { "update", update }
        };

                string json = entry.ToJson(UtilMethods.GetJsonWriterSettings());

                operations.Add(json);
            }
        }
    }
}
