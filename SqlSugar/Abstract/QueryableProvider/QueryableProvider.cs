using System;
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
            throw new NotImplementedException();
        }

        public T InSingle(object pkValue)
        {
            throw new NotImplementedException();
        }

        public ISugarQueryable<T> In<FieldType>(string InFieldName, params FieldType[] inValues)
        {
            throw new NotImplementedException();
        }

        public ISugarQueryable<T> In<FieldType>(Expression<Func<T, object>> expression, params FieldType[] inValues)
        {
            throw new NotImplementedException();
        }

        public ISugarQueryable<T> In<FieldType>(Expression<Func<T, object>> expression, List<FieldType> inValues)
        {
            throw new NotImplementedException();
        }

        public ISugarQueryable<T> In<FieldType>(string InFieldName, List<FieldType> inValues)
        {
            throw new NotImplementedException();
        }

        public ISugarQueryable<T> OrderBy(string orderFileds)
        {
            var orderByValue = SqlBuilder.LambadaQueryBuilder.OrderByValue;
            if (SqlBuilder.LambadaQueryBuilder.OrderByValue.IsNullOrEmpty()) {
                SqlBuilder.LambadaQueryBuilder.OrderByValue = "ORDER ";
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
            throw new NotImplementedException();
        }

        public ISugarQueryable<T> GroupBy(string groupFileds)
        {
            throw new NotImplementedException();
        }

        public ISugarQueryable<T> Skip(int index)
        {
            throw new NotImplementedException();
        }

        public ISugarQueryable<T> Take(int num)
        {
            throw new NotImplementedException();
        }

        public T Single()
        {
            throw new NotImplementedException();
        }

        public T SingleOrDefault()
        {
            throw new NotImplementedException();
        }

        public T Single(Expression<Func<T, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public T SingleOrDefault(Expression<Func<T, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public T First()
        {
            throw new NotImplementedException();
        }

        public T FirstOrDefault()
        {
            throw new NotImplementedException();
        }

        public T First(Expression<Func<T, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public T FirstOrDefault(Expression<Func<T, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public bool Any(Expression<Func<T, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public bool Any()
        {
            throw new NotImplementedException();
        }

        public ISugarQueryable<TResult> Select<T2, TResult>(Expression<Func<T2, TResult>> expression)
        {
            return SelectMehtod<TResult>(expression);
        }

        public ISugarQueryable<TResult> Select<T2, T3, TResult>(Expression<Func<T2, T3, TResult>> expression)
        {
            return SelectMehtod<TResult>(expression);
        }

        public ISugarQueryable<TResult> Select<T2, T3, T4, TResult>(Expression<Func<T2, T3, T4, TResult>> expression)
        {
            return SelectMehtod<TResult>(expression);
        }

        public ISugarQueryable<TResult> Select<T2, T3, T4, T5, TResult>(Expression<Func<T2, T3, T4, T5, TResult>> expression)
        {
            return SelectMehtod<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<T2, T3, T4, T5, T6, TResult>(Expression<Func<T2, T3, T4, T5, T6, TResult>> expression)
        {
            return SelectMehtod<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<T2, T3, T4, T5, T6, T7, TResult>(Expression<Func<T2, T3, T4, T5, T6, T7, TResult>> expression)
        {
            return SelectMehtod<TResult>(expression);
        }

        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, TResult>> expression)
        {
            return SelectMehtod<TResult>(expression);
        }

        private ISugarQueryable<TResult> SelectMehtod<TResult>(Expression expression)
        {
            var reval = InstanceFactory.GetQueryable<TResult>(this.Context.CurrentConnectionConfig);
            reval.Context = this.Context;
            reval.SqlBuilder = this.SqlBuilder;
            reval.SqlBuilder.LambadaQueryBuilder.QueryPars = this.SqlBuilder.LambadaQueryBuilder.QueryPars;
            reval.SqlBuilder.LambadaQueryBuilder.SelectValue = expression;
            return reval;
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
            throw new NotImplementedException();
        }

        public TResult Max<TResult>(string maxField)
        {
            throw new NotImplementedException();
        }

        public object Max(Expression<Func<T, object>> expression)
        {
            throw new NotImplementedException();
        }

        public TResult Min<TResult>(string minField)
        {
            throw new NotImplementedException();
        }

        public object Min(Expression<Func<T, object>> expression)
        {
            throw new NotImplementedException();
        }

        public List<T> ToList()
        {
            var sqlObj = this.ToSql();
            var isComplexModel = Regex.IsMatch(sqlObj.Key, @"AS \[\w+\.\w+\]");
            using (var dataReader = this.Db.GetDataReader(sqlObj.Key, sqlObj.Value.ToArray()))
            {
                var tType = typeof(T);
                if (tType.IsAnonymousType() || isComplexModel)
                {
                    return this.Context.RewritableMethods.DataReaderToDynamicList<T>(dataReader);
                }
                else
                {
                    var reval = this.Bind.DataReaderToList<T>(tType, dataReader, SqlBuilder.LambadaQueryBuilder.SelectCacheKey);
                    return reval;
                }
            }
        }
        public string ToJson()
        {
            throw new NotImplementedException();
        }

        public string ToJsonPage(int pageIndex, int pageSize)
        {
            throw new NotImplementedException();
        }

        public string ToJsonPage(int pageIndex, int pageSize, ref int totalNumber)
        {
            throw new NotImplementedException();
        }

        public KeyValuePair<string, List<SugarParameter>> ToSql()
        {
            string sql = SqlBuilder.LambadaQueryBuilder.ToSqlString();
            return new KeyValuePair<string, List<SugarParameter>>(sql, SqlBuilder.LambadaQueryBuilder.QueryPars);
        }

        public DataTable ToDataTable()
        {
            throw new NotImplementedException();
        }

        public DataTable ToDataTablePage(int pageIndex, int pageSize)
        {
            throw new NotImplementedException();
        }

        public DataTable ToDataTablePage(int pageIndex, int pageSize, ref int totalNumber)
        {
            throw new NotImplementedException();
        }

        public List<T> ToPageList(int pageIndex, int pageSize)
        {
            throw new NotImplementedException();
        }

        public List<T> ToPageList(int pageIndex, int pageSize, ref int totalNumber)
        {
            throw new NotImplementedException();
        }

        #region 私有方法
        protected void _Where(Expression expression)
        {
            var isSingle = SqlBuilder.LambadaQueryBuilder.IsSingle();
            var result = SqlBuilder.LambadaQueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.WhereSingle : ResolveExpressType.WhereMultiple);
            SqlBuilder.LambadaQueryBuilder.WhereInfos.Add(SqlBuilder.AppendWhereOrAnd(SqlBuilder.LambadaQueryBuilder.WhereInfos.IsNullOrEmpty(), result.GetResultString()));
        }
        protected ISugarQueryable<T> _OrderBy(Expression expression, OrderByType type = OrderByType.Asc)
        {
            var isSingle = SqlBuilder.LambadaQueryBuilder.IsSingle();
            var orderByValue = SqlBuilder.LambadaQueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            OrderBy(orderByValue.GetResultString() + " " + type.ToString().ToUpper());
            return this;
        }
        #endregion
    }
}
