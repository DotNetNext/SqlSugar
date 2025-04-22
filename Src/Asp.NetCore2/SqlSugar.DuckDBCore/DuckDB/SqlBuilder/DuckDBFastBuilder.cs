using DuckDB;
using DuckDB.NET.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar.DuckDB
{
    public class DuckDBFastBuilder : FastBuilder, IFastBuilder
    {
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

            await BulkInsertToDuckDB((DuckDBConnection)this.Context.Ado.Connection, dt.TableName, dt, this.Context.Ado.IsNoTran());
            return dt.Rows.Count;
        }
        public async Task BulkInsertToDuckDB(DuckDBConnection conn, string tableName, DataTable table, bool isTran)
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
                    conn.Open();
                }

                if (isTran)
                {
                    transaction = await conn.BeginTransactionAsync();
                } 
                // 构造 INSERT 语句
                var columnNames = string.Join(", ", table.Columns.Cast<DataColumn>().Select(c => c.ColumnName));
                var valuePlaceholders = string.Join(", ", table.Rows.Cast<DataRow>().Select(row =>
                    $"({string.Join(", ", row.ItemArray.Select(item=>FormatValue(item)))})"
                ));

                var insertSql = $"INSERT INTO {tableName} ({columnNames}) VALUES {valuePlaceholders}";

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
        public object FormatValue(object value)
        {
            if (value == null||value==DBNull.Value)
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
