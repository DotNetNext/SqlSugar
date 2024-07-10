using OrmTest;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KdbndpTest.OracleDemo.UnitTest
{
    internal class UnitDateTime
    {
        public static void Init()
        {
            Console.WriteLine("");
            Console.WriteLine("#### DateTime Start ####");

            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                DbType = DbType.Kdbndp,
                ConnectionString = Config.ConnectionString.Replace("59321", "59322"),
                InitKeyType = InitKeyType.Attribute,
                IsAutoCloseConnection = true,
                MoreSettings = new ConnMoreSettings()
                {
                    DatabaseModel = DbType.MySql
                },
                AopEvents = new AopEvents
                {
                    OnLogExecuting = (sql, p) =>
                    {
                        Console.WriteLine(sql);
                        Console.WriteLine(string.Join(",", p?.Select(it => it.ParameterName + ":" + it.Value)));
                    }
                }
            });

            var dt = db.Ado.GetDataTable("show database_mode;");

            var now = DateTime.Now;
            var insertObj = new UnitOrderDateTest() { Id = 1, Name = "0", CreateTime = now };
            var insertObj2 = new List<UnitOrderDateTest>
            {
                 new UnitOrderDateTest() { Id = 11, Name = "1ms",CreateTime=now.AddMilliseconds(1) },
                 new UnitOrderDateTest() { Id = 12, Name = "1ss" ,CreateTime=now.AddSeconds(1)},
                   new UnitOrderDateTest() { Id = 12, Name = "1mi" ,CreateTime=now.AddMinutes(1)}
            };
            db.CodeFirst.InitTables<UnitOrderDateTest>();
            db.DbMaintenance.TruncateTable<UnitOrderDateTest>();
            var insertObj3 = new UnitOrderDateTest() { Id = 1, Name = "1h", CreateTime = now.AddHours(1) };
            db.Insertable(insertObj).ExecuteCommand();
            db.Insertable(insertObj2).ExecuteCommand();
            db.Insertable(insertObj3).ExecuteCommand();

            var beginTime = now.AddMinutes(-2);
            var endTime = now.AddHours(1).AddMinutes(2);
            var list = db.Queryable<UnitOrderDateTest>().Where(it => it.CreateTime >= beginTime
            && it.CreateTime <= endTime)
                .ToList();

            var sql = db.Queryable<UnitOrderDateTest>().Where(it => it.CreateTime >= beginTime
            && it.CreateTime <= endTime)
                .ToSqlString();

            var list2 = db.Ado.SqlQuery<UnitOrderDateTest>(sql);


            if (list.Count() != 5 || list2.Count() != 5)
            {
                throw new Exception("UnitDateTime Error");
            }

            Console.WriteLine("#### DateTime End ####");
        }
        public class UnitOrderDateTest
        {
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int Id { get; set; }

            public string Name { get; set; }
            [SugarColumn(IsNullable = true)]
            public DateTime CreateTime { get; set; }
        }
    }
}
