using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlSugar;
using NewTest.Dao;
using Models;
using System.Data.SqlClient;
using System.Data;

namespace NewTest.Demos
{
    //查询的例子
    public class Select : IDemos
    {

        public void Init()
        {
            Console.WriteLine("启动Select.Init");
            //使用拉姆达查询 基于Queryable
            QueryableDemo();

            //新容器转换
            QueryableSelectNewClass();

            //使用更接近sql的查询方式 基于Sqlable
            SqlableDemo();

            //使用原生Sql查询 
            SqlQuery();


        }

        /// <summary>
        /// 拉姆达表达示
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
                var singleOrDefault = db.Queryable<Student>().SingleOrDefault(c => c.id == 11111111);
                //查询单条没有记录返回空对象
                var single2 = db.Queryable<Student>().Where(c => c.id == 1).SingleOrDefault();

                //查询第一条
                var first = db.Queryable<Student>().Where(c => c.id == 1).First();
                var first2 = db.Queryable<Student>().Where(c => c.id == 1).FirstOrDefault();

                //取11-20条
                var page1 = db.Queryable<Student>().Where(c => c.id > 10).OrderBy(it => it.id).Skip(10).Take(10).ToList();

                //取11-20条  等于 Skip(pageIndex-1)*pageSize).Take(pageSize) 等于  between (pageIndex-1)*pageSize and  pageIndex*pageSize
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

                //支持字符串Where 让你解决，更复杂的查询
                var student12 = db.Queryable<Student>().Where(c => "a" == "a").Where("id>@id", new { id = 1 }).ToList();
                var student13 = db.Queryable<Student>().Where(c => "a" == "a").Where("id>100 and id in( select 1)").ToList();


                //存在记录反回true，则否返回false
                bool isAny100 = db.Queryable<Student>().Any(c => c.id == 100);
                bool isAny1 = db.Queryable<Student>().Any(c => c.id == 1);


                //获取最大Id
                object maxId = db.Queryable<Student>().Max(it => it.id);
                int maxId1 = db.Queryable<Student>().Max(it => it.id).ObjToInt();//拉姆达
                int maxId2 = db.Queryable<Student>().Max<int>("id"); //字符串写法

                //获取最小
                int minId1 = db.Queryable<Student>().Where(c => c.id > 0).Min(it => it.id).ObjToInt();//拉姆达
                int minId2 = db.Queryable<Student>().Where(c => c.id > 0).Min<int>("id");//字符串写法


                //order By 
                var orderList = db.Queryable<Student>().OrderBy("id desc,name asc").ToList();//字符串支持多个排序
                //可以多个order by表达示
                var order2List = db.Queryable<Student>().OrderBy(it => it.name).OrderBy(it => it.id, OrderByType.desc).ToList(); // order by name as ,order by id desc

                //In
                var intArray = new[] { "5", "2", "3" };
                var intList = intArray.ToList();
                var listnew = db.Queryable<Student>().Where(it => intArray.Contains(it.name)).ToList();
                var list0 = db.Queryable<Student>().In(it => it.id, 1, 2, 3).ToList();
                var list1 = db.Queryable<Student>().In(it => it.id, intArray).ToList();
                var list2 = db.Queryable<Student>().In("id", intArray).ToList();
                var list3 = db.Queryable<Student>().In(it => it.id, intList).ToList();
                var list4 = db.Queryable<Student>().In("id", intList).ToList();

                //分组查询
                var list7 = db.Queryable<Student>().Where(c => c.id < 20).GroupBy(it => it.sex).Select("sex,count(*) Count").ToDynamic();
                var list8 = db.Queryable<Student>().Where(c => c.id < 20).GroupBy(it => it.sex).GroupBy(it => it.id).Select("id,sex,count(*) Count").ToDynamic();
                List<StudentGroup> list9 = db.Queryable<Student>().Where(c => c.id < 20).GroupBy(it => it.sex).Select<StudentGroup>("Sex,count(*) Count").ToList();
                List<StudentGroup> list10 = db.Queryable<Student>().Where(c => c.id < 20).GroupBy("sex").Select<StudentGroup>("Sex,count(*) Count").ToList();
                //SELECT Sex,Count=count(*)  FROM Student  WHERE 1=1  AND  (id < 20)    GROUP BY Sex --生成结果



