using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SqlSugar;

namespace OrmTest
{
    public partial class NewUnitTest
    {
        private static void Fastest2()
        {
            var db = new SqlSugarScope(new SqlSugar.ConnectionConfig()
            {
                ConnectionString =Config.ConnectionString,
                DbType = DbType.SqlServer,
                IsAutoCloseConnection = true
            });

            db.CodeFirst.InitTables<Test2>();
            db.DbMaintenance.TruncateTable<Test2>();
            //用例代码
            db.Insertable(new Test2() { p = "1" }).ExecuteCommand();//用例代码

            db.Insertable(new Test2() { p = "2", delPer = 1 }).ExecuteCommand();//用例代码

            var updateList = db.Queryable<Test2>()
                .ToList();

            db.Fastest<Test2>().BulkCopy(updateList);

            int index = 0;

            foreach (var update in updateList)
            {
                update.p = index.ToString();

                index++;
            }

            db.Fastest<Test2>().BulkUpdate(updateList);
            db.CodeFirst.InitTables<UnitTestoffset11>();
            db.Fastest<UnitTestoffset11>().BulkCopy(new List<UnitTestoffset11>() {
            new  UnitTestoffset11 { },
             new  UnitTestoffset11 {  DateTimeOffset= DateTimeOffset.Now}
            });
            var list = db.Queryable<UnitTestoffset11>().ToList();
            Console.WriteLine("用例跑完");
        }
        public class UnitTestoffset11 
        {
            [SugarColumn(IsNullable =true)]
            public DateTimeOffset? DateTimeOffset { get; set; }
        }

        [SugarTable("UnitFastest0011a")]
        public class Test2
        {
            [SugarColumn(IsNullable = false,IsIdentity =true, IsPrimaryKey = true)]
            public int id { get; set; }

            [SugarColumn(IsNullable = false)]
            public string p { get; set; }

            [SugarColumn(IsNullable = true)]
            public int? delPer { get; set; }

            [SugarColumn(IsNullable = true)]
            public DateTime? del_Time { get; set; }
        }
    }
}