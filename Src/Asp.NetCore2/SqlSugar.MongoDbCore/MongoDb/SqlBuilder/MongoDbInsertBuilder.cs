using System;
using System.Collections.Generic;
using System.Linq;
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

        public static string BuildInsertMany(List<DbColumnInfo> columns, string tableName)
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
            sb.Append($"insertMany {tableName} ");
            sb.Append("[");
            sb.Append(string.Join(", ", jsonObjects));
            sb.Append("]");

            return sb.ToString();
        }

        public object FormatValue(object value, string name, int i, DbColumnInfo columnInfo)
        {
            if (value == null)
            {
                return "NULL";
            }
            else
            {
                var type = value.GetType();
                if (type == UtilConstants.DateType || columnInfo.IsArray || columnInfo.IsJson)
                {
                    var parameterName = this.Builder.SqlParameterKeyWord + name + i;
                    var paramter = new SugarParameter(parameterName, value);
                    if (columnInfo.IsJson)
                    {
                        paramter.IsJson = true;
                    }
                    if (columnInfo.IsArray)
                    {
                        paramter.IsArray = true;
                    }
                    this.Parameters.Add(paramter);
                    return parameterName;
                }
                else if (type == UtilConstants.ByteArrayType)
                {
                    string bytesString = "0x" + BitConverter.ToString((byte[])value);
                    return bytesString;
                }
                else if (type.IsEnum())
                {
                    if (this.Context.CurrentConnectionConfig.MoreSettings?.TableEnumIsString == true)
                    {
                        return value.ToSqlValue();
                    }
                    else
                    {
                        return Convert.ToInt64(value);
                    }
                }
                else if (type == UtilConstants.DateTimeOffsetType)
                {
                    return FormatDateTimeOffset(value);
                }
                else if (type == UtilConstants.BoolType)
                {
                    return value.ObjToBool() ? "1" : "0";
                }
                else if (type == UtilConstants.StringType || type == UtilConstants.ObjType)
                {
                    return "'" + value.ToString().ToSqlFilter() + "'";
                }
                else
                {
                    return "'" + value.ToString() + "'";
                }
            }
        }
        public override string FormatDateTimeOffset(object value)
        {
            return "'" + ((DateTimeOffset)value).ToString("o") + "'";
        }

    }
}
