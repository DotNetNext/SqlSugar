using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Dynamic;
using System.Threading.Tasks;
using System.Threading;
 
namespace SqlSugar
{

    public partial class QueryableProvider<T> : QueryableAccessory, ISugarQueryable<T>
    {
        public async virtual Task<T[]> ToArrayAsync()
        {

            var result = await this.ToListAsync();
            if (result.HasValue())
                return result.ToArray();
            else
                return null;
        }

        public virtual async Task<T> InSingleAsync(object pkValue)
        {
            if (pkValue == null) 
            {
                return default(T);
            }
            Check.Exception(this.QueryBuilder.SelectValue.HasValue(), "'InSingle' and' Select' can't be used together,You can use .Select(it=>...).Single(it.id==1)");
            var list = await In(pkValue).ToListAsync();
            if (list == null) return default(T);
            else return list.SingleOrDefault();
        }
        public async Task<T> SingleAsync()
        {
            if (QueryBuilder.OrderByValue.IsNullOrEmpty())
            {
                QueryBuilder.OrderByValue = QueryBuilder.DefaultOrderByTemplate;
            }
            var oldSkip = QueryBuilder.Skip;
            var oldTake = QueryBuilder.Take;
            var oldOrderBy = QueryBuilder.OrderByValue;
            QueryBuilder.Skip = null;
            QueryBuilder.Take = null;
            QueryBuilder.OrderByValue = null;
            var result = await this.ToListAsync();
            QueryBuilder.Skip = oldSkip;
            QueryBuilder.Take = oldTake;
            QueryBuilder.OrderByValue = oldOrderBy;
            if (result == null || result.Count == 0)
            {
                return default(T);
            }
            else if (result.Count == 2)
            {
                Check.Exception(true, ErrorMessage.GetThrowMessage(".Single()  result must not exceed one . You can use.First()", "使用single查询结果集不能大于1，适合主键查询，如果大于1你可以使用Queryable.First"));
                return default(T);
            }
            else
            {
                return result.SingleOrDefault();
            }
        }
        public async Task<T> SingleAsync(Expression<Func<T, bool>> expression)
        {
            _Where(expression);
            var result = await SingleAsync();
            this.QueryBuilder.WhereInfos.Remove(this.QueryBuilder.WhereInfos.Last());
            return result;
        }
        public Task<T> FirstAsync(CancellationToken token) 
        {
            this.Context.Ado.CancellationToken = token;
            return FirstAsync();
        }
        public async Task<T> FirstAsync()
        {
            if (QueryBuilder.OrderByValue.IsNullOrEmpty())
            {
                QueryBuilder.OrderByValue = QueryBuilder.DefaultOrderByTemplate;
            }
            if (QueryBuilder.Skip.HasValue)
            {
                QueryBuilder.Take = 1;
                var list = await this.ToListAsync();
                return list.FirstOrDefault();
            }
            else
            {
                QueryBuilder.Skip = 0;
                QueryBuilder.Take = 1;
                var result = await this.ToListAsync();
                if (result.HasValue())
                    return result.FirstOrDefault();
                else
                    return default(T);
            }
        }
        public Task<T> FirstAsync(Expression<Func<T, bool>> expression, CancellationToken token) 
        {
            this.Context.Ado.CancellationToken = token;
            return FirstAsync(expression);
        }
        public async Task<T> FirstAsync(Expression<Func<T, bool>> expression)
        {
            _Where(expression);
            var result = await FirstAsync();
            this.QueryBuilder.WhereInfos.Remove(this.QueryBuilder.WhereInfos.Last());
            return result;
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> expression)
        {
            _Where(expression);
            var result = await AnyAsync();
            this.QueryBuilder.WhereInfos.Remove(this.QueryBuilder.WhereInfos.Last());
            return result;
        }

        public Task<bool> AnyAsync(Expression<Func<T, bool>> expression, CancellationToken token) 
        {
            this.Context.Ado.CancellationToken = token;
            return AnyAsync(expression);
        }

        public async Task<bool> AnyAsync()
        {
            return await this.CountAsync() > 0;
        }

