using GBS.Data.GBasedbt;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar.GBase
{
    public class GBaseFastBuilder : IFastBuilder
    {
        public EntityInfo FastEntityInfo { get; set; }
        public string CharacterSet { get; set; }
        public bool IsActionUpdateColumns { get; set; }
        public DbFastestProperties DbFastestProperties { get; set; } = new DbFastestProperties()
        {
            IsMerge = true
        };

        public GBaseFastBuilder()
        {
        }

        public SqlSugarProvider Context { get; set; }

        public void CloseDb()
        {
            if (this.Context.CurrentConnectionConfig.IsAutoCloseConnection && this.Context.Ado.Transaction == null)
            {
                this.Context.Close();
            }
        }

        public async Task CreateTempAsync<T>(DataTable dt) where T : class, new()
        {
            var sqlBuilder = this.Context.Queryable<object>().SqlBuilder;
            var dts = dt.Columns.Cast<DataColumn>().Select(it => sqlBuilder.GetTranslationColumnName(it.ColumnName)).ToList();

            var oldTableName = dt.TableName;
            var columns = this.Context.EntityMaintenance.GetEntityInfo<T>().Columns.Where(it => it.IsPrimarykey).Select(it => it.DbColumnName).ToList();
            dt.TableName = "Temp" + SnowFlakeSingle.instance.getID().ToString();
            if (columns.Count() == 0 && DbFastestProperties != null && DbFastestProperties.WhereColumns.HasValue())
            {
                columns.AddRange(DbFastestProperties.WhereColumns);
            }
            var sql = this.Context.Queryable<T>().AS(oldTableName).Where(it => false).Select(string.Join(",", dts)).ToSql().Key;
            await this.Context.Ado.ExecuteCommandAsync($"CREATE TABLE {dt.TableName} AS {sql} ");
            this.Context.DbMaintenance.AddPrimaryKeys(dt.TableName, columns.ToArray(), "Pk_" + SnowFlakeSingle.instance.getID().ToString());
        }

        public async Task<int> ExecuteBulkCopyAsync(DataTable dt)
        {
            foreach (var item in this.FastEntityInfo.Columns)
            {
                if (item.IsIdentity && dt.Columns.Contains(item.DbColumnName))
                {
                    dt.Columns.Remove(item.DbColumnName);
                }
            }
            var dictionary = this.Context.Utilities.DataTableToDictionaryList(dt.Rows.Cast<DataRow>().Take(1).CopyToDataTable());
            int result = 0;
            var cn = this.Context.Ado.Connection as GbsConnection;
            Open(cn);
            if (this.Context.Ado.Transaction == null)
            {
                using (var transaction = cn.BeginTransaction())
                {
                    result = await _BulkCopy(dt, dictionary, cn, transaction);
                    transaction.Commit();
                }
            }
            else
            {
                result = await _BulkCopy(dt, dictionary, cn, this.Context.Ado.Transaction);
            }

            return result;
        }

        public async Task<int> UpdateByTempAsync(string tableName, string tempName, string[] updateColumns, string[] whereColumns)
        {
            Check.ArgumentNullException(!updateColumns.Any(), "update columns count is 0");
            Check.ArgumentNullException(!whereColumns.Any(), "where columns count is 0");
            var sqlBuilder = this.Context.Queryable<object>().SqlBuilder;
            var whereSql = string.Join("  AND  ", whereColumns.Select(it => $"tgt.{sqlBuilder.GetTranslationColumnName(it)}=src.{sqlBuilder.GetTranslationColumnName(it)}"));
            var updateColumnsSql = string.Join(" , ", updateColumns.Select(it => $"tgt.{sqlBuilder.GetTranslationColumnName(it)}=src.{sqlBuilder.GetTranslationColumnName(it)}"));
            var sql = $@"MERGE INTO {sqlBuilder.GetTranslationColumnName(tableName)} tgt
USING {sqlBuilder.GetTranslationColumnName(tempName)} src
ON ({whereSql})
WHEN MATCHED THEN
    UPDATE SET {updateColumnsSql}";

            return await this.Context.Ado.ExecuteCommandAsync(sql);
        }

        public async Task<int> Merge<T>(string tableName, DataTable dt, EntityInfo entityInfo, string[] whereColumns, string[] updateColumns, List<T> datas) where T : class, new()
        {
            Check.Exception(this.FastEntityInfo.Columns.Any(it => it.OracleSequenceName.HasValue()), "The BulkMerge method cannot be used for  sequence", "BulkMerge方法不能用序列");
            var sqlBuilder = this.Context.Queryable<object>().SqlBuilder;
            var insertColumns = entityInfo.Columns
                .Where(it => it.IsIgnore == false)
                .Where(it => it.IsIdentity == false)
                .Where(it => it.InsertServerTime == false)
                .Where(it => it.InsertSql == null)
                .Where(it => it.OracleSequenceName == null)
                .Where(it => it.IsOnlyIgnoreInsert == false);
            var whereSql = string.Join("  AND  ", whereColumns.Select(it => $"tgt.{sqlBuilder.GetTranslationColumnName(it)}=src.{sqlBuilder.GetTranslationColumnName(it)}"));
            var updateColumnsSql = string.Join(" , ", updateColumns.Select(it => $"tgt.{sqlBuilder.GetTranslationColumnName(it)}=src.{sqlBuilder.GetTranslationColumnName(it)}"));
            var insertColumnsSqlTgt = string.Join(" , ", insertColumns.Select(it => "tgt." + sqlBuilder.GetTranslationColumnName(it.DbColumnName)));
            var insertColumnsSqlsrc = string.Join(" , ", insertColumns.Select(it => "src." + sqlBuilder.GetTranslationColumnName(it.DbColumnName)));
            var sql = $@"MERGE INTO {sqlBuilder.GetTranslationColumnName(tableName)} tgt
USING {sqlBuilder.GetTranslationColumnName(dt.TableName)} src
ON ({whereSql})
WHEN MATCHED THEN
    UPDATE SET {updateColumnsSql}
WHEN NOT MATCHED THEN
    INSERT ({insertColumnsSqlTgt})
    VALUES ({insertColumnsSqlsrc})";

            return await this.Context.Ado.ExecuteCommandAsync(sql);
        }

        private DataTable GetSchemaTable(GbsConnection cn, IDbTransaction transaction, string table_name)
        {
            DataTable schema = new DataTable();
            schema.Columns.Add(new DataColumn("ColumnName", typeof(System.String)));
            schema.Columns.Add(new DataColumn("ColumnSize", typeof(System.Int32)));
            schema.Columns.Add(new DataColumn("ProviderType", typeof(System.Int32)));
            schema.PrimaryKey = new DataColumn[] { schema.Columns[0] };

            using (var cmd_meta = cn.CreateCommand())
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT c.colname, c.coltype, c.collength, c.extended_id, ext.name ");
                sb.Append("FROM systables t ");
                sb.Append("INNER JOIN syscolumns c ON t.tabid = c.tabid ");
                sb.Append("LEFT JOIN sysxtdtypes ext ON ext.extended_id = c.extended_id ");
                sb.Append("WHERE tabname = '" + table_name.ToLower() + "'");

                cmd_meta.CommandText = sb.ToString();
                cmd_meta.Transaction = (GbsTransaction)transaction;
                var reader = cmd_meta.ExecuteReader();

                while (reader.Read())
                {
                    DataRow row = schema.NewRow();
                    row["ColumnName"] = reader.GetString(0);
                    row["ColumnSize"] = reader.GetInt32(2);

                    GbsType provider_type = (GbsType)(reader.GetInt16(1) & 0xFF);
                    int extended_type = reader.GetInt32(3);
                    switch ((GbsType)provider_type)
                    {
                        case GbsType.SQLUDTFixed:
                            switch (extended_type)
                            {
                                case 11:
                                    provider_type = GbsType.Clob;
                                    break;
                                case 10:
                                    provider_type = GbsType.Blob;
                                    break;
                                case 5:
                                    provider_type = GbsType.Boolean;
                                    break;
                            }
                            break;
                        case GbsType.SQLUDTVar:
                            switch (extended_type)
                            {
                                case 1:
                                    provider_type = GbsType.LVarChar;
                                    break;
                            }
                            break;
                        case GbsType.Char1:
                            provider_type = GbsType.Char;
                            break;
                        case GbsType.BigSerial:
                            provider_type = GbsType.BigInt;
                            break;
                        case GbsType.Serial:
                            provider_type = GbsType.Integer;
                            break;
                        default:
                            switch (reader.GetString(4))
                            {
                                case "guid":
                                    provider_type = GbsType.Guid;
                                    break;
                            }
                            break;
                    }
                    row["ProviderType"] = provider_type;
                    schema.Rows.Add(row);
                }
            }

            return schema;
        }

        private void SetupParameters(GbsCommand cmd, DataTable dt, DataTable schema)
        {
            foreach (DataColumn col in dt.Columns.Cast<DataColumn>().OrderBy(o=>o.ColumnName))
            {
                DataRow schema_row = null;
                schema_row = schema.Rows.Find(col.ColumnName);
                if (schema_row != null)
                {
                    int param_len = 0;
                    GbsType gbs_type = (GbsType)schema_row["ProviderType"];
                    switch (gbs_type)
                    {
                        case GbsType.Char1:
                        case GbsType.Char:
                        case GbsType.NChar:
                        case GbsType.VarChar:
                        case GbsType.NVarChar:
                        case GbsType.LVarChar:
                            // assigned column size for char type
                            param_len = (int)schema_row["ColumnSize"];
                            break;
                    }

                    cmd.Parameters.Add(new GbsParameter(col.ColumnName, gbs_type, param_len, col.ColumnName));
                }
            }
        }

        private string GetInsertSql(string table_name, Dictionary<string, object> dictionary)
        {
            string sql = this.Context.Insertable(dictionary).AS(table_name).ToSql().Key;

            // the insert statement is followed by a select statement.
            // remove the select sql statement.
            int index = sql.IndexOf(UtilConstants.ReplaceCommaKey.Replace("{", "").Replace("}", ""));
            sql = (index != -1)
                ? sql.Substring(0, index)
                : sql;

            // replace the @param with ?
            foreach (var k in dictionary)
            {
                sql = sql.Replace("@" + k.Key, " ?");
            }

            return sql;
        }

        private async Task<int> _BulkCopy(DataTable dt, List<Dictionary<string, object>> dictionary, GbsConnection cn, IDbTransaction transaction)
        {
            int result = 0;
            using (var cmd = cn.CreateCommand())
            {
                cmd.CommandText = GetInsertSql(dt.TableName, dictionary.First());
                cmd.Transaction = (GbsTransaction)transaction;

                DataTable schema = GetSchemaTable(cn, transaction, dt.TableName);
                SetupParameters(cmd, dt, schema);

                bool insertOneByOne = false;
                foreach (GbsParameter param in cmd.Parameters)
                {
                    if (param.GbsType == GbsType.NVarChar ||
                      param.GbsType == GbsType.Guid ||
                      param.GbsType == GbsType.Byte ||
                      param.GbsType == GbsType.Blob ||
                      param.GbsType == GbsType.Text ||
                      param.GbsType == GbsType.Date ||
                      param.GbsType == GbsType.Clob)
                    {
                        // some data type does not support insert cursor feature.
                        // insert data row by row.
                        insertOneByOne = true;
                        break;
                    }
                }

                if (insertOneByOne)
                {
                    cmd.Prepare();
                    foreach (DataRow dataRow in dt.Rows)
                    {
                        foreach (DataColumn item in dt.Columns)
                        {
                            cmd.Parameters[item.ColumnName].Value = dataRow[item.ColumnName];
                        }
                        result += await cmd.ExecuteNonQueryAsync();
                    }
                }
                else
                {
                    // use insert cursor
                    GbsDataAdapter adapter = new GbsDataAdapter();
                    cmd.UpdatedRowSource = UpdateRowSource.None;
                    adapter.InsertCommand = cmd;
                    adapter.UpdateBatchSize = dt.Rows.Count;
                    result = adapter.Update(dt);
                }
            }

            return result;
        }

        private static void Open(GbsConnection cn)
        {
            if (cn.State != ConnectionState.Open)
            {
                cn.Open();
            }
        }
    }
}
