using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SqlSugar.MongoDb
{
    public class MongoDbCodeFirst : CodeFirstProvider
    {
        public override void InitTables(Type entityType)
        {
            if (this.Context.CurrentConnectionConfig.LanguageType == LanguageType.English)
                Console.WriteLine($"[MongoDbCodeFirst]: Create \"{entityType.Name}\" when inserting");
            else
                Console.WriteLine($"[MongoDbCodeFirst]: 会在Insert时创建表： \"{entityType.Name}\" ");
        }
    }
}
