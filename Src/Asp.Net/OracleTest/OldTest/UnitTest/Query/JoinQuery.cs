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
    public class JoinQuery : UnitTestBase
    {
        private JoinQuery() { }
        public JoinQuery(int eachCount)
        {
            this.Count = eachCount;
        }
        internal void Init()
        {
            base.Begin();
            for (int i = 0; i < base.Count; i++)
            {
                Q1();
                Q2();
                Q3();
                Q4();
                q5();
                q6();
                q7();
            }
            base.End("Method Test");
        }

        private void q6()
        {
            using (var db = GetInstance())
            {
                var join6 = db.Queryable<Student, School>((st, sc) => new object[] {
                JoinType.Left,st.SchoolId==sc.Id
                 }).Select((st, sc) => new ViewModelStudent { Name = st.Name, SchoolId = SqlFunc.AggregateMin(sc.Id) }).ToSql();

                string sql = @"SELECT  [st].[Name] AS [Name] , MIN([sc].[Id]) AS [SchoolId]  FROM [STudent] st Left JOIN School sc ON ( [st].[SchoolId] = [sc].[Id] )  ";
                base.Check(sql, null, join6.Key, null, "join 6 Error");
            }
        }

        private void q7()
        {
            using (var db = GetInstance())
            {
                var join7 = db.Queryable<Student, School>((st, sc) => new object[] {
                JoinType.Left,st.SchoolId==sc.Id
                 }).Select((st, sc) => new ViewModelStudent { Name = st.Name, SchoolId = SqlFunc.AggregateMin(sc.Id*1) }).ToSql();

                string sql = @"SELECT  [st].[Name] AS [Name] , MIN(( [sc].[Id] * @Id0 )) AS [SchoolId]  FROM [STudent] st Left JOIN School sc ON ( [st].[SchoolId] = [sc].[Id] )   ";
                base.Check(sql, new List<SugarParameter>() {
                    new SugarParameter("@Id0",1)
                }, join7.Key, join7.Value, "join 7 Error");
            }
        }

        private void q5()
        {
            using (var db = GetInstance())
            {
                db.MappingTables.Add("School", "SchoolTable");
                var join5= db.Queryable<Student, School>((st, sc) => st.SchoolId == sc.Id).Select(st => st)
                    .GroupBy(st=> new{ st.Id,st.Name })
                    .ToSql();
                string sql = @"SELECT st.* FROM [STudent] st  ,[SchoolTable]  sc  WHERE ( [st].[SchoolId] = [sc].[Id] )GROUP BY [st].[ID],[st].[Name] ";
                base.Check(sql, null, join5.Key, null, "join 5 Error");
            }
        }

        private void Q4()
        {
            using (var db = GetInstance())
            {
                db.MappingTables.Add("School", "SchoolTable");
                var join4 = db.Queryable<Student, School>((st, sc) => st.SchoolId == sc.Id).Select(st=>st).ToSql();
                string sql = @"SELECT st.* FROM [STudent] st  ,[SchoolTable]  sc  WHERE ( [st].[SchoolId] = [sc].[Id] )  ";
                base.Check(sql, null, join4.Key, null, "join 4 Error");
            }
        }


        private void Q3()
        {
            using (var db = GetInstance())
            {
                var join3 = db.Queryable("Student", "st")
                            .AddJoinInfo("School", "sh", "sh.id=st.schoolid")
                            .Where("st.id>@id")
                            .AddParameters(new { id = 1 })
                            .Select("st.*").ToSql();
                string sql = @"SELECT st.* FROM [Student] st Left JOIN School sh ON sh.id=st.schoolid   WHERE st.id>@id ";
                base.Check(sql,new List<SugarParameter>() {new SugarParameter("@id",1)}, join3.Key, join3.Value, "join 3 Error");
            }
        }

        public void Q1()
        {
            using (var db = GetInstance())
            {
                var join1 = db.Queryable<Student, School>((st, sc) => new object[] {
                          JoinType.Left,st.SchoolId==sc.Id
                }).Where(st => st.Id > 0).Select<Student>("*").ToSql();
                base.Check(@"SELECT * FROM [STudent] st Left JOIN School sc ON ( [st].[SchoolId] = [sc].[Id] )   WHERE ( [st].[ID] > @Id0 ) ",
                    new List<SugarParameter>() {
                        new SugarParameter("@Id0",0)
                    }, join1.Key, join1.Value, "join 1 Error");
            }
        }
        public void Q2()
        {
            using (var db = GetInstance())
            {
                var join2 = db.Queryable<Student, School>((st, sc) => new object[] {
                          JoinType.Left,st.SchoolId==sc.Id
                }).Where(st => st.Id > 2).Select<Student>("*").ToSql();
                base.Check(@"SELECT * FROM [STudent] st Left JOIN School sc ON ( [st].[SchoolId] = [sc].[Id] )   WHERE ( [st].[ID] > @Id0 ) ",
    new List<SugarParameter>() {
                        new SugarParameter("@Id0",2)
    }, join2.Key, join2.Value, "join 2 Error");
            }
        }


        public new SqlSugarClient GetInstance()
        {
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig() { ConnectionString = Config.ConnectionString, DbType = DbType.Oracle });
            db.Aop.OnLogExecuting = (sql, pars) =>
            {
                Console.WriteLine(sql + " " + pars);
            };
            return db;
        }
    }
}
