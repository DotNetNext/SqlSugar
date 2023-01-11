using System;
using System.Collections.Generic;
using System.Text;

namespace SqlSugar 
{
    public class StaticConfig
    {
        public static Func<string,string> Encode { get; set; }
        public static Func<string,string> Decode{ get; set; }
        public const string CodeFirst_BigString = "varcharmax,longtext,text,clob";
    }
}
