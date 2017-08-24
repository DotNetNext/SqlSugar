using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public interface IUpdateable<T>
    {
        int ExecuteCommand();
        IUpdateable<T> AS(string tableName);
        IUpdateable<T> With(string lockString);
        IUpdateable<T> Where(bool isNoUpdateNull,bool IsOffIdentity = false);
        IUpdateable<T> Where(Expression<Func<T, bool>> expression);
        /// <summary>
        /// Non primary key entity update function
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        IUpdateable<T> WhereColumns(Expression<Func<T, object>> columns);
        IUpdateable<T> UpdateColumns(Expression<Func<T, object>> columns);
        IUpdateable<T> UpdateColumns(Func<string, bool> updateColumMethod);
        IUpdateable<T> UpdateColumns(Expression<Func<T, T>> columns);
        IUpdateable<T> IgnoreColumns(Expression<Func<T, object>> columns);
        IUpdateable<T> IgnoreColumns(Func<string, bool> ignoreColumMethod);
        IUpdateable<T> ReSetValue(Expression<Func<T, bool>> setValueExpression);
        KeyValuePair<string,List<SugarParameter>> ToSql();
    }
}
