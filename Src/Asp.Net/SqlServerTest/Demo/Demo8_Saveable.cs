using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public  class Demo8_Saveable
    {
        public static void Init()
        {
            Console.WriteLine("");
            Console.WriteLine("#### Saveable Start ####");

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


            //insert or update
            var x= db.Storageable<Order>(new Order() { Id=1, Name="jack" }).ToStorage();
            x.AsUpdateable.ExecuteCommand();
            x.AsInsertable.ExecuteCommand();


            var x2 = db.Storageable<Order>(new Order() { Id = 0, Name = "jack" }).ToStorage();
            x2.BulkCopy();
            x2.BulkUpdate();

            var dt = db.Queryable<Order>().Take(1).ToDataTable();
            dt.TableName = "order";
            var addRow = dt.NewRow();
            addRow["id"] = 0;
            addRow["price"] = 1;
            addRow["Name"] = "a";
            dt.Rows.Add(addRow);
            var x3 = 
                db.Storageable(dt)
                .WhereColumns("id").ToStorage();

            x3.AsInsertable.IgnoreColumns("id").ExecuteCommand();
            x3.AsUpdateable.ExecuteCommand();


            var x4 =
               db.Storageable(dt)
               .SplitDelete(it=>Convert.ToInt32( it["id"])>0)
               .WhereColumns("id").ToStorage();
            x4.AsDeleteable.ExecuteCommand();

            Console.WriteLine("");
            Console.WriteLine("#### Saveable End ####");
        }
    }
}
