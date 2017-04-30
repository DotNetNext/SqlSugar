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
    public class SelectQuery : ExpTestBase
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
                db.Database.LogEventStarting = (sql, pars) =>
                {
                    Console.WriteLine(sql + " " + pars);
                };


                #region dr ot entity
                db.IgnoreComumns.Add("TestId", "Student");
                var s1 = db.Queryable<Student>().Select(it => new ViewModelStudent2 {    Name=it.Name,Student=it}).ToList();
                var s2 = db.Queryable<Student>().Select(it => new  { id=it.Id,w=new { x=it } }).ToList();
                var s3 = db.Queryable<Student>().Select(it => new { newid = it.Id }).ToList();
                var s4 = db.Queryable<Student>().Select(it => new { newid = it.Id, obj = it }).ToList();
                var s5 = db.Queryable<Student>().Select(it => new ViewModelStudent2 { Student = it,  Name =it.Name }).ToList();
                #endregion


                #region sql and parameters validate
                var ss0 = db.Queryable<Student, School>((st,sc)=>new object[] {
                    JoinType.Inner,st.Id==sc.Id
                }).GroupBy(st => st.Id).Select(st => new { avgId=NBORM.AggregateAvg(st.Id) }).ToSql();
                base.Check(" SELECT  AVG([st].[Id]) AS [avgId]  FROM [Student] st Inner JOIN School sc ON ( [st].[Id] = [sc].[Id] )  GROUP BY [st].[Id] ", null,
                    ss0.Key, null," ss0 Error");


                var ss1 = db.Queryable<School, School>((st, st2) => new object[] {
                           JoinType.Left,st.Id==st2.Id
                    })
                          .Where(st => st.Id > 0)
                          .Select<School, School, dynamic>((st, st2) => new { stid = st.Id, scId = st2.Id, xx = st }).ToSql();

                base.Check("SELECT  [st].[Id] AS [stid] , [st2].[Id] AS [scId] , [st].[Id] AS [School.Id] , [st].[Name] AS [School.Name]  FROM [School] st Left JOIN School st2 ON ( [st].[Id] = [st2].[Id] )   WHERE ( [st].[Id] > @Id0 ) "
                    , new List<SugarParameter>() {
                        new SugarParameter("@Id0",0)
                    }, ss1.Key, ss1.Value, "ss1 Error");


                var ss2 = db.Queryable<Student, School, School>((st, sc, sc2) => new object[] {
                          JoinType.Left,st.SchoolId==sc.Id,
                          JoinType.Left,sc2.Id==sc.Id
                }).Where(st => st.Id > 0)
                .Select<School>((st) => new School() { Id = st.Id }).ToSql();
                base.Check("SELECT  [st].[Id] AS [Id]  FROM [Student] st Left JOIN School sc ON ( [st].[SchoolId] = [sc].[Id] )  Left JOIN School sc2 ON ( [sc2].[Id] = [sc].[Id] )   WHERE ( [st].[Id] > @Id0 ) ",
                   new List<SugarParameter>() {
                        new SugarParameter("@Id0",0)
                    }, ss2.Key, ss2.Value, "ss2 Error");
                #endregion


            }
        }

        public SqlSugarClient GetInstance()
        {
            SqlSugarClient db = new SqlSugarClient(new SystemTablesConfig() { ConnectionString = Config.ConnectionString, DbType = DbType.SqlServer });
            return db;
        }
    }
}
