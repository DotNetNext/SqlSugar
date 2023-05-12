using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar 
{
    public class StaticConfig
    {
        public static Func<string,string> Encode { get; set; }
        public static Func<string,string> Decode{ get; set; }
        public const string CodeFirst_BigString = "varcharmax,longtext,text,clob";
        public static Func<long> CustomSnowFlakeFunc;

        public static Action<object> CompleteQueryableFunc;
        public static Action<object> CompleteInsertableFunc;
        public static Action<object> CompleteUpdateableFunc;
        public static Action<object> CompleteDeleteableFunc;
        public static Action<ISqlSugarClient> CompleteDbFunc;
    }
}
