using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrmTest
{
    public class Demo1_Queryable
    {

        public static void Init()
        {
            ConditionScreening();
            Async();
        }

        private static void ConditionScreening()
        {
            Console.WriteLine("");
            Console.WriteLine("#### Condition Screening Start ####");

            SqlSugarClient db = GetInstance();
            //id=@id
            var list = db.Queryable<Order>().Where(it => it.Id == 1).ToList();
            //id=@id or name like '%'+@name+'%'
            var list2 = db.Queryable<Order>().Where(it => it.Id == 1 || it.Name.Contains("jack")).ToList();


            //Create expression 
            var exp = Expressionable.Create<Order>()
                            .And(it => it.Id == 1)
                            .Or(it => it.Name.Contains("jack")).ToExpression();
            var list3 = db.Queryable<Order>().Where(exp).ToList();

            Console.WriteLine("#### Condition Screening End ####");
        }

        private static void Async()
        {
            Console.WriteLine("");
            Console.WriteLine("#### Async Start ####");

            SqlSugarClient db = GetInstance();
            var task1 = db.Queryable<Order>().FirstAsync();
            var task2 = db.Queryable<Order>().Where(it => it.Id == 1).ToListAsync();

            task1.Wait();
            task2.Wait();

            Console.WriteLine("#### Async End ####");
        }

        private static SqlSugarClient GetInstance()
        {
            return new SqlSugarClient(new ConnectionConfig()
            {
                DbType = DbType.SqlServer,
                ConnectionString = Config.ConnectionString,
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
        }
    }
}
