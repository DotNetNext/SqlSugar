using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    internal class UnitDefaultValueTest
    {
        public static void Init() 
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<UnitXXXYYYZZZ>(); 
            db.DbMaintenance.TruncateTable<UnitXXXYYYZZZ>();
            db.Insertable(new UnitXXXYYYZZZ() { Id = "1" }).ExecuteCommand();
            db.CodeFirst.InitTables<unitxxxyyyzzz>();//用同名的新类添加一个字段
            var list=db.Queryable<unitxxxyyyzzz>().ToList();
            if (list.First().Id2 != "001") throw new Exception("unit error");
        }
    }
    public class UnitXXXYYYZZZ
    {
        public string Id { get; set; } 
    }
    public class unitxxxyyyzzz
    {
        public string Id { get; set; }
        [SqlSugar.SugarColumn(DefaultValue = "001")]
        public string Id2 { get; set; }
    }
}
