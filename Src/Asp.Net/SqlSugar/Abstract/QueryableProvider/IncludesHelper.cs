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
            Action<ISugarQueryable<object>> SelectR1 = it => it.Select<TReturn1>();
            var navigat = new NavigatManager<T>();
            navigat.SelectR1 = SelectR1;
            navigat.Expressions = expressions;
            navigat.Context = this.Context;
            this.QueryBuilder.Includes = navigat;
        }
        private void _Includes<T1, TReturn1, TReturn2>(SqlSugarProvider context, params Expression[] expressions)
        {
            Action<ISugarQueryable<object>> SelectR1 = it => it.Select<TReturn1>();
            Action<ISugarQueryable<object>> SelectR2 = it => it.Select<TReturn2>();
            var navigat = new NavigatManager<T>();
            navigat.SelectR1 = SelectR1;
            navigat.SelectR2 = SelectR2;
            navigat.Expressions = expressions;
            navigat.Context = this.Context;
            this.QueryBuilder.Includes = navigat;
        }
        private void _Includes<T1, TReturn1, TReturn2, TReturn3>(SqlSugarProvider context, params Expression[] expressions)
        {
            Action<ISugarQueryable<object>> SelectR1 = it => it.Select<TReturn1>();
            Action<ISugarQueryable<object>> SelectR2 = it => it.Select<TReturn2>();
            Action<ISugarQueryable<object>> SelectR3 = it => it.Select<TReturn3>();
            var navigat = new NavigatManager<T>();
            navigat.SelectR1 = SelectR1;
            navigat.SelectR2 = SelectR2;
            navigat.SelectR2 = SelectR3;
            navigat.Expressions = expressions;
            navigat.Context = this.Context;
            this.QueryBuilder.Includes = navigat;
        }
        private void _Includes<T1, TReturn1, TReturn2, TReturn3, TReturn4>(SqlSugarProvider context, params Expression[] expressions)
        {
            throw new NotImplementedException();
        }
        private void _Includes<T1, TReturn1, TReturn2, TReturn3, TReturn4, TReturn5>(SqlSugarProvider context, params Expression[] expressions)
        {
            throw new NotImplementedException();
        }
        private void _Includes<T1, TReturn1, TReturn2, TReturn3, TReturn4, TReturn5, TReturn6>(SqlSugarProvider context, params Expression[] expressions)
        {
            throw new NotImplementedException();
        }
        private void _Includes<T1, TReturn1, TReturn2, TReturn3, TReturn4, TReturn5, TReturn6, TReturn7>(SqlSugarProvider context, params Expression[] expressions)
        {
            throw new NotImplementedException();
        }
        private void _Includes<T1, TReturn1, TReturn2, TReturn3, TReturn4, TReturn5, TReturn6, TReturn7, TReturn8>(SqlSugarProvider context, params Expression[] expressions)
        {
            throw new NotImplementedException();
        }

    }
}
