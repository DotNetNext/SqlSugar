using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrmTest.Demo
{
    public class CodeTable
    {
     
        [SugarColumn(IsNullable =false ,IsPrimaryKey =true,IsIdentity =true)]
        public int Id { get; set; }
        [SugarColumn(Length = 21,OldColumnName = "Name2")]
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
        public string A1 { get; set; }
    }
    public class CodeTable2 {
        [SugarColumn(IsPrimaryKey =true,IsIdentity =true)]
        public int Id { get; set; }
        public string Name { get; set; }
        [SugarColumn(IsIgnore =true)]
        public string TestId { get; set; }
        public string Test { get; set; }
        public int Test22222x2 { get; set; }
    }
    public class CodeFirst : DemoBase
    {
        public static void Init()
        {
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = Config.ConnectionString,
                DbType = DbType.Sqlite,
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.Attribute 
            });

            //Backup table
            //db.CodeFirst.BackupTable().InitTables(typeof(CodeTable),typeof(CodeTable2));

            //No backup table
            db.CodeFirst.BackupTable().InitTables(typeof(CodeTable),typeof(CodeTable2));
        }
    }
}
