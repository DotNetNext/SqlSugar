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
    /// 偷懒我就不给参数赋值
    /// </summary>
    public partial class NoParameter : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                

                using (SqlSugarClient db = SugarDao.GetInstance())//开启数据库连接
                {

                    db.IsGetPageParas = true;//使用无参模式直接将Requst中的ID传给@id无需在代码中写出来

                    var list = db.Queryable<Student>().Where("id=@id").ToList();

                    //sqlable queryable sqlquery通用

                    //获取页面所有参数到键值对
                    //var kvs= SqlSugarTool.GetParameterDictionary();

                    //以前写法
                    //var xx = db.Queryable<Student>().Where("id=@id", new { id=Request["id"] }).ToList();
                }
            }
            catch (Exception)
            {
                
                throw new Exception("请在当前页面URL地址后面加上参数?id=1");
            }
        }
    }
}