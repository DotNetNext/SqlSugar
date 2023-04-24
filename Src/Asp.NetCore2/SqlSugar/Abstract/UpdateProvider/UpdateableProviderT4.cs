using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public class Updateable<T, T2, T3, T4> : IUpdateable<T, T2, T3, T4> where T : class, new()
    {
        public int ExecuteCommand()
        {
            throw new NotImplementedException();
        }

        public Task<int> ExecuteCommandAsync()
        {
            throw new NotImplementedException();
        }

        public IUpdateable<T, T2, T3, T4> SetColumns(Expression<Func<T, T2, T3, T4, T>> columns)
        {
            throw new NotImplementedException();
        }

        public IUpdateable<T, T2, T3, T4> Where(Expression<Func<T, T2, T3, T4, bool>> whereExpression)
        {
            throw new NotImplementedException();
        }
    }
}
