using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
using TDengineDriver; 

namespace OrmTest
{
    public class ORMTest
    {

        public static void Init()
        {
            var db = new SqlSugarClient(new ConnectionConfig()
            {
                DbType = SqlSugar.DbType.TDengine,
                ConnectionString = Config.ConnectionString,
                IsAutoCloseConnection = true,
                AopEvents = new AopEvents
                {
                    OnLogExecuting = (sql, p) =>
                    {
                        Console.WriteLine(UtilMethods.GetNativeSql(sql,p)); 
                    }
                }
            });

            //建库
            db.DbMaintenance.CreateDatabase();

            //建超级表
            db.Ado.ExecuteCommand("CREATE STABLE IF NOT EXISTS  St01 (ts TIMESTAMP, current FLOAT, voltage INT, phase FLOAT, isdelete BOOL, name BINARY(64)) TAGS (location BINARY(64), groupId INT)");

            //创建子表
            db.Ado.ExecuteCommand(@"create table IF NOT EXISTS  MyTable02 using St01 tags('California.SanFrancisco',1)");


            //查询子表
            var dt = db.Ado.GetDataTable("select * from MyTable02 ");


            //插入单条子表
            db.Insertable(new MyTable02()
            {
                ts = DateTime.Now,
                current = Convert.ToSingle(1.1),
                groupId = 1,
                isdelete = true,
                name = "haha",
                location = "aa",
                phase = Convert.ToSingle(1.2),
                voltage = 11
            }).ExecuteCommand();

            //批量插入子表
            db.Insertable(GetInsertDatas()).ExecuteCommand();


            //查询子表(主表字段也能查出来)
            var list = db.Queryable<MyTable02>().OrderBy(it => it.ts).ToList();

            //条件查询
            var list2 = db.Queryable<MyTable02>().Where(it => it.name== "测试2").ToList();
            var list22 = db.Queryable<MyTable02>().Where(it => it.voltage == 222).ToList();
            var list222 = db.Queryable<MyTable02>().Where(it => it.phase == 1.2).ToList();
            var list2222 = db.Queryable<MyTable02>().Where(it => it.isdelete==true).ToList();

            //模糊查询
            var list3 = db.Queryable<MyTable02>().Where(it => it.name.Contains("a")).ToList();


            //分页
            var Count = 0;
            var list4 = db.Queryable<MyTable02>().Where(it => it.voltage == 111)
                .ToPageList(1, 2, ref Count);

            //删除子表
            var ts = list.First().ts;
            var de = DateTime.Now.AddYears(-1);
            var count = db.Deleteable<MyTable02>().Where(it => it.ts > de).ExecuteCommand();
        }

        private static List<MyTable02> GetInsertDatas()
        {
            return new List<MyTable02>() {
            new MyTable02()
            {
                ts = DateTime.Now.AddDays(-1),
                current = Convert.ToSingle(1.1),
                groupId = 1,
                isdelete = false,
                name = "测试1",
                location = "false",
                phase = Convert.ToSingle(1.1),
                voltage = 222
            },
             new MyTable02()
            {
                ts = DateTime.Now.AddDays(-2),
                current = Convert.ToSingle(1.1),
                groupId = 1,
                isdelete = false,
                name = "测试2",
                location = "false",
                phase = Convert.ToSingle(1.1),
                voltage = 222
            },
                new MyTable02()
            {
                ts = DateTime.Now,
                current = Convert.ToSingle(1.1),
                groupId = 1,
                isdelete = true,
                name = "测试3",
                location = "true",
                phase = Convert.ToSingle(1.1),
                voltage = 111
            }
            };
        }

        public class MyTable02
        {
            [SugarColumn(IsPrimaryKey =true)]
            public DateTime ts { get; set; }
            public float current { get; set; }
            public bool isdelete { get; set; }
            public string name { get; set; }
            public int voltage { get; set; }
            public float phase { get; set; }
            [SugarColumn(IsOnlyIgnoreInsert =true,IsOnlyIgnoreUpdate =true)]
            public string location { get; set; }
            [SugarColumn(IsOnlyIgnoreInsert = true, IsOnlyIgnoreUpdate = true)]
            public int groupId { get; set; }
        } 

    }

}
