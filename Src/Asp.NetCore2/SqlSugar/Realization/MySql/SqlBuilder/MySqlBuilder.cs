using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace SqlSugar
{
    public class MySqlBuilder : SqlBuilderProvider
    {
        public override string SqlTranslationLeft { get { return "`"; } }
        public override string SqlTranslationRight { get { return "`"; } }
        public override string SqlDateNow
        {
            get
            {
                return "NOW(6)";
            }
        }
        public override string FullSqlDateNow
        {
            get
            {
                return "select NOW(6)";
            }
        }
        public override string GetUnionFomatSql(string sql)
        {
            return " ( " + sql + " )  ";
        }

        public override string RemoveParentheses(string sql)
        {
            if (sql.Contains(" ORDER BY")&&sql.StartsWith("(")&&sql.EndsWith(")") &&!sql.ToLower().Contains("limit")) 
            {
                sql = $" {sql.TrimEnd(')')} limit 0,{int.MaxValue}) ";
            }
            return sql;
        }
    }
}
