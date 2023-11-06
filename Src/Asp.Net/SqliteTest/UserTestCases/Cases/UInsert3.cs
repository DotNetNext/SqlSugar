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
            db.CodeFirst.InitTables<Order1>();
            db.Insertable(new Order1() { Name = "a" }).ExecuteCommand();

            db.Insertable(new List<Order1>() {
                 new Order1() { Name = "a" },
                  new Order1() { Name = "a" }
            }).ExecuteCommand();

            db.Insertable(new ORDER1() { Name = "a" }).ExecuteCommand();

            db.Updateable(new Order1()
            {
                CustomId = 1,
                CreateTime = DateTime.Now,
                Id = 1,
                Price = 1,
                Name = "a"
            }).ExecuteCommand();

            db.Updateable(new List<Order1>(){ new Order1()
            {
                CustomId = 1,
                CreateTime = DateTime.Now,
                Id = 1,
                Price = 1,
                Name = "a"
            },
            new Order1()
            {
                CustomId = 1,
                CreateTime = DateTime.Now,
                Id = 1,
                Price = 1,
                Name = "a"
            } }).ExecuteCommand();


            db.Updateable<Order1>().SetColumns(it => new Order1()
            {
                CustomId = 1,
                Price = 1,
                Name = "a"
            }, true).Where(it => it.Id == 1).ExecuteCommand();

            db.Updateable<ORDER1>().SetColumns(it => new ORDER1()
            {
                CustomId = 1,
                Price = 1,
                Name = "a"
            }, true).Where(it => it.Id == 1).ExecuteCommand();
        }

        public class Order1
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

        public class ORDER1
        {
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int Id { get; set; }
            /// <summary>
            /// 姓名
            /// </summary>
            public string Name { get; set; }
            public decimal Price { get; set; }
            [SugarColumn(InsertSql = "'2020-1-1'", UpdateSql  = "'2020-1-1'")]
            public DateTime CreateTime { get; set; }
            [SugarColumn(IsNullable = true)]
            public int CustomId { get; set; }
        }
    }
}
