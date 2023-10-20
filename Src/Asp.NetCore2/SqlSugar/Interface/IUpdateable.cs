using NetTaste;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SqlSugar
{
    public interface IUpdateable<T> where T : class, new()
    {
        UpdateBuilder UpdateBuilder { get; set; }
        bool UpdateParameterIsNull { get; set; }

        int ExecuteCommandWithOptLock(bool isThrowError = false);
        int ExecuteCommandWithOptLockIF(bool? IsVersionValidation, bool? IsOptLock = null);
        Task<int> ExecuteCommandWithOptLockAsync(bool isThrowError = false);
        int ExecuteCommand();
        bool ExecuteCommandHasChange();
        Task<int> ExecuteCommandAsync();
        Task<int> ExecuteCommandAsync(CancellationToken token);
        Task<bool> ExecuteCommandHasChangeAsync();
        Task<bool> ExecuteCommandHasChangeAsync(CancellationToken token);


        IUpdateable<T> AS(string tableName);
        IUpdateable<T> AsType(Type tableNameType);
        IUpdateable<T> With(string lockString);


        IUpdateable<T> Where(Expression<Func<T, bool>> expression);
        IUpdateable<T> WhereIF(bool isWhere,Expression<Func<T, bool>> expression);
        IUpdateable<T> Where(string whereSql,object parameters=null);
        /// <summary>
        ///  
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="conditionalType">for example : = </param>
        /// <param name="fieldValue"></param>
        /// <returns></returns>
        IUpdateable<T> Where(string fieldName, string conditionalType, object fieldValue);



        /// <summary>
        /// Non primary key entity update function,.WhereColumns(it=>new{ it.Id })
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        IUpdateable<T> WhereColumns(Expression<Func<T, object>> columns);
        IUpdateable<T> WhereColumns(string columnName);
        IUpdateable<T> WhereColumns(params string [] columnNames);
        IUpdateable<T> Where(List<IConditionalModel> conditionalModels);

        /// <summary>
        /// .UpdateColumns(it=>new{ it.Name,it.Price})
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        IUpdateable<T> UpdateColumns(Expression<Func<T, object>> columns);
        IUpdateable<T> UpdateColumns(Expression<Func<T, object>> columns, bool appendColumnsByDataFilter);
        IUpdateable<T> UpdateColumns(params string[] columns);
        IUpdateable<T> UpdateColumns(string[] columns,bool appendColumnsByDataFilter);


        /// <summary>
        ///.SetColumns(it=>it.Name=="a")
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        IUpdateable<T> SetColumns(Expression<Func<T, bool>> columns);
        /// <summary>
        /// .SetColumns(it=> new class() { it.Name="a",it.Price=0})
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        IUpdateable<T> SetColumns(Expression<Func<T, T>> columns);
        IUpdateable<T> SetColumns(Expression<Func<T, T>> columns,bool appendColumnsByDataFilter);
        IUpdateable<T> SetColumns(string fieldName,object fieldValue);

        IUpdateable<T> SetColumns(Expression<Func<T,object>> filedNameExpression, object fieldValue);
        IUpdateable<T> SetColumns(Expression<Func<T, object>> filedNameExpression, Expression<Func<T, object>> valueExpression);
        IUpdateable<T> SetColumnsIF(bool isUpdateColumns, Expression<Func<T, object>> filedNameExpression, object fieldValue);
        IUpdateable<T> UpdateColumnsIF(bool isUpdateColumns,Expression<Func<T, object>> columns);
        IUpdateable<T> UpdateColumnsIF(bool isUpdateColumns, params string[] columns);


        IUpdateable<T> SetColumnsIF(bool isUpdateColumns,Expression<Func<T, T>> columns);
        IUpdateable<T> SetColumnsIF(bool isUpdateColumns, Expression<Func<T, bool>> columns);



        IUpdateable<T> IgnoreColumns(bool ignoreAllNullColumns, bool isOffIdentity = false, bool ignoreAllDefaultValue = false);
        IUpdateable<T> IgnoreColumns(Expression<Func<T, object>> columns);
        IUpdateable<T> IgnoreColumnsIF(bool isIgnore, Expression<Func<T, object>> columns);

        IUpdateable<T> IgnoreColumns(params string[] columns);
        IUpdateable<T> IgnoreNullColumns(bool isIgnoreNull=true);


        IUpdateable<T> IsEnableUpdateVersionValidation();
        IUpdateable<T> EnableDiffLogEvent(object businessData = null);
        IUpdateable<T> EnableDiffLogEventIF(bool isEnableDiffLog,object businessData = null);
        IUpdateable<T> ReSetValue(Action<T> setValueExpression);
        IUpdateable<T> PublicSetColumns(Expression<Func<T, object>> filedNameExpression,string computationalSymbol);
        IUpdateable<T> PublicSetColumns (Expression<Func<T,object>> filedNameExpression, Expression<Func<T, object>> ValueExpExpression);
        IUpdateable<T> RemoveDataCache();
        IUpdateable<T> RemoveDataCache(string likeString);
        IUpdateable<T> CallEntityMethod(Expression<Action<T>> method);
        KeyValuePair<string,List<SugarParameter>> ToSql();
        string ToSqlString();
        void AddQueue();
        SplitTableUpdateProvider<T> SplitTable(Func<List<SplitTableInfo>, IEnumerable<SplitTableInfo>> getTableNamesFunc);
        SplitTableUpdateByObjectProvider<T> SplitTable();
        IUpdateable<T> EnableQueryFilter();
        IUpdateable<T> Clone();
        IUpdateable<T,T2> InnerJoin<T2>(Expression<Func<T,T2,bool>> joinExpress);
        IUpdateable<T, T2> InnerJoin<T2>(Expression<Func<T, T2, bool>> joinExpress,string tableName);
        UpdateablePage<T> PageSize(int pageSize);
        IUpdateable<T> In(object[] ids);
    }
    public interface IUpdateable<T, T2> 
    {
        int ExecuteCommand();
        Task<int> ExecuteCommandAsync();
        IUpdateable<T, T2,T3> InnerJoin<T3>(Expression<Func<T, T2,T3, bool>> joinExpress);
        IUpdateable<T, T2> SetColumns(Expression<Func<T, T2,T>> columns);
        IUpdateable<T, T2> Where(Expression<Func<T, T2,bool>> whereExpression);
    }
    public interface IUpdateable<T, T2,T3>
    {
        IUpdateable<T, T2, T3,T4> InnerJoin<T4>(Expression<Func<T, T2, T3,T4, bool>> joinExpress);
        int ExecuteCommand();
        Task<int> ExecuteCommandAsync();
        IUpdateable<T, T2,T3> SetColumns(Expression<Func<T, T2,T3, T>> columns);
        IUpdateable<T, T2,T3> Where(Expression<Func<T, T2,T3, bool>> whereExpression);
    }
    public interface IUpdateable<T, T2, T3,T4>
    {
        int ExecuteCommand();
        Task<int> ExecuteCommandAsync();
        IUpdateable<T, T2, T3,T4> SetColumns(Expression<Func<T, T2, T3,T4, T>> columns);
        IUpdateable<T, T2, T3,T4> Where(Expression<Func<T, T2, T3,T4, bool>> whereExpression);
    }
}
