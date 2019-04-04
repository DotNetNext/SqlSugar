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
        [Obsolete("Use IUpdateable<T> IgnoreColumns(bool ignoreAllNullColumns, bool isOffIdentity = false);")]
        IUpdateable<T> Where(bool isNoUpdateNull,bool IsOffIdentity = false);
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
        /// Non primary key entity update function
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        IUpdateable<T> WhereColumns(Expression<Func<T, object>> columns);
        IUpdateable<T> WhereColumns(string columnName);
        IUpdateable<T> WhereColumns(string [] columnNames);
        IUpdateable<T> UpdateColumns(Expression<Func<T, object>> columns);
        IUpdateable<T> UpdateColumns(Expression<Func<T, bool>> columns);
        IUpdateable<T> UpdateColumns(Func<string, bool> updateColumMethod);
        IUpdateable<T> UpdateColumns(string[] columns);
        IUpdateable<T> UpdateColumns(Expression<Func<T, T>> columns);
        IUpdateable<T> UpdateColumnsIF(bool isUpdateColumns,Expression<Func<T, object>> columns);
        IUpdateable<T> UpdateColumnsIF(bool isUpdateColumns,Expression<Func<T, bool>> columns);
        IUpdateable<T> UpdateColumnsIF(bool isUpdateColumns,Func<string, bool> updateColumMethod);
        IUpdateable<T> UpdateColumnsIF(bool isUpdateColumns,Expression<Func<T, T>> columns);
        IUpdateable<T> IgnoreColumns(bool ignoreAllNullColumns, bool isOffIdentity = false);
        IUpdateable<T> IgnoreColumns(Expression<Func<T, object>> columns);
        IUpdateable<T> IgnoreColumns(Func<string, bool> ignoreColumMethod);
        IUpdateable<T> IsEnableUpdateVersionValidation();
        IUpdateable<T> EnableDiffLogEvent(object businessData = null);
        IUpdateable<T> ReSetValue(Expression<Func<T, bool>> setValueExpression);
        IUpdateable<T> RemoveDataCache();
        KeyValuePair<string,List<SugarParameter>> ToSql();
        void AddQueue();
    }
}
