using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{   public class Subqueryable<T1, T2, T3, T4, T5, T6, T7, T8,T9,T10> : Subqueryable<T1> where T1 : class, new()
    { }
    public class Subqueryable<T1, T2, T3, T4, T5, T6, T7, T8, T9> : Subqueryable<T1> where T1 : class, new()
    {
        public new Subqueryable<T1, T2, T3, T4, T5, T6, T7, T8,T9> AsWithAttr()
        {
            return this;
        }
        public Subqueryable<T1, T2, T3, T4, T5, T6, T7, T8, T9, JoinType> InnerJoin<JoinType>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, JoinType, bool> expression)
        {
            return new Subqueryable<T1, T2, T3, T4, T5, T6, T7, T8, T9, JoinType>();
        }
        public Subqueryable<T1, T2, T3, T4, T5, T6, T7, T8, T9, JoinType> LeftJoin<JoinType>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, JoinType, bool> expression)
        {
            return new Subqueryable<T1, T2, T3, T4, T5, T6, T7, T8, T9, JoinType>();
        }
        public Subqueryable<T1, T2, T3, T4, T5, T6, T7, T8, T9, JoinType> InnerJoin<JoinType>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, JoinType, bool> expression,string tableName)
        {
            return new Subqueryable<T1, T2, T3, T4, T5, T6, T7, T8, T9, JoinType>();
        }
        public Subqueryable<T1, T2, T3, T4, T5, T6, T7, T8, T9, JoinType> LeftJoin<JoinType>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, JoinType, bool> expression,string tableName)
        {
            return new Subqueryable<T1, T2, T3, T4, T5, T6, T7, T8, T9, JoinType>();
        }
        public new Subqueryable<T1, T2, T3, T4, T5, T6, T7, T8, T9> Where(Func<T1, bool> expression)
        {
            return this;
        }
        public Subqueryable<T1, T2, T3, T4, T5, T6, T7, T8, T9> Where(Func<T1, T2, bool> expression)
        {
            return this;
        }
        public Subqueryable<T1, T2, T3, T4, T5, T6, T7, T8, T9> Where(Func<T1, T2, T3, bool> expression)
        {
            return this;
        }
        public Subqueryable<T1, T2, T3, T4, T5, T6, T7, T8, T9> Where(Func<T1, T2, T3, T4, bool> expression)
        {
            return this;
        }
        public Subqueryable<T1, T2, T3, T4, T5, T6, T7, T8, T9> Where(Func<T1, T2, T3, T4, T5, bool> expression)
        {
            return this;
        }
        public Subqueryable<T1, T2, T3, T4, T5, T6, T7, T8, T9> Where(Func<T1, T2, T3, T4, T5, T6, bool> expression)
        {
            return this;
        }
        public Subqueryable<T1, T2, T3, T4, T5, T6, T7, T8, T9> Where(Func<T1, T2, T3, T4, T5, T6, T7, bool> expression)
        {
            return this;
        }
        public Subqueryable<T1, T2, T3, T4, T5, T6, T7, T8, T9> Where(Func<T1, T2, T3, T4, T5, T6, T7, T8, bool> expression)
        {
            return this;
        }
        public Subqueryable<T1, T2, T3, T4, T5, T6, T7, T8, T9> Where(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, bool> expression)
        {
            return this;
        }
        public List<TResult> ToList<TResult>(Func<T1, T2, T3, T4, T5, T6,T7,T8,T9, TResult> selector) 
        {
            return null;
        }
    }
    public class Subqueryable<T1, T2, T3, T4, T5, T6, T7, T8> : Subqueryable<T1> where T1 : class, new()
    {
        public new Subqueryable<T1, T2, T3, T4, T5, T6, T7,T8> AsWithAttr()
        {
            return this;
        }
        public Subqueryable<T1, T2, T3, T4, T5, T6, T7, T8, JoinType> InnerJoin<JoinType>(Func<T1, T2, T3, T4, T5, T6, T7, T8, JoinType, bool> expression)
        {
            return new Subqueryable<T1, T2, T3, T4, T5, T6, T7, T8, JoinType>();
        }
        public Subqueryable<T1, T2, T3, T4, T5, T6, T7, T8, JoinType> LeftJoin<JoinType>(Func<T1, T2, T3, T4, T5, T6, T7, T8, JoinType, bool> expression)
        {
            return new Subqueryable<T1, T2, T3, T4, T5, T6, T7, T8, JoinType>();
        }
        public Subqueryable<T1, T2, T3, T4, T5, T6, T7, T8, JoinType> InnerJoin<JoinType>(Func<T1, T2, T3, T4, T5, T6, T7, T8, JoinType, bool> expression,string tableName)
        {
            return new Subqueryable<T1, T2, T3, T4, T5, T6, T7, T8, JoinType>();
        }
        public Subqueryable<T1, T2, T3, T4, T5, T6, T7, T8, JoinType> LeftJoin<JoinType>(Func<T1, T2, T3, T4, T5, T6, T7, T8, JoinType, bool> expression,string tableName)
        {
            return new Subqueryable<T1, T2, T3, T4, T5, T6, T7, T8, JoinType>();
        }
        public new Subqueryable<T1, T2, T3, T4, T5, T6, T7, T8> Where(Func<T1, bool> expression)
        {
            return this;
        }
        public Subqueryable<T1, T2, T3, T4, T5, T6, T7, T8> Where(Func<T1, T2, bool> expression)
        {
            return this;
        }
        public Subqueryable<T1, T2, T3, T4, T5, T6, T7, T8> Where(Func<T1, T2, T3, bool> expression)
        {
            return this;
        }
        public Subqueryable<T1, T2, T3, T4, T5, T6, T7, T8> Where(Func<T1, T2, T3, T4, bool> expression)
        {
            return this;
        }
        public Subqueryable<T1, T2, T3, T4, T5, T6, T7, T8> Where(Func<T1, T2, T3, T4, T5, bool> expression)
        {
            return this;
        }
        public Subqueryable<T1, T2, T3, T4, T5, T6, T7, T8> Where(Func<T1, T2, T3, T4, T5, T6, bool> expression)
        {
            return this;
        }
        public Subqueryable<T1, T2, T3, T4, T5, T6, T7, T8> Where(Func<T1, T2, T3, T4, T5, T6, T7, bool> expression)
        {
            return this;
        }
        public Subqueryable<T1, T2, T3, T4, T5, T6, T7, T8> Where(Func<T1, T2, T3, T4, T5, T6, T7, T8, bool> expression)
        {
            return this;
        }
        public List<TResult> ToList<TResult>(Func<T1, T2, T3, T4, T5, T6,T7,T8, TResult> selector) 
        {
            return null;
        }
    }
    public class Subqueryable<T1, T2, T3, T4, T5, T6, T7> : Subqueryable<T1> where T1 : class, new()
    {
        public new Subqueryable<T1, T2, T3, T4, T5, T6,T7> AsWithAttr()
        {
            return this;
        }
        public Subqueryable<T1, T2, T3, T4, T5, T6, T7, JoinType> InnerJoin<JoinType>(Func<T1, T2, T3, T4, T5, T6, T7, JoinType, bool> expression)
        {
            return new Subqueryable<T1, T2, T3, T4, T5, T6, T7, JoinType>();
        }
        public Subqueryable<T1, T2, T3, T4, T5, T6, T7, JoinType> LeftJoin<JoinType>(Func<T1, T2, T3, T4, T5, T6, T7, JoinType, bool> expression)
        {
            return new Subqueryable<T1, T2, T3, T4, T5, T6, T7, JoinType>();
        }
        public Subqueryable<T1, T2, T3, T4, T5, T6, T7, JoinType> InnerJoin<JoinType>(Func<T1, T2, T3, T4, T5, T6, T7, JoinType, bool> expression,string tableName)
        {
            return new Subqueryable<T1, T2, T3, T4, T5, T6, T7, JoinType>();
        }
        public Subqueryable<T1, T2, T3, T4, T5, T6, T7, JoinType> LeftJoin<JoinType>(Func<T1, T2, T3, T4, T5, T6, T7, JoinType, bool> expression,string tableName)
        {
            return new Subqueryable<T1, T2, T3, T4, T5, T6, T7, JoinType>();
        }
        public new Subqueryable<T1, T2, T3, T4, T5, T6, T7> Where(Func<T1, bool> expression)
        {
            return this;
        }
        public Subqueryable<T1, T2, T3, T4, T5, T6, T7> Where(Func<T1, T2, bool> expression)
        {
            return this;
        }
        public Subqueryable<T1, T2, T3, T4, T5, T6, T7> Where(Func<T1, T2, T3, bool> expression)
        {
            return this;
        }
        public Subqueryable<T1, T2, T3, T4, T5, T6, T7> Where(Func<T1, T2, T3, T4, bool> expression)
        {
            return this;
        }
        public Subqueryable<T1, T2, T3, T4, T5, T6, T7> Where(Func<T1, T2, T3, T4, T5, bool> expression)
        {
            return this;
        }
        public Subqueryable<T1, T2, T3, T4, T5, T6, T7> Where(Func<T1, T2, T3, T4, T5, T6, bool> expression)
        {
            return this;
        }
        public Subqueryable<T1, T2, T3, T4, T5, T6, T7> Where(Func<T1, T2, T3, T4, T5, T6, T7, bool> expression)
        {
            return this;
        }
        public List<TResult> ToList<TResult>(Func<T1, T2, T3, T4, T5, T6,T7, TResult> selector) 
        {
            return null;
        }
    }
    public class Subqueryable<T1, T2, T3, T4, T5, T6> : Subqueryable<T1> where T1 : class, new()
    {
        public new Subqueryable<T1, T2, T3, T4, T5,T6> AsWithAttr()
        {
            return this;
        }
        public new Subqueryable<T1, T2, T3, T4, T5, T6> Where(List<IConditionalModel> conditionals)
        {
            return this;
        }
        public Subqueryable<T1, T2, T3, T4, T5, T6, JoinType> InnerJoin<JoinType>(Func<T1, T2, T3, T4, T5, T6, JoinType, bool> expression)
        {
            return new Subqueryable<T1, T2, T3, T4, T5, T6, JoinType>();
        }
        public Subqueryable<T1, T2, T3, T4, T5, T6, JoinType> LeftJoin<JoinType>(Func<T1, T2, T3, T4, T5, T6, JoinType, bool> expression)
        {
            return new Subqueryable<T1, T2, T3, T4, T5, T6, JoinType>();
        }

        public Subqueryable<T1, T2, T3, T4, T5, T6, JoinType> InnerJoin<JoinType>(Func<T1, T2, T3, T4, T5, T6, JoinType, bool> expression,string tableName)
        {
            return new Subqueryable<T1, T2, T3, T4, T5, T6, JoinType>();
        }
        public Subqueryable<T1, T2, T3, T4, T5, T6, JoinType> LeftJoin<JoinType>(Func<T1, T2, T3, T4, T5, T6, JoinType, bool> expression,string tableName)
        {
            return new Subqueryable<T1, T2, T3, T4, T5, T6, JoinType>();
        }
        public new Subqueryable<T1, T2, T3, T4, T5, T6> Where(Func<T1, bool> expression)
        {
            return this;
        }
        public Subqueryable<T1, T2, T3, T4, T5, T6> Where(Func<T1, T2, bool> expression)
        {
            return this;
        }
        public Subqueryable<T1, T2, T3, T4, T5, T6> Where(Func<T1, T2, T3, bool> expression)
        {
            return this;
        }
        public Subqueryable<T1, T2, T3, T4, T5, T6> Where(Func<T1, T2, T3, T4, bool> expression)
        {
            return this;
        }
        public Subqueryable<T1, T2, T3, T4, T5, T6> Where(Func<T1, T2, T3, T4, T5, bool> expression)
        {
            return this;
        }
        public Subqueryable<T1, T2, T3, T4, T5, T6> Where(Func<T1, T2, T3, T4, T5, T6, bool> expression)
        {
            return this;
        }
        public List<TResult> ToList<TResult>(Func<T1, T2, T3, T4, T5,T6, TResult> selector)  
        {
            return null;
        }
    }
    public class Subqueryable<T1, T2, T3, T4, T5> : Subqueryable<T1> where T1 : class, new()
    {
        public new Subqueryable<T1, T2, T3, T4,T5> AsWithAttr()
        {
            return this;
        }
        public new Subqueryable<T1, T2, T3, T4,T5> Where(List<IConditionalModel> conditionals)
        {
            return this;
        }
        public Subqueryable<T1, T2, T3, T4, T5, JoinType> InnerJoin<JoinType>(Func<T1, T2, T3, T4, T5, JoinType, bool> expression)
        {
            return new Subqueryable<T1, T2, T3, T4, T5, JoinType>();
        }
        public Subqueryable<T1, T2, T3, T4, T5, JoinType> LeftJoin<JoinType>(Func<T1, T2, T3, T4, T5, JoinType, bool> expression)
        {
            return new Subqueryable<T1, T2, T3, T4, T5, JoinType>();
        }

        public Subqueryable<T1, T2, T3, T4, T5, JoinType> InnerJoin<JoinType>(Func<T1, T2, T3, T4, T5, JoinType, bool> expression,string tableName)
        {
            return new Subqueryable<T1, T2, T3, T4, T5, JoinType>();
        }
        public Subqueryable<T1, T2, T3, T4, T5, JoinType> LeftJoin<JoinType>(Func<T1, T2, T3, T4, T5, JoinType, bool> expression,string tableName)
        {
            return new Subqueryable<T1, T2, T3, T4, T5, JoinType>();
        }
        public new Subqueryable<T1, T2, T3, T4, T5> Where(Func<T1, bool> expression)
        {
            return this;
        }
        public Subqueryable<T1, T2, T3, T4, T5> Where(Func<T1, T2, bool> expression)
        {
            return this;
        }
        public Subqueryable<T1, T2, T3, T4, T5> Where(Func<T1, T2, T3, bool> expression)
        {
            return this;
        }
        public Subqueryable<T1, T2, T3, T4, T5> Where(Func<T1, T2, T3, T4, bool> expression)
        {
            return this;
        }
        public Subqueryable<T1, T2, T3, T4, T5> Where(Func<T1, T2, T3, T4, T5, bool> expression)
        {
            return this;
        }
        public List<TResult> ToList<TResult>(Func<T1, T2, T3, T4,T5, TResult> selector)  
        {
            return null;
        }
    }
    public class Subqueryable<T1, T2, T3, T4> : Subqueryable<T1> where T1 : class, new()
    {
        public new Subqueryable<T1, T2, T3,T4> AsWithAttr()
        {
            return this;
        }
        public new Subqueryable<T1, T2, T3,T4> Where(List<IConditionalModel> conditionals)
        {
            return this;
        }
        public Subqueryable<T1, T2, T3, T4, JoinType> InnerJoin<JoinType>(Func<T1, T2, T3, T4, JoinType, bool> expression)
        {
            return new Subqueryable<T1, T2, T3, T4, JoinType>();
        }
        public Subqueryable<T1, T2, T3, T4, JoinType> LeftJoin<JoinType>(Func<T1, T2, T3, T4, JoinType, bool> expression)
        {
            return new Subqueryable<T1, T2, T3, T4, JoinType>();
        }
        public Subqueryable<T1, T2, T3, T4, JoinType> InnerJoin<JoinType>(Func<T1, T2, T3, T4, JoinType, bool> expression,string tableName)
        {
            return new Subqueryable<T1, T2, T3, T4, JoinType>();
        }
        public Subqueryable<T1, T2, T3, T4, JoinType> LeftJoin<JoinType>(Func<T1, T2, T3, T4, JoinType, bool> expression,string tableName)
        {
            return new Subqueryable<T1, T2, T3, T4, JoinType>();
        }
        public new Subqueryable<T1, T2, T3, T4> Where(Func<T1, bool> expression)
        {
            return this;
        }
        public Subqueryable<T1, T2, T3, T4> Where(Func<T1, T2, bool> expression)
        {
            return this;
        }
        public Subqueryable<T1, T2, T3, T4> Where(Func<T1, T2, T3, bool> expression)
        {
            return this;
        }
        public Subqueryable<T1, T2, T3, T4> Where(Func<T1, T2, T3, T4, bool> expression)
        {
            return this;
        }
        public Subqueryable<T1, T2, T3,T4> WhereIF(bool isWhere, Func<T1, T2, T3,T4, bool> expression)
        {
            return this;
        }
        public TResult Select<TResult>(Func<T1, T2, T3,T4, TResult> expression) where TResult : struct
        {
            return default(TResult);
        }
        public string Select(Func<T1, T2, T3,T4, string> expression)
        {
            return default(string);
        }
        public Subqueryable<T1, T2, T3,T4> OrderBy(Func<T1, T2, T3,T4, object> expression)
        {
            return this;
        }
        public Subqueryable<T1, T2, T3,T4> OrderByDesc(Func<T1, T2, T3,T4, object> expression)
        {
            return this;
        }
        public Subqueryable<T1, T2, T3,T4> GroupBy(Func<T1, T2, T3,T4, object> expression)
        {
            return this;
        }
        public string SelectStringJoin(Func<T1, T2, T3, T4, string> expression, string separator)
        {
            return default(string);
        }
        public List<TResult> ToList<TResult>(Func<T1, T2, T3,T4, TResult> selector)  
        {
            return null;
        }
        public List<TResult> ToList<TResult>(Func<T1, T2, T3,T4, TResult> selector, bool isAutoDto) where TResult : class, new()
        {
            return null;
        }
        public TResult First<TResult>(Func<T1, T2, T3,T4, TResult> selector, bool isAutoDto) where TResult : class, new()
        {
            return default(TResult);
        }
        public TResult Max<TResult>(Func<T1, T2, T3,T4, TResult> expression)
        {
            return default(TResult);
        }
        public Byte[] Max(Func<T1, T2, T3, T4, Byte[]> expression)
        {
            return null;
        }
        public string Max(Func<T1, T2, T3, T4, string> expression)
        {
            return default(string);
        }

        public string Min(Func<T1, T2, T3, T4, string> expression)
        {
            return default(string);
        }
        public TResult Min<TResult>(Func<T1, T2,T3, T4, TResult> expression)
        {
            return default(TResult);
        }
        public Byte[] Min(Func<T1, T2, T3,T4, Byte[]> expression)
        {
            return null;
        }
        public string Sum(Func<T1, T2, T3,T4, string> expression)
        {
            return default(string);
        }
        public TResult Sum<TResult>(Func<T1, T2,T3,T4, TResult> expression)
        {
            return default(TResult);
        }
    }
    public class Subqueryable<T1, T2, T3> : Subqueryable<T1> where T1 : class, new()
    {
        public new Subqueryable<T1, T2,T3> AsWithAttr()
        {
            return this;
        }
        public new Subqueryable<T1, T2,T3> Where(List<IConditionalModel> conditionals)
        {
            return this;
        }
        public Subqueryable<T1, T2, T3, JoinType> InnerJoin<JoinType>(Func<T1, T2, T3, JoinType, bool> expression)
        {
            return new Subqueryable<T1, T2, T3, JoinType>();
        }
        public Subqueryable<T1, T2, T3, JoinType> LeftJoin<JoinType>(Func<T1, T2, T3, JoinType, bool> expression)
        {
            return new Subqueryable<T1, T2, T3, JoinType>();
        }
        public new Subqueryable<T1, T2, T3> Where(Func<T1, bool> expression)
        {
            return this;
        }
        public Subqueryable<T1, T2, T3> Where(Func<T1, T2, bool> expression)
        {
            return this;
        }
        public Subqueryable<T1, T2, T3> Where(Func<T1, T2, T3, bool> expression)
        {
            return this;
        }
        public Subqueryable<T1, T2,T3> WhereIF(bool isWhere, Func<T1, T2,T3, bool> expression)
        {
            return this;
        }
        public Subqueryable<T1, T2,T3> OrderBy(Func<T1, T2,T3, object> expression)
        {
            return this;
        }
        public Subqueryable<T1, T2,T3> OrderByDesc(Func<T1, T2,T3, object> expression)
        {
            return this;
        }
        public Subqueryable<T1, T2,T3> GroupBy(Func<T1, T2,T3, object> expression)
        {
            return this;
        }
        public TResult Select<TResult>(Func<T1, T2,T3, TResult> expression) where TResult : struct
        {
            return default(TResult);
        }
        public string Select(Func<T1, T2,T3, string> expression)
        {
            return default(string);
        }
        public string SelectStringJoin(Func<T1, T2,T3, string> expression, string separator)
        {
            return default(string);
        }
        public List<TResult> ToList<TResult>(Func<T1, T2,T3, TResult> selector)  
        {
            return null;
        }
        public List<TResult> ToList<TResult>(Func<T1,T2,T3, TResult> selector, bool isAutoDto) where TResult : class, new()
        {
            return null;
        }
        public TResult First<TResult>(Func<T1,T2,T3, TResult> selector, bool isAutoDto) where TResult : class, new()
        {
            return default(TResult);
        }
        public TResult Max<TResult>(Func<T1, T2,T3, TResult> expression)
        {
            return default(TResult);
        }
        public Byte[] Max(Func<T1, T2, T3, Byte[]> expression)
        {
            return null;
        }
        public string Max(Func<T1, T2, T3, string> expression)
        {
            return default(string);
        }

        public string Min(Func<T1, T2, T3, string> expression)
        {
            return default(string);
        }
        public TResult Min<TResult>(Func<T1, T2,T3, TResult> expression)
        {
            return default(TResult);
        }
        public Byte[] Min(Func<T1, T2, T3, Byte[]> expression)
        {
            return null;
        }
        public string Sum(Func<T1, T2, T3, string> expression)
        {
            return default(string);
        }
        public TResult Sum<TResult>(Func<T1, T2,T3, TResult> expression)
        {
            return default(TResult);
        }
    }
    public class Subqueryable<T1, T2> : Subqueryable<T1> where T1 : class, new()
    {
        public new Subqueryable<T1,T2> AsWithAttr()
        {
            return this;
        }
        public new Subqueryable<T1,T2> Where(List<IConditionalModel> conditionals)
        {
            return this;
        }
        public Subqueryable<T1, T2, JoinType> InnerJoin<JoinType>(Func<T1, T2, JoinType, bool> expression)
        {
            return new Subqueryable<T1, T2, JoinType>();
        }
        public Subqueryable<T1, T2, JoinType> LeftJoin<JoinType>(Func<T1, T2, JoinType, bool> expression)
        {
            return new Subqueryable<T1, T2, JoinType>();
        }
        public Subqueryable<T1, T2, JoinType> InnerJoin<JoinType>(Func<T1, T2, JoinType, bool> expression,string tableName)
        {
            return new Subqueryable<T1, T2, JoinType>();
        }
        public Subqueryable<T1, T2, JoinType> LeftJoin<JoinType>(Func<T1, T2, JoinType, bool> expression, string tableName)
        {
            return new Subqueryable<T1, T2, JoinType>();
        }
        public new Subqueryable<T1, T2> Where(Func<T1, bool> expression)
        {
            return this;
        }
        public Subqueryable<T1, T2> Where(Func<T1, T2, bool> expression)
        {
            return this;
        }
        public Subqueryable<T1,T2> OrderBy(Func<T1,T2, object> expression)
        {
            return this;
        }
        public Subqueryable<T1,T2> OrderByDesc(Func<T1,T2, object> expression)
        {
            return this;
        }
        public Subqueryable<T1,T2> GroupBy(Func<T1,T2, object> expression)
        {
            return this;
        }
        public Subqueryable<T1,T2> WhereIF(bool isWhere, Func<T1, T2, bool> expression)
        {
            return this;
        }
        public TResult Select<TResult>(Func<T1,T2, TResult> expression) where TResult : struct
        {
            return default(TResult);
        }
        public string Select(Func<T1, T2, string> expression)  
        {
            return default(string);
        }
        public string SelectStringJoin(Func<T1,T2, string> expression, string separator)
        {
            return default(string);
        }
        public List<TResult> ToList<TResult>(Func<T1,T2, TResult> selector)
        {
            return null;
        }
        public List<TResult> ToList<TResult>(Func<T1,T2, TResult> selector, bool isAutoDto) where TResult : class, new()
        {
            return null;
        }
        public TResult First<TResult>(Func<T1,T2, TResult> selector, bool isAutoDto) where TResult : class, new()
        {
            return default(TResult);
        }
        public TResult Max<TResult>(Func<T1,T2, TResult> expression)
        {
            return default(TResult);
        }
        public Byte[] Max(Func<T1, T2, Byte[]> expression)
        {
            return null;
        }
        public string Max(Func<T1, T2, string> expression)
        {
            return default(string);
        }

        public string Min(Func<T1, T2, string> expression)
        {
            return default(string);
        }
        public TResult Min<TResult>(Func<T1, T2, TResult> expression)
        {
            return default(TResult);
        }
        public Byte[] Min(Func<T1, T2, Byte[]> expression)
        {
            return null;
        }
        public string Sum(Func<T1, T2, string> expression)
        {
            return default(string);
        }
        public TResult Sum<TResult>(Func<T1, T2, TResult> expression)
        {
            return default(TResult);
        }
    }
}
