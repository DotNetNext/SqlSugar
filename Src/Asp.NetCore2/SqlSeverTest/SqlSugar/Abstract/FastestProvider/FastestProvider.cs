using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
namespace SqlSugar 
{
    public partial class FastestProvider<T>:IFastest<T> where T:class,new()
    {
        internal SqlSugarProvider context;
        private ISugarQueryable<T> queryable;
        private EntityInfo entityInfo { get; set; }
        public bool isLog;
        public FastestProvider(SqlSugarProvider sqlSugarProvider)
        {
            this.context = sqlSugarProvider;
            this.queryable = this.context.Queryable<T>();
            entityInfo=this.context.EntityMaintenance.GetEntityInfo<T>();
        }

        #region BulkCopy
        public int BulkCopy(string tableName,DataTable dt)
        {
            return BulkCopyAsync(tableName,dt).ConfigureAwait(true).GetAwaiter().GetResult();
        }
        public async Task<int> BulkCopyAsync(string tableName, DataTable dt)
        {
            if (Size > 0)
            {
                int resul = 0;
                await this.context.Utilities.PageEachAsync(dt.Rows.Cast<DataRow>().ToList(), Size, async item =>
                {
                    resul += await _BulkCopy(tableName,item.CopyToDataTable());
                });
                return resul;
            }
            else
            {
                return await _BulkCopy(tableName,dt);
            }
        }
        public int BulkCopy(List<T> datas)
        {
            return BulkCopyAsync(datas).ConfigureAwait(true).GetAwaiter().GetResult();
        }
        public async Task<int> BulkCopyAsync(List<T> datas)
        {
            if (Size > 0)
            {
                int resul=0;
                 await this.context.Utilities.PageEachAsync(datas, Size, async item =>
                {
                    resul+= await _BulkCopy(item);
                });
                return resul;
            }
            else
            {
                return await _BulkCopy(datas);
            }
        }
        #endregion

        #region BulkUpdate
        public int BulkUpdate(List<T> datas)
        {
            return BulkUpdateAsync(datas).ConfigureAwait(true).GetAwaiter().GetResult();
        }
        public async Task<int> BulkUpdateAsync(List<T> datas)
        {
           var whereColumns=entityInfo.Columns.Where(it => it.IsPrimarykey).Select(it=>it.DbColumnName??it.PropertyName).ToArray();
           var updateColumns = entityInfo.Columns.Where(it => !it.IsPrimarykey&&!it.IsIdentity&&!it.IsOnlyIgnoreUpdate&&!it.IsIgnore).Select(it => it.DbColumnName ?? it.PropertyName).ToArray();
           return await BulkUpdateAsync(datas,whereColumns,updateColumns);
        }
        public int BulkUpdate(List<T> datas, string[] whereColumns, string[] updateColumns) 
        {
            return BulkUpdateAsync(datas,whereColumns,updateColumns).ConfigureAwait(true).GetAwaiter().GetResult();
        }
        public async Task<int> BulkUpdateAsync(List<T> datas,string [] whereColumns,string [] updateColumns)
        {

            if (Size > 0)
            {
                int resul = 0;
                await this.context.Utilities.PageEachAsync(datas, Size, async item =>
                {
                    resul += await _BulkUpdate(item, whereColumns, updateColumns);
                });
                return resul;
            }
            else
            {
                return  await _BulkUpdate(datas, whereColumns, updateColumns);
            }
        }

        public int BulkUpdate(string tableName,DataTable dataTable, string[] whereColumns, string[] updateColumns)
        {
            return BulkUpdateAsync(tableName,dataTable, whereColumns, updateColumns).ConfigureAwait(true).GetAwaiter().GetResult();
        }
        public async Task<int> BulkUpdateAsync(string tableName, DataTable dataTable, string[] whereColumns, string[] updateColumns)
        {

            if (Size > 0)
            {
                int resul = 0;
                await this.context.Utilities.PageEachAsync(dataTable.Rows.Cast<DataRow>().ToList(), Size, async item =>
                {
                    resul += await _BulkUpdate(tableName,item.CopyToDataTable(), whereColumns, updateColumns);
                });
                return resul;
            }
            else
            {
                return await _BulkUpdate(tableName,dataTable, whereColumns, updateColumns);
            }
        }
        #endregion

