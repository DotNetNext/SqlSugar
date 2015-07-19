using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SqlSugar;
using SyntacticSugar;
using System.Data.SqlClient;
using Dapper;
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
         

                //ef
                using (WebTest.TestLib.SqlSugarTestEntities ef = new TestLib.SqlSugarTestEntities())
                {
                    pt.Execute(i =>
                    {
                        var datas = ef.InsertTests.Where(c => c.id == 1 || c.id > 1000);
                        foreach (var r in datas)
                        {
                            r.v1 = "new11";
                            r.v2 = "newv22";
                            r.v3 = "new33";
                        }
                        ef.SaveChanges();


                    }, m => { }, "EF4.0+sql05 Linq语法");
                }

                //ado.net
                pt.Execute(i =>
                {

                    db.ExecuteCommand(@" UPDATE InsertTest SET  v1 =@v1  , v2 =@v2  , v3 =@v3   WHERE  1=1   and ((id =@id1) Or (id >@id1000))",new SqlParameter[]{
                      new SqlParameter("@v1","new11"),
                      new SqlParameter("@v2","new22"),
                      new SqlParameter("@v3","new33"),
                      new SqlParameter("@id1","1"),
                      new SqlParameter("@id1000","1000")
                    });

                }, m => { },"ado.net");

                //dapper
                var sqlConn=db.GetConnection();
                pt.Execute(i =>
                {

                    sqlConn.Execute(@" UPDATE InsertTest SET  v1 =@v1  , v2 =@v2  , v3 =@v3   WHERE  1=1   and ((id ='1') Or (id >'1000'))", 
                      new { v1 = "newv11", v2 = "newv22", v3 = "newv33",id1=1,id1000=1000 } );

                }, m => { }, "dapper");

                //sqlSugar
                pt.Execute(i =>
                {
                    //更新10000次
                    db.Update<Models.InsertTest>(new { v1 = "newv11", v2 = "newv22", v3 = "newv33" }, it => it.id == 1 || it.id > 1000);

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