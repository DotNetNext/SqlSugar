using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
namespace SqlSugar 
{
    public class FastestProvider<T>:IFastest<T> where T:class,new()
    {
        private SqlSugarProvider context;
        private ISugarQueryable<T> queryable;
        private string AsName { get; set; }
        private EntityInfo entityInfo { get; set; }
        public FastestProvider(SqlSugarProvider sqlSugarProvider)
        {
            this.context = sqlSugarProvider;
            this.queryable = this.context.Queryable<T>();
            entityInfo=this.context.EntityMaintenance.GetEntityInfo<T>();
        }
        #region Api
        public int BulkCopy(List<T> datas)
        {
            return BulkCopyAsync(datas).ConfigureAwait(true).GetAwaiter().GetResult();
        }
        public async Task<int> BulkCopyAsync(List<T> datas)
        {
            DataTable dt = ToDdateTable(datas);
            IFastBuilder buider = new SqlServerFastBuilder();
            buider.Context = context;
            var result = await buider.ExecuteBulkCopyAsync(dt);
            return result;
        }
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
        public async Task<int> BulkUpdateAsync(List<T> datas,string [] whereColumns,string [] updateColumns)
        {
            var isAuto = this.context.CurrentConnectionConfig.IsAutoCloseConnection;
            this.context.CurrentConnectionConfig.IsAutoCloseConnection = false;
            DataTable dt = ToDdateTable(datas);
            IFastBuilder buider = GetBuider();
            buider.Context = context;
            await buider.CreateTempAsync<T>(dt);
            await buider.ExecuteBulkCopyAsync(dt);
            //var queryTemp = this.context.Queryable<T>().AS(dt.TableName).ToList();//test
            var result = await buider.UpdateByTempAsync(GetTableName(), dt.TableName, updateColumns, whereColumns);
            this.context.DbMaintenance.DropTable(dt.TableName);
            this.context.CurrentConnectionConfig.IsAutoCloseConnection = isAuto;
            return result;
        }
        #endregion

        #region Setting
        public IFastest<T> AS(string tableName)
        {
            this.AsName = tableName;
            return this;
        }
        #endregion

        #region Helper
        private SqlServerFastBuilder GetBuider()
        {
            switch (this.context.CurrentConnectionConfig.DbType)
            {
                case DbType.MySql:
                    break;
                case DbType.SqlServer:
                    return new SqlServerFastBuilder();
                case DbType.Sqlite:
                    break;
                case DbType.Oracle:
                    break;
                case DbType.PostgreSQL:
                    break;
                case DbType.Dm:
                    break;
                case DbType.Kdbndp:
                    break;
                case DbType.Oscar:
                    break;
                default:
                    break;
            }
            throw new Exception(this.context.CurrentConnectionConfig.DbType + "开发中");
        }
        private DataTable ToDdateTable(List<T> datas)
        {
            DataTable tempDataTable = ReflectionInoCore<DataTable>.GetInstance().GetOrCreate("BulkCopyAsync" + typeof(T).FullName, () => queryable.Where(it => false).ToDataTable());
            var dt = new DataTable();
            foreach (DataColumn item in tempDataTable.Columns)
            {
                dt.Columns.Add(item.ColumnName, item.DataType);
            }
            dt.TableName = GetTableName();
            var columns = entityInfo.Columns;
            foreach (var item in datas)
            {
                var dr = dt.NewRow();
                foreach (var column in columns)
                {
                    if (column.IsIgnore || column.IsOnlyIgnoreInsert)
                    {
                        continue;
                    }
                    var name = column.DbColumnName;
                    if (name == null)
                    {
                        name = column.PropertyName;
                    }
                    var value = ValueConverter(column, PropertyCallAdapterProvider<T>.GetInstance(column.PropertyName).InvokeGet(item));
                    dr[name] = value;
                }
                dt.Rows.Add(dr);
            }

            return dt;
        }
        private string GetTableName()
        {
            if (this.AsName.HasValue())
            {
                return queryable.SqlBuilder.GetTranslationTableName(AsName);
            }
            else
            {
                return queryable.SqlBuilder.GetTranslationTableName(entityInfo.DbTableName);
            }
        }
        private object ValueConverter(EntityColumnInfo columnInfo, object value)
        {
            if (value == null)
                return value;
            if (value is DateTime && (DateTime)value == DateTime.MinValue)
            {
                value = Convert.ToDateTime("1900-01-01");
            }
            return value;
        } 
        #endregion
    }
}
