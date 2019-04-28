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
            if (context.IsSingle && oppsiteExpression != null && oppsiteExpression is MemberExpression)
            {
                var childExpression = (oppsiteExpression as MemberExpression).Expression;
                if (childExpression is ParameterExpression)
                    this.context.SingleTableNameSubqueryShortName = (childExpression as ParameterExpression).Name;
                else {
                    this.context.SingleTableNameSubqueryShortName = (context.Expression as LambdaExpression).Parameters.First().Name;
                }
            }
            else if (context.IsSingle)
            {
                if (context.Expression is LambdaExpression)
                {
                    this.context.SingleTableNameSubqueryShortName = (context.Expression as LambdaExpression).Parameters.First().Name;
                }
                else if (context.Expression is MethodCallExpression)
                {
                    var expArgs = ((context.Expression as MethodCallExpression).Object as MethodCallExpression).Arguments;
                    if (expArgs != null && expArgs.Any())
                    {
                        var meExp = expArgs[0] as LambdaExpression;
                        var selfParameterName = meExp.Parameters.First().Name;
                        context.SingleTableNameSubqueryShortName = (((meExp.Body as BinaryExpression).Left as MemberExpression).Expression as ParameterExpression).Name;
                        if (context.SingleTableNameSubqueryShortName == selfParameterName)
                        {
                            context.SingleTableNameSubqueryShortName = (((meExp.Body as BinaryExpression).Right as MemberExpression).Expression as ParameterExpression).Name;
                        }
                    }
                }
                else if (context.Expression.GetType().Name == "MethodBinaryExpression")
                {

                    var subExp = (context.Expression as BinaryExpression).Left is MethodCallExpression ? (context.Expression as BinaryExpression).Left : (context.Expression as BinaryExpression).Right;
                    var meExp = ((subExp as MethodCallExpression).Object as MethodCallExpression).Arguments[0] as LambdaExpression;
                    var selfParameterName = meExp.Parameters.First().Name;
                    context.SingleTableNameSubqueryShortName = (((meExp.Body as BinaryExpression).Left as MemberExpression).Expression as ParameterExpression).Name;
                    if (context.SingleTableNameSubqueryShortName == selfParameterName)
                    {
                        context.SingleTableNameSubqueryShortName = (((meExp.Body as BinaryExpression).Right as MemberExpression).Expression as ParameterExpression).Name;
                    }
                }
                else
                {
                    Check.Exception(true, "I'm sorry I can't parse the current expression");
                }
            }
            var subIndex = this.context.SubQueryIndex;
            while (currentExpression != null)
            {
                var addItem = currentExpression.Object as MethodCallExpression;
                if (addItem != null)
                    allMethods.Add(addItem);
                if (subIndex==this.context.SubQueryIndex&&addItem !=null&&addItem.Arguments.HasValue()&&addItem.Arguments.Any(it=>it.ToString().Contains("Subqueryable()"))) {
                    this.context.SubQueryIndex++;
                }
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
                 var items = SubTools.SubItems(this.context);
                 var item = items.First(s => s.Name == methodName);
                 if (item is SubWhere && hasWhere == false)
                 {
                     hasWhere = true;
                 }
                 else if (item is SubWhere)
                 {
                     item = items.First(s => s is SubAnd);
                 }

                 if (item is SubWhereIF && hasWhere == false)
                 {
                     hasWhere = true;
                 }
                 else if (item is SubWhereIF)
                 {
                     item = items.First(s => s is SubAndIF);
                 }

                 item.Context = this.context;
                 item.Expression = exp;
                 return item;
             }).ToList();
            isubList.Insert(0, new SubBegin());
            if (isubList.Any(it => it is SubSelect))
            {
                isubList.Add(new SubTop() { Context = this.context });
            }
            if (isubList.Any(it => it is SubAny || it is SubNotAny))
            {
                isubList.Add(new SubLeftBracket());
                isubList.Add(new SubRightBracket());
                isubList.Add(new SubSelectDefault());
            }
            isubList = isubList.OrderBy(it => it.Sort).ToList();
            var isHasWhere = isubList.Where(it => it is SubWhere).Any();
            List<string> result = isubList.Select(it =>
            {
                it.HasWhere = isHasWhere;
                return it.GetValue(it.Expression);
            }).ToList();
            return result;
        }
    }
}
