using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace SqlSugar
{
    public class SqliteBuilder : SqlBuilderProvider
    {

        public override string GetTranslationTableName(string name)
        {
            if (name.Contains("`")) return name;
            Check.ArgumentNullException(name, string.Format(ErrorMessage.ObjNotExist, "Table Name"));
            var context = this.Context;
            var mappingInfo = context
                .MappingTables
                .FirstOrDefault(it => it.EntityName.Equals(name, StringComparison.CurrentCultureIgnoreCase));
            return "`" + (mappingInfo == null ? name : mappingInfo.DbTableName) + "`";
        }
        public override string GetTranslationColumnName(string entityName, string propertyName)
        {
            Check.ArgumentNullException(entityName, string.Format(ErrorMessage.ObjNotExist, "Table Name"));
            Check.ArgumentNullException(propertyName, string.Format(ErrorMessage.ObjNotExist, "Column Name"));
            var context = this.Context;
            var mappingInfo = context
                 .MappingColumns
                 .FirstOrDefault(it =>
                 it.EntityName.Equals(entityName, StringComparison.CurrentCultureIgnoreCase) &&
                 it.PropertyName.Equals(propertyName, StringComparison.CurrentCultureIgnoreCase));
            return (mappingInfo == null ? "`" + propertyName + "`" : "`" + mappingInfo.DbColumnName + "`");
        }

        public override string GetTranslationColumnName(string propertyName)
        {
            if (propertyName.Contains("`")) return propertyName;
            else
                return "`" + propertyName + "`";
        }

        public override string GetNoTranslationColumnName(string name)
        {
            if (!name.Contains("`")) return name;
            return name == null ? string.Empty : Regex.Match(name, @"\`(.*?)\`").Groups[1].Value;
        }
    }
}
