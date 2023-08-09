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
                DbType = DbType.GBase,
                ConnectionString = Config.ConnectionString3,
                InitKeyType = InitKeyType.Attribute,
                IsAutoCloseConnection = true
            });
            //db.DbMaintenance.CreateDatabase(); 
            db.CodeFirst.InitTables(typeof(CodeFirstTable1));//Create CodeFirstTable1 
            db.Insertable(new CodeFirstTable1() { Name = "a", Text="a" }).ExecuteCommand();
            var list = db.Queryable<CodeFirstTable1>().ToList();
            db.CodeFirst.InitTables<CodeFirstTable2>();
            db.DbMaintenance.TruncateTable<CodeFirstTable2>();
            db.Insertable(new CodeFirstTable2() { IsOk = true, Name = "a", Text = "a" }).ExecuteCommand();
            db.Insertable(new CodeFirstTable2() { IsOk = false, Name = "a", Text = "a" }).ExecuteCommand();
            var data= db.Queryable<CodeFirstTable2>().Where(it => it.IsOk==true).First();
            var data2 = db.Queryable<CodeFirstTable2>().Where(it => it.IsOk == false).First();
            db.Updateable(data).ExecuteCommand();
            Console.WriteLine("#### CodeFirst end ####");
        }
    }
    public class CodeFirstTable2
    {
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public int Id { get; set; }
        public string Name { get; set; }
        [SugarColumn(ColumnDataType = "varchar(255)")]//custom
        public string Text { get; set; }
        [SugarColumn(IsNullable = true)]
        public bool IsOk { get; set; }
    }
    public class CodeFirstTable1
    {
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public int Id { get; set; }
        public string Name { get; set; }
        [SugarColumn(ColumnDataType = "varchar(255)")]//custom
        public string Text { get; set; }
        [SugarColumn(IsNullable = true)]
        public DateTime CreateTime { get; set; }
    }
}
