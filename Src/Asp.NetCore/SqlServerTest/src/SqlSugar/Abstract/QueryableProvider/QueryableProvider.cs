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
        public bool IsAs { get; set; }
        public QueryBuilder QueryBuilder
        {
            get
            {
                return this.SqlBuilder.QueryBuilder;
            }
        }
        public EntityInfo EntityInfo
        {
            get
            {
                return this.Context.EntityProvider.GetEntityInfo<T>();
            }
        }
        public void Clear()
        {
            QueryBuilder.Clear();
        }

        public virtual ISugarQueryable<T> AS<T2>(string tableName)
        {
            var entityName = typeof(T2).Name;
            IsAs = true;
            OldMappingTableList = this.Context.MappingTables;
            this.Context.MappingTables = this.Context.RewritableMethods.TranslateCopy(this.Context.MappingTables);
            this.Context.MappingTables.Add(entityName, tableName);
            this.QueryableMappingTableList = this.Context.MappingTables;
            return this;
        }
        public ISugarQueryable<T> AS(string tableName)
        {
            var entityName = typeof(T).Name;
            IsAs = true;
            OldMappingTableList = this.Context.MappingTables;
            this.Context.MappingTables = this.Context.RewritableMethods.TranslateCopy(this.Context.MappingTables);
            this.Context.MappingTables.Add(entityName, tableName);
            this.QueryableMappingTableList = this.Context.MappingTables;
            return this;
        }
        public virtual ISugarQueryable<T> With(string withString)
        {
            QueryBuilder.TableWithString = withString;
            return this;
        }

        public virtual ISugarQueryable<T> Filter(string FilterName, bool isDisabledGobalFilter = false)
        {
            QueryBuilder.IsDisabledGobalFilter = isDisabledGobalFilter;
            if (this.Context.QueryFilter.GeFilterList.IsValuable() && FilterName.IsValuable())
            {
                var list = this.Context.QueryFilter.GeFilterList.Where(it => it.FilterName == FilterName && it.IsJoinQuery == !QueryBuilder.IsSingle());
                foreach (var item in list)
                {
                    var filterResult = item.FilterValue(this.Context);
                    Where(SqlBuilder.AppendWhereOrAnd(QueryBuilder.WhereInfos.IsNullOrEmpty(), filterResult.Sql), filterResult.Parameters);
                }
            }
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
            if (whereString.IsValuable())
                this.Where<T>(whereString, whereObj);
            return this;
        }
        public virtual ISugarQueryable<T> Where<T2>(string whereString, object whereObj = null)
        {
            var whereValue = QueryBuilder.WhereInfos;
            whereValue.Add(SqlBuilder.AppendWhereOrAnd(whereValue.Count == 0, whereString + PubConst.Space));
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

        public virtual ISugarQueryable<T> In<FieldType>(Expression<Func<T, object>> expression, ISugarQueryable<FieldType> childQueryExpression) {
            var sqlObj=childQueryExpression.ToSql();
            if (sqlObj.Value.IsValuable()) {
                this.QueryBuilder.Parameters.AddRange(sqlObj.Value);
            }
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            var whereSql = string.Format(this.QueryBuilder.InTemplate, fieldName, sqlObj.Key);
            this.QueryBuilder.WhereInfos.Add(SqlBuilder.AppendWhereOrAnd(this.QueryBuilder.WhereInfos.IsNullOrEmpty(),whereSql));
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
            QueryBuilder.Skip = 0;
            QueryBuilder.Take = 1;
            var reval = this.ToList();
            if (reval.IsValuable())
            {
                return reval.SingleOrDefault();
            }
            else
            {
                return default(T);
            }
        }
        public virtual T Single(Expression<Func<T, bool>> expression)
        {
            _Where(expression);
            return Single();
        }

        public virtual T First()
        {
            if (QueryBuilder.OrderByValue.IsNullOrEmpty())
            {
                QueryBuilder.OrderByValue = QueryBuilder.DefaultOrderByTemplate;
            }
            QueryBuilder.Skip = 0;
            QueryBuilder.Take = 1;
            var reval = this.ToList();
            if (reval.IsValuable())
            {
                return reval.FirstOrDefault();
            }
            else
            {
                return default(T);
            }
        }
        public virtual T First(Expression<Func<T, bool>> expression)
        {
            _Where(expression);
            return First();
        }

        public virtual bool Any(Expression<Func<T, bool>> expression)
        {
            _Where(expression);
            var result= Any();
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
            var reval = InstanceFactory.GetQueryable<TResult>(this.Context.CurrentConnectionConfig);
            reval.Context = this.Context;
            reval.SqlBuilder = this.SqlBuilder;
            QueryBuilder.SelectValue = selectValue;
            return reval;
        }
        public virtual ISugarQueryable<T> Select(string selectValue)
        {
            QueryBuilder.SelectValue = selectValue;
            return this;
        }

        public virtual int Count()
        {
            InitMapping();
            QueryBuilder.IsCount = true;
            var sql = string.Empty;
            sql = QueryBuilder.ToSqlString();
            sql = QueryBuilder.ToCountSql(sql);
            var reval = Context.Ado.GetInt(sql, QueryBuilder.Parameters.ToArray());
            RestoreMapping();
            QueryBuilder.IsCount = false;
            return reval;
        }

        public virtual TResult Max<TResult>(string maxField)
        {
            this.Select(string.Format(QueryBuilder.MaxTemplate, maxField));
            var reval = this._ToList<TResult>().SingleOrDefault();
            return reval;
        }
        public virtual TResult Max<TResult>(Expression<Func<T, TResult>> expression)
        {
            return _Max<TResult>(expression);
        }

        public virtual TResult Min<TResult>(string minField)
        {
            this.Select(string.Format(QueryBuilder.MinTemplate, minField));
            var reval = this._ToList<TResult>().SingleOrDefault();
            return reval;
        }
        public virtual TResult Min<TResult>(Expression<Func<T, TResult>> expression)
        {
            return _Min<TResult>(expression);
        }

        public virtual TResult Sum<TResult>(string sumField)
        {
            this.Select(string.Format(QueryBuilder.SumTemplate, sumField));
            var reval = this._ToList<TResult>().SingleOrDefault();
            return reval;
        }
        public virtual TResult Sum<TResult>(Expression<Func<T, TResult>> expression)
        {
            return _Sum<TResult>(expression);
        }

        public virtual TResult Avg<TResult>(string avgField)
        {
            this.Select(string.Format(QueryBuilder.AvgTemplate, avgField));
            var reval = this._ToList<TResult>().SingleOrDefault();
            return reval;
        }
        public virtual TResult Avg<TResult>(Expression<Func<T, TResult>> expression)
        {
            return _Avg<TResult>(expression);
        }
        public virtual string ToJson()
        {
            return this.Context.RewritableMethods.SerializeObject(this.ToList());
        }
        public virtual string ToJsonPage(int pageIndex, int pageSize)
        {
            return this.Context.RewritableMethods.SerializeObject(this.ToPageList(pageIndex, pageSize));
        }
        public virtual string ToJsonPage(int pageIndex, int pageSize, ref int totalNumber)
        {
            return this.Context.RewritableMethods.SerializeObject(this.ToPageList(pageIndex, pageSize, ref totalNumber));
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
            if (QueryBuilder.PartitionByValue.IsValuable())
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
            if (QueryBuilder.PartitionByValue.IsValuable())
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
            totalNumber = this.Count();
            if (totalNumber == 0)
                result = new List<T>();
            else
                result = ToPageList(pageIndex, pageSize);
            _RestoreMapping = true;
            return result;
        }

        public virtual KeyValuePair<string, List<SugarParameter>> ToSql()
        {
            InitMapping();
            string sql = QueryBuilder.ToSqlString();
            RestoreMapping();
            return new KeyValuePair<string, List<SugarParameter>>(sql, QueryBuilder.Parameters);
        }


        #region Private Methods
        protected ISugarQueryable<TResult> _Select<TResult>(Expression expression)
        {
            var reval = InstanceFactory.GetQueryable<TResult>(this.Context.CurrentConnectionConfig);
            reval.Context = this.Context;
            reval.SqlBuilder = this.SqlBuilder;
            reval.SqlBuilder.QueryBuilder.Parameters = QueryBuilder.Parameters;
            reval.SqlBuilder.QueryBuilder.SelectValue = expression;
            return reval;
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
            OrderBy(lamResult.GetResultString() + PubConst.Space + type.ToString().ToUpper());
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
            return Min<TResult>(lamResult.GetResultString());
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
            return Max<TResult>(lamResult.GetResultString());
        }
        protected TResult _Sum<TResult>(Expression expression)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            return Sum<TResult>(lamResult.GetResultString());
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
            var isComplexModel = QueryBuilder.IsComplexModel(sqlObj.Key);
            var entityType = typeof(TResult);
            using (var dataReader = this.Db.GetDataReader(sqlObj.Key, sqlObj.Value.ToArray()))
            {
                if (typeof(TResult) == typeof(ExpandoObject))
                {
                     return this.Context.RewritableMethods.DataReaderToExpandoObjectList(dataReader) as List<TResult>;
                }
                if (entityType.IsAnonymousType() || isComplexModel)
                {
                    result = this.Context.RewritableMethods.DataReaderToDynamicList<TResult>(dataReader);
                }
                else
                {
                    result = this.Bind.DataReaderToList<TResult>(entityType, dataReader, QueryBuilder.SelectCacheKey);
                }
            }
            RestoreMapping();
            SetContextModel(result, entityType);
            return result;
        }

        protected List<string> GetPrimaryKeys()
        {
            if (this.Context.IsSystemTablesConfig)
            {
                return this.Context.DbMaintenance.GetPrimaries(this.Context.EntityProvider.GetTableName(this.EntityInfo.EntityName));
            }
            else
            {
                return this.EntityInfo.Columns.Where(it => it.IsPrimarykey).Select(it => it.DbColumnName).ToList();
            }
        }
        protected List<string> GetIdentityKeys()
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
            if (result.IsValuable())
            {
                if (entityType.GetTypeInfo().BaseType.IsValuable() && entityType.GetTypeInfo().BaseType == PubConst.ModelType)
                {
                    foreach (var item in result)
                    {
                        var contextProperty = item.GetType().GetProperty("Context");
                        ConnectionConfig config = new ConnectionConfig();
                        config = this.Context.CurrentConnectionConfig;
                        var newClient = new SqlSugarClient(config);
                        newClient.MappingColumns = this.Context.MappingColumns;
                        newClient.MappingTables = this.Context.MappingTables;
                        newClient.IgnoreColumns = this.Context.IgnoreColumns;
                        contextProperty.SetValue(item, newClient, null);
                    }
                }
            }
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
    }
    #endregion
    #region T9
    public partial class QueryableProvider<T, T2, T3, T4, T5, T6, T7, T8,T9> : QueryableProvider<T>, ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9>
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
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8,T9, bool>> expression)
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
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8,T9, bool>> expression)
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
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8,T9, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        #endregion

        #region OrderBy
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7,T8,T9> OrderBy(Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc)
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
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8,T9, object>> expression, OrderByType type = OrderByType.Asc)
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
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8,T9, object>> expression)
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
    }
    #endregion
    #region T10
    public partial class QueryableProvider<T, T2, T3, T4, T5, T6, T7, T8, T9,T10> : QueryableProvider<T>, ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9,T10>
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
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9,T10, bool>> expression)
        {
            _Where(expression);
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
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9,T10, bool>> expression)
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
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9,T10, TResult>> expression)
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
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9,T10, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        #endregion

        #region GroupBy
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9,T10> GroupBy(Expression<Func<T, object>> expression)
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
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9,T10, object>> expression)
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
    }
    #endregion
    #region T11
    public partial class QueryableProvider<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11> : QueryableProvider<T>, ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>
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
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11, bool>> expression)
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
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11, bool>> expression)
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
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11, TResult>> expression)
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
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11, object>> expression, OrderByType type = OrderByType.Asc)
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
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11, object>> expression)
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
    }
    #endregion
    #region T12
    public partial class QueryableProvider<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> : QueryableProvider<T>, ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12>
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
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12, bool>> expression)
        {
            _Where(expression);
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
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12, bool>> expression)
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
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12, TResult>> expression)
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
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12, object>> expression, OrderByType type = OrderByType.Asc)
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
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12, object>> expression)
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
    }
    #endregion
}
