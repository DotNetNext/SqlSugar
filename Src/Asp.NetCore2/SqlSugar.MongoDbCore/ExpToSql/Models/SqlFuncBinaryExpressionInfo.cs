using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar.MongoDb 
{
    public class SqlFuncBinaryExpressionInfo
    {
        public bool IsSqlFunc { get { return LeftIsFunc || RightIsFunc; } }
        public bool LeftIsFunc { get; set; }
        public bool RightIsFunc { get; set; }
        public string LeftMethodName { get; set; }
        public string RightMethodName { get; set; }
        public SqlFuncBinaryExpressionInfoType methodType { get; set; } = SqlFuncBinaryExpressionInfoType.Default;
        public Expression LeftExp { get;  set; }
        public ExpressionType NodeType { get;  set; }
        public Expression RightExp { get;  set; }
    }
    public enum SqlFuncBinaryExpressionInfoType 
    {
        Default=1
    }
}
