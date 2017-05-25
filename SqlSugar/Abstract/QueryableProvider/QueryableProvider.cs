using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;

namespace SqlSugar
{
    public partial class QueryableProvider<T> : QueryableAccessory, ISugarQueryable<T>
    {
        public SqlSugarClient Context { get; set; }
        public IAdo Db { get { return Context.Ado; } }
        public IDbBind Bind { get { return this.Db.DbBind; } }
        public ISqlBuilder SqlBuilder { get; set; }
        public MappingTableList OldMappingTableList { get; set; }
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
        public ISugarQueryable<T> AS<T2>(string tableName)
        {
            var entityName = typeof(T2).Name;
            IsAs = true;
            this.Context.MappingTables = this.Context.RewritableMethods.TranslateCopy(this.Context.MappingTables);
            this.Context.MappingTables.Add(entityName, tableName);
            return this;
        }
        public ISugarQueryable<T> AS(string tableName)
        {
            var entityName = typeof(T).Name;
            IsAs = true;
            this.Context.MappingTables = this.Context.RewritableMethods.TranslateCopy(this.Context.MappingTables);
            this.Context.MappingTables.Add(entityName, tableName);
            return this;
        }
        public ISugarQueryable<T> AddParameters(object whereObj)
        {
            if (whereObj != null)
                QueryBuilder.Parameters.AddRange(Context.Ado.GetParameters(whereObj));
            return this;
        }
        public ISugarQueryable<T> AddParameters(SugarParameter[] pars)
        {
            QueryBuilder.Parameters.AddRange(pars);
            return this;
        }
        public ISugarQueryable<T> AddParameters(SugarParameter par)
        {
            QueryBuilder.Parameters.Add(par);
            return this;
        }

        public ISugarQueryable<T> AddJoinInfo(string tableName, string shortName, string joinWhere, JoinType type = JoinType.Left)
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
        public ISugarQueryable<T> Where(string whereString, object whereObj = null)
        {
            this.Where<T>(whereString, whereObj);
            return this;
        }
        public ISugarQueryable<T> Where<T2>(string whereString, object whereObj = null)
        {
            var whereValue = QueryBuilder.WhereInfos;
            whereValue.Add(SqlBuilder.AppendWhereOrAnd(whereValue.Count == 0, whereString));
            if (whereObj != null)
                QueryBuilder.Parameters.AddRange(Context.Ado.GetParameters(whereObj));
            return this;
        }
        public ISugarQueryable<T> Where<T2>(Expression<Func<T2, bool>> expression)
        {
            this._Where(expression);
            return this;
        }
        public ISugarQueryable<T> Where<T2, T3>(Expression<Func<T2, T3, bool>> expression)
        {
            this._Where(expression);
            return this;
        }
        public ISugarQueryable<T> Where<T2, T3, T4>(Expression<Func<T2, T3, T4, bool>> expression)
        {
            this._Where(expression);
            return this;
        }
        public ISugarQueryable<T> Where<T2, T3, T4, T5>(Expression<Func<T2, T3, T4, T5, bool>> expression)
        {
            this._Where(expression);
            return this;
        }
        public ISugarQueryable<T> Where<T2, T3, T4, T5, T6>(Expression<Func<T2, T3, T4, T5, T6, bool>> expression)
        {
            this._Where(expression);
            return this;
        }

        public ISugarQueryable<T> Having(Expression<Func<T, bool>> expression)
        {
            this._Having(expression);
            return this;
        }
        public ISugarQueryable<T> Having(string whereString, object whereObj = null)
        {

            QueryBuilder.HavingInfos = SqlBuilder.AppendHaving(whereString);
            if (whereObj != null)
                QueryBuilder.Parameters.AddRange(Context.Ado.GetParameters(whereObj));
            return this;
        }
        public ISugarQueryable<T> Having<T2>(Expression<Func<T2, bool>> expression)
        {
            this._Having(expression);
            return this;
        }
        public ISugarQueryable<T> Having<T2, T3>(Expression<Func<T2, T3, bool>> expression)
        {
            this._Having(expression);
            return this;
        }
        public ISugarQueryable<T> Having<T2, T3, T4>(Expression<Func<T2, T3, T4, bool>> expression)
        {
            this._Having(expression);
            return this;
        }
        public ISugarQueryable<T> Having<T2, T3, T4, T5>(Expression<Func<T2, T3, T4, T5, bool>> expression)
        {
            this._Having(expression);
            return this;
        }
        public ISugarQueryable<T> Having<T2, T3, T4, T5, T6>(Expression<Func<T2, T3, T4, T5, T6, bool>> expression)
        {
            this._Having(expression);
            return this;
        }

