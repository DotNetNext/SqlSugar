using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SyntacticSugar;
using SqlSugar;
using Dapper;
using System.Data.SqlClient;
namespace WebTest.Select
{
    public partial class OneWhere : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            PerformanceTest pt = new PerformanceTest();
            pt.SetCount(10000);
            using (SqlSugarClient db = new SqlSugarClient(System.Configuration.ConfigurationManager.ConnectionStrings["sqlConn"].ToString()))
            {


                //ado.GetDataTable
                pt.Execute(i =>
                {
                    db.GetDataTable("select * from Student WHERE ID>@id", new SqlParameter("@id", i));

                }, m => { }, "ado.DateTable 纯SQL写法");


                //dapper
                var conn = db.GetConnection();
                pt.Execute(i =>
                {
                    conn.Query<Models.Student>("select * from Student where id>@id", new { id = i}).ToList();

                }, m => { }, "dapper 纯SQL写法");

                //sqlSugar
                pt.Execute(i =>
                {
                    db.Queryable<Models.Student>().Where(c => c.id == i).ToList();

                }, m => { }, "sqlSugar 拉姆达");


            }

            using (WebTest.TestLib.SqlSugarTestEntities db = new TestLib.SqlSugarTestEntities())
            {
                //EF
                pt.Execute(i =>
                {
                    db.Student.Where(c => c.id == i).ToList();

                }, m => { }, "EF4.0+sql05 拉姆达");
            }
            GridView gv = new GridView();
            gv.DataSource = pt.GetChartSource();
            gv.DataBind();
            Form.Controls.Add(gv);
        }
    }
}