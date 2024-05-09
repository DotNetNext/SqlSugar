using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static OrmTest.DemoE_CodeFirst;

namespace OrmTest
{
    public class DemoE_CodeFirst
    {
        public static void Init()
        {
            Console.WriteLine("");
            Console.WriteLine("#### CodeFirst Start ####");
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                DbType = DbType.QuestDB,
                ConnectionString = Config.ConnectionString3,
                InitKeyType = InitKeyType.Attribute,
                IsAutoCloseConnection = true
            });
            db.Aop.OnLogExecuting = (s, p) =>
            {
                Console.WriteLine(s);
            };
            db.CodeFirst.InitTables<QuestdbTestData3>();
            db.DbMaintenance.CreateDatabase();
            db.CodeFirst.InitTables(typeof(CodeFirstTable1));//Create CodeFirstTable1 
            db.Insertable(new CodeFirstTable1() { Name = "a", Text = "a" }).ExecuteCommand();
            var list = db.Queryable<CodeFirstTable1>().ToList();
            db.CodeFirst.InitTables<IndexClass>();
            db.CodeFirst.InitTables<SplitTableEntity>();
            TestBool(db);
            TestGuid(db);
            Console.WriteLine("#### CodeFirst end ####");
        }
        [SugarTable("QuestdbTestData3")]

        public class QuestdbTestData3

        {

            //不支持自增和主键 （标识主键是用来更新用的）

            [SugarColumn(IsPrimaryKey = true)]

            public string Id { get; set; }
            [SugarColumn(IsJson = true)]

            public List<QuestdbTestData> Data { get; set; }



            [TimeDbSplitField(DateType.Year)]

            public DateTime Time1 { get; set; }

        }
        public class QuestdbTestData
        {
            public int xx { get; set; }
        }
        private static void TestGuid(SqlSugarClient db)
        {
            db.CodeFirst.InitTables<GuidTest>();
            //db.DbMaintenance.TruncateTable("BoolTest");
            var Id =SnowFlakeSingle.Instance.NextId();
            db.Insertable<GuidTest>(new GuidTest() { A = Guid.Empty, Id = Id }).ExecuteCommand();
            Console.Write(db.Queryable<GuidTest>().First().A);
            db.Updateable<GuidTest>(new GuidTest() { A = Guid.NewGuid(), Id = Id }).ExecuteCommand();
            Console.Write(db.Queryable<GuidTest>().First().A);
        }
        private static void TestBool(SqlSugarClient db)
        {
            db.CodeFirst.InitTables<BoolTest5>();
            //db.DbMaintenance.TruncateTable("BoolTest3");
            var Id = SnowFlakeSingle.Instance.NextId();
            db.Insertable<BoolTest5>(new List<BoolTest5>(){ new BoolTest5() {  dateTime=DateTime.Now,A = true, Id = Id } }).ExecuteCommand();
            Console.Write(db.Queryable<BoolTest5>().First(it=>it.Id==Id).A);
            //db.Updateable<BoolTest4>(new BoolTest4() { dateTime = DateTime.Now,A = false, Id = Id }).ExecuteCommand();
            db.Updateable<BoolTest5>(  new BoolTest5() { dateTime = DateTime.Now, A = false, Id = Id  }).ExecuteCommand();
            db.CodeFirst.InitTables<UnitSiafayyy>();
            var list=db.Queryable<UnitSiafayyy>().SampleBy(1, SampleByUnit.Minute)
                .Select(it=>new { 
                  Id=SqlFunc.AggregateMin(it.Id),
                  Count=SqlFunc.AggregateCount(it.Id)
                })
                .ToList();
            Console.Write(db.Queryable<BoolTest5>().First(it => it.Id == Id).A);
        }
    }
    public class UnitSiafayyy
    {
        [SugarColumn(IsPrimaryKey = true)]
        public long Id { get; set; }
        [TimeDbSplitField(DateType.Day)]
        public DateTime DateTime { get; set; }
    }
    public class GuidTest
    {
        [SugarColumn(IsPrimaryKey = true)]
        public long Id { get; set; }
        public Guid A { get; set; }
    }
    public class BoolTest5
    {
        [SugarColumn(IsPrimaryKey = true)]
        public long Id { get; set; }
        public bool A { get; set; }
        public DateTime dateTime { get; set; }
    }

    [SugarIndex(null, nameof(IndexClass.Name), OrderByType.Asc)]
    public class IndexClass
    {
        public int Id { get; set; }
        [SugarColumn(ColumnDataType = "symbol")]
        public string Name { get; set; }
    }



    public class SplitTableEntity
    {
        public string Id { get; set; }
        [TimeDbSplitField(DateType.Day)]
        public DateTime Ts { get; set; }
    }


    public class CodeFirstTable1
    {
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public int Id { get; set; }
        public string Name { get; set; }
        [SugarColumn(ColumnDataType = "string")]//custom
        public string Text { get; set; }
        [SugarColumn(IsNullable = true)]
        public DateTime CreateTime { get; set; }
    }
}
