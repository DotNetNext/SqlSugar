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
        IInsertable<T> With(string lockString);
        IInsertable<T> Update(T InsertObj);
        IInsertable<T> Where(bool isUpdateNull);
        IInsertable<T> UpdateColumns(Expression<Func<T, object[]>> columns);
        IInsertable<T> IgnoreColumns(Expression<Func<T, object[]>> columns);
        IInsertable<T> UpdateRange(List<T> InsertObjs);
    }
}
