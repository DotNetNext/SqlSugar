using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
                DbType = DbType.ClickHouse,
                ConnectionString = Config.ConnectionString3,
                InitKeyType = InitKeyType.Attribute,
                IsAutoCloseConnection = true
            },
            db => {
                db.Aop.OnLogExecuting = (s, p) => Console.WriteLine(s);
            });
            //db.DbMaintenance.CreateDatabase(); 
            db.CodeFirst.InitTables(typeof(CodeFirstTable1));//Create CodeFirstTable1 
            db.Insertable(new CodeFirstTable1() { Name = "a", Text = "a" }).ExecuteCommand();
            var list = db.Queryable<CodeFirstTable1>().ToList();
            TestBool(db);
            TestGuid(db);
            db.CodeFirst.InitTables<CodeFirstTable2>();
            db.Insertable(new List<CodeFirstTable2>() { new CodeFirstTable2() { CreateTime = DateTime.Now, Name = "a", Text = new ulong[] { } } }).ExecuteCommand();
            var list2 = db.Queryable<CodeFirstTable2>().ToList();

            db.CodeFirst.InitTables<CodeFirstTable3>();
            db.Insertable(
            new CodeFirstTable3() { CreateTime = DateTime.Now, Name = "123123123", Text = new string[0] }
             ).ExecuteCommand();

            db.Insertable(new List<CodeFirstTable3>()
            {
                new CodeFirstTable3() { CreateTime = DateTime.Now, Name = "add", Text = new string[] { "sdfsfd" } },
                new CodeFirstTable3() { CreateTime = DateTime.Now, Name = "addd", Text = new string[]{ "13123123" } },
            }).ExecuteCommand();
            var list3 = db.Queryable<CodeFirstTable3>().ToList();

            db.CodeFirst.InitTables<TestNull>();
            db.Insertable(new TestNull() { Id=1 }).ExecuteCommand();
            db.Insertable(new List<TestNull>() { new TestNull() { Id = 2}, new TestNull() { Id = 3 } }).ExecuteCommand();
            var list4=db.Queryable<TestNull>().ToList();
            db.Deleteable(list4).ExecuteCommand();
            Console.WriteLine("#### CodeFirst end ####");
        }
        public class TestNull 
        {
            [SugarColumn(IsPrimaryKey =true)]
            public long Id { get; set; }    
            public DateTime? DateTime { get; set; }
            [SugarColumn(IsNullable = true)]
            public Guid? UUID { get; set; }
        }
        private static void TestGuid(SqlSugarClient db)
        {
            db.CodeFirst.InitTables<GuidTest>();
            db.DbMaintenance.TruncateTable("BoolTest");
            var Id = 1;
            db.Insertable<GuidTest>(new GuidTest() { A = Guid.Empty, Id = Id }).ExecuteCommand();
            Console.Write(db.Queryable<GuidTest>().First().A);
            db.Updateable<GuidTest>(new GuidTest() { A = Guid.NewGuid(), Id = Id }).ExecuteCommand();
            Console.Write(db.Queryable<GuidTest>().First().A);
            db.CodeFirst.InitTables<GuidTest22>();
            db.Insertable(new GuidTest22() { }).ExecuteReturnSnowflakeId();
            var list=db.Queryable<GuidTest22>().ToList();
        }
        private static void TestBool(SqlSugarClient db)
        {
            db.CodeFirst.InitTables<BoolTest>();
            db.DbMaintenance.TruncateTable("BoolTest");
            var Id = 1;
            db.Insertable<BoolTest>(new BoolTest() { A = true, Id = Id }).ExecuteCommand();
            Console.Write(db.Queryable<BoolTest>().First().A);
            db.Updateable<BoolTest>(new BoolTest() { A = false, Id = Id }).ExecuteCommand();
            Console.Write(db.Queryable<BoolTest>().First().A);
            db.CodeFirst.InitTables<CKTest>();
        }
    }

    [SqlSugar.ClickHouse.CKTable(@"engine = MergeTree PARTITION BY toYYYYMM(dt)
        ORDER BY(toYYYYMM(dt))
        SETTINGS index_granularity = 8192;")]
    public class CKTest 
    {
        public string Id { get; set; }
        public DateTime dt { get; set; }
    }
    public class GuidTest22
    {
        [SugarColumn(IsPrimaryKey = true)]
        public long Id { get; set; }
        [SugarColumn(IsNullable = true)]
        public Guid? A { get; set; }
        [SugarColumn(IsNullable =true)]
        public DateTime? Date { get; set; }
    }
    public class GuidTest
    {
        [SugarColumn(IsPrimaryKey = true)]
        public long Id { get; set; }
        public Guid A { get; set; }
    }
    public class BoolTest 
    {
        [SugarColumn(IsPrimaryKey =true)]
        public long Id { get; set; }
        public bool A { get; set; }
    }
    public class CodeFirstTable2
    {
        [SugarColumn(IsPrimaryKey = true)]
        public int Id { get; set; }
        public string Name { get; set; }
        [SugarColumn(ColumnDataType = "Array(UInt64)",IsArray  =true)]//custom
        public UInt64[] Text { get; set; }
        [SugarColumn(IsNullable = true)]
        public DateTime CreateTime { get; set; }
    }
    public class CodeFirstTable3
    {
        [SugarColumn(IsPrimaryKey = true)]
        public int Id { get; set; }
        public string Name { get; set; }
        [SugarColumn(ColumnDataType = "Array(String)", IsArray = true)]//custom
        public string[] Text { get; set; }
        [SugarColumn(IsNullable = true)]
        public DateTime CreateTime { get; set; }
    }
    public class CodeFirstTable1
    {
        [SugarColumn(IsPrimaryKey = true)]
        public int Id { get; set; }
        public string Name { get; set; }
        [SugarColumn(ColumnDataType = "String")]//custom
        public string Text { get; set; }
        [SugarColumn(IsNullable = true)]
        public DateTime CreateTime { get; set; }
    }
}
