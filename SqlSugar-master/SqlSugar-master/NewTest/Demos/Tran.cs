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
    //事务
    public class Tran : IDemos
    {

        public void Init()
        {
            Console.WriteLine("启动Tran.Init");
            using (SqlSugarClient db = SugarDao.GetInstance())//开启数据库连接
            {
                db.IsNoLock = true;//启用无锁查询
                db.CommandTimeOut = 30000;//设置超时时间
                try
                {
                    db.BeginTran();//开启事务
                    //db.BeginTran(IsolationLevel.ReadCommitted);+3重载可以设置事世隔离级别

                    db.CommitTran();//提交事务
                }
                catch (Exception)
                {
                    db.RollbackTran();//回滚事务
                    throw;
                }
            }
        }
    }
}
