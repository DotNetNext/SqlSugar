using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public class DeleteNavProvider<Root, T> where T : class, new() where Root : class, new()
    {
        public SqlSugarProvider _Context { get; internal set; }

        public DeleteNavTask<Root, TChild> ThenInclude< TChild>(Expression<Func<Root, TChild>> expression)
            where TChild : class, new()
        {
            throw new NotImplementedException();
        }
        public DeleteNavTask<Root, TChild> ThenInclude<TChild>(Expression<Func<Root, List<TChild>>> expression)
         where TChild : class, new()
        {
            throw new NotImplementedException();
        }
    }
}
