using SqlSeverTest.UserTestCases;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public class Demo4_Deleteable
    {
        public static void Init()
        {
            Console.WriteLine("");
            Console.WriteLine("#### Deleteable Start ####");

            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
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
            //by entity
            db.Deleteable<Order>().Where(new Order() { Id = 1111 }).ExecuteCommand();

            //by primary key
            db.Deleteable<Order>().In(1111).ExecuteCommand();

            //by primary key array
            db.Deleteable<Order>().In(new int[] { 1111, 2222 }).ExecuteCommand();

            //by expression
            db.Deleteable<Order>().Where(it => it.Id == 11111).ExecuteCommand();

            //logic delete
            db.CodeFirst.InitTables<LogicDeleteTezt>();
            db.Deleteable<LogicDeleteTezt>().Where(it => it.Name == "a").IsLogic().ExecuteCommand();

            db.Deleteable<LogicDeleteTezt>().Where(it => it.Name == "a").IsLogic().ExecuteCommand();

            db.Deleteable<Order>().WhereColumns(db.Queryable<Order>().Take(2).ToList(), it => new { it.Id, it.Name }).ExecuteCommand();

            db.CodeFirst.InitTables<RdBaseTerm>();
            db.Deleteable<RdBaseTerm>()
                .Where(it=>it.F_ID>1).IsLogic()
                .ExecuteCommandAsync("F_DELETE_MARK", 1, "F_DELETE_TIME", "F_DELETE_USER_ID", "1")
                .GetAwaiter().GetResult();


            db.Deleteable<RdBaseTerm>()
             .In(Enumerable.Range(1,10001).ToArray())
             .IsLogic()
             .ExecuteCommandAsync("F_DELETE_MARK", 1, "F_DELETE_TIME", "F_DELETE_USER_ID", "1")
             .GetAwaiter().GetResult();
            Console.WriteLine("#### Deleteable End ####");

        }
        public class RdBaseTerm
        {
            [SqlSugar.SugarColumn(IsPrimaryKey =true ,IsIdentity =true)]
            public long F_ID { get; set; } // 假设F_ID是一个长整型字段，根据实际情况可以调整类型  
            public int  F_DELETE_MARK { get; set; } // 假设F_DELETE_MARK是字符串类型，根据实际情况可以调整类型  
            public DateTime? F_DELETE_TIME { get; set; } // 使用可空的DateTime类型，因为F_DELETE_TIME可能为空  
            public string F_DELETE_USER_ID { get; set; } // 假设用户ID是字符串类型，根据实际情况可以调整类型  

            // 可以根据需要添加其他表字段  
        }
        public class LogicDeleteTezt 
        {
            public string Name { get; set; }
            [SugarColumn(IsPrimaryKey =true)]
            public int Id { get; set; }
            public bool IsDeleted { get; set; }
        }
    }
}
