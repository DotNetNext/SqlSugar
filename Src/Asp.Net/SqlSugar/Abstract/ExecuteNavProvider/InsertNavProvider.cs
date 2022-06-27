using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public class InsertNavProvider<Root,T>
    {

        public List<Root> Roots { get;  set; }

        public InsertNavProvider<Root,TChild> ThenInclude<TChild>(Expression<Func<T,TChild>> expression)
        {
            return null;
        }

        public InsertNavProvider<Root,Root> AsNav()
        {
            return null;
        }
    }
}
