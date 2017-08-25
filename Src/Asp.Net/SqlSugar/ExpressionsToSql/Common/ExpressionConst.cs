using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
namespace SqlSugar
{
    internal class ExpressionConst
    {
        public const string BinaryFormatString = " ( {0} {1} {2} ) ";
        public const string Format0 = "{0}";
        public const string Format1 = "$__$";
        public const string Format2 = "o__o";
        public const string Format3 = "(";
        public const string Format4 = ")";
        public const string SqlFuncFullName = "SqlSugar.SqlFunc"; 
        public const string MethodConst = "MethodConst";
        public const string Const = "Const";
        public readonly static Type MemberExpressionType = typeof(MemberExpression);
        public readonly static Type ConstantExpressionType = typeof(ConstantExpression);
        public readonly static Type StringType = typeof(string);
    }
}
