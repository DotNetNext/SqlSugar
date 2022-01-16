using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public partial class FastestProvider<T> : IFastest<T> where T : class, new()
    {
        private IFastBuilder GetBuider()
        {
            switch (this.context.CurrentConnectionConfig.DbType)
            {
                case DbType.MySql:
                    return new MySqlFastBuilder();
                case DbType.SqlServer:
                    return new SqlServerFastBuilder();
                case DbType.Sqlite:
                    return new SqliteFastBuilder(this.entityInfo);
                case DbType.Oracle:
                    return new OracleFastBuilder(this.entityInfo);
                case DbType.PostgreSQL:
                    return new PostgreSQLFastBuilder(this.entityInfo);
                case DbType.Dm:
                    break;
                case DbType.Kdbndp:
                    break;
                case DbType.Oscar:
                    break;
                default:
                    break;
            }
            throw new Exception(this.context.CurrentConnectionConfig.DbType + "开发中...");
        }
        private DataTable ToDdateTable(List<T> datas)
        {
            DataTable tempDataTable = ReflectionInoCore<DataTable>.GetInstance().GetOrCreate("BulkCopyAsync" + typeof(T).FullName,
            () =>
            {
                if (AsName == null)
                {
                    return queryable.Where(it => false).Select("*").ToDataTable();
                }
                else
                {
                    return queryable.AS(AsName).Where(it => false).Select("*").ToDataTable();
                }
            }
            );
            var dt = new DataTable();
            foreach (DataColumn item in tempDataTable.Columns)
            {
                dt.Columns.Add(item.ColumnName, item.DataType);
            }
            dt.TableName = GetTableName();
            var columns = entityInfo.Columns;
            var isMySql = this.context.CurrentConnectionConfig.DbType == DbType.MySql;
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
                    if (isMySql&& column.UnderType==UtilConstants.BoolType) 
                    {
                        if (value.ObjToBool() == false)
                        {
                            value = DBNull.Value;
                        }
                    }
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
                return queryable.SqlBuilder.GetTranslationTableName(this.context.EntityMaintenance.GetTableName<T>());
            }
        }
        private object ValueConverter(EntityColumnInfo columnInfo, object value)
        {
            if (value == null)
                return DBNull.Value;
            if (value is DateTime && (DateTime)value == DateTime.MinValue)
            {
                value = Convert.ToDateTime("1900-01-01");
            }
            //else if (columnInfo.IsJson)
            //{
            //    columnInfo.IsJson = true;
            //}
            //else if (columnInfo.IsArray)
            //{
            //    columnInfo.IsArray = true;
            //}
            else if (columnInfo.UnderType.IsEnum() )
            {
                value = Convert.ToInt64(value);
            }
            else if (columnInfo.IsJson && value != null)
            {
                 value = this.context.Utilities.SerializeObject(value);
            }
            else if (columnInfo.IsTranscoding && value.HasValue())
            {
                value = UtilMethods.EncodeBase64(value.ToString());
            }
            return value;
        }
        private DataTable GetCopyWriteDataTable(DataTable dt)
        {
            DataTable tempDataTable = ReflectionInoCore<DataTable>.GetInstance().GetOrCreate("BulkCopyAsync_dt" + dt.TableName,
            () =>
            {
                if (AsName == null)
                {
                    return queryable.Where(it => false).Select("*").ToDataTable();
                }
                else
                {
                    return queryable.AS(AsName).Where(it => false).Select("*").ToDataTable();
                }
            }
            );
            var temColumnsList = tempDataTable.Columns.Cast<DataColumn>().Select(it => it.ColumnName.ToLower()).ToList();
            var columns = dt.Columns.Cast<DataColumn>().Where(it => temColumnsList.Contains(it.ColumnName.ToLower())).ToList();
            foreach (DataRow item in dt.Rows)
            {
                DataRow dr = tempDataTable.NewRow();
                foreach (DataColumn column in columns)
                {

                    dr[column.ColumnName] = item[column.ColumnName];
                    if (dr[column.ColumnName] == null)
                    {
                        dr[column.ColumnName] = DBNull.Value;
                    }
                }
                tempDataTable.Rows.Add(dr);
            }
            tempDataTable.TableName = dt.TableName;
            return tempDataTable;
        }


        private void RemoveCache()
        {
            if (!string.IsNullOrEmpty(CacheKey) || !string.IsNullOrEmpty(CacheKeyLike))
            {
                Check.Exception(this.context.CurrentConnectionConfig.ConfigureExternalServices?.DataInfoCacheService == null, "ConnectionConfig.ConfigureExternalServices.DataInfoCacheService is null");
                var service = this.context.CurrentConnectionConfig.ConfigureExternalServices?.DataInfoCacheService;
                if (!string.IsNullOrEmpty(CacheKey)) 
                {
                    CacheSchemeMain.RemoveCache(service, CacheKey);
                }
                if (!string.IsNullOrEmpty(CacheKeyLike))
                {
                    CacheSchemeMain.RemoveCacheByLike(service, CacheKeyLike);
                }
            }
        }

    }
}
