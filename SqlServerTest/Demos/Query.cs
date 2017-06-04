using OrmTest.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest.Demo
{
    public class Query:DemoBase
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
            var result= db.Ado.UseStoredProcedure<dynamic>(() =>
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
                var p2= new SugarParameter("@p2", null,true);//isOutput=true
                var dbResult= db.Ado.SqlQueryDynamic(spName,new SugarParameter[] {p1,p2 });
                outPutValue = p2.Value;
                return dbResult;
            });
        }
        private static void Tran()
        {
            var db = GetInstance();

            //1. no result 
            var result = db.Ado.UseTran(() =>
               {
                   var beginCount = db.Queryable<Student>().Count();
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
        }
        private static void Group()
        {
            var db = GetInstance();
            var list = db.Queryable<Student>()
                .GroupBy(it => it.Name)
                .GroupBy(it => it.Id).Having(it => SqlFunc.AggregateAvg(it.Id) > 0)
                .Select(it => new { idAvg = SqlFunc.AggregateAvg(it.Id), name = it.Name }).ToList();

            //SQL:
            //SELECT AVG([Id]) AS[idAvg], [Name] AS[name]  FROM[Student] GROUP BY[Name],[Id] HAVING(AVG([Id]) > 0 )

            //SqlFunc.AggregateSum(object thisValue) 
            //SqlFunc.AggregateAvg<TResult>(TResult thisValue)
            //SqlFunc.AggregateMin(object thisValue) 
            //SqlFunc.AggregateMax(object thisValue) 
            //SqlFunc.AggregateCount(object thisValue) 
        }
        private static void Ado()
        {
            var db = GetInstance();
            var t1 = db.Ado.SqlQuery<string>("select 'a'");
            var t2 = db.Ado.GetInt("select 1");
            var t3 = db.Ado.GetDataTable("select 1 as id");
            //more
            //db.Ado.GetXXX...
        }
        public static void Easy()
        {
            var db = GetInstance();
            var getAll = db.Queryable<Student>().ToList();
            var getAllNoLock = db.Queryable<Student>().With(SqlWith.NoLock).ToList();
            var getByPrimaryKey = db.Queryable<Student>().InSingle(2);
            var getByWhere = db.Queryable<Student>().Where(it => it.Id == 1 || it.Name == "a").ToList();
            var getByFuns = db.Queryable<Student>().Where(it => SqlFunc.IsNullOrEmpty(it.Name)).ToList();
            var sum = db.Queryable<Student>().Sum(it => it.Id);
            var isAny = db.Queryable<Student>().Where(it => it.Id == -1).Any();
            var isAny2 = db.Queryable<Student>().Any(it => it.Id == -1);
            var getListByRename = db.Queryable<School>().AS("Student").ToList();
            var in1 = db.Queryable<Student>().In(it=>it.Id,new int[] { 1, 2, 3 }).ToList();
            var in2 = db.Queryable<Student>().In(new int[] { 1, 2, 3 }).ToList();
            int[] array = new int[] { 1, 2 };
            var in3 = db.Queryable<Student>().Where(it=>SqlFunc.ContainsArray(array, it.Id)).ToList();
            var group = db.Queryable<Student>().GroupBy(it => it.Id)
                .Having(it => SqlFunc.AggregateCount(it.Id) > 10)
                .Select(it => new { id = SqlFunc.AggregateCount(it.Id) }).ToList();

            var between = db.Queryable<Student>().Where(it => SqlFunc.Between(it.Id, 1, 20)).ToList();

            var getTodayList = db.Queryable<Student>().Where(it => SqlFunc.DateIsSame(it.CreateTime, DateTime.Now)).ToList();
        }
        public static void Page()
        {
            var db = GetInstance();
            var pageIndex = 1;
            var pageSize = 2;
            var totalCount = 0;
            //page
            var page = db.Queryable<Student>().OrderBy(it=>it.Id).ToPageList(pageIndex, pageSize, ref totalCount);

            //page join
            var pageJoin = db.Queryable<Student, School>((st, sc) => new object[] {
              JoinType.Left,st.SchoolId==sc.Id
            }).ToPageList(pageIndex, pageSize, ref totalCount);

            //top 5
            var top5 = db.Queryable<Student>().Take(5).ToList();

            //skip5
            var skip5 = db.Queryable<Student>().Skip(5).ToList();
        }
        public static void Where()
        {
            var db = GetInstance();
            //join 
            var list = db.Queryable<Student, School>((st, sc) => new object[] {
              JoinType.Left,st.SchoolId==sc.Id
            })
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
        }
        public static void Join()
        {
            var db = GetInstance();
            //join  2
            var list = db.Queryable<Student, School>((st, sc) => new object[] {
              JoinType.Left,st.SchoolId==sc.Id
            })
            .Where(st => st.Name == "jack").ToList();

            //join  3
            var list2 = db.Queryable<Student, School, Student>((st, sc, st2) => new object[] {
              JoinType.Left,st.SchoolId==sc.Id,
              JoinType.Left,st.SchoolId==st2.Id
            })
            .Where((st, sc, st2) => st2.Id == 1 || sc.Id == 1 || st.Id == 1).ToList();

            //join return List<ViewModelStudent>
            var list3 = db.Queryable<Student, School>((st, sc) => new object[] {
              JoinType.Left,st.SchoolId==sc.Id
            }).Select((st, sc) => new ViewModelStudent { Name = st.Name, SchoolId = sc.Id }).ToList();

            //join Order By (order by st.id desc,sc.id desc)
            var list4 = db.Queryable<Student, School>((st, sc) => new object[] {
              JoinType.Left,st.SchoolId==sc.Id
            })
            .OrderBy(st => st.Id, OrderByType.Desc)
            .OrderBy((st, sc) => sc.Id, OrderByType.Desc)
            .Select((st, sc) => new ViewModelStudent { Name = st.Name, SchoolId = sc.Id }).ToList();
        }
        public static void Funs()
        {
            var db = GetInstance();
            var t1 = db.Queryable<Student>().Where(it => SqlFunc.ToLower(it.Name) == SqlFunc.ToLower("JACK")).ToList();
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
            var s1 = db.Queryable<Student>().Select(it => new ViewModelStudent2 { Name = it.Name, Student = it }).ToList();
            var s2 = db.Queryable<Student>().Select(it => new { id = it.Id, w = new { x = it } }).ToList();
            var s3 = db.Queryable<Student>().Select(it => new { newid = it.Id }).ToList();
            var s4 = db.Queryable<Student>().Select(it => new { newid = it.Id, obj = it }).ToList();
            var s5 = db.Queryable<Student>().Select(it => new ViewModelStudent2 { Student = it, Name = it.Name }).ToList();
            var s6 = db.Queryable<Student, School>((st, sc) => new object[] {
              JoinType.Left,st.SchoolId==sc.Id
            })
         .OrderBy(st => st.Id, OrderByType.Desc)
         .OrderBy((st, sc) => sc.Id, OrderByType.Desc)
         .Select((st, sc) => new { Name = st.Name, SchoolId = sc.Id }).ToList();
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
           // var list = db.Queryable<StudentEnum>().AS("Student").Where(it=>it.SchoolId== SchoolEnum.HarvardUniversity).ToList();
        }
    }
}
