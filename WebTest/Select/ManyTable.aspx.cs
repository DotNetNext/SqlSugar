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
    public partial class ManyTable : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            PerformanceTest pt = new PerformanceTest();
            pt.SetCount(1000);
            using (SqlSugarClient db = new SqlSugarClient(System.Configuration.ConfigurationManager.ConnectionStrings["sqlConn"].ToString()))
            {


                //ado.GetDataTable
                pt.Execute(i =>
                {
                    db.GetDataTable(@"select * from dbo.Student s inner join dbo.School sc on sc.id=s.sch_id 
left join dbo.Subject sb on sb.sid=s.id");

                }, m => { }, "ado.DateTable 纯SQL写法");


                //dapper
                var conn = db.GetConnection();
                pt.Execute(i =>
                {
                    conn.Query<Models.Student>(@"select * from dbo.Student s inner join dbo.School sc on sc.id=s.sch_id 
left join dbo.Subject sb on sb.sid=s.id").ToList();

                }, m => { }, "dapper 纯SQL写法");

                //sqlSugar
                pt.Execute(i =>
                {
                    db.Sqlable().Form("Student", "s")
                        .Join("School","sc","sc.id","s.sch_id",JoinType.INNER)
                        .Join("subject","sb","sb.sid","s.id",JoinType.LEFT).SelectToList<Models.Student>("*");

                }, m => { }, "sqlSugar SQL语法糖");
                //sqlSugar
                pt.Execute(i =>
                {
                    db.SqlQuery<Models.Student>(@"select * from dbo.Student s inner join dbo.School sc on sc.id=s.sch_id 
left join dbo.Subject sb on sb.sid=s.id").ToList();

                }, m => { }, "sqlSugar 纯SQL写法");


            }

            using (WebTest.TestLib.SqlSugarTestEntities db = new TestLib.SqlSugarTestEntities())
            {
                //EF
                pt.Execute(i =>
                {
                    db.ExecuteStoreQuery<WebTest.TestLib.Student>(@"select * from dbo.Student s inner join dbo.School sc on sc.id=s.sch_id 
left join dbo.Subject sb on sb.sid=s.id");

                }, m => { }, "EF4.0+sql05 纯SQL写法");
            }
            GridView gv = new GridView();
            gv.DataSource = pt.GetChartSource();
            gv.DataBind();
            Form.Controls.Add(gv);
        }
    }
}