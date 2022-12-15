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
                DbType = DbType.Kdbndp,
                ConnectionString = Config.ConnectionString3,
                InitKeyType = InitKeyType.Attribute,
                IsAutoCloseConnection = true
            });
            db.DbMaintenance.CreateDatabase(); 
            db.CodeFirst.InitTables(typeof(CodeFirstTable1));//Create CodeFirstTable1 
            db.Insertable(new CodeFirstTable1() { Name = "a", Text="a" }).ExecuteCommand();
            var list = db.Queryable<CodeFirstTable1>().ToList();
            db.Aop.OnLogExecuting=(sql,p)=>Console.WriteLine(sql);
            db.CurrentConnectionConfig.MoreSettings = new ConnMoreSettings()
            {
                IsAutoToUpper = false
            };
            if (db.DbMaintenance.IsAnyTable("CodeFirstNoUpper4",false))
            {
                db.DbMaintenance.DropTable<CodeFirstNoUpper4>();
            }
            db.CodeFirst.InitTables<CodeFirstNoUpper4>();
            db.CodeFirst.InitTables<CodeFirstNoUpper4>();
            db.Insertable(new CodeFirstNoUpper4() { Id = Guid.NewGuid() + "", Name = "a" }).ExecuteCommand();
            var list2 = db.Queryable<CodeFirstNoUpper4>().Where(it => it.Id != null).ToList();
            db.Updateable(list2).ExecuteCommand();
            db.Deleteable(list2).ExecuteCommand();
            db.Updateable(list2.First()).ExecuteCommand();
            db.Deleteable<CodeFirstNoUpper4>().Where(it => it.Id != null).ExecuteCommand();
            db.Updateable<CodeFirstNoUpper4>().SetColumns(it => it.Name == "a").Where(it => it.Id != null).ExecuteCommand();
            db.Updateable<CodeFirstNoUpper4>().SetColumns(it => new CodeFirstNoUpper4()
            {
                Name = "a"
            }).Where(it => it.Id != null).ExecuteCommand();
            Console.WriteLine("#### CodeFirst end ####");
        }
    }
    public class CodeFirstNoUpper4
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string Id { get; set; }
        public string Name { get; set; }
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
