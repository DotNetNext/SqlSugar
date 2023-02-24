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
                var mappingColumn = GetMappingColumn(expression);
                MegerList(result, includeQueryable.ToList(), sqlfuncQueryable.Context, mappingColumn);
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
            else if (expression.ToString().Contains("FirstOrDefault()")) 
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
                        Console.WriteLine("Select DTO  error  . Warning:" + ex.Message);
                        result = Action(expression, queryableProvider);
                    }
                    catch
                    {

                        throw;
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
            var mappingColumn = GetMappingColumn(expression);
            if (mappingColumn.Any(it => it.IsError)) 
            {
                return Action(expression,queryableProvider);
            }
            List<TResult> result;
            var sqlfuncQueryable = queryableProvider.Clone();
            var dtoEntity = sqlfuncQueryable.Context.EntityMaintenance.GetEntityInfo<TResult>().Columns;
            var tableEntity = sqlfuncQueryable.Context.EntityMaintenance.GetEntityInfo<T>().Columns;
            var ignoreColumns = GetIgnoreColumns(dtoEntity,tableEntity);
            sqlfuncQueryable.QueryBuilder.Includes = null;
            result = sqlfuncQueryable
                .IgnoreColumns(ignoreColumns)
                .Select(expression)
                .ToList();
            var selector = GetDefaultSelector(queryableProvider.Context.EntityMaintenance.GetEntityInfo<T>(), queryableProvider.QueryBuilder);
            var queryable = queryableProvider.Select(selector).Clone();
            queryable.QueryBuilder.NoCheckInclude = true;
            var includeList = queryable.ToList();
            MegerList(result, includeList, sqlfuncQueryable.Context,mappingColumn);
            return result;
        }

        private static  string[] GetIgnoreColumns(List<EntityColumnInfo> dtoEntity, List<EntityColumnInfo> tableEntity)
        {
            var column = (from dto in dtoEntity
                    join tab in tableEntity on dto.PropertyInfo.PropertyType equals tab.PropertyInfo.PropertyType
                    where tab.Navigat!=null
                    select tab.PropertyName).Distinct().ToArray();
            return column;
        }

        internal static async Task<List<TResult>> GetListAsync<T, TResult>(Expression<Func<T, TResult>> expression, QueryableProvider<T> queryableProvider)
        {
            return await Task.Run(()=> { return GetList(expression,queryableProvider); });
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

        private static void MegerList<TResult, T>(List<TResult> result, List<T> includeList,SqlSugarProvider context,List<NavMappingColumn> navMappingColumns)
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
                        if (resColumn == null && navMappingColumns.Any(z => z.Value == column.PropertyName)) 
                        {
                            var mappingColumn = navMappingColumns.First(z => z.Value == column.PropertyName);
                            resColumn = resColumns
                           .FirstOrDefault(z =>
                           z.PropertyName.Equals(mappingColumn.Key) &&
                           z.PropertyInfo.PropertyType == column.PropertyInfo.PropertyType
                           );
                        }
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
        private static List<NavMappingColumn> GetMappingColumn(Expression expression)
        {
            var body = ExpressionTool.GetLambdaExpressionBody(expression);
            var parameterName=(expression as LambdaExpression).Parameters.FirstOrDefault().Name;
            List<NavMappingColumn> result = new List<NavMappingColumn>();
            if (body is NewExpression)
            {
                var index = 0;
                var arg = ((NewExpression)body).Arguments;
                var members = ((NewExpression)body).Members;
                foreach (var item in arg)
                {
                    var name=members[index].Name;
                    if (item is MethodCallExpression)
                    {
                        AddCallError(result, item, parameterName);
                    }
                    index++;
                }
            }
            else if (body is MemberInitExpression)
            {
                foreach (var item in ((MemberInitExpression)body).Bindings)
                {
                    MemberAssignment memberAssignment = (MemberAssignment)item;
                    var key= memberAssignment.Member.Name;
                    var value = memberAssignment.Expression;
                    if (memberAssignment.Expression is MemberExpression)
                    {
                        result.Add(new NavMappingColumn() { Key = key, Value = ExpressionTool.GetMemberName(memberAssignment.Expression) });
                    }
                    else if(memberAssignment.Expression is MethodCallExpression)
                    {
                        AddCallError(result, memberAssignment.Expression,parameterName);
                    }
                }
            }
            return result;
        }

        private static void AddCallError(List<NavMappingColumn> result, Expression item,string parameterName)
        {
            var method = (item as MethodCallExpression);
            if (method.Method.Name == "ToList" && method.Arguments.Count > 0 && method.Arguments[0] is MethodCallExpression)
            {
                method = (MethodCallExpression)method.Arguments[0];
            }
            if (method.Method.Name == "Select")
            {
                if (!item.ToString().Contains("Subqueryable"))
                {
                    result.Add(new NavMappingColumn() { IsError = true });
                }
            }
            else if (method.Method.Name == "Join")
            {
    
                if (item.ToString().Contains($" {parameterName}."))
                {
                    result.Add(new NavMappingColumn() { IsError = true });
                }
            }
        }

        private static bool isGroup<T, TResult>(Expression<Func<T, TResult>> expression, QueryableProvider<T> queryableProvider)
        {
            var isGroup=queryableProvider.QueryBuilder.GetGroupByString.HasValue();
            return isGroup;
        }

        internal class NavMappingColumn 
        {
            public string Key { get; set; }
            public string Value { get; set; }
            public bool IsError { get;  set; }
        }
    }
}
