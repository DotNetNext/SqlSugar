using SqlSugar.ExpressionsToSql.Subquery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar
{
    public class SubTool
    {
        public static List<ISubOperation> SubItems = new List<ISubOperation>()
        {
           new SubSelect(),
           new SubWhere(),
           new SubAny(),
           new SubBegin(),
           new SubFromTable()
        };

        public static string GetMethodValue(ExpressionContext context,Expression item,ResolveExpressType type)
        {
            var newContext = context.GetCopyContext();
            newContext.MappingColumns = context.MappingColumns;
            newContext.MappingTables = context.MappingTables;
            newContext.IgnoreComumnList = context.IgnoreComumnList;
            newContext.Resolve(item,type);
            context.Index = newContext.Index;
            context.ParameterIndex = newContext.ParameterIndex;
            return newContext.Result.GetResultString();
        }
    }
}
