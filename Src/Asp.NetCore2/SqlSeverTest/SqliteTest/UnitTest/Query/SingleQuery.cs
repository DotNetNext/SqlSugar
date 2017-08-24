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
    public class SingleQuery : UnitTestBase
    {
        private SingleQuery() { }
        public SingleQuery(int eachCount)
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
                var t1 = db.Queryable<Student>().ToSql();
                base.Check("SELECT `ID`,`SchoolId`,`Name`,`CreateTime` FROM `STudent`", null, t1.Key, null, "single t1 Error");

                var t2 = db.Queryable<Student>().With(SqlWith.NoLock).ToSql();
                base.Check("SELECT `ID`,`SchoolId`,`Name`,`CreateTime` FROM `STudent` ", null, t2.Key, null, "single t2 Error");

                var t3 = db.Queryable<Student>().OrderBy(it=>it.Id).ToSql();
                base.Check("SELECT `ID`,`SchoolId`,`Name`,`CreateTime` FROM `STudent` ORDER BY `ID` ASC", null, t3.Key, null, "single t3 Error");

                var t4 = db.Queryable<Student>().OrderBy(it => it.Id).Take(3).ToSql();
                base.Check(@"SELECT `ID`,`SchoolId`,`Name`,`CreateTime` FROM `STudent`    ORDER BY `ID` ASC LIMIT 0,3", null, t4.Key, null, "single t4 Error");

                var t5 = db.Queryable<Student>().OrderBy(it => it.Id).Skip(3).ToSql();
                base.Check(@"SELECT `ID`,`SchoolId`,`Name`,`CreateTime` FROM `STudent`     LIMIT 3,9223372036854775807", null, t5.Key,null, "single t5 Error");

                int pageIndex = 2;
                int pageSize = 10;
                var t6 = db.Queryable<Student>().OrderBy(it => it.Id,OrderByType.Desc).Skip((pageIndex-1)*pageSize).Take(pageSize).ToSql();
                base.Check(@"SELECT `ID`,`SchoolId`,`Name`,`CreateTime` FROM `STudent`    ORDER BY `ID` DESC LIMIT 10,10", null, t6.Key, null, "single t6 Error");


                int studentCount=db.Ado.GetInt("select count(1) from Student");
                var countIsSuccess=db.Queryable<Student>().Count()== studentCount;
                if (!countIsSuccess) {
                    throw new Exception(" single countIsSuccess Error");
                }

                var t7 = db.Queryable<Student>().OrderBy(it => it.Id, OrderByType.Desc).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToPageList(pageIndex,pageSize,ref studentCount);
                countIsSuccess = studentCount == db.Queryable<Student>().OrderBy(it => it.Id, OrderByType.Desc).Skip((pageIndex - 1) * pageSize).Take(pageSize * pageIndex).Count();
                if (!countIsSuccess)
                {
                    throw new Exception("single t7 Error");
                }

                int studentMin = db.Ado.GetInt("select min(id)  from Student");
                var minIsSuccess = db.Queryable<Student>().Min(it=>it.Id) == studentMin;
                if (!minIsSuccess)
                {
                    throw new Exception("single minIsSuccess Error");
                }

                int studentMax = db.Ado.GetInt("select max(id)  from Student");
                var maxIsSuccess = db.Queryable<Student>().Max(it => it.Id) == studentMax;
                if (!maxIsSuccess)
                {
                    throw new Exception("single maxIsSuccess Error");
                }

                int studentAvg = db.Ado.GetInt("select avg(id)  from Student");
                var avgIsSuccess = db.Queryable<Student>().Avg(it => it.Id) == studentAvg;
                if (!maxIsSuccess)
                {
                    throw new Exception(" single avgIsSuccess Error");
                }

                int studentSum = db.Ado.GetInt("select sum(id)  from Student");
                var sumIsSuccess = db.Queryable<Student>().Sum(it => it.Id) == studentSum;
                if (!sumIsSuccess)
                {
                    throw new Exception("single sumIsSuccess Error");
                }

                var t8 = db.Queryable<Student>()
                    .Where(it=>it.Id==1)
                    .WhereIF(true,it=> SqlFunc.Contains(it.Name,"a"))
                    .OrderBy(it => it.Id, OrderByType.Desc).Skip((pageIndex - 1) * pageSize).Take(pageSize ).With(SqlWith.NoLock).ToSql();
                base.Check(@"SELECT `ID`,`SchoolId`,`Name`,`CreateTime` FROM `STudent`   WHERE ( `ID` = @Id0 )  AND  (`Name` like '%'||@MethodConst1||'%')   ORDER BY `ID` DESC LIMIT 10,10", new List<SugarParameter>() {
                               new SugarParameter("@Id0",1),new SugarParameter("@MethodConst1","a")
               }, t8.Key, t8.Value,"single t8 Error");



                var t9 = db.Queryable<Student>()
                    .In(1)
                    .Select(it => new { it.Id, it.Name,x=it.Id }).ToSql();
                base.Check("SELECT  `ID` AS `Id` , `Name` AS `Name` , `ID` AS `x`  FROM `STudent`  WHERE `Id` IN (@InPara0)  ", new List<SugarParameter>() {
                     new SugarParameter("@InPara0",1)   },t9.Key,t9.Value, "single t9 error");
            }
        }


        public SqlSugarClient GetInstance()
        {
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig() { ConnectionString = Config.ConnectionString, DbType = DbType.Sqlite });
             return db;
        }
    }
}
