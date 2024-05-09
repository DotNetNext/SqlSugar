using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar
{
    public partial class Subqueryable<T> where T : class, new()
    {

        public Subqueryable<T> AS(string tableName) 
        {
            return this;
        }
        public Subqueryable<T, JoinType> InnerJoin<JoinType>(Func<T, JoinType, bool> expression)
        {
            return new Subqueryable<T, JoinType>();
        }

        public Subqueryable<T, JoinType> InnerJoin<JoinType>(Func<T, JoinType, bool> expression,string tableName)
        {
            return new Subqueryable<T, JoinType>();
        }

        public Subqueryable<T, JoinType> LeftJoin<JoinType>(Func<T, JoinType, bool> expression)
        {
            return new Subqueryable<T, JoinType>();
        }

        public Subqueryable<T, JoinType> LeftJoin<JoinType>(Func<T, JoinType, bool> expression,string tableName)
        {
            return new Subqueryable<T, JoinType>();
        }
         

        public Subqueryable<T> Where(string where)
        {
            return this;
        }
        public Subqueryable<T> Where(List<IConditionalModel> conditionals)
        {
            return this;
        }
        public Subqueryable<T> Where(Expression exp)
        {
            return this;
        }
        public Subqueryable<T> Where(Func<T, bool> expression)
        {
            return this;
        }
        public Subqueryable<T> Having(Func<T, bool> expression)
        {
            return this;
        }
        public Subqueryable<T> Where<Main, Join1>(Func<Main, Join1, bool> expression)
        {
            return this;
        }
        public Subqueryable<T> Where<Main, Join1, Join2>(Func<Main, Join1, Join2, bool> expression)
        {
            return this;
        }
        public Subqueryable<T> Where<Main, Join1, Join2, Join3>(Func<Main, Join1, Join2, Join3, bool> expression)
        {
            return this;
        }
        public Subqueryable<T> Where<Main, Join1, Join2, Join3, Join4>(Func<Main, Join1, Join2, Join3, Join4, bool> expression)
        {
            return this;
        }
        public Subqueryable<T> WhereIF(bool isWhere,Func<T, bool> expression)
        {
            return this;
        }
        public Subqueryable<T> OrderBy(Func<T, object> expression)
        {
            return this;
        }
        public Subqueryable<T> GroupBy(Func<T, object> expression)
        {
            return this;
        }
        public Subqueryable<T> OrderByDesc(Func<T, object> expression)
        {
            return this;
        }

        public TResult Select<TResult>(Func<T, TResult> expression)  
        {
            return default(TResult);
        }
        public Byte[] Select(Func<T, Byte[]> expression) 
        {
            return null;
        }
        public string Select(Func<T, string> expression) 
        {
            return default(string);
        }

        public string SelectStringJoin(Func<T, string> expression,string separator)
        {
            return default(string);
        }

        public TResult Max<TResult>(Func<T, TResult> expression)  
        {
            return default(TResult);
        }
        public Byte[] Max(Func<T, Byte[]> expression)
        {
            return null;
        }
        public string Max(Func<T, string> expression)
        {
            return default(string);
        }

        public string Min(Func<T, string> expression)
        {
            return default(string);
        }
        public TResult Min<TResult>(Func<T, TResult> expression) 
        {
            return default(TResult);
        }
        public Byte[] Min(Func<T, Byte[]> expression)
        {
            return null;
        }


        public string Sum(Func<T, string> expression)
        {
            return default(string);
        }

        public int DistinctCount(Func<T, object> expression) 
        {
            return default(int);
        }
        public TResult Sum<TResult>(Func<T, TResult> expression) 
        {
            return default(TResult);
        }
        public Byte[] Sum(Func<T, Byte[]> expression)
        {
            return null;
        }

        public string Avg(Func<T, string> expression)
        {
            return default(string);
        }
        public TResult Avg<TResult>(Func<T, TResult> expression) where TResult : struct
        {
            return default(TResult);
        }
        public Byte[] Avg(Func<T, Byte[]> expression)
        {
            return null;
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

        public Subqueryable<T> WithNoLock()
        {
            return this;
        }
        public Subqueryable<T> EnableTableFilter()
        {
            return this;
        }

        public List<T> ToList()
        {
            return new List<T>();
        }
        public List<TResult> ToList<TResult>(Func<T, TResult> selector)
        {
            return null;
        }
        public List<TResult> ToList<TResult>()
        {
            return null;
        }
        public List<TResult> ToList<TResult>(Func<T, TResult> selector,bool isAutoDto)where TResult : class, new()
        {
            return null;
        }
        public T First()
        {
            return default(T);
        }
        public TResult First<TResult>(Func<T, TResult> selector) where TResult : class, new()
        {
            return default(TResult);
        }
        public TResult First<TResult>(Func<T, TResult> selector, bool isAutoDto) where TResult : class, new()
        {
            return default(TResult);
        }
        public TResult First<TResult>() where TResult : class, new()
        {
            return default(TResult);
        }

        public Subqueryable<T> AsWithAttr()
        {
            return this;
        }
    }
}