        public virtual ISugarQueryable<T> WhereIF(bool isWhere, Expression<Func<T, bool>> expression)
        {
            if (!isWhere) return this;
            Where<T>(expression);
            return this;
        }
        public ISugarQueryable<T> WhereIF(bool isWhere, string whereString, object whereObj = null)
        {
            if (!isWhere) return this;
            this.Where<T>(whereString, whereObj);
            return this;
        }
        public ISugarQueryable<T> WhereIF<T2>(bool isWhere, string whereString, object whereObj = null)
        {
            if (!isWhere) return this;
            this.Where<T2>(whereString, whereObj);
            return this;
        }
        public ISugarQueryable<T> WhereIF<T2>(bool isWhere, Expression<Func<T2, bool>> expression)
        {
            if (!isWhere) return this;
            this.Where(expression);
            return this;
        }
        public ISugarQueryable<T> WhereIF<T2, T3>(bool isWhere, Expression<Func<T2, T3, bool>> expression)
        {
            if (!isWhere) return this;
            this.Where(expression);
            return this;
        }
        public ISugarQueryable<T> WhereIF<T2, T3, T4>(bool isWhere, Expression<Func<T2, T3, T4, bool>> expression)
        {
            if (!isWhere) return this;
            this.Where(expression);
            return this;
        }
        public ISugarQueryable<T> WhereIF<T2, T3, T4, T5>(bool isWhere, Expression<Func<T2, T3, T4, T5, bool>> expression)
        {
            if (!isWhere) return this;
            this.Where(expression);
            return this;
        }
        public ISugarQueryable<T> WhereIF<T2, T3, T4, T5, T6>(bool isWhere, Expression<Func<T2, T3, T4, T5, T6, bool>> expression)
        {
            if (!isWhere) return this;
            this.Where(expression);
            return this;
        }

        public ISugarQueryable<T> In(params object[] pkValues)
        {
            if (pkValues == null || pkValues.Length == 0)
            {
                Where("1=2 ");
                return this;
            }
            var pks = GetPrimaryKeys().Select(it => SqlBuilder.GetTranslationTableName(it)).ToList();
            Check.Exception(pks == null || pks.Count != 1, "Queryable.In(params object[] pkValues): Only one primary key");
            string filed = pks.FirstOrDefault();
            string shortName = QueryBuilder.TableShortName == null ? null : (QueryBuilder.TableShortName + ".");
            filed = shortName + filed;
            return In(filed, pkValues);
        }

        public T InSingle(object pkValue)
        {
            var list = In(pkValue).ToList();
            if (list == null) return default(T);
            else return list.SingleOrDefault();
        }

        public ISugarQueryable<T> In<FieldType>(string filed, params FieldType[] inValues)
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

        public ISugarQueryable<T> In<FieldType>(Expression<Func<T, object>> expression, params FieldType[] inValues)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            return In(fieldName, inValues);
        }

        public ISugarQueryable<T> OrderBy(string orderFileds)
        {
            var orderByValue = QueryBuilder.OrderByValue;
            if (QueryBuilder.OrderByValue.IsNullOrEmpty())
            {
                QueryBuilder.OrderByValue = QueryBuilder.OrderByTemplate;
            }
            QueryBuilder.OrderByValue += string.IsNullOrEmpty(orderByValue) ? orderFileds : ("," + orderFileds);
            return this;
        }

        public ISugarQueryable<T> OrderBy(Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc)
        {
            this._OrderBy(expression, type);
            return this;
        }

        public ISugarQueryable<T2> OrderBy<T2>(Expression<Func<T2, object>> expression, OrderByType type = OrderByType.Asc)
        {
            this._OrderBy(expression, type);
            var reval = InstanceFactory.GetQueryable<T2>(this.Context.CurrentConnectionConfig);
            reval.Context = this.Context;
            reval.SqlBuilder = this.SqlBuilder;
            reval.SqlBuilder.QueryBuilder.Parameters = QueryBuilder.Parameters;
            reval.SqlBuilder.QueryBuilder.SelectValue = expression;
            return reval;
        }

