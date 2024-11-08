using OrmTest;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KdbndpTest.SqlServerDemo
{
    internal class PgSqlDemo
    {
        public static void Init() 
        {
           SqlSugarClient Db = new SqlSugarClient(new ConnectionConfig()
           {
               DbType = DbType.Kdbndp,
               ConnectionString = "Server=211.95.20.236 ;Port=35052;UID=system;PWD=abc123;database=test222",
               InitKeyType = InitKeyType.Attribute,
               IsAutoCloseConnection = true,
               MoreSettings=new ConnMoreSettings() { 
                 DatabaseModel=DbType.PostgreSQL
               }
           }, db => {
               db.Aop.OnLogExecuting = (sql, p) =>
               {
                   Console.WriteLine(sql);
                   Console.WriteLine(string.Join(",", p?.Select(it => it.ParameterName + ":" + it.Value)));
               };
           });
            Db.DbMaintenance.CreateDatabase();

            foreach (var item in Db.DbMaintenance.GetColumnInfosByTableName("order",false))
            {
                Console.WriteLine($"{item.DbColumnName} DataType:{item.DataType} IsIdentity :{item.IsIdentity}   IsPrimarykey :{item.IsPrimarykey} IsNullable: {item.IsNullable} Length:{item.Length} Scale:{item.Scale}");
            }

            var yyy = Db.Queryable<Order>().ToList();
            var xxx=Db.Ado.GetDataTable("select 1 as id");

            Db.CodeFirst.InitTables<Order>();
            Db.Insertable(new Order()
            {
                 CreateTime=DateTime.Now,
                 CustomId=1,
                 Name="a",
                 Price=1
            }).ExecuteCommand();
            Db.Updateable(new Order()
            {
                CreateTime = DateTime.Now,
                CustomId = 1,
                Name = "a",
                Price = 1
            }).ExecuteCommand();
            Db.Deleteable(new Order()
            {
                CreateTime = DateTime.Now,
                CustomId = 1,
                Name = "a",
                Price = 1
            }).ExecuteCommand();
        }
    }
}
