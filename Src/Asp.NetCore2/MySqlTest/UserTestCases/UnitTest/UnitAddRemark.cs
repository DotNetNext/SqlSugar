using OrmTest;
using System;
using System.Collections.Generic;
using System.Text;

namespace  SqlSugar
{
    internal class UnitAddRemark
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<UnitAddRemark2>();//添加表
            db.DbMaintenance.AddColumnRemark("id", "UnitAddRemark2", "AA");
            foreach (var item in db.DbMaintenance.GetColumnInfosByTableName("UnitAddRemark2",false))
            {
                Console.WriteLine($" {item.DbColumnName} {item.ColumnDescription} ");
            }
            db.DbMaintenance.AddColumnRemark("id", "UnitAddRemark2", "");
            foreach (var item in db.DbMaintenance.GetColumnInfosByTableName("UnitAddRemark2", false))
            {
                Console.WriteLine($" {item.DbColumnName} {item.ColumnDescription} ");
            }
            db.DbMaintenance.AddColumnRemark("id", "UnitAddRemark2", "AA");
            foreach (var item in db.DbMaintenance.GetColumnInfosByTableName("UnitAddRemark2", false))
            {
                Console.WriteLine($" {item.DbColumnName} {item.ColumnDescription} ");
            }
            db.DbMaintenance.DropTable<UnitAddRemark2>();//删除表
        }
        public class UnitAddRemark2 
        {
            public int id { get; set; }
            public string name { get; set; }
        }
    }
}
