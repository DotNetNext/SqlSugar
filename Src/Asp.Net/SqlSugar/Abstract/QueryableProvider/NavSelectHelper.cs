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
            var isClass = IsClass(expression, queryableProvider);
            if (isGroup(expression, queryableProvider))
            {
                var sqlfuncQueryable = queryableProvider.Clone();
                sqlfuncQueryable.QueryBuilder.Includes = null;
                result = sqlfuncQueryable
                    .Select(expression)
                    .ToList();
                var includeQueryable = queryableProvider.Clone();
                includeQueryable.Select(GetGroupSelect(typeof(T), queryableProvider.Context, queryableProvider.QueryBuilder));
                includeQueryable.QueryBuilder.NoCheckInclude = true;
                MegerList(result, includeQueryable.ToList(), sqlfuncQueryable.Context);
            }
            else if (isSqlFunc)
            {
                result = SqlFunc(expression, queryableProvider);
            }
            else if (typeof(TResult).IsAnonymousType() && isClass == false) 
            {
                result = SqlFunc(expression, queryableProvider);
            }
            else if (typeof(TResult).IsAnonymousType() && isClass == true)
            {
                result = Action(expression, queryableProvider);
            }
            else
            {
                try
                {
                    result = SqlFunc(expression, queryableProvider);
                }
                catch (Exception ex)
                {
                    try
                    {
                        result = Action(expression, queryableProvider);
                    }
                    catch  
                    {

                        throw ex;
                    }
                }
            }
            return result;
        }

        private static List<TResult> Action<T, TResult>(Expression<Func<T, TResult>> expression, QueryableProvider<T> queryableProvider)
        {
            List<TResult> result;
            var entity = queryableProvider.Context.EntityMaintenance.GetEntityInfo<TResult>();
            var list = queryableProvider.Clone().ToList();
            var dt=queryableProvider.Context.Utilities.ListToDataTable(list);
            foreach (System.Data.DataRow item in dt.Rows)
            {
                foreach (System.Data.DataColumn columnInfo in dt.Columns)
                {
                    if (columnInfo.DataType.IsClass())
                    {
                        if (item[columnInfo.ColumnName] == null || item[columnInfo.ColumnName] == DBNull.Value)
                        {
                            item[columnInfo.ColumnName] = Activator.CreateInstance(columnInfo.DataType, true);
                        }
                    }
                }
            }
            list = queryableProvider.Context.Utilities.DataTableToList<T>(dt);
            result = list.Select(expression.Compile()).ToList();
            return result;
        }

        private static List<TResult> SqlFunc<T, TResult>(Expression<Func<T, TResult>> expression, QueryableProvider<T> queryableProvider)
        {
            List<TResult> result;
            var sqlfuncQueryable = queryableProvider.Clone();
            sqlfuncQueryable.QueryBuilder.Includes = null;
            result = sqlfuncQueryable
                .Select(expression)
                .ToList();
            var selector = GetDefaultSelector(queryableProvider.Context.EntityMaintenance.GetEntityInfo<T>(), queryableProvider.QueryBuilder);
            var queryable = queryableProvider.Select(selector).Clone();
            queryable.QueryBuilder.NoCheckInclude = true;
            var includeList = queryable.ToList();
            MegerList(result, includeList, sqlfuncQueryable.Context);
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
                includeQueryable.Select(GetGroupSelect(typeof(T), queryableProvider.Context, queryableProvider.QueryBuilder));
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

        private static string GetGroupSelect(Type type,SqlSugarProvider context,QueryBuilder queryBuilder)
        {
            var entity = context.EntityMaintenance.GetEntityInfo(type);
            List<string> selector = new List<string>();
            List<EntityColumnInfo> columns = GetListNavColumns(entity);
            foreach (var item in columns)
            {
                if (queryBuilder.TableShortName.HasValue())
                {
                    selector.Add($" min({queryBuilder.TableShortName}.{item.DbColumnName}) as {item.DbColumnName}");
                }
                else
                {
                    selector.Add($" min({item.DbColumnName}) as {item.DbColumnName}");
                }
            }
            return string.Join(",", selector);
        }

        private static string GetDefaultSelector(EntityInfo entityInfo, QueryBuilder queryBuilder)
        {
            List<EntityColumnInfo> columns = GetListNavColumns(entityInfo);
            var selector = new List<string>();
            if (columns.Count == 0) return null;
            foreach (var item in columns)
            {
                if (queryBuilder.TableShortName.HasValue())
                {
                    selector.Add($" {queryBuilder.TableShortName}.{item.DbColumnName} as {item.DbColumnName}");
                }
                else
                {
                    selector.Add($" {item.DbColumnName} as {item.DbColumnName}");
                }
            }
            return string.Join(",", selector);
        }

        private static List<EntityColumnInfo> GetListNavColumns(EntityInfo entityInfo)
        {
            var list = entityInfo.Columns.Where(it => it.Navigat != null).Select(
                 it => it.Navigat.Name
                ).ToArray();
            var list2 = entityInfo.Columns.Where(it => it.Navigat != null && it.Navigat.Name2 != null).Select(
                 it => it.Navigat.Name2
                ).ToArray();
            var columns = entityInfo.Columns.Where(it => it.IsPrimarykey ||
              list.Contains(it.PropertyName) ||
              list2.Contains(it.PropertyName)
            ).ToList();
            return columns;
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
        private static bool IsClass<T, TResult>(Expression<Func<T, TResult>> expression, QueryableProvider<T> queryableProvider)
        {
            var body = ExpressionTool.GetLambdaExpressionBody(expression);
            if (body is NewExpression)
            {
                var newExp = ((NewExpression)body);
                foreach (var item in newExp.Arguments)
                {
                    if (item is MemberExpression)
                    {
                        var member = (MemberExpression)item;
                        if (member.Type.IsClass())
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
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
