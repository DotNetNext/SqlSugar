using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public class DeleteNavProvider<Root,T>
    {

        public List<Root> Roots { get;  set; }
        public SqlSugarProvider Context { get; internal set; }

        public DeleteNavProvider<Root,TChild> ThenInclude<TChild>(Expression<Func<T,TChild>> expression)
        {
            return null;
        }

        public DeleteNavProvider<Root,Root> AsNav()
        {
            return null;
        }
    }
}
