using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public class Unitdfsdyss
    {
        public static void Init()
        {

            var db = NewUnitTest.Db;


            //建表 

            db.CodeFirst.InitTables<Test>();



            //建表 

            db.CodeFirst.InitTables<TestDtl>();

            db.Aop.OnLogExecuting = (s, p) =>
            {
                Console.WriteLine(s);
            };


            //清空表

            db.DbMaintenance.TruncateTable<Test>();

            db.DbMaintenance.TruncateTable<TestDtl>();



            var addList = new List<Test>()

            {

                new() { Id = 1,Name = "名1" },

                new() { Id = 2,Name = "名2"},

                new() { Id = 3,Name = "名3" }

            };



            var add2List = new List<TestDtl>()

            {

                new() { Id = 100, TestId = 1, Memo = "1001", OrderNo = 3 },

                new() { Id = 101, TestId = 1, Memo = "1001", OrderNo = 1 },

                new() { Id = 102, TestId = 1, Memo = "1002", OrderNo = 4 },

                new() { Id = 103, TestId = 1, Memo = "1003", OrderNo = 2 },

            };



            //插入测试数据

            db.Insertable(addList).ExecuteCommandAsync().GetAwaiter().GetResult();

            db.Insertable(add2List).ExecuteCommandAsync().GetAwaiter().GetResult();



            var result = db.Queryable<Test>() 
                .Select(x => new OutputResult()

                {

                    Id = x.Id,

                    TestDtlList = SqlFunc.Subqueryable<TestDtl>().Where(dtl => dtl.TestId == x.Id).OrderBy(dtl => dtl.OrderNo).OrderBy(dtl => dtl.Id).ToList()

                }, true)

                .ToListAsync().GetAwaiter().GetResult();


             if(result.First().TestDtlList.Min(it=>it.OrderNo)!= result.First().TestDtlList.First().OrderNo) 
            {
                throw new Exception("unit error");
            }

        }



        public class OutputResult : Test

        {

            public List<TestDtl> TestDtlList { get; set; } = new();

        }



        /// <summary>

        /// 测试表

        /// </summary>

        [SugarTable("Test1231311", "测试表")]

        public class Test

        {

            [SugarColumn(IsPrimaryKey = true)]

            public long Id { get; set; }





            [SugarColumn(Length = 50, IsNullable = true)]

            public string? Name { get; set; }

        }



        /// <summary>

        /// 测试表

        /// </summary>

        [SugarTable(null, "测试表")]

        public class TestDtl

        {

            [SugarColumn(IsPrimaryKey = true)]

            public long Id { get; set; }



            public long TestId { get; set; }





            [SugarColumn(Length = 50, IsNullable = true)]

            public string? Memo { get; set; }





            public int OrderNo { get; set; }

        }

    }
}
