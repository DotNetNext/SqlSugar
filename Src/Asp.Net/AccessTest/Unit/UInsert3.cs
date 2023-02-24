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
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = Config.ConnectionString,//Master Connection
                DbType = DbType.Access,
                InitKeyType = InitKeyType.Attribute,
                IsAutoCloseConnection = true
            });
            db.CodeFirst.InitTables<Order1>();
            db.Aop.OnLogExecuting = (sql, pars) =>
            {
                Console.WriteLine(UtilMethods.GetSqlString(db.CurrentConnectionConfig.DbType, sql, pars));//输出sql,查看执行sql 性能无影响
            };
            db.Insertable(new Order1() { Name = "a" }).ExecuteCommand();

            db.Insertable(new List<Order1>() {
                 new Order1() { Name = "a" },
                  new Order1() { Name = "a" }
            }).ExecuteCommand();

            db.Insertable(new ORDER1() { Name = "a" }).ExecuteCommand();
        }

        public class Order1
        {
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int Id { get; set; }
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

        public class ORDER1
        {
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int Id { get; set; }
            /// <summary>
            /// 姓名
            /// </summary>
            public string Name { get; set; }
            public decimal Price { get; set; }
            [SugarColumn(InsertSql = "'2020-1-1'")]
            public DateTime CreateTime { get; set; }
            [SugarColumn(IsNullable = true)]
            public int CustomId { get; set; }
        }
    }
}
