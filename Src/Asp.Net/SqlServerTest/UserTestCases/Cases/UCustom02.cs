using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
namespace OrmTest 
{
    [SugarTable("UnitTest0")]

    public class Test0

    {

        [SugarColumn(IsNullable = false, IsPrimaryKey = true)]

        public long Id { get; set; }



        public string Title { get; set; }

    }

    [SugarTable("UnitTest_1")]

    public class Test1

    {

        [SugarColumn(IsNullable = false, IsPrimaryKey = true)]

        public long Id { get; set; }



        [SugarColumn(ColumnName = "test0id")]

        public long Test0Id { get; set; }



        public string Title1 { get; set; }

    }



    [SugarTable("UnitTest_2")]

    public class Test2

    {

        [SugarColumn(IsNullable = false, IsPrimaryKey = true)]

        public long Id { get; set; }



        [SugarColumn(ColumnName = "test0i_d")]

        public long Test0Id { get; set; }



        public string Title2 { get; set; }

    }



    [SugarTable("UnitTest_3")]

    public class Test3

    {

        [SugarColumn(IsNullable = false, IsPrimaryKey = true)]

        public long Id { get; set; }



        [SugarColumn(ColumnName = "test0_id")]

        public long Test0Id { get; set; }



        public string Title3 { get; set; }

    }



    public class UCustom02

    {

        public static void Init()

        {


            var db = NewUnitTest.Db; ;
 

            //用来打印Sql方便你调式

            db.Aop.OnLogExecuting = (sql, pars) =>

            {

                Console.WriteLine(sql + "\r\n" +

                db.Utilities.SerializeObject(pars.ToDictionary(it => it.ParameterName, it => it.Value)));

                Console.WriteLine();

            };

            db.Aop.OnError = (sql) =>

            {

                string ss = sql.Sql;

            };

            db.Aop.OnLogExecuting = (sql, p) =>

            {

                Console.WriteLine(sql);

            };

            db.Aop.OnLogExecuted = (sql, p) =>

            {

                Console.WriteLine(sql);

            };



            db.CodeFirst.InitTables(typeof(Test0), typeof(Test1), typeof(Test2), typeof(Test3));



            db.Deleteable<Test0>().ExecuteCommand();

            db.Deleteable<Test1>().ExecuteCommand();

            db.Deleteable<Test2>().ExecuteCommand();

            db.Deleteable<Test3>().ExecuteCommand();

            db.Insertable(new Test0 { Id = 1, Title = "111111" }).ExecuteCommand();

            db.Insertable(new Test1 { Id = 1, Test0Id = 1, Title1 = "111111" }).ExecuteCommand();

            db.Insertable(new Test2 { Id = 1, Test0Id = 1, Title2 = "222222222" }).ExecuteCommand();

            db.Insertable(new Test3 { Id = 1, Test0Id = 1, Title3 = "333333333" }).ExecuteCommand();



            var cacheQueryT = db.Queryable<Test0>().Select(x => new

            {

                id = x.Id,

                title = x.Title,

                title1 = 
                SqlFunc.Subqueryable<Test1>().Where(w => w.Test0Id == x.Id && 
                SqlFunc.Subqueryable<Test2>().Where(t => t.Test0Id == w.Test0Id && 
                SqlFunc.Subqueryable<Test3>().Where(t1 => t1.Test0Id == w.Test0Id).Any()).Any()).Select(w => w.Title1)

            }).ToList();

        }

    }


}
