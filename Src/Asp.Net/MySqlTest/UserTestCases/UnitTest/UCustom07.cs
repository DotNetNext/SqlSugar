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
            var x=db.Deleteable(new Date111() { dateTime = DateTime.Now,name="a" }).ToSql();
            if (x.Key.Contains("//") || x.Key.Contains("\\")) { throw new Exception("unit error"); }
            var x2 = db.Deleteable(new Date222() { dateTime = DateTime.Now, name = "a" }).ToSql();
            if (x2.Key.Contains("//") || x.Key.Contains("\\")) { throw new Exception("unit error"); }
            db.CodeFirst.InitTables<Date222, Date111, Date333>();
            db.Deleteable(new Date111() { dateTime = DateTime.Now, name = "a" }).ExecuteCommand();
            db.Deleteable(new Date222() { dateTime = DateTime.Now, name = "a" }).ExecuteCommand();
            db.Insertable(new Date333() { name1 = "a" }).ExecuteCommand();
            db.Updateable(new Date333() { name1 = "a" }).ExecuteCommand();
            db.DbMaintenance.DropTable("Date111");
            db.DbMaintenance.DropTable("Date222");
            db.DbMaintenance.DropTable("Date333");
        }
    }

    public class Date333
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true, ColumnName = "date Time")]
        public DateTime dateTime { get; set; }
        [SqlSugar.SugarColumn(  ColumnName = "name 1")]
        public string name1 { get; set; }
    }
    public class Date222
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public DateTime dateTime { get; set; }
 
        public string name { get; set; }
    }
    public class Date111
    {
        [SqlSugar.SugarColumn(IsPrimaryKey =true)]
        public DateTime dateTime { get; set; }
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string name { get; set; }
    }
}
