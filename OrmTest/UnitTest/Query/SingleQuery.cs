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
                var s1 = db.Queryable<Student>().ToSql();
                base.Check("SELECT [ID],[SchoolId],[Name],[CreateTime] FROM [STudent]", null, s1.Key, null, "single s1 Error");

                var s2 = db.Queryable<Student>().With(SqlWith.NoLock).ToSql();
                base.Check("SELECT [ID],[SchoolId],[Name],[CreateTime] FROM [STudent] WITH(NOLOCK)", null, s2.Key, null, "single s2 Error");

                var s3 = db.Queryable<Student>().OrderBy(it=>it.Id).ToSql();
                base.Check("SELECT [ID],[SchoolId],[Name],[CreateTime] FROM [STudent] ORDER BY [ID] ASC", null, s3.Key, null, "single s3 Error");

                var s4 = db.Queryable<Student>().OrderBy(it => it.Id).Take(3).ToSql();
                base.Check(@"WITH PageTable AS(
                          SELECT [ID],[SchoolId],[Name],[CreateTime] FROM [STudent]  
                  )
                  SELECT * FROM (SELECT *,ROW_NUMBER() OVER(ORDER BY [ID] ASC) AS RowIndex FROM PageTable ) T WHERE RowIndex BETWEEN 1 AND 3", null, s4.Key, null, "single s4 Error");

                var s5 = db.Queryable<Student>().OrderBy(it => it.Id).Skip(3).ToSql();
                base.Check(@"WITH PageTable AS(
                          SELECT [ID],[SchoolId],[Name],[CreateTime] FROM [STudent]  
                  )
                  SELECT * FROM (SELECT *,ROW_NUMBER() OVER(ORDER BY [ID] ASC) AS RowIndex FROM PageTable ) T WHERE RowIndex BETWEEN 4 AND 9223372036854775807", null, s5.Key,null, "single s5 Error");

                int pageIndex = 2;
                int pageSize = 10;
                var s6 = db.Queryable<Student>().OrderBy(it => it.Id,OrderByType.Desc).Skip((pageIndex-1)*pageSize).Take(pageSize*pageIndex).ToSql();
                base.Check(@"WITH PageTable AS(
                          SELECT [ID],[SchoolId],[Name],[CreateTime] FROM [STudent]  
                  )
                  SELECT * FROM (SELECT *,ROW_NUMBER() OVER(ORDER BY [ID] DESC) AS RowIndex FROM PageTable ) T WHERE RowIndex BETWEEN 11 AND 20", null, s6.Key, null, "single s6 Error");


                int studentCount=db.Ado.GetInt("select count(1) from Student");
                var countIsSuccess=db.Queryable<Student>().Count()== studentCount;
                if (!countIsSuccess) {
                    throw new Exception(" single countIsSuccess Error");
                }

                var s7 = db.Queryable<Student>().OrderBy(it => it.Id, OrderByType.Desc).Skip((pageIndex - 1) * pageSize).Take(pageSize * pageIndex).ToPageList(pageIndex,pageSize,ref studentCount);
                countIsSuccess = studentCount == db.Queryable<Student>().OrderBy(it => it.Id, OrderByType.Desc).Skip((pageIndex - 1) * pageSize).Take(pageSize * pageIndex).Count();
                if (!countIsSuccess)
                {
                    throw new Exception("single s7 Error");
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

                var s8 = db.Queryable<Student>()
                    .Where(it=>it.Id==1)
                    .WhereIF(true,it=> NBORM.Contains(it.Name,"a"))
                    .OrderBy(it => it.Id, OrderByType.Desc).Skip((pageIndex - 1) * pageSize).Take(pageSize * pageIndex).With(SqlWith.NoLock).ToSql();
                base.Check(@"WITH PageTable AS(
                          SELECT [ID],[SchoolId],[Name],[CreateTime] FROM [STudent] WITH(NOLOCK)   WHERE ( [ID] = @Id0 )  AND  ([Name] like '%'+@MethodConst1+'%')  
                  )
                  SELECT * FROM (SELECT *,ROW_NUMBER() OVER(ORDER BY [ID] DESC) AS RowIndex FROM PageTable ) T WHERE RowIndex BETWEEN 11 AND 20", new List<SugarParameter>() {
                               new SugarParameter("@Id0",1),new SugarParameter("@MethodConst1","a")
               }, s8.Key, s8.Value,"single s8 Error");
            }
        }


        public SqlSugarClient GetInstance()
        {
            SqlSugarClient db = new SqlSugarClient(new SystemTableConfig() { ConnectionString = Config.ConnectionString, DbType = DbType.SqlServer });
             return db;
        }
    }
}
