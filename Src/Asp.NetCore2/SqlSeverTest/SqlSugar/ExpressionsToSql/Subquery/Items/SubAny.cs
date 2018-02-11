using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar
{
    public class SubAny : ISubOperation
    {
        public ExpressionContext Context
        {
            get;set;
        }

        public Expression Expression
        {
            get;set;
        }

        public bool HasWhere
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
                return 0;
            }
        }

        public string GetValue(Expression expression)
        {
            return "EXISTS";
        }
    }
}
