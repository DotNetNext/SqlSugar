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
        IInsertable<T> TableName(string name);
        IInsertable<T> With(string lockString);
        IInsertable<T> Where(T deleteObj);
        IInsertable<T> Where(Expression<Func<T, bool>> expression);
        IInsertable<T> Where(List<T> deleteObjs);
        IInsertable<T> Where<PkType>(PkType primaryKeyValue);
        IInsertable<T> Where<PkType>(PkType [] primaryKeyValues);
        IInsertable<T> Where(string whereString,object whereObj);
    }
}
