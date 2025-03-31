using Npgsql;
using NpgsqlTypes;
using OpenGauss.NET;
using OpenGauss.NET.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar.GaussDB
{
    internal class GaussDBFastBuilder : FastBuilder, IFastBuilder
    {
        public static Dictionary<string , OpenGaussDbType> PgSqlType = UtilMethods.EnumToDictionary<OpenGaussDbType>();

        public async Task<int> ExecuteBulkCopyAsync(DataTable dt)
        {
            List<string> lsColNames = new List<string>();
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                lsColNames.Add($"\"{dt.Columns[i].ColumnName}\"");
            }
            string copyString = $"COPY  {dt.TableName} ( {string.Join(",", lsColNames)} ) FROM STDIN (FORMAT BINARY)";
            if (this.Context?.CurrentConnectionConfig?.MoreSettings?.DatabaseModel == DbType.OpenGauss)
            {
                copyString = copyString.Replace("(FORMAT BINARY)", "(FORMAT 'BINARY')");
            }
            OpenGaussConnection conn = (OpenGaussConnection)this.Context.Ado.Connection;
            var columns = this.Context.DbMaintenance.GetColumnInfosByTableName(this.FastEntityInfo.DbTableName);
            try
            {
                var identityColumnInfo = this.FastEntityInfo.Columns.FirstOrDefault(it => it.IsIdentity);
                if (identityColumnInfo != null)
                {
                    throw new Exception("PgSql bulkcopy no support identity");
                }
                BulkCopy(dt, copyString, conn, columns);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                base.CloseDb();
            }
            return await Task.FromResult(dt.Rows.Count);
        }

        private void BulkCopy(DataTable dt, string copyString, OpenGaussConnection conn, List<DbColumnInfo> columns)
        {
            if (conn.State == ConnectionState.Closed)
                conn.Open();
            List<ColumnView> columnViews = new List<ColumnView>();
            foreach (DataColumn item in dt.Columns)
            {
                ColumnView result = new ColumnView();
                result.DbColumnInfo = columns.FirstOrDefault(it => it.DbColumnName.Equals(item.ColumnName, StringComparison.OrdinalIgnoreCase));
                result.DataColumn = item;
                result.EntityColumnInfo = this.FastEntityInfo.Columns.FirstOrDefault(it => it.DbColumnName.Equals(item.ColumnName, StringComparison.OrdinalIgnoreCase));
                var key = result.DbColumnInfo?.DataType?.ToLower();
                if (result.DbColumnInfo == null)
                {
                    result.Type = null;
                }
                else if (PgSqlType.ContainsKey(key))
                {
                    result.Type = PgSqlType[key];
                }
                else if (key?.First() == '_')
                {
                    if (key == "_int4")
                    {
                        result.Type = OpenGaussDbType.Array | OpenGaussDbType.Integer;
                    }
                    else if (key == "_int2")
                    {
                        result.Type = OpenGaussDbType.Array | OpenGaussDbType.Smallint;
                    }
                    else if (key == "_int8")
                    {
                        result.Type = OpenGaussDbType.Array | OpenGaussDbType.Bigint;
                    }
                    else
                    {
                        var type = PgSqlType[key.Substring(1)];
                        result.Type = OpenGaussDbType.Array | type;
                    }
                }
                else
                {
                    result.Type = null;
                }
                columnViews.Add(result);
            }
            using (var writer = conn.BeginBinaryImport(copyString))
            {
                foreach (DataRow row in dt.Rows)
                {
                    writer.StartRow();
                    foreach (var column in columnViews)
                    {
                        var value = row[column.DataColumn.ColumnName];
                        if (value == null)
                        {
                            value = DBNull.Value;
                        }
                        //else if (value is double&&this.Context?.CurrentConnectionConfig?.MoreSettings?.DatabaseModel==null) 
                        //{
                        //    column.Type = NpgsqlDbType.Double;
                        //}
                        if (column.Type == null)
                        {
                            writer.Write(value);
                        }
                        else
                        {
                            writer.Write(value, column.Type.Value);
                        }
                    }
                }
                writer.Complete();
            }
        }
        public class ColumnView
        {
            public DataColumn DataColumn { get; set; }
            public EntityColumnInfo EntityColumnInfo { get; set; }
            public DbColumnInfo DbColumnInfo { get; set; }
            public OpenGaussDbType? Type { get; set; }
        }

    }
}
