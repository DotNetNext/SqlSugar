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
    public partial class WherePage : System.Web.UI.Page
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
                    db.GetDataTable("select * from(select *,row_number() over(order by id) as r from Student where id>@id) t where t.r between @b and @e", new SqlParameter("@id","0"), new SqlParameter("@b", 11), new SqlParameter("@e", 20));

                }, m => { }, "ado.DateTable  纯SQL写法");


                //dapper
                var conn = db.GetConnection();
                pt.Execute(i =>
                {
                    conn.Query<Models.Student>("select * from(select *,row_number() over(order by id) as r from Student where id>@id) t where t.r between @b and @e", new { id = 0,b=11,e=20 }).ToList();

                }, m => { }, "dapper  纯SQL写法");

                //sqlSugar
                pt.Execute(i =>
                {
                    db.Queryable<Models.Student>().Where(c => c.id > 0).OrderBy("id").ToPageList(2, 10);

                }, m => { }, "sqlSugar 拉姆达");


            }

            using (WebTest.TestLib.SqlSugarTestEntities db = new TestLib.SqlSugarTestEntities())
            {
                //EF
                pt.Execute(i =>
                {
                    db.Student.Where(c => c.id == 2).OrderBy(c=>c.id).Skip(10).Take(10).ToList();

                }, m => { }, "EF4.0+sql05 拉姆达");
            }
            GridView gv = new GridView();
            gv.DataSource = pt.GetChartSource();
            gv.DataBind();
            Form.Controls.Add(gv);
        }
    }
}