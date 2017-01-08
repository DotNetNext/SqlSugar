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
    public partial class QueryableProvider<T> : QueryableAccessory, ISugarQueryable<T> where T : class, new()
    {
        public SqlSugarClient Context { get; set; }
        public IDb Db { get { return Context.Database; } }
        public IDbBind Bind { get { return this.Db.DbBind; } }
        public ISqlBuilder SqlBuilder { get { return this.Context.SqlBuilder; } }
        public List<SugarParameter> Pars
        {
            get { return PubMethod.IsNullReturnNew<List<SugarParameter>>(base._Pars); }
            set { base._Pars = value; }
        }
        public void Clear()
        {
            Pars = null;
            SqlBuilder.LambadaQueryBuilder.Clear();
        }

        public virtual ISugarQueryable<T> Where(Expression<Func<T, bool>> expression)
        {
            base.Where<T>(expression, ResolveExpressType.WhereSingle, this.Context);
            return this;
        }

        public ISugarQueryable<T> Where(string whereString, object whereObj = null)
        {
            base.Where<T>(whereString, whereObj, this.Context);
            return this;
        }

        public ISugarQueryable<T> Where<T2>(string whereString, object whereObj = null) where T2 : class, new()
        {
            base.Where<T2>(whereString, whereObj, this.Context);
            return this;
        }

        public ISugarQueryable<T> Where<T2>(Expression<Func<T, T2, bool>> expression) where T2 : class, new()
        {
            throw new NotImplementedException();
        }

        public ISugarQueryable<T> Where<T2, T3>(Expression<Func<T, T2, T3, bool>> expression) where T2 : class, new() where T3 : class, new()
        {
            throw new NotImplementedException();
        }

        public ISugarQueryable<T> Where<T2, T3, T4>(Expression<Func<T, T2, T3, T4, bool>> expression) where T2 : class, new() where T3 : class, new() where T4 : class, new()
        {
            throw new NotImplementedException();
        }

        public ISugarQueryable<T> Where<T2, T3, T4, T5>(Expression<Func<T, T2, T3, T4, T5, bool>> expression) where T2 : class, new() where T3 : class, new() where T4 : class, new() where T5 : class, new()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public ISugarQueryable<T> OrderBy(Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc)
        {
            throw new NotImplementedException();
        }

        public ISugarQueryable<T> OrderBy<T2>(Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc)
        {
            throw new NotImplementedException();
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

        public ISugarQueryable<TResult> Select<T2, TResult>(Expression<Func<T, T2, TResult>> expression) where TResult : class, new()
        {
            throw new NotImplementedException();
        }

        public ISugarQueryable<TResult> Select<T2, T3, TResult>(Expression<Func<T, T2, T3, TResult>> expression) where TResult : class, new()
        {
            throw new NotImplementedException();
        }

        public ISugarQueryable<TResult> Select<T2, T3, T4, TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression) where TResult : class, new()
        {
            throw new NotImplementedException();
        }

        public ISugarQueryable<TResult> Select<T2, T3, T4, T5, TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression) where TResult : class, new()
        {
            throw new NotImplementedException();
        }

        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, TResult>> expression) where TResult : class, new()
        {
            throw new NotImplementedException();
        }

        public ISugarQueryable<TResult> Select<TResult>(string select) where TResult : class, new()
        {
            throw new NotImplementedException();
        }

        public ISugarQueryable<T> Select(string select)
        {
            throw new NotImplementedException();
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
            string sql = SqlBuilder.LambadaQueryBuilder.ToSqlString();
            using (var dataReader = this.Db.GetDataReader(sql, this.Pars.ToArray()))
            {
                var reval = this.Bind.DataReaderToList<T>(typeof(T), dataReader, SqlBuilder.LambadaQueryBuilder.SelectValue);
                this.Clear();
                return reval;
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

        public string ToJsonPage(int pageIndex, int pageSize, ref int pageCount)
        {
            throw new NotImplementedException();
        }

        public KeyValuePair<string, Dictionary<string, string>> ToSql()
        {
            throw new NotImplementedException();
        }

        public DataTable ToDataTable()
        {
            throw new NotImplementedException();
        }

        public DataTable ToDataTablePage(int pageIndex, int pageSize)
        {
            throw new NotImplementedException();
        }

        public DataTable ToDataTablePage(int pageIndex, int pageSize, ref int pageCount)
        {
            throw new NotImplementedException();
        }

        public List<T> ToPageList(int pageIndex, int pageSize)
        {
            throw new NotImplementedException();
        }

        public List<T> ToPageList(int pageIndex, int pageSize, ref int pageCount)
        {
            throw new NotImplementedException();
        }

        public ISugarQueryable<T> JoinTable<T2>(Expression<Func<T, T2, object>> expression, JoinType type = JoinType.Left)
        {
            var sqlBuilder = this.Context.SqlBuilder;
            var items = sqlBuilder.LambadaQueryBuilder;
            items.WhereIndex = items.WhereIndex + 100;
            items.ResolveType = ResolveExpressType.WhereMultiple;
            ResolveExpress re = new ResolveExpress();
            re.Context = this.Context;
            var exLeftArray = re.GetLeftArray(expression);
            re.ResolveExpression(expression);
            JoinQueryInfo joinInfo = new JoinQueryInfo()
            {
                JoinType = type,
                JoinIndex = items.JoinIndex,
                JoinWhere = re.SqlWhere,
                PreShortName= exLeftArray.First(),
                ShortName= exLeftArray.Last(),
                TableName=sqlBuilder.GetTranslationTableName(typeof(T2).Name)
            };
            items.JoinIndex++;
            items.JoinQueryInfos.Add(joinInfo);
            base.AddPars(re.Paras,this.Context);
            return this;
        }

        public ISugarQueryable<T> JoinTable<T2, T3>(Expression<Func<T, T2, T3, object>> expression, JoinType type = JoinType.Left)
        {
            throw new NotImplementedException();
        }

        public ISugarQueryable<T> JoinTable(string tableName, string shortName, string onWhere, object whereObj, JoinType type = JoinType.Left)
        {
            throw new NotImplementedException();
        }
    }
}
