using System;
using System.Collections.Generic;
using System.Text;

namespace SqlSugar.MongoDb 
{
    public class MongoDbConfig
    {
        public static Func<string, bool> NoObjectIdFunc { get; set; }
    }
}
