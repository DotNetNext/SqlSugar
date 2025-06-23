using MongoDB.Bson;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SqlSugar.MongoDb
{
    public class MongoDbDeleteBuilder : DeleteBuilder
    {
        public override string ToSqlString()
        {
            var sb = new StringBuilder($"deleteMany {this.GetTableNameString} ");
            var jsonObjects = new List<string>();
            foreach (var item in this.WhereInfos)
            {
                if (item.StartsWith("{") && item.EndsWith("}")) 
                {
                    jsonObjects.Add(item);
                    continue;
                }
                var key = this.EntityInfo.Columns.FirstOrDefault(it => it.IsPrimarykey);
                var startWithValue = $"{Builder.GetTranslationColumnName(key.DbColumnName)} IN (";
                if (item.StartsWith(startWithValue))
                {
                    var sql = item;
                    sql = sql.TrimEnd(')');
                    sql = sql.Replace(startWithValue, "").Replace("'", "");
                    var dict = new Dictionary<string, object>();
                    var array = sql.Split(",");
                    var idStrings = sql.Split(",");

                    // 将字符串数组转为 ObjectId 列表（如果不是 ObjectId 可保留为字符串）
                    var bsonArray = new BsonArray();
                    foreach (var idStr in idStrings)
                    {
                        bsonArray.Add(ObjectId.Parse(idStr)); // fallback 为普通字符串
                    }

                    var filter = new BsonDocument
                    {
                        { "_id", new BsonDocument { { "$in", bsonArray } } }
                    };

                    string json = filter.ToJson(UtilMethods.GetJsonWriterSettings()); // 使用 MongoDB 驱动的序列化
                    jsonObjects.Add(json);
                }
            }
            sb.Append("[{\"filter\":");
            sb.Append(string.Join(", ", jsonObjects));
            sb.Append("}]");
            return sb.ToString();
        }
    }
}
