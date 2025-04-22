
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
            try
            {
                // Build the column names and value placeholders
                var valuePlaceholdersList = table.Rows.Cast<DataRow>().Select(row =>
                {
                    var values = row.ItemArray.Select(item => FormatValue(item)).ToList();

                    // If there are tags, move them to the beginning of the values list and adjust insert syntax
                    if (tagColumns != null && tagColumns.Length > 0)
                    {
                        foreach (var tag in tagColumns)
                        {
                            int index = table.Columns.IndexOf(tag);
                            values.Insert(0, FormatValue(row[index]));
                        }
                    }

                    return $"({string.Join(", ", values)})";
                }).ToList();

                var valuePlaceholders = string.Join(", ", valuePlaceholdersList);

                // Check if tagColumns is provided and adjust the SQL statement accordingly
                string insertSql;
                if (tagColumns != null && tagColumns.Length > 0)
                {
                    // Construct SQL with tags included
                    var columnNames = string.Join(", ", tagColumns.Concat(table.Columns.Cast<DataColumn>().Where(c => !tagColumns.Contains(c.ColumnName)).Select(c => c.ColumnName)));
                    insertSql = $"INSERT INTO {tableName} ({columnNames}) VALUES {valuePlaceholders}";
                }
                else
                {
                    // Construct SQL without tags
                    var columnNames = string.Join(", ", table.Columns.Cast<DataColumn>().Select(c => c.ColumnName));
                    insertSql = $"INSERT INTO {tableName} ({columnNames}) VALUES {valuePlaceholders}";
                }

                // Execute the command asynchronously
                await this.Context.Ado.ExecuteCommandAsync(insertSql);
            }
            catch
            {
                throw;
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
