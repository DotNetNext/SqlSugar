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
        IUpdateable<T> With(string lockString);
        IUpdateable<T> Where(bool isUpdateNull);
        IUpdateable<T> Where(Expression<Func<T, bool>> expression);
        IUpdateable<T> UpdateColumns(Expression<Func<T, object>> columns);
        IUpdateable<T> IgnoreColumns(Expression<Func<T, object>> columns);
        IUpdateable<T> IgnoreColumns(Func<string, bool> ignoreColumMethod);
        IUpdateable<T> ReSetValue(Expression<Func<T, bool>> setValueExpression);
        KeyValuePair<string,List<SugarParameter>> ToSql();
    }
}
