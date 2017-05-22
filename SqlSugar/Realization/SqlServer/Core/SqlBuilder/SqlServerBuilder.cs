using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public class SqlServerBuilder : SqlBuilderProvider
    {
        public override string GetTranslationTableName(string name)
        {
            Check.ArgumentNullException(name, string.Format(ErrorMessage.ObjNotExist, "Table Name"));
            var context = this.Context;
            var mappingInfo = context
                .MappingTables
                .FirstOrDefault(it => it.EntityName.Equals(name, StringComparison.CurrentCultureIgnoreCase));
            return "[" + (mappingInfo == null ? name : mappingInfo.DbTableName) + "]";
        }
        public override string GetTranslationColumnName(string entityName, string propertyName)
        {
            Check.ArgumentNullException(entityName, string.Format(ErrorMessage.ObjNotExist, "Table Name"));
            Check.ArgumentNullException(propertyName, string.Format(ErrorMessage.ObjNotExist, "Column Name"));
            var context = this.Context;
            var mappingInfo = context
                 .MappingColumns
                 .FirstOrDefault(it => 
                 it.EntityName.Equals(entityName, StringComparison.CurrentCultureIgnoreCase)&&
                 it.PropertyName.Equals(propertyName, StringComparison.CurrentCultureIgnoreCase));
            return (mappingInfo == null ? "["+ propertyName + "]" : "["+mappingInfo.DbColumnName+"]");
        }

        public override string GetTranslationColumnName(string propertyName)
        {
            return "[" + propertyName + "]";
        }
    }
}
