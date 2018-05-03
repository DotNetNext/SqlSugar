using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar
{
    public partial class SimpleClient
    {
        protected SqlSugarClient Context { get; set; }
        public SqlSugarClient FullClient { get { return this.Context; } }

        private SimpleClient()
        {

        }
        public SimpleClient(SqlSugarClient context)
        {
            this.Context = context;
        }

        public T GetById<T>(dynamic id) where T : class, new()
        {
            return Context.Queryable<T>().InSingle(id);
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
        public bool InsertRange<T>(List<T>[] insertObjs) where T : class, new()
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
        public bool Update<T>(Expression<Func<T, T>> columns, Expression<Func<T, bool>> whereExpression) where T : class, new()
        {
            return this.Context.Updateable<T>().UpdateColumns(columns).Where(whereExpression).ExecuteCommand() > 0;
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
    }
    public partial class SimpleClient<T> where T : class, new()
    {
        protected SqlSugarClient Context { get; set; }
        public SqlSugarClient FullClient { get { return this.Context; } }

        private SimpleClient()
        {

        }
        public SimpleClient(SqlSugarClient context)
        {
            this.Context = context;
        }

        public T GetById(dynamic id)
        {
            return Context.Queryable<T>().InSingle(id);
        }
        public List<T> GetList()
        {
            return Context.Queryable<T>().ToList();
        }
        public List<T> GetList(Expression<Func<T, bool>> whereExpression)
        {
            return Context.Queryable<T>().Where(whereExpression).ToList();
        }
        public T GetSingle(Expression<Func<T, bool>> whereExpression)
        {
            return Context.Queryable<T>().Single(whereExpression);
        }
        public List<T> GetPageList(Expression<Func<T, bool>> whereExpression, PageModel page)
        {
            int count = 0;
            var result = Context.Queryable<T>().Where(whereExpression).ToPageList(page.PageIndex, page.PageSize, ref count);
            page.PageCount = count;
            return result;
        }
        public List<T> GetPageList(Expression<Func<T, bool>> whereExpression, PageModel page, Expression<Func<T, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc) 
        {
            int count = 0;
            var result = Context.Queryable<T>().OrderByIF(orderByExpression != null, orderByExpression, orderByType).Where(whereExpression).ToPageList(page.PageIndex, page.PageSize, ref count);
            page.PageCount = count;
            return result;
        }
        public List<T> GetPageList(List<IConditionalModel> conditionalList, PageModel page)
        {
            int count = 0;
            var result = Context.Queryable<T>().Where(conditionalList).ToPageList(page.PageIndex, page.PageSize, ref count);
            page.PageCount = count;
            return result;
        }
        public List<T> GetPageList(List<IConditionalModel> conditionalList, PageModel page, Expression<Func<T, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc)  
        {
            int count = 0;
            var result = Context.Queryable<T>().OrderByIF(orderByExpression != null, orderByExpression, orderByType).Where(conditionalList).ToPageList(page.PageIndex, page.PageSize, ref count);
            page.PageCount = count;
            return result;
        }
        public bool IsAny(Expression<Func<T, bool>> whereExpression)
        {
            return Context.Queryable<T>().Where(whereExpression).Any();
        }
        public int Count(Expression<Func<T, bool>> whereExpression)
        {

            return Context.Queryable<T>().Where(whereExpression).Count();
        }

        public bool Insert(T insertObj)
        {
            return this.Context.Insertable(insertObj).ExecuteCommand() > 0;
        }
        public int InsertReturnIdentity(T insertObj)
        {
            return this.Context.Insertable(insertObj).ExecuteReturnIdentity();
        }
        public bool InsertRange(T[] insertObjs)
        {
            return this.Context.Insertable(insertObjs).ExecuteCommand() > 0;
        }
        public bool InsertRange(List<T>[] insertObjs)
        {
            return this.Context.Insertable(insertObjs).ExecuteCommand() > 0;
        }
        public bool Update(T updateObj)
        {
            return this.Context.Updateable(updateObj).ExecuteCommand() > 0;
        }
        public bool UpdateRange(T [] updateObjs)
        {
            return this.Context.Updateable(updateObjs).ExecuteCommand() > 0;
        }
        public bool Update(Expression<Func<T, T>> columns, Expression<Func<T, bool>> whereExpression)
        {
            return this.Context.Updateable<T>().UpdateColumns(columns).Where(whereExpression).ExecuteCommand() > 0;
        }
        public bool Delete(T deleteObj)
        {
            return this.Context.Deleteable<T>().Where(deleteObj).ExecuteCommand() > 0;
        }
        public bool Delete(Expression<Func<T, bool>> whereExpression)
        {
            return this.Context.Deleteable<T>().Where(whereExpression).ExecuteCommand() > 0;
        }
        public bool DeleteById(dynamic id)
        {
            return this.Context.Deleteable<T>().In(id).ExecuteCommand() > 0;
        }
        public bool DeleteByIds(dynamic[] ids)
        {
            return this.Context.Deleteable<T>().In(ids).ExecuteCommand() > 0;
        }
    }
}
