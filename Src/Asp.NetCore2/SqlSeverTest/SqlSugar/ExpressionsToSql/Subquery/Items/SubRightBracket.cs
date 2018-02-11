using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar
{
    public class SubRightBracket : ISubOperation
    {
        public bool HasWhere
        {
            get; set;
        }

        public ExpressionContext Context
        {
            get;set;
        }

        public Expression Expression
        {
            get;set;
        }

        public string Name
        {
            get
            {
                return "RightBracket";
            }
        }

        public int Sort
        {
            get
            {
                return 500;
            }
        }

        public string GetValue(Expression expression)
        {
            return ")";
        }
    }
}
