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
            Console.WriteLine(ErrorMessage.GetThrowMessage($"Create \"{entityType.Name}\" when inserting", $"会在Insert时创建表： \"{entityType.Name}\" ")); 
        }
    }
}
