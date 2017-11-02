using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar
{
    public class Expressionable<T> where T : class, new()
    {
        Expression<Func<T, bool>> _exp = it=>true;

        public Expressionable<T> And(Expression<Func<T, bool>> exp)
        {
            if (_exp == null)
                _exp = exp;
            else
                _exp = Expression.Lambda<Func<T, bool>>(Expression.AndAlso(_exp.Body, exp.Body), _exp.Parameters);
            return this;
        }

        public Expressionable<T> AndIF(bool isAnd, Expression<Func<T, bool>> exp)
        {
            if (isAnd)
                And(exp);
            return this;
        }

        public Expressionable<T> Or(Expression<Func<T, bool>> exp)
        {
            if (_exp == null)
                _exp = exp;
            else
                _exp = Expression.Lambda<Func<T, bool>>(Expression.OrElse(_exp.Body, exp.Body), _exp.Parameters);
            return this;
        }

        public Expressionable<T> OrIF(bool isOr, Expression<Func<T, bool>> exp)
        {
            if (isOr)
                Or(exp);
            return this;
        }


        public Expression<Func<T, bool>> ToExpression()
        {
            return _exp;
        }
    }

    public class Expressionable
    {
        public static Expressionable<T> Create<T>() where T : class, new()
        {
            return new Expressionable<T>();
        }
    }
}
