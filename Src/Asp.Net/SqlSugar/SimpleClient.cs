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
        public List<T> GetList<T>(Expression<Func<T, bool>> whereExpression) where T : class, new()
        {
            return Context.Queryable<T>().Where(whereExpression).ToList();
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
}
