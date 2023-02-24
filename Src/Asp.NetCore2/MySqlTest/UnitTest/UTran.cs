using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public class UTran
    {
        public static void Init()
        {
            AdoTran();
            Tran();
            RollTest().GetAwaiter().GetResult();
            Tran2();
            Console.ReadKey();
        }

        private static void AdoTran()
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<UTran121>();
            db.DbMaintenance.TruncateTable<UTran121>();
            db.Insertable(new UTran121()).ExecuteCommand();
            for (int i = 0; i < 1000; i++)
            {
                AdoTest(i).GetAwaiter().GetResult();
            }
            var id = db.Queryable<UTran121>().ToList().First().ID;
            if (id != 2000)
            {
                throw new Exception("unit error");
            }

        }

        public static async Task AdoTest(int i) 
        {
            var db = NewUnitTest.Db;
            await db.Ado.BeginTranAsync();

            await db.Updateable<UTran121>()
                .SetColumns(it => it.ID == it.ID+1)
                .Where(it=>true)
                .ExecuteCommandAsync();

            await db.Updateable<UTran121>()
         .SetColumns(it => it.ID == it.ID + 1)
                     .Where(it => true)
         .ExecuteCommandAsync();

            await db.Ado.CommitTranAsync();
            Console.WriteLine("ok"+i);
        }


        private static void Tran2()
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<UTran121>();
            db.DbMaintenance.TruncateTable<UTran121>();
            db.Insertable(new UTran121()).ExecuteCommand();
            for (int i = 0; i < 1000; i++)
            {
                Test(i);
            }
           
        }


        private static void Tran()
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<UTran121>();
            db.DbMaintenance.TruncateTable<UTran121>();
            db.Insertable(new UTran121()).ExecuteCommand();
            for (int i = 0; i < 1000; i++)
            {
                Test(i).GetAwaiter().GetResult();
            }
            var id = db.Queryable<UTran121>().ToList().First().ID;
            if (id != 2000)
            {
                throw new Exception("unit error");
            }
        }

        public static async Task Test(int i)
        {
            var db = NewUnitTest.Db;
            await db.BeginTranAsync();

            await db.Updateable<UTran121>()
                .SetColumns(it => it.ID == it.ID + 1)
                .Where(it => true)
                .ExecuteCommandAsync();

            await db.Updateable<UTran121>()
         .SetColumns(it => it.ID == it.ID + 1)
                     .Where(it => true)
         .ExecuteCommandAsync();

            await db.CommitTranAsync();
            Console.WriteLine("ok" + i);
        }

        public static async Task RollTest()
        {
            int i = 0;
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<UTran121>();
            db.DbMaintenance.TruncateTable<UTran121>();
            db.Insertable(new UTran121() {  ID=100}).ExecuteCommand();

            try
            {
                Console.WriteLine( db.Queryable<UTran121>().First().ID);

                await db.BeginTranAsync();

                await db.Updateable<UTran121>()
                    .SetColumns(it => it.ID == it.ID + 1)
                    .Where(it => true)
                    .ExecuteCommandAsync();

                Console.WriteLine(db.Queryable<UTran121>().First().ID);

                throw new Exception("");

                await db.CommitTranAsync();
            }
            catch (Exception ex)
            {
                await db.RollbackTranAsync();
                if (db.Queryable<UTran121>().First().ID != 100) 
                {
                    throw new Exception("unit error");
                }
            }
            Console.WriteLine("ok" + i);
        }
        public class UTran121 
        {
            public int ID { get; set; }
        }
    }
}
