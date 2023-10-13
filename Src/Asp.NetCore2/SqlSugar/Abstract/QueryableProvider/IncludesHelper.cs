using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public partial class QueryableProvider<T> : QueryableAccessory, ISugarQueryable<T>
    {
        private void _Includes<T1, TReturn1>(SqlSugarProvider context, params Expression[] expressions)
        {
            Func<ISugarQueryable<object>,List<object>> SelectR1 = it => it.Select<TReturn1>().ToList().Select(x=>x as object).ToList();
            var navigat = new NavigatManager<T>();
            navigat.SelectR1 = SelectR1;
            navigat.Expressions = expressions;
            navigat.Context = this.Context;
            navigat.IsCrossQueryWithAttr = this.QueryBuilder.IsCrossQueryWithAttr;
            navigat.CrossQueryItems = this.QueryBuilder.CrossQueryItems;
            navigat.QueryBuilder = this.QueryBuilder;
            if (this.QueryBuilder.Includes == null) this.QueryBuilder.Includes = new List<object>();
            this.QueryBuilder.Includes.Add(navigat);
        }
        private void _Includes<T1, TReturn1, TReturn2>(SqlSugarProvider context, params Expression[] expressions)
        {
            Func<ISugarQueryable<object>, List<object>> SelectR1 = it => it.Select<TReturn1>().ToList().Select(x => x as object).ToList();
            Func<ISugarQueryable<object>, List<object>> SelectR2 = it => it.Select<TReturn2>().ToList().Select(x => x as object).ToList();
            var navigat = new NavigatManager<T>();
            navigat.SelectR1 = SelectR1;
            navigat.SelectR2 = SelectR2;
            navigat.IsCrossQueryWithAttr = this.QueryBuilder.IsCrossQueryWithAttr;
            navigat.CrossQueryItems = this.QueryBuilder.CrossQueryItems;
            navigat.Expressions = expressions;
            navigat.Context = this.Context;
            navigat.QueryBuilder = this.QueryBuilder;
            if (this.QueryBuilder.Includes == null) this.QueryBuilder.Includes = new List<object>();
            this.QueryBuilder.Includes.Add(navigat);
        }
        private void _Includes<T1, TReturn1, TReturn2, TReturn3>(SqlSugarProvider context, params Expression[] expressions)
        {
            Func<ISugarQueryable<object>, List<object>> SelectR1 = it => it.Select<TReturn1>().ToList().Select(x => x as object).ToList();
            Func<ISugarQueryable<object>, List<object>> SelectR2 = it => it.Select<TReturn2>().ToList().Select(x => x as object).ToList();
            Func<ISugarQueryable<object>, List<object>> SelectR3 = it => it.Select<TReturn3>().ToList().Select(x => x as object).ToList();
            var navigat = new NavigatManager<T>();
            navigat.SelectR1 = SelectR1;
            navigat.SelectR2 = SelectR2;
            navigat.SelectR3 = SelectR3;
            navigat.IsCrossQueryWithAttr = this.QueryBuilder.IsCrossQueryWithAttr;
            navigat.CrossQueryItems = this.QueryBuilder.CrossQueryItems;
            navigat.Expressions = expressions;
            navigat.Context = this.Context;
            navigat.QueryBuilder = this.QueryBuilder;
            if (this.QueryBuilder.Includes == null) this.QueryBuilder.Includes = new List<object>();
            this.QueryBuilder.Includes.Add(navigat);
        }
        private void _Includes<T1, TReturn1, TReturn2, TReturn3, TReturn4>(SqlSugarProvider context, params Expression[] expressions)
        {
            Func<ISugarQueryable<object>, List<object>> SelectR1 = it => it.Select<TReturn1>().ToList().Select(x => x as object).ToList();
            Func<ISugarQueryable<object>, List<object>> SelectR2 = it => it.Select<TReturn2>().ToList().Select(x => x as object).ToList();
            Func<ISugarQueryable<object>, List<object>> SelectR3 = it => it.Select<TReturn3>().ToList().Select(x => x as object).ToList();
            Func<ISugarQueryable<object>, List<object>> SelectR4 = it => it.Select<TReturn4>().ToList().Select(x => x as object).ToList();
            var navigat = new NavigatManager<T>();
            navigat.SelectR1 = SelectR1;
            navigat.SelectR2 = SelectR2;
            navigat.SelectR3 = SelectR3;
            navigat.SelectR4 = SelectR4;
            navigat.Expressions = expressions;
            navigat.IsCrossQueryWithAttr = this.QueryBuilder.IsCrossQueryWithAttr;
            navigat.CrossQueryItems = this.QueryBuilder.CrossQueryItems;
            navigat.Context = this.Context;
            navigat.QueryBuilder = this.QueryBuilder;
            if (this.QueryBuilder.Includes == null) this.QueryBuilder.Includes = new List<object>();
            this.QueryBuilder.Includes.Add(navigat);
        }
        private void _Includes<T1, TReturn1, TReturn2, TReturn3, TReturn4, TReturn5>(SqlSugarProvider context, params Expression[] expressions)
        {
            Func<ISugarQueryable<object>, List<object>> SelectR1 = it => it.Select<TReturn1>().ToList().Select(x => x as object).ToList();
            Func<ISugarQueryable<object>, List<object>> SelectR2 = it => it.Select<TReturn2>().ToList().Select(x => x as object).ToList();
            Func<ISugarQueryable<object>, List<object>> SelectR3 = it => it.Select<TReturn3>().ToList().Select(x => x as object).ToList();
            Func<ISugarQueryable<object>, List<object>> SelectR4 = it => it.Select<TReturn4>().ToList().Select(x => x as object).ToList();
            Func<ISugarQueryable<object>, List<object>> SelectR5 = it => it.Select<TReturn5>().ToList().Select(x => x as object).ToList();
            var navigat = new NavigatManager<T>();
            navigat.SelectR1 = SelectR1;
            navigat.SelectR2 = SelectR2;
            navigat.SelectR3 = SelectR3;
            navigat.SelectR4 = SelectR4;
            navigat.SelectR5 = SelectR5;
            navigat.Expressions = expressions;
            navigat.IsCrossQueryWithAttr = this.QueryBuilder.IsCrossQueryWithAttr;
            navigat.CrossQueryItems = this.QueryBuilder.CrossQueryItems;
            navigat.Context = this.Context;
            navigat.QueryBuilder = this.QueryBuilder;
            if (this.QueryBuilder.Includes == null) this.QueryBuilder.Includes = new List<object>();
            this.QueryBuilder.Includes.Add(navigat);
        }
        public NavISugarQueryable<T> AsNavQueryable() 
        {
            return GetNavSugarQueryable();
        }
        private  NavISugarQueryable<T> GetNavSugarQueryable()
        {
            var result= new NavQueryableProvider<T>();
            result.Context = this.Context;
            var clone = this.Clone();
            result.SqlBuilder = clone.SqlBuilder;
            result.QueryBuilder = clone.QueryBuilder;
            return result;
        }
        private ISugarQueryable<T> GetManyQueryable<TReturn1>(Expression<Func<T, TReturn1>> include1)
        {
            ISugarQueryable<T> result = null;
            var isManyMembers = IsMembers(include1);
            if (isManyMembers)
            {
                var array = ExpressionTool.ExtractMemberNames(include1);
                if (array.Count > 1)
                {

                    if (array.Count == 2)
                    {
                        result = this.IncludesByNameString(array[0], array[1]);
                    }
                    else if (array.Count == 3)
                    {
                        result = this.IncludesByNameString(array[0], array[1], array[2]);
                    }
                    else if (array.Count == 4)
                    {
                        result = this.IncludesByNameString(array[0], array[1], array[2], array[3]);
                    }
                    else if (array.Count == 5)
                    {
                        result = this.IncludesByNameString(array[0], array[1], array[2], array[3], array[4]);
                    }
                    else if (array.Count == 6)
                    {
                        throw new Exception("Multiple levels of expression exceeded the upper limit");
                    }
                }
            }
            return result;
        }
        private static bool IsMembers<TReturn1>(Expression<Func<T, TReturn1>> include1)
        {
            var isManyMembers = false;
            var x = ((include1 as LambdaExpression).Body as MemberExpression)?.Expression;
            if (x is MemberExpression)
            {
                var exp = (x as MemberExpression)?.Expression;
                if (exp != null)
                {
                    isManyMembers = true;
                }
            }
            return isManyMembers;
        }

    }
    public partial class NavQueryableProvider<T> : QueryableProvider<T>, NavISugarQueryable<T>
    {
        private void _Includes<T1, TReturn1>(SqlSugarProvider context, params Expression[] expressions)
        {
            Func<ISugarQueryable<object>, List<object>> SelectR1 = it => it.Select<TReturn1>().ToList().Select(x => x as object).ToList();
            var navigat = new NavigatManager<T>();
            navigat.SelectR1 = SelectR1;
            navigat.Expressions = expressions;
            navigat.Context = this.Context;
            navigat.IsCrossQueryWithAttr = this.QueryBuilder.IsCrossQueryWithAttr;
            navigat.CrossQueryItems = this.QueryBuilder.CrossQueryItems;
            navigat.QueryBuilder = this.QueryBuilder;
            if (this.QueryBuilder.Includes == null) this.QueryBuilder.Includes = new List<object>();
            this.QueryBuilder.Includes.Add(navigat);
        }
        private void _Includes<T1, TReturn1, TReturn2>(SqlSugarProvider context, params Expression[] expressions)
        {
            Func<ISugarQueryable<object>, List<object>> SelectR1 = it => it.Select<TReturn1>().ToList().Select(x => x as object).ToList();
            Func<ISugarQueryable<object>, List<object>> SelectR2 = it => it.Select<TReturn2>().ToList().Select(x => x as object).ToList();
            var navigat = new NavigatManager<T>();
            navigat.SelectR1 = SelectR1;
            navigat.SelectR2 = SelectR2;
            navigat.Expressions = expressions;
            navigat.IsCrossQueryWithAttr = this.QueryBuilder.IsCrossQueryWithAttr;
            navigat.CrossQueryItems = this.QueryBuilder.CrossQueryItems;
            navigat.Context = this.Context;
            navigat.QueryBuilder = this.QueryBuilder;
            if (this.QueryBuilder.Includes == null) this.QueryBuilder.Includes = new List<object>();
            this.QueryBuilder.Includes.Add(navigat);
        }
        private void _Includes<T1, TReturn1, TReturn2, TReturn3>(SqlSugarProvider context, params Expression[] expressions)
        {
            Func<ISugarQueryable<object>, List<object>> SelectR1 = it => it.Select<TReturn1>().ToList().Select(x => x as object).ToList();
            Func<ISugarQueryable<object>, List<object>> SelectR2 = it => it.Select<TReturn2>().ToList().Select(x => x as object).ToList();
            Func<ISugarQueryable<object>, List<object>> SelectR3 = it => it.Select<TReturn3>().ToList().Select(x => x as object).ToList();
            var navigat = new NavigatManager<T>();
            navigat.SelectR1 = SelectR1;
            navigat.SelectR2 = SelectR2;
            navigat.SelectR3 = SelectR3;
            navigat.Expressions = expressions;
            navigat.IsCrossQueryWithAttr = this.QueryBuilder.IsCrossQueryWithAttr;
            navigat.CrossQueryItems = this.QueryBuilder.CrossQueryItems;
            navigat.Context = this.Context;
            navigat.QueryBuilder = this.QueryBuilder;
            if (this.QueryBuilder.Includes == null) this.QueryBuilder.Includes = new List<object>();
            this.QueryBuilder.Includes.Add(navigat);
        }
        private void _Includes<T1, TReturn1, TReturn2, TReturn3, TReturn4>(SqlSugarProvider context, params Expression[] expressions)
        {
            Func<ISugarQueryable<object>, List<object>> SelectR1 = it => it.Select<TReturn1>().ToList().Select(x => x as object).ToList();
            Func<ISugarQueryable<object>, List<object>> SelectR2 = it => it.Select<TReturn2>().ToList().Select(x => x as object).ToList();
            Func<ISugarQueryable<object>, List<object>> SelectR3 = it => it.Select<TReturn3>().ToList().Select(x => x as object).ToList();
            Func<ISugarQueryable<object>, List<object>> SelectR4 = it => it.Select<TReturn4>().ToList().Select(x => x as object).ToList();
            var navigat = new NavigatManager<T>();
            navigat.SelectR1 = SelectR1;
            navigat.SelectR2 = SelectR2;
            navigat.SelectR3 = SelectR3;
            navigat.SelectR4 = SelectR4;
            navigat.Expressions = expressions;
            navigat.IsCrossQueryWithAttr = this.QueryBuilder.IsCrossQueryWithAttr;
            navigat.CrossQueryItems = this.QueryBuilder.CrossQueryItems;
            navigat.Context = this.Context;
            navigat.QueryBuilder = this.QueryBuilder;
            if (this.QueryBuilder.Includes == null) this.QueryBuilder.Includes = new List<object>();
            this.QueryBuilder.Includes.Add(navigat);
        }
        private void _Includes<T1, TReturn1, TReturn2, TReturn3, TReturn4, TReturn5>(SqlSugarProvider context, params Expression[] expressions)
        {
            Func<ISugarQueryable<object>, List<object>> SelectR1 = it => it.Select<TReturn1>().ToList().Select(x => x as object).ToList();
            Func<ISugarQueryable<object>, List<object>> SelectR2 = it => it.Select<TReturn2>().ToList().Select(x => x as object).ToList();
            Func<ISugarQueryable<object>, List<object>> SelectR3 = it => it.Select<TReturn3>().ToList().Select(x => x as object).ToList();
            Func<ISugarQueryable<object>, List<object>> SelectR4 = it => it.Select<TReturn4>().ToList().Select(x => x as object).ToList();
            Func<ISugarQueryable<object>, List<object>> SelectR5 = it => it.Select<TReturn5>().ToList().Select(x => x as object).ToList();
            var navigat = new NavigatManager<T>();
            navigat.SelectR1 = SelectR1;
            navigat.SelectR2 = SelectR2;
            navigat.SelectR3 = SelectR3;
            navigat.SelectR4 = SelectR4;
            navigat.SelectR5 = SelectR5;
            navigat.Expressions = expressions;
            navigat.IsCrossQueryWithAttr = this.QueryBuilder.IsCrossQueryWithAttr;
            navigat.CrossQueryItems = this.QueryBuilder.CrossQueryItems;
            navigat.Context = this.Context;
            navigat.QueryBuilder = this.QueryBuilder;
            if (this.QueryBuilder.Includes == null) this.QueryBuilder.Includes = new List<object>();
            this.QueryBuilder.Includes.Add(navigat);
        }
        private void _Includes<T1, TReturn1, TReturn2, TReturn3, TReturn4, TReturn5, TReturn6>(SqlSugarProvider context, params Expression[] expressions)
        {
            Func<ISugarQueryable<object>, List<object>> SelectR1 = it => it.Select<TReturn1>().ToList().Select(x => x as object).ToList();
            Func<ISugarQueryable<object>, List<object>> SelectR2 = it => it.Select<TReturn2>().ToList().Select(x => x as object).ToList();
            Func<ISugarQueryable<object>, List<object>> SelectR3 = it => it.Select<TReturn3>().ToList().Select(x => x as object).ToList();
            Func<ISugarQueryable<object>, List<object>> SelectR4 = it => it.Select<TReturn4>().ToList().Select(x => x as object).ToList();
            Func<ISugarQueryable<object>, List<object>> SelectR5 = it => it.Select<TReturn5>().ToList().Select(x => x as object).ToList();
            Func<ISugarQueryable<object>, List<object>> SelectR6 = it => it.Select<TReturn6>().ToList().Select(x => x as object).ToList();
            var navigat = new NavigatManager<T>();
            navigat.SelectR1 = SelectR1;
            navigat.SelectR2 = SelectR2;
            navigat.SelectR3 = SelectR3;
            navigat.SelectR4 = SelectR4;
            navigat.SelectR5 = SelectR5;
            navigat.SelectR6 = SelectR6;
            navigat.Expressions = expressions;
            navigat.IsCrossQueryWithAttr = this.QueryBuilder.IsCrossQueryWithAttr;
            navigat.CrossQueryItems = this.QueryBuilder.CrossQueryItems;
            navigat.Context = this.Context;
            navigat.QueryBuilder = this.QueryBuilder;
            if (this.QueryBuilder.Includes == null) this.QueryBuilder.Includes = new List<object>();
            this.QueryBuilder.Includes.Add(navigat);
        }
        private void _Includes<T1, TReturn1, TReturn2, TReturn3, TReturn4, TReturn5, TReturn6, TReturn7>(SqlSugarProvider context, params Expression[] expressions)
        {
            Func<ISugarQueryable<object>, List<object>> SelectR1 = it => it.Select<TReturn1>().ToList().Select(x => x as object).ToList();
            Func<ISugarQueryable<object>, List<object>> SelectR2 = it => it.Select<TReturn2>().ToList().Select(x => x as object).ToList();
            Func<ISugarQueryable<object>, List<object>> SelectR3 = it => it.Select<TReturn3>().ToList().Select(x => x as object).ToList();
            Func<ISugarQueryable<object>, List<object>> SelectR4 = it => it.Select<TReturn4>().ToList().Select(x => x as object).ToList();
            Func<ISugarQueryable<object>, List<object>> SelectR5 = it => it.Select<TReturn5>().ToList().Select(x => x as object).ToList();
            Func<ISugarQueryable<object>, List<object>> SelectR6 = it => it.Select<TReturn6>().ToList().Select(x => x as object).ToList();
            Func<ISugarQueryable<object>, List<object>> SelectR7 = it => it.Select<TReturn7>().ToList().Select(x => x as object).ToList();
            var navigat = new NavigatManager<T>();
            navigat.SelectR1 = SelectR1;
            navigat.SelectR2 = SelectR2;
            navigat.SelectR3 = SelectR3;
            navigat.SelectR4 = SelectR4;
            navigat.SelectR5 = SelectR5;
            navigat.SelectR6 = SelectR6;
            navigat.SelectR7 = SelectR7;
            navigat.Expressions = expressions;
            navigat.QueryBuilder = this.QueryBuilder;
            navigat.IsCrossQueryWithAttr = this.QueryBuilder.IsCrossQueryWithAttr;
            navigat.CrossQueryItems = this.QueryBuilder.CrossQueryItems;
            navigat.Context = this.Context;
            if (this.QueryBuilder.Includes == null) this.QueryBuilder.Includes = new List<object>();
            this.QueryBuilder.Includes.Add(navigat);
        }


    }
}
