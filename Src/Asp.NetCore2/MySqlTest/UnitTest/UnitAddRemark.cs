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
            db.DbMaintenance.AddColumnRemark("id", "UnitAddRemark2", "");
            db.DbMaintenance.AddColumnRemark("id", "UnitAddRemark2", "AA");
            db.DbMaintenance.DropTable<UnitAddRemark2>();//删除表
        }
        public class UnitAddRemark2 
        {
            public int id { get; set; }
            public string name { get; set; }
        }
    }
}
