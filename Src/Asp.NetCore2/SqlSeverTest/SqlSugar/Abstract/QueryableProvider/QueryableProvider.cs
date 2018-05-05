using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Dynamic;
using System.Threading.Tasks;

namespace SqlSugar
{
    #region T1
    public partial class QueryableProvider<T> : QueryableAccessory, ISugarQueryable<T>
    {
        public SqlSugarClient Context { get; set; }
        public IAdo Db { get { return Context.Ado; } }
        public IDbBind Bind { get { return this.Db.DbBind; } }
        public ISqlBuilder SqlBuilder { get; set; }
        public MappingTableList OldMappingTableList { get; set; }
        public MappingTableList QueryableMappingTableList { get; set; }
        public bool IsCache { get; set; }
        public int CacheTime { get; set; }
        public bool IsAs { get; set; }
        public QueryBuilder QueryBuilder
        {
            get
            {
                return this.SqlBuilder.QueryBuilder;
            }
            set
            {
                this.SqlBuilder.QueryBuilder = value;
            }
        }
        public EntityInfo EntityInfo
        {
            get
            {
                return this.Context.EntityMaintenance.GetEntityInfo<T>();
            }
        }
        public void Clear()
        {
            QueryBuilder.Clear();
        }

        public virtual ISugarQueryable<T> AS<T2>(string tableName)
        {
            var entityName = typeof(T2).Name;
            return _As(tableName, entityName);
        }
        public ISugarQueryable<T> AS(string tableName)
        {
            var entityName = typeof(T).Name;
            return _As(tableName, entityName);
        }
        public virtual ISugarQueryable<T> With(string withString)
        {
            QueryBuilder.TableWithString = withString;
            return this;
        }

        public virtual ISugarQueryable<T> Filter(string FilterName, bool isDisabledGobalFilter = false)
        {
            _Filter(FilterName, isDisabledGobalFilter);
            return this;
        }

