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
        public IDbFirst SettingClassDescriptionTemplate(Func<string> func)
        {
            throw new NotImplementedException();
        }

        public IDbFirst SettingClassTemplate(Func<string> func)
        {
            throw new NotImplementedException();
        }

        public IDbFirst SettingConstructorTemplate(Func<string> func)
        {
            throw new NotImplementedException();
        }

        public IDbFirst SettingPropertyDescriptionTemplate(Func<string> func)
        {
            throw new NotImplementedException();
        }

        public IDbFirst SettingNamespaceTemplate(Func<string> func)
        {
            throw new NotImplementedException();
        }

        public IDbFirst SettingPropertyTemplate(Func<string> func)
        {

            throw new NotImplementedException();
        }
        #endregion

        #region Setting Content
        public IDbFirst IsCreateAttribute(bool isCreateAttribute = true)
        {
            return null;
        }
        public IDbFirst IsCreateDefaultValue(bool isCreateDefaultValue = true)
        {
            return null;
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
            this.TableInfoList=this.TableInfoList.Where(it => func(it.Name)).ToList();
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

        public void ToClassStringList()
        {
            throw new NotImplementedException();
        }
        public void CreateClassFile(string savePath)
        {
            throw new NotImplementedException();
        }
    }
}
