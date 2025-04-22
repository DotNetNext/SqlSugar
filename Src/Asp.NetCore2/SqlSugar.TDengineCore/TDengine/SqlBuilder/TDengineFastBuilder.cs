
using SqlSugar.TDengineAdo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar.TDengine
{
    public class TDengineFastBuilder : FastBuilder, IFastBuilder
    {
        public const string TagKey = "TDengineFastBuilderTagNames";


        public async Task<int> ExecuteBulkCopyAsync(DataTable dt)
        {
            // 移除自增列  
            var identities = this.FastEntityInfo.Columns.Where(it => it.IsIdentity).Select(it => it.DbColumnName).ToList();
            foreach (var identity in identities)
            {
                if (dt.Columns.Contains(identity))
                {
                    dt.Columns.Remove(identity);
                }
            }
            var db = this.Context;
            string[] tagNames = null;
            if (db.TempItems!=null&&db.TempItems.ContainsKey(TagKey))
            {
                tagNames = db.TempItems[TagKey]as string[];
            } 
            await BulkInsertToTDengine((TDengineConnection)this.Context.Ado.Connection, dt.TableName, dt, this.Context.Ado.IsNoTran(), tagNames);
            return dt.Rows.Count;
        }

        public static void SetTags(ISqlSugarClient db,params string [] tagNames) 
        {
            if (db.TempItems == null) 
            {
                db.TempItems = new Dictionary<string, object>();
            }
            db.TempItems.Add(TagKey, tagNames);
        }
        public async Task BulkInsertToTDengine(TDengineConnection conn, string tableName, DataTable table, bool isTran, string[] tagColumns)
        {
             
            string insertSql=string.Empty;
            try
            {
                
                if (tagColumns != null && tagColumns.Length > 0)
                {
                     
                    string tags = string.Join(", ", tagColumns.Select(tag => FormatValue(tag)));
                    foreach (var item in tagColumns)
                    {
                        table.Columns.Remove(item); 
                    }
                    // Build the column names and value placeholders
                    var valuePlaceholdersList = table.Rows.Cast<DataRow>().Select(row =>
                    {
                        var values = row.ItemArray.Select(item => FormatValue(item)).ToList();
                        return $"({string.Join(", ", values)})";
                    });
                    var valuePlaceholders = string.Join(", ", valuePlaceholdersList);
                    // 排除标签列，确保数据列不包含标签列
                    var columnNames = string.Join(", ", table.Columns.Cast<DataColumn>() 
                        .Select(c => c.ColumnName));
                    // 生成插入语句，USING 部分包含标签列，VALUES 部分只包含数据列
                    insertSql = $"INSERT INTO {tableName} USING {tableName} TAGS({tags})  ({columnNames}) VALUES {valuePlaceholders}";
                }
                else
                {
                    // Build the column names and value placeholders
                    var valuePlaceholdersList = table.Rows.Cast<DataRow>().Select(row =>
                    {
                        var values = row.ItemArray.Select(item => FormatValue(item)).ToList();
                        return $"({string.Join(", ", values)})";
                    }).ToList();

                    var valuePlaceholders = string.Join(", ", valuePlaceholdersList);
                    // Construct SQL without tags
                    var columnNames = string.Join(", ", table.Columns.Cast<DataColumn>().Select(c => c.ColumnName));
                    insertSql = $"INSERT INTO {tableName} ({columnNames}) VALUES {valuePlaceholders}";
                }

                // Execute the command asynchronously
                await this.Context.Ado.ExecuteCommandAsync(insertSql);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message +"\r\n"+ insertSql);
            }
            finally
            {
            }
        }


        public object FormatValue(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return "NULL";
            }
            else
            {
                var type = value.GetType();
                if (type == UtilConstants.DateType)
                {
                    return Convert.ToDateTime(value).ToString("yyyy-MM-dd HH:mm:ss.ms").ToSqlValue();
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
                else if (type == UtilConstants.BoolType)
                {
                    return value.ObjToBool() ? "1" : "0";
                }
                else if (type == UtilConstants.StringType || type == UtilConstants.ObjType)
                {
                    return "'" + value.ToString().ToSqlFilter() + "'";
                }
                else if (value is decimal v)
                {
                    return v.ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    return "'" + value.ToString() + "'";
                }
            }
        }

    }
}
