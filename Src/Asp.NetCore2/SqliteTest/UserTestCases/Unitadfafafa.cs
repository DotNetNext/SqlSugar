using System;
using System.Reflection;
using System.Threading.Tasks;
using SqliteTest.UnitTest;
using SqlSugar;
namespace OrmTest
{
   public class Unitadfafafa
    {
       public static void Init()
        {


            ISqlSugarClient db = new SqlSugarScope(new SqlSugar.ConnectionConfig()
            {
                ConnectionString = Config.ConnectionString,
                DbType = DbType.Sqlite,
                IsAutoCloseConnection = true,
                ConfigureExternalServices = new ConfigureExternalServices
                {
                    EntityService = (c, p) =>
                    {
                        if (new NullabilityInfoContext()
                        .Create(c).WriteState is NullabilityState.Nullable)
                        {
                            p.IsNullable = true;
                        }


                    }
                },

            }, db =>
            {

                //db.Aop.OnLogExecuting = (sql, pars) => //SQL执行前
                //{

                //    var str = UtilMethods.GetSqlString(DbType.MySql, sql, pars);
                //    Console.WriteLine("===");
                //    Console.WriteLine(str);
                //    Console.WriteLine("===");
                //};

            });
            db.CodeFirst.InitTables<Test003>();
            var res2 = db.Insertable<Test003>(new Test003() { OVOV = new ObjctValueTest { Code333 = "11", Code4444 = "333" } }).ExecuteCommand();
            var res = db.Queryable<Test003>().Any(x => true);
             

        }


        [SugarTable("Test003")]
        //建类
        public class Test003
        {
            [SugarColumn(IsPrimaryKey = true)]
            public Guid Id { get; set; } = Guid.NewGuid();

            public DateTime? CreateTime { get; set; } = DateTime.Now;


            public string? Code { get; set; }

            [SqlSugar.SugarColumn(IsOwnsOne = true)]
            public ObjctValueTest OVOV { get; set; }



        }

        public class ObjctValueTest
        {
            public string Code4444 { get; set; }

            public string Code333 { get; set; }

        }
    }
}