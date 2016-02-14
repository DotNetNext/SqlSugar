using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SqlSugar;
using Models;
using WebTest.Dao;

namespace WebTest.NewDemo
{
    public partial class QueryableDifferentTableName : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            using (SqlSugarClient db = SugarDao.GetInstance())//开启数据库连接
            {
                var list = db.Queryable<Student>().ToList();
                //如果类名与数据库名不一致可以写在这样
                var list2 = db.Queryable<Student2>("Student").ToList();
            }
        }
    }
}