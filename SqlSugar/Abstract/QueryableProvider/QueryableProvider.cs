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
        public IDb Db { get { return Context.Database; } }
        public IDbBind Bind { get { return this.Db.DbBind; } }
        public ISqlBuilder SqlBuilder { get; set; }
        public void Clear()
        {
            SqlBuilder.LambadaQueryBuilder.Clear();
        }

        public ISugarQueryable<T> AddParameters(object whereObj)
        {
            if (whereObj != null)
                this.SqlBuilder.LambadaQueryBuilder.QueryPars.AddRange(Context.Database.GetParameters(whereObj));
            return this;
        }
        public ISugarQueryable<T> AddParameters(SugarParameter[] pars)
        {
            this.SqlBuilder.LambadaQueryBuilder.QueryPars.AddRange(pars);
            return this;
        }

        public ISugarQueryable<T> AddJoinInfo(string tableName, string shortName, string joinWhere, JoinType type = JoinType.Left)
        {

            SqlBuilder.LambadaQueryBuilder.JoinIndex = +1;
            SqlBuilder.LambadaQueryBuilder.JoinQueryInfos
                .Add(new JoinQueryInfo()
                {
                    JoinIndex = SqlBuilder.LambadaQueryBuilder.JoinIndex,
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
            var whereValue = SqlBuilder.LambadaQueryBuilder.WhereInfos;
            whereValue.Add(SqlBuilder.AppendWhereOrAnd(whereValue.Count == 0, whereString));
            if (whereObj != null)
                this.SqlBuilder.LambadaQueryBuilder.QueryPars.AddRange(Context.Database.GetParameters(whereObj));
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
            string filed = Context.Database.DbMaintenance.GetSinglePrimaryFiled(this.SqlBuilder.GetTableName(typeof(T).Name));
            string shortName = this.SqlBuilder.LambadaQueryBuilder.TableShortName == null ? null : (this.SqlBuilder.LambadaQueryBuilder.TableShortName + ".");
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
                    var whereIndex = this.SqlBuilder.LambadaQueryBuilder.WhereIndex;
                    string parameterName = this.SqlBuilder.SqlParameterKeyWord + "InPara" + whereIndex;
                    this.Where(string.Format("{0} = {1} ", filed, parameterName));
                    this.AddParameters(new SqlParameter(parameterName, inValues[0]));
                    this.SqlBuilder.LambadaQueryBuilder.WhereIndex++;
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
                    this.Where(string.Format("{0} in ({1}) ", filed, string.Join(",", values)));
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
                this.Where(string.Format("{0} in ({1}) ", filed, string.Join(",", values)));

            }
            return this;
        }

        public ISugarQueryable<T> In<FieldType>(Expression<Func<T, object>> expression, params FieldType[] inValues)
        {
            var isSingle = SqlBuilder.LambadaQueryBuilder.IsSingle();
            var lamResult = SqlBuilder.LambadaQueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            return In(fieldName, inValues);
        }

        public ISugarQueryable<T> OrderBy(string orderFileds)
        {
            var orderByValue = SqlBuilder.LambadaQueryBuilder.OrderByValue;
            if (SqlBuilder.LambadaQueryBuilder.OrderByValue.IsNullOrEmpty())
            {
                SqlBuilder.LambadaQueryBuilder.OrderByValue = "ORDER BY ";
            }
            SqlBuilder.LambadaQueryBuilder.OrderByValue += string.IsNullOrEmpty(orderByValue) ? orderFileds : ("," + orderFileds);
            return this;
        }

        public ISugarQueryable<T> OrderBy(Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc)
        {
            this._OrderBy(expression, type);
            return this;
        }

        public ISugarQueryable<T> OrderBy<T2>(Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc)
        {
            this._OrderBy(expression, type);
            return this;
        }

        public ISugarQueryable<T> GroupBy(Expression<Func<T, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }

        public ISugarQueryable<T> GroupBy(string groupFileds)
        {
            var croupByValue = SqlBuilder.LambadaQueryBuilder.GroupByValue;
            if (SqlBuilder.LambadaQueryBuilder.GroupByValue.IsNullOrEmpty())
            {
                SqlBuilder.LambadaQueryBuilder.GroupByValue = "GROUP BY ";
            }
            SqlBuilder.LambadaQueryBuilder.GroupByValue += string.IsNullOrEmpty(croupByValue) ? groupFileds : ("," + groupFileds);
            return this;
        }

        public ISugarQueryable<T> Skip(int num)
        {
            SqlBuilder.LambadaQueryBuilder.Skip = num;
            return this;
        }

        public ISugarQueryable<T> Take(int num)
        {
            SqlBuilder.LambadaQueryBuilder.Take = num;
            return this;
        }

        public T Single()
        {
            if (SqlBuilder.LambadaQueryBuilder.OrderByValue.IsNullOrEmpty())
            {
                SqlBuilder.LambadaQueryBuilder.OrderByValue = " ORDER BY GETDATE()";
            }
            SqlBuilder.LambadaQueryBuilder.Skip = 0;
            SqlBuilder.LambadaQueryBuilder.Take = 1;
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
            if (SqlBuilder.LambadaQueryBuilder.OrderByValue.IsNullOrEmpty())
            {
                SqlBuilder.LambadaQueryBuilder.OrderByValue = " ORDER BY GETDATE()";
            }
            SqlBuilder.LambadaQueryBuilder.Skip = 0;
            SqlBuilder.LambadaQueryBuilder.Take = 1;
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
            return this.ToList().IsValuable();
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
            SqlBuilder.LambadaQueryBuilder.SelectValue = selectValue;
            return reval;
        }
        public ISugarQueryable<T> Select(string selectValue)
        {
            SqlBuilder.LambadaQueryBuilder.SelectValue = selectValue;
            return this;
        }

        public int Count()
        {
            SqlBuilder.LambadaQueryBuilder.IsCount = true;
            var sql = SqlBuilder.LambadaQueryBuilder.ToSqlString();
            var reval = Context.Database.GetInt(sql, SqlBuilder.LambadaQueryBuilder.QueryPars.ToArray());
            SqlBuilder.LambadaQueryBuilder.IsCount = false;
            return reval;
        }

        public TResult Max<TResult>(string maxField)
        {
            this.Select("Max("+ maxField + ")");
            var reval = this._ToList<TResult>().SingleOrDefault();
            return reval;
        }

        public TResult Max<TResult>(Expression<Func<T, object>> expression)
        {
            var isSingle = SqlBuilder.LambadaQueryBuilder.IsSingle();
            var lamResult = SqlBuilder.LambadaQueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            return Max<TResult>(lamResult.GetResultString());
        }

        public TResult Min<TResult>(string minField)
        {
            this.Select("Min("+ minField + ")");
            var reval = this._ToList<TResult>().SingleOrDefault();
            return reval;
        }

        public TResult Min<TResult>(Expression<Func<T, object>> expression)
        {
            var isSingle = SqlBuilder.LambadaQueryBuilder.IsSingle();
            var lamResult = SqlBuilder.LambadaQueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            return Min<TResult>(lamResult.GetResultString());
        }

        public TResult Sum<TResult>(string sumField)
        {
            this.Select("Sum(" + sumField + ")");
            var reval = this._ToList<TResult>().SingleOrDefault();
            return reval;
        }

        public TResult Sum<TResult>(Expression<Func<T, object>> expression)
        {
            var isSingle = SqlBuilder.LambadaQueryBuilder.IsSingle();
            var lamResult = SqlBuilder.LambadaQueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            return Sum<TResult>(lamResult.GetResultString());
        }
        public TResult Avg<TResult>(string avgField)
        {
            this.Select("Sum(" + avgField + ")");
            var reval = this._ToList<TResult>().SingleOrDefault();
            return reval;
        }
        public TResult Avg<TResult>(Expression<Func<T, object>> expression)
        {
            var isSingle = SqlBuilder.LambadaQueryBuilder.IsSingle();
            var lamResult = SqlBuilder.LambadaQueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
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
            return this.Context.RewritableMethods.SerializeObject(this.ToPageList(pageIndex,pageSize));
        }

        public string ToJsonPage(int pageIndex, int pageSize, ref int totalNumber)
        {
            return this.Context.RewritableMethods.SerializeObject(this.ToPageList(pageIndex, pageSize,ref totalNumber));
        }

        public KeyValuePair<string, List<SugarParameter>> ToSql()
        {
            string sql = SqlBuilder.LambadaQueryBuilder.ToSqlString();
            return new KeyValuePair<string, List<SugarParameter>>(sql, SqlBuilder.LambadaQueryBuilder.QueryPars);
        }

        public ISugarQueryable<T> With(string withString)
        {
            SqlBuilder.LambadaQueryBuilder.TableWithString = withString;
            return this;
        }

        public DataTable ToDataTable()
        {
            var sqlObj = this.ToSql();
            var result = this.Db.GetDataTable(sqlObj.Key, sqlObj.Value.ToArray());
            return result;
        }

        public DataTable ToDataTablePage(int pageIndex, int pageSize)
        {
            if (pageIndex == 0)
                pageIndex = 1;
            SqlBuilder.LambadaQueryBuilder.Skip = (pageIndex - 1) * pageSize;
            SqlBuilder.LambadaQueryBuilder.Take = pageSize;
            return ToDataTable();

        }

        public DataTable ToDataTablePage(int pageIndex, int pageSize, ref int totalNumber)
        {
            totalNumber = this.Count();
            return ToDataTablePage(pageIndex,pageSize);
        }

        public List<T> ToPageList(int pageIndex, int pageSize)
        {
            if (pageIndex == 0)
                pageIndex = 1;
            SqlBuilder.LambadaQueryBuilder.Skip = (pageIndex - 1) * pageSize;
            SqlBuilder.LambadaQueryBuilder.Take = pageSize;
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
            reval.SqlBuilder.LambadaQueryBuilder.QueryPars = this.SqlBuilder.LambadaQueryBuilder.QueryPars;
            reval.SqlBuilder.LambadaQueryBuilder.SelectValue = expression;
            return reval;
        }
        protected void _Where(Expression expression)
        {
            var isSingle = SqlBuilder.LambadaQueryBuilder.IsSingle();
            var result = SqlBuilder.LambadaQueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.WhereSingle : ResolveExpressType.WhereMultiple);
            SqlBuilder.LambadaQueryBuilder.WhereInfos.Add(SqlBuilder.AppendWhereOrAnd(SqlBuilder.LambadaQueryBuilder.WhereInfos.IsNullOrEmpty(), result.GetResultString()));
        }
        protected ISugarQueryable<T> _OrderBy(Expression expression, OrderByType type = OrderByType.Asc)
        {
            var isSingle = SqlBuilder.LambadaQueryBuilder.IsSingle();
            var lamResult = SqlBuilder.LambadaQueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            OrderBy(lamResult.GetResultString() + " " + type.ToString().ToUpper());
            return this;
        }
        protected ISugarQueryable<T> _GroupBy(Expression expression)
        {
            var isSingle = SqlBuilder.LambadaQueryBuilder.IsSingle();
            var lamResult = SqlBuilder.LambadaQueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            GroupBy(lamResult.GetResultString());
            return this;
        }

        private List<TResult> _ToList<TResult>()
        {
            var sqlObj = this.ToSql();
            var isComplexModel = Regex.IsMatch(sqlObj.Key, @"AS \[\w+\.\w+\]");
            using (var dataReader = this.Db.GetDataReader(sqlObj.Key, sqlObj.Value.ToArray()))
            {
                var tType = typeof(TResult);
                if (tType.IsAnonymousType() || isComplexModel)
                {
                    return this.Context.RewritableMethods.DataReaderToDynamicList<TResult>(dataReader);
                }
                else
                {
                    var reval = this.Bind.DataReaderToList<TResult>(tType, dataReader, SqlBuilder.LambadaQueryBuilder.SelectCacheKey);
                    return reval;
                }
            }
        }
        #endregion
    }
}
