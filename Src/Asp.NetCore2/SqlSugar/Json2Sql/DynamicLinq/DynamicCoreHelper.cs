using System;
using System.Collections.Generic; 
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace SqlSugar 
{
    public class DynamicCoreHelper
    {
        public static BuildPropertySelectorResult BuildPropertySelector(string shortName, Type type, List<string> propertyNames, params object[] args)
        {
            BuildPropertySelectorResult result = new BuildPropertySelectorResult();
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (propertyNames == null || propertyNames.Count == 0)
                throw new ArgumentNullException(nameof(propertyNames));

            var parameter = Expression.Parameter(type, shortName);

            // 解析多个属性，生成匿名类型
            var newAnonymousTypeStr = $"new {{ {string.Join(", ", propertyNames)} }}";
            newAnonymousTypeStr = ReplaceFormatParameters(newAnonymousTypeStr);
            result.formattableString = FormattableStringFactory.Create(newAnonymousTypeStr, args); 
            var lambda = SqlSugarDynamicExpressionParser.ParseLambda(new[] { parameter }, null, newAnonymousTypeStr, args);
            result.ResultNewType = lambda.Body.Type;
            result.ShortName = shortName;
            result.Exp = lambda;
            return result;
        }

        public class BuildPropertySelectorResult
        {
            public FormattableString formattableString { get; set; }
            public string ShortName { get; set; }
            public Type ResultNewType { get; set; }
            public LambdaExpression Exp { get; internal set; }
        }
        public static Expression<Func<T, bool>> GetWhere<T>(string shortName, FormattableString whereSql) 
        {
            return (Expression<Func<T, bool>>)GetWhere(typeof(T), shortName, whereSql);
        }
        public static LambdaExpression GetWhere(Type entityType, string shortName, FormattableString whereSql)
        {
            var parameter = Expression.Parameter(entityType, shortName);

            // 提取 FormattableString 中的参数值
            var arguments = whereSql.GetArguments();


            var sql = ReplaceFormatParameters(whereSql.Format);
 
            sql = CompatibleDynamicLinqCoreBug(sql);

            // 构建动态表达式，使用常量表达式和 whereSql 中的参数值
            var lambda = SqlSugarDynamicExpressionParser.ParseLambda(
                new[] { parameter },
                typeof(bool),
               sql,
               whereSql.GetArguments()
            );

            return lambda;
        }
         
        private static string CompatibleDynamicLinqCoreBug(string sql)
        {
            //Compatible DynamicCore.Linq bug
            if (sql?.Contains("SqlFunc.") == true)
            {
                sql = sql.Replace("SqlFunc.LessThan(", "SqlFunc.LessThan_LinqDynamicCore(");
                sql = sql.Replace("SqlFunc.LessThan (", "SqlFunc.LessThan_LinqDynamicCore (");
                sql = sql.Replace("SqlFunc.GreaterThan(", "SqlFunc.GreaterThan_LinqDynamicCore(");
                sql = sql.Replace("SqlFunc.GreaterThan (", "SqlFunc.GreaterThan_LinqDynamicCore (");
            }
            return sql;
        }

        public static LambdaExpression GetObject(Type entityType, string shortName, FormattableString whereSql)
        {
            var parameter = Expression.Parameter(entityType, shortName);

            // 提取 FormattableString 中的参数值
            var arguments = whereSql.GetArguments();


            var sql = ReplaceFormatParameters(whereSql.Format);

            // 构建动态表达式，使用常量表达式和 whereSql 中的参数值
            var lambda = SqlSugarDynamicExpressionParser.ParseLambda(
                new[] { parameter },
                typeof(object),
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

            sql= CompatibleDynamicLinqCoreBug(sql);

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
