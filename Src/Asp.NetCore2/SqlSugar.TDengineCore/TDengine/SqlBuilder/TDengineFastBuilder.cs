
using SqlSugar.TDengineAdo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar.TDengine
{
    public class TDengineFastBuilder : FastBuilder, IFastBuilder
    {
        public const string TagKey = "TDengineFastBuilderTagNames";
        public static void SetTags(ISqlSugarClient db,params string [] tagNames) 
        {
            if (db.TempItems == null) 
            {
                db.TempItems = new Dictionary<string, object>();
            }
            db.TempItems.Add(TagKey, tagNames);
        }
        public async Task<int> ExecuteBulkCopyAsync(DataTable dt)
        {
            var db = this.Context;
            if (db.TempItems == null)
            {
                db.TempItems = new Dictionary<string, object>();
            }
            string[] tagNames = new string[] { };
            if (db.TempItems.ContainsKey("TDengineFastBuilderTagNames")) 
            {
                tagNames = (string[])db.TempItems[TagKey];
            }
            db.TempItems.Add("TDengineFastBuilderTagNames", tagNames);
            await BulkInsertToTDengine((TDengineConnection)this.Context.Ado.Connection,dt.TableName,dt, this.Context.Ado.IsNoTran(), tagNames);
            return dt.Rows.Count;
        }
        public async Task BulkInsertToTDengine(TDengineConnection conn, string tableName, DataTable table, bool isTran, string[] tagColumns)
        {
            DbTransaction? transaction = null;
            var isAutoClose = this.Context.CurrentConnectionConfig.IsAutoCloseConnection;
            if (isAutoClose)
            {
                this.Context.CurrentConnectionConfig.IsAutoCloseConnection = false;
            }

            try
            {
                if (conn.State != ConnectionState.Open)
                {
                    await conn.OpenAsync();
                }

                if (isTran)
                {
                    transaction = await conn.BeginTransactionAsync();
                }

                // 自动建表
                var createTableSql = BuildCreateTableSql(tableName, table, tagColumns);
                await ExecuteSqlAsync(conn, createTableSql);

                // 构造 INSERT 语句
                var columnNames = string.Join(", ", table.Columns.Cast<DataColumn>().Select(c => c.ColumnName));
                var valuePlaceholders = string.Join(", ", table.Rows.Cast<DataRow>().Select(row =>
                    $"({string.Join(", ", row.ItemArray.Select(item => FormatValue(item)))})"
                ));

                var insertSql = $"INSERT INTO {tableName} ({columnNames}) TAGS ({string.Join(", ", tagColumns)}) VALUES {valuePlaceholders}";

                using var cmd = conn.CreateCommand();
                cmd.CommandText = insertSql;

                // 执行批量插入
                await cmd.ExecuteNonQueryAsync();

                if (isTran)
                {
                    await transaction!.CommitAsync();
                }
            }
            catch
            {
                if (isTran && transaction != null)
                {
                    await transaction.RollbackAsync();
                }

                throw;
            }
            finally
            {
                if (isTran && transaction != null)
                {
                    transaction?.Dispose();
                }
                this.Context.CurrentConnectionConfig.IsAutoCloseConnection = isAutoClose;
            }
        }

        private string BuildCreateTableSql(string tableName, DataTable table, string[] tagColumns)
        {
            // 自动建表语句
            var columnDefinitions = table.Columns.Cast<DataColumn>()
                .Select(c => $"{c.ColumnName} {GetTDengineColumnType(c.DataType)}");

            var tagDefinitions = tagColumns.Select(tag => $"{tag} STRING");

            var createTableSql = $@"
        CREATE TABLE IF NOT EXISTS {tableName} (
            {string.Join(", ", columnDefinitions)},
            {string.Join(", ", tagDefinitions)},
            ts TIMESTAMP
        ) TAGS ({string.Join(", ", tagColumns)});
    ";

            return createTableSql;
        }

        private string GetTDengineColumnType(Type type)
        {
            if (type == typeof(int) || type == typeof(long) || type == typeof(float) || type == typeof(double))
            {
                return "FLOAT";
            }
            else if (type == typeof(DateTime))
            {
                return "TIMESTAMP";
            }
            else if (type == typeof(bool))
            {
                return "BOOLEAN";
            }
            else if (type == typeof(string))
            {
                return "STRING";
            }
            else
            {
                return "STRING";  // 默认类型为 STRING
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
                if (type == typeof(DateTime))
                {
                    return $"'{Convert.ToDateTime(value).ToString("yyyy-MM-dd HH:mm:ss.ms")}'";
                }
                else if (type == typeof(byte[]))
                {
                    return $"0x{BitConverter.ToString((byte[])value).Replace("-", "")}";
                }
                else if (type.IsEnum)
                {
                    return Convert.ToInt64(value);
                }
                else if (type == typeof(bool))
                {
                    return value.ObjToBool() ? "1" : "0";
                }
                else if (type == typeof(string))
                {
                    return $"'{value.ToString().ToSqlFilter()}'";
                }
                else if (value is decimal)
                {
                    return value.ToString();
                }
                else
                {
                    return $"'{value.ToString()}'";
                }
            }
        }

        private async Task ExecuteSqlAsync(TDengineConnection conn, string sql)
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
