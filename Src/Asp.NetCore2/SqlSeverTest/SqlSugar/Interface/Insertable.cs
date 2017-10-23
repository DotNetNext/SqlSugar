using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public interface IInsertable<T>
    {
        InsertBuilder InsertBuilder { get; set; }
        int ExecuteCommand();
        Task<int> ExecuteCommandAsync();
        int ExecuteReturnIdentity();
        Task<int> ExecuteReturnIdentityAsync();
        T ExecuteReturnEntity();
        Task<T> ExecuteReturnEntityAsync();
        bool ExecuteCommandIdentityIntoEntity();
        Task<bool> ExecuteCommandIdentityIntoEntityAsync();
        long ExecuteReturnBigIdentity();
        Task<long> ExecuteReturnBigIdentityAsync();
        IInsertable<T> AS(string tableName);
        IInsertable<T> With(string lockString);
        IInsertable<T> InsertColumns(Expression<Func<T, object>> columns);
        IInsertable<T> InsertColumns(Func<string, bool> insertColumMethod);
        IInsertable<T> IgnoreColumns(Expression<Func<T, object>> columns);
        IInsertable<T> IgnoreColumns(Func<string,bool> ignoreColumMethod);
        IInsertable<T> Where(bool isInsertNull, bool isOffIdentity = false);
        IInsertable<T> RemoveDataCache();
        KeyValuePair<string, List<SugarParameter>> ToSql();
    }
}
