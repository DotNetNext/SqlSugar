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
    public class Field
    {
        internal static void Init()
        {
            FieldSingle();
            FieldMultiple();
        }
        private static void FieldSingle()
        {
            Expression<Func<Student, object>> exp = it => it.Name;
            ExpressionContext expContext = new ExpressionContext(exp, ResolveExpressType.FieldSingle);
            expContext.Resolve();
            var selectorValue = expContext.Result.GetString();
        }
        private static void FieldMultiple()
        {
            Expression<Func<Student, object>> exp = it => it.Name;
            ExpressionContext expContext = new ExpressionContext(exp, ResolveExpressType.FieldMultiple);
            expContext.Resolve();
            var selectorValue = expContext.Result.GetString();
        }
    }
}
