using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar.OceanBaseForOracle
{
   
    public class OceanBaseForOracleFastBuilder : FastBuilder,IFastBuilder
    {
        private EntityInfo entityInfo;

        public OceanBaseForOracleFastBuilder(EntityInfo entityInfo)
        {
            this.entityInfo = entityInfo;
        }

        public override string UpdateSql { get; set; } = "UPDATE (SELECT {4}  FROM {2} TM,{3} TE WHERE {1})SET {0}";
        public override async Task CreateTempAsync<T>(DataTable dt)
        {
            var sqlBuilder = this.Context.Queryable<object>().SqlBuilder;
            var dts = dt.Columns.Cast<DataColumn>().Select(it => sqlBuilder.GetTranslationColumnName(it.ColumnName)).ToList();
            //await Task.FromResult(0);
            //throw new Exception("Oracle no support BulkUpdate");
            var oldTableName = dt.TableName;
            var columns = this.Context.EntityMaintenance.GetEntityInfo<T>().Columns.Where(it => it.IsPrimarykey).Select(it => it.DbColumnName).ToList();
            dt.TableName = "Temp" + SnowFlakeSingle.instance.getID().ToString();
            if (columns.Count() == 0 && DbFastestProperties != null && DbFastestProperties.WhereColumns.HasValue())
            {
                columns.AddRange(DbFastestProperties.WhereColumns);
            }
            var sql = this.Context.Queryable<T>().AS(oldTableName).Where(it => false).Select(string.Join(",", dts)).ToSql().Key;
            await this.Context.Ado.ExecuteCommandAsync($"create table {dt.TableName} as {sql} ");
            this.Context.DbMaintenance.AddPrimaryKeys(dt.TableName, columns.ToArray(), "Pk_" + SnowFlakeSingle.instance.getID().ToString());
        }
        public override async Task<int> UpdateByTempAsync(string tableName, string tempName, string[] updateColumns, string[] whereColumns)
        {
            var sqlBuilder = this.Context.Queryable<object>().SqlBuilder;
            Check.ArgumentNullException(!updateColumns.Any(), "update columns count is 0");
            Check.ArgumentNullException(!whereColumns.Any(), "where columns count is 0");
            var sets = string.Join(",", updateColumns.Select(it => $"TM{it}=TE{it}"));
            var wheres = string.Join(" AND ", whereColumns.Select(it => $"TM.{sqlBuilder.GetTranslationColumnName(it)}=TE.{sqlBuilder.GetTranslationColumnName(it)}"));
            var forms = string.Join(",", updateColumns.Select(it => $" TM.{sqlBuilder.GetTranslationColumnName(it)} TM{it},TE.{sqlBuilder.GetTranslationColumnName(it)} TE{it}")); ;
            string sql = string.Format(UpdateSql, sets, wheres, tableName, tempName, forms);
            return await this.Context.Ado.ExecuteCommandAsync(sql);
        }

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
            copy.BulkCopyTimeout = this.Context.Ado.CommandTimeOut;
            return copy;
        }
    }
}