                //2表关联查询
                var jList = db.Queryable<Student>()
                .JoinTable<School>((s1, s2) => s1.sch_id == s2.id) //默认left join
                .Where<School>((s1, s2) => s1.id == 1)
                .Select("s1.*,s2.name as schName")
                .ToDynamic();

                /*等于同于
                 SELECT s1.*,s2.name as schName 
                 FROM [Student]  s1 
                 LEFT JOIN [School]  s2 ON  s1.sch_id  = s2.id 
                 WHERE  s1.id  = 1 */

                //2表关联查询并分页
                var jList2 = db.Queryable<Student>()
                .JoinTable<School>((s1, s2) => s1.sch_id == s2.id) //默认left join
                    //如果要用inner join这么写
                    //.JoinTable<School>((s1, s2) => s1.sch_id == s2.id ,JoinType.INNER)
                .Where<School>((s1, s2) => s1.id > 1)
                .OrderBy(s1 => s1.name)
                .Skip(10)
                .Take(20)
                .Select("s1.*,s2.name as schName")
                .ToDynamic();

                //3表查询并分页
                var jList3 = db.Queryable<Student>()
               .JoinTable<School>((s1, s2) => s1.sch_id == s2.id) // left join  School s2  on s1.id=s2.id
               .JoinTable<School>((s1, s3) => s1.sch_id == s3.id) // left join  School s3  on s1.id=s3.id
               .Where<School>((s1, s2) => s1.id > 1)  // where s1.id>1
               .Where(s1 => s1.id > 0)
               .OrderBy<School>((s1, s2) => s1.id) //order by s1.id 多个order可以  .oderBy().orderby 叠加 
               .Skip(10)
               .Take(20)
               .Select("s1.*,s2.name as schName,s3.name as schName2")//select目前只支持这种写法
               .ToDynamic();


                //上面的方式都是与第一张表join，第三张表想与第二张表join写法如下
                List<V_Student> jList4 =
                 db.Queryable<Student>()
                 .JoinTable<School>((s1, s2) => s1.sch_id == s2.id) // left join  School s2  on s1.id=s2.id
                 .JoinTable<School, Area>((s1, s2, a1) => a1.id == s2.AreaId)// left join  Area a1  on a1.id=s2.AreaId  第三张表与第二张表关联
                 .JoinTable<Area, School>((s1, a1, s3) => a1.id == s3.AreaId)// left join  School s3  on a1.id=s3.AreaId  第四第表第三张表关联
                 .JoinTable<School>((s1, s4) => s1.sch_id == s4.id) // left join  School s2  on s1.id=s4.id
                 .Select<School, Area, V_Student>((s1, s2, a1) => new V_Student { id = s1.id, name = s1.name, SchoolName = s2.name, AreaName = a1.name }).ToList();

                //等同于
                //SELECT id = s1.id, name = s1.name, SchoolName = s2.name, AreaName = a1.name  
                //FROM [Student]   s1 
                //LEFT JOIN School  s2 ON  ( s1.sch_id  = s2.id )    
                //LEFT JOIN Area  a1 ON  ( a1.id  = s2.AreaId )     //第三张表与第二张表关联
                //LEFT JOIN School  s3 ON  ( a1.id  = s3.AreaId )   //第四张表与第三张表关联
                //LEFT JOIN School  s4 ON  ( s1.sch_id  = s4.id )    
                //WHERE 1=1    


                //Join子查询语句加分页的写法
                var childQuery = db.Queryable<Area>().Where("id=@id").Select(it => new { id = it.id }).ToSql();//创建子查询SQL
                string childTableName =SqlSugarTool.PackagingSQL(childQuery.Key);//将SQL语句用()包成表
                var queryable = db.Queryable<Student>()
                 .JoinTable<School>((s1, s2) => s1.sch_id == s2.id)  //LEFT JOIN School  s2 ON  ( s1.sch_id  = s2.id )  
                 .JoinTable(childTableName, "a1", "a1.id=s2.areaid", new { id = 1 }, JoinType.INNER) //INNER JOIN (SELECT *  FROM [Area]   WHERE 1=1  AND id=@id   ) a1 ON a1.id=s2.areaid
                 .OrderBy(s1 => s1.id);

