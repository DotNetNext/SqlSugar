using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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

            try
            {
                Db.Ado.ExecuteCommand(@" create schema [user]");
            }
            catch
            {
            }
            try
            {
                Db.Ado.ExecuteCommand(@" create schema [ddd]");
            }
            catch
            {
            }
            db.CodeFirst.InitTables<UnitTableName>();
            db.CodeFirst.InitTables<UnitGe>();
            db.Insertable(new UnitGe() { geometry1 = "POINT (20 180)" }).ExecuteCommand();
            var gelist=db.Queryable<UnitGe>().Select(it=>new { geometry1 = it.geometry1.ToString()}).ToList();
            if (Db.DbMaintenance.IsAnyTable("User", false))
                Db.DbMaintenance.DropTable("User");
            db.CodeFirst.InitTables<User>();
            Db.CodeFirst.InitTables<UnitCodeFirstpks2>();
            if (Db.DbMaintenance.IsAnyTable("UnitCodeFirst131", false))
                Db.DbMaintenance.DropTable("UnitCodeFirst131");
            Db.CodeFirst.InitTables<UnitCodeFirst131>();
            Db.Insertable(new UnitCodeFirst131() { Id = 1 }).ExecuteCommand();
            Db.CodeFirst.InitTables<UNITCODEFIRST131>();
            var diffInfo = db.CodeFirst.GetDifferenceTables(typeof(UNITCOdEFIRST131)).ToDiffString();
            db.CodeFirst.InitTables<UNITCOdEFIRST131>();
            var diffInfo2 = db.CodeFirst.GetDifferenceTables(typeof(UNITCODEFIRST131)).ToDiffString();
            Db.CodeFirst.InitTables<UNITCOdEFIRST131>();
            Db.CodeFirst.InitTables<UnitTableUserName>();
            db.CodeFirst.InitTables<UnitTablename>();
            db.CodeFirst.InitTables<UnitXml>();
            db.Insertable(new UnitXml()
            {
                 name= XElement.Parse("<xml>aa</xml>")
            }).ExecuteCommand();
            var list= db.Queryable<UnitXml>().ToList();
            if (db.DbMaintenance.IsAnyTable("UnitDateTimeOffset", false))
            {
                db.DbMaintenance.DropTable("UnitDateTimeOffset");
            }
            db.CodeFirst.InitTables<UnitDateTimeOffset>();
            if (db.DbMaintenance.GetColumnInfosByTableName("UnitDateTimeOffset", false).First().DataType.ToLower() != "datetimeoffset") 
            {
                throw new Exception("unit error");
            }
        
        }

        public class UnitDateTimeOffset
        {
            public DateTimeOffset timeOffset { get; set; }
        }

        public class UnitXml 
        {
            [SugarColumn(ColumnDataType ="xml")]
            public XElement name { get; set; }
            [SugarColumn(IsNullable =true)]
            public string name2 { get; set; }
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
            [SqlSugar.SugarColumn(DefaultValue = "getdate()")]
            public DateTime dt { get; set; }
        }
        public class User 
        {
            [SugarColumn(IndexGroupNameList =new string[] { "index"})]
            public int key { get; set; }
            [SugarColumn(UniqueGroupNameList  = new string[] { "index" })]
            public int key2 { get; set; }
        }
        public class UnitGe 
        {
            [SugarColumn(ColumnDataType = "geometry")]
            public string geometry1 { get; set; }
        }

        [SugarTable("abp.UnitTableName","备注")]
        public class UnitTableName 
        {
            public string Id { get; set; }
        }
        [SugarTable("ddd.UnitTableName", "备注")]
        public class UnitTablename
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
        [SugarTable("user.UnitTableUserName", "备注")]
        public class UnitTableUserName
        {
            public string Id { get; set; }
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
