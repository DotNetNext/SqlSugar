using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace SqlSugar
{
    internal partial class ResolveExpress
    {
        /// <summary>
        /// 错误信息
        /// </summary>
        public const string ErrorMessage = "OrderBy、GroupBy、In、Min和Max等操作不是有效拉姆达格式 ，正确格式 it=>it.name ";

        /// <summary>
        /// 获取拉姆达表达式的字段值
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public string GetExpressionRightField(Expression exp)
        {
            LambdaExpression lambda = exp as LambdaExpression;
            if (lambda.Body.NodeType.IsIn(ExpressionType.Convert))
            {
                try
                {
                    var memberExpr =
                  ((UnaryExpression)lambda.Body).Operand as MemberExpression;
                    return memberExpr.Member.Name;
                }
                catch (Exception)
                {
                    throw new SqlSugarException(ErrorMessage);
                }
            }
            else if (lambda.Body.NodeType.IsIn(ExpressionType.MemberAccess))
            {
                try
                {
                    return (lambda.Body as MemberExpression).Member.Name;
                }
                catch (Exception)
                {
                    throw new SqlSugarException(ErrorMessage);
                }
            }
            else
            {
                throw new SqlSugarException(ErrorMessage);
            }
        }

        /// <summary>
        /// 获取拉姆达表达式的字段值多个T模式
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public string GetExpressionRightFieldByNT(Expression exp)
        {
            LambdaExpression lambda = exp as LambdaExpression;
            if (lambda.Body.NodeType.IsIn(ExpressionType.Convert))
            {
                try
                {
                    var memberExpr =
                             ((UnaryExpression)lambda.Body).Operand as MemberExpression;
                    return memberExpr.ToString();
                }
                catch (Exception)
                {
                    throw new SqlSugarException(ErrorMessage);
                }
            }
            else if (lambda.Body.NodeType.IsIn(ExpressionType.MemberAccess))
            {
                try
                {
                    return lambda.Body.ToString();
                }
                catch (Exception)
                {
                    throw new SqlSugarException(ErrorMessage);
                }
            }
            else
            {
                throw new SqlSugarException(ErrorMessage);
            }
        }
    }
}
