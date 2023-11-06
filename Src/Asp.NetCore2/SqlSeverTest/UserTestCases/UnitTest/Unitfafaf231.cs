using SqlSeverTest.UserTestCases;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace OrmTest
{
    public class Unitadfafa
    {
        public static void Init()
        {
            //
            SqlSugarClient Db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = Config.ConnectionString,
                DbType = DbType.SqlServer,
                IsAutoCloseConnection = true
            },
                  db =>
                  {
                      //5.1.3.24统一了语法和SqlSugarScope一样，老版本AOP可以写外面

                      db.Aop.OnLogExecuting = (sql, pars) =>
                      {
                          Console.WriteLine(sql);//输出sql,查看执行sql 性能无影响
                      };
                  });
            Init(Db);

            var list = Db.Queryable<OrderModel>()
                .Includes(x => x.FleetInfo)
                .GroupBy(x => new
                {
                    x.CreateTime!.Value.Date,
                })
                .Select(x => new
                {
                    Date = x.CreateTime!.Value.Date,
                    SumPrice = SqlFunc.AggregateSum(x.Price)
                }).MergeTable().ToList();
             
            for (int i = 0; i < 100; i++)
            {
                var guids = Db.Queryable<Order>().Select(it => new
                {
                    id = SqlFunc.NewUid()
                }).Take(10).ToList(); 
            }

             var sql= Db.Queryable<Order>()
               .Select(it => new {
                   xx = SqlFunc.IIF(true, 1, -it.Id)
               }).ToSqlString();

            if(sql!= "SELECT  ( CASE  WHEN ( 1 = 1 )  THEN 1  ELSE [Id] * -1  END ) AS [xx]  FROM [Order] ")
            { 
                throw new Exception("unit error");
            }
            Db.Queryable<Order>()
               .Select(it => new
               {
                   xx = SqlFunc.IIF(true, 1, -it.Id)
               }).ToList();
            Console.WriteLine(list.Count);
        }

        private static void Init(SqlSugarClient db)
        {
            var orders = new List<OrderModel>();
            var fleets = new List<Fleet>();
            int a = 0;
            for (int i = 0; i < 10; i++)
            {
                orders.Add(new OrderModel
                {
                    Id = i,
                    CreateTime = DateTime.Now.AddDays(-a),
                    FleetId = i,
                    Price = (decimal)(i * 0.3)
                });
                a++;
                if (a == 5)
                    a = 0;
                fleets.Add(new Fleet
                {
                    Id = i,
                    Name = $"{i}--号"
                });
            }

            db.DbMaintenance.CreateDatabase();
            db.CodeFirst.InitTables(typeof(OrderModel));
            db.CodeFirst.InitTables(typeof(Fleet));
            db.DbMaintenance.TruncateTable<OrderModel,Fleet>();
            db.Insertable<OrderModel>(orders).ExecuteCommand();
            db.Insertable<Fleet>(fleets).ExecuteCommand();
        }
    }

    public class OrderModel
    {
        [SugarColumn(IsPrimaryKey = true)]
        public long Id { get; set; }

        public decimal Price { get; set; }
        public DateTime? CreateTime { get; set; }

        public long FleetId { get; set; }

        [SugarColumn(IsIgnore = true, IsNullable = true)]
        [Navigate(NavigateType.OneToOne, nameof(FleetId))]
        public Fleet? FleetInfo { get; set; }
    }

    public class Fleet
    {
        [SugarColumn(IsPrimaryKey = true)]
        public long Id { get; set; }

        public string Name { get; set; }

        public long Pid { get; set; } = 0;

        [Navigate(NavigateType.OneToOne, nameof(Pid))]
        public Fleet? Flee { get; set; }
    }
}
