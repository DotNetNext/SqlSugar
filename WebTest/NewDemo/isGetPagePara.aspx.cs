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

                //获取页面所有参数到键值对
                //var kvs= SqlSugarTool.GetParameterDictionary();
          
                //以前写法
                //var xx = db.Queryable<Student>().Where("id=@id", new { id=Request["id"] }).ToList();
            }
        }

       
        public void OldMethod()
        {
            //请在页面加上参数id=1

            using (SqlSugarClient db = SugarDao.GetInstance())//开启数据库连接
            {

                db.IsGetPageParas = true;//使用无参模式直接将Requst中的ID传给@id无需在代码中写出来
                var kvs = SqlSugarTool.GetParameterDictionary();//获取QqueryString和Form参数集合
                var list = db.Queryable<Student>();

                if (!string.IsNullOrEmpty(Request["id"]))
                {
                    list = list.Where(it => it.id == Convert.ToInt32(kvs["name"]));
                }
                //if (!string.IsNullOrEmpty(Request["id"]))
                //{
                //    list = list.Where(it => it.Sex == Convert.ToInt32(kvs["Sex"]));
                //}
                //if (!string.IsNullOrEmpty(Request["id"]))
                //{
                //    list = list.Where(it => it.xxx == Convert.ToInt32(kvs["xxx"]));
                //}
                //if (!string.IsNullOrEmpty(Request["id"]))
                //{
                //    list = list = list.Where(it => it.xxxx == Convert.ToInt32(kvs["xxxxx"]));
                //}

                //获取页面所有参数到键值对
                //var kvs= SqlSugarTool.GetParameterDictionary();

               
            }
        }

        public void NewMethod()
        {
            //请在页面加上参数id=1

            using (SqlSugarClient db = SugarDao.GetInstance())//开启数据库连接
            {

                db.IsGetPageParas = true;//使用无参模式直接将Requst中的ID传给@id无需在代码中写出来
                var kvs = SqlSugarTool.GetParameterDictionary(true); //true不为空的所有参数
                var list = db.Queryable<Student>();
                foreach (var kv in kvs) {
                    list = list.Where(string.Format("{0}=@{0}",kv.Key));
                }
               
 
            }
        }
    }
}