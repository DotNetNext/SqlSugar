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
using Newtonsoft.Json.Serialization;

namespace SqlSugar
{

    public partial class QueryableProvider<T> : QueryableAccessory, ISugarQueryable<T>
    {
        public virtual T Single()
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
            var result = this.ToList();
            QueryBuilder.Skip = oldSkip;
            QueryBuilder.Take = oldTake;
            QueryBuilder.OrderByValue = oldOrderBy;
            if (result == null || result.Count == 0)
            {
                return default(T);
            }
            else if (result.Count >= 2)
            {
                Check.Exception(true, ErrorMessage.GetThrowMessage(".Single()  result must not exceed one . You can use.First()", "使用single查询结果集不能大于1，适合主键查询，如果大于1你可以使用Queryable.First"));
                return default(T);
            }
            else
            {
                return result.SingleOrDefault();
            }
        }
        public virtual T Single(Expression<Func<T, bool>> expression)
        {
            _Where(expression);
            var result = Single();
            this.QueryBuilder.WhereInfos.Remove(this.QueryBuilder.WhereInfos.Last());
            return result;
        }

        public virtual T First()
        {
            if (QueryBuilder.OrderByValue.IsNullOrEmpty())
            {
                QueryBuilder.OrderByValue = QueryBuilder.DefaultOrderByTemplate;
            }
            if (QueryBuilder.Skip.HasValue)
            {
                QueryBuilder.Take = 1;
                return this.ToList().FirstOrDefault();
            }
            else
            {
                QueryBuilder.Skip = 0;
                QueryBuilder.Take = 1;
                var result = this.ToList();
                if (result.HasValue())
                    return result.FirstOrDefault();
                else
                    return default(T);
            }
        }
        public virtual T First(Expression<Func<T, bool>> expression)
        {
            _Where(expression);
            var result = First();
            this.QueryBuilder.WhereInfos.Remove(this.QueryBuilder.WhereInfos.Last());
            return result;
        }

        public virtual bool Any(Expression<Func<T, bool>> expression)
        {
            _Where(expression);
            var result = Any();
            this.QueryBuilder.WhereInfos.Remove(this.QueryBuilder.WhereInfos.Last());
            return result;
        }
        public virtual bool Any()
        {
            return this.Select("1").ToList().Count() > 0;
        }

        public virtual List<TResult> ToList<TResult>(Expression<Func<T, TResult>> expression)
        {
            if (this.QueryBuilder.Includes != null && this.QueryBuilder.Includes.Count > 0)
            {
                return NavSelectHelper.GetList(expression, this);
                // var list = this.ToList().Select(expression.Compile()).ToList();
                // return list;
            }
            else
            {
                var list = this.Select(expression).ToList();
                return list;
            }
        }

        public virtual int Count()
        {
            if (this.QueryBuilder.Skip == null &&
                this.QueryBuilder.Take == null &&
                this.QueryBuilder.OrderByValue == null &&
                this.QueryBuilder.PartitionByValue == null &&
                this.QueryBuilder.SelectValue == null &&
                this.QueryBuilder.Includes == null &&
                this.QueryBuilder.IsDistinct == false)
            {

                return this.Clone().Select<int>(" COUNT(1) ").ToList().FirstOrDefault();
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
                result = GetCount();
            }
            _CountEnd(expMapping);
            return result;
        }
        public virtual int Count(Expression<Func<T, bool>> expression)
        {
            _Where(expression);
            var result = Count();
            this.QueryBuilder.WhereInfos.Remove(this.QueryBuilder.WhereInfos.Last());
            return result;
        }

        public virtual TResult Max<TResult>(string maxField)
        {
            this.Select(string.Format(QueryBuilder.MaxTemplate, maxField));
            var result = this._ToList<TResult>().SingleOrDefault();
            return result;
        }
        public virtual TResult Max<TResult>(Expression<Func<T, TResult>> expression)
        {
            return _Max<TResult>(expression);
        }

