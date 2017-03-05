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
                //var list = db.Queryable<Student>()
                //    .Where(st => st.Id > 0)
                //    .Select(it => new ViewModelStudent { Name = it.Name }).ToList();
                //var list2 = db.Queryable<Student>()
                //  .Where(st => st.Id > 0)
                 // .Select("id").ToList();
                var list3 = db.Queryable<Student>()
                 .Where(st => st.Id > 0)
                 .Select(it => new ViewModelStudent2 { Student = it }).ToList();
            }
        }

        public SqlSugarClient GetInstance()
        {
            SqlSugarClient db = new SqlSugarClient(new SystemTablesConfig() { ConnectionString = Config.ConnectionString, DbType = DbType.SqlServer });
            return db;
        }
    }
}
