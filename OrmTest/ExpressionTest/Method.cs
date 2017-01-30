using OrmTest.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest.ExpressionTest
{
    public class Method : ExpTestBase
    {
        private Method() { }
        public Method(int eachCount)
        {
            this.Count = eachCount;
        }
        internal void Init()
        {
            base.Begin();
            for (int i = 0; i < base.Count; i++)
            {
                StringIsNullOrEmpty();
            }
            base.End("Method Test");
        }
        private void StringIsNullOrEmpty()
        {
            Expression<Func<Student, bool>> exp = it =>it.Id>2|| NBORM.IsNullOrEmpty(it.Id);;
            SqlServerExpressionContext expContext = new SqlServerExpressionContext(exp, ResolveExpressType.WhereSingle);
            expContext.Resolve();
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(value, pars, "(( Id  > @Id0 )  OR  ( Id='' OR Id IS NULL ))", new List<SugarParameter>() {
                new SugarParameter("@Id0",2)
            }, "whereSingle1");
        }
    }
}
