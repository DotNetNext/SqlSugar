using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebTest.Dao;
using Models;
using SqlSugar;
namespace WebTest.Demo
{
    /// <summary>
    /// 更新
    /// </summary>
    public partial class Update : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int id = 11;
            using (var db = SugarDao.GetInstance())
            {
                //指定列更新
                db.Update<School>(new { name = "蓝翔14" }, it => it.id == 14);
                db.Update<School, int>(new { name = "蓝翔11 23 12", areaId=2 }, 11, 23, 12);
                db.Update<School, string>(new { name = "蓝翔2" }, new string[] { "11", "21" });
                db.Update<School>(new { name = "蓝翔2" }, it => it.id == id);

   

                //支持字段指定列更新，适合动态权限
                var dic =new Dictionary<string, string>();
                dic.Add("name","第十三条");
                dic.Add("areaId", "1");
                db.Update<School,int>(dic, 13);
                db.Update<School>(dic, it => it.id == 13);



                //整个实体更新
                db.Update(new School { id = 16, name = "蓝翔16", AreaId=1 });
                db.Update<School>(new School { id = id, name = "蓝翔18", AreaId=2 }, it => it.id == 18);
                db.Update<School>(new School() { id = 11, name = "xx" });

                //设置不更新列
                db.DisableUpdateColumns = new string[] { "CreateTime" };//设置CreateTime不更新

                TestUpdateColumns updObj = new TestUpdateColumns()
                {
                    VGUID = Guid.Parse("542b5a27-6984-47c7-a8ee-359e483c8470"),
                    Name = "xx",
                    Name2 = "xx2",
                    IdentityField = 0,
                    CreateTime = null
                };

                //CreateTime将不会被更新
                db.Update(updObj);
                //以前实现这种更新需要用指定列的方式实现，现在就简单多了。
               
            }

        }
    }
}