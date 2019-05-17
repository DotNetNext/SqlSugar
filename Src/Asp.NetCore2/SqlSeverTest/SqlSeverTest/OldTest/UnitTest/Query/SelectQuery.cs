using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
using System.Linq.Expressions;
using OrmTest.Models;
namespace OrmTest.UnitTest
{
    public class SelectQuery : UnitTestBase
    {
        private SelectQuery() { }
        public SelectQuery(int eachCount)
        {
            this.Count = eachCount;
        }
        internal void Init()
        {
            base.Begin();
            for (int i = 0; i < base.Count; i++)
            {
                Q2();
            }
            base.End("Method Test");
        }

        public void Q2()
        {
            using (var db = GetInstance())
            {
                //db.Database.IsEnableLogEvent = true;
                db.Aop.OnLogExecuting = (sql, pars) =>
                {
                    Console.WriteLine(sql + " " + pars);
                };


                #region dr ot entity
                db.IgnoreColumns.Add("TestId", "Student");
                var s1 = db.Queryable<Student>().Select(it => new ViewModelStudent2 { Name = it.Name, Student = it }).ToList();
                var s2 = db.Queryable<Student>().Select(it => new { id = it.Id, w = new { x = it } }).ToList();
                var s3 = db.Queryable<Student>().Select(it => new { newid = it.Id }).ToList();
                var s4 = db.Queryable<Student>().Select(it => new { newid = it.Id, obj = it }).ToList();
                var s5 = db.Queryable<Student>().Select(it => new ViewModelStudent2 { Student = it, Name = it.Name }).ToList();
                #endregion


                #region sql and parameters validate
                var t1 = db.Queryable<Student, School>((st, sc) => new object[] {
                    JoinType.Inner,st.Id==sc.Id
                }).GroupBy(st => st.Id).Having(st => SqlFunc.AggregateAvg(st.Id) == 1).Select(st => new { avgId = SqlFunc.AggregateAvg(st.Id) }).ToSql();
                base.Check("SELECT  AVG([st].[ID]) AS [avgId]  FROM [STudent] st Inner JOIN [School] sc ON ( [st].[ID] = [sc].[Id] )  GROUP BY [st].[ID]  HAVING (AVG([st].[ID]) = @Const0 ) ",
                    new List<SugarParameter>() {
                      new SugarParameter("@Const0",1)
                    }
                    ,
                    t1.Key, t1.Value, " select t1 Error");


                var t2 = db.Queryable<School, School>((st, st2) => new object[] {
                           JoinType.Left,st.Id==st2.Id
                    })
                          .Where(st => st.Id > 0)
                          .Select((st, st2) => new { stid = st.Id, scId = st2.Id, xx = st }).ToSql();

                base.Check("SELECT  [st].[Id] AS [stid] , [st2].[Id] AS [scId] , [st].[Id] AS [School.Id] , [st].[Name] AS [School.Name]  FROM [School] st Left JOIN [School] st2 ON ( [st].[Id] = [st2].[Id] )   WHERE ( [st].[Id] > @Id0 ) "
                    , new List<SugarParameter>() {
                        new SugarParameter("@Id0",0)
                    }, t2.Key, t2.Value, "select t2  Error");


                var t3 = db.Queryable<Student, School, School>((st, sc, sc2) => new object[] {
                          JoinType.Left,st.SchoolId==sc.Id,
                          JoinType.Left,sc2.Id==sc.Id
                }).Where(st => st.Id > 0)
                .Select<School>((st) => new School() { Id = st.Id }).ToSql();
                base.Check("SELECT  [st].[ID] AS [Id]  FROM [STudent] st Left JOIN [School] sc ON ( [st].[SchoolId] = [sc].[Id] )  Left JOIN [School] sc2 ON ( [sc2].[Id] = [sc].[Id] )   WHERE ( [st].[ID] > @Id0 ) ",
                   new List<SugarParameter>() {
                        new SugarParameter("@Id0",0)
                    }, t3.Key, t3.Value, "select t3 Error");

 
                db.Aop.OnLogExecuting = (sql, pars) =>
                {
                    base.Check(" SELECT COUNT(1) FROM (SELECT [st].[ID] FROM [STudent] st Left JOIN [School] sc ON ( [st].[SchoolId] = [sc].[Id] )  Left JOIN [School] sc2 ON ( [sc2].[Id] = [sc].[Id] )  GROUP BY [st].[ID] ) CountTable ",
                  null, sql, null, "select t4 Error");
                };

                var t4 = db.Queryable<Student, School, School>((st, sc, sc2) => new object[] {
                          JoinType.Left,st.SchoolId==sc.Id,
                          JoinType.Left,sc2.Id==sc.Id
                }).GroupBy(st => st.Id).Select(st => st.Id).Count();

                DateTime? result = DateTime.Now;
                var t5 = db.Queryable<Student>().Where(it => it.CreateTime > result.Value.Date).ToSql();
                base.Check("SELECT [ID],[SchoolId],[Name],[CreateTime] FROM [STudent]  WHERE ( [CreateTime] > @Const0 )",
           new List<SugarParameter>() {
                new SugarParameter("@Const0",result.Value.Date)
           }, t5.Key, t5.Value, "select t5 Error");
                db.Ado.IsEnableLogEvent = false;

                var t6 = db.Queryable<DataTestInfo2>().Where(it => SqlFunc.HasValue(it.Bool2) == false).ToSql();
                base.Check("SELECT [PK],[Bool1],[Bool2],[Text1] FROM [DataTestInfo2]  WHERE (( CASE  WHEN ( [Bool2]<>'' AND [Bool2] IS NOT NULL )  THEN 1 ELSE 0 END ) = @Const0 )",
   new List<SugarParameter>() {
                new SugarParameter("@Const0",false)
   }, t6.Key, t6.Value, "select t6 Error");


                var t7 = db.Queryable<Student>().Select(it=>new DataTestInfo2() {
                     Bool1=SqlFunc.IIF(SqlFunc.Subqueryable<Student>().Where(x=>x.Id
                     ==it.Id).Any(),true,false)
                }).ToSql();

                base.Check("SELECT  ( CASE  WHEN (EXISTS ( SELECT * FROM [STudent] WHERE ( [ID] = [it].[ID] ) )) THEN @MethodConst0  ELSE @MethodConst1 END ) AS [Bool1]  FROM [STudent] it ",
                    new List<SugarParameter>() {
                        new SugarParameter("@MethodConst0",true),
                         new SugarParameter("@MethodConst1",false)
                    }, t7.Key, t7.Value, "select t7 Error");
                #endregion

                try
                {

                    var t8 = db.Queryable<Student, School, School>((st, sc, sc2) => new object[] {
                          JoinType.Left,st.SchoolId==sc.Id,
                          JoinType.Left,sc2.Id==sc.Id
                }).Where(st => st.Id > 0)
                    .Select<School>((st1) => new School() { Id = st1.Id }).ToList();
                }
                catch (Exception ex)
                {
                    if (!ex.Message.Contains("English Message : Join st needs to be the same as Select st1")){
                        throw new Exception("selec t8 error");
                    }
                    Console.WriteLine(ex.Message);
                }

                try
                {

                    var t8 = db.Queryable<Student, School>((st, sc) =>st.Id==sc.Id).Where(st => st.Id > 0)
                    .Where(x=>x.Id==1)
                    .Select<School>((st) => new School() { Id = st.Id }).ToList();
                }
                catch (Exception ex)
                {

                    if (!ex.Message.Contains("English Message : Join st needs to be the same as Where x")){
                        throw new Exception("selec t8 error");
                    }
                    Console.WriteLine(ex.Message);
                }

                try
                {

                    var t8 = db.Queryable<Student, School>((st, sc) => st.Id == sc.Id)
                    .Sum(x =>x.Id );
                }
                catch (Exception ex)
                {

                    if (!ex.Message.Contains("English Message : Join st needs to be the same as Sum x")){
                        throw new Exception("selec t8 error");
                    }
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
