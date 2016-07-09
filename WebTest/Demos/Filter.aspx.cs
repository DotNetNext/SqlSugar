using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SqlSugar;
using Models;

namespace WebTest.Demo
{
    /// <summary>
    /// 过滤器
    /// </summary>
    public partial class Filter : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            using (SqlSugarClient db = SugarDaoFilter.GetInstance())//开启数据库连接
            {
                //设置走哪个过滤器
                db.CurrentFilterKey = "role";

                //queryable
                var list = db.Queryable<Student>().ToList(); //通过全局过滤器对需要权限验证的数据进行过滤
                //相当于db.Queryable<Student>().Where("id=@id",new{id=1})


                //sqlable
                var list2 = db.Sqlable().From<Student>("s").SelectToList<Student>("*");
                //同上

                //sqlQuery
                var list3 = db.SqlQuery<Student>("select * from Student");
                //同上
            }
        }
    }
    /// <summary>
    /// 扩展SqlSugarClient
    /// </summary>
    public class SugarDaoFilter
    {
        //禁止实例化
        private SugarDaoFilter()
        {

        }
        /// <summary>
        /// 页面所需要的过滤函数
        /// </summary>
        private static Dictionary<string, Func<KeyValueObj>> _filterParas = new Dictionary<string, Func<KeyValueObj>>()
        {
          { "role",()=>{
                    return new KeyValueObj(){ Key=" id=@id" , Value=new{ id=1}};
               }
          },
          { "org",()=>{ 
                    return new KeyValueObj(){ Key=" orgId=@orgId" , Value=new{ orgId=1}};
              }
          },
        };
        public static SqlSugarClient GetInstance()
        {
            string connection = System.Configuration.ConfigurationManager.ConnectionStrings[@"sqlConn"].ToString(); //这里可以动态根据cookies或session实现多库切换
            var reval = new SqlSugarClient(connection);
            reval.SetFilterFilterParas(_filterParas);
            return reval;
        }
    }
}