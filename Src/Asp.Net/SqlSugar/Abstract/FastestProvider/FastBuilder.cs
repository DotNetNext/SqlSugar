using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public class FastBuilder 
    {
        public SqlSugarProvider Context { get; set; }

        public virtual string UpdateSql { get; set; } = @"UPDATE TM
                                                    SET  {0}
                                                    FROM {1} TM
                                                    INNER JOIN {2} TE ON {3} ";

      
        public virtual void CloseDb()
        {
            if (this.Context.CurrentConnectionConfig.IsAutoCloseConnection && this.Context.Ado.Transaction == null)
            {
                this.Context.Ado.Connection.Close();
            }
        }

        public  virtual async Task<int> UpdateByTempAsync(string tableName, string tempName, string[] updateColumns, string[] whereColumns)
        {
            var sqlbuilder = this.Context.Queryable<object>().SqlBuilder;
            Check.ArgumentNullException(!updateColumns.Any(), "update columns count is 0");
            Check.ArgumentNullException(!whereColumns.Any(), "where columns count is 0");
            var sets = string.Join(",", updateColumns.Select(it => $"TM.{sqlbuilder.GetTranslationColumnName(it)}=TE.{sqlbuilder.GetTranslationColumnName(it)}"));
            var wheres = string.Join(" AND ", whereColumns.Select(it => $"TM.{sqlbuilder.GetTranslationColumnName(it)}=TE.{sqlbuilder.GetTranslationColumnName(it)}"));
            string sql = string.Format(UpdateSql, sets, tableName, tempName, wheres);
            return await this.Context.Ado.ExecuteCommandAsync(sql);
        }

        public virtual async Task CreateTempAsync<T>(DataTable dt) where T : class, new()
        {
            await this.Context.UnionAll(
                this.Context.Queryable<T>().Select("*").Where(it => false).AS(dt.TableName),
                this.Context.Queryable<T>().Select("*").Where(it => false).AS(dt.TableName)).Select("top 1 * into #temp").ToListAsync();
            dt.TableName = "#temp";
        }
    }
}
