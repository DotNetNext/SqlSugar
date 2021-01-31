using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public interface IDeleteable<T> where T : class, new()
    {
        DeleteBuilder DeleteBuilder { get; set; }
        int ExecuteCommand();
        bool ExecuteCommandHasChange();
        Task<int> ExecuteCommandAsync();
        Task<bool> ExecuteCommandHasChangeAsync();
        IDeleteable<T> AS(string tableName);
        IDeleteable<T> With(string lockString);
        IDeleteable<T> Where(T deleteObj);
        IDeleteable<T> Where(Expression<Func<T, bool>> expression);
        IDeleteable<T> Where(List<T> deleteObjs);
        IDeleteable<T> In<PkType>(PkType primaryKeyValue);
        IDeleteable<T> In<PkType>(PkType[] primaryKeyValues);
        IDeleteable<T> In<PkType>(List<PkType> primaryKeyValues);
        IDeleteable<T> In<PkType>(Expression<Func<T,object>> inField,PkType primaryKeyValue);
        IDeleteable<T> In<PkType>(Expression<Func<T, object>> inField,PkType[] primaryKeyValues);
        IDeleteable<T> In<PkType>(Expression<Func<T, object>> inField,List<PkType> primaryKeyValues);
        IDeleteable<T> Where(string whereString,object parameters=null);
        IDeleteable<T> Where(string whereString, SugarParameter parameter);
        IDeleteable<T> Where(string whereString, SugarParameter[] parameters);
        IDeleteable<T> Where(string whereString, List<SugarParameter> parameters);
        IDeleteable<T> WhereColumns(Expression<Func<T, object>> columns);
        IDeleteable<T> EnableDiffLogEvent(object businessData = null);
        IDeleteable<T> RemoveDataCache();
        KeyValuePair<string, List<SugarParameter>> ToSql();
        void AddQueue();
    }
}
