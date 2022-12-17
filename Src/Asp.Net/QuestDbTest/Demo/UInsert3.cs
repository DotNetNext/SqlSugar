using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest 
{
    internal class UInsert3
    {
        public static void Init() 
        {
            var db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = "host=localhost;port=8812;username=admin;password=quest;database=qdb;ServerCompatibilityMode=NoTypeLoading;",
                DbType = DbType.QuestDB,
                LanguageType = LanguageType.Chinese,
                IsAutoCloseConnection = true
            });
            db.Aop.OnLogExecuted = (s, p) => Console.WriteLine(s);
            db.CodeFirst.InitTables<Order1>();
            db.Insertable(new Order1() { Name = "a" }).ExecuteReturnSnowflakeId();

            db.Insertable(new List<Order1>() {
                 new Order1() { Name = "a" },
                  new Order1() { Name = "a" }
            }).ExecuteCommand();

            db.Insertable(new ORDER1() { Name = "a" }).ExecuteReturnSnowflakeId();
        }

        public class Order1
        {
            [SugarColumn(IsPrimaryKey = true )]
            public long Id { get; set; }
            /// <summary>
            /// 姓名
            /// </summary>
            public string Name { get; set; }
            public decimal Price { get; set; }
            [SugarColumn(InsertServerTime =true)]
            public DateTime CreateTime { get; set; }
            [SugarColumn(IsNullable = true)]
            public int CustomId { get; set; } 
        }
        [SugarTable("Order1")]
        public class ORDER1
        {
            [SugarColumn(IsPrimaryKey = true )]
            public long Id { get; set; }
            /// <summary>
            /// 姓名
            /// </summary>
            public string Name { get; set; }
            public decimal Price { get; set; }
            [SugarColumn(InsertSql = "now()")]
            public DateTime CreateTime { get; set; }
            [SugarColumn(IsNullable = true)]
            public int CustomId { get; set; }
        }
    }
}
