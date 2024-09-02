using SqlSugar;
using System;
using System.Collections.Generic;

namespace OrmTest
{
    internal class _a7_JsonType
    {
        /// <summary>
        /// Demonstrates JSON operations with SqlSugar.
        /// 展示了在 SqlSugar 中进行 JSON 操作的示例。
        /// </summary>
        internal static void Init()
        {
            // Get a new database connection object
            // 获取一个新的数据库连接对象
            var db = DbHelper.GetNewDb();

            // Create table
            // 创建表
            db.CodeFirst.InitTables<UnitJsonTest>();

            // Insert a record with a JSON property
            // 插入一条包含 JSON 属性的记录
            db.Insertable(new UnitJsonTest()
            {
                Name = "json1",
                Order = new Order { Id = 1, Name = "order1" }
            }).ExecuteCommand();

            // Query all records from the table
            // 查询表中的所有记录
            var list = db.Queryable<UnitJsonTest>().ToList();

            //Sqlfunc.JsonXXX
            var list2=db.Queryable<UnitJsonTest>()
                .Select(it => new
                {
                    id=it.Id,
                    jsonname=SqlFunc.JsonField(it.Order,"Name")
                })
                .ToList();

            db.CodeFirst.InitTables<Unitasdfafaass>();
            db.DbMaintenance.TruncateTable<Unitasdfafaass>();
            db.Insertable(new Unitasdfafaass()
            {
                 aaa=new string[] { "a","c"},
                 name="a"
            }).ExecuteCommand();
            var isOk= db.Queryable<Unitasdfafaass>().Any(it => SqlFunc.JsonArrayAny(it.aaa, "a"));
            var isOk2 = db.Queryable<Unitasdfafaass>().Any(it => SqlFunc.JsonArrayAny(it.aaa, it.name));
            if (isOk2 != isOk|| isOk== false)
            {
                throw new Exception("unit error");
            }
        }
        [SugarTable("unitdfasx44")]
        public class Unitasdfafaass 
        {
            [SugarColumn(IsJson =true)]
            public string[] aaa { get; set; }
            public string name { get; set; }
        }

        /// <summary>
        /// Represents a class with a JSON property.
        /// 表示一个包含 JSON 属性的类。
        /// </summary>
        [SugarTable("UnitJsonTest_a7")]
        public class UnitJsonTest
        {
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int Id { get; set; }

            [SugarColumn(IsJson = true)]
            public Order Order { get; set; }

            public string Name { get; set; }
        }

        /// <summary>
        /// Represents an order entity.
        /// 表示订单实体。
        /// </summary>
        public class Order
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}