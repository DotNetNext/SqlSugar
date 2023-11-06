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
        }
    }
}
