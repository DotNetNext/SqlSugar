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
    public class JoinQuery : ExpTestBase
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
            }
            base.End("Method Test");
        }

        public void Q1()
        {
            using (var db = GetInstance())
            {
                var list = db.Queryable<Student, School>((st, sc) => new object[] {
                          JoinType.Left,st.SchoolId==sc.Id
                }).Where(st => st.Id > 0).Select<Student>("*").ToList();
            }
        }
        public void Q2()
        {
            using (var db = GetInstance())
            {
                var list = db.Queryable<Student, School>((st, sc) => new object[] {
                          JoinType.Left,st.SchoolId==sc.Id
                }).Where(st=>st.Id>0).Select<Student>("*").ToList();
            }
        }


        public SqlSugarClient GetInstance()
        {
            SqlSugarClient db = new SqlSugarClient(new SystemTablesConfig() { ConnectionString = Config.ConnectionString, DbType = DbType.SqlServer });
            db.Database.IsEnableLogEvent = true;
            db.Database.LogEventStarting = (sql, pars) =>
            {
                Console.WriteLine(sql+" "+pars);
            };
            return db;
        }
    }
}