        public virtual TResult Min<TResult>(string minField)
        {
            this.Select(string.Format(QueryBuilder.MinTemplate, minField));
            var result = this._ToList<TResult>().SingleOrDefault();
            return result;
        }
        public virtual TResult Min<TResult>(Expression<Func<T, TResult>> expression)
        {
            return _Min<TResult>(expression);
        }

        public virtual TResult Sum<TResult>(string sumField)
        {
            this.Select(string.Format(QueryBuilder.SumTemplate, sumField));
            var result = this._ToList<TResult>().SingleOrDefault();
            return result;
        }
        public virtual TResult Sum<TResult>(Expression<Func<T, TResult>> expression)
        {
            return _Sum<TResult>(expression);
        }

        public virtual TResult Avg<TResult>(string avgField)
        {
            this.Select(string.Format(QueryBuilder.AvgTemplate, avgField));
            var result = this._ToList<TResult>().SingleOrDefault();
            return result;
        }
        public virtual TResult Avg<TResult>(Expression<Func<T, TResult>> expression)
        {
            return _Avg<TResult>(expression);
        }
        public virtual T[] ToArray()
        {

            var result = this.ToList();
            if (result.HasValue())
                return result.ToArray();
            else
                return null;
        }

        public virtual string ToJson()
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
                return this.Context.Utilities.SerializeObject(this.ToList(), typeof(T));
            }
        }
        public virtual string ToJsonPage(int pageIndex, int pageSize)
        {
            return this.Context.Utilities.SerializeObject(this.ToPageList(pageIndex, pageSize), typeof(T));
        }
        public virtual string ToJsonPage(int pageIndex, int pageSize, ref int totalNumber)
        {
            return this.Context.Utilities.SerializeObject(this.ToPageList(pageIndex, pageSize, ref totalNumber), typeof(T));
        }
        public virtual DataTable ToPivotTable<TColumn, TRow, TData>(Func<T, TColumn> columnSelector, Expression<Func<T, TRow>> rowSelector, Func<IEnumerable<T>, TData> dataSelector)
        {
            return this.ToList().ToPivotTable(columnSelector, rowSelector, dataSelector);
        }
        public virtual List<dynamic> ToPivotList<TColumn, TRow, TData>(Func<T, TColumn> columnSelector, Expression<Func<T, TRow>> rowSelector, Func<IEnumerable<T>, TData> dataSelector)
        {
            return this.ToList().ToPivotList(columnSelector, rowSelector, dataSelector);
        }
        public virtual string ToPivotJson<TColumn, TRow, TData>(Func<T, TColumn> columnSelector, Expression<Func<T, TRow>> rowSelector, Func<IEnumerable<T>, TData> dataSelector)
        {
            var list = this.ToPivotList(columnSelector, rowSelector, dataSelector);
            return this.Context.Utilities.SerializeObject(list);
        }
        public List<T> ToChildList(Expression<Func<T, object>> parentIdExpression, object primaryKeyValue, bool isContainOneself = true)
        {
            var entity = this.Context.EntityMaintenance.GetEntityInfo<T>();
            var pk = GetTreeKey(entity);
            var list = this.ToList();
            return GetChildList(parentIdExpression, pk, list, primaryKeyValue, isContainOneself);
        }
        public List<T> ToChildList(Expression<Func<T, object>> parentIdExpression, object [] primaryKeyValues, bool isContainOneself = true)
        {
            var entity = this.Context.EntityMaintenance.GetEntityInfo<T>();
            var pk = GetTreeKey(entity);
            var list = this.ToList();
            List<T> result = new List<T>();
            foreach (var item in primaryKeyValues)
            {
                result.AddRange(GetChildList(parentIdExpression, pk, list, item, isContainOneself));
            }
            return result;
        }
        public List<T> ToParentList(Expression<Func<T, object>> parentIdExpression, object primaryKeyValue)
        {
            var entity = this.Context.EntityMaintenance.GetEntityInfo<T>();
            var isTreeKey = entity.Columns.Any(it => it.IsTreeKey);
            if (isTreeKey)
            {
                return _ToParentListByTreeKey(parentIdExpression, primaryKeyValue);
            }
            List<T> result = new List<T>() { };
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
            var current = this.Context.Queryable<T>().AS(tableName).Filter(null, this.QueryBuilder.IsDisabledGobalFilter).InSingle(primaryKeyValue);
            if (current != null)
            {
                result.Add(current);
                object parentId = ParentInfo.PropertyInfo.GetValue(current, null);
                int i = 0;
                while (parentId != null && this.Context.Queryable<T>().AS(tableName).Filter(null, this.QueryBuilder.IsDisabledGobalFilter).In(parentId).Any())
                {
                    Check.Exception(i > 100, ErrorMessage.GetThrowMessage("Dead cycle", "出现死循环或超出循环上限（100），检查最顶层的ParentId是否是null或者0"));
                    var parent = this.Context.Queryable<T>().AS(tableName).Filter(null, this.QueryBuilder.IsDisabledGobalFilter).InSingle(parentId);
                    result.Add(parent);
                    parentId = ParentInfo.PropertyInfo.GetValue(parent, null);
                    ++i;
                }
            }
            return result;
        }
        public List<T> ToParentList(Expression<Func<T, object>> parentIdExpression, object primaryKeyValue, Expression<Func<T, bool>> parentWhereExpression)
        {
            var entity = this.Context.EntityMaintenance.GetEntityInfo<T>();
            var isTreeKey = entity.Columns.Any(it => it.IsTreeKey);
            if (isTreeKey)
            {
                return _ToParentListByTreeKey(parentIdExpression, primaryKeyValue,parentWhereExpression);
            }
            List<T> result = new List<T>() { };
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
            var current = this.Context.Queryable<T>().AS(tableName).WhereIF(parentWhereExpression!=default, parentWhereExpression).Filter(null, this.QueryBuilder.IsDisabledGobalFilter).InSingle(primaryKeyValue);
            if (current != null)
            {
                result.Add(current);
                object parentId = ParentInfo.PropertyInfo.GetValue(current, null);
                int i = 0;
                while (parentId != null && this.Context.Queryable<T>().AS(tableName).WhereIF(parentWhereExpression!=default, parentWhereExpression).Filter(null, this.QueryBuilder.IsDisabledGobalFilter).In(parentId).Any())
                {
                    Check.Exception(i > 100, ErrorMessage.GetThrowMessage("Dead cycle", "出现死循环或超出循环上限（100），检查最顶层的ParentId是否是null或者0"));
                    var parent = this.Context.Queryable<T>().AS(tableName).WhereIF(parentWhereExpression!=default, parentWhereExpression).Filter(null, this.QueryBuilder.IsDisabledGobalFilter).InSingle(parentId);
                    result.Add(parent);
                    parentId = ParentInfo.PropertyInfo.GetValue(parent, null);
                    ++i;
                }
            }
            return result;
        }

        public List<T> ToTree(string childPropertyName, string parentIdPropertyName, object rootValue, string primaryKeyPropertyName)
        {
            var entity = this.Context.EntityMaintenance.GetEntityInfo<T>();
            var pk = primaryKeyPropertyName;
            var list = this.ToList();
            Expression<Func<T,IEnumerable<object> >> childListExpression = (Expression<Func<T, IEnumerable<object>>>)ExpressionBuilderHelper.CreateExpressionSelectField(typeof(T),childPropertyName,typeof(IEnumerable<object>));
            Expression<Func<T, object>> parentIdExpression = (Expression<Func<T, object>>)ExpressionBuilderHelper.CreateExpressionSelectFieldObject(typeof(T), parentIdPropertyName); 
            return GetTreeRoot(childListExpression, parentIdExpression, pk, list, rootValue) ?? new List<T>();
        }
        public List<T> ToTree(Expression<Func<T, IEnumerable<object>>> childListExpression, Expression<Func<T, object>> parentIdExpression, object rootValue, Expression<Func<T, object>> primaryKeyExpression)
        {
            var entity = this.Context.EntityMaintenance.GetEntityInfo<T>();
            var pk = ExpressionTool.GetMemberName(primaryKeyExpression);
            var list = this.ToList();
            return GetTreeRoot(childListExpression, parentIdExpression, pk, list, rootValue) ?? new List<T>();
        }
        public List<T> ToTree(Expression<Func<T, IEnumerable<object>>> childListExpression, Expression<Func<T, object>> parentIdExpression, object rootValue)
        {
            var entity = this.Context.EntityMaintenance.GetEntityInfo<T>();
            var pk = GetTreeKey(entity);
            var list = this.ToList();
            return GetTreeRoot(childListExpression, parentIdExpression, pk, list, rootValue)??new List<T>();
        }
        public  List<T> ToTree(Expression<Func<T, IEnumerable<object>>> childListExpression, Expression<Func<T, object>> parentIdExpression, object rootValue, object[] childIds)
        {
            var list =  this.ToList();
            return TreeAndFilterIds(childListExpression, parentIdExpression, rootValue, childIds, ref list) ?? new List<T>();
        } 
        public List<T> ToTree(Expression<Func<T, IEnumerable<object>>> childListExpression, Expression<Func<T, object>> parentIdExpression, object rootValue, object[] childIds, Expression<Func<T, object>> primaryKeyExpression)
        {
            var list = this.ToList();
            return TreeAndFilterIds(childListExpression, parentIdExpression,primaryKeyExpression, rootValue, childIds, ref list) ?? new List<T>();
        }
        public virtual DataTable ToDataTableByEntity()
        {
            var list = this.ToList();
            return this.Context.Utilities.ListToDataTable(list);
        }
        public virtual DataTable ToDataTable()
        {
            QueryBuilder.ResultType = typeof(SugarCacheDataTable);
            InitMapping();
            var sqlObj = this.ToSql();
            RestoreMapping();
            DataTable result = null;
            bool isChangeQueryableMasterSlave = GetIsMasterQuery();
            bool isChangeQueryableSlave = GetIsSlaveQuery();
            if (IsCache)
            {
                var cacheService = this.Context.CurrentConnectionConfig.ConfigureExternalServices.DataInfoCacheService;
                result = CacheSchemeMain.GetOrCreate<DataTable>(cacheService, this.QueryBuilder, () => { return this.Db.GetDataTable(sqlObj.Key, sqlObj.Value.ToArray()); }, CacheTime, this.Context, CacheKey);
            }
            else
            {
                result = this.Db.GetDataTable(sqlObj.Key, sqlObj.Value.ToArray());
            }
            RestChangeMasterQuery(isChangeQueryableMasterSlave);
            RestChangeSlaveQuery(isChangeQueryableSlave);
            return result;
        }
        public virtual DataTable ToDataTablePage(int pageIndex, int pageSize)
        {
            if (pageIndex == 0)
                pageIndex = 1;
            if (QueryBuilder.PartitionByValue.HasValue())
            {
                QueryBuilder.ExternalPageIndex = pageIndex;
                QueryBuilder.ExternalPageSize = pageSize;
            }
            else
            {
                QueryBuilder.Skip = (pageIndex - 1) * pageSize;
                QueryBuilder.Take = pageSize;
            }
            return ToDataTable();
        }
        public DataTable ToDataTableByEntityPage(int pageNumber, int pageSize, ref int totalNumber) 
        {
            var  list=this.ToPageList(pageNumber, pageSize,ref totalNumber);
            return this.Context.Utilities.ListToDataTable(list);    
        }
        public virtual DataTable ToDataTablePage(int pageIndex, int pageSize, ref int totalNumber)
        {
            _RestoreMapping = false;
            totalNumber = this.Clone().Count();
            _RestoreMapping = true;
            var result = this.Clone().ToDataTablePage(pageIndex, pageSize);
            return result;
        }
        public virtual DataTable ToDataTablePage(int pageIndex, int pageSize, ref int totalNumber, ref int totalPage)
        {
            var result = ToDataTablePage(pageIndex, pageSize, ref totalNumber);
            totalPage = (totalNumber + pageSize - 1) / pageSize;
            return result;
        }

        public Dictionary<string, object> ToDictionary(Expression<Func<T, object>> key, Expression<Func<T, object>> value)
        {
            if (this.QueryBuilder.IsSingle() == false && (this.QueryBuilder.AsTables == null||this.QueryBuilder.AsTables.Count==0)) 
            {
                return this.MergeTable().ToDictionary(key,value);
            }
            this.QueryBuilder.ResultType = typeof(SugarCacheDictionary);
            var keyName = QueryBuilder.GetExpressionValue(key, ResolveExpressType.FieldSingle).GetResultString();
            var valueName = QueryBuilder.GetExpressionValue(value, ResolveExpressType.FieldSingle).GetResultString();
            if (this.QueryBuilder.IsSingle() == false)
            {
                keyName = this.QueryBuilder.TableShortName + "." + keyName;
                valueName = this.QueryBuilder.TableShortName + "." + valueName;
            }
            var isJson=this.Context.EntityMaintenance.GetEntityInfo<T>().Columns.Where(it => it.IsJson && it.PropertyName == ExpressionTool.GetMemberName(value)).Any();
            if (isJson)
            {
                var result = this.Select<T>(keyName + "," + valueName).ToList().ToDictionary(ExpressionTool.GetMemberName(key), ExpressionTool.GetMemberName(value));
                return result;
            }
            else if (valueName == null) 
            {
                // 编译key和value的表达式树为委托
                var keySelector = key.Compile();
                var valueSelector = value.Compile();
                Dictionary<object,object> objDic= this.ToList().ToDictionary(keySelector, valueSelector); 
                return objDic.ToDictionary(it=>it.Key?.ToString(),it=>it.Value);
            }
            else
            {
                var result = this.Select<KeyValuePair<string, object>>(keyName + "," + valueName).ToList().ToDictionary(it => it.Key.ObjToString(), it => it.Value);
                return result;
            }
        }

        public List<Dictionary<string, object>> ToDictionaryList()
        {
            var list = this.ToList();
            if (list == null)
                return null;
            else
                return this.Context.Utilities.DeserializeObject<List<Dictionary<string, object>>>(this.Context.Utilities.SerializeObject(list));
        }
        public async Task<List<Dictionary<string, object>>> ToDictionaryListAsync()
        {
            var list = await this.ToListAsync();
            if (list == null)
                return null;
            else
                return this.Context.Utilities.DeserializeObject<List<Dictionary<string, object>>>(this.Context.Utilities.SerializeObject(list));
        }

        public virtual List<T> ToList()
        {
            InitMapping();
            return _ToList<T>();
        }
        public List<T> SetContext<ParameterT>(Expression<Func<T, bool>> whereExpression, ParameterT parameter) 
        {
            var queryableContext = this.Context.TempItems["Queryable_To_Context"] as MapperContext<ParameterT>;
            var rootList = queryableContext.list;
            List<ISugarQueryable<object>> queryableList = new List<ISugarQueryable<object>>();
            var index = rootList.IndexOf(parameter);
            var selector = this.Clone().QueryBuilder.GetSelectValue+$",{index} as ";
            var sqlObj=this.Clone().Where(whereExpression).Select(selector+"")
            .Select(it => (object) new { it, sql_sugar_index = index });
            queryableList.Add(sqlObj);

            var allList = this.Context.Union(queryableList)
                .Select(it=>new { it=default(T), sql_sugar_index =0})
                .Select("*").ToList();
            var result = new List<T>();
            throw new Exception("开发中");
        }
        public List<T> SetContext<ParameterT>(Expression<Func<T, object>> thisFiled, Expression<Func<object>> mappingFiled, ParameterT parameter)
        {
            if (parameter == null)
            {
                return new List<T>();
            }
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
            var ids = list.Where(it => it != null).Select(it => it.GetType().GetProperty(pkName).GetValue(it)).Distinct().ToArray();
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
                this.Context.Utilities.PageEach(ids, 200, pageIds =>
                {
                    result.AddRange(this.Clone().In(thisFiled, pageIds).ToList());
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
        public List<T> SetContext<ParameterT>(Expression<Func<T, object>> thisFiled1, Expression<Func<object>> mappingFiled1,
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
                result = this.Clone().Where(conditionals, true).ToList();
                queryableContext.TempChildLists[key] = result;
            }
            List<object> listObj = result.Select(it => (object)it).ToList();
            object obj = (object)parameter;
            var newResult = fieldsHelper.GetSetList(obj, listObj, mappings).Select(it => (T)it).ToList();
            return newResult;
        }

        public virtual void ForEach(Action<T> action, int singleMaxReads = 300, System.Threading.CancellationTokenSource cancellationTokenSource = null)
        {
            Check.Exception(this.QueryBuilder.Skip > 0 || this.QueryBuilder.Take > 0, ErrorMessage.GetThrowMessage("no support Skip take, use PageForEach", "不支持Skip Take,请使用 Queryale.PageForEach"));
            var totalNumber = 0;
            var totalPage = 1;
            for (int i = 1; i <= totalPage; i++)
            {
                if (cancellationTokenSource?.IsCancellationRequested == true) return;
                var queryable = this.Clone();
                var page =
                    totalPage == 1 ?
                    queryable.ToPageList(i, singleMaxReads, ref totalNumber, ref totalPage) :
                    queryable.ToPageList(i, singleMaxReads);
                foreach (var item in page)
                {
                    if (cancellationTokenSource?.IsCancellationRequested == true) return;
                    action.Invoke(item);
                }
            }
        }

        public virtual void ForEachByPage(Action<T> action, int pageIndex, int pageSize, ref int totalNumber, int singleMaxReads = 300, System.Threading.CancellationTokenSource cancellationTokenSource = null)
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
                        foreach (var item in this.Clone().Skip(Skip).Take(singleMaxReads).ToList())
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
            totalNumber = count;
        }

        public List<T> ToOffsetPage(int pageIndex, int pageSize)
        {
            if (this.Context.CurrentConnectionConfig.DbType != DbType.SqlServer)
            {
                this.QueryBuilder.Offset = "true";
                return this.ToPageList(pageIndex, pageSize);
            }
            else
            {
                _ToOffsetPage(pageIndex, pageSize);
                return this.ToList();
            }
        }
        public List<T> ToOffsetPage(int pageIndex, int pageSize, ref int totalNumber)
        {
            if (this.Context.CurrentConnectionConfig.DbType != DbType.SqlServer)
            {
                this.QueryBuilder.Offset = "true";
                return this.ToPageList(pageIndex, pageSize, ref totalNumber);
            }
            else
            {
                totalNumber = this.Clone().Count();
                _ToOffsetPage(pageIndex, pageSize);
                return this.Clone().ToList();
            }
        }
        public Task<List<T>> ToOffsetPageAsync(int pageIndex, int pageSize)
        {
            if (this.Context.CurrentConnectionConfig.DbType != DbType.SqlServer)
            {
                this.QueryBuilder.Offset = "true";
                return this.ToPageListAsync(pageIndex, pageSize);
            }
            else
            {
                _ToOffsetPage(pageIndex, pageSize);
                return this.ToListAsync();
            }
        }

        public virtual List<T> ToPageList(int pageIndex, int pageSize)
        {
            pageIndex = _PageList(pageIndex, pageSize);
            return ToList();
        }
        public virtual List<TResult> ToPageList<TResult>(int pageIndex, int pageSize, ref int totalNumber, Expression<Func<T, TResult>> expression)
        {
            if (this.QueryBuilder.Includes != null && this.QueryBuilder.Includes.Count > 0)
            {
                if (pageIndex == 0)
                    pageIndex = 1;
                var list = this.Clone().Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList(expression);
                var countQueryable = this.Clone();
                countQueryable.QueryBuilder.Includes = null;
                totalNumber = countQueryable.Count();
                return list;
            }
            else
            {
                var list = this.Select(expression).ToPageList(pageIndex, pageSize, ref totalNumber).ToList();
                return list;
            }
        }
        public virtual List<T> ToPageList(int pageIndex, int pageSize, ref int totalNumber)
        {
            var oldMapping = this.Context.MappingTables;
            var countQueryable = this.Clone();
            if (countQueryable.QueryBuilder.Offset == "true")
            {
                countQueryable.QueryBuilder.Offset = null;
            }
            totalNumber = countQueryable.Count();
            this.Context.MappingTables = oldMapping;
            return this.Clone().ToPageList(pageIndex, pageSize);
        }
        public virtual List<T> ToPageList(int pageIndex, int pageSize, ref int totalNumber, ref int totalPage)
        {
            var result = ToPageList(pageIndex, pageSize, ref totalNumber);
            totalPage = (totalNumber + pageSize - 1) / pageSize;
            return result;
        }
        public virtual string ToSqlString()
        {
            var sqlObj = this.Clone().ToSql();
            var result = sqlObj.Key;
            if (result == null) return null;
            result = UtilMethods.GetSqlString(this.Context.CurrentConnectionConfig, sqlObj);
            return result;
        }


        public virtual KeyValuePair<string, List<SugarParameter>> ToSql()
        {
            if (!QueryBuilder.IsClone)
            {
                var newQueryable = this.Clone();
                newQueryable.QueryBuilder.IsClone = true;
                return newQueryable.ToSql();
            }
            else
            {
                return _ToSql();
            }
        }
        public string ToClassString(string className)
        {
            List<DbColumnInfo> columns = new List<DbColumnInfo>();
            var properties = typeof(T).GetProperties();
            foreach (var item in properties)
            {
                columns.Add(new DbColumnInfo()
                {
                    DbColumnName = item.Name,
                    PropertyName = UtilMethods.GetUnderType(item.PropertyType).Name,
                    PropertyType = UtilMethods.GetUnderType(item.PropertyType)
                });
            }
            var result = ((this.Context.DbFirst) as DbFirstProvider).GetClassString(columns, ref className);
            return result;
        }

        public int IntoTable<TableEntityType>() 
        {
            return IntoTable(typeof(TableEntityType));
        }
        public int IntoTable<TableEntityType>(string TableName)
        {
            return IntoTable(typeof(TableEntityType), TableName);
        }
        public int IntoTable(Type TableEntityType) 
        {
            var entityInfo = this.Context.EntityMaintenance.GetEntityInfo(TableEntityType);
            var name = this.SqlBuilder.GetTranslationTableName(entityInfo.DbTableName);
            return IntoTable(TableEntityType, name);
        }
        public int IntoTable(Type TableEntityType,string TableName)
        {
            KeyValuePair<string, List<SugarParameter>> sqlInfo;
            string sql;
            OutIntoTableSql(TableName, out sqlInfo, out sql, TableEntityType);
            return this.Context.Ado.ExecuteCommand(sql, sqlInfo.Value);
        }
  }
}
