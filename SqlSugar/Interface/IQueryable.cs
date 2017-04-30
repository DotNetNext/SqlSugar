using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
namespace SqlSugar
{
    public partial interface ISugarQueryable<T>
    {
        SqlSugarClient Context { get; set; }
        ISqlBuilder SqlBuilder { get; set; }

        ISugarQueryable<T> With(string withString);
        ISugarQueryable<T> AddParameters(object pars);
        ISugarQueryable<T> AddParameters(SugarParameter[] pars);
        ISugarQueryable<T> AddJoinInfo(string tableName, string shortName, string joinWhere, JoinType type=JoinType.Left);

        ISugarQueryable<T> Where(Expression<Func<T, bool>> expression);
        ISugarQueryable<T> Where(string whereString, object whereObj = null);
        ISugarQueryable<T> Where<T2>(Expression<Func<T2, bool>> expression);
        ISugarQueryable<T> Where<T2, T3>(Expression<Func<T2, T3, bool>> expression);
        ISugarQueryable<T> Where<T2, T3, T4>(Expression<Func<T2, T3, T4, bool>> expression);
        ISugarQueryable<T> Where<T2, T3, T4, T5>(Expression<Func<T2, T3, T4, T5, bool>> expression) ;
        ISugarQueryable<T> Where<T2, T3, T4, T5, T6>(Expression<Func<T2, T3, T4, T5, T6, bool>> expression) ;

        ISugarQueryable<T> Having(Expression<Func<T, bool>> expression);
        ISugarQueryable<T> Having(string whereString, object whereObj = null);
        ISugarQueryable<T> Having<T2>(Expression<Func<T2, bool>> expression);
        ISugarQueryable<T> Having<T2, T3>(Expression<Func<T2, T3, bool>> expression);
        ISugarQueryable<T> Having<T2, T3, T4>(Expression<Func<T2, T3, T4, bool>> expression);
        ISugarQueryable<T> Having<T2, T3, T4, T5>(Expression<Func<T2, T3, T4, T5, bool>> expression);
        ISugarQueryable<T> Having<T2, T3, T4, T5, T6>(Expression<Func<T2, T3, T4, T5, T6, bool>> expression);

        ISugarQueryable<T> WhereIF(bool isWhere, Expression<Func<T, bool>> expression);
        ISugarQueryable<T> WhereIF(bool isWhere,string whereString, object whereObj = null);
        ISugarQueryable<T> WhereIF<T2>(bool isWhere, Expression<Func<T2, bool>> expression);
        ISugarQueryable<T> WhereIF<T2, T3>(bool isWhere, Expression<Func<T2, T3, bool>> expression);
        ISugarQueryable<T> WhereIF<T2, T3, T4>(bool isWhere, Expression<Func<T2, T3, T4, bool>> expression);
        ISugarQueryable<T> WhereIF<T2, T3, T4, T5>(bool isWhere, Expression<Func<T2, T3, T4, T5, bool>> expression) ;
        ISugarQueryable<T> WhereIF<T2, T3, T4, T5, T6>(bool isWhere, Expression<Func<T2, T3, T4, T5, T6, bool>> expression);
        ISugarQueryable<T> In(params object[] pkValues);

        T InSingle(object pkValue);
        ISugarQueryable<T> In<FieldType>(string InFieldName, params FieldType[] inValues);
        ISugarQueryable<T> In<FieldType>(Expression<Func<T, object>> expression, params FieldType[] inValues);

        ISugarQueryable<T> OrderBy(string orderFileds);
        ISugarQueryable<T> OrderBy(Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T> OrderBy<T2>(Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc);

        ISugarQueryable<T> GroupBy(Expression<Func<T, object>> expression);
        ISugarQueryable<T> GroupBy(string groupFileds);

        ISugarQueryable<T> Skip(int index);
        ISugarQueryable<T> Take(int num);

        T Single();
        T Single(Expression<Func<T, bool>> expression);

        T First();
        T First(Expression<Func<T, bool>> expression);

        bool Any(Expression<Func<T, bool>> expression);
        bool Any();
        ISugarQueryable<TResult> Select<T2, TResult>(Expression<Func<T2, TResult>> expression) ;
        ISugarQueryable<TResult> Select<T2, T3, TResult>(Expression<Func<T2, T3, TResult>> expression) ;
        ISugarQueryable<TResult> Select<T2, T3, T4, TResult>(Expression<Func<T2, T3, T4, TResult>> expression);
        ISugarQueryable<TResult> Select<T2, T3, T4, T5, TResult>(Expression<Func<T2, T3, T4, T5, TResult>> expression);
        ISugarQueryable<TResult> Select<T2, T3, T4, T5,T6, TResult>(Expression<Func<T2, T3, T4, T5,T6, TResult>> expression) ;
        ISugarQueryable<TResult> Select<T2, T3, T4, T5, T6,T7, TResult>(Expression<Func<T2, T3, T4, T5, T6,T7, TResult>> expression) ;
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(string select) where TResult : class, new();
        ISugarQueryable<T> Select(string select);


        int Count();
        TResult Max<TResult>(string maxField);
        TResult Max<TResult>(Expression<Func<T, TResult>> expression);
        TResult Min<TResult>(string minField);
        TResult Min<TResult>(Expression<Func<T, TResult>> expression);
        TResult Sum<TResult>(string sumField);
        TResult Sum<TResult>(Expression<Func<T, TResult>> expression);
        TResult Avg<TResult>(string avgField);
        TResult Avg<TResult>(Expression<Func<T, TResult>> expression);

        List<T> ToList();

        string ToJson();
        string ToJsonPage(int pageIndex, int pageSize);
        string ToJsonPage(int pageIndex, int pageSize, ref int totalNumber);

        KeyValuePair<string, List<SugarParameter>> ToSql();


        DataTable ToDataTable();
        DataTable ToDataTablePage(int pageIndex, int pageSize);
        DataTable ToDataTablePage(int pageIndex, int pageSize, ref int totalNumber);

        List<T> ToPageList(int pageIndex, int pageSize);
        List<T> ToPageList(int pageIndex, int pageSize, ref int totalNumber);

      
        void Clear();
    }
}
