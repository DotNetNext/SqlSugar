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
            Tran1();
            Tran2();
            Tran3().Wait();
            Tran4().Wait();
        }

        private static void Tran1()
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
        private static void Tran2()
        {
            var db = Db;
            int oldCount = GetCount(db);
            Console.WriteLine(oldCount);
            db.UseTran(()=>{

                db.Deleteable<Order>().ExecuteCommand();
                Console.WriteLine(GetCount(db));
                throw new Exception("");
            });
            int newCount = GetCount(db);
            Console.WriteLine(newCount);
            if (newCount != oldCount)
            {
                throw new Exception("NewUnitTest Tran ");
            }
        }
        private static async Task Tran3()
        {
            var db = Db;
            int oldCount = GetCount(db);
            Console.WriteLine(oldCount);
            db.BeginTran();
            await db.Deleteable<Order>().ExecuteCommandAsync();
            Console.WriteLine(GetCount(db));
            db.RollbackTran();
            int newCount = GetCount(db);
            Console.WriteLine(newCount);
            if (newCount != oldCount)
            {
                throw new Exception("NewUnitTest Tran ");
            }
        }
        private  static async Task Tran4()
        {
            var db = Db;
            int oldCount = GetCount(db);
            Console.WriteLine(oldCount);
            await db.UseTranAsync( async () => {

                await db.Deleteable<Order>().ExecuteCommandAsync();
                Console.WriteLine(GetCount(db));
                throw new Exception("");
            });
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
