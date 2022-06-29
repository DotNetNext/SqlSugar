using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public class UpdateNavProvider<Root,T>
    {

        public List<Root> Roots { get;  set; }
        public SqlSugarProvider Context { get; internal set; }

        public UpdateNavProvider<Root,TChild> ThenInclude<TChild>(Expression<Func<T,TChild>> expression)
        {
            return null;
        }

        public UpdateNavProvider<Root,Root> AsNav()
        {
            return null;
        }
    }
}