        #region Core
        private async Task<int> _BulkUpdate(List<T> datas, string[] whereColumns, string[] updateColumns)
        {
            Begin(datas,false);
            Check.Exception(whereColumns == null || whereColumns.Count() == 0, "where columns count=0 or need primary key");
            Check.Exception(updateColumns == null || updateColumns.Count() == 0, "set columns count=0");
            var isAuto = this.context.CurrentConnectionConfig.IsAutoCloseConnection;
            this.context.CurrentConnectionConfig.IsAutoCloseConnection = false;
            DataTable dt = ToDdateTable(datas);
            IFastBuilder buider = GetBuider();
            buider.Context = context;
            await buider.CreateTempAsync<T>(dt);
            await buider.ExecuteBulkCopyAsync(dt);
            //var queryTemp = this.context.Queryable<T>().AS(dt.TableName).ToList();//test
            var result = await buider.UpdateByTempAsync(GetTableName(), dt.TableName, updateColumns, whereColumns);
            if (this.context.CurrentConnectionConfig.DbType != DbType.Sqlite)
            {
                this.context.DbMaintenance.DropTable(dt.TableName);
            }
            this.context.CurrentConnectionConfig.IsAutoCloseConnection = isAuto;
            buider.CloseDb();
            End(datas, false);
            return result;
        }
        private async Task<int> _BulkUpdate(string tableName,DataTable dataTable, string[] whereColumns, string[] updateColumns)
        {
            var datas = new string[dataTable.Rows.Count].ToList();
            Begin(datas, false);
            Check.Exception(whereColumns == null || whereColumns.Count() == 0, "where columns count=0 or need primary key");
            Check.Exception(updateColumns == null || updateColumns.Count() == 0, "set columns count=0");
            var isAuto = this.context.CurrentConnectionConfig.IsAutoCloseConnection;
            this.context.CurrentConnectionConfig.IsAutoCloseConnection = false;
            dataTable.TableName = this.queryable.SqlBuilder.GetTranslationTableName(tableName);
            DataTable dt = GetCopyWriteDataTable(dataTable);
            IFastBuilder buider = GetBuider();
            buider.Context = context;
            await buider.CreateTempAsync<object>(dt);
            await buider.ExecuteBulkCopyAsync(dt);
            //var queryTemp = this.context.Queryable<T>().AS(dt.TableName).ToList();//test
            var result = await buider.UpdateByTempAsync(GetTableName(), dt.TableName, updateColumns, whereColumns);
            this.context.DbMaintenance.DropTable(dt.TableName);
            this.context.CurrentConnectionConfig.IsAutoCloseConnection = isAuto;
            buider.CloseDb();
            End(datas, false);
            return result;
        }
        private async Task<int> _BulkCopy(List<T> datas)
        {
            Begin(datas,true);
            DataTable dt = ToDdateTable(datas);
            IFastBuilder buider =GetBuider();
            buider.Context = context;
            var result = await buider.ExecuteBulkCopyAsync(dt);
            End(datas,true);
            return result;
        }
        private async Task<int> _BulkCopy(string tableName,DataTable dataTable)
        {
            var datas =new string[dataTable.Rows.Count].ToList();
            Begin(datas, true);
            DataTable dt = dataTable;
            dt.TableName =this.queryable.SqlBuilder.GetTranslationTableName(tableName);
            dt = GetCopyWriteDataTable(dt);
            IFastBuilder buider = GetBuider();
            buider.Context = context;
            var result = await buider.ExecuteBulkCopyAsync(dt);
            End(datas, true);
            return result;
        }
        #endregion

        #region AOP
        private void End<Type>(List<Type> datas,bool isAdd)
        {
            var title = isAdd ? "BulkCopy" : "BulkUpdate";
            this.context.Ado.IsEnableLogEvent = isLog;
            if (this.context.CurrentConnectionConfig?.AopEvents?.OnLogExecuted != null)
            {
                this.context.CurrentConnectionConfig?.AopEvents?.OnLogExecuted($"End {title}  name:{GetTableName()} ,count: {datas.Count},current time: {DateTime.Now}", new SugarParameter[] { });
            }
            RemoveCache();
        }
        private void Begin<Type>(List<Type> datas,bool isAdd)
        {
            var title = isAdd ? "BulkCopy" : "BulkUpdate";
            isLog = this.context.Ado.IsEnableLogEvent;
            this.context.Ado.IsEnableLogEvent = false;
            if (this.context.CurrentConnectionConfig?.AopEvents?.OnLogExecuting != null)
            {
                this.context.CurrentConnectionConfig?.AopEvents?.OnLogExecuting($"Begin {title} name:{GetTableName()} ,count: {datas.Count},current time: {DateTime.Now} ", new SugarParameter[] { });
            }
        }
        #endregion

    }
}
