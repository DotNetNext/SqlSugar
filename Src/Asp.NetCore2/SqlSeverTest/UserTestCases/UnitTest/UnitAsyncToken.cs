using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    internal class UnitAsyncToken
    {
        public static void Init() 
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Order>();
            if (db.Queryable<Order>().Count() < 10) 
            {
                for(int i = 0; i < 10; i++)
                {
                    db.Insertable(new Order() { Name = "a" + i , CreateTime=DateTime.Now }).ExecuteCommand();
                }
            }

            var x0 = db.Queryable<Order>().Take(10).ToList();

            var x1 = db.Queryable<Order>().Take(10).ToListAsync().GetAwaiter().GetResult();

            var x2 = db.Queryable<Order>().Take(10).ToListAsync(new System.Threading.CancellationToken()).GetAwaiter().GetResult();

            var r1 = db.Utilities.SerializeObject(x0);
            var r2 = db.Utilities.SerializeObject(x1);
            var r3 = db.Utilities.SerializeObject(x2);
            if (r1 !=r2 || r1!=r3)
            {
                throw new Exception("UnitAsyncToken");
            }

            db.CodeFirst.InitTables<StudentWithIdentityLong>();
            db.DbMaintenance.TruncateTable<StudentWithIdentityLong>();
            var big1=db.Insertable(new StudentWithIdentityLong()
            {
                Name = "a"
            }).ExecuteReturnBigIdentity();

            var big2 = db.Insertable(new StudentWithIdentityLong()
            {
                Name = "a"
            }).ExecuteReturnBigIdentityAsync().GetAwaiter().GetResult();

            if (big1 != 1 || big2 != 2) 
            {
                throw new Exception("unit error"); 
            }
        }

        [SugarTable("StudentWithIdentity08Long")]
        public class StudentWithIdentityLong
        {
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}
