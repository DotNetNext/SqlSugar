using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar
{
    public class SubTools
    {
        public static List<ISubOperation> SubItems(ExpressionContext Context)
        {

            return new List<ISubOperation>()
                                                {
                                                    new SubSelect() { Context=Context },
                                                    new SubWhere(){ Context=Context },
                                                    new SubWhereIF(){ Context=Context },
                                                    new SubLeftJoin(){ Context=Context },
                                                    new SubInnerJoin(){ Context=Context },
                                                    new SubAnd(){ Context=Context },
                                                    new SubAndIF(){ Context=Context },
                                                    new SubAny(){ Context=Context },
                                                    new SubNotAny(){ Context=Context },
                                                    new SubBegin(){ Context=Context },
                                                    new SubFromTable(){ Context=Context },
                                                    new SubCount(){ Context=Context },
                                                    new SubMax(){ Context=Context },
                                                    new SubMin(){ Context=Context },
                                                    new SubSum(){ Context=Context },
                                                    new SubAvg(){ Context=Context },
                                                    new SubOrderBy(){ Context=Context },
                                                    new SubOrderByDesc(){ Context=Context },
                                                    new SubGroupBy(){ Context=Context}
                                                };
        }

        public static string GetSubReplace(ExpressionContext context)
        {
            if (context.SubQueryIndex == 0)
                return string.Empty;
            else
                return "subTableIndex"+context.SubQueryIndex+".";
        }

        public static List<ISubOperation> SubItemsConst = SubItems(null);

        public static string GetMethodValue(ExpressionContext context, Expression item, ResolveExpressType type)
        {
            var newContext = context.GetCopyContext();
            newContext.MappingColumns = context.MappingColumns;
            newContext.MappingTables = context.MappingTables;
            newContext.InitMappingInfo = context.InitMappingInfo;
            newContext.RefreshMapping = context.RefreshMapping;
            newContext.IgnoreComumnList = context.IgnoreComumnList;
            newContext.SqlFuncServices = context.SqlFuncServices;
            newContext.Resolve(item, type);
            context.Index = newContext.Index;
            context.ParameterIndex = newContext.ParameterIndex;
            if (newContext.Parameters.HasValue())
                context.Parameters.AddRange(newContext.Parameters);
            return newContext.Result.GetResultString();
        }
    }
}
