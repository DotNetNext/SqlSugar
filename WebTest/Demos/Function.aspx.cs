using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SqlSugar;
namespace WebTest.Demo
{
    /// <summary>
    /// 一些公开函数
    /// </summary>
    public partial class Function : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            //ToJoinSqlInVal
            var par = new string[] { "a", "c", "3" };
            var ids = par.ToJoinSqlInVal();//将数组转成 'a','c','3'  有防SQL注入处理


            //ToSqlValue
            try
            {
                var par2 = "a'";
                var newpar2 = par2.ToSqlValue();//对字符串防注入处理
            }
            catch (Exception ex)
            {

                Response.Write(ex.Message);
            }

            //SqlLikeWordEncode 处理LIKE特殊字符
            var likestr = SqlSugarTool.SqlLikeWordEncode("[%%%");


            //GetParameterArray 获取页面参数所有参数的键和值
            var pars = SqlSugarTool.GetParameterArray();


            //将匿名对象转成SqlParameter
            var par3= SqlSugarTool.GetParameters(new { id=1});

        }
    }
}