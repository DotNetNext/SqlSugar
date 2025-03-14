using SqlSugar;
using System;
using System.Drawing;
using System.Linq.Expressions;
using System.Reflection;

namespace OrmTest
{
    public class Unitadfsa1ysfds
    {
        public static  void Init()
        {
            var db = NewUnitTest.Db;
            db.CurrentConnectionConfig.MoreSettings = new ConnMoreSettings()
            {
                IsCorrectErrorSqlParameterName = true
            };

            //建表
            if (!db.DbMaintenance.IsAnyTable("客户"))
            {
                db.CodeFirst.SetStringDefaultLength(200).InitTables(typeof(客户));
                db.Insertable(new 客户() { 客户名 = "张三", 手机 = "111", VIP = true }).ExecuteReturnEntity();
                db.Insertable(new 客户() { 客户名 = "李四", 手机 = "222", VIP = false }).ExecuteReturnEntity();
            }

            if (!db.DbMaintenance.IsAnyTable("工作表"))
            {
                db.CodeFirst.SetStringDefaultLength(200).InitTables(typeof(工作表));
                db.Insertable(new 工作表() { 日期 = DateTime.Now, 客户 = "张三", 工作名 = "测试1" }).ExecuteReturnEntity();
                db.Insertable(new 工作表() { 日期 = DateTime.Now, 客户 = "李四", 工作名 = "测试2" }).ExecuteReturnEntity();
                db.Insertable(new 工作表() { 日期 = DateTime.Now, 客户 = "张三", 工作名 = "测试3" }).ExecuteReturnEntity();
                db.Insertable(new 工作表() { 日期 = DateTime.Now, 客户 = "张三", 工作名 = "测试4" }).ExecuteReturnEntity();
                db.Insertable(new 工作表() { 日期 = DateTime.Now, 客户 = "李四", 工作名 = "测试5" }).ExecuteReturnEntity();

            }

            Console.WriteLine("数据库已经建立");

            var listA = db.Queryable<工作表>().ToList();
            var listB = db.Queryable<客户>().ToList();
            foreach (var item in listA)
            {
                Console.WriteLine($"编号: {item.编号}, 日期: {item.日期},  客户名: {item.客户},  工作名:{item.工作名}");
            }
            foreach (var item in listB)
            {
                Console.WriteLine($"Id: {item.ID},  客户名: {item.客户名}, 电话: {item.手机},  VIP:{item.VIP}");
            }
            Console.WriteLine("-------------------------------------------------");
            db.Aop.OnLogExecuting = (x, y) =>
            {
                Console.WriteLine(x);
            };
            var query5 = db.Queryable<工作表>()
                         .LeftJoin<客户>((o, cus) => o.客户 == cus.客户名)
                         .Where((o, cus) => cus.VIP == true)
                         .Select((o, cus) =>
                         new xxxx { Id = o.编号, 日期 = o.日期, 客户 = o.客户, 工作名 = o.工作名, 电话 = cus.手机 })
                         .ToList();

            foreach (var item in query5)
            {
                Console.WriteLine($"Id: {item.Id}, 客户名: {item.客户}, 电话: {item.电话}, 工作名:{item.工作名}");
            }
            Console.WriteLine("-------------------------------------------------");

            var list = db.Queryable<工作表, 客户>((o, i) => o.客户 == i.客户名)
                       .Where((o, i) => i.VIP == true)
                       .Select((o, i) => new xxxx { Id = o.编号, 日期 = o.日期, 客户 = o.客户, 工作名 = o.工作名, 电话 = i.手机 })
                       .ToList();

            foreach (var item in list)
            {
                Console.WriteLine($"Id: {item.Id}, 客户名: {item.客户}, 电话: {item.电话}, 工作名:{item.工作名}");
            }

        }

    } 

    //实体与数据库结构一样
    public class 客户
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int ID { get; set; }
        //[SugarColumn(ColumnName = "客户 名称")]
        [SugarColumn(ColumnDataType = "nvarchar(100)")]
        public string? 客户名 { get; set; }
        public string? 手机 { get; set; }
        public bool? VIP { get; set; }

    }


    public class 工作表
    {

        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int 编号 { get; set; }
        public DateTime? 日期 { get; set; }
        [SugarColumn(ColumnDataType = "nvarchar(100)")]
        public string? 客户 { get; set; }
        [SugarColumn(ColumnDataType = "nvarchar(100)")]
        public string? 工作名 { get; set; }

    }


    public class xxxx
    {
        public int Id { get; set; }
        public DateTime? 日期 { get; set; }
        public string? 客户 { get; set; }
        public string? 工作名 { get; set; }
        public string? 电话 { get; set; }

    }

}
