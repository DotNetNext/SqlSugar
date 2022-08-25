using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest 
{
    public class UCustom07
    {
        public static void Init() 
        {
            var db = NewUnitTest.Db;
            var x=db.Deleteable(new Date1() { dateTime = DateTime.Now,name="a" }).ToSql();
            if (x.Key.Contains("//") || x.Key.Contains("\\")) { throw new Exception("unit error"); }
            var x2 = db.Deleteable(new Date2() { dateTime = DateTime.Now, name = "a" }).ToSql();
            if (x2.Key.Contains("//") || x.Key.Contains("\\")) { throw new Exception("unit error"); }
            db.CodeFirst.InitTables<Date2, Date1, Date3>();
            db.Deleteable(new Date1() { dateTime = DateTime.Now, name = "a" }).ExecuteCommand();
            db.Deleteable(new Date2() { dateTime = DateTime.Now, name = "a" }).ExecuteCommand();
            db.Insertable(new Date3() { name1 = "a" }).ExecuteCommand();
            db.Updateable(new Date3() { name1 = "a" }).ExecuteCommand();
            db.DbMaintenance.DropTable("Date1");
            db.DbMaintenance.DropTable("Date2");
            db.DbMaintenance.DropTable("Date3");
        }
    }

    public class Date3
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true, ColumnName = "date Time")]
        public DateTime dateTime { get; set; }
        [SqlSugar.SugarColumn(  ColumnName = "name 1")]
        public string name1 { get; set; }
    }
    public class Date2
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public DateTime dateTime { get; set; }
 
        public string name { get; set; }
    }
    public class Date1 
    {
        [SqlSugar.SugarColumn(IsPrimaryKey =true)]
        public DateTime dateTime { get; set; }
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string name { get; set; }
    }
}
