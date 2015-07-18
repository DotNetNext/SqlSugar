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
namespace WebTest.add
{
    public partial class Add : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            PerformanceTest pt = new PerformanceTest();
            pt.SetCount(10000);//设置循环次数
            using (SqlSugarClient db = new SqlSugarClient(System.Configuration.ConfigurationManager.ConnectionStrings["sqlConn"].ToString()))
            {
                var sqlconn = db.GetConnection();

                TuncateTable(db);//清空

                //sqlSuagr
                pt.Execute(i =>
                {
                    db.Insert<Models.InsertTest>(GetData());

                }, m => { }, "sqlSuagr");




          
                TuncateTable(db);


                //ef4.0+Sql05
                using (TestLib.SqlSugarTestEntities ef = new TestLib.SqlSugarTestEntities())
                {
                    pt.Execute(i =>
                    {
                        ef.InsertTests.AddObject(GetEFData());
                        ef.SaveChanges();

                    }, m => { }, "ef4.0+Sql05");
                }


                TuncateTable(db);

                //dapper
                pt.Execute(i =>
                {
                    sqlconn.Execute("insert into InsertTest (v1,v2,v3,int1,d1,txt ) values(@v1,@v2,@v3,@int1,@d1,@txt);select @@identity;", GetData());

                }, m => { }, "dapper");


                TuncateTable(db);

                //ado.net
                pt.Execute(i =>
                {
                    db.GetScalar("insert into InsertTest (v1,v2,v3,int1,d1,txt ) values(@v1,@v2,@v3,@int1,@d1,@txt);select @@identity;", new SqlParameter[]{
                   new SqlParameter("@d1", DateTime.Now),
                   new SqlParameter("@int1",11),
                   new SqlParameter("@txt","哈另一个txt"),
                   new SqlParameter("@v1","v1sdfasdas"),
                   new SqlParameter("@v2","v2sdfasfas"),
                   new SqlParameter("@v3","v3adfasdf"),
                   new SqlParameter("@id",1)
                    });

                }, m => { }, "ado.net");


            }

            //输出测试页面
            GridView gv = new GridView();
            gv.DataSource = pt.GetChartSource();
            gv.DataBind();
            Form.Controls.Add(gv);



        }

        //清空数据表
        private static void TuncateTable(SqlSugarClient db)
        {
            db.ExecuteCommand("truncate table InsertTest");
            System.Threading.Thread.Sleep(1000);
        }

        //ef entity data
        public TestLib.InsertTest GetEFData()
        {
            WebTest.TestLib.InsertTest test = new TestLib.InsertTest()
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