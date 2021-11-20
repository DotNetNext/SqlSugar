using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public class OracleFastBuilder : FastBuilder, IFastBuilder
    {
        private EntityInfo entityInfo;

        public OracleFastBuilder(EntityInfo entityInfo)
        {
            this.entityInfo = entityInfo;
        }

        public override string UpdateSql { get; set; } = "UPDATE (SELECT A.NAME ANAME,B.NAME BNAME FROM A,B WHERE A.ID=B.ID)SET ANAME = BNAME;";
        public override async Task CreateTempAsync<T>(DataTable dt)
        {
            await Task.FromResult(0);
            throw new Exception("Oracle no support BulkUpdate");
            //dt.TableName = "T" + SnowFlakeSingle.instance.getID().ToString().Substring(4,16);
            //var sql = this.Context.Queryable<T>().Where(it => false).Select("*").ToSql().Key;
            //await this.Context.Ado.ExecuteCommandAsync($"create global temporary table {dt.TableName} as {sql} ");
            //var xxx = this.Context.Queryable<T>().AS(dt.TableName).ToList();
        }
        public override async Task<int> UpdateByTempAsync(string tableName, string tempName, string[] updateColumns, string[] whereColumns)
        {
            Check.ArgumentNullException(!updateColumns.Any(), "update columns count is 0");
            Check.ArgumentNullException(!whereColumns.Any(), "where columns count is 0");
            var sets = string.Join(",", updateColumns.Select(it => $"TM.{it}=TE.{it}"));
            var wheres = string.Join(",", whereColumns.Select(it => $"TM.{it}=TE.{it}"));
            string sql = string.Format(UpdateSql, sets, tableName, tempName, wheres);
            return await this.Context.Ado.ExecuteCommandAsync(sql);
        }
        private OracleBulkCopy GetBulkCopyInstance()

        {
            if (this.Context.Ado.Connection.State == ConnectionState.Closed)
            {
                this.Context.Ado.Connection.Open();
            }

            OracleBulkCopy copy;
            if (this.Context.Ado.Transaction == null)
            {
                copy = new OracleBulkCopy((OracleConnection)this.Context.Ado.Connection, Oracle.ManagedDataAccess.Client.OracleBulkCopyOptions.Default);
            }
            else
            {
                copy = new OracleBulkCopy((OracleConnection)this.Context.Ado.Connection, OracleBulkCopyOptions.UseInternalTransaction);
            }
            return copy;

        }
        public Task<int> ExecuteBulkCopyAsync(DataTable dt)
        {
            var identityColumnInfo = this.entityInfo.Columns.FirstOrDefault(it => it.IsIdentity);
            if (identityColumnInfo != null)
            {
                throw new Exception("Oracle bulkcopy no support identity");
            }
            OracleBulkCopy copy = GetBulkCopyInstance();
            try
            {
                copy.DestinationTableName = dt.TableName;
                copy.WriteToServer(dt);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally 
            {
                CloseDb();
            }
            return Task.FromResult(dt.Rows.Count);
        }
    }
}
