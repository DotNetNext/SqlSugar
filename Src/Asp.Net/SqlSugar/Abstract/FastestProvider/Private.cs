using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
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
                    var result= new MySqlFastBuilder();
                    result.CharacterSet = this.CharacterSet;
                    return result;
                case DbType.SqlServer:
                    var result2= new SqlServerFastBuilder();
                    result2.DbFastestProperties.IsOffIdentity = this.IsOffIdentity;
                    return result2;
                case DbType.Sqlite:
                    return new SqliteFastBuilder(this.entityInfo);
                case DbType.Oracle:
                    return new OracleFastBuilder(this.entityInfo);
                case DbType.PostgreSQL:
                    return new PostgreSQLFastBuilder(this.entityInfo);
                case DbType.MySqlConnector:
                    var resultConnector = InstanceFactory.CreateInstance<IFastBuilder>("SqlSugar.MySqlConnector.MySqlFastBuilder");
                    resultConnector.CharacterSet = this.CharacterSet;
                    return resultConnector;
                case DbType.Dm:
                    var result3= new DmFastBuilder();
                    result3.DbFastestProperties.IsOffIdentity = this.IsOffIdentity;
                    return result3;
                case DbType.ClickHouse:
                    var resultConnectorClickHouse = InstanceFactory.CreateInstance<IFastBuilder>("SqlSugar.ClickHouse.ClickHouseFastBuilder");
                    resultConnectorClickHouse.CharacterSet = this.CharacterSet;
                    return resultConnectorClickHouse;
                case DbType.Kdbndp:
                    break;
                case DbType.Oscar:
                    break;
                default:
                    break;
            }
            var reslut = InstanceFactory.CreateInstance<IFastBuilder>($"SqlSugar.{this.context.CurrentConnectionConfig.DbType}FastBuilder");
            reslut.CharacterSet = this.CharacterSet;
            reslut.FastEntityInfo = this.entityInfo;
            return reslut;
        }
        private DataTable ToDdateTable(List<T> datas)
        {
            var builder = GetBuider();
            DataTable tempDataTable = ReflectionInoCore<DataTable>.GetInstance().GetOrCreate("BulkCopyAsync" + typeof(T).GetHashCode(),
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
            List<string> uInt64TypeName = new List<string>();
            foreach (DataColumn item in tempDataTable.Columns)
            {
                if (item.DataType == typeof(UInt64)) 
                {
                    uInt64TypeName.Add(item.ColumnName);
                }
                if (item.DataType.Name == "ClickHouseDecimal")
                {
                    dt.Columns.Add(item.ColumnName, typeof(decimal));
                }
                else
                {
                    dt.Columns.Add(item.ColumnName, item.DataType);
                }
            }
            dt.TableName = GetTableName();
            var columns = entityInfo.Columns;
            var isMySql = this.context.CurrentConnectionConfig.DbType.IsIn(DbType.MySql, DbType.MySqlConnector);
            var isSqliteCore = SugarCompatible.IsFramework==false&& this.context.CurrentConnectionConfig.DbType.IsIn(DbType.Sqlite);
            foreach (var item in datas)
            {
                var dr = dt.NewRow();
                foreach (var column in columns)
                {
                    if (column.IsIgnore)
                    {
                        continue;
                    }
                    var name = column.DbColumnName;
                    if (name == null)
                    {
                        name = column.PropertyName;
                    }
                    var value = ValueConverter(column, GetValue(item,column));
                    if (column.SqlParameterDbType != null&& column.SqlParameterDbType is Type && UtilMethods.HasInterface((Type)column.SqlParameterDbType, typeof(ISugarDataConverter))) 
                    {
                        var columnInfo = column;
                        var type = columnInfo.SqlParameterDbType as Type;
                        var ParameterConverter = type.GetMethod("ParameterConverter").MakeGenericMethod(columnInfo.PropertyInfo.PropertyType);
                        var obj = Activator.CreateInstance(type);
                        var p = ParameterConverter.Invoke(obj, new object[] { value, 100 }) as SugarParameter;
                        value = p.Value;
                    }
                    else if (isMySql && column.UnderType == UtilConstants.BoolType)
                    {

                        if (value.ObjToBool() == false&& uInt64TypeName.Any(z=>z.EqualCase(column.DbColumnName)))
                        {
                            value = DBNull.Value;
                        }
                    }
                    else if (isSqliteCore&&column.UnderType == UtilConstants.StringType && value is bool)
                    {
                        value = "isSqliteCore_"+value.ObjToString();
                    }
                    else if (isSqliteCore && column.UnderType == UtilConstants.BoolType && value is bool)
                    {
                        value =Convert.ToBoolean(value)?1:0;
                    }
                    else if (column.UnderType == UtilConstants.DateTimeOffsetType && value != null && value != DBNull.Value)
                    {
                        if (builder.DbFastestProperties != null && builder.DbFastestProperties.HasOffsetTime == true)
                        {
                            //Don't need to deal with
                        }
                        else
                        {
                            value = UtilMethods.ConvertFromDateTimeOffset((DateTimeOffset)value);
                        }
                    }
                    else if (value != null && column.UnderType?.FullName == "System.TimeOnly")
                    {
                        value = UtilMethods.TimeOnlyToTimeSpan(value);
                    }
                    else if (value != null && column.UnderType?.FullName == "System.DateOnly")
                    {
                        value = UtilMethods.DateOnlyToDateTime(value);
                    }
                    dr[name] = value;
                }
                dt.Rows.Add(dr);
            }

            return dt;
        }
        private static object GetValue(T item, EntityColumnInfo column)
        {
            if (StaticConfig.EnableAot)
            {
                return column.PropertyInfo.GetValue(item);
            }
            else
            {
                return PropertyCallAdapterProvider<T>.GetInstance(column.PropertyName).InvokeGet(item);
            }
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
            DataTable tempDataTable = null;
            if (AsName == null)
            {
                tempDataTable=queryable.Clone().Where(it => false).Select("*").ToDataTable();
            }
            else
            {
                tempDataTable=queryable.Clone().AS(AsName).Where(it => false).Select("*").ToDataTable();
            };
            List<string> uInt64TypeName = new List<string>();
            foreach (DataColumn item in tempDataTable.Columns)
            {
                if (item.DataType == typeof(UInt64))
                {
                    uInt64TypeName.Add(item.ColumnName);
                }
            }
            var temColumnsList = tempDataTable.Columns.Cast<DataColumn>().Select(it => it.ColumnName.ToLower()).ToList();
            var columns = dt.Columns.Cast<DataColumn>().Where(it => temColumnsList.Contains(it.ColumnName.ToLower())).ToList();
            foreach (DataRow item in dt.Rows)
            {
                DataRow dr = tempDataTable.NewRow();
                foreach (DataColumn column in columns)
                {

                    dr[column.ColumnName] = item[column.ColumnName];
                    if (dr[column.ColumnName] == null|| dr[column.ColumnName] == DBNull.Value)
                    {
                        dr[column.ColumnName] = DBNull.Value;
                    }
                    else if(column.DataType==UtilConstants.BoolType&&this.context.CurrentConnectionConfig.DbType.IsIn(DbType.MySql, DbType.MySqlConnector)) 
                    {
                        if (Convert.ToBoolean(dr[column.ColumnName]) == false&&uInt64TypeName.Any(z => z.EqualCase(column.ColumnName))) 
                        {
                            dr[column.ColumnName] = DBNull.Value;
                        }
                    }
                }
                tempDataTable.Rows.Add(dr);
            }
            tempDataTable.TableName = dt.TableName;
            return tempDataTable;
        }
        private DataTable GetCopyWriteDataTableUpdate(DataTable dt)
        {
            var sqlBuilder = this.context.Queryable<object>().SqlBuilder;
            var dts = dt.Columns.Cast<DataColumn>().Select(it => sqlBuilder.GetTranslationColumnName(it.ColumnName)).ToList();
            DataTable tempDataTable = null;
            if (AsName == null)
            {
                tempDataTable = queryable.Where(it => false).Select(string.Join(",", dts)).ToDataTable();
            }
            else
            {
                tempDataTable = queryable.AS(AsName).Where(it => false).Select(string.Join(",", dts)).ToDataTable();
            };
            List<string> uInt64TypeName = new List<string>();
            foreach (DataColumn item in tempDataTable.Columns)
            {
                if (item.DataType == typeof(UInt64))
                {
                    uInt64TypeName.Add(item.ColumnName);
                }
            }
            var temColumnsList = tempDataTable.Columns.Cast<DataColumn>().Select(it => it.ColumnName.ToLower()).ToList();
            var columns = dt.Columns.Cast<DataColumn>().Where(it => temColumnsList.Contains(it.ColumnName.ToLower())).ToList();
            foreach (DataRow item in dt.Rows)
            {
                DataRow dr = tempDataTable.NewRow();
                foreach (DataColumn column in columns)
                {

                    dr[column.ColumnName] = item[column.ColumnName];
                    if (dr[column.ColumnName] == null || dr[column.ColumnName] == DBNull.Value)
                    {
                        dr[column.ColumnName] = DBNull.Value;
                    }
                    else if (column.DataType == UtilConstants.BoolType && this.context.CurrentConnectionConfig.DbType.IsIn(DbType.MySql, DbType.MySqlConnector))
                    {
                        if (Convert.ToBoolean(dr[column.ColumnName]) == false && uInt64TypeName.Any(z => z.EqualCase(column.ColumnName)))
                        {
                            dr[column.ColumnName] = DBNull.Value;
                        }
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
            if (this.context.CurrentConnectionConfig?.MoreSettings?.IsAutoRemoveDataCache == true) 
            {
                var cacheService = this.context.CurrentConnectionConfig?.ConfigureExternalServices?.DataInfoCacheService;
                if (cacheService != null)
                {
                    CacheSchemeMain.RemoveCache(cacheService, this.context.EntityMaintenance.GetTableName<T>());
                }
            }
        }

    }
}
