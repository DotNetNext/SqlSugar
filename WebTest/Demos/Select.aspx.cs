using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Models;
using WebTest.Dao;
using SqlSugar;
using System.Data;
namespace WebTest.Demo
{
    /// <summary>
    /// 查询例子
    /// </summary>
    public partial class Select : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            //单表查询
            QueryableDemo();

            //多表查询
            SqlableDemo();

            //Sql查询
            SqlQuery();

            //新容器转换
            NewSelect();

        }

        /// <summary>
        /// 新容器转换
        /// </summary>
        private void NewSelect()
        {

            using (SqlSugarClient db = SugarDao.GetInstance())
            {
                var list2 = db.Queryable<Student>().Where(c => c.id < 10).Select(c => new classNew { newid = c.id, newname = c.name, xx_name = c.name }).ToList();//不支持匿名类转换,也不建议使用

                var list3 = db.Queryable<Student>().Where(c => c.id < 10).Select(c => new { newid = c.id, newname = c.name, xx_name = c.name }).ToDynamic();//匿名类转换

                var list4 = db.Queryable<Student>().Where(c => c.id < 10).Select("id as newid, name as newname ,name as xx_name").ToDynamic();//匿名类转换
            }
        }
        /// <summary>
        /// Sql查询
        /// </summary>
        private void SqlQuery()
        {
            using (var db = SugarDao.GetInstance())
            {
                //转成list
                List<Student> list1 = db.SqlQuery<Student>("select * from Student");
                //转成list带参
                List<Student> list2 = db.SqlQuery<Student>("select * from Student where id=@id", new { id = 1 });
                //转成dynamic
                dynamic list3 = db.SqlQueryDynamic("select * from student");
                //转成json
                string list4 = db.SqlQueryJson("select * from student");
                //返回int
                var list5 = db.SqlQuery<int>("select top 1 id from Student").SingleOrDefault();
                //反回键值
                Dictionary<string, string> list6 = db.SqlQuery<KeyValuePair<string, string>>("select id,name from Student").ToDictionary(it => it.Key, it => it.Value);
                //反回List<string[]>
                var list7 = db.SqlQuery<string[]>("select top 1 id,name from Student").SingleOrDefault();
                //存储过程
                var spResult = db.SqlQuery<School>("exec sp_school @p1,@p2", new { p1 = 1, p2 = 2 });

                //获取第一行第一列的值
                string v1 = db.GetString("select '张三' as name");
                int v2 = db.GetInt("select 1 as name");
                double v3 = db.GetDouble("select 1 as name");
                decimal v4 = db.GetDecimal("select 1 as name");
                //....
            }
        }
        /// <summary>
        /// 多表查询
        /// </summary>
        private void SqlableDemo()
        {
            using (var db = SugarDao.GetInstance())
            {
                //---------Sqlable,创建多表查询---------//

                //多表查询
                List<School> dataList = db.Sqlable()
                   .From("school", "s")
                   .Join("student", "st", "st.id", "s.id", JoinType.INNER)
                   .Join("student", "st2", "st2.id", "st.id", JoinType.LEFT)
                   .Where("s.id>100 and s.id<@id")
                   .Where("1=1")//可以多个WHERE
                   .OrderBy("id")
                   .SelectToList<School/*新的Model我这里没有所以写的School*/>("st.*", new { id = 1 });

                //多表分页
                List<School> dataPageList = db.Sqlable()
                    .From("school", "s")
                    .Join("student", "st", "st.id", "s.id", JoinType.INNER)
                    .Join("student", "st2", "st2.id", "st.id", JoinType.LEFT)
                    .Where("s.id>100 and s.id<100")
                    .SelectToPageList<School>("st.*", "s.id", 1, 10);

                //多表分页WHERE加子查询
                List<School> dataPageList2 = db.Sqlable()
                    .From("school", "s")
                    .Join("student", "st", "st.id", "s.id", JoinType.INNER)
                    .Join("student", "st2", "st2.id", "st.id", JoinType.LEFT)
                    .Where("s.id>100 and s.id<100 and s.id in (select 1 )" /*这里面写子查询都可以*/)
                    .SelectToPageList<School>("st.*", "s.id", 1, 10);



                //--------转成List Dynmaic 或者 Json-----//

                //不分页
                var list1 = db.Sqlable().From("student", "s").Join("school", "l", "s.sch_id", "l.id and l.id=@id", JoinType.INNER).SelectToDynamic("*", new { id = 1 });
                var list2 = db.Sqlable().From("student", "s").Join("school", "l", "s.sch_id", "l.id and l.id=@id", JoinType.INNER).SelectToJson("*", new { id = 1 });
                var list3 = db.Sqlable().From("student", "s").Join("school", "l", "s.sch_id", "l.id and l.id=@id", JoinType.INNER).SelectToDataTable("*", new { id = 1 });

                //分页
                var list4 = db.Sqlable().From("student", "s").Join("school", "l", "s.sch_id", "l.id and l.id=@id", JoinType.INNER).SelectToPageDynamic("s.*", "l.id", 1, 10, new { id = 1 });
                var list5 = db.Sqlable().From("student", "s").Join("school", "l", "s.sch_id", "l.id and l.id=@id", JoinType.INNER).SelectToPageTable("s.*", "l.id", 1, 10, new { id = 1 });
                var list6 = db.Sqlable().From("student", "s").Join("school", "l", "s.sch_id", "l.id and l.id=@id", JoinType.INNER).SelectToPageDynamic("s.*", "l.id", 1, 10, new { id = 1 });


                //--------拼接-----//
                Sqlable sable = db.Sqlable().From<Student>("s").Join<School>("l", "s.sch_id", "l.id", JoinType.INNER);
                string name = "a";
                int id = 1;
                if (!string.IsNullOrEmpty(name))
                {
                    sable = sable.Where("s.name=@name");
                }
                if (!string.IsNullOrEmpty(name))
                {
                    sable = sable.Where("s.id=@id or s.id=100");
                }
                if (id > 0)
                {
                    sable = sable.Where("l.id in (select top 10 id from school)");//where加子查询
                }
                var pars = new { id = id, name = name };
                int pageCount = sable.Count(pars);
                var list7 = sable.SelectToPageList<Student>("s.*", "l.id desc", 1, 20, pars);


            }
        }

        /// <summary>
        /// 单表查询
        /// </summary>
        private void QueryableDemo()
        {

            using (var db = SugarDao.GetInstance())
            {


                //---------Queryable<T>,扩展函数查询---------//

                //针对单表或者视图查询

                //查询所有
                var student = db.Queryable<Student>().ToList();
                var studentDynamic = db.Queryable<Student>().ToDynamic();
                var studentJson = db.Queryable<Student>().ToJson();

                //查询单条
                var single = db.Queryable<Student>().Single(c => c.id == 1);
                //查询单条没有记录返回空对象
                var single2 = db.Queryable<Student>().Where(c => c.id == 1).SingleOrDefault();

                //查询第一条
                var first = db.Queryable<Student>().Where(c => c.id == 1).First();
                var first2 = db.Queryable<Student>().Where(c => c.id == 1).FirstOrDefault();

                //取10-20条
                var page1 = db.Queryable<Student>().Where(c => c.id > 10).OrderBy(it => it.id).Skip(10).Take(20).ToList();

                //上一句的简化写法，同样取10-20条
                var page2 = db.Queryable<Student>().Where(c => c.id > 10).OrderBy(it => it.id).ToPageList(2, 10);

                //查询条数
                var count = db.Queryable<Student>().Where(c => c.id > 10).Count();

                //从第2条开始以后取所有
                var skip = db.Queryable<Student>().Where(c => c.id > 10).OrderBy(it => it.id).Skip(2).ToList();

                //取前2条
                var take = db.Queryable<Student>().Where(c => c.id > 10).OrderBy(it => it.id).Take(2).ToList();

                // Not like 
                string conval = "a";
                var notLike = db.Queryable<Student>().Where(c => !c.name.Contains(conval.ToString())).ToList();
                //Like
                conval = "三";
                var like = db.Queryable<Student>().Where(c => c.name.Contains(conval)).ToList();

                // 可以在拉姆达使用 ToString和 Convert,比EF出色的地方
                var convert1 = db.Queryable<Student>().Where(c => c.name == "a".ToString()).ToList();
                var convert2 = db.Queryable<Student>().Where(c => c.id == Convert.ToInt32("1")).ToList();// 
                var convert3 = db.Queryable<Student>().Where(c => DateTime.Now > Convert.ToDateTime("2015-1-1")).ToList();
                var convert4 = db.Queryable<Student>().Where(c => DateTime.Now > DateTime.Now).ToList();

                //支持字符串Where 让你解决，更复杂的查询
                var student12 = db.Queryable<Student>().Where(c => "a" == "a").Where("id>100").ToList();
                var student13 = db.Queryable<Student>().Where(c => "a" == "a").Where("id>100 and id in( select 1)").ToList();


                //存在记录反回true，则否返回false
                bool isAny100 = db.Queryable<Student>().Any(c => c.id == 100);
                bool isAny1 = db.Queryable<Student>().Any(c => c.id == 1);


                //获取最大Id
                object maxId = db.Queryable<Student>().Max(it => it.id);
                int maxId1 = db.Queryable<Student>().Max(it => it.id).ObjToInt();//拉姆达
                int maxId2 = db.Queryable<Student>().Max<Student, int>("id"); //字符串写法

                //获取最小
                int minId1 = db.Queryable<Student>().Where(c => c.id > 0).Min(it => it.id).ObjToInt();//拉姆达
                int minId2 = db.Queryable<Student>().Where(c => c.id > 0).Min<Student, int>("id");//字符串写法


                //order By 
                var orderList = db.Queryable<Student>().OrderBy("id desc,name asc").ToList();//字符串支持多个排序
                //可以多个order by表达示
                var order2List = db.Queryable<Student>().OrderBy(it=>it.name).OrderBy(it => it.id, OrderByType.desc).ToList(); // order by name as ,order by id desc

                //In
                var list1 = db.Queryable<Student>().In("id", "1", "2", "3").ToList();
                var list2 = db.Queryable<Student>().In("id", new string[] { "1", "2", "3" }).ToList();
                var list3 = db.Queryable<Student>().In("id", new List<string> { "1", "2", "3" }).ToList();
                var list4 = db.Queryable<Student>().Where(it => it.id < 10).In("id", new List<string> { "1", "2", "3" }).ToList();

                //分组查询
                var list7 = db.Queryable<Student>().Where(c => c.id < 20).GroupBy(it => it.sex).Select("sex,Count=count(*)").ToDynamic();
                var list8 = db.Queryable<Student>().Where(c => c.id < 20).GroupBy(it => it.sex).GroupBy(it=>it.id).Select("id,sex,Count=count(*)").ToDynamic();
                List<SexTotal> list5 = db.Queryable<Student>().Where(c => c.id < 20).GroupBy(it => it.sex).Select<Student, SexTotal>("Sex,Count=count(*)").ToList();
                List<SexTotal> list6 = db.Queryable<Student>().Where(c => c.id < 20).GroupBy("sex").Select<Student, SexTotal>("Sex,Count=count(*)").ToList();
                //SELECT Sex,Count=count(*)  FROM Student  WHERE 1=1  AND  (id < 20)    GROUP BY Sex --生成结果

            }
        }
    }
}