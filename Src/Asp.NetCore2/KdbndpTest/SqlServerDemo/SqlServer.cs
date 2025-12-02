using KdbndpTest.Models;
using OrmTest;
using SqlSugar;
using System;
using System.Collections.Generic;
 
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace KdbndpTest.SqlServerDemo
{
    internal class SqlServerDemo
    {
        public static void Init()
        {
            SqlSugarClient Db = new SqlSugarClient(new ConnectionConfig()
            {
                DbType = DbType.Kdbndp,
                ConnectionString = "Server=8.137.16.241;Port=54321;UID=system;PWD=123456;database=test;CommandTimeout=120;DbVersion=sqlserver;Search Path=dbo,public;ErrorThrow=true;",
                InitKeyType = InitKeyType.Attribute,
                IsAutoCloseConnection = true,
                MoreSettings = new ConnMoreSettings()
                {
                    DatabaseModel = DbType.SqlServer
                }
            }, db =>
            {
                db.Aop.OnLogExecuting = (sql, p) =>
                {
                    Console.WriteLine(sql);
                    Console.WriteLine(string.Join(",", p?.Select(it => it.ParameterName + ":" + it.Value)));
                };
            });

            InitDatas(Db); 

            InsertDemo(Db);

            UpdateDemo(Db); 

            QueryDemo(Db); 

            DeleteDemo(Db); 

            GetTableInfos(Db);

            BytesTest(Db);
        }

        private static void BytesTest(SqlSugarClient db)
        {
            db.CodeFirst.InitTables<ByteArrayModel>();
            db.DbMaintenance.TruncateTable<ByteArrayModel>();
            db.Insertable(new List<ByteArrayModel>() {
                new ByteArrayModel() { Id = 1, Bytes = new byte[] { 0, 1 } },
                new ByteArrayModel() { Id = 2, Bytes = new byte[] { 0, 1 } } }).ExecuteCommand();
            var list=db.Queryable<ByteArrayModel>().ToList();
        }

        private static void QueryDemo(SqlSugarClient Db)
        {
            var list1 = Db.Queryable<Order>().Where(it => it.CreateTime.AddDays(1)>DateTime.Now).ToList();
            var list2 = Db.Queryable<Order>().PartitionBy(it => it.Id).ToList();
            try
            {
                var list5 = Db.Queryable<Order>().Select(it => new
                {
                    date = it.CreateTime.ToString("yyyy-MM-dd")
                }).ToList();
            }
            catch (Exception)
            {
                Console.WriteLine("需要金仓那边支持format");
            }
        }
        private static void DeleteDemo(SqlSugarClient Db)
        {
            Db.Deleteable(new Order()
            {
                CreateTime = DateTime.Now,
                CustomId = 1,
                Name = "a",
                Price = 1
            }).ExecuteCommand();
        }

        private static void UpdateDemo(SqlSugarClient Db)
        {

            Db.Updateable(new Order()
            {
                CreateTime = DateTime.Now,
                CustomId = 1,
                Name = "a",
                Price = 1
            }).ExecuteCommand();

            Db.Updateable<Order>()
                .SetColumns(it => new Order()
                {

                     CreateTime=DateTime.Now
                })
                .Where(it=>it.Id==1).ExecuteCommand();

            Db.Updateable(Db.Queryable<Order>().Take(2).ToList())
               .ExecuteCommand();
            Db.Updateable(Db.Queryable<Order>().Take(2).ToList())
                .Where(it=>it.Name!=null).ExecuteCommand();

          
        }

        private static void GetTableInfos(SqlSugarClient Db)
        {
            foreach (var item in Db.DbMaintenance.GetColumnInfosByTableName("order", false))
            {
                Console.WriteLine($"{item.DbColumnName} DataType:{item.DataType} IsIdentity :{item.IsIdentity}   IsPrimarykey :{item.IsPrimarykey} IsNullable: {item.IsNullable} Length:{item.Length} Scale:{item.Scale}");
            }

            var yyy = Db.Queryable<Order>().ToList();
            var xxx = Db.Ado.GetDataTable("select 1 as id");
        }

        private static void InitDatas(SqlSugarClient Db)
        {
            Db.DbMaintenance.CreateDatabase();
            Db.CodeFirst.InitTables<Order>();
        }

        private static void InsertDemo(SqlSugarClient Db)
        {
            Db.Insertable(new Order()
            {
                CreateTime = DateTime.Now,
                CustomId = 1,
                Name = "a",
                Price = 1
            }).ExecuteCommand();

            var orders = new List<Order>()
            {
                new Order()
            {
                CreateTime = DateTime.Now,
                CustomId = 1,
                Name = "a",
                Price = 1
            },
                new Order()
            {
                CreateTime = DateTime.Now,
                CustomId = 1,
                Name = "a",
                Price = 1
            }
            };
            Db.Insertable(orders).ExecuteCommand();
        }
    }
}
