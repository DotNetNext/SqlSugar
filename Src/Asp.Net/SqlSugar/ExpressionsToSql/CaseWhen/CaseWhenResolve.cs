using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar
{

    public class CaseWhenResolve
    {
        List<MethodCallExpression> allMethods = new List<MethodCallExpression>();
        private ExpressionContext context = null;
        public CaseWhenResolve(MethodCallExpression expression, ExpressionContext context, Expression oppsiteExpression)
        {
            this.context = context;
            var currentExpression = expression;
            allMethods.Add(currentExpression);
            if (context.IsSingle && oppsiteExpression != null && oppsiteExpression is MemberExpression)
            {
                var childExpression = (oppsiteExpression as MemberExpression).Expression;
                if ((childExpression as ParameterExpression) != null)
                {
                    this.context.SingleTableNameSubqueryShortName = (childExpression as ParameterExpression).Name;
                }
            }
            else if (context.IsSingle)
            {
                if ((context.Expression as LambdaExpression) != null)
                {
                    this.context.SingleTableNameSubqueryShortName = (context.Expression as LambdaExpression).Parameters.First().Name;
                }
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
            allMethods.Reverse();
            List<KeyValuePair<string, string>> sqls = new List<KeyValuePair<string, string>>();
            foreach (var methodExp in allMethods)
            {
                var isFirst = allMethods.First() == methodExp;
                var isLast = allMethods.Last() == methodExp;
                if (methodExp.Arguments.Count == 0)
                {
                    sqls.Add(new KeyValuePair<string, string>(methodExp.Method.Name, "null"));
                }
                else
                {
                    var sql = SubTools.GetMethodValue(this.context, methodExp.Arguments[0], this.context.IsSingle ? ResolveExpressType.WhereSingle : ResolveExpressType.WhereMultiple);
                    sqls.Add(new KeyValuePair<string, string>(methodExp.Method.Name, sql));
                }
            }
            var result = this.context.DbMehtods.CaseWhen(sqls);
            return result;
        }
    }
}
