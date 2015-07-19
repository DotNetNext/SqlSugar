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
    public partial class Del : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
            PerformanceTest pt = new PerformanceTest();
            pt.SetCount(1000);//设置循环次数
            using (SqlSugarClient db = new SqlSugarClient(System.Configuration.ConfigurationManager.ConnectionStrings["sqlConn"].ToString()))
            {

                TuncateTable(db);
                AddTest(pt, db);

                //sqlSugar
                pt.Execute(i =>
                {
                    db.Delete<Models.InsertTest>(it=>it.id==i);

                }, m => { }, "sqlSugar");


                TuncateTable(db);
                AddTest(pt, db);

                //ado.net
                pt.Execute(i =>
                {
                    db.ExecuteCommand("delete InsertTest where id=@id", new SqlParameter("@id", i));

                }, m => { }, "ado.net");


                TuncateTable(db);
                AddTest(pt, db);

                //dapper
                pt.Execute(i =>
                {
                    db.GetConnection().Execute("delete InsertTest where id=@id", new { id=i});

                }, m => { }, "dapper");
            }

            //输出测试页面
            GridView gv = new GridView();
            gv.DataSource = pt.GetChartSource();
            gv.DataBind();
            Form.Controls.Add(gv);

        }
        public void AddTest(PerformanceTest pt, SqlSugarClient db)
        {
            //sqlSuagr
            pt.Execute(i =>
            {
                db.Insert<Models.InsertTest>(GetData());

            }, m => { }, "添加记录");

        }
        //清空数据表
        private static void TuncateTable(SqlSugarClient db)
        {
            db.ExecuteCommand("truncate table InsertTest");
            System.Threading.Thread.Sleep(1000);
        }

        //entity data
        public Models.InsertTest GetData()
        {
            Models.InsertTest test = new Models.InsertTest()
            {
                d1 = DateTime.Now,
                int1 = 11,
                txt = "哈另一个txt",
                v1 = "v1sdfasdas",
                v2 = "v2sdfasfas",
                v3 = "v3adfasdf",
                id = 1
            };
            return test;
        }

    }
}