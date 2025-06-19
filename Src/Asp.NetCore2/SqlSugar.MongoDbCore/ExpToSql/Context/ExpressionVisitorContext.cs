using System;
using System.Collections.Generic;
using System.Text;

namespace SqlSugar.MongoDb  
{
    public class ExpressionVisitorContext
    {
        public Type ExpType { get;  set; }
        public bool IsText { get; internal set; }
    }
}
