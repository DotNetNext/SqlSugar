using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public class Subqueryable<T, T2, T3, T4, T5> : Subqueryable<T> where T : class, new()
    {
    }
    public class Subqueryable<T, T2, T3, T4> : Subqueryable<T> where T : class, new()
    {
        public Subqueryable<T, T2, T3, JoinType> InnerJoin<JoinType>(Func<T, T2, T3, JoinType, bool> expression)
        {
            return new Subqueryable<T, T2, T3, JoinType>();
        }
        public Subqueryable<T, T2, T3, JoinType> LeftJoin<JoinType>(Func<T, T2, T3, JoinType, bool> expression)
        {
            return new Subqueryable<T, T2, T3, JoinType>();
        }
        public Subqueryable<T, T2, T3, T4, JoinType> Where(Func<T, T2, T3, T4, bool> expression)
        {
            return new Subqueryable<T, T2, T3, T4, JoinType>();
        }
        public Subqueryable<T, T2, T3, T4, JoinType> Where(Func<T, T2, T3, bool> expression)
        {
            return new Subqueryable<T, T2, T3, T4, JoinType>();
        }
        public Subqueryable<T, T2, T3, T4, JoinType> Where(Func<T, T2, bool> expression)
        {
            return new Subqueryable<T, T2, T3, T4, JoinType>();
        }
    }
    public class Subqueryable<T, T2, T3> : Subqueryable<T> where T : class, new()
    {
        public Subqueryable<T, T2, T3, JoinType> InnerJoin<JoinType>(Func<T, T2, T3, JoinType, bool> expression)
        {
            return new Subqueryable<T, T2, T3, JoinType>();
        }
        public Subqueryable<T, T2, T3, JoinType> LeftJoin<JoinType>(Func<T, T2, T3, JoinType, bool> expression)
        {
            return new Subqueryable<T, T2, T3, JoinType>();
        }
        public Subqueryable<T, T2, T3, JoinType> Where(Func<T, T2, T3, bool> expression)
        {
            return new Subqueryable<T, T2, T3, JoinType>();
        }
        public Subqueryable<T, T2, T3, JoinType> Where(Func<T, T2, bool> expression)
        {
            return new Subqueryable<T, T2, T3, JoinType>();
        }
    }
    public class Subqueryable<T, T2> : Subqueryable<T> where T : class, new()
    {
        public Subqueryable<T, T2, JoinType> InnerJoin<JoinType>(Func<T, T2, JoinType, bool> expression)
        {
            return new Subqueryable<T, T2, JoinType>();
        }
        public Subqueryable<T, T2, JoinType> LeftJoin<JoinType>(Func<T, T2, JoinType, bool> expression)
        {
            return new Subqueryable<T, T2, JoinType>();
        }
        public Subqueryable<T, T2, JoinType> Where(Func<T, T2, bool> expression)
        {
            return new Subqueryable<T, T2, JoinType>();
        }
    }
}
