using SqlSugar.DbConvert;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace SqlSugar
{
    public class EntityMaintenance
    {
        public SqlSugarProvider Context { get; set; }

        public EntityInfo GetEntityInfo<T>()
        {
            return GetEntityInfo(typeof(T));
        }
        public EntityInfo GetEntityInfoWithAttr(Type type)
        {
            return GetEntityInfo(type);
        }
        public EntityInfo GetEntityInfo(Type type)
        {
            var attr = type?.GetCustomAttribute<TenantAttribute>();
            if (attr == null)
            {
                return _GetEntityInfo(type);
            }
            else if (attr.configId.ObjToString() == this.Context?.CurrentConnectionConfig?.ConfigId + "")
            {
                return _GetEntityInfo(type);
            }
            else if (this.Context.Root == null)
            {
                return _GetEntityInfo(type);
            }
            else if (!this.Context.Root.IsAnyConnection(attr.configId))
            {
                return _GetEntityInfo(type);
            }
            else
            {
                return this.Context.Root.GetConnection(attr.configId).EntityMaintenance._GetEntityInfo(type);
            }
        }

        private EntityInfo _GetEntityInfo(Type type)
        {
            string cacheKey = "GetEntityInfo" + type.GetHashCode() + type.FullName + this.Context?.CurrentConnectionConfig?.ConfigId;
            return this.Context.Utilities.GetReflectionInoCacheInstance().GetOrCreate(cacheKey,
            () =>
            {
                return GetEntityInfoNoCache(type);
            });
        }

        public EntityInfo GetEntityInfoNoCache(Type type)
        {
            EntityInfo result = new EntityInfo();
            var sugarAttributeInfo = type.GetTypeInfo().GetCustomAttributes(typeof(SugarTable), false).Where(it => it is SugarTable).SingleOrDefault();
            if (sugarAttributeInfo.HasValue())
            {
                var sugarTable = (SugarTable)sugarAttributeInfo;
                result.DbTableName = sugarTable.TableName;
                result.TableDescription = sugarTable.TableDescription.ToSqlFilter();
                result.IsDisabledUpdateAll = sugarTable.IsDisabledUpdateAll;
                result.IsDisabledDelete = sugarTable.IsDisabledDelete;
                result.IsCreateTableFiledSort = sugarTable.IsCreateTableFiledSort;
                result.Discrimator = sugarTable.Discrimator;
            }
            var indexs = type.GetCustomAttributes(typeof(SugarIndexAttribute));
            if (indexs != null && indexs.Any())
            {
                result.Indexs = indexs.Select(it => it as SugarIndexAttribute).ToList();
            }
            if (result.TableDescription.IsNullOrEmpty()) result.TableDescription = GetTableAnnotation(type);
            if (this.Context.CurrentConnectionConfig.ConfigureExternalServices != null && this.Context.CurrentConnectionConfig.ConfigureExternalServices.EntityNameService != null)
            {
                if (result.DbTableName == null)
                {
                    result.DbTableName = type.Name;
                }
                this.Context.CurrentConnectionConfig.ConfigureExternalServices.EntityNameService(type, result);
            }
            result.Type = type;
            result.EntityName = result.Type.Name;
            result.Columns = new List<EntityColumnInfo>();
            SetColumns(result);
            return result;
        }

        public string GetTableName<T>()
        {
            return GetTableName(typeof(T));
        }
        public string GetTableName(Type entityType)
        {
            var typeName = entityType.Name;
            if (this.Context.MappingTables == null || this.Context.MappingTables.Count == 0 || !this.Context.MappingTables.Any(it => it.EntityName == typeName))
            {
                var entity = this.GetEntityInfo(entityType);
                if (entity.DbTableName.HasValue()) return entity.DbTableName;
                else return entity.EntityName;
            }
            else
            {
                var mappingInfo = this.Context.MappingTables.SingleOrDefault(it => it.EntityName == typeName);
                return mappingInfo == null ? typeName : mappingInfo.DbTableName;
            }
        }
        public string GetTableName(string entityName)
        {
            var typeName = entityName;
            if (this.Context.MappingTables == null || this.Context.MappingTables.Count == 0) return typeName;
            else
            {
                var mappingInfo = this.Context.MappingTables.SingleOrDefault(it => it.EntityName == typeName);
                return mappingInfo == null ? typeName : mappingInfo.DbTableName;
            }
        }
        public string GetEntityName(string tableName)
        {
            if (this.Context.MappingTables == null || this.Context.MappingTables.Count == 0) return tableName;
            else
            {
                var mappingInfo = this.Context.MappingTables.SingleOrDefault(it => it.DbTableName == tableName);
                return mappingInfo == null ? tableName : mappingInfo.EntityName;
            }
        }
        public string GetEntityName<T>()
        {
            return this.Context.EntityMaintenance.GetEntityInfo<T>().EntityName;
        }
        public string GetEntityName(Type type)
        {
            return this.Context.EntityMaintenance.GetEntityInfo(type).EntityName;
        }
        public string GetDbColumnName<T>(string propertyName)
        {
            return GetDbColumnName(propertyName, typeof(T));
        }
        public string GetDbColumnName(string propertyName, Type entityType)
        {
            var isAny = this.GetEntityInfo(entityType).Columns.Any(it => it.PropertyName.Equals(propertyName, StringComparison.CurrentCultureIgnoreCase));
            Check.Exception(!isAny, "Property " + propertyName + " is Invalid");
            var typeName = entityType.Name;
            if (this.Context.MappingColumns == null || this.Context.MappingColumns.Count == 0 || !this.Context.MappingColumns.Any(it => it.EntityName == typeName && it.PropertyName == propertyName))
            {
                var column = this.GetEntityInfo(entityType).Columns.First(it => it.PropertyName.Equals(propertyName, StringComparison.CurrentCultureIgnoreCase));
                if (column.DbColumnName.HasValue()) return column.DbColumnName;
                else return column.PropertyName;
            }
            else
            {
                var mappingInfo = this.Context.MappingColumns.SingleOrDefault(it => it.EntityName == typeName && it.PropertyName == propertyName);
                return mappingInfo == null ? propertyName : mappingInfo.DbColumnName;
            }
        }
        public string GetPropertyName<T>(string dbColumnName)
        {
            var columnInfo=this.Context.EntityMaintenance.GetEntityInfo<T>().Columns.FirstOrDefault(it=>it.DbColumnName.EqualCase(dbColumnName));
            if (columnInfo != null) 
            {
                return columnInfo.PropertyName;
            }
            var typeName = typeof(T).Name;
            if (this.Context.MappingColumns == null || this.Context.MappingColumns.Count == 0) return dbColumnName;
            else
            {
                var mappingInfo = this.Context.MappingColumns.SingleOrDefault(it => it.EntityName == typeName && it.DbColumnName.Equals(dbColumnName, StringComparison.CurrentCultureIgnoreCase));
                return mappingInfo == null ? dbColumnName : mappingInfo.PropertyName;
            }
        }
        public string GetPropertyName(string dbColumnName, Type entityType)
        {
            var columnInfo = this.Context.EntityMaintenance.GetEntityInfo(entityType).Columns.FirstOrDefault(it => it.DbColumnName.EqualCase(dbColumnName));
            if (columnInfo != null)
            {
                return columnInfo.PropertyName;
            }
            var typeName = entityType.Name;
            if (this.Context.MappingColumns == null || this.Context.MappingColumns.Count == 0) return dbColumnName;
            else
            {
                var mappingInfo = this.Context.MappingColumns.SingleOrDefault(it => it.EntityName == typeName && it.DbColumnName.Equals(dbColumnName, StringComparison.CurrentCultureIgnoreCase));
                return mappingInfo == null ? dbColumnName : mappingInfo.PropertyName;
            }
        }
        public PropertyInfo GetProperty<T>(string dbColumnName)
        {
            var propertyName = GetPropertyName<T>(dbColumnName);
            return typeof(T).GetProperties().First(it => it.Name == propertyName);
        }
        /// <summary>
        /// Gets the text contents of this XML element node
        /// </summary>
        /// <param name="entityType">entity type</param>
        /// <param name="nodeAttributeName">The value of the name attribute of the XML node</param>
        /// <returns>the text contents of this XML element node</returns>
        public string GetXElementNodeValue(Type entityType, string nodeAttributeName)
        {

            try
            {

                if (this.Context.CurrentConnectionConfig?.MoreSettings?.IsNoReadXmlDescription == true)
                {
                    return "";
                }
                if (entityType.Assembly.IsDynamic&& entityType.Assembly.FullName.StartsWith("Dynamic"))
                {
                    return null;
                }
                var path = entityType.Assembly.Location;
                if (string.IsNullOrEmpty(path))
                {
                    return null;
                }
                FileInfo file = new FileInfo(path);
                string xmlPath = entityType.Assembly.Location.Replace(file.Extension, ".xml");
                if (!File.Exists(xmlPath))
                {
                    return string.Empty;
                }
                XElement xe =new ReflectionInoCacheService().GetOrCreate("EntityXml_"+xmlPath,()=> XElement.Load(xmlPath));
                if (xe == null)
                {
                    return string.Empty;
                }
                var xeNode = xe.Element("members").Elements("member").Where(ele => ele.Attribute("name").Value == nodeAttributeName).FirstOrDefault();
                if (xeNode == null)
                {
                    return string.Empty;
                }
                var summary = xeNode.Element("summary");
                if (summary != null)
                {
                    return summary.Value.ToSqlFilter().Trim();
                }
                else
                {
                    var summaryValue = xeNode.Elements().Where(x => x.Name.ToString().EqualCase("summary")).Select(it => it.Value).FirstOrDefault();
                    if(summaryValue==null)
                        return string.Empty;
                    else
                        return summaryValue.ToSqlFilter().Trim()??"";
                }

            }
            catch
            {
                Check.ExceptionEasy("ORM error reading entity class XML, check entity XML or disable reading XML: MoreSettings IsNoReadXmlDescription set to true (same place to set DbType)", "ORM读取实体类的XML出现错误,检查实体XML或者禁用读取XML: MoreSettings里面的IsNoReadXmlDescription设为true （设置DbType的同一个地方）");
                throw;
            }
        }
        /// <summary>
        /// Gets the code annotation for the database table
        /// </summary>
        /// <param name="entityType">entity type</param>
        /// <returns>the code annotation for the database table</returns>
        public string GetTableAnnotation(Type entityType)
        {
            if (entityType.IsClass() == false) 
            {
                return null;
            }
            var result= GetXElementNodeValue(entityType, $"T:{entityType.FullName}");
            if (string.IsNullOrEmpty(result))
            {
                return null;
            }
            else
            {
                return result;
            }
        }
        /// <summary>
        /// Gets the code annotation for the field
        /// </summary>
        /// <param name="entityType">entity type</param>
        /// <param name="dbColumnName">column name</param>
        /// <returns>the code annotation for the field</returns>
        public string GetPropertyAnnotation(Type entityType, string dbColumnName)
        {
            if (entityType.IsClass() == false || entityType == typeof(object))
            {
                return null;
            }

            var result = GetXElementNodeValue(entityType, $"P:{entityType.FullName}.{dbColumnName}");
            if (string.IsNullOrEmpty(result))
            {
                return GetPropertyAnnotation(entityType.BaseType, dbColumnName);
            }
            else
            {
                return result;
            }
        }

        #region Primary key
        private void SetColumns(EntityInfo result)
        {
            foreach (var property in result.Type.GetProperties())
            {
                EntityColumnInfo column = new EntityColumnInfo();
                //var isVirtual = property.GetGetMethod().IsVirtual;
                //if (isVirtual) continue;
                var navigat=property.GetCustomAttribute(typeof(Navigate));
                if (navigat != null) 
                {
                    column.IsIgnore = true;
                    column.Navigat = navigat as Navigate;
                }
                var sugarColumn = property.GetCustomAttributes(typeof(SugarColumn), true)
                .Where(it => it is SugarColumn)
                .Select(it => (SugarColumn)it)
                .FirstOrDefault();
                column.ExtendedAttribute = sugarColumn?.ExtendedAttribute;
                column.DbTableName = result.DbTableName;
                column.EntityName = result.EntityName;
                column.PropertyName = property.Name;
                column.PropertyInfo = property;
                column.UnderType = UtilMethods.GetUnderType(column.PropertyInfo.PropertyType);
                if (sugarColumn.IsNullOrEmpty())
                {
                    column.DbColumnName = property.Name;
                }
                else
                {
                    
                    if (sugarColumn.IsIgnore == false)
                    {
                        column.DbColumnName = sugarColumn.ColumnName.IsNullOrEmpty() ? property.Name : sugarColumn.ColumnName;
                        column.IsPrimarykey = sugarColumn.IsPrimaryKey;
                        column.IsIdentity = sugarColumn.IsIdentity;
                        column.ColumnDescription = sugarColumn.ColumnDescription.ToSqlFilter();
                        column.IsNullable = sugarColumn.IsNullable;
                        column.Length = sugarColumn.Length;
                        column.OldDbColumnName = sugarColumn.OldColumnName;
                        column.DataType = sugarColumn.ColumnDataType;
                        column.DecimalDigits = sugarColumn.DecimalDigits;
                        column.OracleSequenceName = sugarColumn.OracleSequenceName;
                        column.IsOnlyIgnoreInsert = sugarColumn.IsOnlyIgnoreInsert;
                        column.IsEnableUpdateVersionValidation = sugarColumn.IsEnableUpdateVersionValidation;
                        column.IsTranscoding = sugarColumn.IsTranscoding;
                        column.SerializeDateTimeFormat = sugarColumn.SerializeDateTimeFormat;
                        column.IsJson = sugarColumn.IsJson;
                        column.NoSerialize = sugarColumn.NoSerialize;
                        column.DefaultValue = sugarColumn.DefaultValue;
                        column.IndexGroupNameList = sugarColumn.IndexGroupNameList;
                        column.UIndexGroupNameList = sugarColumn.UniqueGroupNameList;
                        column.IsOnlyIgnoreUpdate = sugarColumn.IsOnlyIgnoreUpdate;
                        column.IsArray = sugarColumn.IsArray;
                        column.IsTreeKey = sugarColumn.IsTreeKey;
                        column.SqlParameterDbType = sugarColumn.SqlParameterDbType;
                        column.SqlParameterSize = sugarColumn.SqlParameterSize;
                        column.CreateTableFieldSort = sugarColumn.CreateTableFieldSort;
                        column.InsertServerTime = sugarColumn.InsertServerTime;
                        column.InsertSql = sugarColumn.InsertSql;
                        column.UpdateServerTime= sugarColumn.UpdateServerTime;
                        column.UpdateSql= sugarColumn.UpdateSql;
                        if (sugarColumn.IsJson && String.IsNullOrEmpty(sugarColumn.ColumnDataType))
                        {
                            if (this.Context.CurrentConnectionConfig.DbType == DbType.PostgreSQL)
                            {
                                column.DataType = "json";
                            }
                            else if (column.Length > 0)
                            {
                                column.DataType = "varchar";
                            } 
                            else
                            {
                                column.DataType = "varchar(4000)";
                            }
                        }
                        else if (typeof(Nvarchar2PropertyConvert).Equals(sugarColumn.SqlParameterDbType)&&column.DataType==null) 
                        {
                            column.DataType = "nvarchar2";
                            if (column.Length == 0) 
                            {
                                column.Length = 200;
                            }
                        }
                    }
                    else
                    {
                        column.IsIgnore = true;
                        column.NoSerialize = sugarColumn.NoSerialize;
                        column.ColumnDescription = sugarColumn.ColumnDescription;
                        column.IsJson = sugarColumn.IsJson;
                        column.IsArray = sugarColumn.IsArray;
                    }
                }
                if (column.ColumnDescription.IsNullOrEmpty()) column.ColumnDescription = GetPropertyAnnotation(result.Type, column.PropertyName);
                if (this.Context.MappingColumns.HasValue())
                {
                    var golbalMappingInfo = this.Context.MappingColumns.FirstOrDefault(it => it.EntityName.Equals(result.EntityName, StringComparison.CurrentCultureIgnoreCase) && it.PropertyName == column.PropertyName);
                    if (golbalMappingInfo != null)
                        column.DbColumnName = golbalMappingInfo.DbColumnName;
                }
                if (this.Context.IgnoreColumns.HasValue())
                {
                    var golbalMappingInfo = this.Context.IgnoreColumns.FirstOrDefault(it => it.EntityName.Equals(result.EntityName, StringComparison.CurrentCultureIgnoreCase) && it.PropertyName == column.PropertyName);
                    if (golbalMappingInfo != null)
                        column.IsIgnore = true;
                }
                if (this.Context.CurrentConnectionConfig.ConfigureExternalServices != null && this.Context.CurrentConnectionConfig.ConfigureExternalServices.EntityService != null)
                {
                    if (!column.EntityName.ObjToString().StartsWith("<>f__AnonymousType"))
                    {
                        this.Context.CurrentConnectionConfig.ConfigureExternalServices.EntityService(property, column);
                    }
                }
                if (column.PropertyInfo.DeclaringType != null
                    && column.PropertyInfo.DeclaringType != result.Type
                    &&result.Columns.Any(x=>x.PropertyName==column.PropertyName)) 
                {
                    continue;
                }
                if (column.DataType == null&& property != null&& property.PropertyType.Name.IsIn("TimeOnly")) 
                {
                    column.DataType = "time";
                }
                if (column.DataType == null && property != null && property.PropertyType.Name.IsIn("DateOnly"))
                {
                    column.DataType = "date";
                }
                if (column.DataType == null&&column.UnderType == typeof(TimeSpan) )
                {
                    column.DataType = "time";
                    column.Length = 0;
                    column.DecimalDigits = 0;
                }
                if (column.OracleSequenceName.HasValue() &&
                     this.Context.CurrentConnectionConfig?.MoreSettings?.EnableOracleIdentity == true) 
                {
                    column.OracleSequenceName = null;
                }
                result.Columns.Add(column);
            }
        }
        #endregion
    }
}
