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
                DbType = DbType.Dm,
                ConnectionString = Config.ConnectionString3,
                InitKeyType = InitKeyType.Attribute,
                IsAutoCloseConnection = true,
                 AopEvents = new AopEvents
                  {
                      OnLogExecuting = (sql, p) =>
                      {
                          Console.WriteLine(sql);
                          Console.WriteLine(string.Join(",", p?.Select(it => it.ParameterName + ":" + it.Value)));
                      }
                  }
            });
            // db.DbMaintenance.CreateDatabase(); 
            if (db.DbMaintenance.IsAnyTable("CodeFirstTable1", false))
                db.DbMaintenance.DropTable("CodeFirstTable1");
            if (db.DbMaintenance.IsAnyTable("CodeFirstLong", false))
                db.DbMaintenance.DropTable("CodeFirstLong");
            db.CodeFirst.InitTables(typeof(CodeFirstTable1));//Create CodeFirstTable1 
            db.Insertable(new CodeFirstTable1() { Name = "a", Text="a" }).ExecuteCommand();
            var list = db.Queryable<CodeFirstTable1>().ToList();
            db.CodeFirst.InitTables<CodeFirstLong>();
            var tableInfo=db.DbMaintenance.GetColumnInfosByTableName("CodeFirstLong", false);
            Console.WriteLine("#### CodeFirst end ####");
        }
    }

    public class CodeFirstLong 
    {
        [SugarColumn(IsPrimaryKey =true,IsIdentity =true)]
        public long id { get; set; }
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
