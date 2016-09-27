﻿using System;
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
            var isConvet = lambda.Body.NodeType.IsIn(ExpressionType.Convert);
            var isMember = lambda.Body.NodeType.IsIn(ExpressionType.MemberAccess);
            if (!isConvet && !isMember)
            {
                throw new SqlSugarException(FileldErrorMessage);
            }
            try
            {
                if (isConvet)
                {
                    var memberExpr =((UnaryExpression)lambda.Body).Operand as MemberExpression;
                    return memberExpr.Member.Name;
                }
                else//isMember
                {
                    return (lambda.Body as MemberExpression).Member.Name;
                }
            }
            catch (Exception)
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
            var isConvet = lambda.Body.NodeType.IsIn(ExpressionType.Convert);
            var isMember = lambda.Body.NodeType.IsIn(ExpressionType.MemberAccess);
            if (!isConvet && !isMember)
            {
                throw new SqlSugarException(FileldErrorMessage);
            }
            try
            {
                if (isConvet)
                {
                    var memberExpr = ((UnaryExpression)lambda.Body).Operand as MemberExpression;
                    return memberExpr.ToString();
                }
                else//isMember
                {
                    return lambda.Body.ToString();
                }
            }
            catch (Exception)
            {
                 throw new SqlSugarException(FileldErrorMessage);
            }
        }
    }
}
