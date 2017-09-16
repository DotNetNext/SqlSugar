using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar
{
    public class SubNotAny : ISubOperation
    {
        public Expression Expression
        {
            get; set;
        }

        public string Name
        {
            get
            {
                return "NotAny";
            }
        }

        public int Sort
        {
            get
            {
                return 0;
            }
        }

        public string GetValue(ExpressionContext context, Expression expression)
        {
            return "NOT EXISTS";
        }
    }
}
