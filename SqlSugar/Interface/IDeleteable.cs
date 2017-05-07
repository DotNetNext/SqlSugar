using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public interface IDeleteable<T>
    {
        int ExecuteCommand();
        IDeleteable<T> TableName(string name);
        IDeleteable<T> With(string lockString);
        IDeleteable<T> Where(T deleteObj);
        IDeleteable<T> Where(Expression<Func<T, bool>> expression);
        IDeleteable<T> Where(List<T> deleteObjs);
        IDeleteable<T> Where<PkType>(PkType primaryKeyValue);
        IDeleteable<T> Where<PkType>(PkType [] primaryKeyValues);
        IDeleteable<T> Where(string whereString,object whereObj);
    }
}
