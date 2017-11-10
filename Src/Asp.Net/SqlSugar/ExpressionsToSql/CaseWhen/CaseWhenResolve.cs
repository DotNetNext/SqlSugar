using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar
{
    ///<summary>
    /// ** description：Get subquery sql
    /// ** author：sunkaixuan
    /// ** date：2017/9/17
    /// ** email:610262374@qq.com
    /// </summary>
    public class CaseWhenResolve
    {
        List<MethodCallExpression> allMethods = new List<MethodCallExpression>();
        private ExpressionContext context = null;
        private bool hasWhere;
        public CaseWhenResolve(MethodCallExpression expression, ExpressionContext context, Expression oppsiteExpression)
        {
            this.context = context;
            var currentExpression = expression;
            allMethods.Add(currentExpression);
            if (context.IsSingle && oppsiteExpression != null&& oppsiteExpression is MemberExpression)
            {
                var childExpression = (oppsiteExpression as MemberExpression).Expression;
                this.context.SingleTableNameSubqueryShortName = (childExpression as ParameterExpression).Name;
            }
            else if (context.IsSingle)
            {
                this.context.SingleTableNameSubqueryShortName = (context.Expression as LambdaExpression).Parameters.First().Name;
            }
            while (currentExpression != null)
            {
                var addItem = currentExpression.Object as MethodCallExpression;
                if (addItem != null)
                    allMethods.Add(addItem);
                currentExpression = addItem;
            }
        }

        public string GetSql()
        {
            return "";
        }
    }
}