                var list = queryable.Select<School, Area, V_Student>((s1, s2, a1) => new V_Student { id = s1.id, name = s1.name, SchoolName = s2.name, AreaName = a1.name })
                    .ToPageList(0, 200);
                var count2 = queryable.Count();


                //拼接例子
                var queryable2 = db.Queryable<Student>().Where(it => true);
                if (maxId.ObjToInt() == 1)
                {
                    queryable2.Where(it => it.id == 1);
                }
                else
                {
                    queryable2.Where(it => it.id == 2);
                }
                var listJoin = queryable2.ToList();


                //queryable和SqlSugarClient解耦
                var par = new Queryable<Student>().Where(it => it.id == 1);//声名没有connection对象的Queryable
                par.DB = db;
                var listPar = par.ToList();


                //查看生成的sql和参数
                var id = 1;
                var sqlAndPars = db.Queryable<Student>().Where(it => it.id == id).OrderBy(it => it.id).ToSql();



                //条件函数的支持(字段暂不支持函数,只有参数支持) 目前只支持这么多
                var par1 = "2015-1-1"; var par2 = "   我 有空格A, ";
                var r1 = db.Queryable<Student>().Where(it => it.name == par1.ObjToString()).ToList(); //ObjToString会将null转转成""
                var r2 = db.Queryable<InsertTest>().Where(it => it.d1 == par1.ObjToDate()).ToList();
                var r3 = db.Queryable<InsertTest>().Where(it => it.id == 1.ObjToInt()).ToList();//ObjToInt会将null转转成0
                var r4 = db.Queryable<InsertTest>().Where(it => it.id == 2.ObjToDecimal()).ToList();
                var r5 = db.Queryable<InsertTest>().Where(it => it.id == 3.ObjToMoney()).ToList();
                var r6 = db.Queryable<InsertTest>().Where(it => it.v1 == par2.Trim()).ToList();
                var convert1 = db.Queryable<Student>().Where(c => c.name == "a".ToString()).ToList();
                var convert2 = db.Queryable<Student>().Where(c => c.id == Convert.ToInt32("1")).ToList();
                var convert3 = db.Queryable<Student>().Where(c => c.name == par2.ToLower()).ToList();
                var convert4 = db.Queryable<Student>().Where(c => c.name == par2.ToUpper()).ToList();
                var convert5= db.Queryable<Student>().Where(c => DateTime.Now > Convert.ToDateTime("2015-1-1")).ToList();
                var c1 = db.Queryable<Student>().Where(c => c.name.Contains("a")).ToList();
                var c2 = db.Queryable<Student>().Where(c => c.name.StartsWith("a")).ToList();
                var c3 = db.Queryable<Student>().Where(c => c.name.EndsWith("a")).ToList();
                var c4 = db.Queryable<Student>().Where(c => !string.IsNullOrEmpty(c.name)).ToList();

            }
        }

        /// <summary>
        /// 新容器转换
        /// </summary>
        private void QueryableSelectNewClass()
        {

            using (SqlSugarClient db = SugarDao.GetInstance())
            {

                //单表操作将Student转换成V_Student
                var queryable = db.Queryable<Student>().Where(c => c.id < 10)
                    .Select<V_Student>(c => new V_Student { id = c.id, name = c.name, AreaName = "默认地区", SchoolName = "默认学校", SubjectName = "NET" });

                var list = queryable.ToList();
                var json = queryable.ToJson();
                var dynamic = queryable.ToDynamic();


                //多表操作将Student转换成V_Student
                var queryable2 = db.Queryable<Student>()
                 .JoinTable<School>((s1, s2) => s1.sch_id == s2.id)
                 .Where<School>((s1, s2) => s2.id < 10)
                 .Select<School, V_Student>((s1, s2) => new V_Student() { id = s1.id, name = s1.name, AreaName = "默认地区", SchoolName = s2.name, SubjectName = "NET" });//select new 目前只支持这种写法

                var list2 = queryable2.ToList();
                var json2 = queryable2.ToJson();
                var dynamic2 = queryable2.ToDynamic();


                //select字符串 转换成V_Student
                var list3 = db.Queryable<Student>()
               .JoinTable<School>((s1, s2) => s1.sch_id == s2.id)
               .Where(s1 => s1.id <= 3)
               .Select<V_Student>("s1.*,s2.name SchoolName")
               .ToList();

                //select字符串 转换成Json
                var json3 = db.Queryable<Student>()
               .JoinTable<School>((s1, s2) => s1.sch_id == s2.id)
               .Where(s1 => s1.id <= 3)
               .Select<V_Student>("s1.*,s2.name SchoolName")
               .ToJson();

                //select字符串 转换成Json
                var dynamic3 = db.Queryable<Student>()
               .JoinTable<School>((s1, s2) => s1.sch_id == s2.id)
               .Where(s1 => s1.id <= 3)
               .Select<V_Student>("s1.*,s2.name SchoolName")
               .ToDynamic();


                //新容器转换函数的支持 只支持ObjToXXX和Convert.ToXXX
                var f1 = db.Queryable<InsertTest>().Select<Student>(it => new Student()
                {
                    name = it.d1.ObjToString(),
                    id = it.int1.ObjToInt() // 支持ObjToXXX 所有函数

                }).ToList();

                var f2 = db.Queryable<InsertTest>().Select<Student>(it => new Student()
                {
                    name = Convert.ToString(it.d1),//支持Convet.ToXX所有函数
                    id = it.int1.ObjToInt(),
                    sex = Convert.ToString(it.d1),

                }).ToList();

                var f3 = db.Queryable<InsertTest>()
                    .JoinTable<InsertTest>((i1, i2) => i1.id == i2.id)
                    .Select<InsertTest, Student>((i1, i2) => new Student()
                    {
                        name = Convert.ToString(i1.d1), //多表查询例子
                        id = i1.int1.ObjToInt(),
                        sex = Convert.ToString(i2.d1),

                    }).ToList();


                //Select 外部参数用法
                var f4 = db.Queryable<InsertTest>().Where("1=1", new { id = 100 }).Select<Student>(it => new Student()
                {
                    id = "@id".ObjToInt(), //取的是 100 的值
                    name = "张三",//内部参数可以直接写
                    sex = it.txt,
                    sch_id = it.id

                }).ToList();
                var f6 = db.Queryable<InsertTest>()
               .JoinTable<InsertTest>((i1, i2) => i1.id == i2.id)
               .Where("1=1", new { id = 100, name = "张三", isOk = true }) //外部传参给@id
               .Select<InsertTest, Student>((i1, i2) => new Student()
               {
                   name = "@name".ObjToString(), //多表查询例子
                   id = "@id".ObjToInt(),
                   sex = i2.txt,
                   sch_id = 1,
                   isOk = "@isOk".ObjToBool()

               }).ToList();
            }
        }

        /// <summary>
        /// 基于原生Sql的查询
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

                //存储过程加Output 
                var pars = SqlSugarTool.GetParameters(new { p1 = 1,p2=0 }); //将匿名对象转成SqlParameter
                db.IsClearParameters = false;//禁止清除参数
                pars[1].Direction = ParameterDirection.Output; //将p2设为 output
                var spResult2 = db.SqlQuery<School>("exec sp_school @p1,@p2 output", pars);
                db.IsClearParameters = true;//启用清除参数
                var outPutValue = pars[1].Value;//获取output @p2的值

                //获取第一行第一列的值
                string v1 = db.GetString("select '张三' as name");
                int v2 = db.GetInt("select 1 as name");
                double v3 = db.GetDouble("select 1 as name");
                decimal v4 = db.GetDecimal("select 1 as name");
                //....
            }
        }


        /// <summary>
        /// 接近Sql的编程模式
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

    }
}
