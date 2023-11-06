using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    internal class _2_DbFirst
    {
        public static void Init() 
        {
            var db = DbHelper.GetNewDb(); 

            db.DbFirst.CreateClassFile("c:\\Demo\\1", "Models");

            db.DbFirst.IsCreateAttribute().CreateClassFile("c:\\Demo\\2", "Models");

            db.DbFirst.StringNullable().IsCreateAttribute().CreateClassFile("c:\\Demo\\3", "Models");

            db.DbFirst.FormatFileName(it => "Sys_"+it).CreateClassFile("c:\\Demo\\4", "Models"); 
        }
    }
}
