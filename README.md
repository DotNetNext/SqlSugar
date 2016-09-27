#Contact information

Email 610262374@qq.com
QQ Group 225982985




#Other versions

ASP.NET CORE MSSQL https://github.com/sunkaixuan/ASP_NET_CORE_ORM_SqlSugar

ASP.NET 4.0+ MYSQL https://github.com/sunkaixuan/MySqlSugar

ASP.NET 4.0+ Sqlite  https://github.com/sunkaixuan/SqliteSugar

ASP.NET 4.0+ ORACLE https://github.com/sunkaixuan/OracleSugar


# Select Queryable<T>


     using (var db =  new SqlSugarClient("sever=.;sa=saxxxxxxxx"))
     {
                //select all
                var student = db.Queryable<Student>().ToList();
                var studentDynamic = db.Queryable<Student>().ToDynamic();
                var studentJson = db.Queryable<Student>().ToJson();

                //select single
                var single = db.Queryable<Student>().Single(c => c.id == 1);
                //select single return null
                var single2 = db.Queryable<Student>().Where(c => c.id == 1).SingleOrDefault();

                //top 1
                var first = db.Queryable<Student>().Where(c => c.id == 1).First();
                var first2 = db.Queryable<Student>().Where(c => c.id == 1).FirstOrDefault();

                //10-20
                var page1 = db.Queryable<Student>().Where(c => c.id > 10).OrderBy(it => it.id).Skip(10).Take(20).ToList();

                //page
                var page2 = db.Queryable<Student>().Where(c => c.id > 10).OrderBy(it => it.id).ToPageList(2, 10);

                //count
                var count = db.Queryable<Student>().Where(c => c.id > 10).Count();

                //skip 2
                var skip = db.Queryable<Student>().Where(c => c.id > 10).OrderBy(it => it.id).Skip(2).ToList();

                //top 2
                var take = db.Queryable<Student>().Where(c => c.id > 10).OrderBy(it => it.id).Take(2).ToList();

                // Not like 
                string conval = "a";
                var notLike = db.Queryable<Student>().Where(c => !c.name.Contains(conval.ToString())).ToList();
                //Like
                conval = "三";
                var like = db.Queryable<Student>().Where(c => c.name.Contains(conval)).ToList();

                // Where
                var convert1 = db.Queryable<Student>().Where(c => c.name == "a".ToString()).ToList();
                var convert2 = db.Queryable<Student>().Where(c => c.id == Convert.ToInt32("1")).ToList();// 
                var convert3 = db.Queryable<Student>().Where(c => DateTime.Now > Convert.ToDateTime("2015-1-1")).ToList();
                var convert4 = db.Queryable<Student>().Where(c => DateTime.Now > DateTime.Now).ToList();

                //where string 
                var student12 = db.Queryable<Student>().Where(c => "a" == "a").Where("id>100").ToList();
                var student13 = db.Queryable<Student>().Where(c => "a" == "a").Where("id>100 and id in( select 1)").ToList();


                //any
                bool isAny100 = db.Queryable<Student>().Any(c => c.id == 100);
                bool isAny1 = db.Queryable<Student>().Any(c => c.id == 1);


                //max Id
                object maxId = db.Queryable<Student>().Max(it => it.id);
                int maxId1 = db.Queryable<Student>().Max(it => it.id).ObjToInt();
                int maxId2 = db.Queryable<Student>().Max<Student, int>("id"); 

                //min id
                int minId1 = db.Queryable<Student>().Where(c => c.id > 0).Min(it => it.id).ObjToInt();
                int minId2 = db.Queryable<Student>().Where(c => c.id > 0).Min<Student, int>("id");


                //order by 
                var orderList = db.Queryable<Student>().OrderBy("id desc,name asc").ToList();
                //multiple order by
                var order2List = db.Queryable<Student>().OrderBy(it=>it.name).OrderBy(it => it.id, OrderByType.desc).ToList();
                // order by name as ,order by id desc

                //in
                var intArray=new []{"5", "2", "3"};
                var intList = intArray.ToList();
                var list0 = db.Queryable<Student>().In(it => it.id, 1,2,3).ToList();
                var list1 = db.Queryable<Student>().In(it=>it.id, intArray).ToList();
                var list2 = db.Queryable<Student>().In("id", intArray).ToList();
                var list3 = db.Queryable<Student>().In(it => it.id, intList).ToList();
                var list4 = db.Queryable<Student>().In("id", intList).ToList();

                //group by
                var list7 = db.Queryable<Student>().Where(c => c.id < 20).GroupBy(it => it.sex).Select("sex,Count=count(*)").ToDynamic();
                var list8 = db.Queryable<Student>().Where(c => c.id < 20).GroupBy(it => it.sex).GroupBy(it=>it.id).Select("id,sex,Count=count(*)").ToDynamic();
                List<SexTotal> list5 = db.Queryable<Student>().Where(c => c.id < 20).GroupBy(it => it.sex).Select<Student, SexTotal>("Sex,Count=count(*)").ToList();
                List<SexTotal> list6 = db.Queryable<Student>().Where(c => c.id < 20).GroupBy("sex").Select<Student, SexTotal>("Sex,Count=count(*)").ToList();
                //SELECT Sex,Count=count(*)  FROM Student  WHERE 1=1  AND  (id < 20)    GROUP BY Sex 
                
                
                //join
                var jList = db.Queryable<Student>()
                .JoinTable<Student, School>((s1, s2) => s1.sch_id == s2.id) //默认left join
                .Where<Student, School>((s1, s2) => s1.id == 1)
                .Select("s1.*,s2.name as schName")
                .ToDynamic();
                
                //join by page
                var jList2 = db.Queryable<Student>()
                .JoinTable<Student, School>((s1, s2) => s1.sch_id == s2.id) //default left join
               //inner join
               //.JoinTable<Student, School>((s1, s2) => s1.sch_id == s2.id  ,JoinType.INNER)
                .Where<Student, School>((s1, s2) => s1.id > 1)
                .OrderBy<Student, School>((s1, s2) => s1.name)
                .Skip(10)
                .Take(20)
                .Select("s1.*,s2.name as schName")
                .ToDynamic();
                
                //join select new 
                 var jList3 = db.Queryable<Student>()
                 .JoinTable<Student, School>((s1, s2) => s1.sch_id == s2.id) // left join  School s2  on s1.id=s2.id
                 .Where<Student, School>((s1, s2) => s1.id > 1)  // where s1.id>1
                 .OrderBy<Student, School>((s1, s2) => s1.id) //order by s1.id no one  ordder   .oderBy().orderby  
                 .Skip(1)
                 .Take(2)
                 .Select<Student, School, classNew>((s1, s2) => new classNew() { newid = s1.id, newname = s2.name, xx_name = s1.name }) 
                 .ToList();
                
      }

