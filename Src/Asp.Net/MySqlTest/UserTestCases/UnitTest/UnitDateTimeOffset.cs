using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    internal class UnitDateTimeOffset
    {
        public static void Init() 
        {
            var db = NewUnitTest.Db;

            db.DbMaintenance.CreateDatabase();
            //建表 

            db.CodeFirst.InitTables<Test>();

            SnowFlakeSingle.WorkId = 1;

            var test = new Test() { Name = "A", Date = DateTimeOffset.Now };
 
            var id = db.Insertable(test).ExecuteReturnSnowflakeId();
            test = db.Queryable<Test>().First(v => v.Id == id);
      
            //Console.ReadKey();

    }

        [SugarTable("unittest121")]
        public class Test
        {

            public string Name { get; set; }
            [SugarColumn(IsPrimaryKey = true)]
            public long Id { get; set; }
            public DateTimeOffset Date { get; set; } = DateTimeOffset.MinValue;


        }
    }
}
