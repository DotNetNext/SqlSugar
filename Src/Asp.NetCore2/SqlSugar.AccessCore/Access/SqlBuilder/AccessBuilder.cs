using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SqlSugar.Access
{
    public class AccessBuilder : SqlBuilderProvider
    {
        public override string SqlTranslationLeft { get { return "["; } }
        public override string SqlTranslationRight { get { return "]"; } }
        public override string SqlDateNow { get { return " NOW()"; } }
        public override string FullSqlDateNow { get { return "SELECT NOW()"; } }
    }
}
