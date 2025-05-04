using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar.MongoDbCore.ExpToSql.VisitorItems 
{
    public static class ValueExtractor
    {
        public static JToken GetValue(ConstantExpression expr)
        {
            return JToken.FromObject(expr.Value);
        }

        public static JToken GetValue(Expression expr)
        {
            var lambda = Expression.Lambda(expr);
            var compiled = lambda.Compile();
            return JToken.FromObject(compiled.DynamicInvoke());
        }
    }
}
