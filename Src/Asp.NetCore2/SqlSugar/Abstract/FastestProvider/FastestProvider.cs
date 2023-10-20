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
        public int BulkCopy(DataTable dt) 
        {
            Check.ExceptionEasy(this.AsName.IsNullOrEmpty(), "need .AS(tablaeName) ", "需要 .AS(tablaeName) 设置表名");
            return BulkCopyAsync(this.AsName, dt).ConfigureAwait(true).GetAwaiter().GetResult();
        }
        public  Task<int> BulkCopyAsync(DataTable dt)
        {
            Check.ExceptionEasy(this.AsName.IsNullOrEmpty(), "need .AS(tablaeName) ", "需要 .AS(tablaeName) 设置表名");
            return  BulkCopyAsync(this.AsName, dt);
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
            whereColumns = whereColumns.Select(x => this.entityInfo.Columns.FirstOrDefault(it => it.PropertyName.EqualCase(x) || it.DbColumnName.EqualCase(x))?.DbColumnName ?? x).ToArray();
            updateColumns = updateColumns.Select(x => this.entityInfo.Columns.FirstOrDefault(it => it.PropertyName.EqualCase(x) || it.DbColumnName.EqualCase(x))?.DbColumnName ?? x).ToArray(); 
            return BulkUpdateAsync(datas,whereColumns,updateColumns).ConfigureAwait(true).GetAwaiter().GetResult();
        }

        public int BulkUpdate(List<T> datas, string[] whereColumns) 
        {
            return BulkUpdateAsync(datas, whereColumns).GetAwaiter().GetResult();
        }

        public async Task<int> BulkUpdateAsync(List<T> datas, string[] whereColumns)
        {
            whereColumns = whereColumns.Select(x => this.entityInfo.Columns.FirstOrDefault(it => it.PropertyName.EqualCase(x) || it.DbColumnName.EqualCase(x))?.DbColumnName ?? x).ToArray();
            var updateColumns = this.entityInfo.Columns
                 .Where(it => !whereColumns.Any(z => z.EqualCase(it.DbColumnName)))
                 .Where(it => !it.IsIdentity)
                 .Where(it => !it.IsPrimarykey)
                 .Where(it => !it.IsOnlyIgnoreUpdate)
                 .Where(it => !it.IsIgnore)
                .Select(it => it.DbColumnName)
                .ToArray();
            return await BulkUpdateAsync(datas, whereColumns, updateColumns).ConfigureAwait(true);
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
        public int BulkUpdate(DataTable dataTable, string[] whereColumns, string[] updateColumns)
        {
            Check.ExceptionEasy(this.AsName.IsNullOrEmpty(), "need .AS(tablaeName) ", "需要 .AS(tablaeName) 设置表名");
            return BulkUpdateAsync(this.AsName, dataTable, whereColumns, updateColumns).ConfigureAwait(true).GetAwaiter().GetResult();
        }
        public int BulkUpdate(DataTable dataTable, string[] whereColumns)
        {
            string[] updateColumns = dataTable.Columns.Cast<DataColumn>().Select(it => it.ColumnName).Where(it => !whereColumns.Any(z => z.EqualCase(it))).ToArray();
            Check.ExceptionEasy(this.AsName.IsNullOrEmpty(), "need .AS(tablaeName) ", "需要 .AS(tablaeName) 设置表名");
            return BulkUpdateAsync(this.AsName, dataTable, whereColumns, updateColumns).ConfigureAwait(true).GetAwaiter().GetResult();
        }
        public Task<int> BulkUpdateAsync(DataTable dataTable, string[] whereColumns)
        {
            string[] updateColumns = dataTable.Columns.Cast<DataColumn>().Select(it => it.ColumnName).Where(it => !whereColumns.Any(z => z.EqualCase(it))).ToArray();
            Check.ExceptionEasy(this.AsName.IsNullOrEmpty(), "need .AS(tablaeName) ", "需要 .AS(tablaeName) 设置表名");
            return BulkUpdateAsync(this.AsName, dataTable, whereColumns, updateColumns);
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

        #region BulkMerge
        public Task<int> BulkMergeAsync(List<T> datas)
        {
            var updateColumns = entityInfo.Columns.Where(it => !it.IsPrimarykey && !it.IsIdentity && !it.IsOnlyIgnoreUpdate && !it.IsIgnore).Select(it => it.DbColumnName ?? it.PropertyName).ToArray();
            var whereColumns = entityInfo.Columns.Where(it => it.IsPrimarykey).Select(it => it.DbColumnName ?? it.PropertyName).ToArray(); ;
            return BulkMergeAsync(datas, whereColumns, updateColumns);
        }
        public int BulkMerge(List<T> datas)
        {
            return BulkMergeAsync(datas).GetAwaiter().GetResult();
        }
        public Task<int> BulkMergeAsync(List<T> datas, string[] whereColumns)
        {
            var updateColumns = entityInfo.Columns.Where(it => !it.IsPrimarykey && !it.IsIdentity && !it.IsOnlyIgnoreUpdate && !it.IsIgnore).Select(it => it.DbColumnName ?? it.PropertyName).ToArray();
            return BulkMergeAsync(datas, whereColumns, updateColumns);
        }
        public int BulkMerge(List<T> datas, string[] whereColumns)
        {
            return BulkMergeAsync(datas, whereColumns).GetAwaiter().GetResult();
        }
        public async Task<int> BulkMergeAsync(List<T> datas, string[] whereColumns, string[] updateColumns)
        {
            if (Size > 0)
            {
                int resul = 0;
                await this.context.Utilities.PageEachAsync(datas, Size, async item =>
                { 
                    resul += await _BulkMerge(item, updateColumns, whereColumns);
                });
                return resul;
            }
            else
            {
                return await _BulkMerge(datas, updateColumns, whereColumns);
            }
        }
        public int BulkMerge(List<T> datas, string[] whereColumns, string[] updateColumns)
        {
            return BulkMergeAsync(datas, whereColumns, updateColumns).GetAwaiter().GetResult();
        }

        private async Task<int> _BulkMerge(List<T> datas, string[] updateColumns, string[] whereColumns)
        {
            Begin(datas, false,true);
            Check.Exception(whereColumns == null || whereColumns.Count() == 0, "where columns count=0 or need primary key");
            Check.Exception(whereColumns == null || whereColumns.Count() == 0, "where columns count=0 or need primary key");
            var isAuto = this.context.CurrentConnectionConfig.IsAutoCloseConnection;
            this.context.CurrentConnectionConfig.IsAutoCloseConnection = false;
            DataTable dt = ToDdateTable(datas);
            IFastBuilder buider = GetBuider();
            buider.Context = context;
            if (buider?.DbFastestProperties?.IsMerge == true)
            {
                await buider.CreateTempAsync<T>(dt);
                await buider.ExecuteBulkCopyAsync(dt);
            }
            var result = await buider.Merge(GetTableName(),dt, this.entityInfo,whereColumns,updateColumns, datas);
            //var queryTemp = this.context.Queryable<T>().AS(dt.TableName).ToList();//test
            //var result = await buider.UpdateByTempAsync(GetTableName(), dt.TableName, updateColumns, whereColumns);
            if (buider?.DbFastestProperties?.IsMerge == true&&this.context.CurrentConnectionConfig.DbType != DbType.Sqlite)
            {
                this.context.DbMaintenance.DropTable(dt.TableName);
            }
            this.context.CurrentConnectionConfig.IsAutoCloseConnection = isAuto;
            buider.CloseDb();
            End(datas, false,true);
            return result;
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
            ActionIgnoreColums(whereColumns, updateColumns, dt, buider.IsActionUpdateColumns);
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

        private  void ActionIgnoreColums(string[] whereColumns, string[] updateColumns, DataTable dt,bool IsActionUpdateColumns)
        {
            if (entityInfo.Columns.Where(it => it.IsIgnore == false).Count() > whereColumns.Length + updateColumns.Length &&IsActionUpdateColumns)
            {
                var ignoreColums = dt.Columns.Cast<DataColumn>()
                .Where(it => !whereColumns.Any(y => y.EqualCase(it.ColumnName)))
                .Where(it => !updateColumns.Any(y => y.EqualCase(it.ColumnName))).ToList();
                foreach (DataRow item in dt.Rows)
                {
                    foreach (var col in ignoreColums)
                    {
                        if (item[col.ColumnName].IsNullOrEmpty())
                        {
                            if (col.DataType == UtilConstants.StringType)
                            {
                                item[col.ColumnName] = string.Empty;
                            }
                            else if (col.DataType == UtilConstants.DateType)
                            {
                                item[col.ColumnName] =UtilMethods.GetMinDate(this.context.CurrentConnectionConfig);
                            }
                            else
                            {
                                item[col.ColumnName] = Activator.CreateInstance(col.DataType);
                            }
                        }
                    }
                }
            }
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
            DataTable dt = GetCopyWriteDataTableUpdate(dataTable);
            IFastBuilder buider = GetBuider();
            if (dt.Columns.Count != dataTable.Columns.Count)
            {
                ActionIgnoreColums(whereColumns, updateColumns, dt, buider.IsActionUpdateColumns);
            }
            buider.Context = context;
            if (buider.DbFastestProperties == null)
            {
                buider.DbFastestProperties = new DbFastestProperties();
            }
            buider.DbFastestProperties.WhereColumns = whereColumns;
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
        private void End<Type>(List<Type> datas,bool isAdd,bool isMerge=false)
        {
            var title = isAdd ? "BulkCopy" : "BulkUpdate";
            if (isMerge) 
            {
                title = "BulkMerge";
            }
            this.context.Ado.IsEnableLogEvent = isLog;
            if (this.context.CurrentConnectionConfig?.AopEvents?.OnLogExecuted != null)
            {
                this.context.CurrentConnectionConfig?.AopEvents?.OnLogExecuted($"End {title}  name:{GetTableName()} ,count: {datas.Count},current time: {DateTime.Now}", new SugarParameter[] { });
            }
            RemoveCache();
        }
        private void Begin<Type>(List<Type> datas,bool isAdd, bool isMerge = false)
        {
            var title = isAdd ? "BulkCopy" : "BulkUpdate";
            if (isMerge)
            {
                title = "BulkMerge";
            }
            isLog = this.context.Ado.IsEnableLogEvent;
            this.context.Ado.IsEnableLogEvent = false;
            if (this.context.CurrentConnectionConfig?.AopEvents?.OnLogExecuting != null)
            {
                this.context.CurrentConnectionConfig?.AopEvents?.OnLogExecuting($"Begin {title} name:{GetTableName()} ,count: {datas.Count},current time: {DateTime.Now} ", new SugarParameter[] { });
            }
            var dataEvent = this.context.CurrentConnectionConfig.AopEvents?.DataExecuting;
            if (IsDataAop&&dataEvent!=null) 
            {
                var entity = this.context.EntityMaintenance.GetEntityInfo(typeof(Type));
                foreach (var item in datas)
                {
                    DataAop(item, isAdd
                                   ? 
                                   DataFilterType.InsertByObject:
                                   DataFilterType.UpdateByObject
                                   , entity);
                }
            }
        }
        private void DataAop<Type>(Type item, DataFilterType type,EntityInfo entity)
        {
            var dataEvent = this.context.CurrentConnectionConfig.AopEvents?.DataExecuting;
            if (dataEvent != null && item != null)
            {
                foreach (var columnInfo in entity.Columns)
                {
                    dataEvent(columnInfo.PropertyInfo.GetValue(item, null), new DataFilterModel() { OperationType = type, EntityValue = item, EntityColumnInfo = columnInfo });
                }
            }
        }
        #endregion

    }
}