        public ISugarQueryable<T> GroupBy(Expression<Func<T, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T2> GroupBy<T2>(Expression<Func<T2, object>> expression)
        {
            _GroupBy(expression);
            var reval = InstanceFactory.GetQueryable<T2>(this.Context.CurrentConnectionConfig);
            reval.Context = this.Context;
            reval.SqlBuilder = this.SqlBuilder;
            reval.SqlBuilder.QueryBuilder.Parameters = QueryBuilder.Parameters;
            reval.SqlBuilder.QueryBuilder.SelectValue = expression;
            return reval;
        }
        public ISugarQueryable<T> GroupBy(string groupFileds)
        {
            var croupByValue = QueryBuilder.GroupByValue;
            if (QueryBuilder.GroupByValue.IsNullOrEmpty())
            {
                QueryBuilder.GroupByValue = QueryBuilder.GroupByTemplate;
            }
            QueryBuilder.GroupByValue += string.IsNullOrEmpty(croupByValue) ? groupFileds : ("," + groupFileds);
            return this;
        }

        public ISugarQueryable<T> Skip(int num)
        {
            QueryBuilder.Skip = num;
            return this;
        }

        public ISugarQueryable<T> Take(int num)
        {
            QueryBuilder.Take = num;
            return this;
        }

        public T Single()
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

        public T Single(Expression<Func<T, bool>> expression)
        {
            _Where(expression);
            return Single();
        }

        public T First()
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

        public T First(Expression<Func<T, bool>> expression)
        {
            _Where(expression);
            return First();
        }

        public bool Any(Expression<Func<T, bool>> expression)
        {
            _Where(expression);
            return Any();
        }

        public bool Any()
        {
            return this.Count() > 0;
        }

        public ISugarQueryable<TResult> Select<T2, TResult>(Expression<Func<T2, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }

        public ISugarQueryable<TResult> Select<T2, T3, TResult>(Expression<Func<T2, T3, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }

        public ISugarQueryable<TResult> Select<T2, T3, T4, TResult>(Expression<Func<T2, T3, T4, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }

        public ISugarQueryable<TResult> Select<T2, T3, T4, T5, TResult>(Expression<Func<T2, T3, T4, T5, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<T2, T3, T4, T5, T6, TResult>(Expression<Func<T2, T3, T4, T5, T6, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<T2, T3, T4, T5, T6, T7, TResult>(Expression<Func<T2, T3, T4, T5, T6, T7, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }

        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }

        public ISugarQueryable<TResult> Select<TResult>(string selectValue) where TResult : class, new()
        {
            var reval = InstanceFactory.GetQueryable<TResult>(this.Context.CurrentConnectionConfig);
            reval.Context = this.Context;
            reval.SqlBuilder = this.SqlBuilder;
            QueryBuilder.SelectValue = selectValue;
            return reval;
        }
        public ISugarQueryable<T> Select(string selectValue)
        {
            QueryBuilder.SelectValue = selectValue;
            return this;
        }

        public int Count()
        {
            QueryBuilder.IsCount = true;
            var sql = QueryBuilder.ToSqlString();
            var reval = Context.Ado.GetInt(sql, QueryBuilder.Parameters.ToArray());
            RestoreMapping();
            QueryBuilder.IsCount = false;
            return reval;
        }

        public TResult Max<TResult>(string maxField)
        {
            this.Select(string.Format(QueryBuilder.MaxTemplate, maxField));
            var reval = this._ToList<TResult>().SingleOrDefault();
            return reval;
        }

        public TResult Max<TResult>(Expression<Func<T, TResult>> expression)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            return Max<TResult>(lamResult.GetResultString());
        }

        public TResult Min<TResult>(string minField)
        {
            this.Select(string.Format(QueryBuilder.MinTemplate, minField));
            var reval = this._ToList<TResult>().SingleOrDefault();
            return reval;
        }

        public TResult Min<TResult>(Expression<Func<T, TResult>> expression)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            return Min<TResult>(lamResult.GetResultString());
        }

        public TResult Sum<TResult>(string sumField)
        {
            this.Select(string.Format(QueryBuilder.SumTemplate, sumField));
            var reval = this._ToList<TResult>().SingleOrDefault();
            return reval;
        }

