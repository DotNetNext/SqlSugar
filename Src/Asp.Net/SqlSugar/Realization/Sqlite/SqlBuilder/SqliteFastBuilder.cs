﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public class SqliteFastBuilder : IFastBuilder
    {
        public EntityInfo FastEntityInfo { get; set; }
        private EntityInfo entityInfo;
        private bool IsUpdate = false;
        public string CharacterSet { get; set; }
        private DataTable UpdateDataTable { get; set; }
        public bool IsActionUpdateColumns { get; set; }
        public DbFastestProperties DbFastestProperties { get; set; } = new DbFastestProperties();
        public SqliteFastBuilder(EntityInfo entityInfo)
        {
            this.entityInfo = entityInfo;
        }

        public SqlSugarProvider Context { get; set; }

        public void CloseDb()
        {
            if (this.Context.CurrentConnectionConfig.IsAutoCloseConnection)
            {
                this.Context.Close();
            }
        }

        public async Task CreateTempAsync<T>(DataTable dt) where T : class, new()
        {
            await Task.Delay(0);
            IsUpdate = true;
        }


        public async Task<int> ExecuteBulkCopyAsync(DataTable dt)
        {
            if (dt.Rows.Count == 0||IsUpdate)
            {
                this.UpdateDataTable = dt;
                return 0;
            }
            foreach (var item in this.entityInfo.Columns)
            {
                if (item.IsIdentity && dt.Columns.Contains(item.DbColumnName))
                {
                    dt.Columns.Remove(item.DbColumnName);
                }
            }
            var dictionary = this.Context.Utilities.DataTableToDictionaryList(dt.Rows.Cast<DataRow>().Take(1).CopyToDataTable());
            int result = 0;
            var cn = this.Context.Ado.Connection as SQLiteConnection;
            Open(cn);
            if (this.Context.Ado.Transaction == null)
            {
                using (var transaction = cn.BeginTransaction())
                {
                    result = await _BulkCopy(dt, dictionary, result, cn);
                    transaction.Commit();
                }
            }
            else 
            {
                result = await _BulkCopy(dt, dictionary, result, cn);
            }
            return result;
        }

        private async Task<int> _BulkCopy(DataTable dt, List<Dictionary<string, object>> dictionary, int i, SQLiteConnection cn)
        {
            using (var cmd = cn.CreateCommand())
            {
                cmd.CommandText = this.Context.Insertable(dictionary.First()).AS(dt.TableName).ToSql().Key.Replace(";SELECT LAST_INSERT_ROWID();","");

                foreach (DataRow dataRow in dt.Rows)
                {
                    foreach (DataColumn item in dt.Columns)
                    {
                        cmd.Parameters.AddWithValue("@" + item.ColumnName, dataRow[item.ColumnName]);
                    }
                    i += await cmd.ExecuteNonQueryAsync();
                    cmd.Parameters.Clear();
                }
            }
            return i;
        }
        private async Task<int> _BulkUpdate(DataTable dt, List<Dictionary<string, object>> dictionary, int i,string [] whereColums,string [] updateColums, SQLiteConnection cn)
        {
            using (var cmd = cn.CreateCommand())
            {
                cmd.CommandText = this.Context.Updateable(dictionary.First())
                    .WhereColumns(whereColums)
                    .UpdateColumns(updateColums)
                    .AS(dt.TableName).ToSql().Key;

                foreach (DataRow dataRow in dt.Rows)
                {
                    foreach (DataColumn item in dt.Columns)
                    {
                        cmd.Parameters.AddWithValue("@" + item.ColumnName, dataRow[item.ColumnName]);
                    }
                    i += await cmd.ExecuteNonQueryAsync();
                    cmd.Parameters.Clear();
                }
            }
            return i;
        }
        private static void Open(SQLiteConnection cn)
        {
            if (cn.State != ConnectionState.Open)
                cn.Open();
        }

        public async Task<int> UpdateByTempAsync(string tableName, string tempName, string[] updateColumns, string[] whereColumns)
        {
            var dt = UpdateDataTable;
            if (dt.Rows.Count == 0)
            {
                return 0;
            }
            var dictionary = this.Context.Utilities.DataTableToDictionaryList(dt.Rows.Cast<DataRow>().Take(1).CopyToDataTable());
            int result = 0;
            var cn = this.Context.Ado.Connection as SQLiteConnection;
            Open(cn);
            if (this.Context.Ado.Transaction == null)
            {
                using (var transaction = cn.BeginTransaction())
                {
                    result = await _BulkUpdate(dt, dictionary, result,whereColumns,updateColumns, cn);
                    transaction.Commit();
                }
            }
            else
            {
                result = await _BulkUpdate(dt, dictionary, result, whereColumns, updateColumns, cn);
            }
            return result;
        }
    }
}
