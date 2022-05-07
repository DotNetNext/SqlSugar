using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
namespace OrmTest 
{
    internal class UCustom019
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;

            //建表 
            if (!db.DbMaintenance.IsAnyTable("Test001", false))
            {
                db.CodeFirst.InitTables<Test001>();
                //用例代码 
                var dataList = new List<Test001>();
                dataList.Add(new Test001() { id = 1, group = 1, addTime = DateTime.Now });
                dataList.Add(new Test001() { id = 2, group = 1, addTime = DateTime.Now.AddDays(1) });
                dataList.Add(new Test001() { id = 3, group = 2, addTime = DateTime.Now.AddDays(1) });
                dataList.Add(new Test001() { id = 4, group = 2, addTime = DateTime.Now.AddDays(1) });
                dataList.Add(new Test001() { id = 5, group = 2, addTime = DateTime.Now.AddDays(1) });
                dataList.Add(new Test001() { id = 6, group = 3, addTime = DateTime.Now.AddDays(1) });
                var result = db.Insertable(dataList).ExecuteCommand();//用例代码
            }

            var defaultTime = new DateTime(1900, 1, 1);
            var nowTime = DateTime.Now;

            var iQueryAble = db.Queryable<Test001>()
                .GroupBy(it => it.group)
                .GroupBy(it => it.addTime)
                .Where(it=>it.id==1)
                .Select<Test001_Ext>(it => new Test001_Ext
                {
                    group = it.group,
                    addTime = it.addTime,
                    lastTime = SqlFunc.AggregateMax(SqlFunc.IIF(it.group == 1, it.addTime, defaultTime)),
                    lastTime2 = SqlFunc.AggregateMax(SqlFunc.IIF(it.group == 1, it.addTime, defaultTime))
                });
            var res = db.Queryable(iQueryAble)
                           
                            .InnerJoin<Test001>((i, t) => i.group == t.group)
                            .Where((i,t)=>t.addTime < nowTime.AddDays(1))
                            .ToList();

            Console.WriteLine("用例跑完");
            Console.ReadKey();
        }
        //建类
        public class Test001
        {
            public int id { get; set; }

            public int group { get; set; }

            public DateTime addTime { get; set; }
        }

        public class Test001_Ext : Test001
        {
            public DateTime lastTime { get; set; }
            public DateTime lastTime2 { get;   set; }
        }
    }
}
