using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public class DeleteableProvider<T> : IDeleteable<T> where T : class, new()
    {
        public SqlSugarClient Context { get; set; }
        public IDb Db { get { return Context.Database; } }
        public ISqlBuilder SqlBuilder { get; set; }
        public DeleteBuilder DeleteBuilder { get; set; }

        public int ExecuteCommand()
        {
            throw new NotImplementedException();
        }

        public IDeleteable<T> TableName(string name)
        {
            throw new NotImplementedException();
        }

        public IDeleteable<T> Where(List<T> deleteObjs)
        {
            throw new NotImplementedException();
        }

        public IDeleteable<T> Where(Expression<Func<T, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public IDeleteable<T> Where(T deleteObj)
        {
            throw new NotImplementedException();
        }

        public IDeleteable<T> Where(string whereString, object whereObj)
        {
            throw new NotImplementedException();
        }

        public IDeleteable<T> Where<PkType>(PkType[] primaryKeyValues)
        {
            throw new NotImplementedException();
        }

        public IDeleteable<T> Where<PkType>(PkType primaryKeyValue)
        {
            throw new NotImplementedException();
        }

        public IDeleteable<T> With(string lockString)
        {
            throw new NotImplementedException();
        }
    }
}
