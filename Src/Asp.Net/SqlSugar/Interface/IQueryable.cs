﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SqlSugar
{
    public partial interface ISugarQueryable<T>
    {
        SqlSugarProvider Context { get; set; }
        ISqlBuilder SqlBuilder { get; set; }
        QueryBuilder QueryBuilder { get; set; }
        bool IsCache { get; set; }
        int CacheTime { get; set; }
        ISugarQueryable<T> Clone();
        ISugarQueryable<T> Hints(string hints);
        ISugarQueryable<T> AS<T2>(string tableName);
        ISugarQueryable<T> AS(string tableName);
        ISugarQueryable<T> AsWithAttr();
        ISugarQueryable<T> AsType(Type tableNameType);
        ISugarQueryable<Type> Cast<Type>();
        ISugarQueryable<T> With(string withString);
        //ISugarQueryable<T> CrossQueryWithAttr();
        ISugarQueryable<T> CrossQuery<Type>(string configId);
        ISugarQueryable<T> CrossQuery(Type type ,string configId);
        ISugarQueryable<T> IncludeLeftJoin(Expression<Func<T, object>> leftObjectExp);
        ISugarQueryable<T> IncludeInnerJoin(Expression<Func<T, object>> innerObjectExp);
        ISugarQueryable<T> IncludeRightJoin(Expression<Func<T, object>> rightObjectExp);
        ISugarQueryable<T> IncludeFullJoin(Expression<Func<T, object>> fullObjectExp);
        ISugarQueryable<T, T2> LeftJoin<T2>(ISugarQueryable<T2> joinQueryable, Expression<Func<T, T2, bool>> joinExpression);
        ISugarQueryable<T, T2> LeftJoinIF<T2>(bool isJoin,ISugarQueryable<T2> joinQueryable, Expression<Func<T, T2, bool>> joinExpression);
        ISugarQueryable<T, T2> InnerJoinIF<T2>(bool isJoin, ISugarQueryable<T2> joinQueryable, Expression<Func<T, T2, bool>> joinExpression);
        ISugarQueryable<T, T2> InnerJoin<T2>(ISugarQueryable<T2> joinQueryable, Expression<Func<T, T2, bool>> joinExpression);
        ISugarQueryable<T, T2> RightJoin<T2>(ISugarQueryable<T2> joinQueryable, Expression<Func<T, T2, bool>> joinExpression);
        ISugarQueryable<T, T2> FullJoin<T2>(ISugarQueryable<T2> joinQueryable, Expression<Func<T, T2, bool>> joinExpression);
        ISugarQueryable<T,T2> LeftJoin<T2>(Expression<Func<T,T2,bool>> joinExpression);
        ISugarQueryable<T, T2> LeftJoinIF<T2>(bool isLeftJoin,Expression<Func<T, T2, bool>> joinExpression);
        ISugarQueryable<T, T2> InnerJoinIF<T2>(bool isJoin, Expression<Func<T, T2, bool>> joinExpression);
        ISugarQueryable<T, T2> LeftJoin<T2>(Expression<Func<T, T2, bool>> joinExpression,string tableName);
        ISugarQueryable<T, T2> FullJoin<T2>(Expression<Func<T, T2, bool>> joinExpression);
        ISugarQueryable<T, T2> FullJoin<T2>(Expression<Func<T, T2, bool>> joinExpression,string tableName);
        ISugarQueryable<T, T2> InnerJoin<T2>(Expression<Func<T, T2, bool>> joinExpression);
        ISugarQueryable<T, T2> InnerJoin<T2>(Expression<Func<T, T2, bool>> joinExpression,string tableName);
        ISugarQueryable<T, T2> RightJoin<T2>(Expression<Func<T, T2, bool>> joinExpression);
        ISugarQueryable<T, T2> RightJoin<T2>(Expression<Func<T, T2, bool>> joinExpression,string tableName);
        ISugarQueryable<T> Filter(string FilterName, bool isDisabledGobalFilter = false);
        ISugarQueryable<T> ClearFilter(params Type[] types);
        ISugarQueryable<T> ClearFilter<FilterType1>();
        ISugarQueryable<T> ClearFilter<FilterType1,FilterType2>();
        ISugarQueryable<T> ClearFilter<FilterType1,FilterType2, FilterType3>();
        ISugarQueryable<T> ClearFilter();
        ISugarQueryable<T> Filter(Type type);
        ISugarQueryable<T> Mapper(Action<T> mapperAction);
        ISugarQueryable<T> Mapper<AType, BType, MappingType>(Expression<Func<MappingType, ManyToMany>> expression);
        ISugarQueryable<T> Mapper(Action<T, MapperCache<T>> mapperAction);
        ISugarQueryable<T> Mapper<TObject>(Expression<Func<T, TObject>> mapperObject, Expression<Func<T, object>> mainField, Expression<Func<T, object>> childField);
        ISugarQueryable<T> Mapper<TObject>(Expression<Func<T, List<TObject>>> mapperObject, Expression<Func<T, object>> mainField, Expression<Func<T, object>> childField);
        ISugarQueryable<T> Mapper<TObject>(Expression<Func<T, TObject>> mapperObject, Expression<Func<T, object>> mapperField);
        ISugarQueryable<T> Mapper<TObject>(Expression<Func<T, List<TObject>>> mapperObject, Expression<Func<T, object>> mapperField);
        ISugarQueryable<T> AddParameters(object parameters);
        ISugarQueryable<T> AddParameters(SugarParameter[] parameters);
        ISugarQueryable<T> AddParameters(List<SugarParameter> parameters);
        ISugarQueryable<T> AddJoinInfo(string tableName, string shortName, string joinWhere, JoinType type = JoinType.Left);
        ISugarQueryable<T> AddJoinInfo(Type JoinEntityType, string shortName, string joinWhere, JoinType type = JoinType.Left);
        ISugarQueryable<T> AddJoinInfo(Type JoinEntityType, Dictionary<string, Type> keyIsShortName_ValueIsType_Dictionary, FormattableString onExpString, JoinType type = JoinType.Left);
        /// <summary>
        /// if a property that is not empty is a condition
        /// </summary>
        /// <param name="whereClass"></param>
        /// <returns></returns>
        ISugarQueryable<T> WhereClass<ClassType>(ClassType whereClass,bool ignoreDefaultValue=false)where ClassType: class,new();
        /// <summary>
        ///  if a property that is not empty is a condition
        /// </summary>
        /// <param name="whereClassTypes"></param>
        /// <returns></returns>
        ISugarQueryable<T> WhereClass<ClassType>(List<ClassType> whereClassList,bool ignoreDefaultValue = false) where ClassType : class, new();
        ISugarQueryable<T> WhereClassByPrimaryKey(List<T> list);
        ISugarQueryable<T> WhereClassByWhereColumns(List<T> list,string[] whereColumns);
        ISugarQueryable<T> WhereClassByPrimaryKey(T data) ;
        ISugarQueryable<T> WhereColumns(List<Dictionary<string, object>> columns);
        ISugarQueryable<T> WhereColumns(Dictionary<string, object> columns, bool ignoreDefaultValue);
        ISugarQueryable<T> WhereColumns(Dictionary<string, object> columns);
        ISugarQueryable<T> TranLock(DbLockType? LockType = DbLockType.Wait);
        ISugarQueryable<T> Where(Expression<Func<T, bool>> expression);
        ISugarQueryable<T> Where(string expShortName, FormattableString expressionString);
        ISugarQueryable<T> Where(Dictionary<string,Type> keyIsShortName_ValueIsType_Dictionary, FormattableString expressionString);
        ISugarQueryable<T> Where(string whereString, object parameters = null);
        ISugarQueryable<T> Where(IFuncModel funcModel);
        ISugarQueryable<T> Where(List<IConditionalModel> conditionalModels);
        ISugarQueryable<T> Where(List<IConditionalModel> conditionalModels,bool isWrap);
        ISugarQueryable<T> Where(string fieldName, string conditionalType, object fieldValue);
        ISugarQueryable<T> Having(Expression<Func<T, bool>> expression);
        ISugarQueryable<T> HavingIF(bool isHaving,Expression<Func<T, bool>> expression);
        ISugarQueryable<T> Having(string whereString, object parameters = null);

        ISugarQueryable<T> WhereIF(bool isWhere, Expression<Func<T, bool>> expression);
        ISugarQueryable<T> WhereIF(bool isWhere, string whereString, object parameters = null);

        T InSingle(object pkValue);
        Task<T> InSingleAsync(object pkValue);
        ISugarQueryable<T> In<TParamter>(params TParamter[] pkValues);
        ISugarQueryable<T> InIF<TParamter>(bool isIn, string fieldName, params TParamter[] pkValues);
        ISugarQueryable<T> InIF<TParamter>(bool isIn,params TParamter[] pkValues);
        ISugarQueryable<T> In<FieldType>(string InFieldName, params FieldType[] inValues);
        ISugarQueryable<T> In<FieldType>(Expression<Func<T, object>> expression, params FieldType[] inValues);
        ISugarQueryable<T> In<TParamter>(List<TParamter> pkValues);
        ISugarQueryable<T> In<FieldType>(string InFieldName, List<FieldType> inValues);
        ISugarQueryable<T> In<FieldType>(Expression<Func<T, object>> expression, List<FieldType> inValues);
        ISugarQueryable<T> In<FieldType>(Expression<Func<T, object>> expression, ISugarQueryable<FieldType> childQueryExpression);
        ISugarQueryable<T> InIF<FieldType>(bool isWhere,Expression<Func<T, object>> expression, ISugarQueryable<FieldType> childQueryExpression);

        ISugarQueryable<T> OrderBy(string orderByFields);
        ISugarQueryable<T> OrderByPropertyName(string orderPropertyName,OrderByType? orderByType=null);
        ISugarQueryable<T> OrderByPropertyNameIF(bool isOrderBy,string orderPropertyName, OrderByType? orderByType = null);
        ISugarQueryable<T> OrderBy(Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T> OrderByDescending(Expression<Func<T, object>> expression);
        ISugarQueryable<T> OrderByIF(bool isOrderBy, string orderByFields);
        ISugarQueryable<T> OrderByIF(bool isOrderBy, Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T> SampleBy(int timeNumber, SampleByUnit timeType);
        ISugarQueryable<T> SampleBy(int timeNumber, string timeType);


        ISugarQueryable<T> GroupBy(Expression<Func<T, object>> expression);
        ISugarQueryable<T> GroupBy(string groupFileds);
        ISugarQueryable<T> GroupByIF(bool isGroupBy, Expression<Func<T, object>> expression);

        ISugarQueryable<T> GroupByIF(bool isGroupBy, string groupFields);

        ISugarQueryable<T> PartitionBy(Expression<Func<T, object>> expression);
        ISugarQueryable<T> PartitionBy(string groupFileds);

        ISugarQueryable<T> Skip(int index);
        ISugarQueryable<T> Take(int num);
        ISugarQueryable<T> Distinct();

        T Single();
        Task<T> SingleAsync();
        T Single(Expression<Func<T, bool>> expression);
        Task<T> SingleAsync(Expression<Func<T, bool>> expression);

        T First();
        Task<T> FirstAsync();
        Task<T> FirstAsync(CancellationToken token);
        T First(Expression<Func<T, bool>> expression);
        Task<T> FirstAsync(Expression<Func<T, bool>> expression);
        Task<T> FirstAsync(Expression<Func<T, bool>> expression, CancellationToken token);

        bool Any(Expression<Func<T, bool>> expression);
        Task<bool> AnyAsync(Expression<Func<T, bool>> expression);
        Task<bool> AnyAsync(Expression<Func<T, bool>> expression, CancellationToken token);
        bool Any();
        Task<bool> AnyAsync();
        ISugarQueryable<TResult> Select<TResult>(string expShortName, FormattableString expSelect, Type resultType);
        ISugarQueryable<TResult> Select<TResult>(Dictionary<string, Type> keyIsShortName_ValueIsType_Dictionary, FormattableString expSelect, Type resultType);
        ISugarQueryable<TResult> Select<TResult>(string expShortName, FormattableString expSelect,Type EntityType, Type resultType);
        ISugarQueryable<T> Select(string expShortName, FormattableString expSelect, Type resultType);
        ISugarQueryable<TResult> Select<TResult>(Expression expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, TResult>> expression,bool isAutoFill);
        ISugarQueryable<TResult> Select<TResult>();
        ISugarQueryable<TResult> Select<TResult>(string select);
        ISugarQueryable<T> Select(string select);
        ISugarQueryable<TResult> SelectMergeTable<TResult>(Expression<Func<T, TResult>> expression);
        ISugarQueryable<T> MergeTable();
        void ForEachDataReader(Action<T> action);
        Task ForEachDataReaderAsync(Action<T> action);
        void ForEach(Action<T> action, int singleMaxReads = 300, System.Threading.CancellationTokenSource cancellationTokenSource = null);
        Task ForEachAsync(Action<T> action, int singleMaxReads = 300, System.Threading.CancellationTokenSource cancellationTokenSource = null);
        void ForEachByPage(Action<T> action, int pageIndex, int pageSize, ref int totalNumber, int singleMaxReads = 300, System.Threading.CancellationTokenSource cancellationTokenSource = null);
        Task ForEachByPageAsync(Action<T> action, int pageIndex, int pageSize, RefAsync<int> totalNumber, int singleMaxReads = 300, System.Threading.CancellationTokenSource cancellationTokenSource = null);
        int Count();
        Task<int> CountAsync();
        Task<int> CountAsync(CancellationToken token);
        int Count(Expression<Func<T, bool>> expression);
        Task<int> CountAsync(Expression<Func<T, bool>> expression);
        Task<int> CountAsync(Expression<Func<T, bool>> expression,CancellationToken token);
        TResult Max<TResult>(string maxField);
        Task<TResult> MaxAsync<TResult>(string maxField);
        Task<TResult> MaxAsync<TResult>(string maxField,CancellationToken token);
        TResult Max<TResult>(Expression<Func<T, TResult>> expression);
        Task<TResult> MaxAsync<TResult>(Expression<Func<T, TResult>> expression);
        Task<TResult> MaxAsync<TResult>(Expression<Func<T, TResult>> expression,CancellationToken token);
        TResult Min<TResult>(string minField);
        Task<TResult> MinAsync<TResult>(string minField);
        TResult Min<TResult>(Expression<Func<T, TResult>> expression);
        Task<TResult> MinAsync<TResult>(Expression<Func<T, TResult>> expression);
        TResult Sum<TResult>(string sumField);
        Task<TResult> SumAsync<TResult>(string sumField);
        TResult Sum<TResult>(Expression<Func<T, TResult>> expression);
        Task<TResult> SumAsync<TResult>(Expression<Func<T, TResult>> expression);
        TResult Avg<TResult>(string avgField);
        Task<TResult> AvgAsync<TResult>(string avgField);
        TResult Avg<TResult>(Expression<Func<T, TResult>> expression);
        Task<TResult> AvgAsync<TResult>(Expression<Func<T, TResult>> expression);
        List<TResult> ToList<TResult>(Expression<Func<T, TResult>> expression);
        Task<List<TResult>> ToListAsync<TResult>(Expression<Func<T, TResult>> expression);
        List<T> ToList();

        int IntoTable<TableEntityType>();
        int IntoTable(Type TableEntityType);
        int IntoTable<TableEntityType>(string tableName);
        int IntoTable(Type TableEntityType,string tableName);

        Task<int> IntoTableAsync<TableEntityType>(CancellationToken cancellationToken = default);
        Task<int> IntoTableAsync(Type TableEntityType, CancellationToken cancellationToken = default);
        Task<int> IntoTableAsync<TableEntityType>(string tableName, CancellationToken cancellationToken = default);
        Task<int> IntoTableAsync(Type TableEntityType, string tableName, CancellationToken cancellationToken = default);

        //bool IntoTable(Type TableEntityType, params string[] columnNameList);
        //bool IntoTable<TableEntityType>(params string[] columnNameList);
        List<T> SetContext<ParameterT>(Expression<Func<T, bool>> whereExpression, ParameterT parameter);
        List<T> SetContext<ParameterT>(Expression<Func<T,object>> thisFiled, Expression<Func<object>> mappingFiled, ParameterT parameter);
        List<T> SetContext<ParameterT>(Expression<Func<T, object>> thisFiled1, Expression<Func<object>> mappingFiled1, Expression<Func<T, object>> thisFiled2, Expression<Func<object>> mappingFiled2, ParameterT parameter);
        Task <List<T>> SetContextAsync<ParameterT>(Expression<Func<T, object>> thisFiled, Expression<Func<object>> mappingFiled, ParameterT parameter);
        Task<List<T>> SetContextAsync<ParameterT>(Expression<Func<T, object>> thisFiled1, Expression<Func<object>> mappingFiled1, Expression<Func<T, object>> thisFiled2, Expression<Func<object>> mappingFiled2, ParameterT parameter);
        Dictionary<string, ValueType> ToDictionary<ValueType>(Expression<Func<T, object>> key, Expression<Func<T, object>> value);
        Dictionary<string, object> ToDictionary(Expression<Func<T, object>> key, Expression<Func<T, object>> value);
        Task<Dictionary<string, object>> ToDictionaryAsync(Expression<Func<T, object>> key, Expression<Func<T, object>> value);
        Task<Dictionary<string, ValueType>> ToDictionaryAsync<ValueType>(Expression<Func<T, object>> key, Expression<Func<T, object>> value);
        List<Dictionary<string, object>> ToDictionaryList();
        Task<List<Dictionary<string, object>>> ToDictionaryListAsync();

        T[] ToArray();
        Task<T[]> ToArrayAsync();
        Task<List<T>> ToListAsync();
        Task<List<T>> ToListAsync(CancellationToken token);

        string ToJson();
        Task<string> ToJsonAsync();
        string ToJsonPage(int pageIndex, int pageSize);
        Task<string> ToJsonPageAsync(int pageIndex, int pageSize);
        string ToJsonPage(int pageIndex, int pageSize, ref int totalNumber);
        Task<string> ToJsonPageAsync(int pageIndex, int pageSize, RefAsync<int> totalNumber);
        KeyValuePair<string, List<SugarParameter>> ToSql();
        string ToSqlString();
        List<T> ToChildList(Expression<Func<T, object>> parentIdExpression, object primaryKeyValue, bool isContainOneself = true);
        List<T> ToChildList(Expression<Func<T, object>> parentIdExpression, object[] primaryKeyValues, bool isContainOneself = true);
        Task<List<T>> ToChildListAsync(Expression<Func<T, object>> parentIdExpression, object primaryKeyValue, bool isContainOneself = true);
        Task<List<T>> ToChildListAsync(Expression<Func<T, object>> parentIdExpression, object[] primaryKeyValues, bool isContainOneself = true);
        List<T> ToParentList(Expression<Func<T, object>> parentIdExpression, object primaryKeyValue);
        List<T> ToParentList(Expression<Func<T, object>> parentIdExpression, object primaryKeyValue, Expression<Func<T, bool>> parentWhereExpression);

        Task<List<T>> ToParentListAsync(Expression<Func<T, object>> parentIdExpression, object primaryKeyValue);
        Task<List<T>> ToParentListAsync(Expression<Func<T, object>> parentIdExpression, object primaryKeyValue, Expression<Func<T, bool>> parentWhereExpression);
        List<T> ToTree(string childPropertyName, string parentIdPropertyName, object rootValue, string primaryKeyPropertyName); 
        List<T> ToTree(Expression<Func<T,IEnumerable<object>>> childListExpression, Expression<Func<T,object>> parentIdExpression,object rootValue);
        List<T> ToTree(Expression<Func<T, IEnumerable<object>>> childListExpression, Expression<Func<T, object>> parentIdExpression, object rootValue, Expression<Func<T, object>> primaryKeyExpression);
        Task<List<T>> ToTreeAsync(string childPropertyName, string parentIdPropertyName, object rootValue, string primaryKeyPropertyName);
        Task<List<T>> ToTreeAsync(Expression<Func<T, IEnumerable<object>>> childListExpression, Expression<Func<T, object>> parentIdExpression, object rootValue);
        Task<List<T>> ToTreeAsync(Expression<Func<T, IEnumerable<object>>> childListExpression, Expression<Func<T, object>> parentIdExpression, object rootValue, Expression<Func<T, object>> primaryKeyExpression);
        List<T> ToTree(Expression<Func<T, IEnumerable<object>>> childListExpression, Expression<Func<T, object>> parentIdExpression, object rootValue, object[] childIds);
        List<T> ToTree(Expression<Func<T, IEnumerable<object>>> childListExpression, Expression<Func<T, object>> parentIdExpression, object rootValue, object[] childIds, Expression<Func<T, object>> primaryKeyExpression);
        Task<List<T>> ToTreeAsync(Expression<Func<T, IEnumerable<object>>> childListExpression, Expression<Func<T, object>> parentIdExpression, object rootValue, object[] childIds);
        Task<List<T>> ToTreeAsync(Expression<Func<T, IEnumerable<object>>> childListExpression, Expression<Func<T, object>> parentIdExpression, object rootValue, object[] childIds, Expression<Func<T, object>> primaryKeyExpression);
        DataTable ToDataTable();
        DataTable ToDataTableByEntity();
        Task<DataTable> ToDataTableAsync();
        Task<DataTable> ToDataTableByEntityAsync();
        DataTable ToDataTablePage(int pageNumber, int pageSize);
        Task<DataTable> ToDataTablePageAsync(int pageNumber, int pageSize);
        DataTable ToDataTablePage(int pageNumber, int pageSize, ref int totalNumber);
        DataTable ToDataTableByEntityPage(int pageNumber, int pageSize, ref int totalNumber);
        DataTable ToDataTablePage(int pageNumber, int pageSize, ref int totalNumber,ref int totalPage);
        Task<DataTable> ToDataTablePageAsync(int pageNumber, int pageSize, RefAsync<int> totalNumber);
        Task<DataTable> ToDataTableByEntityPageAsync(int pageNumber, int pageSize, RefAsync<int> totalNumber);

        List<T> ToOffsetPage(int pageNumber, int pageSize);
        List<T> ToOffsetPage(int pageNumber, int pageSize,ref int totalNumber);
        List<T> ToOffsetPage(int pageNumber, int pageSize, ref int totalNumber,ref int totalPage);
        Task<List<T>> ToOffsetPageAsync(int pageNumber, int pageSize);
        Task<List<T>> ToOffsetPageAsync(int pageNumber, int pageSize, RefAsync<int> totalNumber);
        Task<List<T>> ToOffsetPageAsync(int pageNumber, int pageSize, RefAsync<int> totalNumber, RefAsync<int> totalPage);
        Task<List<T>> ToOffsetPageAsync(int pageNumber, int pageSize, RefAsync<int> totalNumber, CancellationToken token);
        Task<List<T>> ToOffsetPageAsync(int pageNumber, int pageSize, RefAsync<int> totalNumber, RefAsync<int> totalPage, CancellationToken token);
        List<T> ToPageList(int pageNumber, int pageSize);
        Task<List<T>> ToPageListAsync(int pageNumber, int pageSize);
        Task<List<T>> ToPageListAsync(int pageNumber, int pageSize,CancellationToken token);
        List<T> ToPageList(int pageNumber, int pageSize, ref int totalNumber);
        List<TResult> ToPageList<TResult>(int pageNumber, int pageSize, ref int totalNumber, Expression<Func<T, TResult>> expression);
        List<T> ToPageList(int pageNumber, int pageSize, ref int totalNumber,ref int totalPage);
        Task<List<T>> ToPageListAsync(int pageNumber, int pageSize, RefAsync<int> totalNumber);
        Task<List<T>> ToPageListAsync(int pageNumber, int pageSize, RefAsync<int> totalNumber,CancellationToken token);
        Task<List<TResult>> ToPageListAsync<TResult>(int pageNumber, int pageSize, RefAsync<int> totalNumber, Expression<Func<T, TResult>> expression);
        Task<List<T>> ToPageListAsync(int pageNumber, int pageSize, RefAsync<int> totalNumber, RefAsync<int> totalPage);
        Task<List<T>> ToPageListAsync(int pageNumber, int pageSize, RefAsync<int> totalNumber, RefAsync<int> totalPage,CancellationToken token);
        ISugarQueryable<T> WithCache(string cacheKey,int cacheDurationInSeconds = int.MaxValue);
        ISugarQueryable<T> WithCache(int cacheDurationInSeconds = int.MaxValue);
        ISugarQueryable<T> WithCacheIF(bool isCache, int cacheDurationInSeconds = int.MaxValue);
        string ToClassString(string className);
        void Clear();
        void AddQueue();
        ISugarQueryable<T> IgnoreColumns(Expression<Func<T, object>> columns);
        ISugarQueryable<T> IgnoreColumns(params string[] columns);

        #region 内存行转列

        #region 同步
        DataTable ToPivotTable<TColumn, TRow, TData>(Func<T, TColumn> columnSelector, Expression<Func<T, TRow>> rowSelector, Func<IEnumerable<T>, TData> dataSelector);
        List<dynamic> ToPivotList<TColumn, TRow, TData>(Func<T, TColumn> columnSelector, Expression<Func<T, TRow>> rowSelector, Func<IEnumerable<T>, TData> dataSelector);
        IEnumerable<dynamic> ToPivotEnumerable<TColumn, TRow, TData>(Func<T, TColumn> columnSelector, Expression<Func<T, TRow>> rowSelector, Func<IEnumerable<T>, TData> dataSelector);
        string ToPivotJson<TColumn, TRow, TData>(Func<T, TColumn> columnSelector, Expression<Func<T, TRow>> rowSelector, Func<IEnumerable<T>, TData> dataSelector);
        #endregion

        #region 异步
        Task<DataTable> ToPivotTableAsync<TColumn, TRow, TData>(Func<T, TColumn> columnSelector, Expression<Func<T, TRow>> rowSelector, Func<IEnumerable<T>, TData> dataSelector);
        Task<List<dynamic>> ToPivotListAsync<TColumn, TRow, TData>(Func<T, TColumn> columnSelector, Expression<Func<T, TRow>> rowSelector, Func<IEnumerable<T>, TData> dataSelector);
        Task<IEnumerable<dynamic>> ToPivotEnumerableAsync<TColumn, TRow, TData>(Func<T, TColumn> columnSelector, Expression<Func<T, TRow>> rowSelector, Func<IEnumerable<T>, TData> dataSelector);
        Task<string> ToPivotJsonAsync<TColumn, TRow, TData>(Func<T, TColumn> columnSelector, Expression<Func<T, TRow>> rowSelector, Func<IEnumerable<T>, TData> dataSelector);
        #endregion

        #endregion

        ISugarQueryable<T> SplitTable(Func<List<SplitTableInfo>,IEnumerable<SplitTableInfo>> getTableNamesFunc);
        ISugarQueryable<T> SplitTable(DateTime beginTime,DateTime endTime);
        ISugarQueryable<T> SplitTable();
    }
    public partial interface ISugarQueryable<T, T2> : ISugarQueryable<T>
    {
        new ISugarQueryable<T,T2> Hints(string hints);
        new ISugarQueryable<T,T2> SampleBy(int timeNumber, SampleByUnit timeType);
        new ISugarQueryable<T,T2> SampleBy(int timeNumber, string timeType);
        ISugarQueryable<TResult> SelectMergeTable<TResult>(Expression<Func<T,T2, TResult>> expression);
        ISugarQueryable<T, T2,T3> LeftJoinIF<T3>(bool isLeftJoin, Expression<Func<T, T2,T3, bool>> joinExpression);
        ISugarQueryable<T, T2,T3> InnerJoinIF<T3>(bool isJoin, Expression<Func<T, T2,T3, bool>> joinExpression);
        ISugarQueryable<T, T2,T3> LeftJoin<T3>(ISugarQueryable<T3> joinQueryable, Expression<Func<T, T2,T3, bool>> joinExpression);
        ISugarQueryable<T, T2, T3> LeftJoinIF<T3>(bool isJoin,ISugarQueryable<T3> joinQueryable, Expression<Func<T, T2, T3, bool>> joinExpression);
        ISugarQueryable<T, T2, T3> InnerJoinIF<T3>(bool isJoin, ISugarQueryable<T3> joinQueryable, Expression<Func<T, T2, T3, bool>> joinExpression);
        ISugarQueryable<T, T2,T3> InnerJoin<T3>(ISugarQueryable<T3> joinQueryable, Expression<Func<T, T2,T3, bool>> joinExpression);
        ISugarQueryable<T, T2,T3> RightJoin<T3>(ISugarQueryable<T3> joinQueryable, Expression<Func<T, T2,T3, bool>> joinExpression);
        ISugarQueryable<T, T2, T3> FullJoin<T3>(ISugarQueryable<T3> joinQueryable, Expression<Func<T, T2, T3, bool>> joinExpression);
        ISugarQueryable<T, T2,T3> LeftJoin<T3>(Expression<Func<T,T2,T3,bool>> joinExpression);
        ISugarQueryable<T, T2, T3> FullJoin<T3>(Expression<Func<T, T2, T3, bool>> joinExpression);
        ISugarQueryable<T, T2, T3> InnerJoin<T3>(Expression<Func<T, T2, T3, bool>> joinExpression);
        ISugarQueryable<T, T2, T3> RightJoin<T3>(Expression<Func<T, T2, T3, bool>> joinExpression);

        ISugarQueryable<T, T2, T3> LeftJoin<T3>(Expression<Func<T, T2, T3, bool>> joinExpression,string tableName);
        ISugarQueryable<T, T2, T3> FullJoin<T3>(Expression<Func<T, T2, T3, bool>> joinExpression,string tableName);
        ISugarQueryable<T, T2, T3> InnerJoin<T3>(Expression<Func<T, T2, T3, bool>> joinExpression,string tableName);
        ISugarQueryable<T, T2, T3> RightJoin<T3>(Expression<Func<T, T2, T3, bool>> joinExpression,string tableName);
        #region Where
        new ISugarQueryable<T,T2> Where(string expShortName, FormattableString expressionString);
        new ISugarQueryable<T, T2> Where(Expression<Func<T, bool>> expression);
        ISugarQueryable<T, T2> Where(Expression<Func<T, T2, bool>> expression);
        new ISugarQueryable<T, T2> Where(List<IConditionalModel> conditionalModels);
        new ISugarQueryable<T, T2> Where(List<IConditionalModel> conditionalModels, bool isWrap);
        new ISugarQueryable<T,T2> Where(IFuncModel funcModel);

        new ISugarQueryable<T, T2> WhereIF(bool isWhere, Expression<Func<T, bool>> expression);
        ISugarQueryable<T, T2> WhereIF(bool isWhere, Expression<Func<T, T2, bool>> expression);

        new ISugarQueryable<T, T2> Where(string whereString, object whereObj = null);
        new ISugarQueryable<T, T2> WhereIF(bool isWhere, string whereString, object whereObj = null);
        /// <summary>
        /// if a property that is not empty is a condition
        /// </summary>
        /// <param name="whereClass"></param>
        /// <returns></returns>
        new ISugarQueryable<T,T2> WhereClass<ClassType>(ClassType whereClass, bool ignoreDefaultValue = false) where ClassType : class, new();
        /// <summary>
        ///  if a property that is not empty is a condition
        /// </summary>
        /// <param name="whereClassTypes"></param>
        /// <returns></returns>
        new ISugarQueryable<T,T2> WhereClass<ClassType>(List<ClassType> whereClassList, bool ignoreDefaultValue = false) where ClassType : class, new();
        #endregion

        #region Select
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T,T2, TResult>> expression, bool isAutoFill);
        #endregion

        #region OrderBy
        new ISugarQueryable<T,T2> OrderBy(List<OrderByModel> models);
        new ISugarQueryable<T,T2> OrderByPropertyName(string orderPropertyName, OrderByType? orderByType = null);
        new ISugarQueryable<T,T2> OrderByPropertyNameIF(bool isOrderBy, string orderPropertyName, OrderByType? orderByType = null);
        new ISugarQueryable<T,T2> OrderByDescending(Expression<Func<T, object>> expression);
        ISugarQueryable<T, T2> OrderByDescending(Expression<Func<T,T2, object>> expression);
        new ISugarQueryable<T, T2> OrderBy(string orderByFields);
        new ISugarQueryable<T, T2> OrderBy(Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2> OrderBy(Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc);
        new ISugarQueryable<T, T2> OrderByIF(bool isOrderBy, string orderByFields);
        new ISugarQueryable<T, T2> OrderByIF(bool isOrderBy, Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2> OrderByIF(bool isOrderBy, Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc);
        #endregion

        #region GroupBy
        new ISugarQueryable<T, T2> PartitionBy(Expression<Func<T, object>> expression);
        ISugarQueryable<T,T2> PartitionBy(Expression<Func<T,T2, object>> expression);
        new ISugarQueryable<T,T2> PartitionBy(string groupFileds);
        new ISugarQueryable<T, T2> GroupBy(Expression<Func<T, object>> expression);
        ISugarQueryable<T, T2> GroupBy(Expression<Func<T, T2, object>> expression);
        new ISugarQueryable<T, T2> GroupByIF(bool isGroupBy, Expression<Func<T, object>> expression);
        ISugarQueryable<T, T2> GroupByIF(bool isGroupBy, Expression<Func<T,T2, object>> expression);
        new ISugarQueryable<T, T2> GroupByIF(bool isGroupBy, string groupFields);
        new ISugarQueryable<T, T2> Having(Expression<Func<T, bool>> expression);
        ISugarQueryable<T, T2> Having(Expression<Func<T, T2, bool>> expression);
        new ISugarQueryable<T, T2> Having(string whereString, object whereObj = null);
        new ISugarQueryable<T,T2> HavingIF(bool isHaving, Expression<Func<T, bool>> expression);
        ISugarQueryable<T, T2> HavingIF(bool isHaving, Expression<Func<T,T2, bool>> expression);
        #endregion

        #region Aggr
        TResult Max<TResult>(Expression<Func<T, T2, TResult>> expression);
        TResult Min<TResult>(Expression<Func<T, T2, TResult>> expression);
        TResult Sum<TResult>(Expression<Func<T, T2, TResult>> expression);
        TResult Avg<TResult>(Expression<Func<T, T2, TResult>> expression);
        Task<TResult> MaxAsync<TResult>(Expression<Func<T, T2, TResult>> expression);
        Task<TResult> MinAsync<TResult>(Expression<Func<T, T2, TResult>> expression);
        Task<TResult> SumAsync<TResult>(Expression<Func<T, T2, TResult>> expression);
        Task<TResult> AvgAsync<TResult>(Expression<Func<T, T2, TResult>> expression);
        #endregion

        #region In
        new ISugarQueryable<T, T2> In<FieldType>(Expression<Func<T, object>> expression, params FieldType[] inValues);
        new ISugarQueryable<T, T2> In<FieldType>(Expression<Func<T, object>> expression, List<FieldType> inValues);
        new ISugarQueryable<T, T2> In<FieldType>(Expression<Func<T, object>> expression, ISugarQueryable<FieldType> childQueryExpression);

        ISugarQueryable<T, T2> In<FieldType>(Expression<Func<T,T2, object>> expression, params FieldType[] inValues);
        ISugarQueryable<T, T2> In<FieldType>(Expression<Func<T,T2, object>> expression, List<FieldType> inValues);
        ISugarQueryable<T, T2> In<FieldType>(Expression<Func<T,T2, object>> expression, ISugarQueryable<FieldType> childQueryExpression);
        new ISugarQueryable<T,T2> InIF<TParamter>(bool isIn, params TParamter[] pkValues);
        new ISugarQueryable<T,T2> InIF<TParamter>(bool isIn, string fieldName, params TParamter[] pkValues);
        #endregion

        #region Other
        new ISugarQueryable<T,T2> Take(int num);
        new ISugarQueryable<T, T2> Clone();
        new ISugarQueryable<T, T2> AS<AsT>(string tableName);
        new ISugarQueryable<T, T2> AS(string tableName);
        new ISugarQueryable<T,T2> ClearFilter();
        new ISugarQueryable<T,T2> ClearFilter(params Type[] types);
        new ISugarQueryable<T,T2> ClearFilter<FilterType1>();
        new ISugarQueryable<T,T2> ClearFilter<FilterType1, FilterType2>();
        new ISugarQueryable<T,T2> ClearFilter<FilterType1, FilterType2, FilterType3>();
        new ISugarQueryable<T, T2> Filter(string FilterName, bool isDisabledGobalFilter = false);
        new ISugarQueryable<T, T2> AddParameters(object parameters);
        new ISugarQueryable<T, T2> AddParameters(SugarParameter[] parameters);
        new ISugarQueryable<T, T2> AddParameters(List<SugarParameter> parameters);
        new ISugarQueryable<T, T2> AddJoinInfo(string tableName, string shortName, string joinWhere, JoinType type = JoinType.Left);
        new ISugarQueryable<T, T2> With(string withString);
        new ISugarQueryable<T, T2> WithCache(int cacheDurationInSeconds = int.MaxValue);
        new ISugarQueryable<T, T2> WithCacheIF(bool isCache, int cacheDurationInSeconds = int.MaxValue);
        new ISugarQueryable<T,T2> Distinct();
        bool Any(Expression<Func<T,T2, bool>> expression);
        #endregion
    }
    public partial interface ISugarQueryable<T, T2, T3> : ISugarQueryable<T>
    {
        new ISugarQueryable<T, T2,T3> Hints(string hints);
        new ISugarQueryable<T, T2,T3> SampleBy(int timeNumber, SampleByUnit timeType);
        new ISugarQueryable<T, T2,T3> SampleBy(int timeNumber, string timeType);
        ISugarQueryable<TResult> SelectMergeTable<TResult>(Expression<Func<T, T2,T3, TResult>> expression);
        ISugarQueryable<T, T2, T3,T4> LeftJoinIF<T4>(bool isLeftJoin, Expression<Func<T, T2, T3,T4, bool>> joinExpression);
        ISugarQueryable<T, T2, T3,T4> InnerJoinIF<T4>(bool isJoin, Expression<Func<T, T2, T3,T4, bool>> joinExpression);
        ISugarQueryable<T, T2, T3,T4> LeftJoin<T4>(ISugarQueryable<T4> joinQueryable, Expression<Func<T, T2, T3,T4, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4> LeftJoinIF<T4>(bool isJoin,ISugarQueryable<T4> joinQueryable, Expression<Func<T, T2, T3, T4, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4> InnerJoinIF<T4>(bool isJoin, ISugarQueryable<T4> joinQueryable, Expression<Func<T, T2, T3, T4, bool>> joinExpression);
        ISugarQueryable<T, T2, T3,T4> InnerJoin<T4>(ISugarQueryable<T4> joinQueryable, Expression<Func<T, T2, T3,T4, bool>> joinExpression);
        ISugarQueryable<T, T2, T3,T4> RightJoin<T4>(ISugarQueryable<T4> joinQueryable, Expression<Func<T, T2, T3,T4, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4> FullJoin<T4>(ISugarQueryable<T4> joinQueryable, Expression<Func<T, T2, T3, T4, bool>> joinExpression);
        ISugarQueryable<T, T2, T3,T4> LeftJoin<T4>(Expression<Func<T, T2, T3,T4, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4> FullJoin<T4>(Expression<Func<T, T2, T3, T4, bool>> joinExpression);
        ISugarQueryable<T, T2, T3,T4> InnerJoin<T4>(Expression<Func<T, T2, T3,T4, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4> RightJoin<T4>(Expression<Func<T, T2, T3, T4, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4> LeftJoin<T4>(Expression<Func<T, T2, T3, T4, bool>> joinExpression,string tableName);
        ISugarQueryable<T, T2, T3, T4> FullJoin<T4>(Expression<Func<T, T2, T3, T4, bool>> joinExpression,string tableName);
        ISugarQueryable<T, T2, T3, T4> InnerJoin<T4>(Expression<Func<T, T2, T3, T4, bool>> joinExpression,string tableName);
        ISugarQueryable<T, T2, T3, T4> RightJoin<T4>(Expression<Func<T, T2, T3, T4, bool>> joinExpression,string tableName);
        #region Where
        new ISugarQueryable<T, T2,T3> Where(string expShortName, FormattableString expressionString);
        new ISugarQueryable<T, T2, T3> Where(Expression<Func<T, bool>> expression);
        ISugarQueryable<T, T2, T3> Where(Expression<Func<T, T2, bool>> expression);
        ISugarQueryable<T, T2, T3> Where(Expression<Func<T, T2, T3, bool>> expression);
        new ISugarQueryable<T, T2, T3> Where(List<IConditionalModel> conditionalModels);
        new ISugarQueryable<T, T2, T3> Where(List<IConditionalModel> conditionalModels,bool isWrap);
        new ISugarQueryable<T, T2,T3> Where(IFuncModel funcModel);
        new ISugarQueryable<T, T2, T3> WhereIF(bool isWhere, Expression<Func<T, bool>> expression);
        ISugarQueryable<T, T2, T3> WhereIF(bool isWhere, Expression<Func<T, T2, bool>> expression);
        ISugarQueryable<T, T2, T3> WhereIF(bool isWhere, Expression<Func<T, T2, T3, bool>> expression);

        new ISugarQueryable<T, T2, T3> Where(string whereString, object parameters = null);
        new ISugarQueryable<T, T2, T3> WhereIF(bool isWhere, string whereString, object parameters = null);

        /// <summary>
        /// if a property that is not empty is a condition
        /// </summary>
        /// <param name="whereClass"></param>
        /// <returns></returns>
        new ISugarQueryable<T, T2,T3> WhereClass<ClassType>(ClassType whereClass, bool ignoreDefaultValue = false) where ClassType : class, new();
        /// <summary>
        ///  if a property that is not empty is a condition
        /// </summary>
        /// <param name="whereClassTypes"></param>
        /// <returns></returns>
        new ISugarQueryable<T, T2,T3> WhereClass<ClassType>(List<ClassType> whereClassList, bool ignoreDefaultValue = false) where ClassType : class, new();
        #endregion

        #region Select
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2,T3, TResult>> expression, bool isAutoFill);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, TResult>> expression);
        #endregion

        #region OrderBy
        new ISugarQueryable<T, T2,T3> OrderBy(List<OrderByModel> models);
        new ISugarQueryable<T,T2,T3> OrderByPropertyName(string orderPropertyName, OrderByType? orderByType = null);
        new ISugarQueryable<T, T2,T3> OrderByPropertyNameIF(bool isOrderBy, string orderPropertyName, OrderByType? orderByType = null);
        new ISugarQueryable<T, T2,T3> OrderByDescending(Expression<Func<T, object>> expression);
        ISugarQueryable<T, T2,T3> OrderByDescending(Expression<Func<T, T2, object>> expression);
        ISugarQueryable<T, T2, T3> OrderByDescending(Expression<Func<T, T2,T3, object>> expression);
        new ISugarQueryable<T, T2,T3> OrderBy(string orderByFields);
        new ISugarQueryable<T, T2, T3> OrderBy(Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3> OrderBy(Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3> OrderBy(Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc);
        new ISugarQueryable<T, T2, T3> OrderByIF(bool isOrderBy, string orderByFields);
        new ISugarQueryable<T, T2, T3> OrderByIF(bool isOrderBy, Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3> OrderByIF(bool isOrderBy, Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc);
        #endregion

        #region GroupBy
        new ISugarQueryable<T, T2,T3> PartitionBy(Expression<Func<T, object>> expression);
        ISugarQueryable<T, T2,T3> PartitionBy(Expression<Func<T, T2, object>> expression);
        ISugarQueryable<T, T2, T3> PartitionBy(Expression<Func<T, T2,T3, object>> expression);
        new ISugarQueryable<T, T2,T3> PartitionBy(string groupFileds);
        new ISugarQueryable<T, T2, T3> GroupBy(Expression<Func<T, object>> expression);
        ISugarQueryable<T, T2, T3> GroupBy(Expression<Func<T, T2, object>> expression);
        ISugarQueryable<T, T2, T3> GroupBy(Expression<Func<T, T2, T3, object>> expression);
        new ISugarQueryable<T, T2, T3> GroupByIF(bool isGroupBy, Expression<Func<T, object>> expression);
        ISugarQueryable<T, T2, T3> GroupByIF(bool isGroupBy, Expression<Func<T, T2, object>> expression);
        ISugarQueryable<T, T2,T3> GroupByIF(bool isGroupBy, Expression<Func<T, T2,T3, object>> expression);
        new ISugarQueryable<T, T2, T3> GroupByIF(bool isGroupBy, string groupFields);
        new ISugarQueryable<T, T2, T3> Having(Expression<Func<T, bool>> expression);
        ISugarQueryable<T, T2, T3> Having(Expression<Func<T, T2, bool>> expression);
        ISugarQueryable<T, T2, T3> Having(Expression<Func<T, T2, T3, bool>> expression);
        new ISugarQueryable<T, T2, T3> Having(string whereString, object parameters = null);
        new ISugarQueryable<T, T2,T3> HavingIF(bool isHaving, Expression<Func<T, bool>> expression);
        ISugarQueryable<T, T2,T3> HavingIF(bool isHaving, Expression<Func<T, T2, bool>> expression);
        ISugarQueryable<T, T2, T3> HavingIF(bool isHaving, Expression<Func<T, T2,T3, bool>> expression);
        #endregion

        #region Aggr
        TResult Max<TResult>(Expression<Func<T, T2, T3, TResult>> expression);
        TResult Min<TResult>(Expression<Func<T, T2, T3, TResult>> expression);
        TResult Sum<TResult>(Expression<Func<T, T2, T3, TResult>> expression);
        TResult Avg<TResult>(Expression<Func<T, T2, T3, TResult>> expression);
        Task<TResult> MaxAsync<TResult>(Expression<Func<T, T2, T3, TResult>> expression);
        Task<TResult> MinAsync<TResult>(Expression<Func<T, T2, T3, TResult>> expression);
        Task<TResult> SumAsync<TResult>(Expression<Func<T, T2, T3, TResult>> expression);
        Task<TResult> AvgAsync<TResult>(Expression<Func<T, T2, T3, TResult>> expression);
        #endregion

        #region In
        new ISugarQueryable<T, T2, T3> In<FieldType>(Expression<Func<T, object>> expression, params FieldType[] inValues);
        new ISugarQueryable<T, T2, T3> In<FieldType>(Expression<Func<T, object>> expression, List<FieldType> inValues);
        new ISugarQueryable<T, T2, T3> In<FieldType>(Expression<Func<T, object>> expression, ISugarQueryable<FieldType> childQueryExpression);

        ISugarQueryable<T, T2, T3> In<FieldType>(Expression<Func<T,T2, object>> expression, params FieldType[] inValues);
        ISugarQueryable<T, T2, T3> In<FieldType>(Expression<Func<T,T2, object>> expression, List<FieldType> inValues);
        ISugarQueryable<T, T2, T3> In<FieldType>(Expression<Func<T,T2, object>> expression, ISugarQueryable<FieldType> childQueryExpression);

        ISugarQueryable<T, T2, T3> In<FieldType>(Expression<Func<T, T2,T3, object>> expression, params FieldType[] inValues);
        ISugarQueryable<T, T2, T3> In<FieldType>(Expression<Func<T, T2,T3, object>> expression, List<FieldType> inValues);
        ISugarQueryable<T, T2, T3> In<FieldType>(Expression<Func<T, T2,T3, object>> expression, ISugarQueryable<FieldType> childQueryExpression);

        new ISugarQueryable<T, T2,T3> InIF<TParamter>(bool isIn, params TParamter[] pkValues);
        new ISugarQueryable<T, T2,T3> InIF<TParamter>(bool isIn, string fieldName, params TParamter[] pkValues);
        #endregion

        #region Other
        new ISugarQueryable<T, T2,T3> Take(int num);
        new ISugarQueryable<T, T2, T3> Clone();
        new ISugarQueryable<T, T2, T3> AS<AsT>(string tableName);
        new ISugarQueryable<T, T2, T3> AS(string tableName);
        new ISugarQueryable<T, T2,T3> ClearFilter();
        new ISugarQueryable<T, T2, T3> Filter(string FilterName, bool isDisabledGobalFilter = false);
        new ISugarQueryable<T, T2,T3> ClearFilter(params Type[] types);
        new ISugarQueryable<T, T2, T3> ClearFilter<FilterType1>();
        new ISugarQueryable<T, T2, T3> ClearFilter<FilterType1, FilterType2>();
        new ISugarQueryable<T, T2, T3> ClearFilter<FilterType1, FilterType2, FilterType3>();
        new ISugarQueryable<T, T2, T3> AddParameters(object parameters);
        new ISugarQueryable<T, T2, T3> AddParameters(SugarParameter[] parameters);
        new ISugarQueryable<T, T2, T3> AddParameters(List<SugarParameter> parameters);
        new ISugarQueryable<T, T2, T3> AddJoinInfo(string tableName, string shortName, string joinWhere, JoinType type = JoinType.Left);
        new ISugarQueryable<T, T2, T3> With(string withString);
        new ISugarQueryable<T, T2, T3> WithCache(int cacheDurationInSeconds = int.MaxValue);
        new ISugarQueryable<T, T2, T3> WithCacheIF(bool isCache, int cacheDurationInSeconds = int.MaxValue);
        new ISugarQueryable<T, T2,T3> Distinct();
        bool Any(Expression<Func<T, T2,T3, bool>> expression);
        #endregion
    }
    public partial interface ISugarQueryable<T, T2, T3, T4> : ISugarQueryable<T>
    {
        new ISugarQueryable<T, T2, T3,T4> Hints(string hints);
        new ISugarQueryable<T, T2, T3,T4> SampleBy(int timeNumber, SampleByUnit timeType);
        new ISugarQueryable<T, T2, T3,T4> SampleBy(int timeNumber, string timeType);
        ISugarQueryable<TResult> SelectMergeTable<TResult>(Expression<Func<T, T2, T3,T4, TResult>> expression);
        ISugarQueryable<T, T2, T3, T4,T5> LeftJoinIF<T5>(bool isLeftJoin, Expression<Func<T, T2, T3, T4,T5, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4,T5> InnerJoinIF<T5>(bool isJoin, Expression<Func<T, T2, T3, T4,T5, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5> LeftJoin<T5>(ISugarQueryable<T5> joinQueryable, Expression<Func<T, T2, T3, T4, T5, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5> LeftJoinIF<T5>(bool isJoin,ISugarQueryable<T5> joinQueryable, Expression<Func<T, T2, T3, T4, T5, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5> InnerJoinIF<T5>(bool isJoin, ISugarQueryable<T5> joinQueryable, Expression<Func<T, T2, T3, T4, T5, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5> InnerJoin<T5>(ISugarQueryable<T5> joinQueryable, Expression<Func<T, T2, T3, T4, T5, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5> RightJoin<T5>(ISugarQueryable<T5> joinQueryable, Expression<Func<T, T2, T3, T4, T5, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4,T5> LeftJoin<T5>(Expression<Func<T, T2, T3, T4,T5, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5> FullJoin<T5>(Expression<Func<T, T2, T3, T4, T5, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4,T5> InnerJoin<T5>(Expression<Func<T, T2, T3, T4,T5, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5> RightJoin<T5>(Expression<Func<T, T2, T3, T4, T5, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5> LeftJoin<T5>(Expression<Func<T, T2, T3, T4, T5, bool>> joinExpression,string tableName);
        ISugarQueryable<T, T2, T3, T4, T5> FullJoin<T5>(Expression<Func<T, T2, T3, T4, T5, bool>> joinExpression, string tableName);
        ISugarQueryable<T, T2, T3, T4, T5> InnerJoin<T5>(Expression<Func<T, T2, T3, T4, T5, bool>> joinExpression, string tableName);
        ISugarQueryable<T, T2, T3, T4, T5> RightJoin<T5>(Expression<Func<T, T2, T3, T4, T5, bool>> joinExpression, string tableName);

        #region Where
        new ISugarQueryable<T, T2, T3,T4> Where(string expShortName, FormattableString expressionString);
        new ISugarQueryable<T, T2, T3, T4> Where(Expression<Func<T, bool>> expression);
        ISugarQueryable<T, T2, T3, T4> Where(Expression<Func<T, T2, bool>> expression);
        ISugarQueryable<T, T2, T3, T4> Where(Expression<Func<T, T2, T3, bool>> expression);
        ISugarQueryable<T, T2, T3, T4> Where(Expression<Func<T, T2, T3, T4, bool>> expression);
        new ISugarQueryable<T, T2, T3, T4> Where(List<IConditionalModel> conditionalModels);
        new ISugarQueryable<T, T2, T3,T4> Where(List<IConditionalModel> conditionalModels, bool isWrap);
        new ISugarQueryable<T, T2, T3,T4> Where(IFuncModel funcModel);
        new ISugarQueryable<T, T2, T3, T4> WhereIF(bool isWhere, Expression<Func<T, bool>> expression);
        ISugarQueryable<T, T2, T3, T4> WhereIF(bool isWhere, Expression<Func<T, T2, bool>> expression);
        ISugarQueryable<T, T2, T3, T4> WhereIF(bool isWhere, Expression<Func<T, T2, T3, bool>> expression);
        ISugarQueryable<T, T2, T3, T4> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, bool>> expression);

        new ISugarQueryable<T, T2, T3, T4> Where(string whereString, object parameters = null);
        new ISugarQueryable<T, T2, T3, T4> WhereIF(bool isWhere, string whereString, object parameters = null);

        /// <summary>
        /// if a property that is not empty is a condition
        /// </summary>
        /// <param name="whereClass"></param>
        /// <returns></returns>
        new ISugarQueryable<T, T2,T3,T4> WhereClass<ClassType>(ClassType whereClass, bool ignoreDefaultValue = false) where ClassType : class, new();
        /// <summary>
        ///  if a property that is not empty is a condition
        /// </summary>
        /// <param name="whereClassTypes"></param>
        /// <returns></returns>
        new ISugarQueryable<T, T2,T3,T4> WhereClass<ClassType>(List<ClassType> whereClassList, bool ignoreDefaultValue = false) where ClassType : class, new();
        #endregion

        #region Select
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3,T4, TResult>> expression, bool isAutoFill);
        #endregion

        #region OrderBy
        new ISugarQueryable<T, T2, T3,T4> OrderBy(List<OrderByModel> models);
        new ISugarQueryable<T, T2, T3,T4> OrderByPropertyName(string orderPropertyName, OrderByType? orderByType = null);
        new ISugarQueryable<T, T2, T3,T4> OrderByPropertyNameIF(bool isOrderBy, string orderPropertyName, OrderByType? orderByType = null);
        new ISugarQueryable<T, T2, T3,T4> OrderByDescending(Expression<Func<T, object>> expression);
        ISugarQueryable<T, T2, T3, T4> OrderByDescending(Expression<Func<T, T2, object>> expression);
        ISugarQueryable<T, T2, T3, T4> OrderByDescending(Expression<Func<T, T2, T3, object>> expression);
        ISugarQueryable<T, T2, T3, T4> OrderByDescending(Expression<Func<T, T2, T3,T4, object>> expression);
        new ISugarQueryable<T, T2, T3,T4> OrderBy(string orderByFields);
        new ISugarQueryable<T, T2, T3, T4> OrderBy(Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4> OrderBy(Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4> OrderBy(Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4> OrderBy(Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc);
        new ISugarQueryable<T, T2, T3, T4> OrderByIF(bool isOrderBy, string orderByFields);
        new ISugarQueryable<T, T2, T3, T4> OrderByIF(bool isOrderBy, Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4> OrderByIF(bool isOrderBy, Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc);
        #endregion

        #region GroupBy
        new ISugarQueryable<T, T2, T3, T4> GroupBy(Expression<Func<T, object>> expression);
        ISugarQueryable<T, T2, T3, T4> GroupBy(Expression<Func<T, T2, object>> expression);
        ISugarQueryable<T, T2, T3, T4> GroupBy(Expression<Func<T, T2, T3, object>> expression);
        ISugarQueryable<T, T2, T3, T4> GroupBy(Expression<Func<T, T2, T3, T4, object>> expression);
        new ISugarQueryable<T, T2, T3, T4> GroupByIF(bool isGroupBy,Expression<Func<T, object>> expression);
        ISugarQueryable<T, T2, T3, T4> GroupByIF(bool isGroupBy, Expression<Func<T, T2, object>> expression);
        ISugarQueryable<T, T2, T3, T4> GroupByIF(bool isGroupBy, Expression<Func<T, T2, T3, object>> expression);
        ISugarQueryable<T, T2, T3, T4> GroupByIF(bool isGroupBy, Expression<Func<T, T2, T3,T4, object>> expression);
        new ISugarQueryable<T, T2, T3, T4> GroupByIF(bool isGroupBy, string groupFields);
        new ISugarQueryable<T, T2, T3, T4> Having(Expression<Func<T, bool>> expression);
        ISugarQueryable<T, T2, T3, T4> Having(Expression<Func<T, T2, bool>> expression);
        ISugarQueryable<T, T2, T3, T4> Having(Expression<Func<T, T2, T3, bool>> expression);
        ISugarQueryable<T, T2, T3, T4> Having(Expression<Func<T, T2, T3, T4, bool>> expression);
        new ISugarQueryable<T, T2, T3, T4> Having(string whereString, object parameters = null);
        new ISugarQueryable<T, T2, T3,T4> HavingIF(bool isHaving, Expression<Func<T, bool>> expression);
        ISugarQueryable<T, T2, T3,T4> HavingIF(bool isHaving, Expression<Func<T, T2, bool>> expression);
        ISugarQueryable<T, T2, T3,T4> HavingIF(bool isHaving, Expression<Func<T, T2, T3, bool>> expression);
        ISugarQueryable<T, T2, T3, T4> HavingIF(bool isHaving, Expression<Func<T, T2, T3,T4, bool>> expression);
        #endregion

        #region Aggr
        TResult Max<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression);
        TResult Min<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression);
        TResult Sum<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression);
        TResult Avg<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression);
        #endregion

        #region In
        new ISugarQueryable<T, T2, T3, T4> In<FieldType>(Expression<Func<T, object>> expression, params FieldType[] inValues);
        new ISugarQueryable<T, T2, T3, T4> In<FieldType>(Expression<Func<T, object>> expression, List<FieldType> inValues);
        new ISugarQueryable<T, T2, T3, T4> In<FieldType>(Expression<Func<T, object>> expression, ISugarQueryable<FieldType> childQueryExpression);

        ISugarQueryable<T, T2, T3, T4> In<FieldType>(Expression<Func<T,T2, object>> expression, params FieldType[] inValues);
        ISugarQueryable<T, T2, T3, T4> In<FieldType>(Expression<Func<T,T2, object>> expression, List<FieldType> inValues);
        ISugarQueryable<T, T2, T3, T4> In<FieldType>(Expression<Func<T,T2, object>> expression, ISugarQueryable<FieldType> childQueryExpression);

        ISugarQueryable<T, T2, T3, T4> In<FieldType>(Expression<Func<T, T2,T3, object>> expression, params FieldType[] inValues);
        ISugarQueryable<T, T2, T3, T4> In<FieldType>(Expression<Func<T, T2,T3, object>> expression, List<FieldType> inValues);
        ISugarQueryable<T, T2, T3, T4> In<FieldType>(Expression<Func<T, T2,T3, object>> expression, ISugarQueryable<FieldType> childQueryExpression);

        ISugarQueryable<T, T2, T3, T4> In<FieldType>(Expression<Func<T, T2, T3,T4, object>> expression, params FieldType[] inValues);
        ISugarQueryable<T, T2, T3, T4> In<FieldType>(Expression<Func<T, T2, T3,T4, object>> expression, List<FieldType> inValues);
        ISugarQueryable<T, T2, T3, T4> In<FieldType>(Expression<Func<T, T2, T3,T4, object>> expression, ISugarQueryable<FieldType> childQueryExpression);

        new ISugarQueryable<T, T2,T3,T4> InIF<TParamter>(bool isIn, params TParamter[] pkValues);
        new ISugarQueryable<T, T2, T3,T4> InIF<TParamter>(bool isIn, string fieldName, params TParamter[] pkValues);
        #endregion

        #region Other
        new ISugarQueryable<T, T2,T3,T4> Take(int num);
        new ISugarQueryable<T, T2, T3, T4> Clone();
        new ISugarQueryable<T, T2, T3, T4> AS<AsT>(string tableName);
        new ISugarQueryable<T, T2, T3, T4> AS(string tableName);
        new ISugarQueryable<T, T2, T3,T4> ClearFilter();
        new ISugarQueryable<T, T2, T3, T4> Filter(string FilterName, bool isDisabledGobalFilter = false);
        new ISugarQueryable<T, T2, T3, T4> ClearFilter(params Type[] types);
        new ISugarQueryable<T, T2, T3, T4> ClearFilter<FilterType1>();
        new ISugarQueryable<T, T2, T3, T4> ClearFilter<FilterType1, FilterType2>();
        new ISugarQueryable<T, T2, T3, T4> ClearFilter<FilterType1, FilterType2, FilterType3>();
        new ISugarQueryable<T, T2, T3, T4> AddParameters(object parameters);
        new ISugarQueryable<T, T2, T3, T4> AddParameters(SugarParameter[] parameters);
        new ISugarQueryable<T, T2, T3, T4> AddParameters(List<SugarParameter> parameters);
        new ISugarQueryable<T, T2, T3, T4> AddJoinInfo(string tableName, string shortName, string joinWhere, JoinType type = JoinType.Left);
        new ISugarQueryable<T, T2, T3, T4> With(string withString);
        new ISugarQueryable<T, T2, T3, T4> WithCache(int cacheDurationInSeconds = int.MaxValue);
        new ISugarQueryable<T, T2, T3, T4> WithCacheIF(bool isCache, int cacheDurationInSeconds = int.MaxValue);
        new ISugarQueryable<T, T2, T3,T4> Distinct();
        bool Any(Expression<Func<T, T2, T3,T4, bool>> expression);
        #endregion
    }
    public partial interface ISugarQueryable<T, T2, T3, T4, T5> : ISugarQueryable<T>
    {
        new ISugarQueryable<T, T2, T3, T4,T5> Hints(string hints);
        new ISugarQueryable<T, T2, T3, T4,T5> SampleBy(int timeNumber, SampleByUnit timeType);
        new ISugarQueryable<T, T2, T3, T4,T5> SampleBy(int timeNumber, string timeType);
        ISugarQueryable<TResult> SelectMergeTable<TResult>(Expression<Func<T, T2, T3,T4,T5, TResult>> expression);
        ISugarQueryable<T, T2, T3, T4, T5,T6> LeftJoinIF<T6>(bool isLeftJoin, Expression<Func<T, T2, T3, T4, T5,T6, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5,T6> InnerJoinIF<T6>(bool isJoin, Expression<Func<T, T2, T3, T4, T5,T6, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6> LeftJoin<T6>(ISugarQueryable<T6> joinQueryable, Expression<Func<T, T2, T3, T4, T5, T6, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6> LeftJoinIF<T6>(bool isJoin,ISugarQueryable<T6> joinQueryable, Expression<Func<T, T2, T3, T4, T5, T6, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6> InnerJoinIF<T6>(bool isJoin, ISugarQueryable<T6> joinQueryable, Expression<Func<T, T2, T3, T4, T5, T6, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6> InnerJoin<T6>(ISugarQueryable<T6> joinQueryable, Expression<Func<T, T2, T3, T4, T5, T6, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6> RightJoin<T6>(ISugarQueryable<T6> joinQueryable, Expression<Func<T, T2, T3, T4, T5, T6, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5,T6> LeftJoin<T6>(Expression<Func<T, T2, T3, T4, T5,T6, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6> FullJoin<T6>(Expression<Func<T, T2, T3, T4, T5, T6, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5,T6> InnerJoin<T6>(Expression<Func<T, T2, T3, T4, T5,T6, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6> RightJoin<T6>(Expression<Func<T, T2, T3, T4, T5, T6, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6> LeftJoin<T6>(Expression<Func<T, T2, T3, T4, T5, T6, bool>> joinExpression,string tableName);
        ISugarQueryable<T, T2, T3, T4, T5, T6> FullJoin<T6>(Expression<Func<T, T2, T3, T4, T5, T6, bool>> joinExpression,string tableName);
        ISugarQueryable<T, T2, T3, T4, T5, T6> InnerJoin<T6>(Expression<Func<T, T2, T3, T4, T5, T6, bool>> joinExpression,string tableName);
        ISugarQueryable<T, T2, T3, T4, T5, T6> RightJoin<T6>(Expression<Func<T, T2, T3, T4, T5, T6, bool>> joinExpression,string tableName);

        #region Where
        new ISugarQueryable<T, T2, T3, T4,T5> Where(string expShortName, FormattableString expressionString);
        new ISugarQueryable<T, T2, T3, T4, T5> Where(Expression<Func<T, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5> Where(Expression<Func<T, T2, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5> Where(Expression<Func<T, T2, T3, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5> Where(Expression<Func<T, T2, T3, T4, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5> Where(Expression<Func<T, T2, T3, T4, T5, bool>> expression);
        new ISugarQueryable<T, T2, T3, T4, T5> Where(List<IConditionalModel> conditionalModels);
        new ISugarQueryable<T, T2, T3, T4, T5> Where(List<IConditionalModel> conditionalModels, bool isWrap);
        new ISugarQueryable<T, T2, T3, T4,T5> Where(IFuncModel funcModel);
        new ISugarQueryable<T, T2, T3, T4, T5> WhereIF(bool isWhere, Expression<Func<T, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5> WhereIF(bool isWhere, Expression<Func<T, T2, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5> WhereIF(bool isWhere, Expression<Func<T, T2, T3, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, bool>> expression);

        new ISugarQueryable<T, T2, T3, T4, T5> Where(string whereString, object parameters = null);
        new ISugarQueryable<T, T2, T3, T4, T5> WhereIF(bool isWhere, string whereString, object parameters = null);

        /// <summary>
        /// if a property that is not empty is a condition
        /// </summary>
        /// <param name="whereClass"></param>
        /// <returns></returns>
        new ISugarQueryable<T, T2, T3, T4, T5> WhereClass<ClassType>(ClassType whereClass, bool ignoreDefaultValue = false) where ClassType : class, new();
        /// <summary>
        ///  if a property that is not empty is a condition
        /// </summary>
        /// <param name="whereClassTypes"></param>
        /// <returns></returns>
        new ISugarQueryable<T, T2, T3, T4, T5> WhereClass<ClassType>(List<ClassType> whereClassList, bool ignoreDefaultValue = false) where ClassType : class, new();
        #endregion

        #region Select
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4,T5, TResult>> expression, bool isAutoFill);
        #endregion

        #region OrderBy
        new ISugarQueryable<T, T2, T3, T4,T5> OrderBy(List<OrderByModel> models);
        new ISugarQueryable<T, T2, T3, T4,T5> OrderByPropertyName(string orderPropertyName, OrderByType? orderByType = null);
        new ISugarQueryable<T, T2, T3, T4,T5> OrderByPropertyNameIF(bool isOrderBy, string orderPropertyName, OrderByType? orderByType = null);
        new ISugarQueryable<T, T2, T3, T4, T5> OrderByDescending(Expression<Func<T, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5> OrderByDescending(Expression<Func<T, T2, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5> OrderByDescending(Expression<Func<T, T2, T3, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5> OrderByDescending(Expression<Func<T, T2, T3, T4, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5> OrderByDescending(Expression<Func<T, T2, T3, T4,T5,object>> expression);
        new ISugarQueryable<T, T2, T3,T4,T5> OrderBy(string orderByFields);
        new ISugarQueryable<T, T2, T3, T4, T5> OrderBy(Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5> OrderBy(Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5> OrderBy(Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5> OrderBy(Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5> OrderBy(Expression<Func<T, T2, T3, T4, T5, object>> expression, OrderByType type = OrderByType.Asc);
        new ISugarQueryable<T, T2, T3, T4, T5> OrderByIF(bool isOrderBy, string orderByFields);
        new ISugarQueryable<T, T2, T3, T4, T5> OrderByIF(bool isOrderBy, Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5> OrderByIF(bool isOrderBy, Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, object>> expression, OrderByType type = OrderByType.Asc);
        #endregion

        #region GroupBy
        new ISugarQueryable<T, T2, T3, T4, T5> GroupByIF(bool isGroupBy, Expression<Func<T, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5> GroupByIF(bool isGroupBy, Expression<Func<T, T2, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5> GroupByIF(bool isGroupBy, Expression<Func<T, T2, T3, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5> GroupByIF(bool isGroupBy, Expression<Func<T, T2, T3, T4, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5> GroupByIF(bool isGroupBy, Expression<Func<T, T2, T3, T4,T5, object>> expression);
        new ISugarQueryable<T, T2, T3, T4, T5> GroupByIF(bool isGroupBy, string groupFields);
        new ISugarQueryable<T, T2, T3, T4, T5> GroupBy(Expression<Func<T, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5> GroupBy(Expression<Func<T, T2, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5> GroupBy(Expression<Func<T, T2, T3, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5> GroupBy(Expression<Func<T, T2, T3, T4, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5> GroupBy(Expression<Func<T, T2, T3, T4, T5, object>> expression);
        new ISugarQueryable<T, T2, T3, T4,T5> Having(Expression<Func<T, bool>> expression);
        ISugarQueryable<T, T2, T3, T4,T5> Having(Expression<Func<T, T2, bool>> expression);
        ISugarQueryable<T, T2, T3, T4,T5> Having(Expression<Func<T, T2, T3, bool>> expression);
        ISugarQueryable<T, T2, T3, T4,T5> Having(Expression<Func<T, T2, T3, T4, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5> Having(Expression<Func<T, T2, T3, T4,T5, bool>> expression);
        new ISugarQueryable<T, T2, T3, T4,T5> Having(string whereString, object parameters = null);
        new ISugarQueryable<T, T2, T3, T4,T5> HavingIF(bool isHaving, Expression<Func<T, bool>> expression);
        ISugarQueryable<T, T2, T3, T4,T5> HavingIF(bool isHaving, Expression<Func<T, T2, bool>> expression);
        ISugarQueryable<T, T2, T3, T4,T5> HavingIF(bool isHaving, Expression<Func<T, T2, T3, bool>> expression);
        ISugarQueryable<T, T2, T3, T4,T5> HavingIF(bool isHaving, Expression<Func<T, T2, T3, T4, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5> HavingIF(bool isHaving, Expression<Func<T, T2, T3, T4,T5, bool>> expression);
        #endregion

        #region Aggr
        TResult Max<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression);
        TResult Min<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression);
        TResult Sum<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression);
        TResult Avg<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression);
        #endregion

        #region In
        new ISugarQueryable<T, T2, T3, T4, T5> In<FieldType>(Expression<Func<T, object>> expression, params FieldType[] inValues);
        new ISugarQueryable<T, T2, T3, T4, T5> In<FieldType>(Expression<Func<T, object>> expression, List<FieldType> inValues);
        new ISugarQueryable<T, T2, T3, T4, T5> In<FieldType>(Expression<Func<T, object>> expression, ISugarQueryable<FieldType> childQueryExpression);
        #endregion

        #region Other
        new ISugarQueryable<T, T2, T3, T4, T5> Take(int num);
        new ISugarQueryable<T, T2, T3, T4, T5> Clone();
        new ISugarQueryable<T, T2, T3, T4, T5> AS<AsT>(string tableName);
        new ISugarQueryable<T, T2, T3, T4, T5> AS(string tableName);
        new ISugarQueryable<T, T2, T3, T4,T5> ClearFilter();
        new ISugarQueryable<T, T2, T3, T4, T5> Filter(string FilterName, bool isDisabledGobalFilter = false);
        new ISugarQueryable<T, T2, T3, T4, T5> ClearFilter(params Type[] types);
        new ISugarQueryable<T, T2, T3, T4, T5> ClearFilter<FilterType1>();
        new ISugarQueryable<T, T2, T3, T4, T5> ClearFilter<FilterType1, FilterType2>();
        new ISugarQueryable<T, T2, T3, T4, T5> ClearFilter<FilterType1, FilterType2, FilterType3>();
        new ISugarQueryable<T, T2, T3, T4, T5> AddParameters(object parameters);
        new ISugarQueryable<T, T2, T3, T4, T5> AddParameters(SugarParameter[] parameters);
        new ISugarQueryable<T, T2, T3, T4, T5> AddParameters(List<SugarParameter> parameters);
        new ISugarQueryable<T, T2, T3, T4, T5> AddJoinInfo(string tableName, string shortName, string joinWhere, JoinType type = JoinType.Left);
        new ISugarQueryable<T, T2, T3, T4, T5> With(string withString);
        new ISugarQueryable<T, T2, T3, T4, T5> WithCache(int cacheDurationInSeconds = int.MaxValue);
        new ISugarQueryable<T, T2, T3, T4, T5> WithCacheIF(bool isCache, int cacheDurationInSeconds = int.MaxValue);
        new ISugarQueryable<T, T2, T3, T4,T5> Distinct();
        bool Any(Expression<Func<T, T2, T3, T4,T5, bool>> expression);
        #endregion
    }
    public partial interface ISugarQueryable<T, T2, T3, T4, T5, T6> : ISugarQueryable<T>
    {
        new ISugarQueryable<T, T2, T3, T4, T5,T6> Hints(string hints);
        ISugarQueryable<TResult> SelectMergeTable<TResult>(Expression<Func<T, T2, T3, T4, T5,T6, TResult>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6,T7> LeftJoinIF<T7>(bool isLeftJoin, Expression<Func<T, T2, T3, T4, T5, T6,T7, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6,T7> InnerJoinIF<T7>(bool isJoin, Expression<Func<T, T2, T3, T4, T5, T6,T7, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7> LeftJoin<T7>(ISugarQueryable<T7> joinQueryable, Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7> InnerJoin<T7>(ISugarQueryable<T7> joinQueryable, Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7> RightJoin<T7>(ISugarQueryable<T7> joinQueryable, Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6,T7> LeftJoin<T7>(Expression<Func<T, T2, T3, T4, T5, T6,T7, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7> FullJoin<T7>(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6,T7> InnerJoin<T7>(Expression<Func<T, T2, T3, T4, T5, T6,T7, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7> RightJoin<T7>(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7> LeftJoin<T7>(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> joinExpression,string tableName);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7> FullJoin<T7>(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> joinExpression, string tableName);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7> InnerJoin<T7>(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> joinExpression, string tableName);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7> RightJoin<T7>(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> joinExpression, string tableName);

        #region Where
        new ISugarQueryable<T, T2, T3, T4, T5,T6> Where(string expShortName, FormattableString expressionString);
        new ISugarQueryable<T, T2, T3, T4, T5, T6> Where(Expression<Func<T, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6> Where(Expression<Func<T, T2, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6> Where(Expression<Func<T, T2, T3, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6> Where(Expression<Func<T, T2, T3, T4, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6> Where(Expression<Func<T, T2, T3, T4, T5, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6> Where(Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression);
        new ISugarQueryable<T, T2, T3, T4, T5, T6> Where(List<IConditionalModel> conditionalModels);
        new ISugarQueryable<T, T2, T3, T4, T5,T6> Where(List<IConditionalModel> conditionalModels, bool isWrap);
        new ISugarQueryable<T, T2, T3, T4, T5,T6> Where(IFuncModel funcModel);
        new ISugarQueryable<T, T2, T3, T4, T5, T6> WhereIF(bool isWhere, Expression<Func<T, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6> WhereIF(bool isWhere, Expression<Func<T, T2, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6> WhereIF(bool isWhere, Expression<Func<T, T2, T3, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression);

        new ISugarQueryable<T, T2, T3, T4, T5, T6> Where(string whereString, object parameters = null);
        new ISugarQueryable<T, T2, T3, T4, T5, T6> WhereIF(bool isWhere, string whereString, object parameters = null);

        /// <summary>
        /// if a property that is not empty is a condition
        /// </summary>
        /// <param name="whereClass"></param>
        /// <returns></returns>
        new ISugarQueryable<T, T2, T3, T4, T5, T6> WhereClass<ClassType>(ClassType whereClass, bool ignoreDefaultValue = false) where ClassType : class, new();
        /// <summary>
        ///  if a property that is not empty is a condition
        /// </summary>
        /// <param name="whereClassTypes"></param>
        /// <returns></returns>
        new ISugarQueryable<T, T2, T3, T4, T5, T6> WhereClass<ClassType>(List<ClassType> whereClassList, bool ignoreDefaultValue = false) where ClassType : class, new();

        #endregion

        #region Select
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5,T6, TResult>> expression, bool isAutoFill);
        #endregion

        #region OrderBy
        new ISugarQueryable<T, T2, T3, T4, T5,T6> OrderBy(List<OrderByModel> models);
        new ISugarQueryable<T, T2, T3, T4, T5,T6> OrderByPropertyNameIF(bool isOrderBy, string orderPropertyName, OrderByType? orderByType = null);
        new ISugarQueryable<T, T2, T3, T4, T5,T6> OrderByPropertyName(string orderPropertyName, OrderByType? orderByType = null);
        new ISugarQueryable<T, T2, T3, T4, T5, T6> OrderByDescending(Expression<Func<T, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6> OrderByDescending(Expression<Func<T, T2, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6> OrderByDescending(Expression<Func<T, T2, T3, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6> OrderByDescending(Expression<Func<T, T2, T3, T4, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6> OrderByDescending(Expression<Func<T, T2, T3, T4, T5, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6> OrderByDescending(Expression<Func<T, T2, T3, T4, T5,T6, object>> expression);
        new ISugarQueryable<T, T2, T3,T4,T5,T6> OrderBy(string orderByFields);
        new ISugarQueryable<T, T2, T3, T4, T5, T6> OrderBy(Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6> OrderBy(Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6> OrderBy(Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6> OrderBy(Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6> OrderBy(Expression<Func<T, T2, T3, T4, T5, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression, OrderByType type = OrderByType.Asc);
        new ISugarQueryable<T, T2, T3, T4, T5,T6> OrderByIF(bool isOrderBy, string orderByFields);
        new ISugarQueryable<T, T2, T3, T4, T5,T6> OrderByIF(bool isOrderBy, Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5,T6> OrderByIF(bool isOrderBy, Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5,T6> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5,T6> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5,T6> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5,T6, object>> expression, OrderByType type = OrderByType.Asc);
        #endregion

        #region GroupBy
        new ISugarQueryable<T, T2, T3, T4, T5,T6> GroupByIF(bool isGroupBy, string groupFields);
        new ISugarQueryable<T, T2, T3, T4, T5, T6> GroupByIF(bool isGroupBy, Expression<Func<T, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6> GroupByIF(bool isGroupBy, Expression<Func<T, T2, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6> GroupByIF(bool isGroupBy, Expression<Func<T, T2, T3, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6> GroupByIF(bool isGroupBy, Expression<Func<T, T2, T3, T4, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6> GroupByIF(bool isGroupBy, Expression<Func<T, T2, T3, T4, T5, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6> GroupByIF(bool isGroupBy, Expression<Func<T, T2, T3, T4, T5,T6, object>> expression);
        new ISugarQueryable<T, T2, T3, T4, T5, T6> GroupBy(Expression<Func<T, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6> GroupBy(Expression<Func<T, T2, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6> GroupBy(Expression<Func<T, T2, T3, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6> GroupBy(Expression<Func<T, T2, T3, T4, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6> GroupBy(Expression<Func<T, T2, T3, T4, T5, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression);
        new ISugarQueryable<T, T2, T3, T4, T5,T6> Having(Expression<Func<T, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6> Having(Expression<Func<T, T2, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6> Having(Expression<Func<T, T2, T3, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6> Having(Expression<Func<T, T2, T3, T4, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6> Having(Expression<Func<T, T2, T3, T4, T5, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6> Having(Expression<Func<T, T2, T3, T4, T5,T6, bool>> expression);
        new ISugarQueryable<T, T2, T3, T4, T5, T6> Having(string whereString, object parameters = null);
        new ISugarQueryable<T, T2, T3, T4, T5, T6> HavingIF(bool isHaving, Expression<Func<T, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6> HavingIF(bool isHaving, Expression<Func<T, T2, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6> HavingIF(bool isHaving, Expression<Func<T, T2, T3, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6> HavingIF(bool isHaving, Expression<Func<T, T2, T3, T4, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6> HavingIF(bool isHaving, Expression<Func<T, T2, T3, T4, T5, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6> HavingIF(bool isHaving, Expression<Func<T, T2, T3, T4, T5,T6, bool>> expression);
        #endregion

        #region Aggr
        TResult Max<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression);
        TResult Min<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression);
        TResult Sum<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression);
        TResult Avg<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression);
        #endregion

        #region In
        new ISugarQueryable<T, T2, T3, T4, T5, T6> In<FieldType>(Expression<Func<T, object>> expression, params FieldType[] inValues);
        new ISugarQueryable<T, T2, T3, T4, T5, T6> In<FieldType>(Expression<Func<T, object>> expression, List<FieldType> inValues);
        new ISugarQueryable<T, T2, T3, T4, T5, T6> In<FieldType>(Expression<Func<T, object>> expression, ISugarQueryable<FieldType> childQueryExpression);
        #endregion

        #region Other
        new ISugarQueryable<T, T2, T3, T4, T5, T6> Take(int num);
        new ISugarQueryable<T, T2, T3, T4, T5, T6> Clone();
        new ISugarQueryable<T, T2, T3, T4, T5, T6> AS<AsT>(string tableName);
        new ISugarQueryable<T, T2, T3, T4, T5, T6> AS(string tableName);
        new ISugarQueryable<T, T2, T3, T4, T5,T6> ClearFilter();
        new ISugarQueryable<T, T2, T3, T4, T5, T6> Filter(string FilterName, bool isDisabledGobalFilter = false);
        new ISugarQueryable<T, T2, T3, T4, T5, T6> ClearFilter(params Type[] types);
        new ISugarQueryable<T, T2, T3, T4, T5, T6> ClearFilter<FilterType1>();
        new ISugarQueryable<T, T2, T3, T4, T5, T6> ClearFilter<FilterType1, FilterType2>();
        new ISugarQueryable<T, T2, T3, T4, T5, T6> ClearFilter<FilterType1, FilterType2, FilterType3>();
        new ISugarQueryable<T, T2, T3, T4, T5, T6> AddParameters(object parameters);
        new ISugarQueryable<T, T2, T3, T4, T5, T6> AddParameters(SugarParameter[] parameters);
        new ISugarQueryable<T, T2, T3, T4, T5, T6> AddParameters(List<SugarParameter> parameters);
        new ISugarQueryable<T, T2, T3, T4, T5, T6> AddJoinInfo(string tableName, string shortName, string joinWhere, JoinType type = JoinType.Left);
        new ISugarQueryable<T, T2, T3, T4, T5, T6> With(string withString);
        new ISugarQueryable<T, T2, T3, T4, T5, T6> WithCache(int cacheDurationInSeconds = int.MaxValue);
        new ISugarQueryable<T, T2, T3, T4, T5, T6> WithCacheIF(bool isCache, int cacheDurationInSeconds = int.MaxValue);
        new ISugarQueryable<T, T2, T3, T4, T5,T6> Distinct();
        bool Any(Expression<Func<T, T2, T3, T4, T5,T6, bool>> expression);
        #endregion
    }
    public partial interface ISugarQueryable<T, T2, T3, T4, T5, T6, T7> : ISugarQueryable<T>
    {
        new ISugarQueryable<T, T2, T3, T4, T5, T6,T7> Hints(string hints);
        ISugarQueryable<TResult> SelectMergeTable<TResult>(Expression<Func<T, T2, T3, T4, T5, T6,T7, TResult>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7,T8> LeftJoinIF<T8>(bool isLeftJoin, Expression<Func<T, T2, T3, T4, T5, T6, T7,T8, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7,T8> InnerJoinIF<T8>(bool isJoin, Expression<Func<T, T2, T3, T4, T5, T6, T7,T8, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> LeftJoin<T8>(ISugarQueryable<T8> joinQueryable, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> InnerJoin<T8>(ISugarQueryable<T8> joinQueryable, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> RightJoin<T8>(ISugarQueryable<T8> joinQueryable, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> LeftJoin<T8>(Expression<Func<T, T2, T3, T4, T5, T6, T7,T8, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> FullJoin<T8>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> InnerJoin<T8>(Expression<Func<T, T2, T3, T4, T5, T6, T7,T8, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> RightJoin<T8>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> LeftJoin<T8>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> joinExpression,string tableName);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> FullJoin<T8>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> joinExpression, string tableName);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> InnerJoin<T8>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> joinExpression, string tableName);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> RightJoin<T8>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> joinExpression, string tableName);

        #region Where
        new ISugarQueryable<T, T2, T3, T4, T5, T6,T7> Where(string expShortName, FormattableString expressionString);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Where(Expression<Func<T, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Where(Expression<Func<T, T2, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Where(Expression<Func<T, T2, T3, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Where(Expression<Func<T, T2, T3, T4, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Where(Expression<Func<T, T2, T3, T4, T5, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Where(Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Where(List<IConditionalModel> conditionalModels);
        new ISugarQueryable<T, T2, T3, T4, T5,T6,T7> Where(List<IConditionalModel> conditionalModels, bool isWrap);

        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> WhereIF(bool isWhere, Expression<Func<T, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7> WhereIF(bool isWhere, Expression<Func<T, T2, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7> WhereIF(bool isWhere, Expression<Func<T, T2, T3, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression);

        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Where(string whereString, object parameters = null);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> WhereIF(bool isWhere, string whereString, object parameters = null);

        /// <summary>
        /// if a property that is not empty is a condition
        /// </summary>
        /// <param name="whereClass"></param>
        /// <returns></returns>
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> WhereClass<ClassType>(ClassType whereClass, bool ignoreDefaultValue = false) where ClassType : class, new();
        /// <summary>
        ///  if a property that is not empty is a condition
        /// </summary>
        /// <param name="whereClassTypes"></param>
        /// <returns></returns>
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> WhereClass<ClassType>(List<ClassType> whereClassList, bool ignoreDefaultValue = false) where ClassType : class, new();

        #endregion

        #region Select
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5,T6,T7, TResult>> expression, bool isAutoFill);
        #endregion

        #region OrderBy
        new ISugarQueryable<T, T2, T3, T4, T5, T6,T7> OrderBy(List<OrderByModel> models);
        new ISugarQueryable<T, T2, T3, T4, T5, T6,T7> OrderByPropertyNameIF(bool isOrderBy, string orderPropertyName, OrderByType? orderByType = null);
        new ISugarQueryable<T, T2, T3, T4, T5, T6,T7> OrderByPropertyName(string orderPropertyName, OrderByType? orderByType = null);
        new ISugarQueryable<T, T2, T3, T4, T5, T6,T7> OrderByDescending(Expression<Func<T, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6,T7> OrderByDescending(Expression<Func<T, T2, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6,T7> OrderByDescending(Expression<Func<T, T2, T3, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6,T7> OrderByDescending(Expression<Func<T, T2, T3, T4, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6,T7> OrderByDescending(Expression<Func<T, T2, T3, T4, T5, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6,T7> OrderByDescending(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7> OrderByDescending(Expression<Func<T, T2, T3, T4, T5, T6,T7, object>> expression);
        new ISugarQueryable<T, T2, T3, T4, T5, T6,T7> OrderBy(string orderByFields);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> OrderBy(Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7> OrderBy(Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7> OrderBy(Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7> OrderBy(Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7> OrderBy(Expression<Func<T, T2, T3, T4, T5, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression, OrderByType type = OrderByType.Asc);
        new ISugarQueryable<T, T2, T3, T4, T5, T6,T7> OrderByIF(bool isOrderBy, string orderByFields);
        new ISugarQueryable<T, T2, T3, T4, T5, T6,T7> OrderByIF(bool isOrderBy, Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6,T7> OrderByIF(bool isOrderBy, Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6,T7> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6,T7> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6,T7> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6,T7> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6,T7, object>> expression, OrderByType type = OrderByType.Asc);
        #endregion

        #region GroupBy
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> GroupByIF(bool isGroupBy, string groupFields);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> GroupByIF(bool isGroupBy, Expression<Func<T, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7> GroupByIF(bool isGroupBy, Expression<Func<T, T2, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7> GroupByIF(bool isGroupBy, Expression<Func<T, T2, T3, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7> GroupByIF(bool isGroupBy, Expression<Func<T, T2, T3, T4, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7> GroupByIF(bool isGroupBy, Expression<Func<T, T2, T3, T4, T5, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7> GroupByIF(bool isGroupBy, Expression<Func<T, T2, T3, T4, T5, T6, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7> GroupByIF(bool isGroupBy, Expression<Func<T, T2, T3, T4, T5, T6,T7, object>> expression);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> GroupBy(Expression<Func<T, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7> GroupBy(Expression<Func<T, T2, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7> GroupBy(Expression<Func<T, T2, T3, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7> GroupBy(Expression<Func<T, T2, T3, T4, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7> GroupBy(Expression<Func<T, T2, T3, T4, T5, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression);
        #endregion

        #region Aggr
        TResult Max<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> expression);
        TResult Min<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> expression);
        TResult Sum<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> expression);
        TResult Avg<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> expression);
        #endregion

        #region In
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> In<FieldType>(Expression<Func<T, object>> expression, params FieldType[] inValues);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> In<FieldType>(Expression<Func<T, object>> expression, List<FieldType> inValues);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> In<FieldType>(Expression<Func<T, object>> expression, ISugarQueryable<FieldType> childQueryExpression);
        #endregion

        #region Other
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Take(int num);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Clone();
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> AS<AsT>(string tableName);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> AS(string tableName);
        new ISugarQueryable<T, T2, T3, T4, T5, T6,T7> ClearFilter();
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Filter(string FilterName, bool isDisabledGobalFilter = false);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> ClearFilter(params Type[] types);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> ClearFilter<FilterType1>();
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> ClearFilter<FilterType1, FilterType2>();
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> ClearFilter<FilterType1, FilterType2, FilterType3>();
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> AddParameters(object parameters);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> AddParameters(SugarParameter[] parameters);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> AddParameters(List<SugarParameter> parameters);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> AddJoinInfo(string tableName, string shortName, string joinWhere, JoinType type = JoinType.Left);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> With(string withString);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> WithCache(int cacheDurationInSeconds = int.MaxValue);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> WithCacheIF(bool isCache, int cacheDurationInSeconds = int.MaxValue);
        new ISugarQueryable<T, T2, T3, T4, T5, T6,T7> Distinct();
        bool Any(Expression<Func<T, T2, T3, T4, T5, T6,T7, bool>> expression);
        #endregion
    }
    public partial interface ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> : ISugarQueryable<T>
    {
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7,T8> Hints(string hints);
        ISugarQueryable<TResult> SelectMergeTable<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7,T8, TResult>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9> LeftJoinIF<T9>(bool isLeftJoin, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8,T9, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9> InnerJoinIF<T9>(bool isJoin, Expression<Func<T, T2, T3, T4, T5, T6, T7,T8,T9, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> LeftJoin<T9>(ISugarQueryable<T9> joinQueryable, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> InnerJoin<T9>(ISugarQueryable<T9> joinQueryable, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> RightJoin<T9>(ISugarQueryable<T9> joinQueryable, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9> LeftJoin<T9>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8,T9, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> FullJoin<T9>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9> InnerJoin<T9>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8,T9, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> RightJoin<T9>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> LeftJoin<T9>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> joinExpression,string tableName);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> FullJoin<T9>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> joinExpression, string tableName);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> InnerJoin<T9>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> joinExpression, string tableName);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> RightJoin<T9>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> joinExpression, string tableName);

        #region Where
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7,T8> Where(string expShortName, FormattableString expressionString);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Where(Expression<Func<T, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Where(Expression<Func<T, T2, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Where(Expression<Func<T, T2, T3, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Where(Expression<Func<T, T2, T3, T4, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Where(Expression<Func<T, T2, T3, T4, T5, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Where(Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Where(List<IConditionalModel> conditionalModels);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7,T8> Where(List<IConditionalModel> conditionalModels, bool isWrap);

        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> WhereIF(bool isWhere, Expression<Func<T, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> WhereIF(bool isWhere, Expression<Func<T, T2, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> WhereIF(bool isWhere, Expression<Func<T, T2, T3, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression);

        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Where(string whereString, object parameters = null);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> WhereIF(bool isWhere, string whereString, object parameters = null);

        /// <summary>
        /// if a property that is not empty is a condition
        /// </summary>
        /// <param name="whereClass"></param>
        /// <returns></returns>
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> WhereClass<ClassType>(ClassType whereClass, bool ignoreDefaultValue = false) where ClassType : class, new();
        /// <summary>
        ///  if a property that is not empty is a condition
        /// </summary>
        /// <param name="whereClassTypes"></param>
        /// <returns></returns>
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> WhereClass<ClassType>(List<ClassType> whereClassList, bool ignoreDefaultValue = false) where ClassType : class, new();

        #endregion                              

        #region Select
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7,T8, TResult>> expression, bool isAutoFill);
        #endregion

        #region OrderBy
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7,T8> OrderBy(List<OrderByModel> models);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7,T8> OrderByPropertyNameIF(bool isOrderBy, string orderPropertyName, OrderByType? orderByType = null);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7,T8> OrderByPropertyName(string orderPropertyName, OrderByType? orderByType = null);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7,T8> OrderByDescending(Expression<Func<T, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7,T8> OrderByDescending(Expression<Func<T, T2, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7,T8> OrderByDescending(Expression<Func<T, T2, T3, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7,T8> OrderByDescending(Expression<Func<T, T2, T3, T4, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7,T8> OrderByDescending(Expression<Func<T, T2, T3, T4, T5, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7,T8> OrderByDescending(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7,T8> OrderByDescending(Expression<Func<T, T2, T3, T4, T5, T6,T7, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> OrderByDescending(Expression<Func<T, T2, T3, T4, T5, T6,T7,T8, object>> expression);
        new ISugarQueryable<T, T2, T3, T4, T5, T6,T7,T8> OrderBy(string orderByFields);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> OrderBy(Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> OrderBy(Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> OrderBy(Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> OrderBy(Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> OrderBy(Expression<Func<T, T2, T3, T4, T5, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression, OrderByType type = OrderByType.Asc);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7,T8> OrderByIF(bool isOrderBy, string orderByFields);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7,T8> OrderByIF(bool isOrderBy, Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7,T8> OrderByIF(bool isOrderBy, Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7,T8> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7,T8> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7,T8> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7,T8> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7,T8> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, T7,T8, object>> expression, OrderByType type = OrderByType.Asc);
        #endregion

        #region GroupBy
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> GroupBy(Expression<Func<T, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> GroupBy(Expression<Func<T, T2, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> GroupBy(Expression<Func<T, T2, T3, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> GroupBy(Expression<Func<T, T2, T3, T4, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> GroupBy(Expression<Func<T, T2, T3, T4, T5, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression);
        #endregion

        #region Aggr
        TResult Max<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> expression);
        TResult Min<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> expression);
        TResult Sum<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> expression);
        TResult Avg<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> expression);
        #endregion

        #region In
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> In<FieldType>(Expression<Func<T, object>> expression, params FieldType[] inValues);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> In<FieldType>(Expression<Func<T, object>> expression, List<FieldType> inValues);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> In<FieldType>(Expression<Func<T, object>> expression, ISugarQueryable<FieldType> childQueryExpression);
        #endregion

        #region Other
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7,T8> Take(int num);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Clone();
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> AS<AsT>(string tableName);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> AS(string tableName);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7,T8> ClearFilter();
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Filter(string FilterName, bool isDisabledGobalFilter = false);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> ClearFilter(params Type[] types);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> ClearFilter<FilterType1>();
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> ClearFilter<FilterType1, FilterType2>();
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> ClearFilter<FilterType1, FilterType2, FilterType3>();
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> AddParameters(object parameters);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> AddParameters(SugarParameter[] parameters);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> AddParameters(List<SugarParameter> parameters);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> AddJoinInfo(string tableName, string shortName, string joinWhere, JoinType type = JoinType.Left);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> With(string withString);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> WithCache(int cacheDurationInSeconds = int.MaxValue);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> WithCacheIF(bool isCache, int cacheDurationInSeconds = int.MaxValue);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7,T8> Distinct();
        bool Any(Expression<Func<T, T2, T3, T4, T5, T6, T7,T8, bool>> expression);
        #endregion
    }

    #region 9-12
    public partial interface ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> : ISugarQueryable<T>
    {
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9> Hints(string hints);
        ISugarQueryable<TResult> SelectMergeTable<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8,T9,TResult>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9,T10> LeftJoinIF<T10>(bool isLeftJoin, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9,T10, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9,T10> InnerJoinIF<T10>(bool isJoin, Expression<Func<T, T2, T3, T4, T5, T6, T7,T8,T9,T10, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> LeftJoin<T10>(ISugarQueryable<T10> joinQueryable, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> InnerJoin<T10>(ISugarQueryable<T10> joinQueryable, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> RightJoin<T10>(ISugarQueryable<T10> joinQueryable, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> LeftJoin<T10>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9,T10, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> FullJoin<T10>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> InnerJoin<T10>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> RightJoin<T10>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> LeftJoin<T10>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> joinExpression,string tableName);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> FullJoin<T10>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> joinExpression,string tableName);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> InnerJoin<T10>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> joinExpression,string tableName);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> RightJoin<T10>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> joinExpression,string tableName);

        #region Where
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9> Where(string expShortName, FormattableString expressionString);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Where(Expression<Func<T, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Where(Expression<Func<T, T2, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Where(Expression<Func<T, T2, T3, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Where(Expression<Func<T, T2, T3, T4, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Where(Expression<Func<T, T2, T3, T4, T5, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Where(Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expression);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Where(List<IConditionalModel> conditionalModels);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7,T8,T9> Where(List<IConditionalModel> conditionalModels, bool isWrap);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> WhereIF(bool isWhere, Expression<Func<T, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> WhereIF(bool isWhere, Expression<Func<T, T2, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> WhereIF(bool isWhere, Expression<Func<T, T2, T3, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expression);

        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Where(string whereString, object parameters = null);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> WhereIF(bool isWhere, string whereString, object parameters = null);
        #endregion                              

        #region Select
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8,T9, TResult>> expression, bool isAutoFill);
        #endregion

        #region OrderBy
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderByDescending(Expression<Func<T, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderByDescending(Expression<Func<T, T2, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderByDescending(Expression<Func<T, T2, T3, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderByDescending(Expression<Func<T, T2, T3, T4, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9> OrderByDescending(Expression<Func<T, T2, T3, T4, T5, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderByDescending(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderByDescending(Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderByDescending(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderByDescending(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8,T9, object>> expression);

        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9> OrderBy(List<OrderByModel> models);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9> OrderByPropertyNameIF(bool isOrderBy, string orderPropertyName, OrderByType? orderByType = null);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9> OrderByPropertyName(string orderPropertyName, OrderByType? orderByType = null);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9> OrderBy(string orderByFields);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderBy(Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderBy(Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderBy(Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderBy(Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderBy(Expression<Func<T, T2, T3, T4, T5, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object>> expression, OrderByType type = OrderByType.Asc);

        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9> OrderByIF(bool isOrderBy, Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9> OrderByIF(bool isOrderBy, Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8,T9, object>> expression, OrderByType type = OrderByType.Asc);
        #endregion

        #region GroupBy
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> GroupBy(Expression<Func<T, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> GroupBy(Expression<Func<T, T2, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> GroupBy(Expression<Func<T, T2, T3, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> GroupBy(Expression<Func<T, T2, T3, T4, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> GroupBy(Expression<Func<T, T2, T3, T4, T5, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object>> expression);
        #endregion

        #region Aggr
        TResult Max<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression);
        TResult Min<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression);
        TResult Sum<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression);
        TResult Avg<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression);
        #endregion

        #region In
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> In<FieldType>(Expression<Func<T, object>> expression, params FieldType[] inValues);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> In<FieldType>(Expression<Func<T, object>> expression, List<FieldType> inValues);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> In<FieldType>(Expression<Func<T, object>> expression, ISugarQueryable<FieldType> childQueryExpression);
        #endregion

        #region Other
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Take(int num);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Clone();
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> AS<AsT>(string tableName);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> AS(string tableName);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9> ClearFilter();
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Filter(string FilterName, bool isDisabledGobalFilter = false);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> ClearFilter(params Type[] types);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9> ClearFilter<FilterType1>();
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> ClearFilter<FilterType1, FilterType2>();
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> ClearFilter<FilterType1, FilterType2, FilterType3>();
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> AddParameters(object parameters);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> AddParameters(SugarParameter[] parameters);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> AddParameters(List<SugarParameter> parameters);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> AddJoinInfo(string tableName, string shortName, string joinWhere, JoinType type = JoinType.Left);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> With(string withString);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> WithCache(int cacheDurationInSeconds = int.MaxValue);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> WithCacheIF(bool isCache, int cacheDurationInSeconds = int.MaxValue);
        #endregion                                       
    }
    public partial interface ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> : ISugarQueryable<T>
    {
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9,T10> Hints(string hints);
        ISugarQueryable<TResult> SelectMergeTable<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7,T8,T9,T10,TResult>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11> LeftJoinIF<T11>(bool isLeftJoin, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11> InnerJoinIF<T11>(bool isJoin, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> LeftJoin<T11>(ISugarQueryable<T11> joinQueryable, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> InnerJoin<T11>(ISugarQueryable<T11> joinQueryable, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> RightJoin<T11>(ISugarQueryable<T11> joinQueryable, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> LeftJoin<T11>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> FullJoin<T11>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> InnerJoin<T11>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> RightJoin<T11>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> LeftJoin<T11>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> joinExpression,string tableName);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> FullJoin<T11>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> joinExpression, string tableName);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> InnerJoin<T11>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> joinExpression, string tableName);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> RightJoin<T11>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> joinExpression, string tableName);

        #region Where
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9,T10> Where(string expShortName, FormattableString expressionString);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Where(Expression<Func<T, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Where(Expression<Func<T, T2, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Where(Expression<Func<T, T2, T3, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Where(Expression<Func<T, T2, T3, T4, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Where(Expression<Func<T, T2, T3, T4, T5, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Where(Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> expression);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Where(List<IConditionalModel> conditionalModels);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9,T10> Where(List<IConditionalModel> conditionalModels, bool isWrap);

        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> WhereIF(bool isWhere, Expression<Func<T, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> WhereIF(bool isWhere, Expression<Func<T, T2, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> WhereIF(bool isWhere, Expression<Func<T, T2, T3, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> expression);

        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Where(string whereString, object parameters = null);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> WhereIF(bool isWhere, string whereString, object parameters = null);
        #endregion                              

        #region Select
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8,T9,T10, TResult>> expression, bool isAutoFill);
        #endregion

        #region OrderBy
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderByDescending(Expression<Func<T, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderByDescending(Expression<Func<T, T2, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderByDescending(Expression<Func<T, T2, T3, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderByDescending(Expression<Func<T, T2, T3, T4, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9, T10> OrderByDescending(Expression<Func<T, T2, T3, T4, T5, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderByDescending(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderByDescending(Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderByDescending(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderByDescending(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderByDescending(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9,T10, object>> expression);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9,T10> OrderBy(List<OrderByModel> models);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9,T10> OrderByPropertyName(string orderPropertyName, OrderByType? orderByType = null);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9,T10> OrderByPropertyNameIF(bool isOrderBy, string orderPropertyName, OrderByType? orderByType = null);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9,T10> OrderBy(string orderByFields);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderBy(Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderBy(Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderBy(Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderBy(Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderBy(Expression<Func<T, T2, T3, T4, T5, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>> expression, OrderByType type = OrderByType.Asc);



        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9,T10> OrderByIF(bool isOrderBy, Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9,T10> OrderByIF(bool isOrderBy, Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9,T10> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9,T10> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9,T10> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9,T10> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9,T10> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9,T10> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9,T10> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9,T10, object>> expression, OrderByType type = OrderByType.Asc);
        #endregion

        #region GroupBy
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> GroupBy(Expression<Func<T, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> GroupBy(Expression<Func<T, T2, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> GroupBy(Expression<Func<T, T2, T3, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> GroupBy(Expression<Func<T, T2, T3, T4, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> GroupBy(Expression<Func<T, T2, T3, T4, T5, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>> expression);
        #endregion

        #region Aggr
        TResult Max<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> expression);
        TResult Min<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> expression);
        TResult Sum<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> expression);
        TResult Avg<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> expression);
        #endregion

        #region In
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> In<FieldType>(Expression<Func<T, object>> expression, params FieldType[] inValues);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> In<FieldType>(Expression<Func<T, object>> expression, List<FieldType> inValues);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> In<FieldType>(Expression<Func<T, object>> expression, ISugarQueryable<FieldType> childQueryExpression);
        #endregion

        #region Other
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9,T10> Take(int num);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Clone();
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> AS<AsT>(string tableName);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> AS(string tableName);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9,T10> ClearFilter();
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Filter(string FilterName, bool isDisabledGobalFilter = false);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> ClearFilter(params Type[] types);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9, T10> ClearFilter<FilterType1>();
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> ClearFilter<FilterType1, FilterType2>();
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> ClearFilter<FilterType1, FilterType2, FilterType3>();
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> AddParameters(object parameters);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> AddParameters(SugarParameter[] parameters);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> AddParameters(List<SugarParameter> parameters);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> AddJoinInfo(string tableName, string shortName, string joinWhere, JoinType type = JoinType.Left);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> With(string withString);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> WithCache(int cacheDurationInSeconds = int.MaxValue);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> WithCacheIF(bool isCache, int cacheDurationInSeconds = int.MaxValue);
        #endregion                                       
    }
    public partial interface ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> : ISugarQueryable<T>
    {
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11> Hints(string hints);
        ISugarQueryable<TResult> SelectMergeTable<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11, TResult>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> LeftJoinIF<T12>(bool isLeftJoin, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11,T12> InnerJoinIF<T12>(bool isJoin, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11,T12, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> LeftJoin<T12>(ISugarQueryable<T12> joinQueryable, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> InnerJoin<T12>(ISugarQueryable<T12> joinQueryable, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> RightJoin<T12>(ISugarQueryable<T12> joinQueryable, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> LeftJoin<T12>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> FullJoin<T12>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> InnerJoin<T12>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> RightJoin<T12>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> joinExpression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> LeftJoin<T12>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> joinExpression,string tableName);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> FullJoin<T12>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> joinExpression, string tableName);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> InnerJoin<T12>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> joinExpression, string tableName);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> RightJoin<T12>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> joinExpression, string tableName);

        #region Where
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11> Where(string expShortName, FormattableString expressionString);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Where(Expression<Func<T, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Where(Expression<Func<T, T2, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Where(Expression<Func<T, T2, T3, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Where(Expression<Func<T, T2, T3, T4, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Where(Expression<Func<T, T2, T3, T4, T5, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Where(Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> expression);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Where(List<IConditionalModel> conditionalModels);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11> Where(List<IConditionalModel> conditionalModels, bool isWrap);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> WhereIF(bool isWhere, Expression<Func<T, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> WhereIF(bool isWhere, Expression<Func<T, T2, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> WhereIF(bool isWhere, Expression<Func<T, T2, T3, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> expression);

        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Where(string whereString, object parameters = null);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> WhereIF(bool isWhere, string whereString, object parameters = null);
        #endregion                              

        #region Select
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11, TResult>> expression, bool isAutoFill);
        #endregion

        #region OrderBy
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11> OrderBy(List<OrderByModel> models);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11> OrderByPropertyNameIF(bool isOrderBy, string orderPropertyName, OrderByType? orderByType = null);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11> OrderByPropertyName(string orderPropertyName, OrderByType? orderByType = null);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11> OrderBy(string orderByFields);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderBy(Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderBy(Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderBy(Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderBy(Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderBy(Expression<Func<T, T2, T3, T4, T5, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, object>> expression, OrderByType type = OrderByType.Asc);

        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11> OrderByIF(bool isOrderBy, Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderByIF(bool isOrderBy, Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11, object>> expression, OrderByType type = OrderByType.Asc);
        #endregion

        #region GroupBy
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> GroupBy(Expression<Func<T, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> GroupBy(Expression<Func<T, T2, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> GroupBy(Expression<Func<T, T2, T3, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> GroupBy(Expression<Func<T, T2, T3, T4, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> GroupBy(Expression<Func<T, T2, T3, T4, T5, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, object>> expression);
        #endregion

        #region Aggr
        TResult Max<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>> expression);
        TResult Min<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>> expression);
        TResult Sum<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>> expression);
        TResult Avg<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>> expression);
        #endregion

        #region In
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> In<FieldType>(Expression<Func<T, object>> expression, params FieldType[] inValues);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> In<FieldType>(Expression<Func<T, object>> expression, List<FieldType> inValues);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> In<FieldType>(Expression<Func<T, object>> expression, ISugarQueryable<FieldType> childQueryExpression);
        #endregion

        #region Other
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9,T10,T11> Take(int num);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Clone();
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> AS<AsT>(string tableName);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> AS(string tableName);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11> ClearFilter();
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Filter(string FilterName, bool isDisabledGobalFilter = false);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> ClearFilter(params Type[] types);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> ClearFilter<FilterType1>();
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> ClearFilter<FilterType1, FilterType2>();
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> ClearFilter<FilterType1, FilterType2, FilterType3>();
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> AddParameters(object parameters);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> AddParameters(SugarParameter[] parameters);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> AddParameters(List<SugarParameter> parameters);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> AddJoinInfo(string tableName, string shortName, string joinWhere, JoinType type = JoinType.Left);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> With(string withString);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> WithCache(int cacheDurationInSeconds = int.MaxValue);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> WithCacheIF(bool isCache, int cacheDurationInSeconds = int.MaxValue);
        #endregion                                       
    }
    public partial interface ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> : ISugarQueryable<T>
    {
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> Hints(string hints);
        ISugarQueryable<TResult> SelectMergeTable<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12, TResult>> expression);
        #region Where
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> Where(string expShortName, FormattableString expressionString);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Where(Expression<Func<T, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Where(Expression<Func<T, T2, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Where(Expression<Func<T, T2, T3, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Where(Expression<Func<T, T2, T3, T4, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Where(Expression<Func<T, T2, T3, T4, T5, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Where(Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> expression);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Where(List<IConditionalModel> conditionalModels);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> Where(List<IConditionalModel> conditionalModels, bool isWrap);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> WhereIF(bool isWhere, Expression<Func<T, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> WhereIF(bool isWhere, Expression<Func<T, T2, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> WhereIF(bool isWhere, Expression<Func<T, T2, T3, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> expression);

        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Where(string whereString, object parameters = null);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> WhereIF(bool isWhere, string whereString, object parameters = null);
        #endregion                              

        #region Select
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>> expression, bool isAutoFill);
        #endregion

        #region OrderBy
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> OrderBy(List<OrderByModel> models);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> OrderByPropertyNameIF(bool isOrderBy, string orderPropertyName, OrderByType? orderByType = null);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> OrderByPropertyName(string orderPropertyName, OrderByType? orderByType = null);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> OrderBy(string orderByFields);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderBy(Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderBy(Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderBy(Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderBy(Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderBy(Expression<Func<T, T2, T3, T4, T5, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, object>> expression, OrderByType type = OrderByType.Asc);

        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> OrderByIF(bool isOrderBy, Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> OrderByIF(bool isOrderBy, Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12, object>> expression, OrderByType type = OrderByType.Asc);
        #endregion

        #region GroupBy
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> GroupBy(Expression<Func<T, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> GroupBy(Expression<Func<T, T2, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> GroupBy(Expression<Func<T, T2, T3, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> GroupBy(Expression<Func<T, T2, T3, T4, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> GroupBy(Expression<Func<T, T2, T3, T4, T5, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, object>> expression);
        #endregion

        #region Aggr
        TResult Max<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>> expression);
        TResult Min<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>> expression);
        TResult Sum<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>> expression);
        TResult Avg<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>> expression);
        #endregion

        #region In
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> In<FieldType>(Expression<Func<T, object>> expression, params FieldType[] inValues);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> In<FieldType>(Expression<Func<T, object>> expression, List<FieldType> inValues);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> In<FieldType>(Expression<Func<T, object>> expression, ISugarQueryable<FieldType> childQueryExpression);
        #endregion

        #region Other
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Take(int num);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Clone();
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> AS<AsT>(string tableName);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> AS(string tableName);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> ClearFilter();
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Filter(string FilterName, bool isDisabledGobalFilter = false);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> ClearFilter(params Type[] types);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> ClearFilter<FilterType1>();
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> ClearFilter<FilterType1, FilterType2>();
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> ClearFilter<FilterType1, FilterType2, FilterType3>();
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> AddParameters(object parameters);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> AddParameters(SugarParameter[] parameters);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> AddParameters(List<SugarParameter> parameters);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> AddJoinInfo(string tableName, string shortName, string joinWhere, JoinType type = JoinType.Left);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> With(string withString);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> WithCache(int cacheDurationInSeconds = int.MaxValue);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> WithCacheIF(bool isCache, int cacheDurationInSeconds = int.MaxValue);
        #endregion                                               
    }
    #endregion
}
