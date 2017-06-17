using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrmTest.Demo
{
    public class CodeTable
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        [SugarColumn(Length = 100)]
        public string Name { get; set; }
        [SugarColumn(IsNullable = true)]
        public bool IsOk { get; set; }
        public Guid Guid { get; set; }
        public decimal Decimal { get; set; }
        [SugarColumn(IsNullable = true)]
        public DateTime? DateTime { get; set; }
        [SugarColumn(IsNullable = true)]
        public double? Dob { get; set; }
        public string A { get; set; }
    }
    public class CodeFirst : DemoBase
    {
        public static void Init()
        {
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = Config.ConnectionString,
                DbType = DbType.SqlServer,
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.Attribute
            });
            //if (db.DbMaintenance.IsAnyTable("CodeTable"))
            //    db.DbMaintenance.DropTable("CodeTable");
            //db.DbMaintenance.BackupDataBase("SqlSugar4XTest","c:\\back\\1.bak");
            db.CodeFirst.InitTables(typeof(CodeTable));
        }
    }
}
