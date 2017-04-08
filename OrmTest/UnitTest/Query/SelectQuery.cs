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
                db.Database.IsEnableLogEvent = true;
                db.Database.LogEventStarting = (sql, pars) =>
                {
                    Console.WriteLine(sql + " " + pars);
                };

                var l1 = db.Queryable<School, School>((st, st2) => new object[] {
                           JoinType.Left,st.Id==st2.Id
                    })
                    .Where(st => st.Id > 0)
                    .Select<School, School, dynamic>((st, st2) => new {stid = st.Id, scId = st2.Id,xx=st }).ToSql();

                base.Check("SELECT  [st].[Id] AS [stid] , [st2].[Id] AS [scId] , [st].[Id] AS [xx_Id] , [st].[Name] AS [xx_Name]  FROM [School] st  Left JOIN School st2  ON ( [st].[Id]  = [st2].[Id] )   WHERE ( [st].[Id]  > @Id0 )"
                    , new List<SugarParameter>() {
                        new SugarParameter("@Id0",0)
                    },l1.Key,l1.Value, "l1错误");

                var l2 = db.Queryable<School, School>((st, st2) => new object[] {
                           JoinType.Left,st.Id==st2.Id
                    }).Where<Student, School>((st, st2) => st2.Id > 2)
                      .Select(st => new ViewModelStudent { School = st }).ToSql();

                base.Check("SELECT  [st].[Id] AS [School_Id] , [st].[Name] AS [School_Name]  FROM [School] st  Left JOIN School st2  ON ( [st].[Id]  = [st2].[Id] )   WHERE ( [st2].[Id]  > @Id0 )",
                     new List<SugarParameter>() { new SugarParameter("@Id0", 2) },
                     l2.Key,
                     l2.Value,
                     "l2报错"
                    );


                var list2 = db.Queryable<Student>()
                  .Where(st => st.Id > 0)
                 .Select("id").ToList();

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
            }
        }

        public SqlSugarClient GetInstance()
        {
            SqlSugarClient db = new SqlSugarClient(new SystemTablesConfig() { ConnectionString = Config.ConnectionString, DbType = DbType.SqlServer });
            return db;
        }
    }
}
