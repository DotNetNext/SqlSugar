using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SqlSugar;
using SyntacticSugar;

namespace WebTest.add
{
    public partial class Update : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            PerformanceTest pt = new PerformanceTest();
            pt.SetCount(10000);//设置循环次数
            using (SqlSugarClient db = new SqlSugarClient(System.Configuration.ConfigurationManager.ConnectionStrings["sqlConn"].ToString()))
            {
                pt.Execute(i =>
                {
                    //更新10000次
                    db.Update<Models.InsertTest>(new { v1="newv11", v2="newv22", v3="newv33" }, it => it.id ==1|| it.id>1000);

                }, m => { }, "sqlSugar");

            }

            //输出测试页面
            GridView gv = new GridView();
            gv.DataSource = pt.GetChartSource();
            gv.DataBind();
            Form.Controls.Add(gv);
        }
    }
}