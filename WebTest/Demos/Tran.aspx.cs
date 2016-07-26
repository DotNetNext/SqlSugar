using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SqlSugar;
using WebTest.Dao;
using System.Data;
namespace WebTest.Demo
{
    /// <summary>
    /// 事务和锁
    /// </summary>
    public partial class Tran : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
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