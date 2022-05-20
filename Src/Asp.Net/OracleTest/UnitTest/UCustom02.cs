using System;
using System.Collections.Generic;
using System.Text;
using SqlSugar;
namespace OrmTest
{
    public class UCustom02
    {
        public static void Init() 
        {
            var db = NewUnitTest.Db;
            //建表 
            if (!db.DbMaintenance.IsAnyTable("Test001", false))
            {
                db.CodeFirst.InitTables<UnitTest001>();
            }

            var dt = DateTime.Now;
            db.Aop.OnLogExecuting = (sql, pars) =>
            {
                var dt1 = pars[0];
            };
            //用例代码 
            var result = db.Insertable(new UnitTest001() { id = dt }).ExecuteCommand();//用例代码
            var res = db.Queryable<UnitTest001>().WhereClass(new UnitTest001() { id = dt }).ToList();
            if (res.Count == 0) 
            {
                throw new Exception("unit error");
            }

        }
        public class UnitTest001
        {
            [SugarColumn(ColumnDataType = "TIMESTAMP")]
            public DateTime id { get; set; }
        }
    }
}
