using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
namespace OrmTest
{
    public class UCustom22
    {

        public static void Init()
        {
            var db = NewUnitTest.Db;



            //建表 

            if (!db.DbMaintenance.IsAnyTable("UnitTest0011", false))

            {

                db.CodeFirst.InitTables<Test0011>();

            }

            db.Insertable(new List<Test0011>() { new Test0011 { id = 1, age = 1 }, new Test0011 { id = 2, age = 2 } }).ExecuteCommand();



            //用例代码        

            Search<Test0011>(db);
            db.CurrentConnectionConfig.MoreSettings = new ConnMoreSettings()
            {
                 IsWithNoLockQuery=true
            };
            db.Queryable<Test0011>()
                .Select<Test0011>()
                .MergeTable().LeftJoin<Test0011>((x, y) => true)
                .ToList();

        }



        public static List<T> Search<T>(SqlSugarClient db) where T : ISearch

        {

            var searchReq = new { age = 1 };

            return db.Queryable<T>().Where(u => u.age == searchReq.age).ToList();

        }



        //用例实体
        [SugarTable("UnitTest0011")]
        public class Test0011 : ISearch

        {

            public int id { get; set; }

            public int age { get; set; }

        }



        public interface ISearch

        {

            int age { get; set; }

        }
    }


}
