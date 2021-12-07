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

        public override string UpdateSql { get; set; } = "UPDATE (SELECT {4}  FROM {2} TM,{3} TE WHERE {1})SET {0}";
        public override async Task CreateTempAsync<T>(DataTable dt)
        {
            //await Task.FromResult(0);
            //throw new Exception("Oracle no support BulkUpdate");
            var oldTableName = dt.TableName;
            var columns = this.Context.EntityMaintenance.GetEntityInfo<T>().Columns.Where(it => it.IsPrimarykey).Select(it => it.DbColumnName).ToArray();
            dt.TableName = "Temp" + SnowFlakeSingle.instance.getID().ToString();
            var sql = this.Context.Queryable<T>().AS(oldTableName).Where(it => false).Select("*").ToSql().Key;
            await this.Context.Ado.ExecuteCommandAsync($"create table {dt.TableName} as {sql} ");
            this.Context.DbMaintenance.AddPrimaryKeys(dt.TableName, columns,"Pk_"+ SnowFlakeSingle.instance.getID().ToString());
            //var xxx = this.Context.Queryable<T>().AS(dt.TableName).ToList();
        }
        public override async Task<int> UpdateByTempAsync(string tableName, string tempName, string[] updateColumns, string[] whereColumns)
        {
            var sqlBuilder = this.Context.Queryable<object>().SqlBuilder;
            Check.ArgumentNullException(!updateColumns.Any(), "update columns count is 0");
            Check.ArgumentNullException(!whereColumns.Any(), "where columns count is 0");
            var sets = string.Join(",", updateColumns.Select(it => $"TM{it}=TE{it}"));
            var wheres = string.Join(" AND ", whereColumns.Select(it => $"TM.{sqlBuilder.GetTranslationColumnName(it)}=TE.{sqlBuilder.GetTranslationColumnName(it)}"));
            var forms= string.Join(",", updateColumns.Select(it => $" TM.{sqlBuilder.GetTranslationColumnName(it)} TM{it},TE.{sqlBuilder.GetTranslationColumnName(it)} TE{it}")); ;
            string sql = string.Format(UpdateSql, sets, wheres,tableName, tempName, forms);
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
