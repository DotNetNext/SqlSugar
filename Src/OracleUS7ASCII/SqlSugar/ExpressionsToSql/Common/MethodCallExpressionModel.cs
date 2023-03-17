using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
namespace SqlSugar
{
    public class MethodCallExpressionModel
    {
        public List<MethodCallExpressionArgs> Args { get; set; }
        public string Name { get; set; }
        public dynamic Data { get; set; }
        public Expression Expression { get; set; }
        public Expression BaseExpression { get; set; }
    }

    public class MethodCallExpressionArgs
    {
        public bool IsMember { get; set; }
        public object MemberName { get; set; }
        public object MemberValue { get; set; }
        public Type Type { get; set; }
    }
}
