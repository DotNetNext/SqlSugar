using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace SqlSugar
{
    public abstract partial class DbFirstProvider : IDbFirst
    {
        public virtual SqlSugarClient Context { get; set; }

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
            throw new NotImplementedException();
        }

        public IDbFirst Where(Func<string, bool> func)
        {
            throw new NotImplementedException();
        }

        public IDbFirst Where(params string[] objectNames)
        {
            throw new NotImplementedException();
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