# Complex query  Sqlable

     using (var db = SqlSugarClient("sever=.;sa=saxxxxxxxx"))
     {
     

                //select  multiple table 
                List<School> dataList = db.Sqlable()
                   .From("school", "s")
                   .Join("student", "st", "st.id", "s.id", JoinType.INNER)
                   .Join("student", "st2", "st2.id", "st.id", JoinType.LEFT)
                   .Where("s.id>100 and s.id<@id")
                   .Where("1=1")  
                   .OrderBy("id")
                   .SelectToList<School/*new model*/>("st.*", new { id = 1 });

                //page
                List<School> dataPageList = db.Sqlable()
                    .From("school", "s")
                    .Join("student", "st", "st.id", "s.id", JoinType.INNER)
                    .Join("student", "st2", "st2.id", "st.id", JoinType.LEFT)
                    .Where("s.id>100 and s.id<100")
                    .SelectToPageList<School>("st.*", "s.id", 1, 10);

                //page 
                List<School> dataPageList2 = db.Sqlable()
                    .From("school", "s")
                    .Join("student", "st", "st.id", "s.id", JoinType.INNER)
                    .Join("student", "st2", "st2.id", "st.id", JoinType.LEFT)
                    .Where("s.id>100 and s.id<100 and s.id in (select 1 )" )
                    .SelectToPageList<School>("st.*", "s.id", 1, 10);



                //--------Convert List Dynmaic OR Json-----//

                
                var list1 = db.Sqlable().From("student", "s").Join("school", "l", "s.sch_id", "l.id and l.id=@id", JoinType.INNER).SelectToDynamic("*", new { id = 1 });
                var list2 = db.Sqlable().From("student", "s").Join("school", "l", "s.sch_id", "l.id and l.id=@id", JoinType.INNER).SelectToJson("*", new { id = 1 });
                var list3 = db.Sqlable().From("student", "s").Join("school", "l", "s.sch_id", "l.id and l.id=@id", JoinType.INNER).SelectToDataTable("*", new { id = 1 });

               
                var list4 = db.Sqlable().From("student", "s").Join("school", "l", "s.sch_id", "l.id and l.id=@id", JoinType.INNER).SelectToPageDynamic("s.*", "l.id", 1, 10, new { id = 1 });
                var list5 = db.Sqlable().From("student", "s").Join("school", "l", "s.sch_id", "l.id and l.id=@id", JoinType.INNER).SelectToPageTable("s.*", "l.id", 1, 10, new { id = 1 });
                var list6 = db.Sqlable().From("student", "s").Join("school", "l", "s.sch_id", "l.id and l.id=@id", JoinType.INNER).SelectToPageDynamic("s.*", "l.id", 1, 10, new { id = 1 });


                //--------Assemble OBJECT-----//
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
                    sable = sable.Where("l.id in (select top 10 id from school)");//where in
                }
                var pars = new { id = id, name = name };
                int pageCount = sable.Count(pars);
                var list7 = sable.SelectToPageList<Student>("s.*", "l.id desc", 1, 20, pars);


    }
    
    
