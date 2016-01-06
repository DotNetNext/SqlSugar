using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SqlSugar;
using WebTest.Dao;
using Models;

namespace WebTest.NewDemo
{
    public partial class isGetPagePara : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //请在页面加上参数id=1

            using (SqlSugarClient db = SugarDao.GetInstance())//开启数据库连接
            {

                db.IsGetPageParas = true;//使用无参模式直接将Requst中的ID传给@id无需在代码中写出来

                var list = db.Queryable<Student>().Where("id=@id").ToList();
                //以前写法
                //var xx = db.Queryable<Student>().Where("id=@id", new { id=Request["id"] }).ToList();
            }
        }
    }
}