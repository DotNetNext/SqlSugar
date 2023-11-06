using Dm;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
   
    public class DmFastBuilder:FastBuilder,IFastBuilder
    {
        public override bool IsActionUpdateColumns { get; set; } = true;
        public override DbFastestProperties DbFastestProperties { get; set; } = new DbFastestProperties() {
          HasOffsetTime=true
        };
        public async Task<int> ExecuteBulkCopyAsync(DataTable dt)
        {
            if (DbFastestProperties?.IsOffIdentity == true)
            {
                var isNoTran = this.Context.Ado.IsNoTran()&&this.Context.CurrentConnectionConfig.IsAutoCloseConnection;
                try
                {
                    if(isNoTran)
                     this.Context.Ado.BeginTran();

                    this.Context.Ado.ExecuteCommand($"SET IDENTITY_INSERT {dt.TableName} ON");
                     var result=await _Execute(dt);
                    this.Context.Ado.ExecuteCommand($"SET IDENTITY_INSERT {dt.TableName} OFF");

                    if (isNoTran)
                        this.Context.Ado.CommitTran();

                    return result;
                }
                catch (Exception)
                {
                    if (isNoTran)
                        this.Context.Ado.CommitTran();
                    throw;
                }
            }
            else
            {
                return await _Execute(dt);
            }
        }
        public override async Task CreateTempAsync<T>(DataTable dt)
        {
            var queryable = this.Context.Queryable<T>();
            var tableName = queryable.SqlBuilder.GetTranslationTableName(dt.TableName);
            dt.TableName = "temp" + SnowFlakeSingle.instance.getID();
            var sql = queryable.AS(tableName).Where(it => false).ToSql().Key;
            await this.Context.Ado.ExecuteCommandAsync($"CREATE  TABLE {dt.TableName}    as ( {sql} ) ");
        }
        public override string UpdateSql { get; set; } = @"UPDATE  {1} TM    INNER JOIN {2} TE  ON {3} SET {0} ";

        private async Task<int> _Execute(DataTable dt)
        {
            DmBulkCopy bulkCopy = GetBulkCopyInstance();
            bulkCopy.DestinationTableName = dt.TableName;
            try
            {
                bulkCopy.WriteToServer(dt);
                await Task.Delay(0);//No Support Async
            }
            catch (Exception ex)
            {
                CloseDb();
                throw ex;
            }
            CloseDb();
            return dt.Rows.Count;
        }

        public DmBulkCopy GetBulkCopyInstance()
        {
            DmBulkCopy copy;
            if (this.Context.Ado.Transaction == null)
            {
                copy = new DmBulkCopy((DmConnection)this.Context.Ado.Connection);
            }
            else
            {
                copy = new DmBulkCopy((DmConnection)this.Context.Ado.Connection, DmBulkCopyOptions.Default, (DmTransaction)this.Context.Ado.Transaction);
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
