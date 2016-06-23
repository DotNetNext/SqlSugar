using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SqlSugar;
using Models;

namespace WebTest.NewDemo
{
    public partial class DisableUpdateColumns : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            using (SqlSugarClient db = SugarDaoFilter.GetInstance())//开启数据库连接
            {
                db.DisableUpdateColumns = new string[] { "CreateTime" };

                TestUpdateColumns updObj = new TestUpdateColumns()
                {
                    VGUID = Guid.Parse("542b5a27-6984-47c7-a8ee-359e483c8470"),
                    Name = "xx",
                    Name2 = "xx2",
                    IdentityField = 0,
                    CreateTime = null
                };

                //CreateTime和IdentityField(为非主键自动添长列)将不会被更新
                db.Update(updObj);
                //以前实现这种更新需要用指定列的方式实现，现在就简单多了。
            }
        }
    }
}