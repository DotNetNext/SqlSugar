using ClickHouse;
using ClickHouse.Client.ADO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar.ClickHouse
{
    public class ClickHouseFastBuilder : FastBuilder, IFastBuilder
    {
     
        public ClickHouseFastBuilder(EntityInfo entityInfo)
        {
            throw new NotSupportedException("NotSupportedException");
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
            throw new NotSupportedException("NotSupportedException");
        }

        private  void BulkCopy(DataTable dt, string copyString, ClickHouseConnection conn, List<DbColumnInfo> columns)
        {
            throw new NotSupportedException("NotSupportedException");
        }
     
    }
}
