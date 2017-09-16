using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar
{
    public class SubLeftBracket : ISubOperation
    {
        public Expression Expression
        {
            get;set;
        }

        public string Name
        {
            get
            {
                return "LeftBracket";
            }
        }

        public int Sort
        {
            get
            {
                return 50;
            }
        }

        public string GetValue(ExpressionContext context, Expression expression)
        {
            return "(";
        }
    }
}
