using MongoDb.Ado.data;
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
using System.Text.Json.Nodes;

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
                    var list = new List<BsonValue>();

                    foreach (var e in enumerable)
                    {  
                        var realType = e?.GetType();
                        if (realType == null)
                        {
                            list.Add(BsonValue.Create(null));
                        }
                        else if (realType.IsClass())
                        {
                            var bson = e.ToBson(realType); // 序列化为 byte[]
                            var doc = BsonSerializer.Deserialize<BsonDocument>(bson); // 反序列化为 BsonDocument
                            list.Add(doc);
                        }
                        else 
                        {
                            if (e is string s && UtilMethods.IsValidObjectId(s))
                            { 
                                list.Add(UtilMethods.MyCreate(ObjectId.Parse(s)));
                            }
                            else
                            {
                                list.Add(UtilMethods.MyCreate(e));
                            }
                        }
                    }

                    var array = new BsonArray(list);
                    ConvertBsonDateTimeToLocal(array);
                    return array.ToJson(UtilMethods.GetJsonWriterSettings());
                }
                else
                {
                    var realType = it.GetType();
                    var bson = it.ToBson(realType); // → byte[]
                    var doc = BsonSerializer.Deserialize<BsonDocument>(bson); // → BsonDocument
                    ConvertBsonDateTimeToLocal(doc);
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
                else if (json is BsonDocument bsons)
                {
                    return BsonSerializer.Deserialize(bsons, type);
                }
                else if (json is List<object> list0 && list0.Any() && list0.FirstOrDefault() is Dictionary<string, object>)
                {
                    Type elementType = type.GetGenericArguments()[0];
                    var resultList = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));
                    BsonArray bsonArray = new BsonArray();
                    foreach (Dictionary<string, object> item in list0)
                    {
                        var bsonDoc = new BsonDocument();
                        foreach (var kvp in item)
                        {
                            bsonDoc.Add(kvp.Key, BsonValue.Create(kvp.Value));
                        }
                        resultList.Add(BsonSerializer.Deserialize(bsonDoc, elementType));
                    }
                    return resultList;
                }
                else if (json is List<object> list)
                {
                    string jsonStr = System.Text.Encoding.UTF8.GetString(list.Select(it => Convert.ToByte(it)).ToArray());
                    // 2. 解析为 BsonArray
                    var bsonArray = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonArray>(jsonStr);

                    // 3. 获取元素类型，例如 List<MyClass> => MyClass
                    Type elementType = type.GetGenericArguments()[0];

                    // 4. 构造泛型列表对象
                    var resultList = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));

                    // 5. 反序列化每一项
                    foreach (var item in bsonArray)
                    {
                        var doc = item.AsBsonDocument;
                        var obj = BsonSerializer.Deserialize(doc, elementType);
                        resultList.Add(obj);
                    }
                    return resultList;
                }
                else if (json is BsonArray array && type.GetGenericArguments().Any())
                {
                    var bsonArray = array;

                    // 3. 获取元素类型，例如 List<MyClass> => MyClass
                    Type elementType = type.GetGenericArguments()[0];

                    // 4. 构造泛型列表对象
                    var resultList = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));

                    // 5. 反序列化每一项
                    foreach (var item in bsonArray)
                    {
                        if (item is BsonDocument)
                        {
                            var doc = item.AsBsonDocument;
                            var obj = BsonSerializer.Deserialize(doc, elementType);
                            UtilMethods.ConvertDateTimeToUnspecified(obj);
                            resultList.Add(obj);
                        }
                        else
                        {
                            var obj = MongoDbDataReaderHelper.ConvertBsonValue(item);
                            resultList.Add(obj);
                        }
                    }
                    return resultList;
                }
                else if (json is BsonArray array2 && type.IsArray)
                {
                    var bsonArray = array2;

                    // 3. 获取元素类型，例如 List<MyClass> => MyClass
                    Type elementType = type.GetElementType();

                    // 4. 构造泛型列表对象
                    var resultList = Array.CreateInstance(elementType, array2.Count());

                    // 5. 反序列化每一项
                    int index = 0;
                    foreach (var item in bsonArray)
                    {
                        if (item is BsonDocument doc)
                        {
                            var obj = BsonSerializer.Deserialize(doc, elementType);
                            resultList.SetValue(obj, index);
                        }
                        else
                        {
                            var obj = MongoDbDataReaderHelper.ConvertBsonValue(item);
                            if (obj is DBNull) 
                            {
                                obj = null;
                            }
                            resultList.SetValue(obj, index);
                        }
                        index++;
                    }
                    return resultList;
                }
                else if (json is string str && type.GetGenericArguments().Any())
                {

                    var jsonArray = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonArray>(str);

                    // 3. 获取元素类型，例如 List<MyClass> => MyClass
                    Type elementType = type.GetGenericArguments()[0];

                    // 4. 构造泛型列表对象
                    var resultList = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));

                    // 5. 反序列化每一项
                    foreach (var item in jsonArray)
                    {
                        if (item is BsonDocument)
                        {
                            var doc = item.AsBsonDocument;
                            var obj = BsonSerializer.Deserialize(doc, elementType);
                            resultList.Add(obj);
                        }
                        else
                        {
                            var obj = MongoDbDataReaderHelper.ConvertBsonValue(item);
                            resultList.Add(obj);
                        }
                    }
                    return resultList;
                }
                else
                {
                    return json;
                }
            };
        }
        private void ConvertBsonDateTimeToLocal(BsonValue bsonValue)
        {
            if (bsonValue == null || bsonValue.IsBsonNull)
                return;

            if (bsonValue.IsBsonDocument)
            {
                var doc = bsonValue.AsBsonDocument;
                foreach (var element in doc.Elements.ToList())
                {
                    var val = element.Value;
                    if (val.BsonType == BsonType.DateTime)
                    {
                        var utcDate = val.ToUniversalTime();
                        var localDate = utcDate.ToLocalTime();
                        doc[element.Name] =UtilMethods.MyCreate(localDate);
                    }
                    else
                    {
                        ConvertBsonDateTimeToLocal(val);
                    }
                }
            }
            else if (bsonValue.IsBsonArray)
            {
                var array = bsonValue.AsBsonArray;
                for (int i = 0; i < array.Count; i++)
                {
                    var val = array[i];
                    if (val.BsonType == BsonType.DateTime)
                    {
                        var utcDate = val.ToUniversalTime();
                        var localDate = utcDate.ToLocalTime();
                        array[i] = new BsonDateTime(localDate);
                    }
                    else
                    {
                        ConvertBsonDateTimeToLocal(val);
                    }
                }
            }
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
            this.Parameters = new List<SugarParameter>();
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
                        doc[col.DbColumnName] = UtilMethods.ParseJsonObject(col.Value);
                    }
                    else if (col.Value != null && col.DataType == nameof(ObjectId))
                    {
                        doc[col.DbColumnName] = UtilMethods.MyCreate(ObjectId.Parse(col.Value?.ToString()));
                    }
                    else if (col.InsertServerTime)
                    {
                        doc[col.DbColumnName] = UtilMethods.MyCreate(DateTime.Now);
                    }
                    else if (col.SqlParameterDbType is Type t&& typeof(ISugarDataConverter).IsAssignableFrom(t)) 
                    {
                        var p =UtilMethods.GetParameterConverter(0,this.Context,col.Value,this.EntityInfo,this.EntityInfo.Columns.FirstOrDefault(s=>s.PropertyName.EqualCase(col.PropertyName)));
                        doc[col.DbColumnName] = UtilMethods.MyCreate(p.Value);
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