        public Task<int> CountAsync(CancellationToken token) 
        {
            this.Context.Ado.CancellationToken = token;
            return CountAsync();
        }
        public async Task<int> CountAsync()
        {
            if (this.QueryBuilder.Skip == null &&
             this.QueryBuilder.Take == null &&
             this.QueryBuilder.OrderByValue == null &&
             this.QueryBuilder.PartitionByValue == null &&
             this.QueryBuilder.SelectValue == null &&
             this.QueryBuilder.Includes == null &&
             this.QueryBuilder.IsDistinct == false)
            {
                var list = await this.Clone().Select<int>(" COUNT(1) ").ToListAsync();
                return list.FirstOrDefault();
            }
            MappingTableList expMapping;
            int result;
            _CountBegin(out expMapping, out result);
            if (IsCache)
            {
                var cacheService = this.Context.CurrentConnectionConfig.ConfigureExternalServices.DataInfoCacheService;
                result = CacheSchemeMain.GetOrCreate<int>(cacheService, this.QueryBuilder, () => { return GetCount(); }, CacheTime, this.Context, CacheKey);
            }
            else
            {
                result = await GetCountAsync();
            }
            _CountEnd(expMapping);
            return result;
        }
        public async Task<int> CountAsync(Expression<Func<T, bool>> expression)
        {
            _Where(expression);
            var result = await CountAsync();
            this.QueryBuilder.WhereInfos.Remove(this.QueryBuilder.WhereInfos.Last());
            return result;
        }

        public Task<int> CountAsync(Expression<Func<T, bool>> expression, CancellationToken token) 
        {
            this.Context.Ado.CancellationToken = token;
            return CountAsync(expression);
        }

        public async Task<TResult> MaxAsync<TResult>(string maxField)
        {
            this.Select(string.Format(QueryBuilder.MaxTemplate, maxField));
            var list = await this._ToListAsync<TResult>();
            var result = list.SingleOrDefault();
            return result;
        }

        public Task<TResult> MaxAsync<TResult>(string maxField, CancellationToken token) 
        {
            this.Context.Ado.CancellationToken= token;
            return MaxAsync<TResult>(maxField);
        }

        public Task<TResult> MaxAsync<TResult>(Expression<Func<T, TResult>> expression)
        {
            return _MaxAsync<TResult>(expression);
        }

        public Task<TResult> MaxAsync<TResult>(Expression<Func<T, TResult>> expression, CancellationToken token) 
        {
            this.Context.Ado.CancellationToken = token;
            return MaxAsync(expression);
        }

        public async Task<TResult> MinAsync<TResult>(string minField)
        {
            this.Select(string.Format(QueryBuilder.MinTemplate, minField));
            var list = await this._ToListAsync<TResult>();
            var result = list.SingleOrDefault();
            return result;
        }
        public Task<TResult> MinAsync<TResult>(Expression<Func<T, TResult>> expression)
        {
            return _MinAsync<TResult>(expression);
        }

        public async Task<TResult> SumAsync<TResult>(string sumField)
        {
            this.Select(string.Format(QueryBuilder.SumTemplate, sumField));
            var list = await this._ToListAsync<TResult>();
            var result = list.SingleOrDefault();
            return result;
        }
        public Task<TResult> SumAsync<TResult>(Expression<Func<T, TResult>> expression)
        {
            return _SumAsync<TResult>(expression);
        }

        public async Task<TResult> AvgAsync<TResult>(string avgField)
        {
            this.Select(string.Format(QueryBuilder.AvgTemplate, avgField));
            var list = await this._ToListAsync<TResult>();
            var result = list.SingleOrDefault();
            return result;
        }
        public Task<TResult> AvgAsync<TResult>(Expression<Func<T, TResult>> expression)
        {
            return _AvgAsync<TResult>(expression);
        }

        public async virtual Task<List<TResult>> ToListAsync<TResult>(Expression<Func<T, TResult>> expression)
        {
            if (this.QueryBuilder.Includes != null && this.QueryBuilder.Includes.Count > 0)
            {
                return await NavSelectHelper.GetListAsync(expression, this);
            }
            else
            {
                var list = await this.Select(expression).ToListAsync();
                return list;
            }
        }
        public Task<List<T>> ToListAsync()
        {
            InitMapping();
            return _ToListAsync<T>();
        }

