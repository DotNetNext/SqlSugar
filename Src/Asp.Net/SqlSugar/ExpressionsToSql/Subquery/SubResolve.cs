using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar
{
    public class SubResolve
    {
        List<MethodCallExpression> allMethods = new List<MethodCallExpression>();
        private ExpressionContext context = null;
        private bool hasWhere;
        public SubResolve(MethodCallExpression expression, ExpressionContext context, Expression oppsiteExpression)
        {
            this.context = context;
            var currentExpression = expression;
            allMethods.Add(currentExpression);
            if (context.IsSingle && oppsiteExpression != null)
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
            List<string> subItems = GetSubItems();
            var sql = string.Join(UtilConstants.Space, subItems);
            return this.context.DbMehtods.Pack(sql);
        }

        private List<string> GetSubItems()
        {
            var isubList = this.allMethods.Select(exp =>
             {
                 var methodName = exp.Method.Name;
                 var item = SubTools.SubItems.First(s => s.Name == methodName);
                 if (item is SubWhere && hasWhere == false)
                 {
                     hasWhere = true;
                 }
                 else
                 {
                     item = SubTools.SubItems.First(s => s is SubAnd);
                 }
                 item.Expression = exp;
                 return item;
             }).ToList();
            isubList.Insert(0, new SubBegin());
            if (isubList.Any(it => it is SubAny || it is SubNotAny))
            {
                isubList.Add(new SubLeftBracket());
                isubList.Add(new SubRightBracket());
                isubList.Add(new SubSelectDefault());
            }
            isubList = isubList.OrderBy(it => it.Sort).ToList();
            List<string> result = isubList.Select(it =>
            {
                return it.GetValue(this.context, it.Expression);
            }).ToList();
            return result;
        }
    }
}
