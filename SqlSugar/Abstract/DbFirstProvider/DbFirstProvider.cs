using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace SqlSugar
{
    public abstract partial class DbFirstProvider : IDbFirst
    {
        public virtual SqlSugarClient Context { get; set; }
        private string ClassTemplate { get; set; }
        private string ClassDescriptionTemplate { get; set; }
        private string PropertyTemplate { get; set; }
        private string PropertyDescriptionTemplate { get; set; }
        private string ConstructorTemplate { get; set; }
        private string UsingTemplate { get; set; }
        private string Namespace { get; set; }
        private bool IsAttribute { get; set; }
        private bool IsDefaultValue { get; set; }
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
            this.TableInfoList = this.Context.DbMaintenance.GetTableInfoList();
            if (this.Context.DbMaintenance.GetViewInfoList().IsValuable())
            {
                this.TableInfoList.AddRange(this.Context.DbMaintenance.GetViewInfoList());
            }
        }

        public List<SchemaInfo> GetSchemaInfoList
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        #region Setting Template
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
        #endregion

        #region Setting Content
        public IDbFirst IsCreateAttribute(bool isCreateAttribute = true)
        {
            this.IsAttribute = isCreateAttribute;
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

        public IDbFirst Where(params string[] objectNames)
        {
            if (objectNames.IsValuable())
            {
                this.TableInfoList = this.TableInfoList.Where(it => objectNames.Contains(it.Name)).ToList();
            }
            return this;
        }
        #endregion

        public Dictionary<string, string> ToClassStringList(string nameSpace = "Models")
        {
            this.Namespace = nameSpace;
            Dictionary<string, string> result = new Dictionary<string, string>();
            if (this.TableInfoList.IsValuable())
            {
                foreach (var tableInfo in this.TableInfoList)
                {
                    var columns = this.Context.DbMaintenance.GetColumnInfosByTableName(tableInfo.Name);
                    string className = tableInfo.Name;
                    string classText = this.ClassTemplate;
                    string ConstructorText =IsDefaultValue? DbFirstTemplate.ConstructorTemplate:null;
                    if (this.Context.MappingTables.IsValuable())
                    {
                        var mappingInfo = this.Context.MappingTables.FirstOrDefault(it => it.DbTableName.Equals(tableInfo.Name, StringComparison.CurrentCultureIgnoreCase));
                        if (mappingInfo.IsValuable())
                        {
                            className = mappingInfo.EntityName;
                        }
                        if (mappingInfo != null)
                        {
                            classText = classText.Replace(DbFirstTemplate.KeyClassName, mappingInfo.EntityName);
                        }
                    }
                    classText = classText.Replace(DbFirstTemplate.KeyClassName, className);
                    classText = classText.Replace(DbFirstTemplate.KeyNamespace, this.Namespace);
                    classText = classText.Replace(DbFirstTemplate.KeyUsing, IsAttribute ? (this.UsingTemplate + "using " + PubConst.AssemblyName + "\r\t") : this.UsingTemplate);
                    classText = classText.Replace(DbFirstTemplate.KeyClassDescription,DbFirstTemplate.ClassDescriptionTemplate.Replace(DbFirstTemplate.KeyClassDescription, tableInfo.Description+"\r\n"));
                    classText = classText.Replace(DbFirstTemplate.KeySugarTable, IsAttribute ? string.Format(DbFirstTemplate.ValueSugarTable, tableInfo.Name) : null);
                    if (columns.IsValuable())
                    {
                        foreach (var item in columns)
                        {
                            var isLast = columns.Last() == item;
                            string PropertyText = DbFirstTemplate.PropertyTemplate;
                            string PropertyDescriptionText = DbFirstTemplate.PropertyDescriptionTemplate;
                            PropertyText = GetPropertyText(item, PropertyText);
                            PropertyDescriptionText = GetPropertyDescriptionText(item, PropertyDescriptionText);
                            PropertyText = PropertyDescriptionText + PropertyText;
                            classText = classText.Replace(DbFirstTemplate.KeyPropertyName, PropertyText + (isLast?"":("\r\n" + DbFirstTemplate.KeyPropertyName)));
                        }
                    }
                    classText = classText.Replace(DbFirstTemplate.KeyConstructor, ConstructorText);
                    result.Add(className, classText);
                }
            }
            return result;
        }

        public void CreateClassFile(string directoryPath, string nameSpace = "Models")
        {
            Check.ArgumentNullException(directoryPath, "directoryPath can't null");
            var classStringList = ToClassStringList(nameSpace);
            if (classStringList.IsValuable())
            {
                foreach (var item in classStringList)
                {
                    FileHeper.CreateFile(directoryPath.TrimEnd('\\').TrimEnd('/') + string.Format("\\{0}.cs", item.Key), item.Value, Encoding.UTF8);
                }
            }
        }

        #region Private methods
        private string GetPropertyText(DbColumnInfo item, string PropertyText)
        {
            string SugarColumnText = DbFirstTemplate.ValueSugarCoulmn;
            var hasSugarColumn = item.IsPrimarykey == true || item.IsIdentity == true || item.DbColumnName.IsValuable();
            if (hasSugarColumn&&this.IsAttribute)
            {
            }
            else
            {
                SugarColumnText = null;
            }
            string typeString = this.Context.Ado.DbBind.ChangeDBTypeToCSharpType(item.DataType);
            if (typeString != "string" && item.IsNullable) {
                typeString += "?";
            }
            PropertyText = PropertyText.Replace(DbFirstTemplate.KeySugarColumn, SugarColumnText);
            PropertyText = PropertyText.Replace(DbFirstTemplate.KeyPropertyType, typeString);
            PropertyText = PropertyText.Replace(DbFirstTemplate.KeyPropertyName, item.DbColumnName);
            return PropertyText;
        }
        private string GetPropertyDescriptionText(DbColumnInfo item, string propertyDescriptionText)
        {
            propertyDescriptionText = propertyDescriptionText.Replace(DbFirstTemplate.KeyPropertyDescription, item.ColumnDescription);
            propertyDescriptionText = propertyDescriptionText.Replace(DbFirstTemplate.KeyDefaultValue, item.Value.ObjToString());
            propertyDescriptionText = propertyDescriptionText.Replace(DbFirstTemplate.KeyIsNullable, item.IsNullable.ObjToString());
            return propertyDescriptionText;
        }

        #endregion
    }
}
