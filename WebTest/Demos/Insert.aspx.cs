using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SqlSugar;
using WebTest.Dao;
using Models;

namespace WebTest.Demos
{
    public partial class Insert : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            using (SqlSugarClient db = SugarDao.GetInstance())//开启数据库连接
            {
                Student s = new Student()
                {
                    name = "张" + new Random().Next(1, int.MaxValue)
                };

                db.Insert(s); //插入一条记录 (有主键也好，没主键也好，有自增列也好都可以插进去)


                List<Student> list = new List<Student>()
                {
                     new Student()
                {
                     name="张"+new Random().Next(1,int.MaxValue)
                },
                 new Student()
                {
                     name="张"+new Random().Next(1,int.MaxValue)
                }
                };

                db.InsertRange(list); //批量插入
            }
        }
    }
}