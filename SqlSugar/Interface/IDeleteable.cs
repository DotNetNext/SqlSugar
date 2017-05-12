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
        IDeleteable<T> With(string lockString);
        IDeleteable<T> Where(T deleteObj);
        IDeleteable<T> Where(Expression<Func<T, bool>> expression);
        IDeleteable<T> Where(List<T> deleteObjs);
        IDeleteable<T> In<PkType>(PkType primaryKeyValue);
        IDeleteable<T> In<PkType>(PkType [] primaryKeyValues);
        IDeleteable<T> Where(string whereString,object whereObj=null);
        KeyValuePair<string, List<SugarParameter>> ToSql();
    }
}
