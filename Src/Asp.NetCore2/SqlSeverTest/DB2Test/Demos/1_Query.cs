﻿using IBM.Data.DB2.Core;
using Oracle.ManagedDataAccess.Client;
using OrmTest.Models;
using SMESCore.Model;
using SMESCore.Model.BaseEntity;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest.Demo
{
    public class Query :DemoBase
    {

        public static void Init()
        {
            mytest();
            myordreby();
            //Easy();
            //Page();
            //Where();
            //Join();
            //Funs();
            //Select();
            //Ado();
            //Group();
            //Sqlable();
            //Tran();
            ////StoredProcedure();
            //Enum();
            //Simple();
        }
        public static void mytest()
        {
            var db = GetInstance();
            UnitTest.UnitTestBase testBase = new UnitTest.UnitTestBase();
            try
            {
                testBase.Begin();
                var connec = new DB2Connection(Config.ConnectionString);
                var cmd = new DB2Command("SELECT t.* FROM aps.equ_work_hours t join aps.equ_work_hours_ver t1 on t1.equ_work_hours_ver_id=t.equ_work_hours_ver_id", connec);
                var adp = new DB2DataAdapter(cmd);
                var dt = new DataTable();
                var dataset = adp.Fill(dt);
                testBase.End("joinadonettable");
            }
            catch (Exception ex)
            {

                throw;
            }
            try
            {
                testBase.Begin();
                var joinado = db.Ado.GetDataTable("SELECT t.* FROM aps.equ_work_hours t join aps.equ_work_hours_ver t1 on t1.equ_work_hours_ver_id=t.equ_work_hours_ver_id");
                testBase.End("joinadotable");
                testBase.Begin();
            }
            catch (Exception ex)
            {

                throw;
            }
            var join = db.Queryable<EquWorkHours, EquWorkHoursVer>((e,e1)=>new object[] { JoinType.Inner,e.EquWorkHoursVerId==e1.EquWorkHoursVerId})
                .Select((e,e1)=>new EquWorkhoursViewModel{ EquWorkHours=e,Factoryname=e1.Factoryid.ToString(),Vername=e1.Vername}).ToDataTable();
            testBase. End("jointable");
            var getequhouts = db.Queryable<EquWorkHours>().ToDataTable();
            var getver = db.Queryable<EquWorkHoursVer>().ToDataTable();
        }

        private static void myordreby()
        {
            var db = GetInstance();
            db.Ado.LogEventStarting = (sql, pars) =>
            {
                Console.WriteLine(sql + "\r\n" + db.Utilities.SerializeObject(pars.ToDictionary(it => it.ParameterName, it => it.Value)));
                Console.WriteLine();
            };
            var join = db.Queryable<EquWorkHours, EquWorkHoursVer>((e,e1)=>new object[] { JoinType.Inner, e.EquWorkHoursVerId==e1.EquWorkHoursVerId}).ToList();
            var joinorder = db.Queryable<EquWorkHours, EquWorkHoursVer>((e, e1) => new object[] { JoinType.Inner, e.EquWorkHoursVerId == e1.EquWorkHoursVerId })
                .OrderBy((e, e1) => e1.Vername, OrderByType.Asc).ToDataTable();
        }

        private static void Simple()
        {
            //SqlSugarClient
            var db = GetInstance();
            var student1 = db.Queryable<Student>().InSingle(1);

            //get SimpleClient
            var sdb = db.SimpleClient;
            var student2 = sdb.GetById<Student>(1);
            sdb.DeleteById<Student>(1);
            sdb.Insert(new Student() { Name = "xx" });
            sdb.Update<Student>(it => new Student { Name = "newvalue" }, it => it.Id == 1);//only update name where id=1
            sdb.Update(new Student() { Name="newavalue" ,Id=1});//update all where id=1

            //SimpleClient Get SqlSugarClient
            var student3=sdb.FullClient.Queryable<Student>().InSingle(1);

        }

        private static void StoredProcedure()
        {
            var db = GetInstance();
 
            object outPutValue;
            var outputResult = db.Ado.UseStoredProcedure<dynamic>(() =>
            {
                string spName = "SpGetSchool";
                var p1 = new SugarParameter("@id_in", "1");
                var p2 = new SugarParameter("@name_out", null, true);//isOutput=true
                var dbResult = db.Ado.ExecuteCommand(spName, new SugarParameter[] { p1, p2 });
                outPutValue = p2.Value;
                return dbResult;
            });
        }
        private static void Tran()
        {
            var db = GetInstance();
            var x=db.Insertable(new Student() { CreateTime = DateTime.Now, Name = "tran" }).ExecuteCommand();
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

            int count = 0;

            var list4 = db.Queryable<Student, School>((st, sc) => st.SchoolId == sc.Id)
               .PartitionBy(st => new { st.Name }).Take(1).OrderBy(st => st.Id,OrderByType.Desc).Select(st => st).ToPageList(1, 1000, ref count);

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
            var t1 = db.Ado.SqlQuery<string>("select 'a'  from dual");
            var t2 = db.Ado.GetInt("select 1  from dual");
            var t3 = db.Ado.GetDataTable("select 1 as id  from dual");
            db.Ado.CommitTran();
            //more
            //db.Ado.GetXXX...
        }
        public static void Easy()
        {
            var db = GetInstance();
            var getAll22 = db.Queryable<Student>().ToDataTable();
            var getAll220 = db.Ado.GetDataSetAll("select * from student");
            var getAll = db.Queryable<Student>().ToList();
            var getAllOrder = db.Queryable<Student>().OrderBy(it => it.Id).OrderBy(it => it.Name, OrderByType.Desc).ToList();
            var getId = db.Queryable<Student>().Select(it => it.Id).ToList();
            var getNew = db.Queryable<Student>().Where(it => it.Id == 1).Select(it => new { id = SqlFunc.IIF(it.Id == 0, 1, it.Id), it.Name, it.SchoolId }).ToList();
            var getAllNoLock = db.Queryable<Student>().With(SqlWith.NoLock).ToList();
            var getByPrimaryKey = db.Queryable<Student>().InSingle(2);
            var getSingleOrDefault = db.Queryable<Student>().Where(it=>it.Id==1).Single();
            var getFirstOrDefault = db.Queryable<Student>().First();
            var getByWhere = db.Queryable<Student>().Where(it => it.Id == 1 || it.Name == "a").ToList();
            var getByFuns = db.Queryable<Student>().Where(it => SqlFunc.IsNullOrEmpty(it.Name)).ToList();
            var sum = db.Queryable<Student>().Select(it => it.SchoolId).ToList();
            var sum2 = db.Queryable<Student,School>((st,sc)=>st.SchoolId==sc.Id).Sum((st,sc) => sc.Id);
            var isAny = db.Queryable<Student>().Where(it => it.Id == -1).Any();
            var isAny2 = db.Queryable<Student>().Any(it => it.Id == -1);
            var getListByRename = db.Queryable<School>().AS("Student").ToList();
            var in1 = db.Queryable<Student>().In(it => it.Id, new int[] { 1, 2, 3 }).ToList();
            var in2 = db.Queryable<Student>().In(new int[] { 1, 2, 3 }).ToList();
            int[] array = new int[] { 1, 2 };
            var in3 = db.Queryable<Student>().Where(it => SqlFunc.ContainsArray(array, it.Id)).ToList();
            var group = db.Queryable<Student>().GroupBy(it => it.Id)
                .Having(it => SqlFunc.AggregateCount(it.Id) > 10)
                .Select(it => new { id = SqlFunc.AggregateCount(it.Id) }).ToList();

            var between = db.Queryable<Student>().Where(it => SqlFunc.Between(it.Id, 1, 20)).ToList();

         //   var getTodayList = db.Queryable<Student>().Where(it => SqlFunc.DateIsSame(it.CreateTime, DateTime.Now)).ToList();

            var joinSql = db.Queryable("student", "s").OrderBy("id").Select("id,name").ToPageList(1, 2);
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



            //join 
            var list3 = db.Queryable<Student, School>((st, sc) => new object[] {
              JoinType.Left,st.SchoolId==sc.Id
            })
            .WhereIF(false, (st, sc) => sc.Id == 1)
            .WhereIF(false, (st, sc) => st.Id == 1).ToList();
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


            //join  2
            var list4_1 = db.Queryable<Student, School>((st, sc) => new object[] {
              JoinType.Left,st.SchoolId==sc.Id&& st.Name == "jack"
            }).ToList();


            //The simple use of Join 2 table
            var list5 = db.Queryable<Student, School>((st, sc) => st.SchoolId == sc.Id).Select((st,sc)=>new {st.Name,st.Id,schoolName=sc.Name}).ToList();

            //join 3 table
            var list6 = db.Queryable<Student, School,School>((st, sc,sc2) => st.SchoolId == sc.Id&&sc.Id==sc2.Id)
                .Select((st, sc,sc2) => new { st.Name, st.Id, schoolName = sc.Name,schoolName2=sc2.Name }).ToList();

            //join 3 table page
            var list7= db.Queryable<Student, School, School>((st, sc, sc2) => st.SchoolId == sc.Id && sc.Id == sc2.Id)
            .Select((st, sc, sc2) => new { st.Name, st.Id, schoolName = sc.Name, schoolName2 = sc2.Name }).ToPageList(1,2);

            //join 3 table page 
            var list8 = db.Queryable<Student, School, School>((st, sc, sc2) => st.SchoolId == sc.Id && sc.Id == sc2.Id)
            .OrderBy(st=>st.Id)
            .Select((st, sc, sc2) => new { st.Name, st.Id, schoolName = sc.Name, schoolName2 = sc2.Name }).ToPageList(1, 2);
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


            var s7 = db.Queryable<Student, School>((st, sc) => new object[] {
              JoinType.Left,st.SchoolId==sc.Id
            }).Select((st, sc) => sc).ToList();

            var s8 = db.Queryable<Student, School>((st, sc) => new object[] {
              JoinType.Left,st.SchoolId==sc.Id
            })
            .OrderBy((st, sc) => st.SchoolId)
            .Select((st, sc) => sc)
            .Take(1).ToList();
        }
        private static void Sqlable()
        {
            var db = GetInstance();
            var join3 = db.Queryable("Student", "st")
                          .AddJoinInfo("School", "sh", "sh.id=st.schoolid")
                          .Where("st.id>:id")
                          .AddParameters(new { id = 1 })
                          .Select("st.*").ToList();
            //SELECT st.* FROM [Student] st Left JOIN School sh ON sh.id=st.schoolid   WHERE st.id>@id 
        }
        private static void Enum()
        {
            var db = GetInstance();
            var list = db.Queryable<StudentEnum>().AS("Student").Where(it => it.SchoolId == SchoolEnum.HarvardUniversity).ToList();
        }
    }
}
