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
            Db.CodeFirst.InitTables<UnitCodeFirstpks2>();
            if (Db.DbMaintenance.IsAnyTable("UnitCodeTest1", false))
                Db.DbMaintenance.DropTable("UnitCodeTest1");
            Db.CodeFirst.InitTables<UnitCodeTest1>();
            Db.CodeFirst.InitTables<UnitCodeFirstpks2>();
            var db = Db;
            db.Aop.OnLogExecuting = (s, p) =>
            {
                Console.WriteLine(s);
            };
            Db.CodeFirst.InitTables<UnitCodeFirstpks3>();
            Db.CodeFirst.InitTables<UnitCodeFirstpks32>();
            db.CodeFirst.InitTables<UnitTest0122132>();
            Db.CodeFirst.InitTables<UnitTimeSpan2>();
            Db.Insertable(new UnitTimeSpan2()
            {
                Id = new TimeSpan(),
                id2 = new TimeSpan(11, 2, 1)
            }).ExecuteCommand();
            var x = Db.Queryable<UnitTimeSpan2>().ToList();
            Db.CodeFirst.InitTables<UnitDateOfTime2>();

            Db.Insertable(new UnitDateOfTime2() { DateTimeOffset1 = DateTimeOffset.Now }).ExecuteCommand();
            Db.Insertable(new List<UnitDateOfTime2> { new UnitDateOfTime2() { DateTimeOffset1 = DateTimeOffset.Now }, new UnitDateOfTime2() { DateTimeOffset1 = DateTimeOffset.Now } }).ExecuteCommand();
            var list2 = Db.Queryable<UnitDateOfTime2>().ToList();

            try
            {
                Db.Ado.ExecuteCommand(@" create schema abp");
            }
            catch  
            { 
            }
            db.CodeFirst.InitTables<UnitTableName>();

        }
        [SugarTable("abp.UnitTableName","备注")]
        public class UnitTableName 
        {
            public string Id { get; set; }
        }

        public class UnitDateOfTime2
        {
          
            public DateTimeOffset DateTimeOffset1 { get; set; }
        }
        public class UnitTimeSpan2
        {
            [SqlSugar.SugarColumn(ColumnDataType = "time")]
            public TimeSpan Id { get; set; }
            public TimeSpan id2 { get; set; }
        }
        public class UnitTest0122132
        {

            [SugarColumn(ColumnDataType = "image,longblob")]
            public byte[] x { get; set; }
        }
        [SqlSugar.SugarTable("[dbo].[UnitCodeFirstpks3aaa122]")]
        public class UnitCodeFirstpks32
        {
            public int id { get; set; }
            public string name2 { get; set; }



        }
        [SqlSugar.SugarTable("UnitCodeFirstpks31","备注"  )]
        public class UnitCodeFirstpks3
        {
            public int id { get; set; }
            public string name2 { get; set; }


 
        }
        public class UnitCodeFirstpks2
        {
            [SqlSugar.SugarColumn(IsPrimaryKey =true)]
            public string id { get; set; }
            [SqlSugar.SugarColumn(IsPrimaryKey = true)]
            public string id2 { get; set; }
        }
        public class UnitCodeTest1
        {
            [SqlSugar.SugarColumn(IndexGroupNameList = new string[] { "group1" })]
            public int Id { get; set; }
            [SqlSugar.SugarColumn(DefaultValue="getdate()", IndexGroupNameList =new string[] {"group1" } )]
            public DateTime? CreateDate { get; set; }
        }
    }
}
