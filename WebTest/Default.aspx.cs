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

                //查询是允许脏读的，可以声名多个（默认值:不允许）
                db.Sqlable().IsNoLock = true;

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

                    //针对单表或者视图查询
                    var student = db.Queryable<Student>().Where(it => it.name == "张三").Where(c => c.id > 10).ToList();
                    var student2 = db.Queryable<Student>().Where(c => c.id > 10).OrderBy("id").Skip(10).Take(20).ToList();//取10-20条
                    var student2Count = db.Queryable<Student>().Where(c => c.id > 10).Count();//查询条数
                    var student3 = db.Queryable<Student>().Where(c => c.id > 10).OrderBy("id asc").ToPageList(2, 2);//分页
                    var student4 = db.Queryable<Student>().Where(c => c.id > 10).OrderBy("id").Skip(2).ToList();//从第2条开始
                    var student6 = db.Queryable<Student>().Where(c => c.id > 10).OrderBy("id").Take(2).ToList();//top2
                    var student7 = db.Queryable<Student>().Where(c => !c.name.Contains("a".ToString())).ToList();// 
                    var student8 = db.Queryable<Student>().Where(c => c.name == "a".ToString()).ToList();// 
                    var student9 = db.Queryable<Student>().Where(c => c.id == Convert.ToInt32("1")).ToList();// 
                    var student10 = db.Queryable<Student>().Where(c => DateTime.Now > Convert.ToDateTime("2015-1-1")).ToList();// 
                    var student11 = db.Queryable<Student>().Where(c => DateTime.Now > DateTime.Now).ToList();// 
                    var student12 = db.Queryable<Student>().Where(c => 1 == 1).Where("id>100").ToList();// 




                    //---------Sqlable,创建多表查询---------//

                    //多表查询
                    List<school> dataList = db.Sqlable()
                       .Form("school", "s")
                       .Join("student", "st", "st.id", "s.id", JoinType.INNER)
                       .Join("student", "st2", "st2.id", "st.id", JoinType.LEFT).Where("s.id>100 and s.id<@id").SelectToList<school>("st.*", new { id = 1 });

                    //多表分页
                    List<school> dataPageList = db.Sqlable()
                        .Form("school", "s")
                        .Join("student", "st", "st.id", "s.id", JoinType.INNER)
                        .Join("student", "st2", "st2.id", "st.id", JoinType.LEFT).Where("s.id>100 and s.id<100").SelectToPageList<school>("st.*", "s.id", 1, 10);


                    //---------SqlQuery,根据SQL或者存储过程---------//

                    //用于多用复杂语句查询
                    var School = db.SqlQuery<school>("select * from School");
                    //存储过程
                    //var spResult = db.SqlQuery<school>("exec sp_school @p1,@p2", new { p1=1,p2=2 });








                    /************************************************************************************************************/
                    /*************************************************3、添加****************************************************/
                    /************************************************************************************************************/

                    school s = new school()
                    {
                        name = "蓝翔"
                    };
                    //插入单条
                    var id = Convert.ToInt32(db.Insert(s));

                    //插入多条
                    List<school> sList = new List<school>();
                    sList.Add(s);
                    var ids = db.InsertRange(sList);


                    /************************************************************************************************************/
                    /*************************************************4、修改****************************************************/
                    /************************************************************************************************************/
                    //指定列更新
                    db.Update<school>(new { name = "蓝翔2" }, it => it.id == id);
                    //整个实体更新,注意主键必需为实体类的第一个属性
                    db.Update<school>(new school { id = id, name = "蓝翔2" }, it => it.id == id);



                    /************************************************************************************************************/
                    /*************************************************5、删除****************************************************/
                    /************************************************************************************************************/

                    db.Delete<school>(id);//注意主键必需为实体类的第一个属性
                    db.Delete<school>(it => id > 100);
                    db.Delete<school>(new string[] { "100", "101", "102" });

                    //db.FalseDelete<school>("is_del", 100);
                    //等同于 update school set is_del=0 where id in(100)
                    //db.FalseDelete<school>("is_del", it=>it.id==100);

                    /************************************************************************************************************/
                    /*************************************************6、基类****************************************************/
                    /************************************************************************************************************/

                    string sql = "select * from Student";

                    db.ExecuteCommand(sql);

                    db.GetDataTable(sql);
                    db.GetList<Student>(sql);
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
