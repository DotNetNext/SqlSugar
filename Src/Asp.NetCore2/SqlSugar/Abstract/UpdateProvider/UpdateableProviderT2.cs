using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public class Updateable<T, T2> : IUpdateable<T, T2> where T : class,new()
    {
        public IUpdateable<T> updateableObj { get; set; }
        public int ExecuteCommand()
        {
            throw new NotImplementedException();
        }

        public Task<int> ExecuteCommandAsync()
        {
            throw new NotImplementedException();
        }

        public IUpdateable<T, T2, T3> InnerJoin<T3>(Expression<Func<T, T2, T3, bool>> joinExpress)
        {
            throw new NotImplementedException();
        }

        public IUpdateable<T, T2> SetColumns(Expression<Func<T, T2, T>> columns)
        {
           // ((UpdateableProvider<T>)updateableObj).SetColumnsByExpression(columns);
            return this;
        }

         

        public IUpdateable<T, T2> Where(Expression<Func<T, T2, bool>> whereExpression)
        {
            throw new NotImplementedException();
        }
    }
}
