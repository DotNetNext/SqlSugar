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
                        Console.WriteLine(sql);
                        Console.WriteLine(string.Join(",", p?.Select(it => it.ParameterName + ":" + it.Value)));
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
            var dt = db.Ado.GetDataTable("SHOW DATABASES");


            //插入子表
            db.Insertable(new MyTable02()
            {
                ts = DateTime.Now,
                current = Convert.ToSingle(1.1),
                groupId = 1,
                isdelete=true,
                name="haha",
                location = "aa",
                phase = Convert.ToSingle(1.1),
                voltage = 11
            }).ExecuteCommand();

            db.Insertable(new List<MyTable02>() {
            new MyTable02()
            {
                ts = DateTime.Now,
                current = Convert.ToSingle(1.1),
                groupId = 1,
                isdelete = true,
                name = "haha",
                location = "aa",
                phase = Convert.ToSingle(1.1),
                voltage = 11
            },
                new MyTable02()
            {
                ts = DateTime.Now,
                current = Convert.ToSingle(1.1),
                groupId = 1,
                isdelete = true,
                name = "haha",
                location = "aa",
                phase = Convert.ToSingle(1.1),
                voltage = 11
            }
            }).ExecuteCommand();


            //查询子表(主表字段也能查出来)
            var list = db.Queryable<MyTable02>().ToList();
              

            //删除子表
            var ts = list.First().ts;
            var count=db.Deleteable<MyTable02>().Where(it=>it.ts==ts).ExecuteCommand();
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
