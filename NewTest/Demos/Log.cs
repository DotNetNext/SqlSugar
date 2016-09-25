using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NewTest.Dao;
using Models;
using System.Data.SqlClient;
using SqlSugar;

namespace NewTest.Demos
{
    //日志记录功能
    public class Log : IDemos
    {
       
        public void Init()
        {
            Console.WriteLine("启动Log.Init");
            using (var db = SugarDemoDao.GetInstance())
            {

                var a1 = db.Queryable<Student>().Where(it => it.id == 1).ToList();
                var a2 = db.Queryable<Student>().OrderBy(it => it.id).ToList();
            }
        }

        public class SugarConfigs
        {
            public static Action<string, string> LogEventStarting = (sql, pars) =>
            {
                Console.WriteLine("starting:" + sql + " " + pars);
                
                using (var db = SugarDemoDao.GetInstance())
                {
                    //日志记录件事件里面用到数据库操作 IsEnableLogEvent一定要为false否则将引起死循环，并且要新开一个数据实例 像我这样写就没问题。
                    db.IsEnableLogEvent = false;
                    db.ExecuteCommand("select 1");
                }
            };
            public static Action<string, string> LogEventCompleted = (sql, pars) =>
            {
                Console.WriteLine("completed:" + sql + " " + pars);
            };
        }

        /// <summary>
        /// SqlSugar
        /// </summary>
        public class SugarDemoDao
        {

            public static SqlSugarClient GetInstance()
            {
                var db = new SqlSugarClient(SugarDao.ConnectionString);
                db.IsEnableLogEvent = true;//启用日志事件
                db.LogEventStarting = SugarConfigs.LogEventStarting;
                db.LogEventCompleted = SugarConfigs.LogEventCompleted;
                return db;
            }
        }
    }
}
