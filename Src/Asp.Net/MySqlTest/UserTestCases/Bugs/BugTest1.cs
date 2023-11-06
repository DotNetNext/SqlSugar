using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
namespace OrmTest.Test
{
    public class BugTest1

    {

       

        public static void Init()

        {

            SqlSugarClient db = new SqlSugarClient(

             new ConnectionConfig()

             {

                 ConnectionString = Config.ConnectionString,

                 DbType = DbType.MySql,//设置数据库类型

                 IsAutoCloseConnection = true,//自动释放数据务，如果存在事务，在事务结束后释放

                 InitKeyType = InitKeyType.Attribute //从实体特性中读取主键自增列信息

             });

            db.Aop.OnError = (exp) =>//SQL报错

            {

                string sql = exp.Sql;

                //exp.sql 这样可以拿到错误SQL

            };

            //db.DbMaintenance.CreateDatabase();
             
            db.Deleteable<Order>().ExecuteCommand();
            db.Insertable(new Order() { CreateTime = DateTime.Now.Date.AddDays(-1), Name = "1a", Price = 1, CustomId = 1 }).ExecuteCommand();
            db.Insertable(new Order() { CreateTime = DateTime.Now.Date.AddDays(-1).AddHours(23), Name = "1a", Price = 1, CustomId = 1 }).ExecuteCommand();

            db.Insertable(new Order() { CreateTime = DateTime.Now.Date.AddDays(1), Name = "1a", Price = 1, CustomId = 1 }).ExecuteCommand();
            db.Insertable(new Order() { CreateTime = DateTime.Now.Date.AddDays(2), Name = "1a", Price = 1, CustomId = 1 }).ExecuteCommand();
            var s =DateTime.Now.Date.AddMilliseconds(-1);
            var list= db.Queryable<Order>().Where(it => SqlFunc.DateIsSame(it.CreateTime,s)).ToList();
            var s2 = DateTime.Now.Date.AddDays(-1);
            var lists = db.Queryable<Order>().Where(it => SqlFunc.DateIsSame(it.CreateTime, s2)).ToSql();
            Console.ReadKey();

        }

    }
}