        public TResult Sum<TResult>(Expression<Func<T, TResult>> expression)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            return Sum<TResult>(lamResult.GetResultString());
        }
        public TResult Avg<TResult>(string avgField)
        {
            this.Select(string.Format(QueryBuilder.AvgTemplate, avgField));
            var reval = this._ToList<TResult>().SingleOrDefault();
            return reval;
        }
        public TResult Avg<TResult>(Expression<Func<T, TResult>> expression)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            return Avg<TResult>(lamResult.GetResultString());
        }
        public List<T> ToList()
        {
            return _ToList<T>();
        }

        public string ToJson()
        {
            return this.Context.RewritableMethods.SerializeObject(this.ToList());
        }

        public string ToJsonPage(int pageIndex, int pageSize)
        {
            return this.Context.RewritableMethods.SerializeObject(this.ToPageList(pageIndex, pageSize));
        }

        public string ToJsonPage(int pageIndex, int pageSize, ref int totalNumber)
        {
            return this.Context.RewritableMethods.SerializeObject(this.ToPageList(pageIndex, pageSize, ref totalNumber));
        }

        public KeyValuePair<string, List<SugarParameter>> ToSql()
        {
            string sql = QueryBuilder.ToSqlString();
            RestoreMapping();
            return new KeyValuePair<string, List<SugarParameter>>(sql, QueryBuilder.Parameters);
        }

        public ISugarQueryable<T> With(string withString)
        {
            QueryBuilder.TableWithString = withString;
            return this;
        }

        public DataTable ToDataTable()
        {
            var sqlObj = this.ToSql();
            RestoreMapping();
            var result = this.Db.GetDataTable(sqlObj.Key, sqlObj.Value.ToArray());
            return result;
        }

        public DataTable ToDataTablePage(int pageIndex, int pageSize)
        {
            if (pageIndex == 0)
                pageIndex = 1;
            QueryBuilder.Skip = (pageIndex - 1) * pageSize;
            QueryBuilder.Take = pageSize;
            return ToDataTable();
        }

        public DataTable ToDataTablePage(int pageIndex, int pageSize, ref int totalNumber)
        {
            totalNumber = this.Count();
            return ToDataTablePage(pageIndex, pageSize);
        }

        public List<T> ToPageList(int pageIndex, int pageSize)
        {
            if (pageIndex == 0)
                pageIndex = 1;
            QueryBuilder.Skip = (pageIndex - 1) * pageSize;
            QueryBuilder.Take = pageSize;
            return ToList();
        }

        public List<T> ToPageList(int pageIndex, int pageSize, ref int totalNumber)
        {
            totalNumber = this.Count();
            return ToPageList(pageIndex, pageSize);
        }


        #region Private Methods
        private ISugarQueryable<TResult> _Select<TResult>(Expression expression)
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
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            GroupBy(lamResult.GetResultString());
            return this;
        }
        protected ISugarQueryable<T> _Having(Expression expression)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.WhereSingle : ResolveExpressType.WhereMultiple);
            Having(lamResult.GetResultString());
            return this;
        }
        private List<TResult> _ToList<TResult>()
        {
            List<TResult> result = null;
            var sqlObj = this.ToSql();
            var isComplexModel = QueryBuilder.IsComplexModel(sqlObj.Key);
            using (var dataReader = this.Db.GetDataReader(sqlObj.Key, sqlObj.Value.ToArray()))
            {
                var tType = typeof(TResult);
                if (tType.IsAnonymousType() || isComplexModel)
                {
                    result = this.Context.RewritableMethods.DataReaderToDynamicList<TResult>(dataReader);
                }
                else
                {
                    result = this.Bind.DataReaderToList<TResult>(tType, dataReader, QueryBuilder.SelectCacheKey);
                }
                if (this.Context.CurrentConnectionConfig.IsAutoCloseConnection) this.Context.Close();
            }
            RestoreMapping();
            return result;
        }
        private List<string> GetPrimaryKeys()
        {
            if (this.Context.IsSystemTablesConfig)
            {
                return this.Context.DbMaintenance.GetPrimaries(this.EntityInfo.DbTableName);
            }
            else
            {
                return this.EntityInfo.Columns.Where(it => it.IsPrimarykey).Select(it => it.DbColumnName).ToList();
            }
        }
        private List<string> GetIdentityKeys()
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
        private void RestoreMapping()
        {
            if (IsAs)
            {
                this.Context.MappingTables = OldMappingTableList;
            }
        }
        #endregion
    }
}
