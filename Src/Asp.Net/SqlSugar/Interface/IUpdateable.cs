using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public interface IUpdateable<T> where T : class, new()
    {
        UpdateBuilder UpdateBuilder { get; set; }
        bool UpdateParameterIsNull { get; set; }

        int ExecuteCommand();
        bool ExecuteCommandHasChange();
        Task<int> ExecuteCommandAsync();
        Task<bool> ExecuteCommandHasChangeAsync();


        IUpdateable<T> AS(string tableName);
        IUpdateable<T> With(string lockString);


        IUpdateable<T> Where(Expression<Func<T, bool>> expression);
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
        IUpdateable<T> WhereColumns(string [] columnNames);

        /// <summary>
        /// .UpdateColumns(it=>new{ it.Name,it.Price})
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        IUpdateable<T> UpdateColumns(Expression<Func<T, object>> columns);
        IUpdateable<T> UpdateColumns(params string[] columns);


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



        IUpdateable<T> UpdateColumnsIF(bool isUpdateColumns,Expression<Func<T, object>> columns);
        IUpdateable<T> UpdateColumnsIF(bool isUpdateColumns, params string[] columns);


        IUpdateable<T> SetColumnsIF(bool isUpdateColumns,Expression<Func<T, T>> columns);
        IUpdateable<T> SetColumnsIF(bool isUpdateColumns, Expression<Func<T, bool>> columns);



        IUpdateable<T> IgnoreColumns(bool ignoreAllNullColumns, bool isOffIdentity = false, bool ignoreAllDefaultValue = false);
        IUpdateable<T> IgnoreColumns(Expression<Func<T, object>> columns);
        IUpdateable<T> IgnoreColumns(params string[] columns);



        IUpdateable<T> IsEnableUpdateVersionValidation();
        IUpdateable<T> EnableDiffLogEvent(object businessData = null);
        IUpdateable<T> ReSetValue(Expression<Func<T, bool>> setValueExpression);
        IUpdateable<T> RemoveDataCache();
        KeyValuePair<string,List<SugarParameter>> ToSql();
        void AddQueue();


        #region delete obj

        [Obsolete("Use IUpdateable<T> UpdateColumns(string [] columns)")]
        IUpdateable<T> UpdateColumns(Func<string, bool> updateColumMethod);
        [Obsolete("Use IUpdateable<T> IgnoreColumns(string [] columns)")]
        IUpdateable<T> IgnoreColumns(Func<string, bool> ignoreColumMethod);
        [Obsolete("Use IUpdateable<T> IgnoreColumns(bool ignoreAllNullColumns, bool isOffIdentity = false);")]
        IUpdateable<T> Where(bool isNoUpdateNull, bool IsOffIdentity = false);
        [Obsolete("Use IUpdateable<T> UpdateColumnsIF(bool isUpdateColumns,string [] columns")]
        IUpdateable<T> UpdateColumnsIF(bool isUpdateColumns, Func<string, bool> updateColumMethod);
        [Obsolete("Use SetColumns(Expression<Func<T, bool>> columns)")]
        IUpdateable<T> UpdateColumns(Expression<Func<T, bool>> columns);
        [Obsolete("Use SetColumns(Expression<Func<T, T>> columns")]
        IUpdateable<T> UpdateColumns(Expression<Func<T, T>> columns);

        [Obsolete("Use SetColumnsIF(bool isUpdateColumns, Expression<Func<T, T>> columns)")]
        IUpdateable<T> UpdateColumnsIF(bool isUpdateColumns, Expression<Func<T, T>> columns);
        [Obsolete("Use SetColumnsIF(bool isUpdateColumns, Expression<Func<T, bool>> columns")]
        IUpdateable<T> UpdateColumnsIF(bool isUpdateColumns, Expression<Func<T, bool>> columns);
        #endregion
    }
}
