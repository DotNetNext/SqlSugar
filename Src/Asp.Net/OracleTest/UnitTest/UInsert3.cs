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
        }

        public class Order
        {
            [SugarColumn(IsPrimaryKey = true,OracleSequenceName = "Seq_Id")]
            public int Id { get; set; }
            /// <summary>
            /// 姓名
            /// </summary>
            public string Name { get; set; }
            public decimal Price { get; set; }
            [SugarColumn(InsertServerTime =true)]
            public DateTime CreateTime { get; set; }
            [SugarColumn(IsNullable = true)]
            public int CustomId { get; set; } 
        }

        public class ORDER
        {
            [SugarColumn(IsPrimaryKey = true, OracleSequenceName = "Seq_Id")]
            public int Id { get; set; }
            /// <summary>
            /// 姓名
            /// </summary>
            public string Name { get; set; }
            public decimal Price { get; set; }
            [SugarColumn(InsertSql = "sysdate")]
            public DateTime CreateTime { get; set; }
            [SugarColumn(IsNullable = true)]
            public int CustomId { get; set; }
        }
    }
}
