using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public class PostgreSQLFastBuilder : FastBuilder, IFastBuilder
    {
        public static Dictionary<string , NpgsqlDbType> PgSqlType = UtilMethods.EnumToDictionary<NpgsqlDbType>();
        private EntityInfo entityInfo;

        public PostgreSQLFastBuilder(EntityInfo entityInfo)
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
            List<string> lsColNames = new List<string>();
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                lsColNames.Add($"\"{dt.Columns[i].ColumnName}\"");
            }
            string copyString = $"COPY  {dt.TableName} ( {string.Join(",", lsColNames) } ) FROM STDIN (FORMAT BINARY)";
            NpgsqlConnection conn = (NpgsqlConnection)this.Context.Ado.Connection;
            var columns = this.Context.DbMaintenance.GetColumnInfosByTableName(this.entityInfo.DbTableName);
            try
            {
                var identityColumnInfo = this.entityInfo.Columns.FirstOrDefault(it => it.IsIdentity);
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

        private  void BulkCopy(DataTable dt, string copyString, NpgsqlConnection conn, List<DbColumnInfo> columns)
        {
            if (conn.State == ConnectionState.Closed)
                conn.Open();
            List<ColumnView> columnViews = new List<ColumnView>();
            foreach (DataColumn item in dt.Columns)
            {
                ColumnView result = new ColumnView();
                result.DbColumnInfo = columns.FirstOrDefault(it => it.DbColumnName.EqualCase(item.ColumnName));
                result.DataColumn = item;
                result.EntityColumnInfo=this.entityInfo.Columns.FirstOrDefault(it => it.DbColumnName.EqualCase(item.ColumnName));
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
                    var type = PgSqlType[key.Substring(1)];
                    result.Type = NpgsqlDbType.Array | type;
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

    
        public override async Task<int> UpdateByTempAsync(string tableName, string tempName, string[] updateColumns, string[] whereColumns)
        {
            var sqlquerybulder= this.Context.Queryable<object>().SqlBuilder;
            Check.ArgumentNullException(!updateColumns.Any(), "update columns count is 0");
            Check.ArgumentNullException(!whereColumns.Any(), "where columns count is 0");
            var sets = string.Join(",", updateColumns.Select(it => $"{sqlquerybulder.GetTranslationColumnName(it)}=TE.{sqlquerybulder.GetTranslationColumnName(it)}"));
            var wheres = string.Join(" AND ", whereColumns.Select(it => $"{tableName}.{sqlquerybulder.GetTranslationColumnName(it)}=TE.{sqlquerybulder.GetTranslationColumnName(it)}"));
            string sql = string.Format(UpdateSql, sets, tableName, tempName, wheres);
            return await this.Context.Ado.ExecuteCommandAsync(sql);
        }
        public override async Task CreateTempAsync<T>(DataTable dt) 
        {
            await this.Context.Queryable<T>().Where(it => false).AS(dt.TableName).Select("  * into  temp mytemptable").ToListAsync();
            dt.TableName = "mytemptable";
        }

        public class ColumnView
        {
            public DataColumn DataColumn { get; set; }
            public EntityColumnInfo EntityColumnInfo { get; set; }
            public DbColumnInfo DbColumnInfo { get; set; }
            public NpgsqlDbType? Type { get; set; }
        }
    }
}
