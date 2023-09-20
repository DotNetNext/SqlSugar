using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace SqlSugar 
{
    public class SqlSugarDynamicExpressionParser
    {

        public static LambdaExpression ParseLambda(ParameterExpression[] parameterExpressions, Type type, string sql, object[] objects)
        {
            if (StaticConfig.DynamicExpressionParserType == null)
            {
                Check.ExceptionEasy("Please at program startup assignment: StaticConfig DynamicExpressionParserType = typeof (DynamicExpressionParser); NUGET is required to install Dynamic.Core", "请在程序启动时赋值: StaticConfig.DynamicExpressionParserType = typeof(DynamicExpressionParser); 需要NUGET安装 Dynamic.Core");
            }

            if (StaticConfig.DynamicExpressionParsingConfig != null)
            {
                // 查找 ParseLambda 方法
                MethodInfo parseLambdaMethod = StaticConfig.DynamicExpressionParserType
                    .GetMyMethod("ParseLambda", 5, StaticConfig.DynamicExpressionParsingConfig.GetType(), typeof(ParameterExpression[]), typeof(Type), typeof(string), typeof(object[]));

                if (parseLambdaMethod == null)
                {
                    throw new InvalidOperationException("ParseLambda method not found in DynamicExpressionParserType.");
                }

                // 调用 ParseLambda 方法来解析 Lambda 表达式
                var lambda = (LambdaExpression)parseLambdaMethod.Invoke(null, new object[] { StaticConfig.DynamicExpressionParsingConfig, parameterExpressions, type, sql, objects });

                return lambda;
            }
            else
            {

                // 查找 ParseLambda 方法
                MethodInfo parseLambdaMethod = StaticConfig.DynamicExpressionParserType
                    .GetMyMethod("ParseLambda",4, typeof(ParameterExpression[]), typeof(Type), typeof(string), typeof(object[]));

                if (parseLambdaMethod == null)
                {
                    throw new InvalidOperationException("ParseLambda method not found in DynamicExpressionParserType.");
                }

                // 调用 ParseLambda 方法来解析 Lambda 表达式
                var lambda = (LambdaExpression)parseLambdaMethod.Invoke(null, new object[] { parameterExpressions, type, sql, objects });

                return lambda;
            }
        }

    }
}
 
