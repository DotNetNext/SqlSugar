using System;
using System.Collections.Generic;
using System.Data.Common;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Data;
using System.Collections;
using System.Linq;
using MongoDB.Bson.Serialization;

namespace MongoDb.Ado.data
{
    public static class MongoDbDataReaderHelper
    {
        public static DbDataReader ToDataReader(IEnumerable<BsonDocument> documents)
        {
            var table = new DataTable();
            var allFields = new HashSet<string>();

            // 收集所有字段名
            foreach (var doc in documents)
            {
                foreach (var elem in doc.Elements)
                {
                    allFields.Add(elem.Name);
                }
            }

            // 建立DataTable列，类型object支持多类型和DBNull
            foreach (var field in allFields)
            {
                table.Columns.Add(field, typeof(object));
            }

            // 填充DataTable行
            foreach (var doc in documents)
            {
                var row = table.NewRow();
                foreach (var field in allFields)
                {
                    if (doc.Contains(field))
                    {
                        var val = doc[field];
                        row[field] = val.IsBsonNull ? DBNull.Value : ConvertBsonValue(val);
                    }
                    else
                    {
                        row[field] = DBNull.Value;
                    }
                }
                table.Rows.Add(row);
            }

            // 返回IDataReader
            return table.CreateDataReader();
        }
        public static object ConvertBsonValue(BsonValue val)
        {
            if (val == null || val.IsBsonNull)
                return DBNull.Value;

            switch (val.BsonType)
            {
                case BsonType.Int32:
                    return val.AsInt32;
                case BsonType.Int64:
                    return val.AsInt64;
                case BsonType.Double:
                    return val.AsDouble;
                case BsonType.String:
                    return val.AsString;
                case BsonType.Boolean:
                    return val.AsBoolean;
                case BsonType.DateTime:
                    return val.ToUniversalTime();
                case BsonType.Decimal128:
                    return val.AsDecimal;
                case BsonType.ObjectId:
                    return val.AsObjectId.ToString();
                case BsonType.Binary:
                    return val.AsBsonBinaryData.Bytes;
                // 其他类型可以扩展
                default:
                    return val;
            }
        }
    }

}
