using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace SqlSugar
{
    //局部类 解析字段名
    internal partial class ResolveExpress
    {
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
                    throw new SqlSugarException(FileldErrorMessage);
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
                    throw new SqlSugarException(FileldErrorMessage);
                }
            }
            else
            {
                throw new SqlSugarException(FileldErrorMessage);
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
                    throw new SqlSugarException(FileldErrorMessage);
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
                    throw new SqlSugarException(FileldErrorMessage);
                }
            }
            else
            {
                throw new SqlSugarException(FileldErrorMessage);
            }
        }
    }
}
