using System;
using System.Collections.Generic;
using System.Text;

namespace SqlSugar.GBase
{
    public class GBaseConfig
    {
        public static string SqlTranslationLeft(SqlSugarProvider context)
        {
            return GBaseConfig.IsMySqlMode(context)
                ? "`" : "";
        }

        public static string SqlTranslationRight(SqlSugarProvider context)
        {
            return GBaseConfig.IsMySqlMode(context)
                ? "`" : "";
        }

        public static bool IsMySqlMode(SqlSugarProvider context)
        {
            return (context != null && 
                context.CurrentConnectionConfig.ConnectionString.Contains("sqlmode=mysql", StringComparison.CurrentCultureIgnoreCase));
        }
    }
}
