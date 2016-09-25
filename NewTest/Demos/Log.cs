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
                var a2 = db.Queryable<Student>().OrderBy(it=>it.id).ToList();

               var logList=db.LogList; //可以查看当前操作域的所有 SQL记录

            }
        }

        /// <summary>
        /// SqlSugar
        /// </summary>
        public class SugarDemoDao
        {

            public static SqlSugarClient GetInstance()
            {
                var db= new SqlSugarClient(SugarDao.ConnectionString);
                db.LogEventStarting = (sql, pars) =>
                {
                    db.LogList.Add("starting:"+sql+"\r\n"+pars);//也可以直接写入数据库和文件,
                };

                db.LogEventCompleted = (sql, pars) =>
                {
                    db.LogList.Add("completed:"+sql + "\r\n" + pars);//也可以直接写入数据库和文件,
                };
                return db;
            }
        }
    }
}