        public Task<List<T>> ToListAsync(CancellationToken token) 
        {
            this.Context.Ado.CancellationToken = token;
            return ToListAsync();
        }
        public Task<List<T>> ToPageListAsync(int pageNumber, int pageSize, CancellationToken token) 
        {
            this.Context.Ado.CancellationToken = token;
            return ToPageListAsync(pageNumber, pageSize);
        }
        public Task<List<T>> ToPageListAsync(int pageIndex, int pageSize)
        {
            pageIndex = _PageList(pageIndex, pageSize);
            return ToListAsync();
        }
        public async virtual Task<List<TResult>> ToPageListAsync<TResult>(int pageIndex, int pageSize, RefAsync<int> totalNumber, Expression<Func<T, TResult>> expression)
        {
            if (this.QueryBuilder.Includes != null && this.QueryBuilder.Includes.Count > 0)
            {
                if (pageIndex == 0)
                    pageIndex = 1;
                var list = await this.Clone().Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(expression);
                var countQueryable = this.Clone();
                countQueryable.QueryBuilder.Includes = null;
                totalNumber.Value = await countQueryable.CountAsync();
                return list;
            }
            else
            {
                var list = await this.Select(expression).ToPageListAsync(pageIndex, pageSize, totalNumber);
                return list;
            }
        }
        public Task<List<T>> ToPageListAsync(int pageNumber, int pageSize, RefAsync<int> totalNumber, CancellationToken token) 
        {
            this.Context.Ado.CancellationToken= token;
            return ToPageListAsync(pageNumber, pageSize, totalNumber);
        }
        public async Task<List<T>> ToPageListAsync(int pageIndex, int pageSize, RefAsync<int> totalNumber)
        {
            var oldMapping = this.Context.MappingTables;
            var countQueryable = this.Clone();
            if (countQueryable.QueryBuilder.Offset == "true")
            {
                countQueryable.QueryBuilder.Offset = null;
            }
            totalNumber.Value = await countQueryable.CountAsync();
            this.Context.MappingTables = oldMapping;
            return await this.Clone().ToPageListAsync(pageIndex, pageSize);
        }
        public async Task<List<T>> ToPageListAsync(int pageNumber, int pageSize, RefAsync<int> totalNumber, RefAsync<int> totalPage)
        {
            var result = await ToPageListAsync(pageNumber, pageSize, totalNumber);
            totalPage.Value = (totalNumber.Value + pageSize - 1) / pageSize;
            return result;
        }

        public Task<List<T>> ToPageListAsync(int pageNumber, int pageSize, RefAsync<int> totalNumber, RefAsync<int> totalPage, CancellationToken token) 
        {
            this.Context.Ado.CancellationToken = token;
            return ToPageListAsync(pageNumber,pageSize,totalNumber,totalPage);
        }

