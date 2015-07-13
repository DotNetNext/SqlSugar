using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SqlSugar;
using Models;

namespace WebTest
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string connStr = @"Server=(LocalDB)\v11.0; Integrated Security=true ;AttachDbFileName=" + Server.MapPath("~/App_Data/SqlSugar.mdf");
            using (SqlSugarClient db = new SqlSugarClient(connStr))
            {
                db.BeginTran();
                db.Sqlable.IsNoLock = true;


                try
                {
                    //生成数据库实体类 
                    //db.ClassGenerating.CreateClassFiles(db,Server.MapPath("~/Models"),"Models");

                    //根据表名生成class字符串
                    //Write(db.ClassGenerating.TableNameToClass(db,"student"));

                    //根据SQL语句生成class字符串
                    // Write(db.ClassGenerating.SqlToClass(db,"select top 1 * from student"));


                    //根据sql语句映射成实体
                    var School = db.SqlQuery<school>("select * from School");


                    //联表查询
                    /*db.Sqlable是一个SQL语句生成帮助类*/
                    string StudentViewSql = db.Sqlable.MappingTable<Student, school>("t1.sch_id=t2.id").Where("sex='男' order by t2.id").SelectToSql("t1.*,t2.name as school_name");
                    var studentView = db.SqlQuery<Student_View>(StudentViewSql);



                }
                catch (Exception)
                {

                    db.RollbackTran();
                }

            }
            ;
            //var xx = SqlTool.CreateMappingTable(20);
            Console.Read();
        }


        private void Write(string html)
        {
            Response.Write("<p>" + html + "</p>");
        }
    }

   

}
