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
            var dr = new Dictionary<string, object>();
            dr.Add("Id", 1);
            dr.Add("Name","2");
            var dr2 = new Dictionary<string, object>();
            dr2.Add("Id", 2);
            dr2.Add("Name", "2");
            //by entity
            db.Deleteable<object>().AS("[Order]").WhereColumns(new List<Dictionary<string,object>>() { dr, dr2 }).ExecuteCommand();

            //by primary key
            db.Deleteable<Order>().In(1111).ExecuteCommand();

            //by primary key array
            db.Deleteable<Order>().In(new int[] { 1111, 2222 }).ExecuteCommand();

            //by expression
            db.Deleteable<Order>().Where(it => it.Id == 11111).ExecuteCommand();


            db.Deleteable<Order>().WhereColumns(new List<Order>() { new Order() {  Name="a"} },it=>it.Name).ExecuteCommand();

            //logic delete
            db.CodeFirst.InitTables<LogicTest>();
  ;
            db.Deleteable<LogicTest>().Where(it=>it.Id==1).IsLogic().ExecuteCommand();
            Console.WriteLine("#### Deleteable End ####");

        }
    }
    public class LogicTest 
    {
        [SugarColumn(IsPrimaryKey =true,IsIdentity =true)]
        public int Id { get; set; }

        public bool isdeleted { get; set; }
    }
}
