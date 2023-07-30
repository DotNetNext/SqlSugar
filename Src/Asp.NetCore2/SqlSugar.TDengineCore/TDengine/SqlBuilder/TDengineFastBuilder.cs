
using SqlSugar.TDengineAdo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar.TDengine
{
    public class TDengineFastBuilder : FastBuilder, IFastBuilder
    {
        
        private EntityInfo entityInfo;

        public TDengineFastBuilder(EntityInfo entityInfo)
        {
            this.entityInfo = entityInfo;
        }

        public override string UpdateSql { get; set; } = @"UPDATE  {1}    SET {0}  FROM   {2}  AS TE  WHERE {3}
";

        //public virtual async Task<int> UpdateByTempAsync(string tableName, string tempName, string[] updateColumns, string[] whereColumns)
        //{
        //    Check.ArgumentNullException(!updateColumns.Any(), "update columns count is 0");
        //    Check.ArgumentNullException(!whereColumns.Any(), "where columns count is 0");
        //    var sets = string.Join(",", updateColumns.Select(it => $"TM.{it}=TE.{it}"));
        //    var wheres = string.Join(",", whereColumns.Select(it => $"TM.{it}=TE.{it}"));
        //    string sql = string.Format(UpdateSql, sets, tableName, tempName, wheres);
        //    return await this.Context.Ado.ExecuteCommandAsync(sql);
        //}
        public async Task<int> ExecuteBulkCopyAsync(DataTable dt)
        {
            return 0;
        }

        private  void BulkCopy(DataTable dt, string copyString, TDengineConnection conn, List<DbColumnInfo> columns)
        {
            throw new NotSupportedException();
        }

    
        public override async Task<int> UpdateByTempAsync(string tableName, string tempName, string[] updateColumns, string[] whereColumns)
        {
            throw new  NotSupportedException();
        }
        public override async Task CreateTempAsync<T>(DataTable dt) 
        {
            throw new NotSupportedException();
        } 
    }
}
