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
    /// <summary>
    /// 多语言支持
    /// </summary>
    public partial class Language : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            using (SqlSugarClient db = SugarDao.GetInstance())//开启数据库连接
            {
                db.Language = new PubModel.Language()
                {
                     LanguageValue=2,//多语言的值一般从COOKIES或SESSION取
                     Suffix="en"//多语言后缀同上
                };
                //给上面赋值后下面的程就可以使用了
                int lanId=db.Language.LanguageValue;
                var list = db.Queryable<LanguageTest>().Where(it => it.LanguageId == lanId).ToList(); 


                /****************************多语言视图才是最大的问题***********************************/


                //注意视图里里怎么办呢？视图里面的JOIN用到语言表怎么处理呢
                //我们就写一个简单的视图作为例子,代码如下
                /*create view V_LanguageTest
                    as
                    select * from LanguageTest where LanguageId=1
                 */

                //下面这代码写到 application_start  不需要重复执行
                LanguageHelper.UpdateView(db.Language, db);
                //执行完上面的代码会创建把所有带LanguageId=1的视图全部生成其它语言的视图
                //现在数据库就有了 V_LanguageTest_$_en

                // V_LanguageTest_$_en结构如下
                /*create view V_LanguageTest_$_en
                 as
                 select * from LanguageTest where LanguageId=2
                 */

                //V_LanguageTest__$_en 是我SqlSugar自动帮你创建的 当视图发生变化需要重新执行 LanguageHelper.UpdateView(db.Language, db);

                var list2=db.Queryable<V_LanguageTest>().ToList();
                //生成的Sql等于 select * from V_LanguageTest_$_en 

                 
                db.Language.LanguageValue = 1;//我们在把LanguageValue改成1
                db.Language.Suffix = null;//后缀清空
                var list3 = db.Queryable<V_LanguageTest>().ToList();

                //下生成的Sql等于 select * from V_LanguageTest

                //注意当 Suffix为null时使用的原始视图

                //自定义视图替换规则请看下面两个参数
               //db.Language.ReplaceViewStringKey 默认值为LanguageId=1
               //db.Language.ReplaceViewStringValue 默认值为LanguageId = {0}
                
            }
        }
    }
}