# SqlQuery  

               //ToList
                List<Student> list1 = db.SqlQuery<Student>("select * from Student");
                //ToList with parameters
                List<Student> list2 = db.SqlQuery<Student>("select * from Student where id=@id", new { id = 1 });
                //To Dynamic
                dynamic list3 = db.SqlQueryDynamic("select * from student");
                //To Json
                string list4 = db.SqlQueryJson("select * from student");
                //get int
                var list5 = db.SqlQuery<int>("select top 1 id from Student").SingleOrDefault();
                //get dictionary
                Dictionary<string, string> list6 = db.SqlQuery<KeyValuePair<string, string>>("select id,name from Student").ToDictionary(it => it.Key, it => it.Value);
                //get array
                var list7 = db.SqlQuery<string[]>("select top 1 id,name from Student").SingleOrDefault();
                //exex sp
                var spResult = db.SqlQuery<School>("exec sp_school @p1,@p2", new { p1 = 1, p2 = 2 });

                //get frist row frist column
                string v1 = db.GetString("select '张三' as name");
                int v2 = db.GetInt("select 1 as name");
                double v3 = db.GetDouble("select 1 as name");
                decimal v4 = db.GetDecimal("select 1 as name");
#select new


               var list2 = db.Queryable<Student>().Where(c => c.id < 10).Select(c => new classNew { newid = c.id, newname = c.name, xx_name = c.name }).ToList();// 

                var list3 = db.Queryable<Student>().Where(c => c.id < 10).Select(c => new { newid = c.id, newname = c.name, xx_name = c.name }).ToDynamic();// 

                var list4 = db.Queryable<Student>().Where(c => c.id < 10).Select("id as newid, name as newname ,name as xx_name").ToDynamic();// 
                
#Insert
             
             
               Student s = new Student()
                {
                    name = "张" + new Random().Next(1, int.MaxValue)
                };
                db.Insert(s);  


                List<Student> list = new List<Student>()
                {
                     new Student()
                {
                     name="张"+new Random().Next(1,int.MaxValue)
                },
                 new Student()
                {
                     name="张"+new Random().Next(1,int.MaxValue)
                }
                };

                db.InsertRange(list);  
#Update 
               //Specify column update
                db.Update<School>(new { name = "蓝翔2" }, it => it.id == id);
                db.Update<School, int>(new { name = "蓝翔2" }, 1, 3, 12);
                db.Update<School, string>(new { name = "蓝翔2" }, new string[] { "1", "2" });

                //update object
                db.Update(new School { id = id, name = "蓝翔2" });
                db.Update<School>(new School { id = id, name = "蓝翔2" }, it => it.id == id);


                // setting disable Update Columns
                db.DisableUpdateColumns = new string[] { "CreateTime" };//设置CreateTime不更新

                TestUpdateColumns updObj = new TestUpdateColumns()
                {
                    VGUID = Guid.Parse("542b5a27-6984-47c7-a8ee-359e483c8470"),
                    Name = "xx",
                    Name2 = "xx2",
                    IdentityField = 0,
                    CreateTime = null
                };

                //CreateTime no update 
                db.Update(updObj);
#Delete 

                //delete
                db.Delete<School, int>(10);
                db.Delete<School>(it => it.id > 100);
                db.Delete<School, string>(new string[] { "100", "101", "102" });

                //false delte 
                //db.FalseDelete<school>("is_del", 100);
                //sql = update school set is_del=0 where id in(100)
                //db.FalseDelete<school>("is_del", it=>it.id==100);
                
                
#Transaction                
                
                db.IsNoLock = true;// with (nolock)
                db.CommandTimeOut = 30000;
                try
                {
                    db.BeginTran();
                    //db.BeginTran(IsolationLevel.ReadCommitted);+3

                    db.CommitTran();
                }
                catch (Exception)
                {
                    db.RollbackTran(); 
                    throw;
                }
