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
            throw new Exception("开发中7月15号之前上线");
        }
        public DeleteNavProvider<Root, TChild>  Include<TChild>(Expression<Func<T, TChild>> expression)
        {
            throw new Exception("开发中7月15号之前上线");
        }
        public DeleteNavProvider<Root, TChild> ThenInclude<TChild>(Expression<Func<T, List<TChild>>> expression)
        {
            throw new Exception("开发中7月15号之前上线");
        }
        public DeleteNavProvider<Root, TChild> Include<TChild>(Expression<Func<T, List<TChild>>> expression)
        {
            throw new Exception("开发中7月15号之前上线");
        }
        private DeleteNavProvider<Root,Root> AsNav()
        {
            throw new Exception("开发中7月15号之前上线");
        }
        public bool ExecuteCommand() 
        {
            return true;
        }
    }
}
