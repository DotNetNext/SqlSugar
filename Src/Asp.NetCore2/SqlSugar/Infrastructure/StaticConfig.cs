using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar 
{
    public class StaticConfig
    {
        public static bool EnableAot { get; set; }
        public static Func<string, string> Encode { get; set; }
        public static Func<string, string> Decode { get; set; }
        public static bool AppContext_ConvertInfinityDateTime { get; set; }

        public const string CodeFirst_BigString = "varcharmax,longtext,text,clob";
        public static string CodeFirst_MySqlCollate{get;set;}

        public static Func<long> CustomSnowFlakeFunc;
        public static Func<long> CustomSnowFlakeTimeErrorFunc;
        public static Func<Guid> CustomGuidFunc;

        public static Action<object> CompleteQueryableFunc;
        public static Action<object> CompleteInsertableFunc;
        public static Action<object> CompleteUpdateableFunc;
        public static Action<object> CompleteDeleteableFunc;
        public static Action<ISqlSugarClient> CompleteDbFunc;

        public static Func<List<SplitTableInfo>> SplitTableGetTablesFunc;

        public static bool Check_StringIdentity = true;
        public static bool EnableAllWhereIF = false;
        public static Func<string,string> Check_FieldFunc;
        public static Type DynamicExpressionParserType;
        public static object DynamicExpressionParsingConfig;
        public static Action<ICacheService, string> CacheRemoveByLikeStringFunc { get; set; }
    }
}
