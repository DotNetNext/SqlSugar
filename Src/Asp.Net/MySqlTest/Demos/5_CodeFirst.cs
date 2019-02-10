using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrmTest.Demo
{
    [SugarTable("CodeTable", " CodeTable test ")]
    public class CodeTable
    {
     
        [SugarColumn(IsNullable =false ,IsPrimaryKey =true,IsIdentity =true,ColumnDescription ="test")]
        public int Id { get; set; }
        [SugarColumn(Length = 21,OldColumnName = "Name2",ColumnDescription ="name")]
        public string Name{ get; set; }
        [SugarColumn(IsNullable = true,Length =11)]
        public string IsOk { get; set; }
        public Guid Guid { get; set; }
        public decimal Decimal { get; set; }
        [SugarColumn(IsNullable = true)]
        public DateTime? DateTime { get; set; }
        [SugarColumn(IsNullable = true,OldColumnName = "Dob")]
        public double? Dob2 { get; set; }
        [SugarColumn(Length =10)]
        public string A { get; set; }
    }
    public class CodeTable2 {
        public int Id { get; set; }
        public string Name { get; set; }
        [SugarColumn(IsIgnore =true)]
        public string TestId { get; set; }
    }
    public class CodeTable3
    {
        [SugarColumn(IsPrimaryKey = true)]
        public int Id { get; set; }
        [SugarColumn(IsPrimaryKey = true)]
        public string Name { get; set; }
        [SugarColumn(IsIgnore = true)]
        public string TestId { get; set; }
    }
    public class CodeFirst : DemoBase
    {
        public static void Init()
        {
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = Config.ConnectionString,
                DbType = DbType.MySql,
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.Attribute 
            });

            //Backup table
            //db.CodeFirst.BackupTable().InitTables(typeof(CodeTable),typeof(CodeTable2));

            //No backup table
            db.CodeFirst.InitTables(typeof(CodeTable),typeof(CodeTable2),typeof(CodeTable3));
        }
    }
}
