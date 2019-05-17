using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public partial interface ISaveable<T> where T : class, new()
    {
        Task<int> ExecuteCommandAsync();
        Task<T> ExecuteReturnEntityAsync();

        Task<List<T>> ExecuteReturnListAsync();
        int ExecuteCommand();
        T ExecuteReturnEntity();
        List<T> ExecuteReturnList();
        ISaveable<T> InsertColumns(Expression<Func<T, object>> columns);
        ISaveable<T> InsertIgnoreColumns(Expression<Func<T, object>> columns);
        ISaveable<T> UpdateColumns(Expression<Func<T, object>> columns);
        ISaveable<T> UpdateIgnoreColumns(Expression<Func<T, object>> columns);
        ISaveable<T> UpdateWhereColumns(Expression<Func<T, object>> columns);
        ISaveable<T> EnableDiffLogEvent(object businessData = null);
    }
}
