using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebTest.Dao;
using Models;
using SqlSugar;
namespace WebTest.Demos
{
    /// <summary>
    /// 新功能的测试页面，还在开发当中
    /// </summary>
    public partial class newFunctionTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            using (var db = SugarDao.GetInstance())
            {
                int id1 = 1;
                db.Queryable<Student>().InnerJoin<Student, School>((s1, s2) => s1.id == s2.id && s2.id > id1 && id1 < s1.id);
            }
        }
    }
}