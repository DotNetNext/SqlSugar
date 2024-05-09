using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public class Demo2_Updateable
    {
        public static void Init()
        {
            Console.WriteLine("");
            Console.WriteLine("#### Updateable Start ####");

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



            /*** 1.entity or List ***/

            var updateObj = new Order() { Id = 1, Name = "order1" };
            var updateObjs = new List<Order> {
                 new Order() { Id = 11, Name = "order11" },
                 new Order() { Id = 12, Name = "order12" }
            };

            //update all columns by primary key
            var result = db.Updateable(updateObj).ExecuteCommand();//update single
            var result2 = db.Updateable(updateObjs).ExecuteCommand();//update List<Class>

            //Ignore  Name and Price
            var result3 = db.Updateable(updateObj).IgnoreColumns(it => new { it.CreateTime, it.Price }).ExecuteCommand();

            //only update Name and CreateTime 
            var result4 = db.Updateable(updateObj).UpdateColumns(it => new { it.Name, it.CreateTime }).ExecuteCommand();

            //If there is no primary key
            var result5 = db.Updateable(updateObj).WhereColumns(it => new { it.Id }).ExecuteCommand();//update single by id
            var result6 = db.Updateable(updateObjs).WhereColumns(it => new { it.Id }).ExecuteCommand();//update List<Class> by id




            /*** 2.by expression ***/

            //update  name,createtime
            var result7 = db.Updateable<Order>(it => new Order() { Name = "a", CreateTime = DateTime.Now }).Where(it => it.Id == 11).ExecuteCommand();
            var result71 = db.Updateable<Order>().SetColumns(it => new Order() { Name = "a", CreateTime = DateTime.Now }).Where(it => it.Id == 11).ExecuteCommand();
            //only update name
            var result8 = db.Updateable<Order>(it => it.Name == "Name" + "1").Where(it => it.Id == 1).ExecuteCommand();
            var result81 = db.Updateable<Order>().SetColumns(it => it.Name == "Name" + "1").Where(it => it.Id == 1).ExecuteCommand();
            //




            /*** 3.by Dictionary ***/
            var dt = new Dictionary<string, object>();
            dt.Add("id", 1);
            dt.Add("name", "abc");
            dt.Add("createTime", DateTime.Now);
            var dtList = new List<Dictionary<string, object>>();
            dtList.Add(dt);

            var t66 = db.Updateable(dt).AS("Order").WhereColumns("id").ExecuteCommand();
            var t666 = db.Updateable(dtList).AS("Order").WhereColumns("id").ExecuteCommand();



            /*** 4.Other instructions ***/

            var caseValue = "1";
            //Do not update NULL columns
            db.Updateable(updateObj).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommand();

            //if 1 update name else if 2 update name,createtime
            db.Updateable(updateObj)
                            .UpdateColumnsIF(caseValue == "1", it => new { it.Name })
                            .UpdateColumnsIF(caseValue == "2", it => new { it.Name, it.CreateTime })
                            .ExecuteCommand();
            //Use Lock
            db.Updateable(updateObj).With(SqlWith.UpdLock).ExecuteCommand();

            //Where Sql
            //db.Updateable(updateObj).Where("id=@x", new { x = "1" }).ExecuteCommand();

            Console.WriteLine("#### Updateable End ####");
        }

    }
}