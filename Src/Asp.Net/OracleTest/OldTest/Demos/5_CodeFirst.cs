using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrmTest.Demo
{
    public class CodeTable
    {

        [SugarColumn(IsPrimaryKey = true)]
        public int Id { get; set; }
        [SugarColumn(Length = 21, OldColumnName = "Name2", ColumnDescription = "haha")]
        public string Name { get; set; }
        [SugarColumn(IsNullable = true, Length = 10)]
        public string IsOk { get; set; }
        public Guid Guid { get; set; }
        [SugarColumn(ColumnDataType = "int")]
        public decimal Decimal { get; set; }
        [SugarColumn(IsNullable = true)]
        public DateTime? DateTime { get; set; }
        [SugarColumn(IsNullable = true, OldColumnName = "Dob")]
        public double? Dob2 { get; set; }
        [SugarColumn(Length = 110)]
        public string A2 { get; set; }
    }
    public class CodeTable2
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [SugarColumn(IsIgnore = true)]
        public string TestId { get; set; }
    }
    public class GuidTable
    {
        public Guid Name { get; set; }
        [SugarColumn(IsPrimaryKey = true)]
        public Guid Id { get; set; }
    }
    public class CodeFirst : DemoBase
    {
        public static void Init()
        {
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = Config.ConnectionString,
                DbType = DbType.Oracle,
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.Attribute
            });

            //Backup table
            //db.CodeFirst.BackupTable().InitTables(typeof(CodeTable),typeof(CodeTable2));

            //No backup table
            db.CodeFirst.InitTables(typeof(GuidTable), typeof(CodeTable), typeof(CodeTable2));
        }

    }
}