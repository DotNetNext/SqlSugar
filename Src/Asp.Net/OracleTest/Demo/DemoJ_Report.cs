using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public class DemoJ_Report
    {
        public static void Init()
        {
            Console.WriteLine("");
            Console.WriteLine("#### Utilities Start ####");

            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                DbType = DbType.Oracle,
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
            Demo1(db);
            Demo2(db);
            Demo3(db);
        }

        private static void Demo1(SqlSugarClient db)
        {
            var list = new List<int>() { 1, 2, 3 };
            var query1 = db.Queryable<Order>();
            var queryable2 = db.Reportable(list).ToQueryable<int>();
            var x = db.Queryable(queryable2, query1, (x2, x1) => x1.Id.Equals(x2.ColumnName))
                .Select((x2, x1) => new { x = x1.Id, x2 = x2.ColumnName  }).ToList();
        }
        private static void Demo2(SqlSugarClient db)
        {
            var list = db.Queryable<OrderItem>().ToList();
            var query1 = db.Queryable<Order>();
            var queryable2 = db.Reportable(list).ToQueryable();
            var x = db.Queryable(query1, queryable2, (x1, x2) => x1.Id.Equals(x2.OrderId))
                .Select((x1, x2) => new { name = x1.Name,id=x1.Id, orderid = x2.OrderId }).ToList();
        }
        private static void Demo3(SqlSugarClient db)
        {
            db.CodeFirst.InitTables<operateinfo>();
            db.Deleteable<operateinfo>().ExecuteCommand();
            db.Insertable(new operateinfo()
            {
                 id=1,
                 operate_type=1,
                 operate_time=Convert.ToDateTime("2021-1-1")
            }).ExecuteCommand();
            db.Insertable(new operateinfo()
            {
                id = 1,
                operate_type = 1,
                operate_time = Convert.ToDateTime("2021-1-2")
            }).ExecuteCommand();
            db.Insertable(new operateinfo()
            {
                id = 1,
                operate_type = 1,
                operate_time = Convert.ToDateTime("2021-3-1")
            }).ExecuteCommand();
            db.Insertable(new operateinfo()
            {
                id = 1,
                operate_type = 1,
                operate_time = Convert.ToDateTime("2021-3-2")
            }).ExecuteCommand();
            db.Insertable(new operateinfo()
            {
                id = 1,
                operate_type = 1,
                operate_time = Convert.ToDateTime("2021-4-2")
            }).ExecuteCommand();


            var queryableLeft = db.Reportable(ReportableDateType.MonthsInLast1years).ToQueryable<DateTime>();
            var queryableRight = db.Queryable<operateinfo>();
            var list= db.Queryable(queryableLeft, queryableRight, JoinType.Left,
                (x1, x2) => x2.operate_time.ToString("yyyy-MM")== x1.ColumnName .ToString("yyyy-MM"))
                .GroupBy((x1,x2)=>x1.ColumnName)
                .Where(x1=>SqlFunc.Between(x1.ColumnName,DateTime.Now.AddYears(-1),DateTime.Now))
                .Select((x1, x2) => new
                {
                    count=SqlFunc.AggregateSum(SqlFunc.IIF(x2.id>0,1,0)) ,
                    date= x1.ColumnName.ToString("yyyy-MM")
                    
                }).ToList();
        }


        public partial class operateinfo
        {
            public operateinfo()
            {


            }
            /// <summary>
            /// Desc:操作序号
            /// Default:
            /// Nullable:False
            /// </summary>           
            public int id { get; set; }

            /// <summary>
            /// Desc:操作时间
            /// Default:
            /// Nullable:False
            /// </summary>           
            public DateTime operate_time { get; set; }

            /// <summary>
            /// Desc:操作类型
            /// Default:
            /// Nullable:False
            /// </summary>           
            public int operate_type { get; set; }

            /// <summary>
            /// Desc:操作人编号
            /// Default:
            /// Nullable:False
            /// </summary>           
            public int user_id { get; set; }

        }
    }

}