        public async Task<string> ToJsonAsync()
        {
            if (IsCache)
            {
                var cacheService = this.Context.CurrentConnectionConfig.ConfigureExternalServices.DataInfoCacheService;
                var result = CacheSchemeMain.GetOrCreate<string>(cacheService, this.QueryBuilder, () =>
                {
                    return this.Context.Utilities.SerializeObject(this.ToList(), typeof(T));
                }, CacheTime, this.Context, CacheKey);
                return result;
            }
            else
            {
                return this.Context.Utilities.SerializeObject(await this.ToListAsync(), typeof(T));
            }
        }
        public async Task<string> ToJsonPageAsync(int pageIndex, int pageSize)
        {
            return this.Context.Utilities.SerializeObject(await this.ToPageListAsync(pageIndex, pageSize), typeof(T));
        }
        public async Task<string> ToJsonPageAsync(int pageIndex, int pageSize, RefAsync<int> totalNumber)
        {
            var oldMapping = this.Context.MappingTables;
            totalNumber.Value = await this.Clone().CountAsync();
            this.Context.MappingTables = oldMapping;
            return await this.Clone().ToJsonPageAsync(pageIndex, pageSize);
        }
        public async virtual Task<DataTable> ToDataTableByEntityAsync()
        {
            var list =await this.ToListAsync();
            return this.Context.Utilities.ListToDataTable(list);
        }
        public async Task<DataTable> ToDataTableAsync()
        {
            QueryBuilder.ResultType = typeof(SugarCacheDataTable);
            InitMapping();
            var sqlObj = this._ToSql();
            RestoreMapping();
            DataTable result = null;
            if (IsCache)
            {
                var cacheService = this.Context.CurrentConnectionConfig.ConfigureExternalServices.DataInfoCacheService;
                result = CacheSchemeMain.GetOrCreate<DataTable>(cacheService, this.QueryBuilder, () => { return this.Db.GetDataTable(sqlObj.Key, sqlObj.Value.ToArray()); }, CacheTime, this.Context, CacheKey);
            }
            else
            {
                result = await this.Db.GetDataTableAsync(sqlObj.Key, sqlObj.Value.ToArray());
            }
            return result;
        }
        public Task<DataTable> ToDataTablePageAsync(int pageIndex, int pageSize)
        {
            pageIndex = _PageList(pageIndex, pageSize);
            return ToDataTableAsync();
        }
        public async Task<DataTable> ToDataTablePageAsync(int pageIndex, int pageSize, RefAsync<int> totalNumber)
        {
            var oldMapping = this.Context.MappingTables;
            totalNumber.Value = await this.Clone().CountAsync();
            this.Context.MappingTables = oldMapping;
            return await this.Clone().ToDataTablePageAsync(pageIndex, pageSize);
        }
        public async Task<DataTable> ToDataTableByEntityPageAsync(int pageNumber, int pageSize, RefAsync<int> totalNumber) 
        {
            var list =await this.ToPageListAsync(pageNumber, pageSize, totalNumber);
            return this.Context.Utilities.ListToDataTable(list);
        }
        public async Task<List<T>> ToOffsetPageAsync(int pageIndex, int pageSize, RefAsync<int> totalNumber)
        {
            if (this.Context.CurrentConnectionConfig.DbType != DbType.SqlServer)
            {
                this.QueryBuilder.Offset = "true";
                return await this.ToPageListAsync(pageIndex, pageSize, totalNumber);
            }
            else
            {
                totalNumber.Value = await this.Clone().CountAsync();
                _ToOffsetPage(pageIndex, pageSize);
                return await this.Clone().ToListAsync();
            }
        }
        
        public virtual async Task ForEachAsync(Action<T> action, int singleMaxReads = 300, System.Threading.CancellationTokenSource cancellationTokenSource = null)
        {
            Check.Exception(this.QueryBuilder.Skip > 0 || this.QueryBuilder.Take > 0, ErrorMessage.GetThrowMessage("no support Skip take, use PageForEach", "不支持Skip Take,请使用 Queryale.PageForEach"));
            RefAsync<int> totalNumber = 0;
            RefAsync<int> totalPage = 1;
            for (int i = 1; i <= totalPage; i++)
            {
                if (cancellationTokenSource?.IsCancellationRequested == true) return;
                var queryable = this.Clone();
                var page =
                    totalPage == 1 ?
                    await queryable.ToPageListAsync(i, singleMaxReads, totalNumber, totalPage) :
                    await queryable.ToPageListAsync(i, singleMaxReads);
                foreach (var item in page)
                {
                    if (cancellationTokenSource?.IsCancellationRequested == true) return;
                    action.Invoke(item);
                }
            }
        }
        public virtual async Task ForEachByPageAsync(Action<T> action, int pageIndex, int pageSize, RefAsync<int> totalNumber, int singleMaxReads = 300, System.Threading.CancellationTokenSource cancellationTokenSource = null)
        {
            int count = this.Clone().Count();
            if (count > 0)
            {
                if (pageSize > singleMaxReads && count - ((pageIndex - 1) * pageSize) > singleMaxReads)
                {
                    Int32 Skip = (pageIndex - 1) * pageSize;
                    Int32 NowCount = count - Skip;
                    Int32 number = 0;
                    if (NowCount > pageSize) NowCount = pageSize;
                    while (NowCount > 0)
                    {
                        if (cancellationTokenSource?.IsCancellationRequested == true) return;
                        if (number + singleMaxReads > pageSize) singleMaxReads = NowCount;
                        foreach (var item in await this.Clone().Skip(Skip).Take(singleMaxReads).ToListAsync())
                        {
                            if (cancellationTokenSource?.IsCancellationRequested == true) return;
                            action.Invoke(item);
                        }
                        NowCount -= singleMaxReads;
                        Skip += singleMaxReads;
                        number += singleMaxReads;
                    }
                }
                else
                {
                    if (cancellationTokenSource?.IsCancellationRequested == true) return;
                    foreach (var item in this.Clone().ToPageList(pageIndex, pageSize))
                    {
                        if (cancellationTokenSource?.IsCancellationRequested == true) return;
                        action.Invoke(item);
                    }
                }
            }
            totalNumber.Value = count;
        }
        
