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
        IUpdateable<T> Update(T InsertObj);
        IUpdateable<T> Where(bool isUpdateNull);
        IUpdateable<T> UpdateColumns(Expression<Func<T, object>> columns);
        IUpdateable<T> IgnoreColumns(Expression<Func<T, object>> columns);
        IUpdateable<T> IgnoreColumns(Func<string, bool> ignoreColumMethod);
        IUpdateable<T> ReSetValue(Func<T, bool> setValueExpression);
        IUpdateable<T> UpdateRange(List<T> InsertObjs);
        KeyValuePair<string,List<SugarParameter>> ToSql();
    }
}
