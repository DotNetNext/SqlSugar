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
                var d5 = db.Queryable<Student>().ToList();
                var d51 = db.Queryable<Student>().OrderBy(it=>it.Id).ToSql();
                var dr3 = db.Queryable<Student>().Select(it => new ViewModelStudent2 {    Name=it.Name,Student=it}).ToList();
                var dr0 = db.Queryable<Student>().Select(it => new  { id=it.Id,w=new { x=it } }).ToList();
                var dr1 = db.Queryable<Student>().Select(it => new { newid = it.Id }).ToList();
                var dr2 = db.Queryable<Student>().Select(it => new { newid = it.Id, obj = it }).ToList();
                var dr4 = db.Queryable<Student>().Select(it => new ViewModelStudent2 { Student = it,  Name =it.Name }).ToList();
                #endregion


                #region sql and parameters validate
                var l1 = db.Queryable<School, School>((st, st2) => new object[] {
                           JoinType.Left,st.Id==st2.Id
                    })
                          .Where(st => st.Id > 0)
                          .Select<School, School, dynamic>((st, st2) => new { stid = st.Id, scId = st2.Id, xx = st }).ToSql();

                base.Check("SELECT  [st].[Id] AS [stid] , [st2].[Id] AS [scId] , [st].[Id] AS [School.Id] , [st].[Name] AS [School.Name]  FROM [School] st  Left JOIN School st2  ON ( [st].[Id]  = [st2].[Id] )   WHERE ( [st].[Id]  > @Id0 )"
                    , new List<SugarParameter>() {
                        new SugarParameter("@Id0",0)
                    }, l1.Key, l1.Value, "l1错误");

                var list2 = db.Queryable<Student>()
                  .Where(st => st.Id > 0)
                 .Select("id").ToSql();

                base.Check("SELECT id FROM [Student]  WHERE ( [Id]  > @Id0 )",
                 new List<SugarParameter>() { new SugarParameter("@Id0", 0) },
                 list2.Key,
                 list2.Value,
                 "list2报错"
                );

                var list3 = db.Queryable<Student, School, School>((st, sc, sc2) => new object[] {
                          JoinType.Left,st.SchoolId==sc.Id,
                          JoinType.Left,sc2.Id==sc.Id
                }).Where(st => st.Id > 0)
                .Select<School>((st) => new School() { Id = st.Id }).ToList();

                var list4 = db.Queryable("Student", "st")
                 .AddJoinInfo("School", "sh", "sh.id=st.schoolid")
                 .Where("st.id>@id")
                 .AddParameters(new { id = 1 })
                 .Select("st.*").ToList();
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
