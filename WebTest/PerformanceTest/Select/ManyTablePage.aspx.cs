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
    public partial class ManyTablePage : System.Web.UI.Page
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
                    db.GetDataTable(@"select * from (select  s.*,row_number() over (order by s.id asc,s.name desc) r from dbo.Student s inner join dbo.School sc on sc.id=s.sch_id 
left join dbo.Subject sb on sb.sid=s.id where  s.id>@id1 and  s.id>@id2) t where  t.r between @b and @e", new SqlParameter("@id1", "1"), new SqlParameter("@id2", "2"), new SqlParameter("@b", "11"), new SqlParameter("@e", "20"));

                }, m => { }, "ado.DateTable 纯SQL写法");


                //dapper
                var conn = db.GetConnection();
                pt.Execute(i =>
                {
                    conn.Query<Models.Student>(@"select * from (select s.*,row_number() over (order by s.id asc,s.name desc) r from dbo.Student s inner join dbo.School sc on sc.id=s.sch_id 
left join dbo.Subject sb on sb.sid=s.id where  s.id>@id1 and  s.id>@id2) t where   t.r between @b and @e", new { id1 = 1, id2 = 2, b = 11, e = 20 }).ToList();

                }, m => { }, "dapper 纯SQL写法");

                //sqlSugar
                pt.Execute(i =>
                {
                    db.Sqlable().From("Student", "s")
                        .Join("School", "sc", "sc.id", "s.sch_id", JoinType.INNER)
                        .Join("subject", "sb", "sb.sid", "s.id", JoinType.LEFT).Where("s.id>@id1").Where("s.id>@id2")
                        .SelectToPageList<Models.Student>("s.*", "s.id asc,s.name desc", 2, 10, new { id1=1,id2=2 });

                }, m => { }, "sqlSugar SQL语法糖");
            }
            using (WebTest.TestLib.SqlSugarTestEntities db = new TestLib.SqlSugarTestEntities())
            {
                //EF
                pt.Execute(i =>
                {
                    var reval = (from s in db.Student
                                join sc in db.School on s.sch_id equals sc.id
                                join sb in db.Subject on s.id equals sb.sid
                                into ssb
                                from sb2 in ssb.DefaultIfEmpty()
                                select new {
                                s.id,
                                s.name,
                                s.sch_id,
                                s.sex
                                }).Where(c=>c.id>1).Where(c=>c.id>2).OrderBy(c=>c.id).ThenByDescending(c=>c.name).Skip(10).Take(10).ToList();




                }, m => { }, "EF4.0+sql05  LINQ TO SQL");
            }
            GridView gv = new GridView();
            gv.DataSource = pt.GetChartSource();
            gv.DataBind();
            Form.Controls.Add(gv);
        }
    }
}