        public async Task<List<T>> SetContextAsync<ParameterT>(Expression<Func<T, object>> thisFiled1, Expression<Func<object>> mappingFiled1,
Expression<Func<T, object>> thisFiled2, Expression<Func<object>> mappingFiled2,
ParameterT parameter)
        {
            if (parameter == null)
            {
                return new List<T>();
            }
            var rightEntity = this.Context.EntityMaintenance.GetEntityInfo<ParameterT>();
            var leftEntity = this.Context.EntityMaintenance.GetEntityInfo<T>();
            List<T> result = new List<T>();
            var queryableContext = this.Context.TempItems["Queryable_To_Context"] as MapperContext<ParameterT>;
            var list = queryableContext.list;
            var key = thisFiled1.ToString() + mappingFiled1.ToString() +
                      thisFiled2.ToString() + mappingFiled2.ToString() +
                       typeof(ParameterT).FullName + typeof(T).FullName;
            MappingFieldsHelper<ParameterT> fieldsHelper = new MappingFieldsHelper<ParameterT>();
            var mappings = new List<MappingFieldsExpression>() {
               new MappingFieldsExpression(){
               LeftColumnExpression=thisFiled1,
               LeftEntityColumn=leftEntity.Columns.First(it=>it.PropertyName==ExpressionTool.GetMemberName(thisFiled1)),
               RightColumnExpression=mappingFiled1,
               RightEntityColumn=rightEntity.Columns.First(it=>it.PropertyName==ExpressionTool.GetMemberName(mappingFiled1))
             },
               new MappingFieldsExpression(){
               LeftColumnExpression=thisFiled2,
               LeftEntityColumn=leftEntity.Columns.First(it=>it.PropertyName==ExpressionTool.GetMemberName(thisFiled2)),
               RightColumnExpression=mappingFiled2,
               RightEntityColumn=rightEntity.Columns.First(it=>it.PropertyName==ExpressionTool.GetMemberName(mappingFiled2))
             }
            };
            var conditionals = fieldsHelper.GetMppingSql(list.Cast<object>().ToList(), mappings);
            if (queryableContext.TempChildLists == null)
                queryableContext.TempChildLists = new Dictionary<string, object>();
            if (list != null && queryableContext.TempChildLists.ContainsKey(key))
            {
                result = (List<T>)queryableContext.TempChildLists[key];
            }
            else
            {
                result = await this.Clone().Where(conditionals, true).ToListAsync();
                queryableContext.TempChildLists[key] = result;
            }
            List<object> listObj = result.Select(it => (object)it).ToList();
            object obj = (object)parameter;
            var newResult = fieldsHelper.GetSetList(obj, listObj, mappings).Select(it => (T)it).ToList();
            return newResult;
        }
        public async Task<List<T>> SetContextAsync<ParameterT>(Expression<Func<T, object>> thisFiled, Expression<Func<object>> mappingFiled, ParameterT parameter)
        {
            List<T> result = new List<T>();
            var entity = this.Context.EntityMaintenance.GetEntityInfo<ParameterT>();
            var queryableContext = this.Context.TempItems["Queryable_To_Context"] as MapperContext<ParameterT>;
            var list = queryableContext.list;
            var pkName = "";
            if ((mappingFiled as LambdaExpression).Body is UnaryExpression)
            {
                pkName = (((mappingFiled as LambdaExpression).Body as UnaryExpression).Operand as MemberExpression).Member.Name;
            }
            else
            {
                pkName = ((mappingFiled as LambdaExpression).Body as MemberExpression).Member.Name;
            }
            var key = thisFiled.ToString() + mappingFiled.ToString() + typeof(ParameterT).FullName + typeof(T).FullName;
            var ids = list.Select(it => it.GetType().GetProperty(pkName).GetValue(it)).ToArray();
            if (queryableContext.TempChildLists == null)
                queryableContext.TempChildLists = new Dictionary<string, object>();
            if (list != null && queryableContext.TempChildLists.ContainsKey(key))
            {
                result = (List<T>)queryableContext.TempChildLists[key];
            }
            else
            {
                if (queryableContext.TempChildLists == null)
                    queryableContext.TempChildLists = new Dictionary<string, object>();
                await this.Context.Utilities.PageEachAsync(ids, 200, async pageIds =>
                {
                    result.AddRange(await this.Clone().In(thisFiled, pageIds).ToListAsync());
                });
                queryableContext.TempChildLists[key] = result;
            }
            var name = "";
            if ((thisFiled as LambdaExpression).Body is UnaryExpression)
            {
                name = (((thisFiled as LambdaExpression).Body as UnaryExpression).Operand as MemberExpression).Member.Name;
            }
            else
            {
                name = ((thisFiled as LambdaExpression).Body as MemberExpression).Member.Name;
            }
            var pkValue = parameter.GetType().GetProperty(pkName).GetValue(parameter);
            result = result.Where(it => it.GetType().GetProperty(name).GetValue(it).ObjToString() == pkValue.ObjToString()).ToList();
            return result;
        }
       
