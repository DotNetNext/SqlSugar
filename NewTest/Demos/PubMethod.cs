using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NewTest.Dao;
using Models;
using System.Data.SqlClient;
using SqlSugar;
namespace NewTest.Demos
{
    //公用函数
    public class PubMethod:IDemos
    {

        public void Init()
        {
            Console.WriteLine("启动PubMethod.Init");
            using (var db = SugarDao.GetInstance())
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

                     Console.WriteLine(ex.Message);
                }

                //SqlLikeWordEncode 处理LIKE特殊字符
                var likestr = SqlSugarTool.SqlLikeWordEncode("[%%%");


                //GetParameterArray 获取页面参数所有参数的键和值
                //var pars = SqlSugarTool.GetParameterArray(); WEB项目中使用


                //将匿名对象转成SqlParameter
                var par3 = SqlSugarTool.GetParameters(new { id = 1 });


                //用于生成多语言的视图
                //LanguageHelper.UpdateView()


                //数组的操作,因为好多SqlSugar配置都是数组，所以提供更好的语法简化代码。
                db.DisableUpdateColumns = new string[] { "id" };
                db.DisableUpdateColumns.ArrayAdd("name");
                db.DisableUpdateColumns.ArrayAdd("name","sex","student");
                db.DisableUpdateColumns.ArrayRemove("id");
                db.DisableUpdateColumns.ArrayWhere(it => it == "name");
            }
        }
    }
}
