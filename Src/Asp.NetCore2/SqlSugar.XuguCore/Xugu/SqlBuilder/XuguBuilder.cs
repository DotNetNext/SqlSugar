using System;
using System.Linq;

namespace SqlSugar.Xugu
{
    public class XuguBuilder : SqlBuilderProvider
    {
        public override string SqlParameterKeyWord => ":";
        public override string SqlTranslationLeft { get; }="\"";
        public override string SqlTranslationRight { get; }="\"";
        public override string GetNoTranslationColumnName(string name) => name;
        public override string SqlDateNow { get; } = "SYSDATE";
        public override string FullSqlDateNow { get; } = "SELECT SYSDATE FROM DUAL";
        public override string GetTranslationTableName(string name)
        {
            Check.ArgumentNullException(name, string.Format(ErrorMessage.ObjNotExist, "Table Name"));
            if (!name.Contains("<>f__AnonymousType") && name.IsContainsIn("(", ")") && name != "Dictionary`2") return name;
            if (Context.MappingTables == null) return name;
            var context = this.Context;
            var mappingInfo = context
                .MappingTables
                .FirstOrDefault(it => it.EntityName.Equals(name, StringComparison.CurrentCultureIgnoreCase));
            name = (mappingInfo == null ? name : mappingInfo.DbTableName);
            if (name.IsContainsIn("(", ")", SqlTranslationLeft)) return name;
            if (name.Contains(".")) return string.Join(".", name.Split('.').Select(it => SqlTranslationLeft + it + SqlTranslationRight));
            else return SqlTranslationLeft + name + SqlTranslationRight;
        }
    }
}
