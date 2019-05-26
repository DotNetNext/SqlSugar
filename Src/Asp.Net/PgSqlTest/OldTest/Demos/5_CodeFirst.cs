using OrmTest.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrmTest.Demo
{
    [SugarTable("CodeTable", " table CodeTable")]
    public class CodeTable
    {
     

        [SugarColumn(IsNullable =false ,IsPrimaryKey =true,IsIdentity =true,ColumnDescription ="XXhaha primary key!!")]
        public int Id { get; set; }
        [SugarColumn(Length = 21,OldColumnName = "Name2")]
        public string Name{ get; set; }
        public string IsOk { get; set; }
        public Guid Guid { get; set; }
        [SugarColumn(ColumnDataType ="int")]
        public decimal Decimal { get; set; }
        [SugarColumn(IsNullable = true)]
        public DateTime? DateTime { get; set; }
        [SugarColumn(IsNullable = true,OldColumnName = "Dob")]
        public double? Dob2 { get; set; }
        [SugarColumn(Length =11000)]
        public string A1 { get; set; }
        [SugarColumn(Length = 18,DecimalDigits=2)]
        public decimal Dec { get; set; }
    }
    public class CodeTable2 {
        public int Id { get; set; }
        public string Name { get; set; }
        [SugarColumn(IsIgnore =true)]
        public string TestId { get; set; }
    }
    public class CodeFirst : DemoBase
    {
        public static void Init()
        {
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = Config.ConnectionString,
                DbType = DbType.PostgreSQL,
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.Attribute 
            });

            db.CodeFirst.SetStringDefaultLength(30).InitTables(typeof(Student), typeof(School));

            //Backup table
            //db.CodeFirst.BackupTable().InitTables(typeof(CodeTable),typeof(CodeTable2));

            //No backup table
             db.CodeFirst.SetStringDefaultLength(10).InitTables(typeof(CodeTable),typeof(CodeTable2));
        }
    }
}
