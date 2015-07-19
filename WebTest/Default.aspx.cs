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
            string connStr = @"Server=.;uid=sa;pwd=sasa;database=SqlSugarTest";

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
                    var str = db.ClassGenerating.TableNameToClass(db, "Student");

                    //根据SQL语句生成class字符串
                    var str2 = db.ClassGenerating.SqlToClass(db, "select top 1 * from Student", "student");





                    /************************************************************************************************************/
                    /*********************************************2、查询********************************************************/
                    /************************************************************************************************************/


                    //---------Queryable<T>,扩展函数查询---------//

                    //针对单表或者视图查询

                    //查询所有
                    var student = db.Queryable<Student>().ToList();

                    //查询单条
                    var single = db.Queryable<Student>().Where(c => c.name=="@a");

                    //取10-20条
                    var page1 = db.Queryable<Student>().Where(c => c.id > 10).OrderBy("id").Skip(10).Take(20).ToList();
                    //上一句的简化写法，同样取10-20条
                    var page2 = db.Queryable<Student>().Where(c => c.id > 10).OrderBy("id").ToPageList(2, 10);

                    //查询条数
                    var count = db.Queryable<Student>().Where(c => c.id > 10).Count();

                    //从第2条开始以后取所有
                    var skip = db.Queryable<Student>().Where(c => c.id > 10).OrderBy("id").Skip(2).ToList();

                    //取前2条
                    var take = db.Queryable<Student>().Where(c => c.id > 10).OrderBy("id").Take(2).ToList();

                    // Not like 
                    string conval = "a";
                    var notLike = db.Queryable<Student>().Where(c => !c.name.Contains(conval.ToString())).ToList();

                    // 可以在拉姆达使用 ToString和 Convert,比EF出色的地方
                    var convert1 = db.Queryable<Student>().Where(c => c.name == "a".ToString()).ToList();
                    var convert2 = db.Queryable<Student>().Where(c => c.id == Convert.ToInt32("1")).ToList();// 
                    var convert3 = db.Queryable<Student>().Where(c => DateTime.Now > Convert.ToDateTime("2015-1-1")).ToList();
                    var convert4 = db.Queryable<Student>().Where(c => DateTime.Now > DateTime.Now).ToList();

                    //支持字符串Where 让你解决，更复杂的查询
                    var student12 = db.Queryable<Student>().Where(c => "a" == "a").Where("id>100").ToList();


                    //存在记录反回true，则否返回false
                    bool isAny100 = db.Queryable<Student>().Any(c => c.id == 100);
                    bool isAny1 = db.Queryable<Student>().Any(c => c.id == 1);


                    //---------Sqlable,创建多表查询---------//

                    //多表查询
                    List<School> dataList = db.Sqlable()
                       .Form("school", "s")
                       .Join("student", "st", "st.id", "s.id", JoinType.INNER)
                       .Join("student", "st2", "st2.id", "st.id", JoinType.LEFT).Where("s.id>100 and s.id<@id").SelectToList<School>("st.*", new { id = 1 });

                    //多表分页
                    List<School> dataPageList = db.Sqlable()
                        .Form("school", "s")
                        .Join("student", "st", "st.id", "s.id", JoinType.INNER)
                        .Join("student", "st2", "st2.id", "st.id", JoinType.LEFT).Where("s.id>100 and s.id<100").SelectToPageList<School>("st.*", "s.id", 1, 10);


                    //---------SqlQuery,根据SQL或者存储过程---------//

                    //用于多用复杂语句查询
                    var School = db.SqlQuery<Student>("select * from Student");

                    //获取id
                    var id = db.SqlQuery<int>("select top 1 id from Student").Single();

                    //存储过程
                    //var spResult = db.SqlQuery<school>("exec sp_school @p1,@p2", new { p1=1,p2=2 });








                    /************************************************************************************************************/
                    /*************************************************3、添加****************************************************/
                    /************************************************************************************************************/

                    School s = new School()
                    {
                        name = "蓝翔"
                    };
                    //插入单条
                    var id2 = Convert.ToInt32(db.Insert(s));

                    //插入多条
                    List<School> sList = new List<School>();
                    sList.Add(s);
                    var ids = db.InsertRange(sList);


                    /************************************************************************************************************/
                    /*************************************************4、修改****************************************************/
                    /************************************************************************************************************/
                    //指定列更新
                    db.Update<School>(new { name = "蓝翔2" }, it => it.id == id);
                    //整个实体更新,注意主键必需为实体类的第一个属性
                    db.Update<School>(new School { id = id, name = "蓝翔2" }, it => it.id == id);



                    /************************************************************************************************************/
                    /*************************************************5、删除****************************************************/
                    /************************************************************************************************************/

                    db.Delete<School>(10);//注意主键必需为实体类的第一个属性
                    db.Delete<School>(it => id > 100);
                    db.Delete<School>(new string[] { "100", "101", "102" });

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
