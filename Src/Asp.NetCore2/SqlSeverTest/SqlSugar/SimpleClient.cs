using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    
    public partial class SimpleClient<T> : ISimpleClient<T> where T : class, new()
    {
        #region Interface
        protected ISqlSugarClient Context { get; set; }

        public ITenant AsTenant()
        {
            return this.Context as ITenant;
        }
        public ISqlSugarClient AsSugarClient()
        {
            return this.Context;
        }

        private SimpleClient()
        {

        }
        public SimpleClient(ISqlSugarClient context)
        {
            this.Context = context;
        }

        public ISugarQueryable<T> AsQueryable()
        {
            return Context.Queryable<T>();
        }
        public IInsertable<T> AsInsertable(T insertObj)
        {
            return Context.Insertable<T>(insertObj);
        }
        public IInsertable<T> AsInsertable(T[] insertObjs)
        {
            return Context.Insertable<T>(insertObjs);
        }
        public IInsertable<T> AsInsertable(List<T> insertObjs)
        {
            return Context.Insertable<T>(insertObjs);
        }
        public IUpdateable<T> AsUpdateable(T updateObj)
        {
            return Context.Updateable<T>(updateObj);
        }
        public IUpdateable<T> AsUpdateable(T[] updateObjs)
        {
            return Context.Updateable<T>(updateObjs);
        }
        public IUpdateable<T> AsUpdateable(List<T> updateObjs)
        {
            return Context.Updateable<T>(updateObjs);
        }
        public IDeleteable<T> AsDeleteable()
        {
            return Context.Deleteable<T>();
        } 
        #endregion

        #region Method
        public virtual T GetById(dynamic id)
        {
            return Context.Queryable<T>().InSingle(id);
        }
        public virtual List<T> GetList()
        {
            return Context.Queryable<T>().ToList();
        }

        public virtual List<T> GetList(Expression<Func<T, bool>> whereExpression)
        {
            return Context.Queryable<T>().Where(whereExpression).ToList();
        }
        public virtual T GetSingle(Expression<Func<T, bool>> whereExpression)
        {
            return Context.Queryable<T>().Single(whereExpression);
        }
        public virtual List<T> GetPageList(Expression<Func<T, bool>> whereExpression, PageModel page)
        {
            int count = 0;
            var result = Context.Queryable<T>().Where(whereExpression).ToPageList(page.PageIndex, page.PageSize, ref count);
            page.PageCount = count;
            return result;
        }
        public virtual List<T> GetPageList(Expression<Func<T, bool>> whereExpression, PageModel page, Expression<Func<T, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc)
        {
            int count = 0;
            var result = Context.Queryable<T>().OrderByIF(orderByExpression != null, orderByExpression, orderByType).Where(whereExpression).ToPageList(page.PageIndex, page.PageSize, ref count);
            page.PageCount = count;
            return result;
        }
        public virtual List<T> GetPageList(List<IConditionalModel> conditionalList, PageModel page)
        {
            int count = 0;
            var result = Context.Queryable<T>().Where(conditionalList).ToPageList(page.PageIndex, page.PageSize, ref count);
            page.PageCount = count;
            return result;
        }
        public virtual List<T> GetPageList(List<IConditionalModel> conditionalList, PageModel page, Expression<Func<T, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc)
        {
            int count = 0;
            var result = Context.Queryable<T>().OrderByIF(orderByExpression != null, orderByExpression, orderByType).Where(conditionalList).ToPageList(page.PageIndex, page.PageSize, ref count);
            page.PageCount = count;
            return result;
        }
        public virtual bool IsAny(Expression<Func<T, bool>> whereExpression)
        {
            return Context.Queryable<T>().Where(whereExpression).Any();
        }
        public virtual int Count(Expression<Func<T, bool>> whereExpression)
        {

            return Context.Queryable<T>().Where(whereExpression).Count();
        }

        public virtual bool Insert(T insertObj)
        {
            return this.Context.Insertable(insertObj).ExecuteCommand() > 0;
        }
        public virtual int InsertReturnIdentity(T insertObj)
        {
            return this.Context.Insertable(insertObj).ExecuteReturnIdentity();
        }
        public virtual bool InsertRange(T[] insertObjs)
        {
            return this.Context.Insertable(insertObjs).ExecuteCommand() > 0;
        }
        public virtual bool InsertRange(List<T> insertObjs)
        {
            return this.Context.Insertable(insertObjs).ExecuteCommand() > 0;
        }
        public virtual bool Update(T updateObj)
        {
            return this.Context.Updateable(updateObj).ExecuteCommand() > 0;
        }
        public virtual bool UpdateRange(T[] updateObjs)
        {
            return this.Context.Updateable(updateObjs).ExecuteCommand() > 0;
        }
        public virtual bool UpdateRange(List<T> updateObjs)
        {
            return this.Context.Updateable(updateObjs).ExecuteCommand() > 0;
        }
        public virtual bool Update(Expression<Func<T, T>> columns, Expression<Func<T, bool>> whereExpression)
        {
            return this.Context.Updateable<T>().SetColumns(columns).Where(whereExpression).ExecuteCommand() > 0;
        }
        public virtual bool Delete(T deleteObj)
        {
            return this.Context.Deleteable<T>().Where(deleteObj).ExecuteCommand() > 0;
        }
        public virtual bool Delete(Expression<Func<T, bool>> whereExpression)
        {
            return this.Context.Deleteable<T>().Where(whereExpression).ExecuteCommand() > 0;
        }
        public virtual bool DeleteById(dynamic id)
        {
            return this.Context.Deleteable<T>().In(id).ExecuteCommand() > 0;
        }
        public virtual bool DeleteByIds(dynamic[] ids)
        {
            return this.Context.Deleteable<T>().In(ids).ExecuteCommand() > 0;
        }
        #endregion

        #region Async Method
        public virtual Task<T> GetByIdAsync(dynamic id)
        {
            return Context.Queryable<T>().InSingleAsync(id);
        }
        public virtual Task<List<T>> GetListAsync()
        {
            return Context.Queryable<T>().ToListAsync();
        }

        public virtual Task<List<T>> GetListAsync(Expression<Func<T, bool>> whereExpression)
        {
            return Context.Queryable<T>().Where(whereExpression).ToListAsync();
        }
        public virtual Task<T> GetSingleAsync(Expression<Func<T, bool>> whereExpression)
        {
            return Context.Queryable<T>().SingleAsync(whereExpression);
        }
        public virtual Task<List<T>> GetPageListAsync(Expression<Func<T, bool>> whereExpression, PageModel page)
        {
            RefAsync<int> count = 0;
            var result = Context.Queryable<T>().Where(whereExpression).ToPageListAsync(page.PageIndex, page.PageSize, count);
            page.PageCount = count;
            return result;
        }
        public virtual Task<List<T>> GetPageListAsync(Expression<Func<T, bool>> whereExpression, PageModel page, Expression<Func<T, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc)
        {
            RefAsync<int> count = 0;
            var result = Context.Queryable<T>().OrderByIF(orderByExpression != null, orderByExpression, orderByType).Where(whereExpression).ToPageListAsync(page.PageIndex, page.PageSize,  count);
            page.PageCount = count;
            return result;
        }
        public virtual Task<List<T>> GetPageListAsync(List<IConditionalModel> conditionalList, PageModel page)
        {
            RefAsync<int> count = 0;
            var result = Context.Queryable<T>().Where(conditionalList).ToPageListAsync(page.PageIndex, page.PageSize,  count);
            page.PageCount = count;
            return result;
        }
        public virtual Task<List<T>> GetPageListAsync(List<IConditionalModel> conditionalList, PageModel page, Expression<Func<T, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc)
        {
            RefAsync<int> count = 0;
            var result = Context.Queryable<T>().OrderByIF(orderByExpression != null, orderByExpression, orderByType).Where(conditionalList).ToPageListAsync(page.PageIndex, page.PageSize,  count);
            page.PageCount = count;
            return result;
        }
        public virtual Task<bool> IsAnyAsync(Expression<Func<T, bool>> whereExpression)
        {
            return Context.Queryable<T>().Where(whereExpression).AnyAsync();
        }
        public virtual Task<int> CountAsync(Expression<Func<T, bool>> whereExpression)
        {

            return Context.Queryable<T>().Where(whereExpression).CountAsync();
        }

        public virtual async Task<bool> InsertAsync(T insertObj)
        {
            return  await this.Context.Insertable(insertObj).ExecuteCommandAsync() > 0;
        }
        public virtual Task<int> InsertReturnIdentityAsync(T insertObj)
        {
            return this.Context.Insertable(insertObj).ExecuteReturnIdentityAsync();
        }
        public virtual async Task<bool> InsertRangeAsync(T[] insertObjs)
        {
            return await this.Context.Insertable(insertObjs).ExecuteCommandAsync() > 0;
        }
        public virtual async Task<bool> InsertRangeAsync(List<T> insertObjs)
        {
            return await this.Context.Insertable(insertObjs).ExecuteCommandAsync() > 0;
        }
        public virtual async Task<bool> UpdateAsync(T updateObj)
        {
            return await this.Context.Updateable(updateObj).ExecuteCommandAsync() > 0;
        }
        public virtual async Task<bool> UpdateRangeAsync(T[] updateObjs)
        {
            return await this.Context.Updateable(updateObjs).ExecuteCommandAsync() > 0;
        }
        public virtual async Task<bool> UpdateRangeAsync(List<T> updateObjs)
        {
            return await this.Context.Updateable(updateObjs).ExecuteCommandAsync() > 0;
        }
        public virtual async Task<bool> UpdateAsync(Expression<Func<T, T>> columns, Expression<Func<T, bool>> whereExpression)
        {
            return await this.Context.Updateable<T>().SetColumns(columns).Where(whereExpression).ExecuteCommandAsync() > 0;
        }
        public virtual async Task<bool> DeleteAsync(T deleteObj)
        {
            return await this.Context.Deleteable<T>().Where(deleteObj).ExecuteCommandAsync() > 0;
        }
        public virtual async Task<bool> DeleteAsync(Expression<Func<T, bool>> whereExpression)
        {
            return await this.Context.Deleteable<T>().Where(whereExpression).ExecuteCommandAsync() > 0;
        }
        public virtual async Task<bool> DeleteByIdAsync(dynamic id)
        {
            return await this.Context.Deleteable<T>().In(id).ExecuteCommand() > 0;
        }
        public virtual async Task<bool> DeleteByIdsAsync(dynamic[] ids)
        {
            return await this.Context.Deleteable<T>().In(ids).ExecuteCommandAsync() > 0;
        }
        #endregion

        [Obsolete("Use AsSugarClient()")]
        public ISqlSugarClient FullClient { get { return this.Context; } }
    }


    [Obsolete("Use SimpleClient<T>")]
    public partial class SimpleClient
    {
        protected ISqlSugarClient Context { get; set; }
        public ITenant AsTenant()
        {
            return this.Context as ITenant;
        }
        public ISqlSugarClient AsSugarClient()
        {
            return this.Context;
        }

        private SimpleClient()
        {

        }
        public SimpleClient(ISqlSugarClient context)
        {
            this.Context = context;
        }

        public T GetById<T>(dynamic id) where T : class, new()
        {
            return Context.Queryable<T>().InSingle(id);
        }
        public int Count<T>(Expression<Func<T, bool>> whereExpression)
        {
            return Context.Queryable<T>().Where(whereExpression).Count();
        }
        public List<T> GetList<T>() where T : class, new()
        {
            return Context.Queryable<T>().ToList();
        }
        public T GetSingle<T>(Expression<Func<T, bool>> whereExpression) where T : class, new()
        {
            return Context.Queryable<T>().Single(whereExpression);
        }
        public List<T> GetList<T>(Expression<Func<T, bool>> whereExpression) where T : class, new()
        {
            return Context.Queryable<T>().Where(whereExpression).ToList();
        }
        public List<T> GetPageList<T>(Expression<Func<T, bool>> whereExpression, PageModel page) where T : class, new()
        {
            int count = 0;
            var result = Context.Queryable<T>().Where(whereExpression).ToPageList(page.PageIndex, page.PageSize, ref count);
            page.PageCount = count;
            return result;
        }
        public List<T> GetPageList<T>(Expression<Func<T, bool>> whereExpression, PageModel page, Expression<Func<T, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc) where T : class, new()
        {
            int count = 0;
            var result = Context.Queryable<T>().OrderByIF(orderByExpression != null, orderByExpression, orderByType).Where(whereExpression).ToPageList(page.PageIndex, page.PageSize, ref count);
            page.PageCount = count;
            return result;
        }
        public List<T> GetPageList<T>(List<IConditionalModel> conditionalList, PageModel page) where T : class, new()
        {
            int count = 0;
            var result = Context.Queryable<T>().Where(conditionalList).ToPageList(page.PageIndex, page.PageSize, ref count);
            page.PageCount = count;
            return result;
        }
        public List<T> GetPageList<T>(List<IConditionalModel> conditionalList, PageModel page, Expression<Func<T, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc) where T : class, new()
        {
            int count = 0;
            var result = Context.Queryable<T>().OrderByIF(orderByExpression != null, orderByExpression, orderByType).Where(conditionalList).ToPageList(page.PageIndex, page.PageSize, ref count);
            page.PageCount = count;
            return result;
        }
        public bool IsAny<T>(Expression<Func<T, bool>> whereExpression) where T : class, new()
        {
            return Context.Queryable<T>().Where(whereExpression).Any();
        }
        public bool Insert<T>(T insertObj) where T : class, new()
        {
            return this.Context.Insertable(insertObj).ExecuteCommand() > 0;
        }
        public int InsertReturnIdentity<T>(T insertObj) where T : class, new()
        {
            return this.Context.Insertable(insertObj).ExecuteReturnIdentity();
        }
        public bool InsertRange<T>(T[] insertObjs) where T : class, new()
        {
            return this.Context.Insertable(insertObjs).ExecuteCommand() > 0;
        }
        public bool InsertRange<T>(List<T> insertObjs) where T : class, new()
        {
            return this.Context.Insertable(insertObjs).ExecuteCommand() > 0;
        }
        public bool Update<T>(T updateObj) where T : class, new()
        {
            return this.Context.Updateable(updateObj).ExecuteCommand() > 0;
        }
        public bool UpdateRange<T>(T[] updateObjs) where T : class, new()
        {
            return this.Context.Updateable(updateObjs).ExecuteCommand() > 0;
        }
        public bool UpdateRange<T>(List<T> updateObjs) where T : class, new()
        {
            return this.Context.Updateable(updateObjs).ExecuteCommand() > 0;
        }
        public bool Update<T>(Expression<Func<T, T>> columns, Expression<Func<T, bool>> whereExpression) where T : class, new()
        {
            return this.Context.Updateable<T>(columns).Where(whereExpression).ExecuteCommand() > 0;
        }
        public bool Delete<T>(T deleteObj) where T : class, new()
        {
            return this.Context.Deleteable<T>().Where(deleteObj).ExecuteCommand() > 0;
        }
        public bool Delete<T>(Expression<Func<T, bool>> whereExpression) where T : class, new()
        {
            return this.Context.Deleteable<T>().Where(whereExpression).ExecuteCommand() > 0;
        }
        public bool DeleteById<T>(dynamic id) where T : class, new()
        {
            return this.Context.Deleteable<T>().In(id).ExecuteCommand() > 0;
        }
        public bool DeleteByIds<T>(dynamic[] ids) where T : class, new()
        {
            return this.Context.Deleteable<T>().In(ids).ExecuteCommand() > 0;
        }
        [Obsolete("Use AsSugarClient()")]
        public ISqlSugarClient FullClient { get { return this.Context; } }
    }
}
