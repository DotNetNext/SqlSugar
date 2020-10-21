using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SqlSugar
{
    public interface ISimpleClient<T> where T : class, new()
    {
        IDeleteable<T> AsDeleteable();
        IInsertable<T> AsInsertable(List<T> insertObjs);
        IInsertable<T> AsInsertable(T insertObj);
        IInsertable<T> AsInsertable(T[] insertObjs);
        ISugarQueryable<T> AsQueryable();
        ISqlSugarClient AsSugarClient();
        ITenant AsTenant();
        IUpdateable<T> AsUpdateable(List<T> updateObjs);
        IUpdateable<T> AsUpdateable(T updateObj);
        IUpdateable<T> AsUpdateable(T[] updateObjs);
        int Count(Expression<Func<T, bool>> whereExpression);
        bool Delete(Expression<Func<T, bool>> whereExpression);
        bool Delete(T deleteObj);
        bool DeleteById(dynamic id);
        bool DeleteByIds(dynamic[] ids);
        T GetById(dynamic id);
        List<T> GetList();
        List<T> GetList(Expression<Func<T, bool>> whereExpression);
        List<T> GetPageList(Expression<Func<T, bool>> whereExpression, PageModel page);
        List<T> GetPageList(Expression<Func<T, bool>> whereExpression, PageModel page, Expression<Func<T, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc);
        List<T> GetPageList(List<IConditionalModel> conditionalList, PageModel page);
        List<T> GetPageList(List<IConditionalModel> conditionalList, PageModel page, Expression<Func<T, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc);
        T GetSingle(Expression<Func<T, bool>> whereExpression);
        bool Insert(T insertObj);
        bool InsertRange(List<T> insertObjs);
        bool InsertRange(T[] insertObjs);
        int InsertReturnIdentity(T insertObj);
        bool IsAny(Expression<Func<T, bool>> whereExpression);
        bool Update(Expression<Func<T, T>> columns, Expression<Func<T, bool>> whereExpression);
        bool Update(T updateObj);
        bool UpdateRange(List<T> updateObjs);
        bool UpdateRange(T[] updateObjs);
    }
}