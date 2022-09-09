using SqlSugar;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OrmTest
{
    public class DemoZ_Thread
    {

        public static void Init()
        {
            var cnt = 0;

            //Separate test , Automatically build table
            TestQuestDb(cnt);


          //   TestPgSql(cnt);

            Console.ReadLine();
        }
        public static object lockx = new object();
        private static int TestQuestDb(int cnt)
        {
            var questDb = GetDb();

            //CreateTable
            questDb.CodeFirst.InitTables<users>();


            //Begin test
            int errorIndex = 0;
            for (var i = 0; i < 50; i++)
            {
                var thread = new Thread(async s =>
                {
                    var sw = new Stopwatch();
                    sw.Start();
                    Random random = new Random();
                    while (sw.Elapsed < TimeSpan.FromSeconds(20))
                    {
                        var param = new { userid = random.Next(0, 10000000) };
                        var db = GetDb();
                        try
                        {


                            var result = await db.Ado.SqlQuerySingleAsync<string>("SELECT nickname from users where userid=@userid;", param);

                            cnt++;
                            Console.WriteLine($"结果:{Guid.NewGuid() + ""}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"#########################异常：{ex.Message}#############################");
                            errorIndex++;
                        }
                    }
                    sw.Stop();
                    Console.WriteLine($"执行次数：{cnt},错误次数{errorIndex}");
                });
                thread.Start(i);
            }

            return cnt;
        }
        private static int TestPgSql(int cnt)
        {
            var pgsql = GetPgSql();

            //CreateTable
            pgsql.CodeFirst.InitTables<users>();


            //Begin test
            int index = 1;
            for (var i = 0; i < 50; i++)
            {
                var thread = new Thread(async s =>
                {
                    var sw = new Stopwatch();
                    sw.Start();
                    Random random = new Random();
                    while (sw.Elapsed < TimeSpan.FromSeconds(10))
                    {
                        try
                        {
                            var param = new { userid = random.Next(0, 10000000) + "" };
                            index++;


                            var db = GetPgSql();

                            var result = await db.Ado.SqlQuerySingleAsync<string>("SELECT nickname from users where userid=@userid;", param);

                            Console.WriteLine($"结果:{Guid.NewGuid() + ""}");


                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"#########################异常：{ex.Message}#############################");
                        }
                        finally
                        {
                            Interlocked.Increment(ref cnt);
                        }
                    }

                    sw.Stop();
                    Console.WriteLine($"执行次数：{cnt}");
                });
                thread.Start(i);
            }

            return cnt;
        }

        private static SqlSugarClient GetDb()
        {
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                DbType = SqlSugar.DbType.QuestDB,
                ConnectionString = Config.ConnectionString,
                InitKeyType = InitKeyType.Attribute,
                IsAutoCloseConnection = true,
            });
            return db;
        }
        private static SqlSugarClient GetPgSql()
        {
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                DbType = SqlSugar.DbType.PostgreSQL,
                ConnectionString = "PORT=5432;DATABASE=SqlSugar4xTest;HOST=localhost;PASSWORD=haosql;USER ID=postgres",
                InitKeyType = InitKeyType.Attribute,
                IsAutoCloseConnection = true,
            });
            return db;
        }

        public class users
        {
            public string nickname { get; set; }
            public string userid { get; set; }
        }
    }
}
