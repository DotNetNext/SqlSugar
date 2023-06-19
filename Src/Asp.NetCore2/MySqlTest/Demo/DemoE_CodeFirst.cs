using Newtonsoft.Json.Linq;
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
                DbType = DbType.MySql,
                ConnectionString = Config.ConnectionString3,
                InitKeyType = InitKeyType.Attribute,
                IsAutoCloseConnection = true
            });
            db.Aop.OnLogExecuting = (s, p) => Console.WriteLine(UtilMethods.GetNativeSql(s, p));
            db.DbMaintenance.CreateDatabase(); 
            db.CodeFirst.InitTables(typeof(CodeFirstTable1));//Create CodeFirstTable1 
            db.Insertable(new CodeFirstTable1() { Name = "a", Text="a" }).ExecuteCommand();
            db.CodeFirst.InitTables<CharTest>();
            db.Insertable(new CharTest() { xx = Guid.NewGuid().ToString() }).ExecuteCommand();
            var list=db.Queryable<CharTest>().ToList();
            db.CodeFirst.InitTables<Charafafa2>();
            db.Insertable(new Charafafa2() { TimeSpanTimeSpan = TimeSpan.FromSeconds(1) }).ExecuteCommand();
            var list2=db.Queryable<Charafafa2>().ToList();
            db.CodeFirst.InitTables<Codeafadfa>();
            db.Insertable(new Codeafadfa() { Array = JArray.FromObject(new string[] { "a" }) }).ExecuteCommand();
            Console.WriteLine("#### CodeFirst end ####");
        }
    }
    public class Codeafadfa 
    {
        [SugarColumn(ColumnDataType ="varchar(500)" )]
        public object Array { get; set; } 
    }
    public class Charafafa2 
    {
        public TimeSpan TimeSpanTimeSpan { get; set; }
    }
    public class CharTest 
    {
        [SugarColumn(ColumnDataType ="char(36)")]
        public string xx { get; set; }
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
