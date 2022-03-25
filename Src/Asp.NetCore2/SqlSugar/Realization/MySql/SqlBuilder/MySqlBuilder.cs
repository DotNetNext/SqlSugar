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
                return "sysdate()";
            }
        }
        public override string FullSqlDateNow
        {
            get
            {
                return "select sysdate()";
            }
        }
    }
}
