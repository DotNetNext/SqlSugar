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
    public class Select
    {
        internal static void Init()
        {

            single();
            singleDynamic();
        }

        private static void single()
        {
            int p = 1;
            Expression<Func<Student, object>> exp = it => new Student() { Name = "a",  Id=it.Id, SchoolId=p };
            ExpressionContext expContext = new ExpressionContext(exp, ResolveExpressType.WhereSingle);
            expContext.ResolveType = ResolveExpressType.SelectSingle;
            expContext.Resolve();
            var selectorValue = expContext.Result.GetString();
            var pars = expContext.Parameters;
        }

        private static void singleDynamic()
        {
            string a = "a";
            Expression<Func<Student, object>> exp = it =>new { x = it.Id, shoolid=1,name=a };
            ExpressionContext expContext = new ExpressionContext(exp, ResolveExpressType.WhereSingle);
            expContext.ResolveType = ResolveExpressType.SelectSingle;
            expContext.Resolve();
            var selectorValue = expContext.Result.GetString();
            var pars = expContext.Parameters;
        }
    }
}
