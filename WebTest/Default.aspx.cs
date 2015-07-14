using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SqlSugar;
using Models;
using System.Linq.Expressions;
using System.Data.SqlClient;

namespace WebTest
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //连接字符串
            string connStr = @"Server=(LocalDB)\v11.0; Integrated Security=true ;AttachDbFileName=" + Server.MapPath("~/App_Data/SqlSugar.mdf");

            using (SqlSugarClient db = new SqlSugarClient(connStr))//开启数据库连接
            {
                //开启事务，可以不使用事务,也可以使用多个事务
                db.BeginTran();

                //db.CommitTran 提交事务会,在using结束前自动执行，可以不声名
                //db.RollbackTran(); 事务回滚，catch中声名

                //查询是允许脏读的，可以声名多个
                db.Sqlable.IsNoLock = true;

                try
                {
                    /************************************************************************************************************/
                    /*********************************************1、实体生成****************************************************/
                    /************************************************************************************************************/

                    //根据当前数据库生成所有表的实体类文件 （参数：SqlSugarClient ，文件目录，命名空间）
                    //db.ClassGenerating.CreateClassFiles(db,Server.MapPath("~/Models"),"Models");
                    //根据表名生成实体类文件
                    //db.ClassGenerating.CreateClassFilesByTableNames(db, Server.MapPath("~/Models"), "Models" , "student","school");

                    //根据表名生成class字符串
                    var str = db.ClassGenerating.TableNameToClass(db, "student");

                    //根据SQL语句生成class字符串
                    var str2 = db.ClassGenerating.SqlToClass(db, "select top 1 * from student", "student");





                    /************************************************************************************************************/
                    /*********************************************2、查询********************************************************/
                    /************************************************************************************************************/


                    //---------Queryable<T>,扩展函数查询---------//
                    var student = db.Queryable<Student>().Where(it => it.name == "张三").Where(c => c.id > 10).ToList();
                    var student2 = db.Queryable<Student>().Where(c => c.id > 10).Order("id").Skip(10).Take(20).ToList();//取10-20条
                    var student22 = db.Queryable<Student>().Where(c => c.id > 10).Order("id asc").ToPageList(2, 2);//分页
                    var student3 = db.Queryable<Student>().Where(c => c.id > 10).Order("id").Skip(2).ToList();//从第2条开始
                    var student4 = db.Queryable<Student>().Where(c => c.id > 10).Order("id").Take(2).ToList();//top2



                    //---------SqlQuery,根据SQL语句映射---------//
                    var School = db.SqlQuery<school>("select * from School");



                    //---------Sqlable,创建SQL语句---------//

                    string sql = db.Sqlable.Table("Student").ToSql();
                    //等于 SELECT * FROM  Student WITH(NOLOCK)

                    string sql1 = db.Sqlable.Table<Student>().ToPageSql(2, 10, "id asc");
                    //等于 SELECT * FROM (SELECT *,row_index=ROW_NUMBER() OVER(ORDER BY id asc ) FROM Student  WITH(NOLOCK) ) t WHERE  t.row_index BETWEEN 11 AND 20


                    //联表查询
                    string sql2 = db.Sqlable.MappingTable<Student, school>("t1.sch_id=t2.id?"/*?代表 left join */).WhereAfter("sex='男' order by t2.id").ToSql("t1.*,t2.name as school_name");
                    //等于：SELECT t1.*,t2.name as school_name FROM Student t1  WITH(NOLOCK)  LEFT JOIN school t2  WITH(NOLOCK)  ON t1.sch_id=t2.id WHERE  sex='男' order by t2.id

                    var studentView = db.SqlQuery<Student_View>(sql2);

                    //联表分页查询
                    string pageSql = db.Sqlable.MappingTable<Student, school>("t1.sch_id=t2.id").ToPageSql(3, 10, "t1.id asc", "t1.*,t2.name as school_name");
                    //等于: SELECT * FROM (SELECT t1.*,t2.name as school_name,row_index=ROW_NUMBER() OVER(ORDER BY t1.id asc ) FROM Student t1  WITH(NOLOCK)  INNER JOIN school t2  WITH(NOLOCK)  ON t1.sch_id=t2.id ) t WHERE  t.row_index BETWEEN 21 AND 30

                    var studentView2 = db.SqlQuery<Student_View>(pageSql);




                    /************************************************************************************************************/
                    /*************************************************3、添加****************************************************/
                    /************************************************************************************************************/

                    school s = new school()
                    {
                        name = "蓝翔"
                    };
                    var id = db.Insert(s);



                    /************************************************************************************************************/
                    /*************************************************4、修改****************************************************/
                    /************************************************************************************************************/

                    db.Update<school>(new { name = "蓝翔2" }, new { id = id });




                    /************************************************************************************************************/
                    /*************************************************5、删除****************************************************/
                    /************************************************************************************************************/
                    db.Delete<school>(id);
                    db.Delete<school>(new string[] { "100", "101", "102" });



                    /************************************************************************************************************/
                    /*************************************************6、基类****************************************************/
                    /************************************************************************************************************/
                    db.ExecuteCommand(sql);

                    db.GetDataTable(sql);
                    db.GetList<Student>(sql1);
                    db.GetSingle<Student>(sql + " where id=1");
                    using (SqlDataReader read = db.GetReader(sql)) { }  //事务中一定要释放DataReader

                    db.GetScalar(sql);
                    db.GetString(sql);
                    db.GetInt(sql);


                }
                catch (Exception ex)
                {
                    //回滚事务
                    db.RollbackTran();
                    throw ex;
                }

            }//关闭数据库连接
            Console.Read();
        }



    }



}
