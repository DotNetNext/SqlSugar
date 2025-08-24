using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar.MongoDb 
{
    public class AnyArgModel
    {
        public bool IsBinary { get; internal set; }
        public LambdaExpression LamdaExpression { get; internal set; }
        public Expression Left { get; internal set; }
        public Expression Right { get; internal set; }
        public ExpressionType NodeType { get; internal set; }
        public int LeftCount { get; internal set; }
        public int RightCount { get; internal set; }
        public bool IsTwoMemeber { get; internal set; }
    }
}
