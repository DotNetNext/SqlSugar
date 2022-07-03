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
            throw new Exception("开发中7月15号之前上线");
        }
        public UpdateNavProvider<Root, TChild>  Include<TChild>(Expression<Func<T, TChild>> expression)
        {
            throw new Exception("开发中7月15号之前上线");
        }
        public UpdateNavProvider<Root, TChild> ThenInclude<TChild>(Expression<Func<T, List<TChild>>> expression)
        {
            throw new Exception("开发中7月15号之前上线");
        }
        public UpdateNavProvider<Root, TChild> Include<TChild>(Expression<Func<T, List<TChild>>> expression)
        {
            throw new Exception("开发中7月15号之前上线");
        }
        public bool ExecuteCommand()
        {
            return true;
        }
        private UpdateNavProvider<Root,Root> AsNav()
        {
            return null;
        }
    }
}
