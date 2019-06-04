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
                base.Check("SELECT  AVG(`st`.`ID`) AS `avgId`  FROM `STudent` st Inner JOIN `School` sc ON ( `st`.`ID` = `sc`.`Id` )  GROUP BY `st`.`ID`  HAVING (AVG(`st`.`ID`) = @Const0 ) ",
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

                base.Check("SELECT  `st`.`Id` AS `stid` , `st2`.`Id` AS `scId` , `st`.`Id` AS `School.Id` , `st`.`Name` AS `School.Name`  FROM `School` st Left JOIN `School` st2 ON ( `st`.`Id` = `st2`.`Id` )   WHERE ( `st`.`Id` > @Id0 ) "
                    , new List<SugarParameter>() {
                        new SugarParameter("@Id0",0)
                    }, t2.Key, t2.Value, "select t2  Error");


                var t3 = db.Queryable<Student, School, School>((st, sc, sc2) => new object[] {
                          JoinType.Left,st.SchoolId==sc.Id,
                          JoinType.Left,sc2.Id==sc.Id
                }).Where(st => st.Id > 0)
                .Select<School>((st) => new School() { Id = st.Id }).ToSql();
                base.Check("SELECT  `st`.`ID` AS `Id`  FROM `STudent` st Left JOIN `School` sc ON ( `st`.`SchoolId` = `sc`.`Id` )  Left JOIN `School` sc2 ON ( `sc2`.`Id` = `sc`.`Id` )   WHERE ( `st`.`ID` > @Id0 ) ",
                   new List<SugarParameter>() {
                        new SugarParameter("@Id0",0)
                    }, t3.Key, t3.Value, "select t3 Error");
                #endregion


            }
        }

        public SqlSugarClient GetInstance()
        {
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig() { ConnectionString = Config.ConnectionString, DbType = DbType.Sqlite });
            return db;
        }
    }
}
