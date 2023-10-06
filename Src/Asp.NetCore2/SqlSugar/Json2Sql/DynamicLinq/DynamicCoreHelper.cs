using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;

namespace SqlSugar 
{
    public class DynamicCoreHelper
    {
        public static Expression<Func<T, bool>> GetWhere<T>(string shortName, FormattableString whereSql) 
        {
            return (Expression<Func<T, bool>>)GetWhere(typeof(T), shortName, whereSql);
        }
        public static LambdaExpression GetWhere(Type entityType, string shortName, FormattableString whereSql)
        {
            var parameter = Expression.Parameter(entityType, "it");

            // 提取 FormattableString 中的参数值
            var arguments = whereSql.GetArguments();


            var sql = ReplaceFormatParameters(whereSql.Format);

            // 构建动态表达式，使用常量表达式和 whereSql 中的参数值
            var lambda = SqlSugarDynamicExpressionParser.ParseLambda(
                new[] { parameter },
                typeof(bool),
               sql,
               whereSql.GetArguments()
            );

            return lambda;
        }
        public static LambdaExpression GetWhere(Dictionary<string, Type> parameterDictionary, FormattableString whereSql)
        {
            var parameters = parameterDictionary.Select(it => Expression.Parameter(it.Value, it.Key)).ToArray();

            // 提取 FormattableString 中的参数值
            var arguments = whereSql.GetArguments();


            var sql = ReplaceFormatParameters(whereSql.Format);

            // 构建动态表达式，使用常量表达式和 whereSql 中的参数值
            var lambda = SqlSugarDynamicExpressionParser.ParseLambda(
                parameters,
                typeof(bool),
               sql,
               whereSql.GetArguments()
            );

            return lambda;
        }
        public static LambdaExpression GetMember(Dictionary<string,Type> parameterDictionary, Type propertyType, FormattableString memberSql)
        {
            var parameters = parameterDictionary.Select(it=> Expression.Parameter(it.Value,it.Key)).ToArray();

            // 提取 FormattableString 中的参数值
            var arguments = memberSql.GetArguments();


            var sql = ReplaceFormatParameters(memberSql.Format);

            // 构建动态表达式，使用常量表达式和 whereSql 中的参数值
            var lambda = SqlSugarDynamicExpressionParser.ParseLambda(
                parameters,
                propertyType,
               sql,
               memberSql.GetArguments()
            );

            return lambda;
        }
        public static LambdaExpression GetMember(Type entityType,Type propertyType, string shortName, FormattableString memberSql)
        {
            var parameter = Expression.Parameter(entityType, "it");

            // 提取 FormattableString 中的参数值
            var arguments = memberSql.GetArguments();


            var sql = ReplaceFormatParameters(memberSql.Format);

            // 构建动态表达式，使用常量表达式和 whereSql 中的参数值
            var lambda = SqlSugarDynamicExpressionParser.ParseLambda(
                new[] { parameter },
                propertyType,
               sql,
               memberSql.GetArguments()
            );

            return lambda;
        }
        private static string ReplaceFormatParameters(string format)
        {
            int parameterIndex = 0; // 起始参数索引
            return Regex.Replace(format, @"\{\d+\}", match =>
            {
                string replacement = $"@{parameterIndex}";
                parameterIndex++;
                return replacement;
            });
        }
    }
}
