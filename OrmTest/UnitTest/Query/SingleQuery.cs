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
    public class SingleQuery : ExpTestBase
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
                base.Check("SELECT [Id],[SchoolId],[Name],[CreateTime] FROM [Student]", null, s1.Key, null, "s1 Error");

                var s2 = db.Queryable<Student>().With(SqlWith.NoLock).ToSql();
                base.Check("SELECT [Id],[SchoolId],[Name],[CreateTime] FROM [Student] WITH(NOLOCK)", null, s2.Key, null, "s2 Error");

                var s3 = db.Queryable<Student>().OrderBy(it=>it.Id).ToSql();
                base.Check("SELECT [Id],[SchoolId],[Name],[CreateTime] FROM [Student] ORDER BY [Id] ASC", null, s3.Key, null, "s3 Error");

                var s4 = db.Queryable<Student>().OrderBy(it => it.Id).Take(3).ToSql();
                base.Check(@"WITH PageTable AS(
                          SELECT [Id],[SchoolId],[Name],[CreateTime] FROM [Student]  
                  )
                  SELECT * FROM (SELECT *,ROW_NUMBER() OVER(ORDER BY [Id] ASC) AS RowIndex FROM PageTable ) T WHERE RowIndex BETWEEN 1 AND 3", null, s4.Key, null, "s4 Error");

                var s5 = db.Queryable<Student>().OrderBy(it => it.Id).Skip(3).ToSql();
                base.Check(@"WITH PageTable AS(
                          SELECT [Id],[SchoolId],[Name],[CreateTime] FROM [Student]  
                  )
                  SELECT * FROM (SELECT *,ROW_NUMBER() OVER(ORDER BY [Id] ASC) AS RowIndex FROM PageTable ) T WHERE RowIndex BETWEEN 4 AND 9223372036854775807", null, s5.Key,null, "s5 Error");

                int pageIndex = 2;
                int pageSize = 10;
                var s6 = db.Queryable<Student>().OrderBy(it => it.Id,OrderByType.Desc).Skip((pageIndex-1)*pageSize).Take(pageSize*pageIndex).ToSql();
                base.Check(@"WITH PageTable AS(
                          SELECT [Id],[SchoolId],[Name],[CreateTime] FROM [Student]  
                  )
                  SELECT * FROM (SELECT *,ROW_NUMBER() OVER(ORDER BY [Id] DESC) AS RowIndex FROM PageTable ) T WHERE RowIndex BETWEEN 11 AND 20", null, s6.Key, null, "s6 Error");


                int studentCount=db.Database.GetInt("select count(1) from Student");
                var countIsSuccess=db.Queryable<Student>().Count()== studentCount;
                if (!countIsSuccess) {
                    throw new Exception("countIsSuccess Error");
                }

                var s7 = db.Queryable<Student>().OrderBy(it => it.Id, OrderByType.Desc).Skip((pageIndex - 1) * pageSize).Take(pageSize * pageIndex).ToPageList(pageIndex,pageSize,ref studentCount);
                countIsSuccess = studentCount == db.Queryable<Student>().OrderBy(it => it.Id, OrderByType.Desc).Skip((pageIndex - 1) * pageSize).Take(pageSize * pageIndex).Count();
                if (!countIsSuccess)
                {
                    throw new Exception("s7 Error");
                }

                int studentMin = db.Database.GetInt("select min(id)  from Student");
                var minIsSuccess = db.Queryable<Student>().Min(it=>it.Id) == studentMin;
                if (!minIsSuccess)
                {
                    throw new Exception("minIsSuccess Error");
                }

                int studentMax = db.Database.GetInt("select max(id)  from Student");
                var maxIsSuccess = db.Queryable<Student>().Max(it => it.Id) == studentMax;
                if (!maxIsSuccess)
                {
                    throw new Exception("maxIsSuccess Error");
                }

                int studentAvg = db.Database.GetInt("select avg(id)  from Student");
                var avgIsSuccess = db.Queryable<Student>().Avg(it => it.Id) == studentAvg;
                if (!maxIsSuccess)
                {
                    throw new Exception("avgIsSuccess Error");
                }

                int studentSum = db.Database.GetInt("select sum(id)  from Student");
                var sumIsSuccess = db.Queryable<Student>().Sum(it => it.Id) == studentSum;
                if (!sumIsSuccess)
                {
                    throw new Exception("sumIsSuccess Error");
                }

                var s8 = db.Queryable<Student>()
                    .Where(it=>it.Id==1)
                    .WhereIF(true,it=> NBORM.Contains(it.Name,"a"))
                    .OrderBy(it => it.Id, OrderByType.Desc).Skip((pageIndex - 1) * pageSize).Take(pageSize * pageIndex).With(SqlWith.NoLock).ToSql();
                base.Check(@"WITH PageTable AS(
                          SELECT [Id],[SchoolId],[Name],[CreateTime] FROM [Student] WITH(NOLOCK)   WHERE ( [Id]  = @Id0 )  AND  ([Name] like '%'+@MethodConst1+'%')  
                  )
                  SELECT * FROM (SELECT *,ROW_NUMBER() OVER(ORDER BY [Id] DESC) AS RowIndex FROM PageTable ) T WHERE RowIndex BETWEEN 11 AND 20", new List<SugarParameter>() {
                               new SugarParameter("@Id0",1),new SugarParameter("@MethodConst1","a")
               }, s8.Key, s8.Value,"s8 Error");
            }
        }


        public SqlSugarClient GetInstance()
        {
            SqlSugarClient db = new SqlSugarClient(new SystemTablesConfig() { ConnectionString = Config.ConnectionString, DbType = DbType.SqlServer });
            return db;
        }
    }
}
