using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SqlSugar
{
    public interface ISubInsertable<T>  
    {
        ISubInsertable<T> AddSubList(Expression<Func<T, object>> items);
        ISubInsertable<T> AddSubList(Expression<Func<T, SubInsertTree>> tree);
        [Obsolete("use ExecuteCommand")]
        object ExecuteReturnPrimaryKey();
        object ExecuteCommand();
        Task<object> ExecuteCommandAsync();
    }
}