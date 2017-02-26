using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
using System.Linq.Expressions;
using OrmTest.Models;
namespace OrmTest.ExpressionTest
{
    public class Join : ExpTestBase
    {
        private Join() { }
        public Join(int eachCount)
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
            ExpressionContext contet = new ExpressionContext();
            SqlSugarClient db = new SqlSugarClient(new SystemTablesConfig() { ConnectionString="x" ,DbType= DbType.SqlServer });
            db.Queryable<Student, School>((st,sc)=> new object[] {
                JoinType.Left,st.SchoolId==sc.Id
            });

        }
    }
}
