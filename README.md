# Select

                //---------Queryable<T>---------//

     using (var db = SqlSugarClient("sever=.;sa=saxxxxxxxx"))
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
                int maxId1 = db.Queryable<Student>().Max(it => it.id).ObjToInt();//拉姆达
                int maxId2 = db.Queryable<Student>().Max<Student, int>("id"); //字符串写法

                //min id
                int minId1 = db.Queryable<Student>().Where(c => c.id > 0).Min(it => it.id).ObjToInt();//拉姆达
                int minId2 = db.Queryable<Student>().Where(c => c.id > 0).Min<Student, int>("id");//字符串写法


                //order by 
                var orderList = db.Queryable<Student>().OrderBy("id desc,name asc").ToList();//字符串支持多个排序
                //multiple order by
                var order2List = db.Queryable<Student>().OrderBy(it=>it.name).OrderBy(it => it.id, OrderByType.desc).ToList(); // order by name as ,order by id desc

                //in
                var list1 = db.Queryable<Student>().In("id", "1", "2", "3").ToList();
                var list2 = db.Queryable<Student>().In("id", new string[] { "1", "2", "3" }).ToList();
                var list3 = db.Queryable<Student>().In("id", new List<string> { "1", "2", "3" }).ToList();
                var list4 = db.Queryable<Student>().Where(it => it.id < 10).In("id", new List<string> { "1", "2", "3" }).ToList();

                //group by
                var list7 = db.Queryable<Student>().Where(c => c.id < 20).GroupBy(it => it.sex).Select("sex,Count=count(*)").ToDynamic();
                var list8 = db.Queryable<Student>().Where(c => c.id < 20).GroupBy(it => it.sex).GroupBy(it=>it.id).Select("id,sex,Count=count(*)").ToDynamic();
                List<SexTotal> list5 = db.Queryable<Student>().Where(c => c.id < 20).GroupBy(it => it.sex).Select<Student, SexTotal>("Sex,Count=count(*)").ToList();
                List<SexTotal> list6 = db.Queryable<Student>().Where(c => c.id < 20).GroupBy("sex").Select<Student, SexTotal>("Sex,Count=count(*)").ToList();
                //SELECT Sex,Count=count(*)  FROM Student  WHERE 1=1  AND  (id < 20)    GROUP BY Sex --生成结果

