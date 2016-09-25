using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace SqlSugar.Core.ResolveExpressCore
{
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
                var memberExpr =
                      ((UnaryExpression)lambda.Body).Operand as MemberExpression;
                return memberExpr.Member.Name;
            }
            else if (lambda.Body.NodeType.IsIn(ExpressionType.MemberAccess))
            {
                return (lambda.Body as MemberExpression).Member.Name;
            }
            else
            {
                Check.Exception(true, "不是有效拉姆达格式" + exp.ToString());
                return null;
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
                var memberExpr =
                      ((UnaryExpression)lambda.Body).Operand as MemberExpression;
                return memberExpr.ToString();
            }
            else if (lambda.Body.NodeType.IsIn(ExpressionType.MemberAccess))
            {
                return lambda.Body.ToString();
            }
            else
            {
                Check.Exception(true, "不是有效拉姆达格式" + exp.ToString());
                return null;
            }
        }
    }
}
