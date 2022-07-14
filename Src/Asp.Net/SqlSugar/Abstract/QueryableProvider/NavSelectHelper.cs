using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    internal class NavSelectHelper
    {
        internal static List<TResult> GetList<T, TResult>(Expression<Func<T, TResult>> expression, QueryableProvider<T> queryableProvider)
        {
            List<TResult> result = new List <TResult>();
            var isSqlFunc = IsSqlFunc(expression, queryableProvider);
            if (isSqlFunc&&isGroup(expression, queryableProvider))
            {
                var sqlfuncQueryable = queryableProvider.Clone();
                sqlfuncQueryable.QueryBuilder.Includes = null;
                result = sqlfuncQueryable
                    .Select(expression)
                    .ToList();
                var includeQueryable = queryableProvider.Clone();
                includeQueryable.Select(GetGroupSelect(typeof(T), queryableProvider.Context));
                includeQueryable.QueryBuilder.NoCheckInclude=true;
                MegerList(result, includeQueryable.ToList(), sqlfuncQueryable.Context);
            }
            else if (isSqlFunc) 
            {
                var sqlfuncQueryable = queryableProvider.Clone();
                sqlfuncQueryable.QueryBuilder.Includes = null;
                result = sqlfuncQueryable
                    .Select(expression)
                    .ToList();
                var includeList = queryableProvider.Clone().ToList();
                MegerList(result, includeList, sqlfuncQueryable.Context);
            }
            else
            {
                result= queryableProvider.ToList().Select(expression.Compile()).ToList();
            }
            return result;
        }
        internal static async Task<List<TResult>> GetListAsync<T, TResult>(Expression<Func<T, TResult>> expression, QueryableProvider<T> queryableProvider)
        {
            List<TResult> result = new List<TResult>();
            var isSqlFunc = IsSqlFunc(expression, queryableProvider);
            if (isSqlFunc && isGroup(expression, queryableProvider))
            {
                var sqlfuncQueryable = queryableProvider.Clone();
                sqlfuncQueryable.QueryBuilder.Includes = null;
                result =await sqlfuncQueryable
                    .Select(expression)
                    .ToListAsync();
                var includeQueryable = queryableProvider.Clone();
                includeQueryable.Select(GetGroupSelect(typeof(T), queryableProvider.Context));
                includeQueryable.QueryBuilder.NoCheckInclude = true;
                MegerList(result,await includeQueryable.ToListAsync(), sqlfuncQueryable.Context);
            }
            else if (isSqlFunc)
            {
                var sqlfuncQueryable = queryableProvider.Clone();
                sqlfuncQueryable.QueryBuilder.Includes = null;
                result =await sqlfuncQueryable
                    .Select(expression)
                    .ToListAsync();
                var includeList =await queryableProvider.Clone().ToListAsync();
                MegerList(result, includeList, sqlfuncQueryable.Context);
            }
            else
            {
                var list =await queryableProvider.ToListAsync();
                result = list.Select(expression.Compile()).ToList();
            }
            return result;
        }

        private static string GetGroupSelect(Type type,SqlSugarProvider context)
        {
            var entity = context.EntityMaintenance.GetEntityInfo(type);
            List<string> selector = new List<string>();
            foreach (var item in entity.Columns.Where(it=>it.IsIgnore==false))
            {
                selector.Add($" min({item.DbColumnName}) as {item.DbColumnName}");
            }
            return string.Join(",", selector);
        }

        private static void MegerList<TResult, T>(List<TResult> result, List<T> includeList,SqlSugarProvider context)
        {
            if (result.Count != includeList.Count) return;
            var columns = context.EntityMaintenance.GetEntityInfo<T>().Columns;
            var resColumns = context.EntityMaintenance.GetEntityInfo<TResult>().Columns;
            var i = 0;
            foreach (var item in includeList) 
            {
                foreach (var column in columns) 
                {
                    if (column.Navigat != null) 
                    {
                        var value = column.PropertyInfo.GetValue(item);
                        var resColumn=resColumns
                            .FirstOrDefault(z=>
                            z.PropertyName.Equals(column.PropertyName)&&
                            z.PropertyInfo.PropertyType==column.PropertyInfo.PropertyType
                            );
                        if (resColumn != null) 
                        {
                          var  resItem=  result[i];
                            resColumn.PropertyInfo.SetValue(resItem,value);
                        }
                    }
                }
                i++;
            }
        }

        private static bool IsSqlFunc<T, TResult>(Expression<Func<T, TResult>> expression, QueryableProvider<T> queryableProvider)
        {
            var body=ExpressionTool.GetLambdaExpressionBody(expression);
            if (body is NewExpression) 
            {
                var newExp=((NewExpression)body);
                foreach (var item in newExp.Arguments)
                {
                    if (item is MethodCallExpression) 
                    {
                        var method = ((MethodCallExpression)item).Method;   
                        if (method.DeclaringType != null&& method.DeclaringType.Name=="SqlFunc")
                        {
                            return true;
                        }
                    }
                }
            }
            if (body is MemberInitExpression)
            {
                var newExp = ((MemberInitExpression)body);
                foreach (var item in newExp.Bindings)
                {
                    MemberAssignment memberAssignment = (MemberAssignment)item;
                    if (memberAssignment.Expression is MethodCallExpression)
                    {
                        var method = ((MethodCallExpression)memberAssignment.Expression).Method;
                        if (method.DeclaringType != null && method.DeclaringType.Name == "SqlFunc")
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private static bool isGroup<T, TResult>(Expression<Func<T, TResult>> expression, QueryableProvider<T> queryableProvider)
        {
            var isGroup=queryableProvider.QueryBuilder.GetGroupByString.HasValue();
            return isGroup;
        }
    }
}
