using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
using Newtonsoft.Json.Linq;

namespace SqlSugar.MongoDbCore 
{

    public static class MongoNestedTranslator
    {
        public static JObject Translate(Expression expr)
        {
            return (JObject)ExpressionVisitor.Visit(expr);
        } 
    } 
}
