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

        ISugarQueryable<T> AS<T2>(string tableName);
        ISugarQueryable<T> AS(string tableName);
        ISugarQueryable<T> With(string withString);
        ISugarQueryable<T> Filter(string FilterName, bool isDisabledGobalFilter= false);
        ISugarQueryable<T> AddParameters(object pars);
        ISugarQueryable<T> AddParameters(SugarParameter[] pars);
        ISugarQueryable<T> AddJoinInfo(string tableName, string shortName, string joinWhere, JoinType type = JoinType.Left);

        ISugarQueryable<T> Where(Expression<Func<T, bool>> expression);
        ISugarQueryable<T> Where(string whereString, object whereObj = null);

        ISugarQueryable<T> Having(Expression<Func<T, bool>> expression);
        ISugarQueryable<T> Having(string whereString, object whereObj = null);

        ISugarQueryable<T> WhereIF(bool isWhere, Expression<Func<T, bool>> expression);
        ISugarQueryable<T> WhereIF(bool isWhere, string whereString, object whereObj = null);

        T InSingle(object pkValue);
        ISugarQueryable<T> In<TParamter>(params TParamter[] pkValues);
        ISugarQueryable<T> In<FieldType>(string InFieldName, params FieldType[] inValues);
        ISugarQueryable<T> In<FieldType>(Expression<Func<T, object>> expression, params FieldType[] inValues);
        ISugarQueryable<T> In<TParamter>(List<TParamter> pkValues);
        ISugarQueryable<T> In<FieldType>(string InFieldName, List<FieldType> inValues);
        ISugarQueryable<T> In<FieldType>(Expression<Func<T, object>> expression, List<FieldType> inValues);
        ISugarQueryable<T> In<FieldType>(Expression<Func<T, object>> expression, ISugarQueryable<FieldType> childQueryExpression);

        ISugarQueryable<T> OrderBy(string orderFileds);
        ISugarQueryable<T> OrderBy(Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc);

        ISugarQueryable<T> GroupBy(Expression<Func<T, object>> expression);
        ISugarQueryable<T> GroupBy(string groupFileds);

        ISugarQueryable<T> PartitionBy(Expression<Func<T, object>> expression);
        ISugarQueryable<T> PartitionBy(string groupFileds);

        ISugarQueryable<T> Skip(int index);
        ISugarQueryable<T> Take(int num);

        T Single();
        T Single(Expression<Func<T, bool>> expression);

        T First();
        T First(Expression<Func<T, bool>> expression);

        bool Any(Expression<Func<T, bool>> expression);
        bool Any();
 
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(string select);
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
    public partial interface ISugarQueryable<T, T2> : ISugarQueryable<T>
    {
        #region Where
        new ISugarQueryable<T, T2> Where(Expression<Func<T, bool>> expression);
        ISugarQueryable<T, T2> Where(Expression<Func<T, T2, bool>> expression);

        new ISugarQueryable<T, T2> WhereIF(bool isWhere,Expression<Func<T, bool>> expression);
        ISugarQueryable<T, T2> WhereIF(bool isWhere, Expression<Func<T, T2, bool>> expression);

        new ISugarQueryable<T,T2> Where(string whereString, object whereObj = null);
        new ISugarQueryable<T,T2> WhereIF(bool isWhere, string whereString, object whereObj = null);
        #endregion

        #region Select
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, TResult>> expression);
        #endregion

        #region OrderBy
        new ISugarQueryable<T, T2> OrderBy(Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2> OrderBy(Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc);
        #endregion

        #region GroupBy
        new ISugarQueryable<T, T2> GroupBy(Expression<Func<T, object>> expression);
        ISugarQueryable<T, T2> GroupBy(Expression<Func<T, T2, object>> expression);
        #endregion

        #region Aggr
        TResult Max<TResult>(Expression<Func<T,T2, TResult>> expression);
        TResult Min<TResult>(Expression<Func<T,T2, TResult>> expression);
        TResult Sum<TResult>(Expression<Func<T,T2, TResult>> expression);
        TResult Avg<TResult>(Expression<Func<T,T2, TResult>> expression);
        #endregion
    }
    public partial interface ISugarQueryable<T, T2, T3> : ISugarQueryable<T>
    {
        #region Where
        new ISugarQueryable<T, T2, T3> Where(Expression<Func<T, bool>> expression);
        ISugarQueryable<T, T2, T3> Where(Expression<Func<T, T2, bool>> expression);
        ISugarQueryable<T, T2, T3> Where(Expression<Func<T, T2, T3, bool>> expression);

        new ISugarQueryable<T, T2, T3> WhereIF(bool isWhere, Expression<Func<T, bool>> expression);
        ISugarQueryable<T, T2, T3> WhereIF(bool isWhere, Expression<Func<T, T2, bool>> expression);
        ISugarQueryable<T, T2, T3> WhereIF(bool isWhere, Expression<Func<T, T2, T3, bool>> expression);

        new ISugarQueryable<T, T2,T3> Where(string whereString, object whereObj = null);
        new ISugarQueryable<T, T2,T3> WhereIF(bool isWhere, string whereString, object whereObj = null);
        #endregion

        #region Select
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, TResult>> expression);
        #endregion

        #region OrderBy
        new ISugarQueryable<T, T2, T3> OrderBy(Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3> OrderBy(Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3> OrderBy(Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc);
        #endregion

        #region GroupBy
        new ISugarQueryable<T, T2, T3> GroupBy(Expression<Func<T, object>> expression);
        ISugarQueryable<T, T2, T3> GroupBy(Expression<Func<T, T2, object>> expression);
        ISugarQueryable<T, T2, T3> GroupBy(Expression<Func<T, T2, T3, object>> expression);
        #endregion

        #region Aggr
        TResult Max<TResult>(Expression<Func<T, T2,T3, TResult>> expression);
        TResult Min<TResult>(Expression<Func<T, T2,T3, TResult>> expression);
        TResult Sum<TResult>(Expression<Func<T, T2,T3, TResult>> expression);
        TResult Avg<TResult>(Expression<Func<T, T2,T3, TResult>> expression);
        #endregion
    }
    public partial interface ISugarQueryable<T, T2, T3, T4> : ISugarQueryable<T>
    {
        #region Where
        new ISugarQueryable<T, T2, T3, T4> Where(Expression<Func<T, bool>> expression);
        ISugarQueryable<T, T2, T3, T4> Where(Expression<Func<T, T2, bool>> expression);
        ISugarQueryable<T, T2, T3, T4> Where(Expression<Func<T, T2, T3, bool>> expression);
        ISugarQueryable<T, T2, T3, T4> Where(Expression<Func<T, T2, T3, T4, bool>> expression);

        new ISugarQueryable<T, T2, T3, T4> WhereIF(bool isWhere, Expression<Func<T, bool>> expression);
        ISugarQueryable<T, T2, T3, T4> WhereIF(bool isWhere, Expression<Func<T, T2, bool>> expression);
        ISugarQueryable<T, T2, T3, T4> WhereIF(bool isWhere, Expression<Func<T, T2, T3, bool>> expression);
        ISugarQueryable<T, T2, T3, T4> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, bool>> expression);

        new ISugarQueryable<T, T2,T3,T4> Where(string whereString, object whereObj = null);
        new ISugarQueryable<T, T2, T3, T4> WhereIF(bool isWhere, string whereString, object whereObj = null);
        #endregion

        #region Select
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression);
        #endregion

        #region OrderBy
        new ISugarQueryable<T, T2, T3, T4> OrderBy(Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4> OrderBy(Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4> OrderBy(Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4> OrderBy(Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc);
        #endregion

        #region GroupBy
        new ISugarQueryable<T, T2, T3, T4> GroupBy(Expression<Func<T, object>> expression);
        ISugarQueryable<T, T2, T3, T4> GroupBy(Expression<Func<T, T2, object>> expression);
        ISugarQueryable<T, T2, T3, T4> GroupBy(Expression<Func<T, T2, T3, object>> expression);
        ISugarQueryable<T, T2, T3, T4> GroupBy(Expression<Func<T, T2, T3, T4, object>> expression);
        #endregion

        #region Aggr
        TResult Max<TResult>(Expression<Func<T, T2, T3,T4,TResult>> expression);
        TResult Min<TResult>(Expression<Func<T, T2, T3,T4,TResult>> expression);
        TResult Sum<TResult>(Expression<Func<T, T2, T3,T4,TResult>> expression);
        TResult Avg<TResult>(Expression<Func<T, T2, T3,T4,TResult>> expression);
        #endregion
    }
    public partial interface ISugarQueryable<T, T2, T3, T4, T5> : ISugarQueryable<T>
    {
        #region Where
        new ISugarQueryable<T, T2, T3, T4, T5> Where(Expression<Func<T, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5> Where(Expression<Func<T, T2, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5> Where(Expression<Func<T, T2, T3, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5> Where(Expression<Func<T, T2, T3, T4, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5> Where(Expression<Func<T, T2, T3, T4, T5, bool>> expression);


        new ISugarQueryable<T, T2, T3, T4, T5> WhereIF(bool isWhere, Expression<Func<T, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5> WhereIF(bool isWhere, Expression<Func<T, T2, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5> WhereIF(bool isWhere, Expression<Func<T, T2, T3, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, bool>> expression);

        new ISugarQueryable<T, T2, T3, T4,T5> Where(string whereString, object whereObj = null);
        new ISugarQueryable<T, T2, T3, T4,T5> WhereIF(bool isWhere, string whereString, object whereObj = null);
        #endregion

        #region Select
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression);
        #endregion

        #region OrderBy
        new ISugarQueryable<T, T2, T3, T4, T5> OrderBy(Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5> OrderBy(Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5> OrderBy(Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5> OrderBy(Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5> OrderBy(Expression<Func<T, T2, T3, T4, T5, object>> expression, OrderByType type = OrderByType.Asc);
        #endregion

        #region GroupBy
        new ISugarQueryable<T, T2, T3, T4, T5> GroupBy(Expression<Func<T, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5> GroupBy(Expression<Func<T, T2, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5> GroupBy(Expression<Func<T, T2, T3, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5> GroupBy(Expression<Func<T, T2, T3, T4, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5> GroupBy(Expression<Func<T, T2, T3, T4, T5, object>> expression);
        #endregion

        #region Aggr
        TResult Max<TResult>(Expression<Func<T, T2, T3, T4,T5,TResult>> expression);
        TResult Min<TResult>(Expression<Func<T, T2, T3, T4,T5,TResult>> expression);
        TResult Sum<TResult>(Expression<Func<T, T2, T3, T4,T5,TResult>> expression);
        TResult Avg<TResult>(Expression<Func<T, T2, T3, T4,T5,TResult>> expression);
        #endregion
    }
    public partial interface ISugarQueryable<T, T2, T3, T4, T5, T6> : ISugarQueryable<T>
    {
        #region Where
        new ISugarQueryable<T, T2, T3, T4, T5, T6> Where(Expression<Func<T, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6> Where(Expression<Func<T, T2, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6> Where(Expression<Func<T, T2, T3, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6> Where(Expression<Func<T, T2, T3, T4, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6> Where(Expression<Func<T, T2, T3, T4, T5, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6> Where(Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression);

        new ISugarQueryable<T, T2, T3, T4, T5, T6> WhereIF(bool isWhere, Expression<Func<T, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6> WhereIF(bool isWhere, Expression<Func<T, T2, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6> WhereIF(bool isWhere, Expression<Func<T, T2, T3, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression);

        new ISugarQueryable<T, T2, T3, T4, T5 ,T6> Where(string whereString, object whereObj = null);
        new ISugarQueryable<T, T2, T3, T4, T5,T6> WhereIF(bool isWhere, string whereString, object whereObj = null);
        #endregion

        #region Select
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression);
        #endregion

        #region OrderBy
        new ISugarQueryable<T, T2, T3, T4, T5, T6> OrderBy(Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6> OrderBy(Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6> OrderBy(Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6> OrderBy(Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6> OrderBy(Expression<Func<T, T2, T3, T4, T5, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression, OrderByType type = OrderByType.Asc);
        #endregion

        #region GroupBy
        new ISugarQueryable<T, T2, T3, T4, T5, T6> GroupBy(Expression<Func<T, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6> GroupBy(Expression<Func<T, T2, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6> GroupBy(Expression<Func<T, T2, T3, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6> GroupBy(Expression<Func<T, T2, T3, T4, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6> GroupBy(Expression<Func<T, T2, T3, T4, T5, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression);
        #endregion

        #region Aggr
        TResult Max<TResult>(Expression<Func<T, T2, T3, T4, T5,T6,TResult>> expression);
        TResult Min<TResult>(Expression<Func<T, T2, T3, T4, T5,T6,TResult>> expression);
        TResult Sum<TResult>(Expression<Func<T, T2, T3, T4, T5,T6,TResult>> expression);
        TResult Avg<TResult>(Expression<Func<T, T2, T3, T4, T5,T6,TResult>> expression);
        #endregion
    }
    public partial interface ISugarQueryable<T, T2, T3, T4, T5, T6, T7> : ISugarQueryable<T>
    {
        #region Where
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Where(Expression<Func<T, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Where(Expression<Func<T, T2, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Where(Expression<Func<T, T2, T3, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Where(Expression<Func<T, T2, T3, T4, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Where(Expression<Func<T, T2, T3, T4, T5, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Where(Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression);

        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> WhereIF(bool isWhere, Expression<Func<T, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7> WhereIF(bool isWhere, Expression<Func<T, T2, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7> WhereIF(bool isWhere, Expression<Func<T, T2, T3, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression);

        new ISugarQueryable<T, T2, T3, T4, T5, T6,T7> Where(string whereString, object whereObj = null);
        new ISugarQueryable<T, T2, T3, T4, T5, T6,T7> WhereIF(bool isWhere, string whereString, object whereObj = null);
        #endregion

        #region Select
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> expression);
        #endregion

        #region OrderBy
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> OrderBy(Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7> OrderBy(Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7> OrderBy(Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7> OrderBy(Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7> OrderBy(Expression<Func<T, T2, T3, T4, T5, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression, OrderByType type = OrderByType.Asc);
        #endregion

        #region GroupBy
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> GroupBy(Expression<Func<T, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7> GroupBy(Expression<Func<T, T2, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7> GroupBy(Expression<Func<T, T2, T3, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7> GroupBy(Expression<Func<T, T2, T3, T4, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7> GroupBy(Expression<Func<T, T2, T3, T4, T5, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression);
        #endregion

        #region Aggr
        TResult Max<TResult>(Expression<Func<T, T2, T3, T4, T5, T6,T7,TResult>> expression);
        TResult Min<TResult>(Expression<Func<T, T2, T3, T4, T5, T6,T7,TResult>> expression);
        TResult Sum<TResult>(Expression<Func<T, T2, T3, T4, T5, T6,T7,TResult>> expression);
        TResult Avg<TResult>(Expression<Func<T, T2, T3, T4, T5, T6,T7,TResult>> expression);
        #endregion
    }
    public partial interface ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> : ISugarQueryable<T>
    {
        #region Where
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Where(Expression<Func<T, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Where(Expression<Func<T, T2, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Where(Expression<Func<T, T2, T3, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Where(Expression<Func<T, T2, T3, T4, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Where(Expression<Func<T, T2, T3, T4, T5, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Where(Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression);

        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> WhereIF(bool isWhere, Expression<Func<T, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> WhereIF(bool isWhere, Expression<Func<T, T2, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> WhereIF(bool isWhere, Expression<Func<T, T2, T3, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression);

        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7,T8> Where(string whereString, object whereObj = null);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7,T8> WhereIF(bool isWhere, string whereString, object whereObj = null);
        #endregion                              

        #region Select
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> expression);
        #endregion

        #region OrderBy
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> OrderBy(Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> OrderBy(Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> OrderBy(Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> OrderBy(Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> OrderBy(Expression<Func<T, T2, T3, T4, T5, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression, OrderByType type = OrderByType.Asc);
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
        TResult Max<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7,T8, TResult>> expression);
        TResult Min<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7,T8, TResult>> expression);
        TResult Sum<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7,T8, TResult>> expression);
        TResult Avg<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7,T8, TResult>> expression);
        #endregion
    }

    #region 9-12
    public partial interface ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9> : ISugarQueryable<T>
    {
        #region Where
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9> Where(Expression<Func<T, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9> Where(Expression<Func<T, T2, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9> Where(Expression<Func<T, T2, T3, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9> Where(Expression<Func<T, T2, T3, T4, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9> Where(Expression<Func<T, T2, T3, T4, T5, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9> Where(Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8,T9, bool>> expression);

        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9> WhereIF(bool isWhere, Expression<Func<T, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9> WhereIF(bool isWhere, Expression<Func<T, T2, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9> WhereIF(bool isWhere, Expression<Func<T, T2, T3, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8,T9, bool>> expression);

        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9> Where(string whereString, object whereObj = null);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9> WhereIF(bool isWhere, string whereString, object whereObj = null);
        #endregion                              

        #region Select
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> expression);
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8,T9, TResult>> expression);
        #endregion

        #region OrderBy
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderBy(Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderBy(Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderBy(Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderBy(Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderBy(Expression<Func<T, T2, T3, T4, T5, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8,T9, object>> expression, OrderByType type = OrderByType.Asc);
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
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8,T9, object>> expression);
        #endregion

        #region Aggr
        TResult Max<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8,T9, TResult>> expression);
        TResult Min<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8,T9, TResult>> expression);
        TResult Sum<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8,T9, TResult>> expression);
        TResult Avg<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8,T9, TResult>> expression);
        #endregion
    }
    public partial interface ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9,T10> : ISugarQueryable<T>
    {
        #region Where
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9,T10> Where(Expression<Func<T, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9,T10> Where(Expression<Func<T, T2, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9,T10> Where(Expression<Func<T, T2, T3, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9,T10> Where(Expression<Func<T, T2, T3, T4, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9,T10> Where(Expression<Func<T, T2, T3, T4, T5, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9,T10> Where(Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9,T10> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9,T10> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9,T10> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9,T10, bool>> expression);

        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9,T10> WhereIF(bool isWhere, Expression<Func<T, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9,T10> WhereIF(bool isWhere, Expression<Func<T, T2, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9,T10> WhereIF(bool isWhere, Expression<Func<T, T2, T3, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9,T10> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9,T10> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9,T10> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9,T10> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9,T10> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9,T10> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9,T10, bool>> expression);

        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9,T10> Where(string whereString, object whereObj = null);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9,T10> WhereIF(bool isWhere, string whereString, object whereObj = null);
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
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9,T10, TResult>> expression);
        #endregion

        #region OrderBy
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9,T10> OrderBy(Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9,T10> OrderBy(Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9,T10> OrderBy(Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9,T10> OrderBy(Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9,T10> OrderBy(Expression<Func<T, T2, T3, T4, T5, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9,T10> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9,T10> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9,T10> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9,T10> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9,T10> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9,T10, object>> expression, OrderByType type = OrderByType.Asc);
        #endregion

        #region GroupBy
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9,T10> GroupBy(Expression<Func<T, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9,T10> GroupBy(Expression<Func<T, T2, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9,T10> GroupBy(Expression<Func<T, T2, T3, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9,T10> GroupBy(Expression<Func<T, T2, T3, T4, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9,T10> GroupBy(Expression<Func<T, T2, T3, T4, T5, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9,T10> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9,T10> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9,T10> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9,T10> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9,T10> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9,T10, object>> expression);
        #endregion

        #region Aggr
        TResult Max<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9,T10, TResult>> expression);
        TResult Min<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9,T10, TResult>> expression);
        TResult Sum<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9,T10, TResult>> expression);
        TResult Avg<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9,T10, TResult>> expression);
        #endregion
    }
    public partial interface ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9,T10,T11> : ISugarQueryable<T>
    {
        #region Where
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11> Where(Expression<Func<T, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11> Where(Expression<Func<T, T2, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11> Where(Expression<Func<T, T2, T3, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11> Where(Expression<Func<T, T2, T3, T4, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11> Where(Expression<Func<T, T2, T3, T4, T5, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11> Where(Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11, bool>> expression);

        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11> WhereIF(bool isWhere, Expression<Func<T, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11> WhereIF(bool isWhere, Expression<Func<T, T2, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11> WhereIF(bool isWhere, Expression<Func<T, T2, T3, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11, bool>> expression);

        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11> Where(string whereString, object whereObj = null);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11> WhereIF(bool isWhere, string whereString, object whereObj = null);
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
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11, TResult>> expression);
        #endregion

        #region OrderBy
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
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11, object>> expression, OrderByType type = OrderByType.Asc);
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
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9, T10, T11> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11, object>> expression);
        #endregion

        #region Aggr
        TResult Max<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11, TResult>> expression);
        TResult Min<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11, TResult>> expression);
        TResult Sum<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11, TResult>> expression);
        TResult Avg<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11, TResult>> expression);
        #endregion
    }
    public partial interface ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9,T10,T11,T12> : ISugarQueryable<T>
    {
        #region Where
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> Where(Expression<Func<T, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> Where(Expression<Func<T, T2, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> Where(Expression<Func<T, T2, T3, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> Where(Expression<Func<T, T2, T3, T4, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> Where(Expression<Func<T, T2, T3, T4, T5, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> Where(Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12, bool>> expression);

        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> WhereIF(bool isWhere, Expression<Func<T, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> WhereIF(bool isWhere, Expression<Func<T, T2, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> WhereIF(bool isWhere, Expression<Func<T, T2, T3, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12, bool>> expression);

        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> Where(string whereString, object whereObj = null);
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> WhereIF(bool isWhere, string whereString, object whereObj = null);
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
        ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12, TResult>> expression);
        #endregion

        #region OrderBy
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderBy(Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> OrderBy(Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> OrderBy(Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> OrderBy(Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> OrderBy(Expression<Func<T, T2, T3, T4, T5, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, object>> expression, OrderByType type = OrderByType.Asc);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12, object>> expression, OrderByType type = OrderByType.Asc);
        #endregion

        #region GroupBy
        new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> GroupBy(Expression<Func<T, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> GroupBy(Expression<Func<T, T2, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> GroupBy(Expression<Func<T, T2, T3, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> GroupBy(Expression<Func<T, T2, T3, T4, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> GroupBy(Expression<Func<T, T2, T3, T4, T5, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, object>> expression);
        ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12, object>> expression);
        #endregion

        #region Aggr
        TResult Max<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12, TResult>> expression);
        TResult Min<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12, TResult>> expression);
        TResult Sum<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12, TResult>> expression);
        TResult Avg<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12, TResult>> expression);
        #endregion
    }
    #endregion
}
