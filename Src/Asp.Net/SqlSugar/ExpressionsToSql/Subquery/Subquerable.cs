using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar
{
    /// <summary>
    /// 开发中....
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Subqueryable<T> where T : class, new()
    {
        public Subqueryable<T> Where(Func<T, bool> expression)
        {
            return this;
        }
        public Subqueryable<T> OrderBy(Func<T, object> expression)
        {
            return this;
        }
        public Subqueryable<T> OrderByDesc(Func<T, object> expression)
        {
            return this;
        }
        public TResult Select<TResult>(Func<T, TResult> expression) where TResult :struct
        {
            return default(TResult);
        }
        public string Select(Func<T, string> expression) 
        {
            return default(string);
        }

        public TResult Max<TResult>(Func<T, TResult> expression) where TResult : struct
        {
            return default(TResult);
        }

        public TResult Min<TResult>(Func<T, TResult> expression) where TResult : struct
        {
            return default(TResult);
        }
        public string Max(Func<T, string> expression)
        {
            return default(string);
        }
        public string Min(Func<T, string> expression)
        {
            return default(string);
        }

        public bool Any()
        {
            return default(bool);
        }

        public bool NotAny()
        {
            return default(bool);
        }

        public int Count()
        {
            return default(int);
        }
    }
}
