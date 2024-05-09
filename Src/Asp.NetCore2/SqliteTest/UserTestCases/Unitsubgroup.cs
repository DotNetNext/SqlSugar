using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OrmTest
{
    internal class UnitSubGroupadfa
    {
        public static void Init()
        {
            Console.WriteLine("Hello, World!");
            SqlSugarClient _db = NewUnitTest.Db;
            var expable = Expressionable.Create<PayList>();
            expable.And(v => v.orderNumber == SqlFunc.Subqueryable<ReceiveList>().Where(v => v.AddTime >= DateTime.Parse("2024-01-01")).GroupBy(v => v.orderNumber).Select(v => v.orderNumber));
            string[] xtzc = new string[] { "a", "b", "c" };
            expable.And(v => xtzc.Contains(v.F_regfrom));
            expable.And(v => v.IsPaid == 1);
            int total = 0;
            _db.CodeFirst.InitTables<PayList, ReceiveList>();
            var result = _db.Queryable<PayList>().Where(expable.ToExpression()).OrderByDescending(q => q.Id).ToPageList(1, 10, ref total);
        }
        public class PayList
        {
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]//通过特性设置主键和自增列 
            public int Id { get; set; }

            public string? F_regfrom { get; set; }
            public string? orderNumber { get; set; }
            public int F_Hide { get; set; }
            public byte IsPaid { get; set; }

        }
        public class ReceiveList
        {
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]//通过特性设置主键和自增列 
            public int Id { get; set; }
            public DateTime? AddTime { get; set; }
            public string? orderNumber { get; set; }
        }
    }
}
