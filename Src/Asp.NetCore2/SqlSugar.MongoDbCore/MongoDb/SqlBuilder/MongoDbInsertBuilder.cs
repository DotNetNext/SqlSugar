using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

namespace SqlSugar.MongoDb
{
    public class MongoDbInsertBuilder : InsertBuilder
    {
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
                var dict = new Dictionary<string, object>();

                foreach (var col in group)
                {
                    dict[col.DbColumnName] = col.Value;
                }

                string json = JsonSerializer.Serialize(dict, new JsonSerializerOptions
                {
                    WriteIndented = false
                });

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
