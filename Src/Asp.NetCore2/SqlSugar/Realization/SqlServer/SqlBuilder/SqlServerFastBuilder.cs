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
   
    public class SqlServerFastBuilder:FastBuilder,IFastBuilder
    {
        public override bool IsActionUpdateColumns { get; set; } = true;
        public override DbFastestProperties DbFastestProperties { get; set; } = new DbFastestProperties() {
          HasOffsetTime=true,
          IsMerge=true
        };
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
                if (this.DbFastestProperties?.IsOffIdentity == true)
                { 
                    copy = new SqlBulkCopy((SqlConnection)this.Context.Ado.Connection, SqlBulkCopyOptions.KeepIdentity,null);
                }
                else
                {
                    copy = new SqlBulkCopy((SqlConnection)this.Context.Ado.Connection);
                }
            }
            else
            {
                if (this.DbFastestProperties?.IsOffIdentity == true)
                {
                    copy = new SqlBulkCopy((SqlConnection)this.Context.Ado.Connection, SqlBulkCopyOptions.KeepIdentity, (SqlTransaction)this.Context.Ado.Transaction);
                }
                else
                {

                    copy = new SqlBulkCopy((SqlConnection)this.Context.Ado.Connection, SqlBulkCopyOptions.CheckConstraints, (SqlTransaction)this.Context.Ado.Transaction);
                }
            }
            if (this.Context.Ado.Connection.State == ConnectionState.Closed)
            {
                this.Context.Ado.Connection.Open();
            }
            copy.BulkCopyTimeout = this.Context.Ado.CommandTimeOut;
            return copy;
        }
        public override Task<int> Merge<T>(string tableName,DataTable dt, EntityInfo entityInfo, string[] whereColumns, string[] updateColumns, List<T> datas) 
        {
            var sqlBuilder = this.Context.Queryable<object>().SqlBuilder;
            var insertColumns = entityInfo.Columns
                .Where(it => it.IsIgnore == false)
                .Where(it => it.IsIdentity == false)
                .Where(it => it.OracleSequenceName == null)
                .Where(it => it.InsertServerTime == false)
                .Where(it => it.InsertSql == null)
                .Where(it => it.IsOnlyIgnoreInsert == false);
            var whereSql = string.Join(" AND ", whereColumns.Select(it => $"tgt.{sqlBuilder.GetTranslationColumnName(it)}=src.{sqlBuilder.GetTranslationColumnName(it)}"));
            var updateColumnsSql = string.Join(" , ", updateColumns.Select(it => $"tgt.{sqlBuilder.GetTranslationColumnName(it)}=src.{sqlBuilder.GetTranslationColumnName(it)}"));
            var insertColumnsSqlTgt = string.Join(" , ", insertColumns.Select(it =>  sqlBuilder.GetTranslationColumnName(it.DbColumnName)));
            var insertColumnsSqlsrc = string.Join(" , ", insertColumns.Select(it => "src." + sqlBuilder.GetTranslationColumnName(it.DbColumnName)));
            var sql = $@"MERGE INTO {sqlBuilder.GetTranslationColumnName(tableName)} tgt
USING {sqlBuilder.GetTranslationColumnName(dt.TableName)} src
ON ({whereSql})
WHEN MATCHED THEN
    UPDATE SET {updateColumnsSql}
WHEN NOT MATCHED THEN
    INSERT ({insertColumnsSqlTgt})
    VALUES ({insertColumnsSqlsrc});";

            return this.Context.Ado.ExecuteCommandAsync(sql);
        }
    }
}
