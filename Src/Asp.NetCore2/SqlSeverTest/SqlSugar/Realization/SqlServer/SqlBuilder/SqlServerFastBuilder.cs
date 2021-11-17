using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public class SqlServerFastBuilder: IFastBuilder
    {

        public SqlSugarProvider Context { get; set;  }

        public string UpdateSql { get; set; } = @"UPDATE TM
                                                    SET  {0}
                                                    FROM {1} TM
                                                    INNER JOIN {2} TE ON {3} ";

        public async Task<int> ExecuteBulkCopyAsync(DataTable dt)
        {

            SqlBulkCopy bulkCopy = GetBulkCopyInstance();
            bulkCopy.DestinationTableName = dt.TableName;
            try
            {
                await bulkCopy.WriteToServerAsync(dt);
            }
            catch (Exception ex)
            {
                CloseDb();
                throw ex;
            }
            CloseDb();
            return dt.Rows.Count;
        }
        public SqlBulkCopy GetBulkCopyInstance()
        {
            SqlBulkCopy copy;
            if (this.Context.Ado.Transaction == null)
            {
                copy = new SqlBulkCopy((SqlConnection)this.Context.Ado.Connection);
            }
            else
            {
                copy = new SqlBulkCopy((SqlConnection)this.Context.Ado.Connection, SqlBulkCopyOptions.CheckConstraints, (SqlTransaction)this.Context.Ado.Transaction);
            }
            if (this.Context.Ado.Connection.State == ConnectionState.Closed)
            {
                this.Context.Ado.Connection.Open();
            }
            return copy;
        }
        public void CloseDb()
        {
            if (this.Context.CurrentConnectionConfig.IsAutoCloseConnection && this.Context.Ado.Transaction == null)
            {
                this.Context.Ado.Connection.Close();
            }
        }

        public async Task<int> UpdateByTempAsync(string tableName, string tempName, string[] updateColumns, string[] whereColumns)
        {
            Check.ArgumentNullException(!updateColumns.Any(),"update columns count is 0");
            Check.ArgumentNullException(!whereColumns.Any(), "where columns count is 0");
            var sets = string.Join(",", updateColumns.Select(it=>$"TM.{it}=TE.{it}"));
            var wheres = string.Join(",", whereColumns.Select(it => $"TM.{it}=TE.{it}"));
            string sql = string.Format(UpdateSql,sets, tableName,tempName, wheres);
            return await this.Context.Ado.ExecuteCommandAsync(sql);
        }

        public async Task CreateTempAsync<T>(DataTable dt) where T :class,new()
        {
            await this.Context.UnionAll(
                this.Context.Queryable<T>().Where(it => false).AS(dt.TableName), 
                this.Context.Queryable<T>().Where(it => false).AS(dt.TableName)).Select("top 1 * into #temp").ToListAsync();
            dt.TableName = "#temp";
        }
    }
}
