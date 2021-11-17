using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public class FastestProvider<T>:IFastest<T>
    {
        private SqlSugarProvider context;
        private ISugarQueryable<T> queryable;
        private string AsName { get; set; }
        public FastestProvider(SqlSugarProvider sqlSugarProvider)
        {
            this.context = sqlSugarProvider;
            this.queryable = this.context.Queryable<T>();
        }
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
        public IFastest<T> AS(string tableName)
        {
            this.AsName = tableName;
            return this;
        }
        private DataTable ToDdateTable(List<T> datas)
        {
            DataTable tempDataTable = ReflectionInoCore<DataTable>.GetInstance().GetOrCreate("BulkCopyAsync" + typeof(T).FullName, () => queryable.Where(it => false).ToDataTable());
            var dt = new DataTable();
            foreach (DataColumn item in tempDataTable.Columns)
            {
                dt.Columns.Add(item.ColumnName, item.DataType);
            }
            var entityInfo = this.context.EntityMaintenance.GetEntityInfo<T>();
            GetTableName(dt, entityInfo);
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

        private void GetTableName(DataTable dt, EntityInfo entityInfo)
        {
            if (this.AsName.HasValue())
            {
                dt.TableName = queryable.SqlBuilder.GetTranslationTableName(AsName);
            }
            else
            {
                dt.TableName = queryable.SqlBuilder.GetTranslationTableName(entityInfo.DbTableName);
            }
        }
        private object ValueConverter(EntityColumnInfo columnInfo,object value)
        {
            if (value == null)
                return value;
            if (value is DateTime&&(DateTime)value == DateTime.MinValue) 
            {
                value = Convert.ToDateTime("1900-01-01");
            }
            return value;
        }

    
    }
}
