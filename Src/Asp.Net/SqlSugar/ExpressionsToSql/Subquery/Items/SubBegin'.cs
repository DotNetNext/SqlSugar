using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar
{
    public class SubBegin : ISubOperation
    {
        public string Name
        {
            get
            {
                return "Begin";
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
                return 100;
            }
        }

        public string GetValue(ExpressionContext context, Expression expression)
        {
            return "SELECT";
        }
    }
}
