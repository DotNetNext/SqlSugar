using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SqlSugar;
namespace OrmTest
{
    public  class Unitdfassyss
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;
            //建表 
            db.CodeFirst.InitTables<NoKeyTable, MayHaveKeyTable>();
            //清空表
            db.DbMaintenance.TruncateTable<NoKeyTable>();
            var enableKey = false;
            var result = db.Queryable<NoKeyTable>()
                .Cast<MayHaveKeyTable>()
                .Select(t => new
                {
                    t.id,
    
                    key = enableKey ? t.key : null
                })
                .ToList();
            enableKey = true;
            var result2 = db.Queryable<MayHaveKeyTable>()
             .Cast<MayHaveKeyTable>()
             .Select(t => new
             {
                 t.id,

                 key = enableKey ? t.key : null
             })
             .ToList();

            Console.WriteLine(result);
            Console.WriteLine("用例跑完");
            Console.ReadKey();
        }
        //建类
        public class NoKeyTable
        {
            public int id { get; set; }
        }

        public class MayHaveKeyTable
        {
            public int id { get; set; }
            public string? key { get; set; }
        }
    }
}