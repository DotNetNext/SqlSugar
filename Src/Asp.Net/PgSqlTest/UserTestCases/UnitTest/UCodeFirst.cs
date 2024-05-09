using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public partial class NewUnitTest
    {
        public static void CodeFirst()
        {
            if (Db.DbMaintenance.IsAnyTable("UnitCodeTest1", false))
                Db.DbMaintenance.DropTable("UnitCodeTest1");
            Db.CodeFirst.InitTables<UnitCodeTest1>();
            Db.CodeFirst.InitTables<UnitCodeTest22>();
            Db.Insertable(new UnitCodeTest22()).ExecuteCommand();
            Db.DbMaintenance.TruncateTable<UnitCodeTest22>();
            Db.CodeFirst.InitTables<UnitCodeTest3>();
            Db.Insertable(new UnitCodeTest22() {  id=1}).ExecuteCommand();
            Db.CodeFirst.InitTables<UnitTimeSpan2>();
            Db.Insertable(new UnitTimeSpan2()
            {
                 Id=new TimeSpan(),
                  id2=new TimeSpan(11,2,1)
            }).ExecuteCommand();
            var x= Db.Queryable<UnitTimeSpan2>().ToList();
            Db.CodeFirst.InitTables<UnitDateOfTime22>();
            Db.Insertable(new UnitDateOfTime22() { DateTimeOffset1 = DateTimeOffset.Now }).ExecuteCommand();
            Db.Insertable(new List<UnitDateOfTime22> { new UnitDateOfTime22() { DateTimeOffset1 = DateTimeOffset.Now }, new UnitDateOfTime22() { DateTimeOffset1 = DateTimeOffset.Now } }).ExecuteCommand();
            var list2 = Db.Queryable<UnitDateOfTime22>().ToList();

            Db.CodeFirst.InitTables<UnitTestPk>();
            Db.CodeFirst.InitTables<UnitTestPk>();
            Db.CodeFirst.InitTables<UnitTestPk>();

            Db.CodeFirst.InitTables<UnitLong1>();
            Db.Insertable(new List<UnitLong1>() { new UnitLong1() { Num = null }, new UnitLong1() { Num = null } }) 
                .UseParameter().ExecuteCommand();
            if (Db.DbMaintenance.IsAnyTable("User", false))
                Db.DbMaintenance.DropTable("User");
            Db.CodeFirst.InitTables<User>();
            if (Db.DbMaintenance.IsAnyTable("UnitCodeFirst131", false))
                Db.DbMaintenance.DropTable("UnitCodeFirst131");
            Db.CodeFirst.InitTables<UnitCodeFirst131>();
            Db.Insertable(new UnitCodeFirst131() { Id = 1 }).ExecuteCommand();
            Db.CodeFirst.InitTables<UNITCODEFIRST131>();
            Db.CodeFirst.InitTables<UNITCOdEFIRST131>();
            Db.CodeFirst.InitTables<TestTbl>();
            var db = Db;
            db.CurrentConnectionConfig.MoreSettings = new ConnMoreSettings()
            {
                PgSqlIsAutoToLower = false,
                 PgSqlIsAutoToLowerCodeFirst=false
            };
            db.CodeFirst.InitTables<UpperOrder>();
            var list=db.Queryable<UpperOrder, UpperOrder>((X1,Y1)=>new JoinQueryInfos(
                 JoinType.Left,X1.Id==Y1.Id
                ) ).ToList();

            var list3 = db.Queryable<UpperOrder, UpperOrder>((X1, Y1) => X1.Id == Y1.Id)
              .Select(X1 => X1).ToList();

            db.Insertable(new UpperOrder()
            {
                 CreateTime=DateTime.Now,
                  CustomId=0,
                   Name="a",
                    Price=1
            }).ExecuteReturnIdentity();

            db.Insertable(new UpperOrder()
            {
                CreateTime = DateTime.Now,
                CustomId = 0,
                Name = "a",
                Price = 1
            }).ExecuteReturnBigIdentity();

            db.Insertable(new List<UpperOrder>() { new UpperOrder()
            {
                CreateTime = DateTime.Now,
                CustomId = 0,
                Name = "a",
                Price = 1
            },new UpperOrder()
            {
                CreateTime = DateTime.Now,
                CustomId = 0,
                Name = "a",
                Price = 1
            } }).ExecuteReturnIdentity();
        }
        public class UpperOrder
        {
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int Id { get; set; }

            public string Name { get; set; }
            public decimal Price { get; set; }
            [SugarColumn(IsNullable = true)]
            public DateTime CreateTime { get; set; }
            [SugarColumn(IsNullable = true)]
            public int CustomId { get; set; }
            [SugarColumn(IsIgnore = true)]
            public List<OrderItem> Items { get; set; }
        }

        [SugarTable("test_tbl")]
        public class TestTbl
        {
            [SugarColumn(ColumnName = "id", IsPrimaryKey = true, IsIdentity = true)]
            public long Id { get; set; }

            [SugarColumn(ColumnName = "birthday", IsNullable = true, DefaultValue = "1900-01-01")]
            public DateTime Birthday { get; set; }
        }
        public class UnitCodeFirst131
        {
            public int Id { get; set; }
        }
        public class UNITCODEFIRST131
        {
            public int Id { get; set; }
            [SqlSugar.SugarColumn(DefaultValue = "a")]
            public string Name { get; set; }
        }
        public class UNITCOdEFIRST131
        {
            public int Id { get; set; }
            [SqlSugar.SugarColumn(DefaultValue = "a")]
            public string Name { get; set; }
            [SqlSugar.SugarColumn(DefaultValue = "0")]
            public int dt { get; set; }
        }
        public class User
        {
            [SugarColumn(IndexGroupNameList = new string[] { "index" })]
            public int key { get; set; }
            [SugarColumn(UniqueGroupNameList = new string[] { "index" })]
            public int key2 { get; set; }
        }
        public class UnitLong1
        {
            [SqlSugar.SugarColumn(IsNullable =true)]
            public long? Num { get; set; }
        }
        public class UnitTestPk 
        {
            [SqlSugar.SugarColumn(IsPrimaryKey =true,IsIdentity =true)]
            public int id { get; set; }
            [SqlSugar.SugarColumn(IsPrimaryKey = true )]
            public long id2 { get; set; }
            [SqlSugar.SugarColumn(IsPrimaryKey = true )]
            public Guid id3 { get; set; }

            public string Name { get; set; }
        
        }

        [SqlSugar.SugarTable("UnitDateOfTime221")]
        public class UnitDateOfTime22
        {
            public DateTimeOffset DateTimeOffset1 { get; set; }
        }
        public class UnitTimeSpan2
        {
            [SqlSugar.SugarColumn(ColumnDataType ="time")]
            public TimeSpan Id { get; set; }
            public TimeSpan id2 { get; set; }
        }
        public class UnitCodeTest22 {
             [SqlSugar.SugarColumn(IsNullable =true)]
             public decimal? id { get; set; }
        }
        [SqlSugar.SugarTable("UnitCodeTest22")]
        public class UnitCodeTest3
        {
            [SqlSugar.SugarColumn(IsNullable = false)]
            public int id { get; set; }
        }

        public class UnitCodeTest1
        {
            [SqlSugar.SugarColumn(IndexGroupNameList = new string[] { "group1" })]
            public int Id { get; set; }
            [SqlSugar.SugarColumn(DefaultValue= "now()", IndexGroupNameList =new string[] {"group1" } )]
            public DateTime? CreateDate { get; set; }
        }
    }
}
