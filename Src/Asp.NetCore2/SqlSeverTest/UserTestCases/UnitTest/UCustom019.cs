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
            if (!db.DbMaintenance.IsAnyTable("UintTest001", false))
            {
                db.CodeFirst.InitTables<UintTest001>();
                //用例代码 
                var dataList = new List<UintTest001>();
                dataList.Add(new UintTest001() { id = 1, group = 1, addTime = DateTime.Now });
                dataList.Add(new UintTest001() { id = 2, group = 1, addTime = DateTime.Now.AddDays(1) });
                dataList.Add(new UintTest001() { id = 3, group = 2, addTime = DateTime.Now.AddDays(1) });
                dataList.Add(new UintTest001() { id = 4, group = 2, addTime = DateTime.Now.AddDays(1) });
                dataList.Add(new UintTest001() { id = 5, group = 2, addTime = DateTime.Now.AddDays(1) });
                dataList.Add(new UintTest001() { id = 6, group = 3, addTime = DateTime.Now.AddDays(1) });
                var result = db.Insertable(dataList).ExecuteCommand();//用例代码
            }

            var defaultTime = new DateTime(1900, 1, 1);
            var nowTime = DateTime.Now;

            var iQueryAble = db.Queryable<UintTest001>()
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
                           
                            .InnerJoin<UintTest001>((i, t) => i.group == t.group)
                            .Where((i,t)=>t.addTime < nowTime.AddDays(1))
                            .ToList();
            var json = @"
[{""ConditionalList"":[{""Key"":-1,""Value"":{""FieldName"":""id"",""FieldValue"":1517454440779616256,""ConditionalType"":0,""CSharpTypeName"":""long""}},{""Key"":0,""Value"":{""FieldName"":""mobile_phone"",""FieldValue"":""13554067074"",""ConditionalType"":0,""CSharpTypeName"":""string""}},{""Key"":0,""Value"":{""ConditionalList"":[{""Key"":-1,""Value"":{""FieldName"":""template_id"",""FieldValue"":""1374856"",""ConditionalType"":0,""CSharpTypeName"":""long""}},{""Key"":0,""Value"":{""FieldName"":""send_content"",""FieldValue"":""1"",""ConditionalType"":1,""CSharpTypeName"":""string""}}]}}]}]
";

            var json2 = @"
[{""ConditionalList"":[{""Key"":-1,""Value"":{""FieldName"":""id"",""FieldValue"":1517454440779616256,""ConditionalType"":0}},{""Key"":0,""Value"":{""FieldName"":""mobile_phone"",""FieldValue"":""13554067074"",""ConditionalType"":0,""CSharpTypeName"":""string""}},{""Key"":0,""Value"":{""ConditionalList"":[{""Key"":-1,""Value"":{""FieldName"":""template_id"",""FieldValue"":""1374856"",""ConditionalType"":0}},{""Key"":0,""Value"":{""FieldName"":""send_content"",""FieldValue"":""1"",""ConditionalType"":1,""CSharpTypeName"":""string""}}]}}]}]
";
            var cons=db.Utilities.JsonToConditionalModels(json);
            var cons2 = db.Utilities.JsonToConditionalModels(json2);

 
            var res21 = db.Queryable<UintTest001>()
                            .InnerJoin<UintTest001>((i, t) => i.group == t.group)
                            .Select((i, t) => new Test001_Ext
                            {
                                id = SqlFunc.Subqueryable<UintTest001>()
                                .Where(s => s.id == 1)
                                .Select(s =>s.id*i.id)
                            }).ToList();
            var res22 = db.Queryable<UintTest001>()
                .InnerJoin<UintTest001>((i, t) => i.group == t.group)
                .Select((i, t) => new Test001_Ext
                {
                    id = SqlFunc.Subqueryable<UintTest001>()
                    .Where(s => s.id == 1)
                    .Select(s => s.id * s.id)
                }).ToList();
            var  res23 = db.Queryable<UintTest001>()
                            .InnerJoin<UintTest001>((i, t) => i.group == t.group)
                            .Select((i, t) => new Test001_Ext
                            {
                                 id = SqlFunc.Subqueryable<UintTest001>()
                                .Where(s => s.id == 1)
                                .Select(s => SqlFunc.ToInt32(SqlFunc.AggregateSum(SqlFunc.DateDiff(DateType.Second, s.addTime, s.addTime)) / 3600.0))
                            }).ToList();

        }
        //建类
        public class UintTest001
        {
            public int id { get; set; }

            public int group { get; set; }

            public DateTime addTime { get; set; }
        }

        public class Test001_Ext : UintTest001
        {
            public DateTime lastTime { get; set; }
            public DateTime lastTime2 { get;   set; }
        }
    }
}
