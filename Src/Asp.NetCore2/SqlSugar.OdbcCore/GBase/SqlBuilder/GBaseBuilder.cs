using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SqlSugar.Odbc
{
    public class OdbcBuilder : SqlBuilderProvider
    {
        public override string SqlTranslationLeft { get { return OdbcConfig.SqlTranslationLeft; } }
        public override string SqlTranslationRight { get { return OdbcConfig.SqlTranslationRight; } }
        public override string GetNoTranslationColumnName(string name)
        {
            if (name.Contains("="))
            {
                name = name.Split('=').First();
            }
            name = name.Trim(' ');

            if (string.IsNullOrEmpty(SqlTranslationLeft) ||
                !name.Contains(SqlTranslationLeft))
            {
                return name;
            }

            if (!name.Contains(".") && name.StartsWith(SqlTranslationLeft) && name.EndsWith(SqlTranslationRight))
            {
                return name.TrimStart(Convert.ToChar(SqlTranslationLeft)).TrimEnd(Convert.ToChar(SqlTranslationRight));
            }

            return name == null ? string.Empty : Regex.Match(name, @".*" + "\\" + SqlTranslationLeft + "(.*?)" + "\\" + SqlTranslationRight + "").Groups[1].Value;
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
            if (string.IsNullOrEmpty(SqlTranslationLeft)&&name.IsContainsIn("(", ")", SqlTranslationLeft))
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
