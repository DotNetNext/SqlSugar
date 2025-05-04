using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
using Newtonsoft.Json.Linq;
using SqlSugar.MongoDbCore.ExpToSql.Context;

namespace SqlSugar.MongoDbCore 
{

    public static class MongoNestedTranslator
    {
        public static JObject Translate(Expression expr, MongoNestedTranslatorContext context)
        {
            return (JObject)new ExpressionVisitor(context).Visit(expr);
        } 
    } 
}