        public virtual ISugarQueryable<T> AddParameters(object parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(Context.Ado.GetParameters(parameters));
            return this;
        }
        public virtual ISugarQueryable<T> AddParameters(SugarParameter[] parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public virtual ISugarQueryable<T> AddParameters(List<SugarParameter> parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public virtual ISugarQueryable<T> AddParameters(SugarParameter parameter)
        {
            if (parameter != null)
                QueryBuilder.Parameters.Add(parameter);
            return this;
        }

        public virtual ISugarQueryable<T> AddJoinInfo(string tableName, string shortName, string joinWhere, JoinType type = JoinType.Left)
        {

            QueryBuilder.JoinIndex = +1;
            QueryBuilder.JoinQueryInfos
                .Add(new JoinQueryInfo()
                {
                    JoinIndex = QueryBuilder.JoinIndex,
                    TableName = tableName,
                    ShortName = shortName,
                    JoinType = type,
                    JoinWhere = joinWhere
                });
            return this;
        }
        public virtual ISugarQueryable<T> Where(Expression<Func<T, bool>> expression)
        {
            this._Where(expression);
            return this;
        }
        public virtual ISugarQueryable<T> Where(string whereString, object whereObj = null)
        {
            if (whereString.HasValue())
                this.Where<T>(whereString, whereObj);
            return this;
        }

        public virtual ISugarQueryable<T> Where(List<IConditionalModel> conditionalModels)
        {
            if (conditionalModels.IsNullOrEmpty()) return this;
            var sqlObj = this.Context.Utilities.ConditionalModelToSql(conditionalModels);
            return this.Where(sqlObj.Key, sqlObj.Value);
        }

        public virtual ISugarQueryable<T> Where<T2>(string whereString, object whereObj = null)
        {
            var whereValue = QueryBuilder.WhereInfos;
            whereValue.Add(SqlBuilder.AppendWhereOrAnd(whereValue.Count == 0, whereString + UtilConstants.Space));
            if (whereObj != null)
                QueryBuilder.Parameters.AddRange(Context.Ado.GetParameters(whereObj));
            return this;
        }

        public virtual ISugarQueryable<T> Having(Expression<Func<T, bool>> expression)
        {
            this._Having(expression);
            return this;
        }
        public virtual ISugarQueryable<T> Having(string whereString, object parameters = null)
        {

            QueryBuilder.HavingInfos = SqlBuilder.AppendHaving(whereString);
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(Context.Ado.GetParameters(parameters));
            return this;
        }

        public virtual ISugarQueryable<T> WhereIF(bool isWhere, Expression<Func<T, bool>> expression)
        {
            if (!isWhere) return this;
            _Where(expression);
            return this;
        }
        public virtual ISugarQueryable<T> WhereIF(bool isWhere, string whereString, object whereObj = null)
        {
            if (!isWhere) return this;
            this.Where<T>(whereString, whereObj);
            return this;
        }

        public virtual T InSingle(object pkValue)
        {
            Check.Exception(this.QueryBuilder.SelectValue.HasValue(), "'InSingle' and' Select' can't be used together,You can use .Select(it=>...).Single(it.id==1)");
            var list = In(pkValue).ToList();
            if (list == null) return default(T);
            else return list.SingleOrDefault();
        }
        public virtual ISugarQueryable<T> In<TParamter>(params TParamter[] pkValues)
        {
            if (pkValues == null || pkValues.Length == 0)
            {
                Where(SqlBuilder.SqlFalse);
                return this;
            }
            var pks = GetPrimaryKeys().Select(it => SqlBuilder.GetTranslationTableName(it)).ToList();
            Check.Exception(pks == null || pks.Count != 1, "Queryable.In(params object[] pkValues): Only one primary key");
            string filed = pks.FirstOrDefault();
            string shortName = QueryBuilder.TableShortName == null ? null : (QueryBuilder.TableShortName + ".");
            filed = shortName + filed;
            return In(filed, pkValues);
        }
        public virtual ISugarQueryable<T> In<FieldType>(string filed, params FieldType[] inValues)
        {
            if (inValues.Length == 1)
            {
                if (inValues.GetType().IsArray)
                {
                    var whereIndex = QueryBuilder.WhereIndex;
                    string parameterName = this.SqlBuilder.SqlParameterKeyWord + "InPara" + whereIndex;
                    this.AddParameters(new SugarParameter(parameterName, inValues[0]));
                    this.Where(string.Format(QueryBuilder.InTemplate, filed, parameterName));
                    QueryBuilder.WhereIndex++;
                }
                else
                {
                    var values = new List<object>();
                    foreach (var item in ((IEnumerable)inValues[0]))
                    {
                        if (item != null)
                        {
                            values.Add(item.ToString().ToSqlValue());
                        }
                    }
                    this.Where(string.Format(QueryBuilder.InTemplate, filed, string.Join(",", values)));
                }
            }
            else
            {
                var values = new List<object>();
                foreach (var item in inValues)
                {
                    if (item != null)
                    {
                        values.Add(item.ToString().ToSqlValue());
                    }
                }
                this.Where(string.Format(QueryBuilder.InTemplate, filed, string.Join(",", values)));

            }
            return this;
        }
        public virtual ISugarQueryable<T> In<FieldType>(Expression<Func<T, object>> expression, params FieldType[] inValues)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            return In(fieldName, inValues);
        }
        public virtual ISugarQueryable<T> In<TParamter>(List<TParamter> pkValues)
        {
            if (pkValues == null || pkValues.Count == 0)
            {
                Where(SqlBuilder.SqlFalse);
                return this;
            }
            return In(pkValues.ToArray());
        }
        public virtual ISugarQueryable<T> In<FieldType>(string InFieldName, List<FieldType> inValues)
        {
            if (inValues == null || inValues.Count == 0)
            {
                Where(SqlBuilder.SqlFalse);
                return this;
            }
            return In(InFieldName, inValues.ToArray());
        }
        public virtual ISugarQueryable<T> In<FieldType>(Expression<Func<T, object>> expression, List<FieldType> inValues)
        {
            if (inValues == null || inValues.Count == 0)
            {
                Where(SqlBuilder.SqlFalse);
                return this;
            }
            return In(expression, inValues.ToArray());
        }

        public virtual ISugarQueryable<T> In<FieldType>(Expression<Func<T, object>> expression, ISugarQueryable<FieldType> childQueryExpression)
        {
            var sqlObj = childQueryExpression.ToSql();
            _InQueryable(expression, sqlObj);
            return this;
        }
        public virtual ISugarQueryable<T> OrderBy(string orderFileds)
        {
            var orderByValue = QueryBuilder.OrderByValue;
            if (QueryBuilder.OrderByValue.IsNullOrEmpty())
            {
                QueryBuilder.OrderByValue = QueryBuilder.OrderByTemplate;
            }
            QueryBuilder.OrderByValue += string.IsNullOrEmpty(orderByValue) ? orderFileds : ("," + orderFileds);
            return this;
        }
        public virtual ISugarQueryable<T> OrderBy(Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc)
        {
            this._OrderBy(expression, type);
            return this;
        }
        public virtual ISugarQueryable<T> GroupBy(Expression<Func<T, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }

        public virtual ISugarQueryable<T> OrderByIF(bool isOrderBy, string orderFileds)
        {
            if (isOrderBy)
                return this.OrderBy(orderFileds);
            else
                return this;
        }
        public virtual ISugarQueryable<T> OrderByIF(bool isOrderBy, Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                return this.OrderBy(expression, type);
            else
                return this;
        }

        public virtual ISugarQueryable<T> GroupBy(string groupFileds)
        {
            var croupByValue = QueryBuilder.GroupByValue;
            if (QueryBuilder.GroupByValue.IsNullOrEmpty())
            {
                QueryBuilder.GroupByValue = QueryBuilder.GroupByTemplate;
            }
            QueryBuilder.GroupByValue += string.IsNullOrEmpty(croupByValue) ? groupFileds : ("," + groupFileds);
            return this;
        }

        public virtual ISugarQueryable<T> PartitionBy(Expression<Func<T, object>> expression)
        {
            if (QueryBuilder.Take == null)
                QueryBuilder.Take = 1;
            _PartitionBy(expression);
            return this;
        }
        public virtual ISugarQueryable<T> PartitionBy(string groupFileds)
        {
            var partitionByValue = QueryBuilder.PartitionByValue;
            if (QueryBuilder.PartitionByValue.IsNullOrEmpty())
            {
                QueryBuilder.PartitionByValue = QueryBuilder.PartitionByTemplate;
            }
            QueryBuilder.PartitionByValue += string.IsNullOrEmpty(partitionByValue) ? groupFileds : ("," + groupFileds);
            return this;
        }

        public virtual ISugarQueryable<T> Skip(int num)
        {
            QueryBuilder.Skip = num;
            return this;
        }
        public virtual ISugarQueryable<T> Take(int num)
        {
            QueryBuilder.Take = num;
            return this;
        }

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
            else if (result.Count == 2)
            {
                Check.Exception(true, ".Single()  result must not exceed one . You can use.First()");
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
            return this.Count() > 0;
        }

        public virtual ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public virtual ISugarQueryable<TResult> Select<TResult>(string selectValue)
        {
            var result = InstanceFactory.GetQueryable<TResult>(this.Context.CurrentConnectionConfig);
            result.Context = this.Context;
            result.SqlBuilder = this.SqlBuilder;
            QueryBuilder.SelectValue = selectValue;
            return result;
        }
        public virtual ISugarQueryable<T> Select(string selectValue)
        {
            QueryBuilder.SelectValue = selectValue;
            return this;
        }
        public virtual ISugarQueryable<T> MergeTable()
        {
            Check.Exception(this.QueryBuilder.SelectValue.IsNullOrEmpty(), "MergeTable need to use Queryable.Select Method .");
            Check.Exception(this.QueryBuilder.Skip > 0 || this.QueryBuilder.Take > 0 || this.QueryBuilder.OrderByValue.HasValue(), "MergeTable  Queryable cannot Take Skip OrderBy PageToList  ");
            ToSqlBefore();
            var sql = QueryBuilder.ToSqlString();
            var tableName = this.SqlBuilder.GetPackTable(sql, "MergeTable");
            var mergeQueryable = this.Context.Queryable<ExpandoObject>();
            mergeQueryable.QueryBuilder.Parameters = QueryBuilder.Parameters;
            mergeQueryable.QueryBuilder.WhereIndex = QueryBuilder.WhereIndex + 1;
            mergeQueryable.QueryBuilder.JoinIndex = QueryBuilder.JoinIndex + 1;
            mergeQueryable.QueryBuilder.LambdaExpressions.ParameterIndex = QueryBuilder.LambdaExpressions.ParameterIndex;
            return mergeQueryable.AS(tableName).Select<T>("*");
        }

        public virtual int Count()
        {
            InitMapping();
            QueryBuilder.IsCount = true;
            int result = 0;
            if (IsCache)
            {
                var cacheService = this.Context.CurrentConnectionConfig.ConfigureExternalServices.DataInfoCacheService;
                result = CacheSchemeMain.GetOrCreate<int>(cacheService, this.QueryBuilder, () => { return GetCount(); }, CacheTime, this.Context);
            }
            else
            {
                result = GetCount();
            }
            RestoreMapping();
            QueryBuilder.IsCount = false;
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
        public virtual string ToJson()
        {
            return this.Context.Utilities.SerializeObject(this.ToList());
        }
        public virtual string ToJsonPage(int pageIndex, int pageSize)
        {
            return this.Context.Utilities.SerializeObject(this.ToPageList(pageIndex, pageSize));
        }
        public virtual string ToJsonPage(int pageIndex, int pageSize, ref int totalNumber)
        {
            return this.Context.Utilities.SerializeObject(this.ToPageList(pageIndex, pageSize, ref totalNumber));
        }

        public virtual DataTable ToDataTable()
        {
            InitMapping();
            var sqlObj = this.ToSql();
            RestoreMapping();
            var result = this.Db.GetDataTable(sqlObj.Key, sqlObj.Value.ToArray());
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
        public virtual DataTable ToDataTablePage(int pageIndex, int pageSize, ref int totalNumber)
        {
            _RestoreMapping = false;
            totalNumber = this.Count();
            var result = ToDataTablePage(pageIndex, pageSize);
            _RestoreMapping = true;
            return result;
        }

        public virtual List<T> ToList()
        {
            InitMapping();
            return _ToList<T>();
        }
        public virtual List<T> ToPageList(int pageIndex, int pageSize)
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
            return ToList();
        }
        public virtual List<T> ToPageList(int pageIndex, int pageSize, ref int totalNumber)
        {
            _RestoreMapping = false;
            List<T> result = null;
            int count = this.Count();
            QueryBuilder.IsDisabledGobalFilter = UtilMethods.GetOldValue(QueryBuilder.IsDisabledGobalFilter, () =>
            {
                QueryBuilder.IsDisabledGobalFilter = true;
                if (count == 0)
                    result = new List<T>();
                else
                    result = ToPageList(pageIndex, pageSize);

            });
            totalNumber = count;
            _RestoreMapping = true;
            return result;
        }

        public virtual KeyValuePair<string, List<SugarParameter>> ToSql()
        {
            InitMapping();
            ToSqlBefore();
            string sql = QueryBuilder.ToSqlString();
            RestoreMapping();
            return new KeyValuePair<string, List<SugarParameter>>(sql, QueryBuilder.Parameters);
        }
        public ISugarQueryable<T> WithCache(int cacheDurationInSeconds = int.MaxValue)
        {
            Check.ArgumentNullException(this.Context.CurrentConnectionConfig.ConfigureExternalServices.DataInfoCacheService, "Use Cache ConnectionConfig.ConfigureExternalServices.DataInfoCacheService is required ");
            this.IsCache = true;
            this.CacheTime = cacheDurationInSeconds;
            return this;
        }
        public ISugarQueryable<T> WithCacheIF(bool isCache, int cacheDurationInSeconds = int.MaxValue)
        {
            if (IsCache)
            {
                this.IsCache = true;
                this.CacheTime = cacheDurationInSeconds;
            }
            return this;
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
        #region Async methods
        public Task<T> SingleAsync()
        {
            Task<T> result = new Task<T>(() =>
            {
                ISugarQueryable<T> asyncQueryable = CopyQueryable();
                return asyncQueryable.Single();
            });
            TaskStart(result);
            return result;
        }

        public Task<T> SingleAsync(Expression<Func<T, bool>> expression)
        {
            Task<T> result = new Task<T>(() =>
            {
                ISugarQueryable<T> asyncQueryable = CopyQueryable();
                return asyncQueryable.Single(expression);
            });
            TaskStart(result);
            return result;
        }

        public Task<T> FirstAsync()
        {
            Task<T> result = new Task<T>(() =>
            {
                ISugarQueryable<T> asyncQueryable = CopyQueryable();
                return asyncQueryable.First();
            });
            TaskStart(result);
            return result;
        }

        public Task<T> FirstAsync(Expression<Func<T, bool>> expression)
        {
            Task<T> result = new Task<T>(() =>
            {
                ISugarQueryable<T> asyncQueryable = CopyQueryable();
                return asyncQueryable.First(expression);
            });
            TaskStart(result);
            return result;
        }

        public Task<bool> AnyAsync(Expression<Func<T, bool>> expression)
        {
            Task<bool> result = new Task<bool>(() =>
            {
                ISugarQueryable<T> asyncQueryable = CopyQueryable();
                return asyncQueryable.Any(expression);
            });
            TaskStart(result);
            return result;
        }

        public Task<bool> AnyAsync()
        {
            Task<bool> result = new Task<bool>(() =>
            {
                ISugarQueryable<T> asyncQueryable = CopyQueryable();
                return asyncQueryable.Any();
            });
            TaskStart(result);
            return result;
        }

        public Task<int> CountAsync()
        {
            Task<int> result = new Task<int>(() =>
            {
                ISugarQueryable<T> asyncQueryable = CopyQueryable();
                return asyncQueryable.Count();
            });
            TaskStart(result);
            return result;
        }
        public Task<int> CountAsync(Expression<Func<T, bool>> expression)
        {
            Task<int> result = new Task<int>(() =>
            {
                ISugarQueryable<T> asyncQueryable = CopyQueryable();
                return asyncQueryable.Count(expression);
            });
            TaskStart(result); ;
            return result;
        }
        public Task<TResult> MaxAsync<TResult>(string maxField)
        {
            Task<TResult> result = new Task<TResult>(() =>
            {
                ISugarQueryable<T> asyncQueryable = CopyQueryable();
                return asyncQueryable.Max<TResult>(maxField);
            });
            TaskStart(result);
            return result;
        }

        public Task<TResult> MaxAsync<TResult>(Expression<Func<T, TResult>> expression)
        {
            Task<TResult> result = new Task<TResult>(() =>
            {
                ISugarQueryable<T> asyncQueryable = CopyQueryable();
                return asyncQueryable.Max<TResult>(expression);
            });
            TaskStart(result);
            return result;
        }

        public Task<TResult> MinAsync<TResult>(string minField)
        {
            Task<TResult> result = new Task<TResult>(() =>
            {
                ISugarQueryable<T> asyncQueryable = CopyQueryable();
                return asyncQueryable.Min<TResult>(minField);
            });
            TaskStart(result);
            return result;
        }

        public Task<TResult> MinAsync<TResult>(Expression<Func<T, TResult>> expression)
        {
            Task<TResult> result = new Task<TResult>(() =>
            {
                ISugarQueryable<T> asyncQueryable = CopyQueryable();
                return asyncQueryable.Min<TResult>(expression);
            });
            TaskStart(result);
            return result;
        }

        public Task<TResult> SumAsync<TResult>(string sumField)
        {
            Task<TResult> result = new Task<TResult>(() =>
            {
                ISugarQueryable<T> asyncQueryable = CopyQueryable();
                return asyncQueryable.Sum<TResult>(sumField);
            });
            TaskStart(result);
            return result;
        }

        public Task<TResult> SumAsync<TResult>(Expression<Func<T, TResult>> expression)
        {
            Task<TResult> result = new Task<TResult>(() =>
            {
                ISugarQueryable<T> asyncQueryable = CopyQueryable();
                return asyncQueryable.Sum<TResult>(expression);
            });
            TaskStart(result);
            return result;
        }

        public Task<TResult> AvgAsync<TResult>(string avgField)
        {
            Task<TResult> result = new Task<TResult>(() =>
            {
                ISugarQueryable<T> asyncQueryable = CopyQueryable();
                return asyncQueryable.Avg<TResult>(avgField);
            });
            TaskStart(result);
            return result;
        }

        public Task<TResult> AvgAsync<TResult>(Expression<Func<T, TResult>> expression)
        {
            Task<TResult> result = new Task<TResult>(() =>
            {
                ISugarQueryable<T> asyncQueryable = CopyQueryable();
                return asyncQueryable.Avg<TResult>(expression);
            });
            TaskStart(result);
            return result;
        }

        public Task<List<T>> ToListAsync()
        {
            Task<List<T>> result = new Task<List<T>>(() =>
            {
                ISugarQueryable<T> asyncQueryable = CopyQueryable();
                return asyncQueryable.ToList();
            });
            TaskStart(result);
            return result;
        }

        public Task<string> ToJsonAsync()
        {
            Task<string> result = new Task<string>(() =>
            {
                ISugarQueryable<T> asyncQueryable = CopyQueryable();
                return asyncQueryable.ToJson();
            });
            TaskStart(result);
            return result;
        }

        public Task<string> ToJsonPageAsync(int pageIndex, int pageSize)
        {
            Task<string> result = new Task<string>(() =>
            {
                ISugarQueryable<T> asyncQueryable = CopyQueryable();
                return asyncQueryable.ToJsonPage(pageIndex, pageSize);
            });
            TaskStart(result);
            return result;
        }

        public Task<KeyValuePair<string, int>> ToJsonPageAsync(int pageIndex, int pageSize, int totalNumber)
        {
            Task<KeyValuePair<string, int>> result = new Task<KeyValuePair<string, int>>(() =>
            {
                int totalNumberAsync = 0;
                ISugarQueryable<T> asyncQueryable = CopyQueryable();
                var list = asyncQueryable.ToJsonPage(pageIndex, pageSize, ref totalNumberAsync);
                return new KeyValuePair<string, int>(list, totalNumberAsync);
            });
            TaskStart(result);
            return result;
        }

        public Task<DataTable> ToDataTableAsync()
        {
            Task<DataTable> result = new Task<DataTable>(() =>
            {
                ISugarQueryable<T> asyncQueryable = CopyQueryable();
                return asyncQueryable.ToDataTable();
            });
            TaskStart(result);
            return result;
        }

        public Task<DataTable> ToDataTablePageAsync(int pageIndex, int pageSize)
        {
            Task<DataTable> result = new Task<DataTable>(() =>
            {
                ISugarQueryable<T> asyncQueryable = CopyQueryable();
                return asyncQueryable.ToDataTablePage(pageIndex, pageSize);
            });
            TaskStart(result);
            return result;
        }

        public Task<KeyValuePair<DataTable, int>> ToDataTablePageAsync(int pageIndex, int pageSize, int totalNumber)
        {
            Task<KeyValuePair<DataTable, int>> result = new Task<KeyValuePair<DataTable, int>>(() =>
            {
                int totalNumberAsync = 0;
                ISugarQueryable<T> asyncQueryable = CopyQueryable();
                var list = asyncQueryable.ToDataTablePage(pageIndex, pageSize, ref totalNumberAsync);
                return new KeyValuePair<DataTable, int>(list, totalNumberAsync);
            });
            TaskStart(result);
            return result;
        }

        public Task<List<T>> ToPageListAsync(int pageIndex, int pageSize)
        {
            Task<List<T>> result = new Task<List<T>>(() =>
            {
                ISugarQueryable<T> asyncQueryable = CopyQueryable();
                return asyncQueryable.ToPageList(pageIndex, pageSize);
            });
            TaskStart(result);
            return result;
        }

        public Task<KeyValuePair<List<T>, int>> ToPageListAsync(int pageIndex, int pageSize, int totalNumber)
        {
            Task<KeyValuePair<List<T>, int>> result = new Task<KeyValuePair<List<T>, int>>(() =>
            {
                int totalNumberAsync = 0;
                ISugarQueryable<T> asyncQueryable = CopyQueryable();
                var list = asyncQueryable.ToPageList(pageIndex, pageSize, ref totalNumberAsync);
                return new KeyValuePair<List<T>, int>(list, totalNumberAsync);
            });
            TaskStart(result);
            return result;
        }
        #endregion

        #region Private Methods
        private void TaskStart<Type>(Task<Type> result)
        {
            if (this.Context.CurrentConnectionConfig.IsShardSameThread)
            {
                Check.Exception(true, "IsShardSameThread=true can't be used async method");
            }
            result.Start();
        }
        protected ISugarQueryable<TResult> _Select<TResult>(Expression expression)
        {
            this.Context.InitMppingInfo<TResult>();
            var result = InstanceFactory.GetQueryable<TResult>(this.Context.CurrentConnectionConfig);
            result.Context = this.Context;
            result.SqlBuilder = this.SqlBuilder;
            result.SqlBuilder.QueryBuilder.Parameters = QueryBuilder.Parameters;
            result.SqlBuilder.QueryBuilder.SelectValue = expression;
            return result;
        }
        protected void _Where(Expression expression)
        {
            var isSingle = QueryBuilder.IsSingle();
            var result = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.WhereSingle : ResolveExpressType.WhereMultiple);
            QueryBuilder.WhereInfos.Add(SqlBuilder.AppendWhereOrAnd(QueryBuilder.WhereInfos.IsNullOrEmpty(), result.GetResultString()));
        }
        protected ISugarQueryable<T> _OrderBy(Expression expression, OrderByType type = OrderByType.Asc)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            OrderBy(lamResult.GetResultString() + UtilConstants.Space + type.ToString().ToUpper());
            return this;
        }
        protected ISugarQueryable<T> _GroupBy(Expression expression)
        {
            LambdaExpression lambda = expression as LambdaExpression;
            expression = lambda.Body;
            var isSingle = QueryBuilder.IsSingle();
            ExpressionResult lamResult = null;
            string result = null;
            if (expression is NewExpression)
            {
                lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.ArraySingle : ResolveExpressType.ArrayMultiple);
                result = string.Join(",", lamResult.GetResultArray().Select(it => it));
            }
            else
            {
                lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
                result = lamResult.GetResultString();
            }
            GroupBy(result);
            return this;
        }
        protected TResult _Min<TResult>(Expression expression)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var result= Min<TResult>(lamResult.GetResultString());
            QueryBuilder.SelectValue = null;
            return result;
        }
        protected TResult _Avg<TResult>(Expression expression)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            return Avg<TResult>(lamResult.GetResultString());
        }
        protected TResult _Max<TResult>(Expression expression)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var reslut= Max<TResult>(lamResult.GetResultString());
            QueryBuilder.SelectValue = null;
            return reslut;
        }
        protected TResult _Sum<TResult>(Expression expression)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var reslut= Sum<TResult>(lamResult.GetResultString());
            QueryBuilder.SelectValue = null;
            return reslut;
        }
        protected ISugarQueryable<T> _As(string tableName, string entityName)
        {
            IsAs = true;
            OldMappingTableList = this.Context.MappingTables;
            this.Context.MappingTables = this.Context.Utilities.TranslateCopy(this.Context.MappingTables);
            this.Context.MappingTables.Add(entityName, tableName);
            this.QueryableMappingTableList = this.Context.MappingTables;
            return this;
        }
        protected void _Filter(string FilterName, bool isDisabledGobalFilter)
        {
            QueryBuilder.IsDisabledGobalFilter = isDisabledGobalFilter;
            if (this.Context.QueryFilter.GeFilterList.HasValue() && FilterName.HasValue())
            {
                var list = this.Context.QueryFilter.GeFilterList.Where(it => it.FilterName == FilterName && it.IsJoinQuery == !QueryBuilder.IsSingle());
                foreach (var item in list)
                {
                    var filterResult = item.FilterValue(this.Context);
                    Where(filterResult.Sql, filterResult.Parameters);
                }
            }
        }
        public ISugarQueryable<T> _PartitionBy(Expression expression)
        {
            LambdaExpression lambda = expression as LambdaExpression;
            expression = lambda.Body;
            var isSingle = QueryBuilder.IsSingle();
            ExpressionResult lamResult = null;
            string result = null;
            if (expression is NewExpression)
            {
                lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.ArraySingle : ResolveExpressType.ArrayMultiple);
                result = string.Join(",", lamResult.GetResultArray());
            }
            else
            {
                lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
                result = lamResult.GetResultString();
            }
            PartitionBy(result);
            return this;
        }
        protected ISugarQueryable<T> _Having(Expression expression)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.WhereSingle : ResolveExpressType.WhereMultiple);
            Having(lamResult.GetResultString());
            return this;
        }
        protected List<TResult> _ToList<TResult>()
        {
            List<TResult> result = null;
            var sqlObj = this.ToSql();
            if (IsCache)
            {
                var cacheService = this.Context.CurrentConnectionConfig.ConfigureExternalServices.DataInfoCacheService;
                result = CacheSchemeMain.GetOrCreate<List<TResult>>(cacheService, this.QueryBuilder, () => { return GetData<TResult>(sqlObj); }, CacheTime, this.Context);
            }
            else
            {
                result = GetData<TResult>(sqlObj);
            }
            RestoreMapping();
            return result;
        }
        protected int GetCount()
        {
            var sql = string.Empty;
            ToSqlBefore();
            sql = QueryBuilder.ToSqlString();
            sql = QueryBuilder.ToCountSql(sql);
            var result = Context.Ado.GetInt(sql, QueryBuilder.Parameters.ToArray());
            return result;
        }

        private void ToSqlBefore()
        {
            var moreSetts = this.Context.CurrentConnectionConfig.MoreSettings;
            if (moreSetts != null && moreSetts.IsWithNoLockQuery && string.IsNullOrEmpty(QueryBuilder.TableWithString))
            {
                this.With(SqlWith.NoLock);
            }
        }

        protected List<TResult> GetData<TResult>(KeyValuePair<string, List<SugarParameter>> sqlObj)
        {
            List<TResult> result;
            var isComplexModel = QueryBuilder.IsComplexModel(sqlObj.Key);
            var entityType = typeof(TResult);
            var dataReader = this.Db.GetDataReader(sqlObj.Key, sqlObj.Value.ToArray());
            if (entityType == UtilConstants.DynamicType)
            {
                result = this.Context.Utilities.DataReaderToExpandoObjectList(dataReader) as List<TResult>;
            }
            else if (entityType == UtilConstants.ObjType)
            {
                result = this.Context.Utilities.DataReaderToExpandoObjectList(dataReader).Select(it => ((TResult)(object)it)).ToList();
            }
            else if (entityType.IsAnonymousType() || isComplexModel)
            {
                result = this.Context.Utilities.DataReaderToDynamicList<TResult>(dataReader);
            }
            else
            {
                result = this.Bind.DataReaderToList<TResult>(entityType, dataReader);
            }
            SetContextModel(result, entityType);
            return result;
        }

        protected void _InQueryable(Expression<Func<T, object>> expression, KeyValuePair<string, List<SugarParameter>> sqlObj)
        {
            string sql = sqlObj.Key;
            if (sqlObj.Value.HasValue())
            {
                this.SqlBuilder.RepairReplicationParameters(ref sql, sqlObj.Value.ToArray(), 100);
                this.QueryBuilder.Parameters.AddRange(sqlObj.Value);
            }
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            var whereSql = string.Format(this.QueryBuilder.InTemplate, fieldName, sql);
            this.QueryBuilder.WhereInfos.Add(SqlBuilder.AppendWhereOrAnd(this.QueryBuilder.WhereInfos.IsNullOrEmpty(), whereSql));
            base._InQueryableIndex += 100;
        }

        protected List<string> GetPrimaryKeys()
        {
            if (this.Context.IsSystemTablesConfig)
            {
                return this.Context.DbMaintenance.GetPrimaries(this.Context.EntityMaintenance.GetTableName(this.EntityInfo.EntityName));
            }
            else
            {
                return this.EntityInfo.Columns.Where(it => it.IsPrimarykey).Select(it => it.DbColumnName).ToList();
            }
        }
        protected virtual List<string> GetIdentityKeys()
        {
            if (this.Context.IsSystemTablesConfig)
            {
                return this.Context.DbMaintenance.GetIsIdentities(this.EntityInfo.DbTableName);
            }
            else
            {
                return this.EntityInfo.Columns.Where(it => it.IsIdentity).Select(it => it.DbColumnName).ToList();
            }
        }

        protected void RestoreMapping()
        {
            if (IsAs && _RestoreMapping)
            {
                this.Context.MappingTables = OldMappingTableList == null ? new MappingTableList() : OldMappingTableList;
            }
        }
        protected void InitMapping()
        {
            if (this.QueryableMappingTableList != null)
                this.Context.MappingTables = this.QueryableMappingTableList;
        }

        private void SetContextModel<TResult>(List<TResult> result, Type entityType)
        {
            if (result.HasValue())
            {
                if (UtilMethods.GetRootBaseType(entityType).HasValue() &&UtilMethods.GetRootBaseType(entityType) == UtilConstants.ModelType)
                {
                    foreach (var item in result)
                    {
                        var contextProperty = item.GetType().GetProperty("Context");
                        SqlSugarClient newClient = this.Context.Utilities.CopyContext();
                        contextProperty.SetValue(item, newClient, null);
                    }
                }
            }
        }
        private ISugarQueryable<T> CopyQueryable()
        {
            var asyncContext = this.Context.Utilities.CopyContext(true);
            asyncContext.CurrentConnectionConfig.IsAutoCloseConnection = true;

            var asyncQueryable = asyncContext.Queryable<ExpandoObject>().Select<T>(string.Empty);
            var asyncQueryableBuilder = asyncQueryable.QueryBuilder;
            asyncQueryableBuilder.Take = this.QueryBuilder.Take;
            asyncQueryableBuilder.Skip = this.QueryBuilder.Skip;
            asyncQueryableBuilder.SelectValue = this.QueryBuilder.SelectValue;
            asyncQueryableBuilder.WhereInfos = this.QueryBuilder.WhereInfos;
            asyncQueryableBuilder.EasyJoinInfos = this.QueryBuilder.EasyJoinInfos;
            asyncQueryableBuilder.JoinQueryInfos = this.QueryBuilder.JoinQueryInfos;
            asyncQueryableBuilder.WhereIndex = this.QueryBuilder.WhereIndex;
            asyncQueryableBuilder.EntityType = this.QueryBuilder.EntityType;
            asyncQueryableBuilder.EntityName = this.QueryBuilder.EntityName;
            asyncQueryableBuilder.Parameters = this.QueryBuilder.Parameters;
            asyncQueryableBuilder.TableShortName = this.QueryBuilder.TableShortName;
            asyncQueryableBuilder.TableWithString = this.QueryBuilder.TableWithString;
            asyncQueryableBuilder.GroupByValue = this.QueryBuilder.GroupByValue;
            asyncQueryableBuilder.OrderByValue = this.QueryBuilder.OrderByValue;
            return asyncQueryable;
        }
        #endregion
    }
    #endregion
    #region T2
    public partial class QueryableProvider<T, T2> : QueryableProvider<T>, ISugarQueryable<T, T2>
    {
        #region Where
        public new ISugarQueryable<T, T2> Where(Expression<Func<T, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2> Where(Expression<Func<T, T2, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public new ISugarQueryable<T, T2> WhereIF(bool isWhere, Expression<Func<T, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2> WhereIF(bool isWhere, Expression<Func<T, T2, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }
        public new ISugarQueryable<T, T2> Where(string whereString, object whereObj)
        {
            Where<T>(whereString, whereObj);
            return this;
        }
        public new ISugarQueryable<T, T2> Where(List<IConditionalModel> conditionalModels)
        {
            base.Where(conditionalModels);
            return this;
        }

        public new ISugarQueryable<T, T2> WhereIF(bool isWhere, string whereString, object whereObj)
        {
            if (!isWhere) return this;
            this.Where<T>(whereString, whereObj);
            return this;
        }
        #endregion

        #region Select
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }

        #endregion

        #region Order
        public new ISugarQueryable<T, T2> OrderBy(string orderFileds)
        {
            base.OrderBy(orderFileds);
            return this;
        }
        public ISugarQueryable<T, T2> OrderBy(Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }

        public new ISugarQueryable<T, T2> OrderBy(Expression<Func<T, object>> expression, OrderByType type)
        {
            _OrderBy(expression, type);
            return this;
        }
        public new ISugarQueryable<T, T2> OrderByIF(bool isOrderBy, string orderFileds)
        {
            if (isOrderBy)
                base.OrderBy(orderFileds);
            return this;
        }
        public new ISugarQueryable<T, T2> OrderByIF(bool isOrderBy, Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2> OrderByIF(bool isOrderBy, Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        #endregion

        #region GroupBy
        public new ISugarQueryable<T, T2> GroupBy(Expression<Func<T, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }

        public ISugarQueryable<T, T2> GroupBy(Expression<Func<T, T2, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }

        public new ISugarQueryable<T, T2> Having(Expression<Func<T, bool>> expression)
        {
            this._Having(expression);
            return this;
        }

        public ISugarQueryable<T, T2> Having(Expression<Func<T, T2, bool>> expression)
        {
            this._Having(expression);
            return this;
        }

        public new ISugarQueryable<T, T2> Having(string whereString, object whereObj)
        {
            base.Having(whereString, whereObj);
            return this;
        }
        #endregion

        #region Aggr
        public TResult Max<TResult>(Expression<Func<T, T2, TResult>> expression)
        {
            return _Max<TResult>(expression);
        }
        public TResult Min<TResult>(Expression<Func<T, T2, TResult>> expression)
        {
            return _Min<TResult>(expression);
        }
        public TResult Sum<TResult>(Expression<Func<T, T2, TResult>> expression)
        {
            return _Sum<TResult>(expression);
        }
        public TResult Avg<TResult>(Expression<Func<T, T2, TResult>> expression)
        {
            return _Avg<TResult>(expression);
        }
        #endregion

        #region In
        public new ISugarQueryable<T, T2> In<FieldType>(Expression<Func<T, object>> expression, params FieldType[] inValues)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public new ISugarQueryable<T, T2> In<FieldType>(Expression<Func<T, object>> expression, List<FieldType> inValues)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public new ISugarQueryable<T, T2> In<FieldType>(Expression<Func<T, object>> expression, ISugarQueryable<FieldType> childQueryExpression)
        {
            var sqlObj = childQueryExpression.ToSql();
            _InQueryable(expression, sqlObj);
            return this;
        }
        #endregion

        #region Other
        public new ISugarQueryable<T, T2> AS<AsT>(string tableName)
        {
            var entityName = typeof(AsT).Name;
            _As(tableName, entityName);
            return this;
        }
        public new ISugarQueryable<T, T2> AS(string tableName)
        {
            var entityName = typeof(T).Name;
            _As(tableName, entityName);
            return this;
        }
        public new ISugarQueryable<T, T2> Filter(string FilterName, bool isDisabledGobalFilter = false)
        {
            _Filter(FilterName, isDisabledGobalFilter);
            return this;
        }
        public new ISugarQueryable<T, T2> AddParameters(object parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(Context.Ado.GetParameters(parameters));
            return this;
        }
        public new ISugarQueryable<T, T2> AddParameters(SugarParameter[] parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public new ISugarQueryable<T, T2> AddParameters(List<SugarParameter> parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public new ISugarQueryable<T, T2> AddJoinInfo(string tableName, string shortName, string joinWhere, JoinType type = JoinType.Left)
        {
            QueryBuilder.JoinIndex = +1;
            QueryBuilder.JoinQueryInfos
                .Add(new JoinQueryInfo()
                {
                    JoinIndex = QueryBuilder.JoinIndex,
                    TableName = tableName,
                    ShortName = shortName,
                    JoinType = type,
                    JoinWhere = joinWhere
                });
            return this;
        }
        public new ISugarQueryable<T, T2> With(string withString)
        {
            base.With(withString);
            return this;
        }
        public new ISugarQueryable<T, T2> WithCache(int cacheDurationInSeconds = int.MaxValue)
        {
            this.IsCache = true;
            this.CacheTime = cacheDurationInSeconds;
            return this;
        }
        public new ISugarQueryable<T, T2> WithCacheIF(bool isCache, int cacheDurationInSeconds = int.MaxValue)
        {
            if (isCache)
            {
                this.IsCache = true;
                this.CacheTime = cacheDurationInSeconds;
            }
            return this;
        }
        #endregion
    }
    #endregion
    #region T3
    public partial class QueryableProvider<T, T2, T3> : QueryableProvider<T>, ISugarQueryable<T, T2, T3>
    {
        #region  Group 
        public ISugarQueryable<T, T2, T3> GroupBy(Expression<Func<T, T2, T3, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3> GroupBy(Expression<Func<T, T2, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public new ISugarQueryable<T, T2, T3> GroupBy(Expression<Func<T, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }

        public new ISugarQueryable<T, T2, T3> Having(Expression<Func<T, bool>> expression)
        {
            this._Having(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3> Having(Expression<Func<T, T2, bool>> expression)
        {
            this._Having(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3> Having(Expression<Func<T, T2, T3, bool>> expression)
        {
            this._Having(expression);
            return this;
        }

        public new ISugarQueryable<T, T2, T3> Having(string whereString, object whereObj)
        {
            base.Having(whereString, whereObj);
            return this;
        }
        #endregion

        #region Order
        public ISugarQueryable<T, T2, T3> OrderBy(Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }

        public ISugarQueryable<T, T2, T3> OrderBy(Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public new ISugarQueryable<T, T2, T3> OrderBy(Expression<Func<T, object>> expression, OrderByType type)
        {
            _OrderBy(expression, type);
            return this;
        }
        public new ISugarQueryable<T, T2, T3> OrderByIF(bool isOrderBy, string orderFileds)
        {
            if (isOrderBy)
                base.OrderBy(orderFileds);
            return this;
        }
        public new ISugarQueryable<T, T2, T3> OrderByIF(bool isOrderBy, Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3> OrderByIF(bool isOrderBy, Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        #endregion

        #region Select
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }

        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }

        public ISugarQueryable<T, T2, T3> Where(Expression<Func<T, T2, T3, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        #endregion

        #region Where
        public ISugarQueryable<T, T2, T3> Where(Expression<Func<T, T2, bool>> expression)
        {
            _Where(expression);
            return this;
        }

        public new ISugarQueryable<T, T2, T3> Where(Expression<Func<T, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public new ISugarQueryable<T, T2, T3> Where(List<IConditionalModel> conditionalModels)
        {
            base.Where(conditionalModels);
            return this;
        }

        public ISugarQueryable<T, T2, T3> WhereIF(bool isWhere, Expression<Func<T, T2, T3, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3> WhereIF(bool isWhere, Expression<Func<T, T2, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public new ISugarQueryable<T, T2, T3> WhereIF(bool isWhere, Expression<Func<T, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public new ISugarQueryable<T, T2, T3> Where(string whereString, object whereObj)
        {
            Where<T>(whereString, whereObj);
            return this;
        }

        public new ISugarQueryable<T, T2, T3> WhereIF(bool isWhere, string whereString, object whereObj)
        {
            if (!isWhere) return this;
            this.Where<T>(whereString, whereObj);
            return this;
        }
        #endregion

        #region Aggr
        public TResult Max<TResult>(Expression<Func<T, T2, T3, TResult>> expression)
        {
            return _Max<TResult>(expression);
        }
        public TResult Min<TResult>(Expression<Func<T, T2, T3, TResult>> expression)
        {
            return _Min<TResult>(expression);
        }
        public TResult Sum<TResult>(Expression<Func<T, T2, T3, TResult>> expression)
        {
            return _Sum<TResult>(expression);
        }
        public TResult Avg<TResult>(Expression<Func<T, T2, T3, TResult>> expression)
        {
            return _Avg<TResult>(expression);
        }
        #endregion

        #region In
        public new ISugarQueryable<T, T2, T3> In<FieldType>(Expression<Func<T, object>> expression, params FieldType[] inValues)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public new ISugarQueryable<T, T2, T3> In<FieldType>(Expression<Func<T, object>> expression, List<FieldType> inValues)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public new ISugarQueryable<T, T2, T3> In<FieldType>(Expression<Func<T, object>> expression, ISugarQueryable<FieldType> childQueryExpression)
        {
            var sqlObj = childQueryExpression.ToSql();
            _InQueryable(expression, sqlObj);
            return this;
        }
        #endregion

        #region Other
        public new ISugarQueryable<T, T2, T3> AS<AsT>(string tableName)
        {
            var entityName = typeof(AsT).Name;
            _As(tableName, entityName);
            return this;
        }
        public new ISugarQueryable<T, T2, T3> AS(string tableName)
        {
            var entityName = typeof(T).Name;
            _As(tableName, entityName);
            return this;
        }
        public new ISugarQueryable<T, T2, T3> Filter(string FilterName, bool isDisabledGobalFilter = false)
        {
            _Filter(FilterName, isDisabledGobalFilter);
            return this;
        }
        public new ISugarQueryable<T, T2, T3> AddParameters(object parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(Context.Ado.GetParameters(parameters));
            return this;
        }
        public new ISugarQueryable<T, T2, T3> AddParameters(SugarParameter[] parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public new ISugarQueryable<T, T2, T3> AddParameters(List<SugarParameter> parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public new ISugarQueryable<T, T2, T3> AddJoinInfo(string tableName, string shortName, string joinWhere, JoinType type = JoinType.Left)
        {
            QueryBuilder.JoinIndex = +1;
            QueryBuilder.JoinQueryInfos
                .Add(new JoinQueryInfo()
                {
                    JoinIndex = QueryBuilder.JoinIndex,
                    TableName = tableName,
                    ShortName = shortName,
                    JoinType = type,
                    JoinWhere = joinWhere
                });
            return this;
        }
        public new ISugarQueryable<T, T2, T3> With(string withString)
        {
            base.With(withString);
            return this;
        }
        public new ISugarQueryable<T, T2, T3> WithCache(int cacheDurationInSeconds = int.MaxValue)
        {
            this.IsCache = true;
            this.CacheTime = cacheDurationInSeconds;
            return this;
        }
        public new ISugarQueryable<T, T2, T3> WithCacheIF(bool isCache, int cacheDurationInSeconds = int.MaxValue)
        {
            if (IsCache)
            {
                this.IsCache = true;
                this.CacheTime = cacheDurationInSeconds;
            }
            return this;
        }
        #endregion
    }
    #endregion
    #region T4
    public partial class QueryableProvider<T, T2, T3, T4> : QueryableProvider<T>, ISugarQueryable<T, T2, T3, T4>
    {
        #region Where
        public new ISugarQueryable<T, T2, T3, T4> Where(Expression<Func<T, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4> Where(Expression<Func<T, T2, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4> Where(Expression<Func<T, T2, T3, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4> Where(Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4> Where(List<IConditionalModel> conditionalModels)
        {
            base.Where(conditionalModels);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4> WhereIF(bool isWhere, Expression<Func<T, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4> WhereIF(bool isWhere, Expression<Func<T, T2, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4> WhereIF(bool isWhere, Expression<Func<T, T2, T3, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4> Where(string whereString, object whereObj)
        {
            Where<T>(whereString, whereObj);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4> WhereIF(bool isWhere, string whereString, object whereObj)
        {
            if (!isWhere) return this;
            this.Where<T>(whereString, whereObj);
            return this;
        }
        #endregion

        #region Select
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        #endregion

        #region OrderBy
        public new ISugarQueryable<T, T2, T3, T4> OrderBy(Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4> OrderBy(Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4> OrderBy(Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4> OrderBy(Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4> OrderByIF(bool isOrderBy, string orderFileds)
        {
            if (isOrderBy)
                base.OrderBy(orderFileds);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4> OrderByIF(bool isOrderBy, Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4> OrderByIF(bool isOrderBy, Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        #endregion

        #region GroupBy
        public new ISugarQueryable<T, T2, T3, T4> GroupBy(Expression<Func<T, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4> GroupBy(Expression<Func<T, T2, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4> GroupBy(Expression<Func<T, T2, T3, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4> GroupBy(Expression<Func<T, T2, T3, T4, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4> Having(Expression<Func<T, bool>> expression)
        {
            this._Having(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4> Having(Expression<Func<T, T2, bool>> expression)
        {
            this._Having(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4> Having(Expression<Func<T, T2, T3, bool>> expression)
        {
            this._Having(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4> Having(Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            this._Having(expression);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4> Having(string whereString, object whereObj)
        {
            base.Having(whereString, whereObj);
            return this;
        }
        #endregion

        #region Aggr
        public TResult Max<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression)
        {
            return _Max<TResult>(expression);
        }
        public TResult Min<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression)
        {
            return _Min<TResult>(expression);
        }
        public TResult Sum<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression)
        {
            return _Sum<TResult>(expression);
        }
        public TResult Avg<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression)
        {
            return _Avg<TResult>(expression);
        }
        #endregion

        #region In
        public new ISugarQueryable<T, T2, T3, T4> In<FieldType>(Expression<Func<T, object>> expression, params FieldType[] inValues)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4> In<FieldType>(Expression<Func<T, object>> expression, List<FieldType> inValues)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4> In<FieldType>(Expression<Func<T, object>> expression, ISugarQueryable<FieldType> childQueryExpression)
        {
            var sqlObj = childQueryExpression.ToSql();
            _InQueryable(expression, sqlObj);
            return this;
        }
        #endregion

        #region Other
        public new ISugarQueryable<T, T2, T3, T4> AS<AsT>(string tableName)
        {
            var entityName = typeof(AsT).Name;
            _As(tableName, entityName);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4> AS(string tableName)
        {
            var entityName = typeof(T).Name;
            _As(tableName, entityName);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4> Filter(string FilterName, bool isDisabledGobalFilter = false)
        {
            _Filter(FilterName, isDisabledGobalFilter);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4> AddParameters(object parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(Context.Ado.GetParameters(parameters));
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4> AddParameters(SugarParameter[] parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4> AddParameters(List<SugarParameter> parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4> AddJoinInfo(string tableName, string shortName, string joinWhere, JoinType type = JoinType.Left)
        {
            QueryBuilder.JoinIndex = +1;
            QueryBuilder.JoinQueryInfos
                .Add(new JoinQueryInfo()
                {
                    JoinIndex = QueryBuilder.JoinIndex,
                    TableName = tableName,
                    ShortName = shortName,
                    JoinType = type,
                    JoinWhere = joinWhere
                });
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4> With(string withString)
        {
            base.With(withString);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4> WithCache(int cacheDurationInSeconds = int.MaxValue)
        {
            this.IsCache = true;
            this.CacheTime = cacheDurationInSeconds;
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4> WithCacheIF(bool isCache, int cacheDurationInSeconds = int.MaxValue)
        {
            if (IsCache)
            {
                this.IsCache = true;
                this.CacheTime = cacheDurationInSeconds;
            }
            return this;
        }
        #endregion
    }
    #endregion
    #region T5
    public partial class QueryableProvider<T, T2, T3, T4, T5> : QueryableProvider<T>, ISugarQueryable<T, T2, T3, T4, T5>
    {
        #region Where
        public new ISugarQueryable<T, T2, T3, T4, T5> Where(Expression<Func<T, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5> Where(Expression<Func<T, T2, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5> Where(Expression<Func<T, T2, T3, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5> Where(Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5> Where(Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5> Where(List<IConditionalModel> conditionalModels)
        {
            base.Where(conditionalModels);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4, T5> WhereIF(bool isWhere, Expression<Func<T, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5> WhereIF(bool isWhere, Expression<Func<T, T2, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5> WhereIF(bool isWhere, Expression<Func<T, T2, T3, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4, T5> Where(string whereString, object whereObj)
        {
            Where<T>(whereString, whereObj);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4, T5> WhereIF(bool isWhere, string whereString, object whereObj)
        {
            if (!isWhere) return this;
            this.Where<T>(whereString, whereObj);
            return this;
        }
        #endregion

        #region Select
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        #endregion

        #region OrderBy
        public new ISugarQueryable<T, T2, T3, T4, T5> OrderBy(Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5> OrderBy(Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5> OrderBy(Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5> OrderBy(Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5> OrderBy(Expression<Func<T, T2, T3, T4, T5, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5> OrderByIF(bool isOrderBy, string orderFileds)
        {
            if (isOrderBy)
                base.OrderBy(orderFileds);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5> OrderByIF(bool isOrderBy, Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5> OrderByIF(bool isOrderBy, Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        #endregion

        #region GroupBy
        public new ISugarQueryable<T, T2, T3, T4, T5> GroupBy(Expression<Func<T, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5> GroupBy(Expression<Func<T, T2, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5> GroupBy(Expression<Func<T, T2, T3, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5> GroupBy(Expression<Func<T, T2, T3, T4, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5> GroupBy(Expression<Func<T, T2, T3, T4, T5, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        #endregion

        #region Aggr
        public TResult Max<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression)
        {
            return _Max<TResult>(expression);
        }
        public TResult Min<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression)
        {
            return _Min<TResult>(expression);
        }
        public TResult Sum<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression)
        {
            return _Sum<TResult>(expression);
        }
        public TResult Avg<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression)
        {
            return _Avg<TResult>(expression);
        }
        #endregion

        #region In
        public new ISugarQueryable<T, T2, T3, T4, T5> In<FieldType>(Expression<Func<T, object>> expression, params FieldType[] inValues)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5> In<FieldType>(Expression<Func<T, object>> expression, List<FieldType> inValues)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5> In<FieldType>(Expression<Func<T, object>> expression, ISugarQueryable<FieldType> childQueryExpression)
        {
            var sqlObj = childQueryExpression.ToSql();
            _InQueryable(expression, sqlObj);
            return this;
        }
        #endregion

        #region Other
        public new ISugarQueryable<T, T2, T3, T4, T5> AS<AsT>(string tableName)
        {
            var entityName = typeof(AsT).Name;
            _As(tableName, entityName);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5> AS(string tableName)
        {
            var entityName = typeof(T).Name;
            _As(tableName, entityName);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5> Filter(string FilterName, bool isDisabledGobalFilter = false)
        {
            _Filter(FilterName, isDisabledGobalFilter);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5> AddParameters(object parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(Context.Ado.GetParameters(parameters));
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5> AddParameters(SugarParameter[] parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5> AddParameters(List<SugarParameter> parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5> AddJoinInfo(string tableName, string shortName, string joinWhere, JoinType type = JoinType.Left)
        {
            QueryBuilder.JoinIndex = +1;
            QueryBuilder.JoinQueryInfos
                .Add(new JoinQueryInfo()
                {
                    JoinIndex = QueryBuilder.JoinIndex,
                    TableName = tableName,
                    ShortName = shortName,
                    JoinType = type,
                    JoinWhere = joinWhere
                });
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5> With(string withString)
        {
            base.With(withString);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5> WithCache(int cacheDurationInSeconds = int.MaxValue)
        {
            this.IsCache = true;
            this.CacheTime = cacheDurationInSeconds;
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5> WithCacheIF(bool isCache, int cacheDurationInSeconds = int.MaxValue)
        {
            if (IsCache)
            {
                this.IsCache = true;
                this.CacheTime = cacheDurationInSeconds;
            }
            return this;
        }
        #endregion
    }
    #endregion
    #region T6
    public partial class QueryableProvider<T, T2, T3, T4, T5, T6> : QueryableProvider<T>, ISugarQueryable<T, T2, T3, T4, T5, T6>
    {
        #region Where
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> Where(Expression<Func<T, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6> Where(Expression<Func<T, T2, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6> Where(Expression<Func<T, T2, T3, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6> Where(Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6> Where(Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6> Where(Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> Where(List<IConditionalModel> conditionalModels)
        {
            base.Where(conditionalModels);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4, T5, T6> WhereIF(bool isWhere, Expression<Func<T, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6> WhereIF(bool isWhere, Expression<Func<T, T2, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6> WhereIF(bool isWhere, Expression<Func<T, T2, T3, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4, T5, T6> Where(string whereString, object whereObj)
        {
            Where<T>(whereString, whereObj);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4, T5, T6> WhereIF(bool isWhere, string whereString, object whereObj)
        {
            if (!isWhere) return this;
            this.Where<T>(whereString, whereObj);
            return this;
        }
        #endregion

        #region Select
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        #endregion

        #region OrderBy
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> OrderBy(Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6> OrderBy(Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6> OrderBy(Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6> OrderBy(Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6> OrderBy(Expression<Func<T, T2, T3, T4, T5, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        #endregion

        #region GroupBy
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> GroupBy(Expression<Func<T, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6> GroupBy(Expression<Func<T, T2, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6> GroupBy(Expression<Func<T, T2, T3, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6> GroupBy(Expression<Func<T, T2, T3, T4, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6> GroupBy(Expression<Func<T, T2, T3, T4, T5, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        #endregion

        #region Aggr
        public TResult Max<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression)
        {
            return _Max<TResult>(expression);
        }
        public TResult Min<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression)
        {
            return _Min<TResult>(expression);
        }
        public TResult Sum<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression)
        {
            return _Sum<TResult>(expression);
        }
        public TResult Avg<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression)
        {
            return _Avg<TResult>(expression);
        }
        #endregion

        #region In
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> In<FieldType>(Expression<Func<T, object>> expression, params FieldType[] inValues)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> In<FieldType>(Expression<Func<T, object>> expression, List<FieldType> inValues)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> In<FieldType>(Expression<Func<T, object>> expression, ISugarQueryable<FieldType> childQueryExpression)
        {
            var sqlObj = childQueryExpression.ToSql();
            _InQueryable(expression, sqlObj);
            return this;
        }
        #endregion

        #region Other
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> AS<AsT>(string tableName)
        {
            var entityName = typeof(AsT).Name;
            _As(tableName, entityName);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> AS(string tableName)
        {
            var entityName = typeof(T).Name;
            _As(tableName, entityName);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> Filter(string FilterName, bool isDisabledGobalFilter = false)
        {
            _Filter(FilterName, isDisabledGobalFilter);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> AddParameters(object parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(Context.Ado.GetParameters(parameters));
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> AddParameters(SugarParameter[] parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> AddParameters(List<SugarParameter> parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> AddJoinInfo(string tableName, string shortName, string joinWhere, JoinType type = JoinType.Left)
        {
            QueryBuilder.JoinIndex = +1;
            QueryBuilder.JoinQueryInfos
                .Add(new JoinQueryInfo()
                {
                    JoinIndex = QueryBuilder.JoinIndex,
                    TableName = tableName,
                    ShortName = shortName,
                    JoinType = type,
                    JoinWhere = joinWhere
                });
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> With(string withString)
        {
            base.With(withString);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> WithCache(int cacheDurationInSeconds = int.MaxValue)
        {
            this.IsCache = true;
            this.CacheTime = cacheDurationInSeconds;
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> WithCacheIF(bool isCache, int cacheDurationInSeconds = int.MaxValue)
        {
            if (IsCache)
            {
                this.IsCache = true;
                this.CacheTime = cacheDurationInSeconds;
            }
            return this;
        }
        #endregion
    }
    #endregion
    #region T7
    public partial class QueryableProvider<T, T2, T3, T4, T5, T6, T7> : QueryableProvider<T>, ISugarQueryable<T, T2, T3, T4, T5, T6, T7>
    {
        #region Where
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Where(Expression<Func<T, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Where(Expression<Func<T, T2, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Where(Expression<Func<T, T2, T3, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Where(Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Where(Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Where(Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Where(List<IConditionalModel> conditionalModels)
        {
            base.Where(conditionalModels);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> WhereIF(bool isWhere, Expression<Func<T, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> WhereIF(bool isWhere, Expression<Func<T, T2, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> WhereIF(bool isWhere, Expression<Func<T, T2, T3, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Where(string whereString, object whereObj)
        {
            Where<T>(whereString, whereObj);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> WhereIF(bool isWhere, string whereString, object whereObj)
        {
            if (!isWhere) return this;
            this.Where<T>(whereString, whereObj);
            return this;
        }
        #endregion

        #region Select
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        #endregion

        #region OrderBy
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> OrderBy(Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> OrderBy(Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> OrderBy(Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> OrderBy(Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> OrderBy(Expression<Func<T, T2, T3, T4, T5, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        #endregion

        #region GroupBy
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> GroupBy(Expression<Func<T, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> GroupBy(Expression<Func<T, T2, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> GroupBy(Expression<Func<T, T2, T3, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> GroupBy(Expression<Func<T, T2, T3, T4, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> GroupBy(Expression<Func<T, T2, T3, T4, T5, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }

        #endregion

        #region Aggr
        public TResult Max<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> expression)
        {
            return _Max<TResult>(expression);
        }
        public TResult Min<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> expression)
        {
            return _Min<TResult>(expression);
        }
        public TResult Sum<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> expression)
        {
            return _Sum<TResult>(expression);
        }
        public TResult Avg<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> expression)
        {
            return _Avg<TResult>(expression);
        }
        #endregion

        #region In
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> In<FieldType>(Expression<Func<T, object>> expression, params FieldType[] inValues)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> In<FieldType>(Expression<Func<T, object>> expression, List<FieldType> inValues)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> In<FieldType>(Expression<Func<T, object>> expression, ISugarQueryable<FieldType> childQueryExpression)
        {
            var sqlObj = childQueryExpression.ToSql();
            _InQueryable(expression, sqlObj);
            return this;
        }
        #endregion

        #region Other
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> AS<AsT>(string tableName)
        {
            var entityName = typeof(AsT).Name;
            _As(tableName, entityName);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> AS(string tableName)
        {
            var entityName = typeof(T).Name;
            _As(tableName, entityName);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Filter(string FilterName, bool isDisabledGobalFilter = false)
        {
            _Filter(FilterName, isDisabledGobalFilter);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> AddParameters(object parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(Context.Ado.GetParameters(parameters));
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> AddParameters(SugarParameter[] parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> AddParameters(List<SugarParameter> parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> AddJoinInfo(string tableName, string shortName, string joinWhere, JoinType type = JoinType.Left)
        {
            QueryBuilder.JoinIndex = +1;
            QueryBuilder.JoinQueryInfos
                .Add(new JoinQueryInfo()
                {
                    JoinIndex = QueryBuilder.JoinIndex,
                    TableName = tableName,
                    ShortName = shortName,
                    JoinType = type,
                    JoinWhere = joinWhere
                });
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> With(string withString)
        {
            base.With(withString);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> WithCache(int cacheDurationInSeconds = int.MaxValue)
        {
            this.IsCache = true;
            this.CacheTime = cacheDurationInSeconds;
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> WithCacheIF(bool isCache, int cacheDurationInSeconds = int.MaxValue)
        {
            if (IsCache)
            {
                this.IsCache = true;
                this.CacheTime = cacheDurationInSeconds;
            }
            return this;
        }
        #endregion
    }
    #endregion
    #region T8
    public partial class QueryableProvider<T, T2, T3, T4, T5, T6, T7, T8> : QueryableProvider<T>, ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8>
    {
        #region Where
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Where(Expression<Func<T, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Where(Expression<Func<T, T2, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Where(Expression<Func<T, T2, T3, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Where(Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Where(Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Where(Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Where(List<IConditionalModel> conditionalModels)
        {
            base.Where(conditionalModels);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> WhereIF(bool isWhere, Expression<Func<T, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> WhereIF(bool isWhere, Expression<Func<T, T2, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> WhereIF(bool isWhere, Expression<Func<T, T2, T3, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Where(string whereString, object whereObj)
        {
            Where<T>(whereString, whereObj);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> WhereIF(bool isWhere, string whereString, object whereObj)
        {
            if (!isWhere) return this;
            this.Where<T>(whereString, whereObj);
            return this;
        }
        #endregion

        #region Select
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        #endregion

        #region OrderBy
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> OrderBy(Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> OrderBy(Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> OrderBy(Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> OrderBy(Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> OrderBy(Expression<Func<T, T2, T3, T4, T5, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        #endregion

        #region GroupBy
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> GroupBy(Expression<Func<T, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> GroupBy(Expression<Func<T, T2, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> GroupBy(Expression<Func<T, T2, T3, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> GroupBy(Expression<Func<T, T2, T3, T4, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> GroupBy(Expression<Func<T, T2, T3, T4, T5, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }

        #endregion

        #region Aggr
        public TResult Max<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> expression)
        {
            return _Max<TResult>(expression);
        }
        public TResult Min<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> expression)
        {
            return _Min<TResult>(expression);
        }
        public TResult Sum<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> expression)
        {
            return _Sum<TResult>(expression);
        }
        public TResult Avg<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> expression)
        {
            return _Avg<TResult>(expression);
        }
        #endregion

        #region In
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> In<FieldType>(Expression<Func<T, object>> expression, params FieldType[] inValues)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> In<FieldType>(Expression<Func<T, object>> expression, List<FieldType> inValues)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> In<FieldType>(Expression<Func<T, object>> expression, ISugarQueryable<FieldType> childQueryExpression)
        {
            var sqlObj = childQueryExpression.ToSql();
            _InQueryable(expression, sqlObj);
            return this;
        }
        #endregion

        #region Other
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> AS<AsT>(string tableName)
        {
            var entityName = typeof(AsT).Name;
            _As(tableName, entityName);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> AS(string tableName)
        {
            var entityName = typeof(T).Name;
            _As(tableName, entityName);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Filter(string FilterName, bool isDisabledGobalFilter = false)
        {
            _Filter(FilterName, isDisabledGobalFilter);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> AddParameters(object parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(Context.Ado.GetParameters(parameters));
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> AddParameters(SugarParameter[] parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> AddParameters(List<SugarParameter> parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> AddJoinInfo(string tableName, string shortName, string joinWhere, JoinType type = JoinType.Left)
        {
            QueryBuilder.JoinIndex = +1;
            QueryBuilder.JoinQueryInfos
                .Add(new JoinQueryInfo()
                {
                    JoinIndex = QueryBuilder.JoinIndex,
                    TableName = tableName,
                    ShortName = shortName,
                    JoinType = type,
                    JoinWhere = joinWhere
                });
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> With(string withString)
        {
            base.With(withString);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> WithCache(int cacheDurationInSeconds = int.MaxValue)
        {
            this.IsCache = true;
            this.CacheTime = cacheDurationInSeconds;
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> WithCacheIF(bool isCache, int cacheDurationInSeconds = int.MaxValue)
        {
            if (IsCache)
            {
                this.IsCache = true;
                this.CacheTime = cacheDurationInSeconds;
            }
            return this;
        }
        #endregion
    }
    #endregion
    #region T9
    public partial class QueryableProvider<T, T2, T3, T4, T5, T6, T7, T8, T9> : QueryableProvider<T>, ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9>
    {
        #region Where
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Where(Expression<Func<T, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Where(Expression<Func<T, T2, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Where(Expression<Func<T, T2, T3, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Where(Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Where(Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Where(Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> WhereIF(bool isWhere, Expression<Func<T, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Where(List<IConditionalModel> conditionalModels)
        {
            base.Where(conditionalModels);
            return this;
        }


        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> WhereIF(bool isWhere, Expression<Func<T, T2, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> WhereIF(bool isWhere, Expression<Func<T, T2, T3, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Where(string whereString, object whereObj)
        {
            Where<T>(whereString, whereObj);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> WhereIF(bool isWhere, string whereString, object whereObj)
        {
            if (!isWhere) return this;
            this.Where<T>(whereString, whereObj);
            return this;
        }
        #endregion

        #region Select
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        #endregion

        #region OrderBy
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderBy(Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderBy(Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderBy(Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderBy(Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderBy(Expression<Func<T, T2, T3, T4, T5, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        #endregion

        #region GroupBy
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> GroupBy(Expression<Func<T, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> GroupBy(Expression<Func<T, T2, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> GroupBy(Expression<Func<T, T2, T3, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> GroupBy(Expression<Func<T, T2, T3, T4, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> GroupBy(Expression<Func<T, T2, T3, T4, T5, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        #endregion

        #region Aggr
        public TResult Max<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression)
        {
            return _Max<TResult>(expression);
        }
        public TResult Min<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression)
        {
            return _Min<TResult>(expression);
        }
        public TResult Sum<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression)
        {
            return _Sum<TResult>(expression);
        }
        public TResult Avg<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression)
        {
            return _Avg<TResult>(expression);
        }
        #endregion

        #region In
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> In<FieldType>(Expression<Func<T, object>> expression, params FieldType[] inValues)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> In<FieldType>(Expression<Func<T, object>> expression, List<FieldType> inValues)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> In<FieldType>(Expression<Func<T, object>> expression, ISugarQueryable<FieldType> childQueryExpression)
        {
            var sqlObj = childQueryExpression.ToSql();
            _InQueryable(expression, sqlObj);
            return this;
        }
        #endregion

        #region Other
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> AS<AsT>(string tableName)
        {
            var entityName = typeof(AsT).Name;
            _As(tableName, entityName);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> AS(string tableName)
        {
            var entityName = typeof(T).Name;
            _As(tableName, entityName);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Filter(string FilterName, bool isDisabledGobalFilter = false)
        {
            _Filter(FilterName, isDisabledGobalFilter);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> AddParameters(object parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(Context.Ado.GetParameters(parameters));
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> AddParameters(SugarParameter[] parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> AddParameters(List<SugarParameter> parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> AddJoinInfo(string tableName, string shortName, string joinWhere, JoinType type = JoinType.Left)
        {
            QueryBuilder.JoinIndex = +1;
            QueryBuilder.JoinQueryInfos
                .Add(new JoinQueryInfo()
                {
                    JoinIndex = QueryBuilder.JoinIndex,
                    TableName = tableName,
                    ShortName = shortName,
                    JoinType = type,
                    JoinWhere = joinWhere
                });
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> With(string withString)
        {
            base.With(withString);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> WithCache(int cacheDurationInSeconds = int.MaxValue)
        {
            this.IsCache = true;
            this.CacheTime = cacheDurationInSeconds;
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> WithCacheIF(bool isCache, int cacheDurationInSeconds = int.MaxValue)
        {
            if (IsCache)
            {
                this.IsCache = true;
                this.CacheTime = cacheDurationInSeconds;
            }
            return this;
        }
        #endregion
    }
    #endregion
    #region T10
    public partial class QueryableProvider<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> : QueryableProvider<T>, ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>
    {
        #region Where
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Where(Expression<Func<T, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Where(Expression<Func<T, T2, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Where(Expression<Func<T, T2, T3, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Where(Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Where(Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Where(Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Where(List<IConditionalModel> conditionalModels)
        {
            base.Where(conditionalModels);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> WhereIF(bool isWhere, Expression<Func<T, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> WhereIF(bool isWhere, Expression<Func<T, T2, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> WhereIF(bool isWhere, Expression<Func<T, T2, T3, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Where(string whereString, object whereObj)
        {
            Where<T>(whereString, whereObj);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> WhereIF(bool isWhere, string whereString, object whereObj)
        {
            if (!isWhere) return this;
            this.Where<T>(whereString, whereObj);
            return this;
        }
        #endregion

        #region Select
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        #endregion

        #region OrderBy
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderBy(Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderBy(Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderBy(Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderBy(Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderBy(Expression<Func<T, T2, T3, T4, T5, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        #endregion

        #region GroupBy
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> GroupBy(Expression<Func<T, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> GroupBy(Expression<Func<T, T2, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> GroupBy(Expression<Func<T, T2, T3, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> GroupBy(Expression<Func<T, T2, T3, T4, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> GroupBy(Expression<Func<T, T2, T3, T4, T5, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        #endregion

        #region Aggr
        public TResult Max<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> expression)
        {
            return _Max<TResult>(expression);
        }
        public TResult Min<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> expression)
        {
            return _Min<TResult>(expression);
        }
        public TResult Sum<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> expression)
        {
            return _Sum<TResult>(expression);
        }
        public TResult Avg<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> expression)
        {
            return _Avg<TResult>(expression);
        }
        #endregion

        #region In
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> In<FieldType>(Expression<Func<T, object>> expression, params FieldType[] inValues)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> In<FieldType>(Expression<Func<T, object>> expression, List<FieldType> inValues)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> In<FieldType>(Expression<Func<T, object>> expression, ISugarQueryable<FieldType> childQueryExpression)
        {
            var sqlObj = childQueryExpression.ToSql();
            _InQueryable(expression, sqlObj);
            return this;
        }
        #endregion

        #region Other
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> AS<AsT>(string tableName)
        {
            var entityName = typeof(AsT).Name;
            _As(tableName, entityName);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> AS(string tableName)
        {
            var entityName = typeof(T).Name;
            _As(tableName, entityName);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Filter(string FilterName, bool isDisabledGobalFilter = false)
        {
            _Filter(FilterName, isDisabledGobalFilter);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> AddParameters(object parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(Context.Ado.GetParameters(parameters));
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> AddParameters(SugarParameter[] parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> AddParameters(List<SugarParameter> parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> AddJoinInfo(string tableName, string shortName, string joinWhere, JoinType type = JoinType.Left)
        {
            QueryBuilder.JoinIndex = +1;
            QueryBuilder.JoinQueryInfos
                .Add(new JoinQueryInfo()
                {
                    JoinIndex = QueryBuilder.JoinIndex,
                    TableName = tableName,
                    ShortName = shortName,
                    JoinType = type,
                    JoinWhere = joinWhere
                });
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> With(string withString)
        {
            base.With(withString);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> WithCache(int cacheDurationInSeconds = int.MaxValue)
        {
            this.IsCache = true;
            this.CacheTime = cacheDurationInSeconds;
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> WithCacheIF(bool isCache, int cacheDurationInSeconds = int.MaxValue)
        {
            if (IsCache)
            {
                this.IsCache = true;
                this.CacheTime = cacheDurationInSeconds;
            }
            return this;
        }
        #endregion
    }
    #endregion
    #region T11
    public partial class QueryableProvider<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> : QueryableProvider<T>, ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>
    {
        #region Where
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Where(Expression<Func<T, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Where(Expression<Func<T, T2, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Where(Expression<Func<T, T2, T3, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Where(Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Where(Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Where(Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> WhereIF(bool isWhere, Expression<Func<T, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Where(List<IConditionalModel> conditionalModels)
        {
            base.Where(conditionalModels);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> WhereIF(bool isWhere, Expression<Func<T, T2, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> WhereIF(bool isWhere, Expression<Func<T, T2, T3, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Where(string whereString, object whereObj)
        {
            Where<T>(whereString, whereObj);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> WhereIF(bool isWhere, string whereString, object whereObj)
        {
            if (!isWhere) return this;
            this.Where<T>(whereString, whereObj);
            return this;
        }
        #endregion

        #region Select
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        #endregion

        #region OrderBy
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderBy(Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderBy(Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderBy(Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderBy(Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderBy(Expression<Func<T, T2, T3, T4, T5, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        #endregion

        #region GroupBy
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> GroupBy(Expression<Func<T, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> GroupBy(Expression<Func<T, T2, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> GroupBy(Expression<Func<T, T2, T3, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> GroupBy(Expression<Func<T, T2, T3, T4, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> GroupBy(Expression<Func<T, T2, T3, T4, T5, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        #endregion

        #region Aggr
        public TResult Max<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>> expression)
        {
            return _Max<TResult>(expression);
        }
        public TResult Min<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>> expression)
        {
            return _Min<TResult>(expression);
        }
        public TResult Sum<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>> expression)
        {
            return _Sum<TResult>(expression);
        }
        public TResult Avg<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>> expression)
        {
            return _Avg<TResult>(expression);
        }
        #endregion

        #region In
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> In<FieldType>(Expression<Func<T, object>> expression, params FieldType[] inValues)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> In<FieldType>(Expression<Func<T, object>> expression, List<FieldType> inValues)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> In<FieldType>(Expression<Func<T, object>> expression, ISugarQueryable<FieldType> childQueryExpression)
        {
            var sqlObj = childQueryExpression.ToSql();
            _InQueryable(expression, sqlObj);
            return this;
        }
        #endregion

        #region Other
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> AS<AsT>(string tableName)
        {
            var entityName = typeof(AsT).Name;
            _As(tableName, entityName);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> AS(string tableName)
        {
            var entityName = typeof(T).Name;
            _As(tableName, entityName);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Filter(string FilterName, bool isDisabledGobalFilter = false)
        {
            _Filter(FilterName, isDisabledGobalFilter);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> AddParameters(object parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(Context.Ado.GetParameters(parameters));
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> AddParameters(SugarParameter[] parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> AddParameters(List<SugarParameter> parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> AddJoinInfo(string tableName, string shortName, string joinWhere, JoinType type = JoinType.Left)
        {
            QueryBuilder.JoinIndex = +1;
            QueryBuilder.JoinQueryInfos
                .Add(new JoinQueryInfo()
                {
                    JoinIndex = QueryBuilder.JoinIndex,
                    TableName = tableName,
                    ShortName = shortName,
                    JoinType = type,
                    JoinWhere = joinWhere
                });
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> With(string withString)
        {
            base.With(withString);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> WithCache(int cacheDurationInSeconds = int.MaxValue)
        {
            this.IsCache = true;
            this.CacheTime = cacheDurationInSeconds;
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> WithCacheIF(bool isCache, int cacheDurationInSeconds = int.MaxValue)
        {
            if (IsCache)
            {
                this.IsCache = true;
                this.CacheTime = cacheDurationInSeconds;
            }
            return this;
        }
        #endregion
    }
    #endregion
    #region T12
    public partial class QueryableProvider<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> : QueryableProvider<T>, ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>
    {
        #region Where
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Where(Expression<Func<T, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Where(Expression<Func<T, T2, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Where(Expression<Func<T, T2, T3, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Where(Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Where(Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Where(Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Where(List<IConditionalModel> conditionalModels)
        {
            base.Where(conditionalModels);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> WhereIF(bool isWhere, Expression<Func<T, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> WhereIF(bool isWhere, Expression<Func<T, T2, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> WhereIF(bool isWhere, Expression<Func<T, T2, T3, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Where(string whereString, object whereObj)
        {
            Where<T>(whereString, whereObj);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> WhereIF(bool isWhere, string whereString, object whereObj)
        {
            if (!isWhere) return this;
            this.Where<T>(whereString, whereObj);
            return this;
        }
        #endregion

        #region Select
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        #endregion

        #region OrderBy
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderBy(Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderBy(Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderBy(Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderBy(Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderBy(Expression<Func<T, T2, T3, T4, T5, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        #endregion

        #region GroupBy
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> GroupBy(Expression<Func<T, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> GroupBy(Expression<Func<T, T2, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> GroupBy(Expression<Func<T, T2, T3, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> GroupBy(Expression<Func<T, T2, T3, T4, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> GroupBy(Expression<Func<T, T2, T3, T4, T5, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        #endregion

        #region Aggr
        public TResult Max<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>> expression)
        {
            return _Max<TResult>(expression);
        }
        public TResult Min<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>> expression)
        {
            return _Min<TResult>(expression);
        }
        public TResult Sum<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>> expression)
        {
            return _Sum<TResult>(expression);
        }
        public TResult Avg<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>> expression)
        {
            return _Avg<TResult>(expression);
        }
        #endregion

        #region In
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> In<FieldType>(Expression<Func<T, object>> expression, params FieldType[] inValues)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> In<FieldType>(Expression<Func<T, object>> expression, List<FieldType> inValues)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> In<FieldType>(Expression<Func<T, object>> expression, ISugarQueryable<FieldType> childQueryExpression)
        {
            var sqlObj = childQueryExpression.ToSql();
            _InQueryable(expression, sqlObj);
            return this;
        }
        #endregion

        #region Other
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> AS<AsT>(string tableName)
        {
            var entityName = typeof(AsT).Name;
            _As(tableName, entityName);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> AS(string tableName)
        {
            var entityName = typeof(T).Name;
            _As(tableName, entityName);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Filter(string FilterName, bool isDisabledGobalFilter = false)
        {
            _Filter(FilterName, isDisabledGobalFilter);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> AddParameters(object parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(Context.Ado.GetParameters(parameters));
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> AddParameters(SugarParameter[] parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> AddParameters(List<SugarParameter> parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> AddJoinInfo(string tableName, string shortName, string joinWhere, JoinType type = JoinType.Left)
        {
            QueryBuilder.JoinIndex = +1;
            QueryBuilder.JoinQueryInfos
                .Add(new JoinQueryInfo()
                {
                    JoinIndex = QueryBuilder.JoinIndex,
                    TableName = tableName,
                    ShortName = shortName,
                    JoinType = type,
                    JoinWhere = joinWhere
                });
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> With(string withString)
        {
            base.With(withString);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> WithCache(int cacheDurationInSeconds = int.MaxValue)
        {
            this.IsCache = true;
            this.CacheTime = cacheDurationInSeconds;
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> WithCacheIF(bool isCache, int cacheDurationInSeconds = int.MaxValue)
        {
            if (IsCache)
            {
                this.IsCache = true;
                this.CacheTime = cacheDurationInSeconds;
            }
            return this;
        }
        #endregion
    }
    #endregion
}
