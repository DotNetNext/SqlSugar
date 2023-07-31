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
    public class Demo0_SqlSugarClient
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
            db.Ado.ExecuteCommand("CREATE DATABASE IF NOT EXISTS power WAL_RETENTION_PERIOD 3600");

            //建超级表
            db.Ado.ExecuteCommand("CREATE STABLE IF NOT EXISTS  MyTable (ts TIMESTAMP, current FLOAT, voltage INT, phase FLOAT) TAGS (location BINARY(64), groupId INT)");

            //创建子表
            db.Ado.ExecuteCommand(@"create table IF NOT EXISTS  MyTable01 using MyTable tags('California.SanFrancisco',1)");

        
            //insert sql
            //db.Ado.ExecuteCommand(insrtSql);

            //查询子表
            var dt = db.Ado.GetDataTable("select * from MyTable01");

            //查询超级表
            var dt2 = db.Ado.GetDataTable("select * from MyTable");

            db.Insertable(new MyTable01()
            {
                ts = DateTime.Now,
                current = Convert.ToSingle(1.1),
                groupId = 1,
                location = "aa",
                phase = Convert.ToSingle(1.1),
                voltage = 11
            }).ExecuteCommand();

            var list = db.Queryable<MyTable01>().ToList();
        }

        public class MyTable01
        {
            public DateTime ts { get; set; }
            public float current { get; set; }
            public int voltage { get; set; }
            public float phase { get; set; }
            [SugarColumn(IsOnlyIgnoreInsert =true,IsOnlyIgnoreUpdate =true)]
            public string location { get; set; }
            [SugarColumn(IsOnlyIgnoreInsert = true, IsOnlyIgnoreUpdate = true)]
            public int groupId { get; set; }
        }


    }

}
