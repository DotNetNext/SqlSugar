using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar
{
    public class SubAny : ISubOperation
    {
        public Expression Expression
        {
            get;set;
        }

        public string Name
        {
            get
            {
                return "Any";
            }
        }

        public int Sort
        {
            get
            {
                return 1000;
            }
        }

        public string GetValue(ExpressionContext context, Expression expression)
        {
            return ">0";
        }
    }
}
