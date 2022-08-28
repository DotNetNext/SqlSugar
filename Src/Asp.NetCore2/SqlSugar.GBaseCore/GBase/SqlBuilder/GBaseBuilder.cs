using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SqlSugar.GBase
{
    public class GBaseBuilder : SqlBuilderProvider
    {
        public override string SqlTranslationLeft { get { return ""; } }
        public override string SqlTranslationRight { get { return ""; } }
        public override string GetNoTranslationColumnName(string name)
        {
            return name;
        }
        public override string SqlDateNow
        {
            get
            {
                return "sysdate";
            }
        }
        public override string FullSqlDateNow
        {
            get
            {
                return "select sysdate from dual";
            }
        }
        public override string GetTranslationTableName(string name)
        {
            Check.ArgumentNullException(name, string.Format(ErrorMessage.ObjNotExist, "Table Name"));
            if (!name.Contains("<>f__AnonymousType") && name.IsContainsIn("(", ")") && name != "Dictionary`2")
            {
                return name;
            }
            if (Context.MappingTables == null)
            {
                return name;
            }
            var context = this.Context;
            var mappingInfo = context
                .MappingTables
                .FirstOrDefault(it => it.EntityName.Equals(name, StringComparison.CurrentCultureIgnoreCase));
            name = (mappingInfo == null ? name : mappingInfo.DbTableName);
            if (name.IsContainsIn("(", ")", SqlTranslationLeft))
            {
                return name;
            }
            if (name.Contains("."))
            {
                return string.Join(".", name.Split('.').Select(it => SqlTranslationLeft + it + SqlTranslationRight));
            }
            else
            {
                return SqlTranslationLeft + name + SqlTranslationRight;
            }
        }

    }

}
