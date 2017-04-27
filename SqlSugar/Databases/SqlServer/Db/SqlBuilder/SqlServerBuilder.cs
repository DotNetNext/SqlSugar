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
            Check.ArgumentNullException(name, string.Format(ErrorMessage.ObjNotExist, "Table Name Or Column Name"));
            var context = this.Context;
            var mappingInfo = context
                .MappingTables
                .FirstOrDefault(it => it.EntityName.Equals(name, StringComparison.CurrentCultureIgnoreCase));
            return "[" + (mappingInfo == null ? name : mappingInfo.DbTableName) + "]";
        }
        public override string GetTableName(string name)
        {
            Check.ArgumentNullException(name, string.Format(ErrorMessage.ObjNotExist, "Table Name Or Column Name"));
            var context = this.Context;
            var mappingInfo = context
                .MappingTables
                .FirstOrDefault(it => it.EntityName.Equals(name, StringComparison.CurrentCultureIgnoreCase));
            return "[" + (mappingInfo == null ? name : mappingInfo.DbTableName) + "]";
        }
        public override string GetTranslationColumnName(string name)
        {
            Check.ArgumentNullException(name, string.Format(ErrorMessage.ObjNotExist, "Table Name Or Column Name"));
            var context = this.Context;
            var mappingInfo = context
                 .MappingColumns
                 .FirstOrDefault(it => it.EntityPropertyName.Equals(name, StringComparison.CurrentCultureIgnoreCase));
            return (mappingInfo == null ? name : mappingInfo.DbColumnName);
        }
    }
}
