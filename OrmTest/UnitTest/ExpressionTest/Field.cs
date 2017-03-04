using OrmTest.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest.UnitTest
{
    public class Field:ExpTestBase
    {
        private Field() { }
        public Field(int eachCount)
        {
            this.Count = eachCount;
        }
        internal void Init()
        {
            base.Begin();
            for (int i = 0; i < base.Count; i++)
            {
                FieldSingle();
                FieldMultiple();
            }
            base.End("Filed Test");
        }
        private void FieldSingle()
        {
            Expression<Func<Student, object>> exp = it => it.Name;
            ExpressionContext expContext = new ExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.FieldSingle);
            var selectorValue = expContext.Result.GetString();
            Check(selectorValue, null, "Name", null, "FieldSingle");
        }
        private void FieldMultiple()
        {
            Expression<Func<Student, object>> exp = it => it.Name;
            ExpressionContext expContext = new ExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.FieldMultiple);
            var selectorValue = expContext.Result.GetString();
            Check(selectorValue, null, "it.Name", null, "FieldMultiple");
        }
    }
}
