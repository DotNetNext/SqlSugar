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
            var db = NewUnitTest.Db;
            db.Insertable(new Order() { Name = "a" }).ExecuteCommand();

            db.Insertable(new List<Order>() {
                 new Order() { Name = "a" },
                  new Order() { Name = "a" }
            }).ExecuteCommand();

            db.Insertable(new ORDER() { Name = "a" }).ExecuteCommand();
            db.Updateable(new Order()
            {
                CustomId = 1,
                CreateTime = DateTime.Now,
                Id = 1,
                Price = 1,
                Name = "a"
            }).ExecuteCommand();
            db.Updateable(new List<Order>(){ new Order()
            {
                CustomId = 1,
                CreateTime = DateTime.Now,
                Id = 1,
                Price = 1,
                Name = "a"
            },
            new Order()
            {
                CustomId = 1,
                CreateTime = DateTime.Now,
                Id = 1,
                Price = 1,
                Name = "a"
            } }).ExecuteCommand();


            db.Updateable<Order>().SetColumns(it => new Order()
            {
                CustomId = 1,
                Price = 1,
                Name = "a"
            }, true).Where(it => it.Id == 1).ExecuteCommand();

            db.Updateable<ORDER>().SetColumns(it => new ORDER()
            {
                CustomId = 1,
                Price = 1,
                Name = "a"
            }, true).Where(it => it.Id == 1).ExecuteCommand();
        }
        public class Order
        {
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int Id { get; set; }
            /// <summary>
            /// 姓名
            /// </summary>
            public string Name { get; set; }
            public decimal Price { get; set; }
            [SugarColumn(InsertServerTime =true,UpdateServerTime =true)]
            public DateTime CreateTime { get; set; }
            [SugarColumn(IsNullable = true)]
            public int CustomId { get; set; } 
        }

        public class ORDER
        {
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int Id { get; set; }
            /// <summary>
            /// 姓名
            /// </summary>
            public string Name { get; set; }
            public decimal Price { get; set; }
            [SugarColumn(InsertSql = "'2020-1-1'",UpdateSql = "'2022-1-1'")]
            public DateTime CreateTime { get; set; }
            [SugarColumn(IsNullable = true)]
            public int CustomId { get; set; }
        }
    }
}
