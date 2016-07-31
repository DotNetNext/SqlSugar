using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SqlSugar;
using WebTest.Dao;
using Models;
namespace WebTest.Demo
{
    /// <summary>
    /// 原始操作数据库函数
    /// </summary>
    public partial class Ado : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            using (SqlSugarClient db = SugarDao.GetInstance())//开启数据库连接
            {
               var r1= db.GetDataTable("select * from student");
               var r2= db.GetSingle<Student>("select top 1 * from student");
               var r3 = db.GetScalar("select  count(1) from student");
               var r4 = db.GetReader("select  count(1) from student");
               r4.Dispose();
               var r5 = db.GetString("select  top 1 name from student");
               var r6 = db.ExecuteCommand("select 1");
            }
        }
    }
}