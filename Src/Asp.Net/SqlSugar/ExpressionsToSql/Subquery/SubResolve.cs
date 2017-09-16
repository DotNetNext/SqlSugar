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
        public SubResolve(MethodCallExpression expression, ExpressionContext context,Expression oppsiteExpression)
        {
            this.context = context;
            var currentExpression = expression;
            allMethods.Add(currentExpression);
            if (context.IsSingle) {
                var childExpression = (oppsiteExpression as MemberExpression).Expression;
                this.context.SingleTableNameSubqueryShortName = (childExpression as ParameterExpression).Name;
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
                 item.Expression = exp;
                 return item;
             })
            .OrderBy(it => it.Sort).ToList();
            isubList.Insert(0, new SubBegin());
            List<string> result = isubList.Select(it =>
            {
                return it.GetValue(this.context, it.Expression);
            }).ToList();
            return result;
        }
    }
}
