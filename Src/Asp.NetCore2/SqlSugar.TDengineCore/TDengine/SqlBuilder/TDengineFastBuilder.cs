
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
        public override DbFastestProperties DbFastestProperties { get; set; } = new DbFastestProperties()
        {
            NoPage=true
        };

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

        public async Task BulkInsertToTDengine(TDengineConnection conn, string tableName, DataTable table, bool isTran, string[] tagColumns)
        {
             
            string insertSql=string.Empty;
            try
            {
                
                if (tagColumns != null && tagColumns.Length > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    StringBuilder sbTables = new StringBuilder();
                    // 创建一个列名映射（忽略大小写）
                    var columnMap = table.Columns.Cast<DataColumn>()
                        .ToDictionary(c => c.ColumnName, c => c, StringComparer.OrdinalIgnoreCase);

                    // 检查所有 tagColumns 是否在 DataTable 中存在
                    foreach (var col in tagColumns)
                    {
                        if (!columnMap.ContainsKey(col))
                            throw new Exception($"Column '{col}' not found in DataTable.");
                    }

                    // 用 LINQ 分组
                    var groups = table.AsEnumerable()
                        .GroupBy(row => string.Join("||", tagColumns.Select(tc => row[columnMap[tc]].ToString())));

                    foreach (var group in groups)
                    {
                        // 构建一个新的子表（结构与原表一致）
                        DataTable childTable = table.Clone();

                        // 将分组行复制到子表中
                        foreach (var row in group)
                        {
                            childTable.ImportRow(row);
                        }

                        // 调用 InsertChildTable
                        InsertChildTable(tableName, childTable, tagColumns,sb, sbTables);

                        var sql = sb.ToString();
                        var result = await this.Context.Ado.ExecuteCommandAsync(sql);
                        sb.Clear();
                    } 
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

                    // Execute the command asynchronously
                    await this.Context.Ado.ExecuteCommandAsync(insertSql);
                } 
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message +"\r\n"+ insertSql);
            }
            finally
            {
                this.Context.TempItems.Remove(TagKey);
            }
        }

        private StringBuilder InsertChildTable(string tableName, DataTable table, string[] tagColumns, StringBuilder sb, StringBuilder sbtables)
        {
            var builder = InstanceFactory.GetSqlBuilderWithContext(this.Context);
            var columnMap = table.Columns.Cast<DataColumn>()
                .ToDictionary(c => c.ColumnName, c => c.ColumnName, StringComparer.OrdinalIgnoreCase);

            var firstRow = table.Rows[0];
            string tags = string.Join(", ", tagColumns.Select(tag => FormatValue(firstRow[columnMap[tag]])));
            string tagsValues = string.Join("_", tagColumns.Select(tag => firstRow[columnMap[tag]].ToString()));

            // 移除标签列，只留下数据列
            foreach (var item in tagColumns)
            {
                table.Columns.Remove(item);
            }

            var columnNames = string.Join(", ", table.Columns.Cast<DataColumn>()
                .Select(c => builder.GetTranslationColumnName(c.ColumnName)));

            var action = this.Context.TempItems[TagKey + "action"] as Func<string, string,string>;
            var subTableName = builder.GetTranslationColumnName(action(tagsValues, tableName.Replace("`","")));

            // sbtables.AppendLine($"CREATE TABLE {subTableName} USING {tableName} TAGS({tags});");

            var sqlBuilder = sb;
            var valuesList = new List<string>();

            foreach (DataRow row in table.Rows)
            {
                var values = row.ItemArray.Select(item => FormatValue(item)).ToList();
                var valuePart = $"({string.Join(", ", values)})";
                valuesList.Add(valuePart);
            }

            if (valuesList.Count > 0)
            {
                string insertSql = $"INSERT INTO {subTableName} USING {tableName} TAGS({tags}) ({columnNames}) VALUES {string.Join(", ", valuesList)};";
                sqlBuilder.AppendLine(insertSql);
            }

            return sqlBuilder;
        }


        public static void SetTags(ISqlSugarClient db, Func<string, string, string> action, params string[] tagNames)
        {
            if (db.TempItems == null)
            {
                db.TempItems = new Dictionary<string, object>();
            }
            // 删除旧的值（如果存在）
            db.TempItems.Remove(TagKey);
            db.TempItems.Remove(TagKey + "action");
            db.TempItems.Add(TagKey, tagNames);
            db.TempItems.Add(TagKey + "action", action);
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
                    return Convert.ToDateTime(value).ToString("yyyy-MM-dd HH:mm:ss.fffffff").ToSqlValue();
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
