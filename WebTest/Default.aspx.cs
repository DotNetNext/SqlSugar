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
                db.BeginTran();//开启事务，可以不使用事务
                //db.CommitTran using释放资源时自动执行


                try
                {
                    /*********************************************1、实体生成****************************************************/

                    //根据当前数据库生成所有表的实体类文件 （参数：SqlSugarClient ，文件目录，命名空间）
                    //db.ClassGenerating.CreateClassFiles(db,Server.MapPath("~/Models"),"Models");

                    //根据表名生成class字符串
                    var str = db.ClassGenerating.TableNameToClass(db, "student");

                    //根据SQL语句生成class字符串
                    var str2 = db.ClassGenerating.SqlToClass(db, "select top 1 * from student", "student");



                    /*********************************************2、查询****************************************************/

                    //根据sql语句映射成实体
                    var School = db.SqlQuery<school>("select * from School");


                    //联表查询
                    /*db.Sqlable是一个SQL语句生成帮助类*/
                    db.Sqlable.IsNoLock = true; //StudentViewSql语句中的表是允许脏读的 
                    string StudentViewSql = db.Sqlable.MappingTable<Student, school>("t1.sch_id=t2.id?"/* 最后加?代表left join否则inner join  */).WhereAfter("sex='男' order by t2.id").SelectToSql("t1.*,t2.name as school_name");
                    var studentView = db.SqlQuery<Student_View>(StudentViewSql);


                    /*********************************************3、添加****************************************************/

                    school s = new school()
                    {
                        name = "蓝翔"
                    };
                    var id = db.Insert(s);

                    /*********************************************4、修改****************************************************/

                    db.Update<school>(new { name = "蓝翔2" }, new { id = id });

                    /*********************************************5、删除****************************************************/

                    db.Delete<school>(id);

                    /*********************************************6、基类****************************************************/

                    db.ExecuteCommand(StudentViewSql);
                    db.GetDataTable(StudentViewSql);
                    db.GetString(StudentViewSql);
                    db.GetInt(StudentViewSql);
                    db.GetScalar(StudentViewSql);
                    db.GetReader(StudentViewSql);

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
