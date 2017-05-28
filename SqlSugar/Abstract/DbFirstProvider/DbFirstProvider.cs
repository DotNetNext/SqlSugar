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
        private string NamespaceTemplate { get; set; }
        private string Namespace { get; set; }
        private bool IsAttribute { get; set; }
        private bool IsDefaultValue { get; set; }
        private List<DbTableInfo> TableInfoList { get; set; }

        public DbFirstProvider()
        {
            this.ClassTemplate = DefaultTemplate.ClassTemplate;
            this.ClassDescriptionTemplate = DefaultTemplate.ClassDescriptionTemplate;
            this.PropertyTemplate = DefaultTemplate.PropertyTemplate;
            this.PropertyDescriptionTemplate = DefaultTemplate.PropertyDescriptionTemplate;
            this.ConstructorTemplate = DefaultTemplate.ConstructorTemplate;
            this.NamespaceTemplate = DefaultTemplate.NamespaceTemplate;
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
            this.NamespaceTemplate = func(this.NamespaceTemplate);
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
            Dictionary<string, string> result = new Dictionary<string, string>();
            if (this.TableInfoList.IsValuable())
            {
                foreach (var tableInfo in this.TableInfoList)
                {

                }
            }
            this.Namespace = nameSpace;
            return result;
        }
        public void CreateClassFile(string directoryPath, string nameSpace = "Models")
        {
            Check.ArgumentNullException(directoryPath, "directoryPath can't null");
            this.Namespace = nameSpace;
            var classStringList = ToClassStringList(nameSpace);
            if (classStringList.IsValuable())
            {
                foreach (var item in classStringList)
                {
                    string className = item.Key;
                    if (this.Context.MappingTables.IsValuable())
                    {
                        var mappingInfo = this.Context.MappingTables.FirstOrDefault(it => it.DbTableName.Equals(item.Key, StringComparison.CurrentCultureIgnoreCase));
                        if (mappingInfo.IsValuable())
                        {
                            className = mappingInfo.EntityName;
                        }
                    }
                    FileHeper.CreateFile(directoryPath.TrimEnd('\\').TrimEnd('/') + string.Format("{0}\\.cs", className), item.Value, Encoding.UTF8);
                }
            }
        }

        #region Private methods

        #endregion
    }
}
