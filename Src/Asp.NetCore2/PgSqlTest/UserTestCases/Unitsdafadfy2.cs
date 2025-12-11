using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using DbType = SqlSugar.DbType;
namespace OrmTest
{
   public class Unitdfasfafay
    {
       public  static void Init()
        {
            var db = NewUnitTest.Db;

            //建表 
            db.CodeFirst.InitTables<TestA>();
            db.CodeFirst.InitTables<TestB>();
            //清空表
            db.DbMaintenance.TruncateTable<TestA>();
            db.DbMaintenance.TruncateTable<TestB>();


            //插入测试数据
            db.Insertable(new TestA() { IdA = "111", StartTime = Convert.ToDateTime("2023/3/30 7:03:40") }).ExecuteCommand();//用例代码
            db.Insertable(new TestA() { IdA = "222", StartTime = Convert.ToDateTime("2023/3/30 9:03:40") }).ExecuteCommand();//用例代码

            db.Insertable(new TestB() { IdB = "B11", IdA = "111", StartTime = Convert.ToDateTime("2023/3/30 7:03:40") }).ExecuteCommand();//用例代码
            db.Insertable(new TestB() { IdB = "B22", IdA = "222", StartTime = Convert.ToDateTime("2023/3/30 9:03:40") }).ExecuteCommand();//用例代码
            var Ids = new List<string>() { "111" };
            var lotMetaList = db.Queryable<TestA>()
                      .Includes(lm => lm.TB)
                      .Where(lm => lm.IdA == Ids.FirstOrDefault())
                      .ToList();

            db.CodeFirst.InitTables<TestC>();
            db.Insertable(new TestC()).ExecuteCommand();
             
        }
        //建类
        [SqlSugar.SugarTable("UnitdsfafasTestA")]
        public class TestA
        {
            [DataMember]
            [SugarColumn(IsPrimaryKey = true, Length = 36, SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string IdA { get; set; }

            [DataMember]
            [SugarColumn(IsNullable = false)]
            public DateTime StartTime { get; set; }

            [SqlSugar.SugarColumn(IsIgnore = true)]
            [Navigate(NavigateType.Dynamic, "[{m:\"IdA\",c:\"IdA\"},{m:\"StartTime\",c:\"StartTime\"}]")]
            public TestB TB { get; set; }
        }
        [SqlSugar.SugarTable("UnitdsfafasTestB")]
        public class TestB
        {
            public string IdB { get; set; }
            public string IdA { get; set; }

            [DataMember]
            [SugarColumn(IsNullable = false)]
            public DateTime StartTime { get; set; }
        }
        [SqlSugar.SugarTable("UnitdsfafasTestC")]
        public class TestC
        {
            [SugarColumn(IsNullable =true)]
            public TimeOnly? StartTime { get; set; }
        }
    }
}