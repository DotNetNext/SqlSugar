//using SqlSugar;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace OrmTest.Test
//{
//    public  class BugTest
//    {
//        public static void Init()
//        {
//            var db = GetInstance();
//            db.CodeFirst.InitTables<InTest>();
//            List<InTest> tl = new List<InTest>();
//            for (int i = 0; i < 10; i++)
//            {
//                InTest t = new InTest();
//                t.name = "1";
//                t.aa = new byte[] {(byte) i, 32, 12, 33,12,11,11,11,11,11,11,11,11,11 };
//                tl.Add(t);
//            }
//            db.Insertable(tl).ExecuteCommand();
//           var xx= db.Queryable<InTest>().ToList();
//            var xxxx = xx.Take(2).ToList();
//            foreach (var item in xxxx)
//            {
//                item.aa = new byte[] { (byte)21, 11, 3, 33, 12, 11, 11, 11, 11, 11, 11, 11, 11, 11 };
//            }
//            db.Updateable(xxxx).WhereColumns(it=>it.name).ExecuteCommand();
//           xx = db.Queryable<InTest>().ToList();
//        }
//        private static SqlSugarClient GetInstance()
//        {
//            return new SqlSugarClient(new ConnectionConfig()
//            {
//                DbType = SqlSugar.DbType.Sqlite,
//                ConnectionString = Config.ConnectionString,
//                InitKeyType = InitKeyType.Attribute,
//                IsAutoCloseConnection = true,
//                AopEvents = new AopEvents
//                {
//                    OnLogExecuting = (sql, p) =>
//                    {
//                        Console.WriteLine(sql);
//                        Console.WriteLine(string.Join(",", p?.Select(it => it.ParameterName + ":" + it.Value)));
//                    }
//                }
//            });
//        }
//        public class InTest
//        {
//            public byte[] aa { get; set; }
//            public string name { get; set; }
//        }
//    }
//}
