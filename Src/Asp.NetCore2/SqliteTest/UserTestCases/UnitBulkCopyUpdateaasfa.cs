using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text; 
namespace OrmTest
{
    public class UnitBulkCopyUpdateaasfa
    {
        public static void Init()
        {
            var db = new SqlSugarScope(new SqlSugar.ConnectionConfig()
            {
                ConnectionString = @"DataSource=C:\sat_master.sqlite",
                DbType = DbType.Sqlite,
                IsAutoCloseConnection = true
            });

            //建表 
            if (!db.DbMaintenance.IsAnyTable("Test001", false))
            {
                db.CodeFirst.InitTables<Test001>();
            }
            if (!db.DbMaintenance.IsAnyTable("Test002", false))
            {
                db.CodeFirst.InitTables<Test002>();
            }
            db.DbMaintenance.TruncateTable<Test001, Test002>();

            var result = db.Insertable(new Test001() { id = 1, name = "1" }).ExecuteCommand();//用例代码

            var result2 = db.Insertable(new Test002() { id = 1, name = "1" }).ExecuteCommand();//用例代码

            List<Test001> list1 = new List<Test001>() { new Test001() { id = 1, name = "2" } };
            List<Test002> list2 = new List<Test002>() { new Test002() { id = 1, name = "2" } };

            db.BeginTran();

            db.Fastest<Test001>().AS("Test001").BulkUpdate(list1);
            db.Fastest<Test002>().AS("Test002").BulkUpdate(list2);

            var listx=db.Queryable<Test001>().ToList();
            if (listx.First().name != "2") 
            {
                throw new Exception("unit error");
            }
            var listy=db.Queryable<Test002>().ToList();
            if (listy.First().name != "2")
            {
                throw new Exception("unit error");
            }

            db.CommitTran();
            //用例代码 
             
         
        }
    }
    //建类
    public class Test001
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int id { get; set; }

        public string name { get; set; }
    }

    public class Test002
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int id { get; set; }

        public string name { get; set; }
    }
}
