using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SqlSugar;
using Models;

namespace WebTest.Demos
{/// <summary>
    /// 过滤器 (查询行过滤加查询列过滤)
    /// </summary>
    public partial class Filter2 : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            using (SqlSugarClient db = SugarDaoFilter.GetInstance())//开启数据库连接
            {
                //设置走哪个过滤器
                db.CurrentFilterKey = "role";


                //queryable
                var list = db.Queryable<Student>().ToJson();


                //设置走哪个过滤器
                db.CurrentFilterKey = "org";


                //queryable
                var list2 = db.Queryable<Student>().ToJson();

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
        /// 页面所需要的过滤行
        /// </summary>
        private static Dictionary<string, Func<KeyValueObj>> _filterParas = new Dictionary<string, Func<KeyValueObj>>()
            {
              { "role",()=>{
                        return new KeyValueObj(){ Key=" id=@id" , Value=new{ id=1}};
                   }
              },
              { "org",()=>{
                  return new KeyValueObj() { Key = " id=@id", Value = new { id = 2 } };
                  }
              },
            };
        /// <summary>
        /// 页面所需要的过滤列
        /// </summary>
        private static Dictionary<string, List<string>> _filterColumns = new Dictionary<string, List<string>>()
            {
              { "role",new List<string>(){"id","name"}
              },
              { "org",new List<string>(){"name"}
              },
            };
        public static SqlSugarClient GetInstance()
        {
            string connection = System.Configuration.ConfigurationManager.ConnectionStrings[@"sqlConn"].ToString(); //这里可以动态根据cookies或session实现多库切换
            var reval = new SqlSugarClient(connection);

            //支持sqlable和queryable
            reval.SetFilterFilterParas(_filterParas);

            //列过滤只支持queryable
            reval.SetFilterFilterParas(_filterColumns);
            return reval;
        }
    }

}