        public async Task<Dictionary<string, object>> ToDictionaryAsync(Expression<Func<T, object>> key, Expression<Func<T, object>> value)
        {
            if (this.QueryBuilder.IsSingle() == false && (this.QueryBuilder.AsTables == null || this.QueryBuilder.AsTables.Count == 0))
            {
                return await this.MergeTable().ToDictionaryAsync(key, value);
            }
            this.QueryBuilder.ResultType = typeof(SugarCacheDictionary);
            var keyName = QueryBuilder.GetExpressionValue(key, ResolveExpressType.FieldSingle).GetResultString();
            var valueName = QueryBuilder.GetExpressionValue(value, ResolveExpressType.FieldSingle).GetResultString();
            var list = await this.Select<KeyValuePair<string, object>>(keyName + "," + valueName).ToListAsync();
            var isJson = this.Context.EntityMaintenance.GetEntityInfo<T>().Columns.Where(it => it.IsJson && it.PropertyName == ExpressionTool.GetMemberName(value)).Any();
            if (isJson)
            {
                var result = this.Select<T>(keyName + "," + valueName).ToList().ToDictionary(ExpressionTool.GetMemberName(key), ExpressionTool.GetMemberName(value));
                return result;
            }
            else
            {
                var result = list.ToDictionary(it => it.Key.ObjToString(), it => it.Value);
                return result;
            }
        }
        public async Task<List<T>> ToTreeAsync(string childPropertyName, string parentIdPropertyName, object rootValue, string primaryKeyPropertyName)
        {
            var entity = this.Context.EntityMaintenance.GetEntityInfo<T>();
            var pk = primaryKeyPropertyName;
            var list =await this.ToListAsync();
            Expression<Func<T, IEnumerable<object>>> childListExpression = (Expression<Func<T, IEnumerable<object>>>)ExpressionBuilderHelper.CreateExpressionSelectField(typeof(T), childPropertyName, typeof(IEnumerable<object>));
            Expression<Func<T, object>> parentIdExpression = (Expression<Func<T, object>>)ExpressionBuilderHelper.CreateExpressionSelectFieldObject(typeof(T), parentIdPropertyName);
            return  GetTreeRoot(childListExpression, parentIdExpression, pk, list, rootValue) ?? new List<T>();
        }
        public async Task<List<T>> ToTreeAsync(Expression<Func<T, IEnumerable<object>>> childListExpression, Expression<Func<T, object>> parentIdExpression, object rootValue, object[] childIds)
        {
            var list = await this.ToListAsync();
            return TreeAndFilterIds(childListExpression, parentIdExpression, rootValue, childIds, ref list) ?? new List<T>();
        }
        public async Task<List<T>> ToTreeAsync(Expression<Func<T, IEnumerable<object>>> childListExpression, Expression<Func<T, object>> parentIdExpression, object rootValue, object[] childIds, Expression<Func<T, object>> primaryKeyExpression)
        {
            var list = await this.ToListAsync();
            return TreeAndFilterIds(childListExpression, parentIdExpression,primaryKeyExpression, rootValue, childIds, ref list) ?? new List<T>();
        }
        public async Task<List<T>> ToTreeAsync(Expression<Func<T, IEnumerable<object>>> childListExpression, Expression<Func<T, object>> parentIdExpression, object rootValue)
        {
            var entity = this.Context.EntityMaintenance.GetEntityInfo<T>();
            var pk = GetTreeKey(entity); ;
            var list = await this.ToListAsync();
            return GetTreeRoot(childListExpression, parentIdExpression, pk, list, rootValue) ?? new List<T>();
        }
        public async Task<List<T>> ToTreeAsync(Expression<Func<T, IEnumerable<object>>> childListExpression, Expression<Func<T, object>> parentIdExpression, object rootValue, Expression<Func<T, object>> primaryKeyExpression)
        {
            var entity = this.Context.EntityMaintenance.GetEntityInfo<T>();
            var pk = ExpressionTool.GetMemberName(primaryKeyExpression); ;
            var list = await this.ToListAsync();
            return GetTreeRoot(childListExpression, parentIdExpression, pk, list, rootValue) ?? new List<T>();
        }
        public async Task<List<T>> ToParentListAsync(Expression<Func<T, object>> parentIdExpression, object primaryKeyValue)
        {
            List<T> result = new List<T>() { };
            var entity = this.Context.EntityMaintenance.GetEntityInfo<T>();
            var isTreeKey = entity.Columns.Any(it => it.IsTreeKey);
            if (isTreeKey)
            {
                return await _ToParentListByTreeKeyAsync(parentIdExpression, primaryKeyValue);
            }
            Check.Exception(entity.Columns.Where(it => it.IsPrimarykey).Count() == 0, "No Primary key");
            var parentIdName = UtilConvert.ToMemberExpression((parentIdExpression as LambdaExpression).Body).Member.Name;
            var ParentInfo = entity.Columns.First(it => it.PropertyName == parentIdName);
            var parentPropertyName = ParentInfo.DbColumnName;
            var tableName = this.QueryBuilder.GetTableNameString;
            if (this.QueryBuilder.IsSingle() == false)
            {
                if (this.QueryBuilder.JoinQueryInfos.Count > 0)
                {
                    tableName = this.QueryBuilder.JoinQueryInfos.First().TableName;
                }
                if (this.QueryBuilder.EasyJoinInfos.Count > 0)
                {
                    tableName = this.QueryBuilder.JoinQueryInfos.First().TableName;
                }
            }
            var current = await this.Context.Queryable<T>().AS(tableName).Filter(null, this.QueryBuilder.IsDisabledGobalFilter).InSingleAsync(primaryKeyValue);
            if (current != null)
            {
                result.Add(current);
                object parentId = ParentInfo.PropertyInfo.GetValue(current, null);
                int i = 0;
                while (parentId != null && await this.Context.Queryable<T>().AS(tableName).Filter(null, this.QueryBuilder.IsDisabledGobalFilter).In(parentId).AnyAsync())
                {
                    Check.Exception(i > 100, ErrorMessage.GetThrowMessage("Dead cycle", "出现死循环或超出循环上限（100），检查最顶层的ParentId是否是null或者0"));
                    var parent = await this.Context.Queryable<T>().AS(tableName).Filter(null, this.QueryBuilder.IsDisabledGobalFilter).InSingleAsync(parentId);
                    result.Add(parent);
                    parentId = ParentInfo.PropertyInfo.GetValue(parent, null);
                    ++i;
                }
            }
            return result;
        }
        public async Task<List<T>> ToParentListAsync(Expression<Func<T, object>> parentIdExpression, object primaryKeyValue, Expression<Func<T, bool>> parentWhereExpression)
        {
            List<T> result = new List<T>() { };
            var entity = this.Context.EntityMaintenance.GetEntityInfo<T>();
            var isTreeKey = entity.Columns.Any(it => it.IsTreeKey);
            if (isTreeKey)
            {
                return await _ToParentListByTreeKeyAsync(parentIdExpression, primaryKeyValue,parentWhereExpression);
            }
            Check.Exception(entity.Columns.Where(it => it.IsPrimarykey).Count() == 0, "No Primary key");
            var parentIdName = UtilConvert.ToMemberExpression((parentIdExpression as LambdaExpression).Body).Member.Name;
            var ParentInfo = entity.Columns.First(it => it.PropertyName == parentIdName);
            var parentPropertyName = ParentInfo.DbColumnName;
            var tableName = this.QueryBuilder.GetTableNameString;
            if (this.QueryBuilder.IsSingle() == false)
            {
                if (this.QueryBuilder.JoinQueryInfos.Count > 0)
                {
                    tableName = this.QueryBuilder.JoinQueryInfos.First().TableName;
                }
                if (this.QueryBuilder.EasyJoinInfos.Count > 0)
                {
                    tableName = this.QueryBuilder.JoinQueryInfos.First().TableName;
                }
            }
            var current = await this.Context.Queryable<T>().AS(tableName).WhereIF(parentWhereExpression!=default, parentWhereExpression).Filter(null, this.QueryBuilder.IsDisabledGobalFilter).InSingleAsync(primaryKeyValue);
            if (current != null)
            {
                result.Add(current);
                object parentId = ParentInfo.PropertyInfo.GetValue(current, null);
                int i = 0;
                while (parentId != null && await this.Context.Queryable<T>().AS(tableName).WhereIF(parentWhereExpression!=default, parentWhereExpression).Filter(null, this.QueryBuilder.IsDisabledGobalFilter).In(parentId).AnyAsync())
                {
                    Check.Exception(i > 100, ErrorMessage.GetThrowMessage("Dead cycle", "出现死循环或超出循环上限（100），检查最顶层的ParentId是否是null或者0"));
                    var parent = await this.Context.Queryable<T>().AS(tableName).WhereIF(parentWhereExpression!=default, parentWhereExpression).Filter(null, this.QueryBuilder.IsDisabledGobalFilter).InSingleAsync(parentId);
                    result.Add(parent);
                    parentId = ParentInfo.PropertyInfo.GetValue(parent, null);
                    ++i;
                }
            }
            return result;
        }
        public async Task<List<T>> ToChildListAsync(Expression<Func<T, object>> parentIdExpression, object primaryKeyValue, bool isContainOneself = true)
        {
            var entity = this.Context.EntityMaintenance.GetEntityInfo<T>();
            var pk = GetTreeKey(entity);
            var list = await this.ToListAsync();
            return GetChildList(parentIdExpression, pk, list, primaryKeyValue, isContainOneself);
        }

