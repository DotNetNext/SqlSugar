using System;
using System.Linq.Expressions;

namespace SqlSugar
{
    public interface ISubInsertable<T>  
    {
        ISubInsertable<T> AddSubList(Expression<Func<T, object>> items);
        ISubInsertable<T> AddSubList(Expression<Func<T, SubInsertTree>> tree);
        object ExecuteReturnPrimaryKey();
    }
}