﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SqlSugar
{
    public abstract partial class DbFirstProvider : IDbFirst
    {
        public virtual ISqlSugarClient Context { get; set; }
        private string ClassTemplate { get; set; }
        private string ClassDescriptionTemplate { get; set; }
        private string PropertyTemplate { get; set; }
        private string PropertyDescriptionTemplate { get; set; }
        private string ConstructorTemplate { get; set; }
        private string UsingTemplate { get; set; }
        private string Namespace { get; set; }
        private bool IsAttribute { get; set; }
        private bool IsDefaultValue { get; set; }
        private Func<string, bool> WhereColumnsfunc;
        private Func<string, string> FormatFileNameFunc { get; set; }
        private Func<string, string> FormatClassNameFunc { get; set; }
        private Func<string, string> FormatPropertyNameFunc { get; set; }
        private bool IsStringNullable {get;set; }
        private Func<DbColumnInfo,string,string,string> PropertyTextTemplateFunc { get; set; }
        private Func<string, string> ReplaceClassStringFunc { get; set; }
        private ISqlBuilder SqlBuilder
        {
            get
            {
                return InstanceFactory.GetSqlbuilder(this.Context.CurrentConnectionConfig);
            }
        }
        private List<DbTableInfo> TableInfoList { get; set; }

        public DbFirstProvider()
        {
            this.ClassTemplate = DbFirstTemplate.ClassTemplate;
            this.ClassDescriptionTemplate = DbFirstTemplate.ClassDescriptionTemplate;
            this.PropertyTemplate = DbFirstTemplate.PropertyTemplate;
            this.PropertyDescriptionTemplate = DbFirstTemplate.PropertyDescriptionTemplate;
            this.ConstructorTemplate = DbFirstTemplate.ConstructorTemplate;
            this.UsingTemplate = DbFirstTemplate.UsingTemplate;
        }

        public void Init()
        {
            this.Context.Utilities.RemoveCacheAll();
            if (!this.Context.DbMaintenance.IsAnySystemTablePermissions())
            {
                Check.Exception(true, "Dbfirst and  Codefirst requires system table permissions");
            }
            this.TableInfoList = this.Context.Utilities.TranslateCopy(this.Context.DbMaintenance.GetTableInfoList());
            var viewList = this.Context.Utilities.TranslateCopy(this.Context.DbMaintenance.GetViewInfoList());
            if (viewList.HasValue())
            {
                this.TableInfoList.AddRange(viewList);
            }
        }

        #region Setting Template
        public IDbFirst StringNullable() 
        {
            IsStringNullable = true;
            return this;
        }
        public IDbFirst SettingClassDescriptionTemplate(Func<string, string> func)
        {
            this.ClassDescriptionTemplate = func(this.ClassDescriptionTemplate);
            return this;
        }

        public IDbFirst SettingClassTemplate(Func<string, string> func)
        {
            this.ClassTemplate = func(this.ClassTemplate);
            return this;
        }

        public IDbFirst SettingConstructorTemplate(Func<string, string> func)
        {
            this.ConstructorTemplate = func(this.ConstructorTemplate);
            return this;
        }

        public IDbFirst SettingPropertyDescriptionTemplate(Func<string, string> func)
        {
            this.PropertyDescriptionTemplate = func(this.PropertyDescriptionTemplate);
            return this;
        }

        public IDbFirst SettingNamespaceTemplate(Func<string, string> func)
        {
            this.UsingTemplate = func(this.UsingTemplate);
            return this;
        }

        public IDbFirst SettingPropertyTemplate(Func<string, string> func)
        {
            this.PropertyTemplate = func(this.PropertyTemplate);
            return this;
        }
        public IDbFirst SettingPropertyTemplate(Func<DbColumnInfo, string,string,string> func)
        {
            this.PropertyTextTemplateFunc = func;
            return this;
        }
        public RazorFirst UseRazorAnalysis(string razorClassTemplate, string classNamespace = "Models")
        {
            if (razorClassTemplate == null)
            {
                razorClassTemplate = "";
            }
            razorClassTemplate = razorClassTemplate.Replace("@Model.Namespace", classNamespace);
            var result = new RazorFirst();
            if (this.Context.CurrentConnectionConfig.ConfigureExternalServices?.RazorService != null)
            {
                List<RazorTableInfo> razorList = new List<RazorTableInfo>();
                var tables = this.TableInfoList;
                if (tables.HasValue())
                {
                    foreach (var item in tables)
                    {
                        var columns = this.Context.DbMaintenance.GetColumnInfosByTableName(item.Name, false);
                        RazorTableInfo table = new RazorTableInfo()
                        {
                            Columns = columns.Where(it => WhereColumnsfunc == null || WhereColumnsfunc(it.DbColumnName)).Select(it => new RazorColumnInfo()
                            {
                                ColumnDescription = it.ColumnDescription,
                                DataType = it.DataType,
                                DbColumnName = it.DbColumnName,
                                DefaultValue = it.DefaultValue,
                                IsIdentity = it.IsIdentity,
                                IsNullable = it.IsNullable,
                                IsPrimarykey = it.IsPrimarykey,
                                Length = it.Length
                            }).ToList(),
                            Description = item.Description,
                            DbTableName = item.Name
                        };
                        foreach (var col in table.Columns)
                        {
                            col.DataType = GetPropertyTypeName(columns.First(it => it.DbColumnName == col.DbColumnName));
                        }
                        razorList.Add(table);
                    }
                }
                result.ClassStringList = this.Context.CurrentConnectionConfig.ConfigureExternalServices.RazorService.GetClassStringList(razorClassTemplate, razorList);
            }
            else
            {
                Check.Exception(true, ErrorMessage.GetThrowMessage("Need to achieve ConnectionConfig.ConfigureExternal Services.RazorService", "需要实现 ConnectionConfig.ConfigureExternal Services.RazorService接口"));
            }
            this.Context.Utilities.RemoveCacheAll();
            result.FormatFileNameFunc = this.FormatFileNameFunc;
            return result;
        }
        #endregion

        #region Setting Content
        public IDbFirst IsCreateAttribute(bool isCreateAttribute = true)
        {
            this.IsAttribute = isCreateAttribute;
            return this;
        }
        public IDbFirst FormatFileName(Func<string, string> formatFileNameFunc)
        {
            this.FormatFileNameFunc = formatFileNameFunc;
            return this;
        }
        public IDbFirst FormatClassName(Func<string, string> formatClassNameFunc) 
        {
            this.FormatClassNameFunc = formatClassNameFunc;
            return this;
        }
        public IDbFirst FormatPropertyName(Func<string, string> formatPropertyNameFunc) 
        {
            this.FormatPropertyNameFunc = formatPropertyNameFunc;
            return this;
        }
        public IDbFirst CreatedReplaceClassString(Func<string, string> replaceClassStringFunc) 
        {
            this.ReplaceClassStringFunc = replaceClassStringFunc;
            return this;
        }
        public IDbFirst IsCreateDefaultValue(bool isCreateDefaultValue = true)
        {
            this.IsDefaultValue = isCreateDefaultValue;
            return this;
        }
        #endregion

        #region Where
        public IDbFirst Where(DbObjectType dbObjectType)
        {
            if (dbObjectType != DbObjectType.All)
                this.TableInfoList = this.TableInfoList.Where(it => it.DbObjectType == dbObjectType).ToList();
            return this;
        }

        public IDbFirst Where(Func<string, bool> func)
        {
            this.TableInfoList = this.TableInfoList.Where(it => func(it.Name)).ToList();
            return this;
        }

        public IDbFirst WhereColumns(Func<string, bool> func)
        {
            WhereColumnsfunc = func;
            return this;
        }


        public IDbFirst Where(params string[] objectNames)
        {
            if (objectNames.HasValue())
            {
                this.TableInfoList = this.TableInfoList.Where(it => objectNames.Select(x => x.ToLower()).Contains(it.Name.ToLower())).ToList();
            }
            return this;
        }
        #endregion

        #region Core
        public Dictionary<string, string> ToClassStringList(string nameSpace = "Models")
        {
            this.Namespace = nameSpace;
            Dictionary<string, string> result = new Dictionary<string, string>();
            if (this.TableInfoList.HasValue())
            {
                foreach (var tableInfo in this.TableInfoList)
                {
                    try
                    {
                        string classText = null;
                        string className = tableInfo.Name;
                        var oldClasName = className;
                        classText = GetClassString(tableInfo, ref className);
                        result.Remove(className);
                        if (this.ReplaceClassStringFunc != null) 
                        {
                            classText=this.ReplaceClassStringFunc(classText);
                        }
                        if (FormatClassNameFunc != null&&FormatFileNameFunc != null) 
                        {
                            className = oldClasName;
                        }
                        result.Add(className, classText);
                    }
                    catch (Exception ex)
                    {
                        Check.Exception(true, "Table '{0}' error,You can filter it with Db.DbFirst.Where(name=>name!=\"{0}\" ) \r\n Error message:{1}", tableInfo.Name, ex.Message);
                    }
                }
            }
            return result;
        }

        internal string GetClassString(DbTableInfo tableInfo, ref string className)
        {
            string classText;
            var columns = this.Context.DbMaintenance.GetColumnInfosByTableName(tableInfo.Name,false);
            if (this.Context.IgnoreColumns.HasValue())
            {
                var entityName = this.Context.EntityMaintenance.GetEntityName(tableInfo.Name);
                columns = columns.Where(c =>
                                         !this.Context.IgnoreColumns.Any(ig => ig.EntityName.Equals(entityName, StringComparison.CurrentCultureIgnoreCase) && c.DbColumnName == ig.PropertyName)
                                        ).ToList();
            }
            classText = this.ClassTemplate;
            string ConstructorText = IsDefaultValue ? this.ConstructorTemplate : null;
            if (this.Context.MappingTables.HasValue())
            {
                var mappingInfo = this.Context.MappingTables.FirstOrDefault(it => it.DbTableName.Equals(tableInfo.Name, StringComparison.CurrentCultureIgnoreCase));
                if (mappingInfo.HasValue())
                {
                    className = mappingInfo.EntityName;
                }
                if (mappingInfo != null)
                {
                    classText = classText.Replace(DbFirstTemplate.KeyClassName, mappingInfo.EntityName);
                }
            }
            if (FormatClassNameFunc != null) 
            {
                className=FormatClassNameFunc(className);
            }
            classText = classText.Replace(DbFirstTemplate.KeyClassName, className);
            classText = classText.Replace(DbFirstTemplate.KeyNamespace, this.Namespace);
            classText = classText.Replace(DbFirstTemplate.KeyUsing, IsAttribute ? (this.UsingTemplate + "using " + UtilConstants.AssemblyName + ";\r\n") : this.UsingTemplate);
            classText = classText.Replace(DbFirstTemplate.KeyClassDescription, this.ClassDescriptionTemplate.Replace(DbFirstTemplate.KeyClassDescription, tableInfo.Description?.Replace(Environment.NewLine,"\t") + "\r\n"));
            classText = classText.Replace(DbFirstTemplate.KeySugarTable, IsAttribute ? string.Format(DbFirstTemplate.ValueSugarTable, tableInfo.Name) : null);
            if (columns.HasValue())
            {
                foreach (var item in columns.Where(it => WhereColumnsfunc == null || WhereColumnsfunc(it.DbColumnName)))
                {
                    var isLast = columns.Last() == item;
                    var index = columns.IndexOf(item);
                    string PropertyText = this.PropertyTemplate;
                    string PropertyDescriptionText = this.PropertyDescriptionTemplate;
                    string propertyName = GetPropertyName(item); 
                    var oldPropertyName = propertyName;
                    if (FormatPropertyNameFunc != null) 
                    {
                      item.DbColumnName=propertyName = FormatPropertyNameFunc(propertyName);
                    }
                    string propertyTypeName = GetPropertyTypeName(item);
                    PropertyText =this.PropertyTextTemplateFunc == null? GetPropertyText(item, PropertyText):this.PropertyTextTemplateFunc(item,this.PropertyTemplate, propertyTypeName);
                    PropertyDescriptionText = GetPropertyDescriptionText(item, PropertyDescriptionText);
                    if (this.IsAttribute && item.DataType?.StartsWith("_") == true && PropertyText.Contains("[]"))
                    {
                        PropertyDescriptionText += "\r\n           [SugarColumn(IsArray=true)]";
                    }
                    else if (item?.DataType?.StartsWith("json") == true)
                    {
                        PropertyDescriptionText += "\r\n           [SugarColumn(IsJson=true)]";
                    }
                    else if (FormatPropertyNameFunc != null) 
                    {
                        if (PropertyText.Contains("SugarColumn"))
                        { 
                            PropertyText  = PropertyText.Replace(")]", ",ColumnName=\"" + oldPropertyName + "\")]");
                        }
                        else
                        {
                            PropertyDescriptionText += "\r\n           [SugarColumn(ColumnName=\"" + oldPropertyName + "\")]";
                        }
                    }
                    PropertyText = PropertyDescriptionText + PropertyText;
                    classText = classText.Replace(DbFirstTemplate.KeyPropertyName, PropertyText + (isLast ? "" : ("\r\n" + DbFirstTemplate.KeyPropertyName)));
                    if (ConstructorText.HasValue() && item.DefaultValue != null&&item.IsIdentity!=true)
                    {
                        var hasDefaultValue = columns.Skip(index + 1).Any(it => it.DefaultValue.HasValue());
                        if (item.DefaultValue.EqualCase("CURRENT_TIMESTAMP"))
                        {
                            item.DefaultValue = "DateTime.Now";
                        }
                        else if (item.DefaultValue == "b'1'") 
                        {
                            item.DefaultValue = "1";
                        }
                        ConstructorText = ConstructorText.Replace(DbFirstTemplate.KeyPropertyName, propertyName);
                        ConstructorText = ConstructorText.Replace(DbFirstTemplate.KeyDefaultValue, GetPropertyTypeConvert(item)) + (!hasDefaultValue ? "" : this.ConstructorTemplate);
                    }
                }
            }
            if (!columns.Any(it => it.DefaultValue != null&&it.IsIdentity==false))
            {
                ConstructorText = null;
            }
            classText = classText.Replace(DbFirstTemplate.KeyConstructor, ConstructorText);
            classText = classText.Replace(DbFirstTemplate.KeyPropertyName, null);
            return classText;
        }

        internal string GetClassString(List<DbColumnInfo> columns, ref string className)
        {
            string classText = this.ClassTemplate;
            string ConstructorText = IsDefaultValue ? this.ConstructorTemplate : null;
            classText = classText.Replace(DbFirstTemplate.KeyClassName, className);
            classText = classText.Replace(DbFirstTemplate.KeyNamespace, this.Namespace);
            classText = classText.Replace(DbFirstTemplate.KeyUsing, IsAttribute ? (this.UsingTemplate + "using " + UtilConstants.AssemblyName + ";\r\n") : this.UsingTemplate);
            classText = classText.Replace(DbFirstTemplate.KeyClassDescription, this.ClassDescriptionTemplate.Replace(DbFirstTemplate.KeyClassDescription, "\r\n"));
            classText = classText.Replace(DbFirstTemplate.KeySugarTable, IsAttribute ? string.Format(DbFirstTemplate.ValueSugarTable, className) : null);
            if (columns.HasValue())
            {
                foreach (var item in columns)
                {
                    var isLast = columns.Last() == item;
                    var index = columns.IndexOf(item);
                    string PropertyText = this.PropertyTemplate;
                    string PropertyDescriptionText = this.PropertyDescriptionTemplate;
                    string propertyName = GetPropertyName(item);
                    string propertyTypeName = item.PropertyName;
                    PropertyText = GetPropertyText(item, PropertyText);
                    PropertyDescriptionText = GetPropertyDescriptionText(item, PropertyDescriptionText);
                    PropertyText = PropertyDescriptionText + PropertyText;
                    classText = classText.Replace(DbFirstTemplate.KeyPropertyName, PropertyText + (isLast ? "" : ("\r\n" + DbFirstTemplate.KeyPropertyName)));
                    if (ConstructorText.HasValue() && item.DefaultValue != null)
                    {
                        var hasDefaultValue = columns.Skip(index + 1).Any(it => it.DefaultValue.HasValue());
                        ConstructorText = ConstructorText.Replace(DbFirstTemplate.KeyPropertyName, propertyName);
                        ConstructorText = ConstructorText.Replace(DbFirstTemplate.KeyDefaultValue, GetPropertyTypeConvert(item)) + (!hasDefaultValue ? "" : this.ConstructorTemplate);
                    }
                }
            }
            if (!columns.Any(it => it.DefaultValue != null))
            {
                ConstructorText = null;
            }
            classText = classText.Replace(DbFirstTemplate.KeyConstructor, ConstructorText);
            classText = classText.Replace(DbFirstTemplate.KeyPropertyName, null);
            return classText;
        }
        public void CreateClassFile(string directoryPath, string nameSpace = "Models")
        {
            var seChar = Path.DirectorySeparatorChar.ToString();
            Check.ArgumentNullException(directoryPath, "directoryPath can't null");
            var classStringList = ToClassStringList(nameSpace);
            if (classStringList.IsValuable())
            {
                foreach (var item in classStringList)
                {
                    var fileName = item.Key;
                    if (FormatFileNameFunc!= null)
                    {
                        fileName = FormatFileNameFunc(fileName);
                     }
                    var filePath = directoryPath.TrimEnd('\\').TrimEnd('/') + string.Format(seChar + "{0}.cs", fileName);
                    FileHelper.CreateFile(filePath, item.Value, Encoding.UTF8);
                }
            }
        }
        #endregion

        #region Private methods
        private string GetProertypeDefaultValue(DbColumnInfo item)
        {
            var result = item.DefaultValue;
            if (result == null) return null;
            if (Regex.IsMatch(result, @"^\(\'(.+)\'\)$"))
            {
                result = Regex.Match(result, @"^\(\'(.+)\'\)$").Groups[1].Value;
            }
            if (Regex.IsMatch(result, @"^\(\((.+)\)\)$"))
            {
                result = Regex.Match(result, @"^\(\((.+)\)\)$").Groups[1].Value;
            }
            if (Regex.IsMatch(result, @"^\((.+)\)$"))
            {
                result = Regex.Match(result, @"^\((.+)\)$").Groups[1].Value;
            }
            if (result.Equals(this.SqlBuilder.SqlDateNow, StringComparison.CurrentCultureIgnoreCase))
            {
                result = "DateTime.Now";
            }
            if (result.Equals("getdate()", StringComparison.CurrentCultureIgnoreCase))
            {
                result = "DateTime.Now";
            }
            if (result.Equals("getutcdate()", StringComparison.CurrentCultureIgnoreCase))
            {
                result = "DateTime.Now";
            }
            if (result.Equals("NOW()", StringComparison.CurrentCultureIgnoreCase))
            {
                result = "DateTime.Now";
            }
            result = result.Replace("\r", "\t").Replace("\n", "\t");
            result = result.IsIn("''", "\"\"") ? string.Empty : result;
            return result;
        }
        private string GetPropertyText(DbColumnInfo item, string PropertyText)
        {
            string SugarColumnText = DbFirstTemplate.ValueSugarCoulmn;
            var propertyName = GetPropertyName(item);
            var isMappingColumn = propertyName != item.DbColumnName;
            var hasSugarColumn = item.IsPrimarykey == true || item.IsIdentity == true || isMappingColumn;
            if (hasSugarColumn && this.IsAttribute)
            {
                List<string> joinList = new List<string>();
                if (item.IsPrimarykey)
                {
                    joinList.Add("IsPrimaryKey=true");
                }
                if (item.IsIdentity)
                {
                    joinList.Add("IsIdentity=true");
                }
                if (isMappingColumn)
                {
                    joinList.Add("ColumnName=\"" + item.DbColumnName + "\"");
                }
                SugarColumnText = string.Format(SugarColumnText, string.Join(",", joinList));
            }
            else
            {
                SugarColumnText = null;
            }
            string typeString = GetPropertyTypeName(item);
            PropertyText = PropertyText.Replace(DbFirstTemplate.KeySugarColumn, SugarColumnText);
            PropertyText = PropertyText.Replace(DbFirstTemplate.KeyPropertyType, typeString);
            PropertyText = PropertyText.Replace(DbFirstTemplate.KeyPropertyName, propertyName);
            if (typeString == "string"&&this.IsStringNullable&&item.IsNullable==false&&PropertyText.EndsWith("{get;set;}\r\n")) 
            {
                PropertyText=PropertyText.Replace("{get;set;}\r\n", "{get;set;} = null!;\r\n");
            }
            return PropertyText;
        }
        private string GetEnityName(DbColumnInfo item)
        {
            var mappingInfo = this.Context.MappingTables.FirstOrDefault(it => it.DbTableName.Equals(item.TableName, StringComparison.CurrentCultureIgnoreCase));
            return mappingInfo == null ? item.TableName : mappingInfo.EntityName;
        }
        private string GetPropertyName(DbColumnInfo item)
        {
            if (this.Context.MappingColumns.HasValue())
            {
                var mappingInfo = this.Context.MappingColumns.SingleOrDefault(it => it.DbColumnName == item.DbColumnName && it.EntityName == GetEnityName(item));
                return mappingInfo == null ? item.DbColumnName : mappingInfo.PropertyName;
            }
            else
            {
                return item.DbColumnName;
            }
        }
        protected virtual string GetPropertyTypeName(DbColumnInfo item)
        {
            string result = item.PropertyType != null ? item.PropertyType.Name : this.Context.Ado.DbBind.GetPropertyTypeName(item.DataType);
            if (result != "string" && result != "byte[]" && result != "object" && item.IsNullable)
            {
                result += "?";
            }
            if (result == "Int32")
            {
                result = item.IsNullable?"int?":"int";
            }
            if (result == "String")
            {
                result = "string";
            }
            if (result == "string" && item.IsNullable && IsStringNullable) 
            {
                result = result + "?";
            }
            if (item.OracleDataType.EqualCase("raw") && item.Length == 16) 
            {
                return "Guid";
            }
            if (item.OracleDataType.EqualCase("number") && item.Length == 1&&item.Scale==0)
            {
                return "bool";
            }
            if (result.EqualCase("char")|| result.EqualCase("char?")) 
            {
                return "string";
            }
            if (item.DataType == "tinyint unsigned") 
            {
                return "short"; 
            }
            if (item.DataType == "smallint unsigned")
            {
                return "ushort";
            }
            if (item.DataType == "bigint unsigned")
            {
                return "ulong";
            }
            if (item.DataType == "int unsigned")
            {
                return "uint";
            }
            if (item.DataType == "MediumInt")
            {
                return "int";
            }
            if (item.DataType == "MediumInt unsigned")
            {
                return "uint";
            }
            return result;
        }
        private string GetPropertyTypeConvert(DbColumnInfo item)
        {
            var convertString = GetProertypeDefaultValue(item);
            if (convertString == "DateTime.Now" || convertString == null)
                return convertString;
            if (convertString.ObjToString() == "newid()")
            {
                return "Guid.NewGuid()";
            }
            if (item.DataType?.ToString()?.EndsWith("unsigned")==true) 
            {
                return convertString;
            }
            if (item.DataType == "bit")
                return (convertString == "1" || convertString.Equals("true", StringComparison.CurrentCultureIgnoreCase)).ToString().ToLower();
            if (convertString.EqualCase("NULL")) 
            {
                return "null";
            }
            string result = this.Context.Ado.DbBind.GetConvertString(item.DataType) + "(\"" + convertString + "\")";
            if (this.SqlBuilder.SqlParameterKeyWord == ":"&&!string.IsNullOrEmpty(item.OracleDataType)) 
            {
                result = this.Context.Ado.DbBind.GetConvertString(item.OracleDataType) + "(\"" + convertString + "\")";
            }
            return result;
        }
        private string GetPropertyDescriptionText(DbColumnInfo item, string propertyDescriptionText)
        {
            propertyDescriptionText = propertyDescriptionText.Replace(DbFirstTemplate.KeyPropertyDescription, GetColumnDescription(item.ColumnDescription));
            propertyDescriptionText = propertyDescriptionText.Replace(DbFirstTemplate.KeyDefaultValue, GetProertypeDefaultValue(item));
            propertyDescriptionText = propertyDescriptionText.Replace(DbFirstTemplate.KeyIsNullable, item.IsNullable.ObjToString());
            return propertyDescriptionText;
        }
        private string GetColumnDescription(string columnDescription)
        {
            if (columnDescription == null) return columnDescription;
            columnDescription = columnDescription.Replace("\r", "\t");
            columnDescription = columnDescription.Replace("\n", "\t");
            columnDescription = columnDescription.Replace(Environment.NewLine, "\t");
            columnDescription = Regex.Replace(columnDescription, "\t{2,}", "\t");
            return columnDescription;
        }
        #endregion
    }
}
