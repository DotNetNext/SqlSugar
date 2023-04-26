using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SqlSugar
{
    public class SqlServerBuilder : SqlBuilderProvider
    {
        public override string SqlTranslationLeft { get { return "["; } }
        public override string SqlTranslationRight { get { return "]"; } }

        public override string RemoveParentheses(string sql)
        {
            if (sql.Contains("ORDER BY")&&!sql.StartsWith("(SELECT TOP 1")) 
            {
                sql = $"SELECT * FROM {sql.Replace("(SELECT ", "(SELECT TOP 1000000")} TEMP";
            }
            else if (sql.Contains("ORDER BY") && sql.StartsWith("(SELECT TOP 1"))
            {
                sql = $"SELECT * FROM ({ sql}) TEMP";
            }
            return sql;
        }

    }
}
