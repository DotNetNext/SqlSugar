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

            o1();
            o2();
        }

        private static void o1()
        {
            Expression<Func<Student, object>> exp = it => new Student() { Name = "a",  Id=it.Id };
            ExpressionContext expContext = new ExpressionContext(exp, ResolveExpressType.WhereSingle);
            expContext.ResolveType = ResolveExpressType.SelectSingle;
            expContext.Resolve();
            var x = expContext.Result.GetString();
            var pars = expContext.Parameters;
        }

        private static void o2()
        {
            Expression<Func<Student, object>> exp = it =>new { x = "a" };
            ExpressionContext expContext = new ExpressionContext(exp, ResolveExpressType.WhereSingle);
            expContext.ResolveType = ResolveExpressType.SelectSingle;
            expContext.Resolve();
            var x = expContext.Result.GetString();
            var pars = expContext.Parameters;
        }
    }
}
