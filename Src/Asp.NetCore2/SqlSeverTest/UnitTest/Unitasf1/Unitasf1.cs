using SqlSugar;
using System;
using System.Linq;

namespace OrmTest
{
    internal class Unitasf1
    {
        public static void Init()
        {
            Console.WriteLine("Hello, World!");


            var db = new SqlSugarScope(new SqlSugar.ConnectionConfig()
            {
                ConnectionString = "server=.;uid=sa;pwd=sasa;database=sqlsugartestxxxx",
                DbType = DbType.SqlServer,
                IsAutoCloseConnection = true
            }, db =>
            {

                //如果是多库看标题6

                //每次Sql执行前事件
                db.Aop.OnLogExecuting = (sql, pars) =>
                {
                    //我可以在这里面写逻辑

                    //技巧：AOP中获取IOC对象
                    //var serviceBuilder = services.BuildServiceProvider();
                    //var log= serviceBuilder.GetService<ILogger<WeatherForecastController>>();


                    //获取原生SQL推荐 5.1.4.63  性能OK
                    Console.WriteLine(UtilMethods.GetNativeSql(sql, pars));
                };

            });
            db.DbMaintenance.CreateDatabase();
            //建表 
            if (!db.DbMaintenance.IsAnyTable("app_category", false)) { db.CodeFirst.InitTables<entity.app_category>(); }
            if (!db.DbMaintenance.IsAnyTable("app_comment", false)) { db.CodeFirst.InitTables<entity.app_comment>(); }
            if (!db.DbMaintenance.IsAnyTable("app_revision", false)) { db.CodeFirst.InitTables<entity.app_revision>(); }
            if (!db.DbMaintenance.IsAnyTable("app_topic", false)) { db.CodeFirst.InitTables<entity.app_topic>(); }


            //用例代码 
            //var result = db.Insertable(new Test001() { id = 1 }).ExecuteCommand();//用例代码

            var _query = db.Queryable <entityMap.app_topic>();
            //_query.Includes(k => k.map_app_category);
            //_query.Includes(k => k.map_app_revision);
            //_query.Includes(k => k.map_app_comment);
            var _queryPagerSelect = _query.Select(k => new entityMapDTO.app_topic()
            {
                fk_category_name = k.map_app_category.category_name,
                fk_comment_total = k.map_app_comment.Count(),
                IsAny = k.map_app_revision.Any(),
            },true)
                .ToList() ;
            db.CodeFirst.InitTables<Order, Custom>();
            var exp = Expressionable.Create<Custom>().And(s => s.Id == 1);
            var list4 = db.Queryable<Order>().Take(10).Select(it => new
            {
                customName2 = SqlFunc.Subqueryable<Custom>()
              .Where(exp.ToExpression())
                    .Where(exp.ToExpression())
             .Select(s => s.Name)
            }).ToList();

            var list2 = db.Queryable<Order>().Take(10).Select(it => new
            {
                customName2 = SqlFunc.Subqueryable<Custom>()
            .AS("CUSTOM".ToString())
                  .Where(exp.ToExpression())
           .Select(s => s.Name)
            }).ToList();
            //Console.WriteLine("用例跑完");
            //Console.ReadKey();
        }
    }
}