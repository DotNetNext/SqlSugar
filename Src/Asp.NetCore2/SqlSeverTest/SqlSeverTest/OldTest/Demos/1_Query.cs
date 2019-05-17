using OrmTest.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest.Demo
{
    public class Query : DemoBase
    {

        public static void Init()
        {
            Easy();
            Page();
            Where();
            Join();
            Funs();
            Select();
            Ado();
            Group();
            Sqlable();
            Tran();
            StoredProcedure();
            Enum();
            Simple();
            Async();
            Subqueryable();
            SqlQueryable();
        }

        private static void SqlQueryable()
        {
            var db = GetInstance();
            var list = db.SqlQueryable<Student>("select * from student").ToPageList(1, 2);
        }

        private static void Subqueryable()
        {
            var db = GetInstance();
            var i = 0;


            var sumflat2num = db.Queryable<Student, Student>((s1, s2) => 
            new object[] { JoinType.Left, s1.Id == s2.Id })
 
            .Select((s1, s2) => new Student
            {  Id = SqlFunc.IsNull(SqlFunc.AggregateSum(SqlFunc.IIF(s1.Id ==1, s1.Id, s1.Id * -1)), 0) })
            .First();

            var getAll11 = db.Queryable<Student>().Where(it => SqlFunc.Subqueryable<School>().Where(s => s.Id == it.Id).Max(s=>s.Id)==i).ToList();
            var getAll12 = db.Queryable<Student>().Where(it => SqlFunc.Subqueryable<School>().Where(s => s.Id == it.Id).Max(s => s.Id) == 1).ToList();
            var getAll7 = db.Queryable<Student>().Where(it => SqlFunc.Subqueryable<School>().Where(s => s.Id == it.Id).Any()).ToList();

            var getAll9 = db.Queryable<Student>().Where(it => SqlFunc.Subqueryable<School>().Where(s => s.Id == it.Id).Count()==1).ToList();

            var getAll10 = db.Queryable<Student>().Where(it => SqlFunc.Subqueryable<School>().Where(s => s.Id == it.Id).OrderBy(s=>s.Id).Select(s=>s.Id) == 1).ToList();
            var getAll14 = db.Queryable<Student>().Where(it => SqlFunc.Subqueryable<School>().Where(s => s.Id == it.Id).OrderByDesc(s => s.Id).Select(s => s.Id) == 1).ToList();

            var getAll8= db.Queryable<Student>().Where(it => SqlFunc.Subqueryable<School>().Where(s => s.Id == it.Id).Where(s=>s.Name==it.Name).NotAny()).ToList();

            var getAll1 = db.Queryable<Student>().Where(it => it.Id == SqlFunc.Subqueryable<School>().Where(s => s.Id == it.Id).Select(s => s.Id)).ToList();

            var getAll2 = db.Queryable<Student, School>((st, sc) => new JoinQueryInfos(
                JoinType.Left,st.Id==sc.Id
            ))
          .Where(st => st.Id == SqlFunc.Subqueryable<School>().Where(s => s.Id == st.Id).Select(s => s.Id))
          .ToList();

            var getAll3 = db.Queryable<Student, School>((st, sc) => new JoinQueryInfos (
                JoinType.Left,st.Id==sc.Id
            ))
           .Select(st =>
                    new
                    {
                        name = st.Name,
                        id = SqlFunc.Subqueryable<School>().Where(s => s.Id == st.Id).Select(s => s.Id)
                    })
          .ToList();

            var getAll4 = db.Queryable<Student>().Select(it =>
                   new
                   {
                       name = it.Name,
                       id = SqlFunc.Subqueryable<School>().Where(s => s.Id == it.Id).Select(s => s.Id)
                   }).ToList();

            var getAll5 = db.Queryable<Student>().Select(it =>
                      new Student
                      {
                          Name = it.Name,
                          Id = SqlFunc.Subqueryable<School>().Where(s => s.Id == it.Id).Select(s => s.Id)
                      }).ToList();

            var getAll6 = db.Queryable<Student>().Select(it =>
                      new
                      {
                          name = it.Name,
                          id = SqlFunc.Subqueryable<Student>().Where(s => s.Id == it.Id).Sum(s => (int)s.SchoolId)
                      }).ToList();

            var getAll66 = db.Queryable<Student>().Select(it =>
                new
                {
                    name = it.Name,
                    id = SqlFunc.Subqueryable<Student>().Where(s => s.Id == it.Id).Sum(s =>s.SchoolId.Value)
                }).ToList();

            var getAll666 = db.Queryable<Student>().Select(it =>
                new
                {
                    name = it.Name,
                    id = SqlFunc.Subqueryable<Student>().Where(s => s.Id == it.Id).Min(s => s.Id)
                }).ToList();
            string name = "a";
            var getAll6666 = db.Queryable<Student>().Select(it =>
            new
            {
                name = it.Name,
                id = SqlFunc.Subqueryable<Student>().WhereIF(!string.IsNullOrEmpty(name), s=>s.Id==1).Min(s => s.Id)
            }).ToList();
            name = null;
            var getAll66666 = db.Queryable<Student>().Select(it =>
            new
            {
                name = it.Name,
                id = SqlFunc.Subqueryable<Student>().WhereIF(!string.IsNullOrEmpty(name), s => s.Id == 1).Min(s => s.Id)
            }).ToList();

            var getAll666666 = db.Queryable<Student>()
               .Where(it => SqlFunc.Subqueryable<School>().Where(s => s.Id == it.Id).Any())
              .Select(it =>
            new
            {
                name = it.Name,
                id = SqlFunc.Subqueryable<Student>().Where(s=>s.Id==SqlFunc.Subqueryable<School>().Where(y=>y.Id==s.SchoolId).Select(y=>y.Id)).Min(s => s.Id),
                id2 = SqlFunc.Subqueryable<Student>().Where(s => s.Id == SqlFunc.Subqueryable<School>().Where(y => y.Id == s.SchoolId).Select(y => y.Id)).Min(s => s.Id)
            }).ToList();


        }

        private static void Async()
        {
            var db = GetInstance();
            var list = db.Queryable<Student>().Where(it => it.Id == 1).SingleAsync();
            list.Wait();

            var list2 = db.Queryable<Student>().SingleAsync(it => it.Id == 1);
            list2.Wait();

            var list3 = db.Queryable<Student>().Where(it => it.Id == 1).ToListAsync();
            list3.Wait();

            var list4 = db.Queryable<Student>().Where(it => it.Id == 1).ToPageListAsync(1, 2);
            list4.Wait();
        }

        private static void Simple()
        {
            //SqlSugarClient
            var db = GetInstance();
            var student1 = db.Queryable<Student>().InSingle(1);

            //get SimpleClient
            var sdb = db.GetSimpleClient();
            var student2 = sdb.GetById<Student>(1);
            sdb.DeleteById<Student>(1);
            sdb.Insert(new Student() { Name = "xx" });
            sdb.Update<Student>(it => new Student { Name = "newvalue" }, it => it.Id == 1);//only update name where id=1
            sdb.Update(new Student() { Name = "newavalue", Id = 1 });//update all where id=1

            //SimpleClient Get SqlSugarClient
            var student3 = sdb.FullClient.Queryable<Student>().InSingle(1);

        }

        private static void StoredProcedure()
        {
            var db = GetInstance();
            //1. no result 
            db.Ado.UseStoredProcedure(() =>
            {
                string spName = "sp_help";
                var getSpReslut = db.Ado.SqlQueryDynamic(spName, new { objname = "student" });
            });

            //2. has result 
            var result = db.Ado.UseStoredProcedure<dynamic>(() =>
             {
                 string spName = "sp_help";
                 return db.Ado.SqlQueryDynamic(spName, new { objname = "student" });
             });

            //2. has output 
            object outPutValue;
            var outputResult = db.Ado.UseStoredProcedure<dynamic>(() =>
            {
                string spName = "sp_school";
                var p1 = new SugarParameter("@p1", "1");
                var p2 = new SugarParameter("@p2", null, true);//isOutput=true
                var dbResult = db.Ado.SqlQueryDynamic(spName, new SugarParameter[] { p1, p2 });
                outPutValue = p2.Value;
                return dbResult;
            });


            //3
            var dt = db.Ado.UseStoredProcedure().GetDataTable("sp_school", new { p1 = 1, p2 = 2 });


            var p11 = new SugarParameter("@p1", "1");
            var p22 = new SugarParameter("@p2", null, true);//isOutput=true
            //4
            var dt2 = db.Ado.UseStoredProcedure().SqlQuery<School>("sp_school", p11, p22);
        }
        private static void Tran()
        {
            var db = GetInstance();
            var x = db.Insertable(new Student() { CreateTime = DateTime.Now, Name = "tran" }).ExecuteCommand();
            //1. no result 
            var result = db.Ado.UseTran(() =>
               {

                   var beginCount = db.Queryable<Student>().ToList();
                   db.Ado.ExecuteCommand("delete student");
                   var endCount = db.Queryable<Student>().Count();
                   throw new Exception("error haha");
               });
            var count = db.Queryable<Student>().Count();

            //2 has result 
            var result2 = db.Ado.UseTran<List<Student>>(() =>
            {
                return db.Queryable<Student>().ToList();
            });

            //3 use try
            try
            {
                db.Ado.BeginTran();

                db.Ado.CommitTran();
            }
            catch (Exception)
            {
                db.Ado.RollbackTran();
                throw;
            }



            //async tran
            var asyncResult = db.Ado.UseTranAsync(() =>
            {

                var beginCount = db.Queryable<Student>().ToList();
                db.Ado.ExecuteCommand("delete student");
                var endCount = db.Queryable<Student>().Count();
                throw new Exception("error haha");
            });
            asyncResult.Wait();
            var asyncCount = db.Queryable<Student>().Count();

            //async
            var asyncResult2 = db.Ado.UseTranAsync<List<Student>>(() =>
            {
                return db.Queryable<Student>().ToList();
            });
            asyncResult2.Wait();
        }
        private static void Group()
        {
            var db = GetInstance();
            var list = db.Queryable<Student>()
                .GroupBy(it => it.Name)
                .GroupBy(it => it.Id).Having(it => SqlFunc.AggregateAvg(it.Id) > 0)
                .Select(it => new { idAvg = SqlFunc.AggregateAvg(it.Id), name = it.Name }).ToList();


            var list2 = db.Queryable<Student>()
             .GroupBy(it => new { it.Id, it.Name }).Having(it => SqlFunc.AggregateAvg(it.Id) > 0)
             .Select(it => new { idAvg = SqlFunc.AggregateAvg(it.Id), name = it.Name }).ToList();
            //SQL:
            //SELECT AVG([Id]) AS[idAvg], [Name] AS[name]  FROM[Student] GROUP BY[Name],[Id] HAVING(AVG([Id]) > 0 )

            // group id,name take first
            var list3 = db.Queryable<Student>()
                .PartitionBy(it => new { it.Id, it.Name }).Take(1).ToList();
            var list31 = db.Queryable<Student>()
                .PartitionBy(it => new { it.Id, it.Name }).Take(1).Count();

            int count = 0;

            var list4 = db.Queryable<Student, School>((st, sc) => st.SchoolId == sc.Id)
               .PartitionBy(st => new { st.Name }).Take(2).OrderBy(st => st.Id, OrderByType.Desc).Select(st => st).ToPageList(1, 1000, ref count);

            //SqlFunc.AggregateSum(object thisValue) 
            //SqlFunc.AggregateAvg<TResult>(TResult thisValue)
            //SqlFunc.AggregateMin(object thisValue) 
            //SqlFunc.AggregateMax(object thisValue) 
            //SqlFunc.AggregateCount(object thisValue) 
        }
        private static void Ado()
        {
            var db = GetInstance();
            db.Ado.BeginTran();
            var t1 = db.Ado.SqlQuery<string>("select 'a'");
            var t2 = db.Ado.GetInt("select 1");
            var t3 = db.Ado.GetDataTable("select 1 as id");
            var t4 = db.Ado.GetScalar("select * from student where id in (@id) ", new { id = new List<int>() { 1, 2, 3 } });
            var t5 = db.Ado.GetScalar("select * from student where id in (@id) ", new { id = new  int [] { 1, 2, 3 } });
            var t6= db.Ado.GetScalar("select * from student where id in (@id) ",  new SugarParameter("@id", new int[] { 1, 2, 3 }));
            db.Ado.CommitTran();
            var t11 = db.Ado.SqlQuery<Student>("select * from student");
            //more
            //db.Ado.GetXXX...
        }
        public static void Easy()
        {
            var db = GetInstance();
            var dbTime = db.GetDate();
            var getAll = db.Queryable<Student>().Select<object>("*").ToList();
            var getAll2 = db.Queryable<Student>().Select(it=>it.Name.Substring(0,4)).ToList();
            var getAll22 = db.Queryable<Student>().ToDataTable();
            var getAll222 = db.Queryable<Student>().ToJson();
            var getAll22222 = db.Queryable<Student>().ToArray();
            var getAll2222 = db.Queryable<Student>().OrderBy(it=>it.Name.Length).ToJson();
            var getAll3 = db.Queryable<Student>().OrderBy(it => new { it.Id, it.Name }).GroupBy(it => new { it.Id, it.Name }).Select<object>("id").ToList();
            var getRandomList = db.Queryable<Student>().OrderBy(it => SqlFunc.GetRandom()).ToList();
            var getAllOrder = db.Queryable<Student>().OrderBy(it => it.Id).OrderBy(it => it.Name, OrderByType.Desc).ToList();
            var getId = db.Queryable<Student>().Select(it => it.Id).ToList();
            var getNew = db.Queryable<Student>().Where(it => it.Id == 1).Select(it => new { id = SqlFunc.IIF(it.Id == 0, 1, it.Id), it.Name, it.SchoolId }).ToList();
            var getAllNoLock = db.Queryable<Student>().With(SqlWith.NoLock).ToList();
            var getByPrimaryKey = db.Queryable<Student>().InSingle(2);
            var getSingleOrDefault = db.Queryable<Student>().Where(it => it.Id == 1).Single();
            var getFirstOrDefault = db.Queryable<Student>().First();
            var getByWhere = db.Queryable<Student>().Where(it => it.Id == 1 || it.Name == "a").ToList();
            var getByWhere2 = db.Queryable<Student>().Where(it => it.Id ==DateTime.Now.Year).ToList();
            var getByFuns = db.Queryable<Student>().Where(it => SqlFunc.IsNullOrEmpty(it.Name)).ToList();
            var sum = db.Queryable<Student>().Select(it => it.SchoolId).ToList();
            var sum2 = db.Queryable<Student, School>((st, sc) => st.SchoolId == sc.Id).Sum((st, sc) => sc.Id);
            var isAny = db.Queryable<Student>().Where(it => it.Id == -1).Any();
            var isAny2 = db.Queryable<Student>().Any(it => it.Id == -1);
            var count = db.Queryable<Student>().Count(it => it.Id > 0);
            var date = db.Queryable<Student>().Where(it => it.CreateTime.Value.Date == DateTime.Now.Date).ToList();
            var getListByRename = db.Queryable<School>().AS("Student").ToList();
            var in1 = db.Queryable<Student>().In(it => it.Id, new int[] { 1, 2, 3 }).ToList();
            var in2 = db.Queryable<Student>().In(new int[] { 1, 2, 3 }).ToList();
            int[] array = new int[] { 1, 2 };
            var in3 = db.Queryable<Student>().Where(it => SqlFunc.ContainsArray(array, it.Id)).ToList();
            var group = db.Queryable<Student>().GroupBy(it => it.Id)
                .Having(it => SqlFunc.AggregateCount(it.Id) > 10)
                .Select(it => new { id = SqlFunc.AggregateCount(it.Id) }).ToList();

            var between = db.Queryable<Student>().Where(it => SqlFunc.Between(it.Id, 1, 20)).ToList();

            var getTodayList = db.Queryable<Student>().Where(it => SqlFunc.DateIsSame(it.CreateTime, DateTime.Now)).ToList();

            var joinSql = db.Queryable("student", "s").OrderBy("id").Select("id,name").ToPageList(1, 2);

            var getDay1List = db.Queryable<Student>().Where(it => it.CreateTime.Value.Hour == 1).ToList();
            var getDateAdd = db.Queryable<Student>().Where(it => it.CreateTime.Value.AddDays(1) == DateTime.Now).ToList();
            var getDateIsSame = db.Queryable<Student>().Where(it => SqlFunc.DateIsSame(DateTime.Now, DateTime.Now, DateType.Hour)).ToList();

            var getSqlList = db.Queryable<Student>().AS("(select * from student) t").ToList();


            var getUnionAllList = db.UnionAll(db.Queryable<Student>().Where(it => it.Id == 1), db.Queryable<Student>().Where(it => it.Id == 2)).ToList();

            var getUnionAllList2 = db.UnionAll(db.Queryable<Student>(), db.Queryable<Student>()).ToList();

            var getUnionAllList3= db.UnionAll(db.Queryable<Student>()
                .Select(it => new Student { Id =SqlFunc.ToInt32(1) ,Name=SqlFunc.ToString("2"), SchoolId = Convert.ToInt32(3) })
                , db.Queryable<Student>()
                .Select(it => new Student { Id = SqlFunc.ToInt32(11) , Name = SqlFunc.ToString("22") , SchoolId=Convert.ToInt32(33)}))
                .Select(it=>new Student() { Id=SqlFunc.ToInt32(111), Name = SqlFunc.ToString("222") }).ToList();

            var test1 = db.Queryable<Student, School>((st, sc) => st.SchoolId == sc.Id).Where(st=>st.CreateTime>SqlFunc.GetDate()).Select((st, sc) => SqlFunc.ToInt64(sc.Id)).ToList();
            var test2 = db.Queryable<Student, School>((st, sc) => st.SchoolId == sc.Id)
                      .Where(st =>
                        SqlFunc.IF(st.Id > 1)
                             .Return(st.Id)
                             .ElseIF(st.Id == 1)
                             .Return(st.SchoolId).End(st.Id) == 1).Select(st=>st).ToList();
            var test3 = db.Queryable<DataTestInfo2>().Select(it => it.Bool1).ToSql();
            var test4 = db.Queryable<DataTestInfo2>().Select(it => new { b=it.Bool1 }).ToSql();
            DateTime? result = DateTime.Now;
            var test5 = db.Queryable<Student>().Where(it=>it.CreateTime> result.Value.Date).ToList();

            var test6 = db.Queryable<DataTestInfo2>().Where(it => SqlFunc.HasValue(it.Bool2)==true && SqlFunc.HasValue(it.Bool2)==true).ToList();
            var test7 = db.Queryable<DataTestInfo2>().Where(it => SqlFunc.HasValue(it.Bool1) && SqlFunc.HasValue(it.Bool1)).ToList();
            var test8 = db.Queryable<Student>().Where(it => SqlFunc.HasValue(it.SchoolId) && SqlFunc.HasValue(it.SchoolId)).ToList();
            bool? b = false;
            var test9 = db.Queryable<DataTestInfo2>().Where(it => it.Bool1 == b).ToList();
            var test10 = db.Queryable<Student>(db.Queryable<Student>().Select(it => new Student() { Name = it.Name.Substring(0, 1) })).GroupBy(it => it.Name).ToList(); ;
            var test11 = db.Queryable<Student>().Distinct().ToList();
            var test12 = db.Queryable<Student>().Distinct().Select(it=>new Student{ Name=it.Name  }).ToList();
            var test13 = db.Queryable<Student>().Where(it=>DateTime.Parse("2014-1-1")==DateTime.Now).Where(it => Boolean.Parse("true") ==true).ToList();
            var test14 = db.Queryable<DataTestInfo2>().Where(it =>Convert.ToBoolean(it.Bool1)).ToList();
            var test15 = db.Queryable<DataTestInfo2>().Where(it => it.Bool2.Value&&it.Bool1).ToList();
            var test16 = db.Queryable<DataTestInfo2>().Where(it => !it.Bool2.Value && !it.Bool1).ToList();
            var test17 = db.Queryable<DataTestInfo2>().Where(it => it.Bool1 && it.Bool1).ToList();
            var test18 = db.Queryable<Student>().Where(it => it.SchoolId.HasValue&&it.SchoolId.HasValue).ToList();
            var test19 = db.Queryable<Student>().Where(it => it.SchoolId.HasValue && it.SchoolId.HasValue&&it.SchoolId.HasValue).ToList();
            var test20 = db.Queryable<Student>().Where(it => it.SchoolId.HasValue && SqlFunc.IsNullOrEmpty(it.Name)).ToList();
            var test21 = db.Queryable<Student>().Where(it => !it.SchoolId.HasValue && it.Name == "").ToList();
            var test22 = db.Queryable<Student>().Where(it => !it.SchoolId.HasValue && it.SchoolId.HasValue).ToList();
            var test23 = db.Queryable<Student>().Where(it => !(it.Id==1) && it.Name=="").ToList();
            var test24 = db.Queryable<Student>().Where(it => string.IsNullOrEmpty("a")).Where(it=>string.IsNullOrEmpty(it.Name)).ToList();
            var test25 = db.Queryable<Student>().Where(it => SqlFunc.IIF(it.Id==0,1,2)==1).ToList();
            var test26 = db.Queryable<Student>().Where(it => (it.Name==null?2:3)==1 )
                .ToList();
            var test27 = db.Queryable<Student>().Select(x => new {
                name=x.Name==null?"1":"2"
            }).ToList();
            var test28 = db.Queryable<Student>().Select(x => new Student{
                Name = x.Name == null ? "1" : "2"
            }).ToList();
            var test29 = db.Queryable<Student>().Where(it=>it.Id%1==0).ToList();
            var test30 = db.Queryable<Student>().Select(x => new Student
            {
                Name = x.Name ?? "a"
            }).ToList();
            var test31 = db.Queryable<Student>().Where(it=>(it.Name??"a")=="a").ToList();
            var test32 = db.Queryable<Student>().Where(it => it.Name == null ? true : false).ToList();
            var test33 = db.Queryable<Student>().Where(it => SqlFunc.IIF(it.Name==null,true ,false)).ToList();
            var test34 = db.Queryable<Student>().Where(it => SqlFunc.IIF(it.Name == null||1==1, true, false)).ToList();
            var test35 = db.Queryable<Student>().Where(it =>it.Id==1&&SqlFunc.IF(it.Id==1).Return(true).End(false)).ToList();
            var test36 = db.Queryable<Student>().Where(it => it.Id == 1 &&it.SchoolId.HasValue).ToList();
            var test37 = db.Queryable<Student>().Where(it => it.Id == 1 && SqlFunc.IIF(it.Id == 1, true, false)).ToList();
            var test38 = db.Queryable<Student>().Where(it => it.Id == 1 && SqlFunc.IIF(it.Id == 1, true, false)==true).ToList();
            var test39 = db.Queryable<Student>().Where(it => it.Id == 1 && (it.Id==1?true:false)).ToList();
            var test40 = db.Queryable<Student>().Where(it => it.Id==1&&Convert.ToBoolean("true")).ToList();
            var test41 = db.Queryable<Student>().Where(it => it.Id==((it.Id==1?2:3)==2?1:2)).ToList();
            var test42 = db.Queryable<Student>().Where(it => new int[] { 1, 2, 3 }.Contains(1)).ToList();
            var test43 = db.Queryable<Student>().Where(it => new int[] { 1, 2, 3 }.Contains(it.Id)).ToList();
          
            var test44 = db.Queryable<Student>().Select(it=>new {
                x= SqlFunc.Subqueryable<DataTestInfo>().Where(x => false).Sum(x => x.Decimal1)
            }).ToList();
            decimal? p = null;
            var test45 = db.Queryable<DataTestInfo>().Select(it => new {
                x =p
            }).ToList();
            var test46 = db.Queryable<Student>().Where(it => it.CreateTime > SqlFunc.ToDate(DateTime.Now.Date)).ToList();
            var test47 = db.Queryable<Student>().Where(it =>string.IsNullOrEmpty(it.Name)==true).ToList();
            var test48 = db.Queryable<Student>().Where(it=>it.CreateTime!=null).Where(it => SqlFunc.ToDate(it.CreateTime).Date==DateTime.Now.Date).ToList();
            var test49 = db.Queryable<Student>().Where(it => it.CreateTime != null).Where(it => SqlFunc.ToDate(it.CreateTime).Year == DateTime.Now.Year).ToList();
            var test50 = db.Queryable<Student>().Where(it => it.CreateTime != null).Where(it => SqlFunc.ToDate(it.CreateTime).Year == SqlFunc.GetDate().Year).ToList();
            var test51 = db.Queryable<Student>().Select(it=>new { x= SqlFunc.ToDate(it.CreateTime).Year+"-" }).ToList();
            var test52 = db.Queryable<Student>().Select(it => SqlFunc.IsNull(it.CreateTime, SqlFunc.GetDate())).ToList();
            var test53 = db.Queryable<Student>().Select(it => SqlFunc.IsNull(it.CreateTime, SqlFunc.GetDate())).First();
            var test54 = db.Queryable<Student>().Where(it => it.CreateTime == test52.First().Value).ToList();
            var test55 = db.Queryable<Student>().Select(it => new {
                isAny = SqlFunc.Subqueryable<School>().Any()?1:2
            }).ToList();
            var test56= db.Queryable<Student>().Select(it=> new {
                isAny=SqlFunc.Subqueryable<Student>().Any(),
                isAny2 = SqlFunc.Subqueryable<Student>().Where(s=>false).Any()
            }).ToList();
            var totalPage = 0;
            var total = 0;
            db.Queryable<Student>().ToPageList(1, 2, ref total, ref totalPage);
        }
 
        public static void Page()
        {
            var db = GetInstance();
            var pageIndex = 1;
            var pageSize = 2;
            var totalCount = 0;
            //page
            var page = db.Queryable<Student>().OrderBy(it => it.Id).ToPageList(pageIndex, pageSize, ref totalCount);

            //page join
            var pageJoin = db.Queryable<Student, School>((st, sc) => new JoinQueryInfos(
              JoinType.Left,st.SchoolId==sc.Id
            )).ToPageList(pageIndex, pageSize, ref totalCount);

            //top 5
            var top5 = db.Queryable<Student>().Take(5).ToList();

            //skip5
            var skip5 = db.Queryable<Student>().Skip(5).ToList();
        }
        public static void Where()
        {
            var db = GetInstance();
            //join 
            var list = db.Queryable<Student, School>((st, sc) =>new JoinQueryInfos(
              JoinType.Left,st.SchoolId==sc.Id
            ))
            .Where((st, sc) => sc.Id == 1)
            .Where((st, sc) => st.Id == 1)
            .Where((st, sc) => st.Id == 1 && sc.Id == 2).ToList();

            //SELECT [st].[Id],[st].[SchoolId],[st].[Name],[st].[CreateTime] FROM [Student] st 
            //Left JOIN School sc ON ( [st].[SchoolId] = [sc].[Id] )   
            //WHERE ( [sc].[Id] = @Id0 )  AND ( [st].[Id] = @Id1 )  AND (( [st].[Id] = @Id2 ) AND ( [sc].[Id] = @Id3 ))


            //Where If
            string name = null;
            string name2 = "sunkaixuan";
            var list2 = db.Queryable<Student>()
                 .WhereIF(!string.IsNullOrEmpty(name), it => it.Name == name)
                 .WhereIF(!string.IsNullOrEmpty(name2), it => it.Name == name2).ToList();



            //join 
            var list3 = db.Queryable<Student, School>((st, sc) => new JoinQueryInfos(
              JoinType.Left,st.SchoolId==sc.Id
            ))
            .WhereIF(false, (st, sc) => sc.Id == 1)
            .WhereIF(false, (st, sc) => st.Id == 1).ToList();


            var list4 = db.Queryable<Student, School>((st, sc) =>new JoinQueryInfos(
              JoinType.Left,st.SchoolId==sc.Id
            ))
            .Select((st, sc) => new { id = st.Id, school = sc }).ToList();


            var list5 = db.Queryable<Student, School>((st, sc) =>new JoinQueryInfos(
              JoinType.Left,st.SchoolId==sc.Id
            )).AS<Student>("STUDENT").AS<School>("SCHOOL")
.Select((st, sc) => new { id = st.Id, school = sc }).ToList();


            var list6 = db.Queryable<Student, School>((st, sc) =>new JoinQueryInfos(
              JoinType.Left,st.SchoolId==sc.Id
            )).With(SqlWith.NoLock).AS<Student>("STUDENT").AS<School>("SCHOOL")
.Select((st, sc) => new { id = st.Id, school = sc }).ToList();
        }
        public static void Join()
        {
            var db = GetInstance();
            //join  2
            var list = db.Queryable<Student, School>((st, sc) =>new JoinQueryInfos(
              JoinType.Left,st.SchoolId==sc.Id
            ))
            .Where(st => st.Name == "jack").ToList();

            //join  3
            var list2 = db.Queryable<Student, School, Student>((st, sc, st2) => new JoinQueryInfos(
              JoinType.Left,st.SchoolId==sc.Id,
              JoinType.Left,st.SchoolId==st2.Id
            ))
            .Where((st, sc, st2) => st2.Id == 1 || sc.Id == 1 || st.Id == 1).With(SqlWith.NoLock).ToList();

            //join return List<ViewModelStudent>
            var list3 = db.Queryable<Student, School>((st, sc) => new JoinQueryInfos(
              JoinType.Left,st.SchoolId==sc.Id
            )).Select((st, sc) => new ViewModelStudent { Name = st.Name, SchoolId = sc.Id }).ToList();

            //join Order By (order by st.id desc,sc.id desc)
            var list4 = db.Queryable<Student, School>((st, sc) =>new JoinQueryInfos (
              JoinType.Left,st.SchoolId==sc.Id
            ))
            .OrderBy(st => st.Id, OrderByType.Desc)
            .OrderBy((st, sc) => sc.Id, OrderByType.Desc)
            .Select((st, sc) => new ViewModelStudent { Name = st.Name, SchoolId = sc.Id }).ToList();


            //join  2
            var list4_1 = db.Queryable<Student, School>((st, sc) => new JoinQueryInfos(
              JoinType.Left,st.SchoolId==sc.Id&& st.Name == "jack"
            )).ToList();


            //The simple use of Join 2 table
            var list5 = db.Queryable<Student, School>((st, sc) => st.SchoolId == sc.Id).Select((st, sc) => new { st.Name, st.Id, schoolName = sc.Name }).ToList();

            //join 3 table
            var list6 = db.Queryable<Student, School, School>((st, sc, sc2) => st.SchoolId == sc.Id && sc.Id == sc2.Id)
                .Select((st, sc, sc2) => new { st.Name, st.Id, schoolName = sc.Name, schoolName2 = sc2.Name }).ToList();

            //join 3 table page
            var list7 = db.Queryable<Student, School, School>((st, sc, sc2) => st.SchoolId == sc.Id && sc.Id == sc2.Id)
            .Select((st, sc, sc2) => new { st.Name, st.Id, schoolName = sc.Name, schoolName2 = sc2.Name }).ToPageList(1, 2);

            //join 3 table page 
            var list8 = db.Queryable<Student, School, School>((st, sc, sc2) => st.SchoolId == sc.Id && sc.Id == sc2.Id)
            .OrderBy(st => st.Id)
            .Select((st, sc, sc2) => new { st.Name, st.Id, schoolName = sc.Name, schoolName2 = sc2.Name }).ToPageList(1, 2);

            //In
            var list9 = db.Queryable<Student>("it")
            .OrderBy(it => it.Id)
            .In(it => it.Id, db.Queryable<School>().Where("it.id=schoolId").Select(it => it.Id))
           .ToList();
            //SELECT [ID],[SchoolId],[Name],[CreateTime] FROM [STudent] it  WHERE [ID] 
            //IN (SELECT [Id] FROM [School]  WHERE it.id=schoolId ) ORDER BY [ID] ASC

            var list10 = db.Queryable<Student, School>((st, sc) => st.SchoolId == sc.Id)
            .In(st => st.Name, db.Queryable<School>("sc2").Where("id=st.schoolid").Select(it => it.Name))
            .OrderBy(st => st.Id)
            .Select(st => st)
            .ToList();
            //SELECT st.* FROM [STudent] st  ,[School]  sc  WHERE ( [st].[SchoolId] = [sc].[Id] )  AND [st].[Name] 
            //IN (SELECT [Name] FROM [School] sc2  WHERE id=st.schoolid ) ORDER BY [st].[ID] ASC

            var list11 = db.Queryable<Student, School>((st, sc) => st.SchoolId == sc.Id)
     .In(st => st.Name, db.Queryable<School>("sc2").Where(it => it.Id == 1).Where("id=st.schoolid").Select(it => it.Name))
     .OrderBy(st => st.Id)
     .Select(st => st)
     .ToList();

            var subquery = db.Queryable<Student>().Where(it => it.Id == 1);
            var subquery2 = db.Queryable<Student>();
            db.Queryable(subquery, subquery2, (st1, st2) => st1.Id == st2.Id).Select((st1,st2)=>new {
                id=st1.Id,
                name=st2.Name
            }).ToList();

            var q1 = db.Queryable<Student>().Select(it => new Student()
            {
                Id = it.Id,
                Name = "a"
            });
            var q2 = db.Queryable<Student>().Select(it => new Student()
            {
                Id = it.Id,
                Name = "b"
            });
            var unionAllList = db.Union(q1, q2).ToList();
        }
        public static void Funs()
        {
            var db = GetInstance();
            var t1 = db.Queryable<Student>().Where(it => SqlFunc.ToLower(it.Name) == SqlFunc.ToLower("JACK")).ToList();
            var t2 = db.Queryable<Student>().Where(it => SqlFunc.IsNull(it.Name,"nullvalue")=="nullvalue").ToList();
            var t3 = db.Queryable<Student>().Where(it => SqlFunc.MergeString("a",it.Name) == "nullvalue").ToList();
            //SELECT [Id],[SchoolId],[Name],[CreateTime] FROM [Student]  WHERE ((LOWER([Name])) = (LOWER(@MethodConst0)) )

            /***More Functions***/
            //SqlFunc.IsNullOrEmpty(object thisValue)
            //SqlFunc.ToLower(object thisValue) 
            //SqlFunc.string ToUpper(object thisValue) 
            //SqlFunc.string Trim(object thisValue) 
            //SqlFunc.bool Contains(string thisValue, string parameterValue) 
            //SqlFunc.ContainsArray(object[] thisValue, string parameterValue) 
            //SqlFunc.StartsWith(object thisValue, string parameterValue) 
            //SqlFunc.EndsWith(object thisValue, string parameterValue)
            //SqlFunc.Equals(object thisValue, object parameterValue) 
            //SqlFunc.DateIsSame(DateTime date1, DateTime date2)
            //SqlFunc.DateIsSame(DateTime date1, DateTime date2, DateType dataType) 
            //SqlFunc.DateAdd(DateTime date, int addValue, DateType millisecond) 
            //SqlFunc.DateAdd(DateTime date, int addValue) 
            //SqlFunc.DateValue(DateTime date, DateType dataType) 
            //SqlFunc.Between(object value, object start, object end) 
            //SqlFunc.ToInt32(object value) 
            //SqlFunc.ToInt64(object value)
            //SqlFunc.ToDate(object value) 
            //SqlFunc.ToString(object value) 
            //SqlFunc.ToDecimal(object value) 
            //SqlFunc.ToGuid(object value) 
            //SqlFunc.ToDouble(object value) 
            //SqlFunc.ToBool(object value) 
            //SqlFunc.Substring(object value, int index, int length)
            //SqlFunc.Replace(object value, string oldChar, string newChar)
            //SqlFunc.Length(object value) { throw new NotImplementedException(); }
            //SqlFunc.AggregateSum(object thisValue) 
            //SqlFunc.AggregateAvg<TResult>(TResult thisValue)
            //SqlFunc.AggregateMin(object thisValue) 
            //SqlFunc.AggregateMax(object thisValue) 
            //SqlFunc.AggregateCount(object thisValue) 
        }
        public static void Select()
        {
            var db = GetInstance();
            db.IgnoreColumns.Add("TestId", "Student");
            var s1 = db.Queryable<Student>().Where(it => it.Id == 136915).Single();
            var s2 = db.Queryable<Student>().Select(it => new { id = it.Id, w = new { x = it } }).ToList();
            var s3 = db.Queryable<Student>().Select(it => new { newid = it.Id }).ToList();
            var s4 = db.Queryable<Student>().Select(it => new { newid = it.Id, obj = it }).ToList();
            var s41 = db.Queryable<Student>().Select<dynamic>("*").ToList();
            var s5 = db.Queryable<Student>().Select(it => new ViewModelStudent2 { Student = it, Name = it.Name }).ToList();
            var s6 = db.Queryable<Student, School>((st, sc) => new object[] {
              JoinType.Left,st.SchoolId==sc.Id
            })
         .OrderBy(st => st.Id, OrderByType.Desc)
         .OrderBy((st, sc) => sc.Id, OrderByType.Desc)
         .Select((st, sc) => new { Name = st.Name, SchoolId = sc.Id }).ToList();


            var s7 = db.Queryable<Student, School>((st, sc) =>new JoinQueryInfos (
              JoinType.Left,st.SchoolId==sc.Id
            )).Select((st, sc) => sc).ToList();

            var s8 = db.Queryable<Student, School>((st, sc) =>new JoinQueryInfos(
              JoinType.Left,st.SchoolId==sc.Id
            ))
            .OrderBy((st, sc) => st.SchoolId)
            .Select((st, sc) => sc)
            .Take(1).ToList();

            var s9 = db.Queryable<Student>().Select(it=>new Student() { Id=it.Id, TestId=1, Name=it.Name, CreateTime=it.CreateTime }).First();
            var s10 = db.Queryable<Student>().Select(it => new Student() { Id = it.Id}).First();

            //auto fill
            var s11 = db.Queryable<Student, School>((st,sc)=>st.SchoolId==sc.Id).Select<ViewModelStudent3>().ToList();
        }
        private static void Sqlable()
        {
            var db = GetInstance();
            var join3 = db.Queryable("Student", "st")
                          .AddJoinInfo("School", "sh", "sh.id=st.schoolid")
                          .Where("st.id>@id")
                          .AddParameters(new { id = 1 })
                          .Select("st.*").ToList();
            //SELECT st.* FROM [Student] st Left JOIN School sh ON sh.id=st.schoolid   WHERE st.id>@id 
        }
        private static void Enum()
        {
            var db = GetInstance();
            var list = db.Queryable<StudentEnum>().AS("Student").Where(it => it.SchoolId == SchoolEnum.HarvardUniversity).ToList();
            var list2 = db.Queryable<StudentEnum>().AS("Student").Where(it => it.Name == SchoolEnum.HarvardUniversity.ToString()).ToList();
        }
    }
}
