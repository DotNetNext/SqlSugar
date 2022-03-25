using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest 
{
    public partial class NewUnitTest
    {

        public static void Tran()
        {
            var db = Db;
            int oldCount = GetCount(db);
            Console.WriteLine(oldCount);
            db.BeginTran();
            db.Deleteable<Order>().ExecuteCommand();
            Console.WriteLine(GetCount(db));
            db.RollbackTran();
            int newCount = GetCount(db);
            Console.WriteLine(newCount);
            if (newCount != oldCount)
            {
                throw new Exception("NewUnitTest Tran ");
            }
        }

        private static int GetCount(SqlSugar.SqlSugarClient db)
        {
            return db.Queryable<Order>().Count();
        }
    }
}
