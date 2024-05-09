using SqliteTest.UnitTest;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrmTest
{
    public class DemoE_CodeFirst
    {
        public static void Init()
        {
            Console.WriteLine("");
            Console.WriteLine("#### CodeFirst Start ####");
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                DbType = DbType.Sqlite,
                ConnectionString = Config.ConnectionString3,
                InitKeyType = InitKeyType.Attribute,
                IsAutoCloseConnection = true 
            });
            db.DbMaintenance.CreateDatabase(); 
            db.CodeFirst.InitTables(typeof(CodeFirstTable1));//Create CodeFirstTable1 
            db.Insertable(new CodeFirstTable1() { Name = "a", Text="a" }).ExecuteCommand();
            var list = db.Queryable<CodeFirstTable1>().ToList();
            db = NewUnitTest.Db;
            db.CurrentConnectionConfig.MoreSettings = new ConnMoreSettings()
            {
               SqliteCodeFirstEnableDefaultValue = true,
                SqliteCodeFirstEnableDescription=true
            };
            db.CodeFirst.InitTables<CodeFirstUnitafa12>();
            var xxx=db.DbMaintenance.GetColumnInfosByTableName("CodeFirstUnitafa12", false);
            Console.WriteLine("#### CodeFirst end ####");
        }
    }
    public class CodeFirstUnitafa12   
    {
        [SugarColumn(IsPrimaryKey =true,IsIdentity =true,ColumnDescription ="aa")]
        public int Id { get; set; }
        [SugarColumn(DefaultValue = "(strftime('%Y-%m-%d %H:%M:%S', 'now', 'localtime'))")]
        public DateTime Name { get; set; }
        [SugarColumn(DefaultValue = "1")]
        public string Name2 { get; set; }
    }
    public class CodeFirstTable1
    {
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public int Id { get; set; }
        public string Name { get; set; }
        [SugarColumn(ColumnDataType = "Nvarchar(255)")]//custom
        public string Text { get; set; }
        [SugarColumn(IsNullable = true)]
        public DateTime CreateTime { get; set; }
    }
}
