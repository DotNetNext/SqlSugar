﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;

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
        private string subKey = "$SubAs:";
        private bool hasWhere;
        private bool isXmlPath = false;
        private bool isAsAttr = false;
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
            else if (context.IsSingle&&  ExpressionTool.GetMethodName(currentExpression) != "ToList")
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
                        if (meExp != null)
                        {
                            var selfParameterName = meExp.Parameters.First().Name;
                            if ((meExp.Body is BinaryExpression))
                            {
                                context.SingleTableNameSubqueryShortName = (((meExp.Body as BinaryExpression).Left as MemberExpression)?.Expression as ParameterExpression)?.Name;
                            }
                            if (ExpressionTool.GetMethodName(context.Expression).IsContainsIn("ToList") && meExp.Parameters.Any(it => it.Name == selfParameterName))
                            {
                                if (meExp.Body is BinaryExpression)
                                {
                                    context.SingleTableNameSubqueryShortName = (((meExp.Body as BinaryExpression).Right as MemberExpression)?.Expression as ParameterExpression)?.Name;
                                }
                            }
                            if (context.SingleTableNameSubqueryShortName == selfParameterName)
                            {
                                if (meExp.Body is BinaryExpression)
                                {
                                    context.SingleTableNameSubqueryShortName = (((meExp.Body as BinaryExpression).Right as MemberExpression)?.Expression as ParameterExpression)?.Name;
                                }
                            }
                        }
                    }
                }
                else if (context.Expression.GetType().Name == "MethodBinaryExpression")
                {

                    var subExp = (context.Expression as BinaryExpression).Left is MethodCallExpression ? (context.Expression as BinaryExpression).Left : (context.Expression as BinaryExpression).Right;
                    if (subExp is MethodCallExpression)
                    {
                        var meExp = ((subExp as MethodCallExpression).Object as MethodCallExpression).Arguments[0] as LambdaExpression;
                        var selfParameterName = meExp.Parameters.First().Name;
                        context.SingleTableNameSubqueryShortName = (((meExp.Body as BinaryExpression).Left as MemberExpression).Expression as ParameterExpression).Name;
                        if (context.SingleTableNameSubqueryShortName == selfParameterName)
                        {
                            context.SingleTableNameSubqueryShortName = (((meExp.Body as BinaryExpression).Right as MemberExpression).Expression as ParameterExpression).Name;
                        }
                    }
                }
                else if (context.RootExpression != null && context.Expression.GetType().Name == "SimpleBinaryExpression")
                {
                    var name = (this.context.RootExpression as LambdaExpression).Parameters[0].Name;
                    context.SingleTableNameSubqueryShortName = name;
                }
                else if (context.Expression is BinaryExpression) 
                {
                    var subExp = (context.Expression as BinaryExpression).Left is MethodCallExpression ? (context.Expression as BinaryExpression).Left : (context.Expression as BinaryExpression).Right;
                    if (subExp is MethodCallExpression)
                    {
                        var argus = ((subExp as MethodCallExpression).Object as MethodCallExpression).Arguments;
                        if (argus.Count > 0)
                        {
                            var meExp = argus[0] as LambdaExpression;
                            var selfParameterName = meExp.Parameters.First().Name;
                            context.SingleTableNameSubqueryShortName = (((meExp.Body as BinaryExpression).Left as MemberExpression)?.Expression as ParameterExpression)?.Name;
                            if (context.SingleTableNameSubqueryShortName == selfParameterName)
                            {
                                context.SingleTableNameSubqueryShortName = (((meExp.Body as BinaryExpression).Right as MemberExpression).Expression as ParameterExpression).Name;
                            }
                        }
                    }
                }
                else if (context.Expression is MemberInitExpression memberInitExpression)
                { 
                    var getParameters = ExpressionTool.GetParameters(context.Expression).Select(it=>it.Name).Distinct().ToList();
                    if (getParameters?.Count()>1)
                    {
                        context.SingleTableNameSubqueryShortName = getParameters.First();
                    }
                }
                else
                {
                    Check.ExceptionEasy( "I'm sorry I can't parse the current expression","不支持当前表达式");
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
            var sqlItems = subItems.Where(it => !it.StartsWith(subKey)).ToList();
            var asItems = subItems.Where(it => it.StartsWith(subKey)).ToList();
            if (asItems.Any())
            {
                GetSubAs(sqlItems, asItems);
            }
            if (this.context.CurrentShortName.HasValue())
            {
                GetShortName(sqlItems);
            }
            var sql = "";

            if (sqlItems.Count(it => IsJoin(it)) > 1)
            {
                var index = sqlItems.IndexOf(sqlItems.First(x=>IsJoin(x)));
                var joinitems = sqlItems.Where(it => IsJoin(it)).ToList();
                joinitems.Reverse();
                var items = sqlItems.Where(it => !IsJoin(it)).ToList();
                items.InsertRange(index, joinitems);
                sql = string.Join(UtilConstants.Space, items);
            }
            else
            {
                sql = string.Join(UtilConstants.Space, sqlItems);
            }
            if (isXmlPath)
            {
                var xmlPath = context.DbMehtods.GetForXmlPath();
                if (xmlPath.HasValue()) 
                {
                    sql = sql + xmlPath;
                }
            }
            return this.context.DbMehtods.Pack(sql);
        }

        private static bool IsJoin(string it)
        {
            return it.StartsWith("  INNER JOIN") || it.StartsWith("  LEFT JOIN");
        }

        private void GetSubAs(List<string> sqlItems, List<string> asItems)
        {
            for (int i = 0; i < sqlItems.Count; i++)
            {
                if (sqlItems[i].StartsWith("FROM " + this.context.SqlTranslationLeft))
                {
                    var asName = this.context.GetTranslationTableName(asItems.First().Replace(subKey, ""), false);
                    var repKey = $"\\{this.context.SqlTranslationLeft}.+\\{this.context.SqlTranslationRight}";
                    if (this.context.IsSingle&&this.context.JoinIndex==0&&this.context.CurrentShortName.HasValue()&& isAsAttr&& !asName.Contains(this.context.CurrentShortName))
                    {
                        asName = asName + " " + this.context.CurrentShortName+" ";
                    }
                    sqlItems[i] = Regex.Replace(sqlItems[i], repKey, asName);
                }
            }
        }
        private void GetShortName(List<string> sqlItems)
        {
            for (int i = 0; i < sqlItems.Count; i++)
            {
                if (sqlItems[i].StartsWith("FROM " + this.context.SqlTranslationLeft))
                {
                    if (isAsAttr&&sqlItems[i].EndsWith(this.context.CurrentShortName + " "))
                    {
                    }
                    else
                    {
                        sqlItems[i] = sqlItems[i] + " " + this.context.CurrentShortName + " ";
                    }
                }
            }
        }

        private List<string> GetSubItems()
        {
            var isSubSubQuery = this.allMethods.Select(it => it.ToString()).Any(it => Regex.Matches(it, "Subquery").Count > 1);
            var isubList = this.allMethods.Select(exp =>
            {
                if (isSubSubQuery)
                {
                    this.context.JoinIndex = 1;
                    this.context.SubQueryIndex = 0;
                }
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
                else if (item is SubSelectStringJoin)
                {
                    isXmlPath = true;
                }
                else if (item is SubAsWithAttr)
                {
                    isAsAttr = true;
                }

                item.Context = this.context;
                item.Expression = exp;
                return item;
            }).ToList();
            SetOrderByIndex(isubList);
            isubList.Insert(0, new SubBegin());
            if (isubList.Any(it => it is SubSelect||it is SubFirst))
            {
                isubList.Add(new SubTop() { Context = this.context });
            }
            if (isubList.Any(it => it is SubAny || it is SubNotAny))
            {
                isubList.Add(new SubLeftBracket());
                isubList.Add(new SubRightBracket());
                isubList.Add(new SubSelectDefault());
            }
            var db = this.context?.SugarContext?.Context;
            if (db != null&& db?.CurrentConnectionConfig?.DbType == DbType.SqlServer)
            {
                if (db.CurrentConnectionConfig?.MoreSettings?.IsWithNoLockSubquery == true)
                {
                    if (!isubList.Any(it => it is SubWithNolock))
                    {
                        isubList.Add(new SubWithNolock() { Context = this.context });
                    }
                }
            }
            isubList = isubList.OrderBy(it => it.Sort).ToList();
            var isHasWhere = isubList.Where(it => it is SubWhere).Any();
            var isJoin = isubList.Any(it => it is SubInnerJoin || it is SubLeftJoin);
            if (isJoin)
            {
                this.context.JoinIndex++;
            }
            if (isAsAttr) 
            {
                this.context.IsAsAttr = true;
            }
            if (isubList.Any(it => it is SubSelect) && isubList.Any(it => it is SubTake)) 
            {
                isubList.RemoveAll(it => it is SubTake);
            }
            List<string> result = isubList.Select(it =>
            {
                it.HasWhere = isHasWhere;
                return it.GetValue(it.Expression);
            }).ToList();
            this.context.JoinIndex = 0;
            this.context.IsAsAttr = false;
            return result;
        }

        private static void SetOrderByIndex(List<ISubOperation> isubList)
        {
            var orderByIndex = 0;
            var orderByList = isubList.Where(it => it is SubOrderBy || it is SubOrderByDesc).ToList();
            if (orderByList.Count > 1)
            {
                orderByList.Reverse();
                foreach (var item in orderByList)
                {
                    if (item is SubOrderBy)
                    {
                        (item as SubOrderBy).OrderIndex = orderByIndex;
                        orderByIndex++;
                    }
                    else if (item is SubOrderByDesc)
                    {
                        (item as SubOrderByDesc).OrderIndex = orderByIndex;
                        orderByIndex++;
                    }
                }
            }
        }
    }
}
