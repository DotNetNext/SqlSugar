using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public class SubAs : ISubOperation
    {
        public bool HasWhere
        {
            get; set;
        }

        public string Name
        {
            get
            {
                return "AS";
            }
        }

        public Expression Expression
        {
            get; set;
        }


        public int Sort
        {
            get
            {
                return 200;
            }
        }

        public ExpressionContext Context
        {
            get; set;
        }

        public string GetValue(Expression expression = null)
        {
            var exp = expression as MethodCallExpression;
            var expString=  SubTools.GetMethodValue(this.Context, exp.Arguments[0], ResolveExpressType.WhereSingle)?.Trim();
            var result =   this.Context.Parameters.First(it => it.ParameterName == expString).Value+"";
            return "$SubAs:"+result;
        }
    }
}