        public async Task<List<T>> ToChildListAsync(Expression<Func<T, object>> parentIdExpression, object[] primaryKeyValues, bool isContainOneself = true)
        {
            var entity = this.Context.EntityMaintenance.GetEntityInfo<T>();
            var pk = GetTreeKey(entity);
            var list =await this.ToListAsync();
            List<T> result = new List<T>();
            foreach (var item in primaryKeyValues)
            {
                result.AddRange(GetChildList(parentIdExpression, pk, list, item, isContainOneself));
            }
            return result;
        }
        public Task<int> IntoTableAsync<TableEntityType>(CancellationToken cancellationToken = default)
        {
            return IntoTableAsync(typeof(TableEntityType), cancellationToken);
        }
        public Task<int> IntoTableAsync<TableEntityType>(string TableName, CancellationToken cancellationToken = default)
        {
            return IntoTableAsync(typeof(TableEntityType), TableName, cancellationToken );
        }
        public Task<int> IntoTableAsync(Type TableEntityType, CancellationToken cancellationToken = default)
        {
            var entityInfo = this.Context.EntityMaintenance.GetEntityInfo(TableEntityType);
            var name = this.SqlBuilder.GetTranslationTableName(entityInfo.DbTableName);
            return IntoTableAsync(TableEntityType, name, cancellationToken);
        }
        public async Task<int> IntoTableAsync(Type TableEntityType, string TableName, CancellationToken cancellationToken = default)
        {
            this.Context.Ado.CancellationToken= cancellationToken;
            KeyValuePair<string, List<SugarParameter>> sqlInfo;
            string sql;
            OutIntoTableSql(TableName, out sqlInfo, out sql,TableEntityType);
            return await this.Context.Ado.ExecuteCommandAsync(sql, sqlInfo.Value);
        }
    }
}
