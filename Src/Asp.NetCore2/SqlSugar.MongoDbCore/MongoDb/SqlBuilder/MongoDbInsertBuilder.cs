using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using NetTaste;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

namespace SqlSugar.MongoDb
{
    public class MongoDbInsertBuilder : InsertBuilder
    {
        public MongoDbInsertBuilder() 
        {
            this.SerializeObjectFunc = it =>
            {
                object value =it;  

                if (value is IEnumerable enumerable)
                {
                    var realType = value.GetType();
                    var bson = value.ToBson(realType);
                    var json = bson.ToJson(UtilMethods.GetJsonWriterSettings());
                    return json;
                }
                else
                {
                    var realType = it.GetType();
                    var bson = it.ToBson(realType); // → byte[]
                    var doc = BsonSerializer.Deserialize<BsonDocument>(bson); // → BsonDocument
                    var json = doc.ToJson(UtilMethods.GetJsonWriterSettings());
                    return json;
                }
            };
            this.DeserializeObjectFunc = (json, type) =>
            {
                if (json is Dictionary<string, object> keyValues)
                {
                    // 先用 Dictionary 构建 BsonDocument
                    var bsonDoc = new BsonDocument();

                    foreach (var kvp in keyValues)
                    {
                        bsonDoc.Add(kvp.Key, BsonValue.Create(kvp.Value));
                    }

                    // 再用 BsonSerializer 反序列化为 T
                    return BsonSerializer.Deserialize(bsonDoc, type);
                }
                else 
                {
                    return null;
                }
            };
        } 
        public override string SqlTemplate
        {
            get
            {
                if (IsReturnIdentity)
                {
                    return @"INSERT INTO {0} 
           ({1})
     VALUES
           ({2}) returning $PrimaryKey";
                }
                else
                {
                    return @"INSERT INTO {0} 
           ({1})
     VALUES
           ({2}) ;";

                }
            }
        }
        public override string SqlTemplateBatch => "INSERT INTO {0} ({1})";
        public override string SqlTemplateBatchUnion => " VALUES ";

        public override string SqlTemplateBatchSelect => " {0} ";

        public override Func<string, string, string> ConvertInsertReturnIdFunc { get; set; } = (name, sql) =>
        {
            return sql.Trim().TrimEnd(';')+ $"returning {name} ";
        }; 
        public override string ToSqlString()
        {
            var sql= BuildInsertMany(this.DbColumnInfoList, this.EntityInfo.DbTableName);
            return sql;
        }

        public string BuildInsertMany(List<DbColumnInfo> columns, string tableName)
        {
            // 分组
            var grouped = columns.GroupBy(c => c.TableId);

            var jsonObjects = new List<string>();

            foreach (var group in grouped)
            {
                BsonDocument doc = new BsonDocument();
                foreach (var col in group)
                {
                    // 自动推断类型，如 string、int、bool、DateTime、ObjectId 等
                    if (col.IsJson == true)
                    {
                        doc[col.DbColumnName] = BsonDocument.Parse(col.Value?.ToString());
                    }
                    else 
                    {
                        doc[col.DbColumnName] = UtilMethods.MyCreate(col.Value);
                    }
                }

                // 转为 JSON 字符串（标准 MongoDB shell 格式）
                string json = doc.ToJson(UtilMethods.GetJsonWriterSettings());

                jsonObjects.Add(json);
            }

            var sb = new StringBuilder();
            sb.Append($"insertMany { this.GetTableNameString } ");
            sb.Append("[");
            sb.Append(string.Join(", ", jsonObjects));
            sb.Append("]");

            return sb.ToString();
        } 
    }
}
