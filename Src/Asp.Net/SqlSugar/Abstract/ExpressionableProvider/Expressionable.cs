using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar
{
    public class Expressionable<T> where T : class, new()
    {
        Expression<Func<T, bool>> _exp = null;

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
            if (_exp == null)
                _exp = it => true;
            return _exp;
        }
    }
    public class Expressionable<T, T2> where T : class, new() where T2 : class, new()
    {
        Expression<Func<T, T2, bool>> _exp = null;

        public Expressionable<T, T2> And(Expression<Func<T, T2, bool>> exp)
        {
            if (_exp == null)
                _exp = exp;
            else
                _exp = Expression.Lambda<Func<T, T2, bool>>(Expression.AndAlso(_exp.Body, exp.Body), _exp.Parameters);
            return this;
        }

        public Expressionable<T, T2> AndIF(bool isAnd, Expression<Func<T, T2, bool>> exp)
        {
            if (isAnd)
                And(exp);
            return this;
        }

        public Expressionable<T, T2> Or(Expression<Func<T, T2, bool>> exp)
        {
            if (_exp == null)
                _exp = exp;
            else
                _exp = Expression.Lambda<Func<T, T2, bool>>(Expression.OrElse(_exp.Body, exp.Body), _exp.Parameters);
            return this;
        }

        public Expressionable<T, T2> OrIF(bool isOr, Expression<Func<T, T2, bool>> exp)
        {
            if (isOr)
                Or(exp);
            return this;
        }


        public Expression<Func<T, T2, bool>> ToExpression()
        {
            if (_exp == null)
                _exp = (it, t2) => true;
            return _exp;
        }
    }
    public class Expressionable<T, T2, T3> where T : class, new() where T2 : class, new() where T3 : class, new()
    {
        Expression<Func<T, T2, T3, bool>> _exp = null;

        public Expressionable<T, T2, T3> And(Expression<Func<T, T2, T3, bool>> exp)
        {
            if (_exp == null)
                _exp = exp;
            else
                _exp = Expression.Lambda<Func<T, T2, T3, bool>>(Expression.AndAlso(_exp.Body, exp.Body), _exp.Parameters);
            return this;
        }

        public Expressionable<T, T2, T3> AndIF(bool isAnd, Expression<Func<T, T2, T3, bool>> exp)
        {
            if (isAnd)
                And(exp);
            return this;
        }

        public Expressionable<T, T2, T3> Or(Expression<Func<T, T2, T3, bool>> exp)
        {
            if (_exp == null)
                _exp = exp;
            else
                _exp = Expression.Lambda<Func<T, T2, T3, bool>>(Expression.OrElse(_exp.Body, exp.Body), _exp.Parameters);
            return this;
        }

        public Expressionable<T, T2, T3> OrIF(bool isOr, Expression<Func<T, T2, T3, bool>> exp)
        {
            if (isOr)
                Or(exp);
            return this;
        }


        public Expression<Func<T, T2, T3, bool>> ToExpression()
        {
            if (_exp == null)
                _exp = (it, t2, t3) => true;
            return _exp;
        }
    }
    public class Expressionable<T, T2, T3, T4> where T : class, new() where T2 : class, new() where T3 : class, new() where T4 : class, new()
    {
        Expression<Func<T, T2, T3, T4, bool>> _exp = null;

        public Expressionable<T, T2, T3, T4> And(Expression<Func<T, T2, T3, T4, bool>> exp)
        {
            if (_exp == null)
                _exp = exp;
            else
                _exp = Expression.Lambda<Func<T, T2, T3, T4, bool>>(Expression.AndAlso(_exp.Body, exp.Body), _exp.Parameters);
            return this;
        }

        public Expressionable<T, T2, T3, T4> AndIF(bool isAnd, Expression<Func<T, T2, T3, T4, bool>> exp)
        {
            if (isAnd)
                And(exp);
            return this;
        }

        public Expressionable<T, T2, T3, T4> Or(Expression<Func<T, T2, T3, T4, bool>> exp)
        {
            if (_exp == null)
                _exp = exp;
            else
                _exp = Expression.Lambda<Func<T, T2, T3, T4, bool>>(Expression.OrElse(_exp.Body, exp.Body), _exp.Parameters);
            return this;
        }

        public Expressionable<T, T2, T3, T4> OrIF(bool isOr, Expression<Func<T, T2, T3, T4, bool>> exp)
        {
            if (isOr)
                Or(exp);
            return this;
        }


        public Expression<Func<T, T2, T3, T4, bool>> ToExpression()
        {
            if (_exp == null)
                _exp = (it, t2, t3, t4) => true;
            return _exp;
        }
    }
    public class Expressionable<T, T2, T3, T4, T5> where T : class, new() where T2 : class, new() where T3 : class, new() where T4 : class, new() where T5 : class, new()
    {
        Expression<Func<T, T2, T3, T4, T5, bool>> _exp = null;

        public Expressionable<T, T2, T3, T4, T5> And(Expression<Func<T, T2, T3, T4, T5, bool>> exp)
        {
            if (_exp == null)
                _exp = exp;
            else
                _exp = Expression.Lambda<Func<T, T2, T3, T4, T5, bool>>(Expression.AndAlso(_exp.Body, exp.Body), _exp.Parameters);
            return this;
        }

        public Expressionable<T, T2, T3, T4, T5> AndIF(bool isAnd, Expression<Func<T, T2, T3, T4, T5, bool>> exp)
        {
            if (isAnd)
                And(exp);
            return this;
        }

        public Expressionable<T, T2, T3, T4, T5> Or(Expression<Func<T, T2, T3, T4, T5, bool>> exp)
        {
            if (_exp == null)
                _exp = exp;
            else
                _exp = Expression.Lambda<Func<T, T2, T3, T4, T5, bool>>(Expression.OrElse(_exp.Body, exp.Body), _exp.Parameters);
            return this;
        }

        public Expressionable<T, T2, T3, T4, T5> OrIF(bool isOr, Expression<Func<T, T2, T3, T4, T5, bool>> exp)
        {
            if (isOr)
                Or(exp);
            return this;
        }


        public Expression<Func<T, T2, T3, T4, T5, bool>> ToExpression()
        {
            if (_exp == null)
                _exp = (it, t2, t3, t4, T5) => true;
            return _exp;
        }
    }

    public class Expressionable<T, T2, T3, T4, T5, T6> where T : class, new() where T2 : class, new() where T3 : class, new() where T4 : class, new() where T5 : class, new() where T6 : class, new()
    {
        Expression<Func<T, T2, T3, T4, T5, T6, bool>> _exp = null;

        public Expressionable<T, T2, T3, T4, T5, T6> And(Expression<Func<T, T2, T3, T4, T5, T6, bool>> exp)
        {
            if (_exp == null)
                _exp = exp;
            else
                _exp = Expression.Lambda<Func<T, T2, T3, T4, T5, T6, bool>>(Expression.AndAlso(_exp.Body, exp.Body), _exp.Parameters);
            return this;
        }

        public Expressionable<T, T2, T3, T4, T5, T6> AndIF(bool isAnd, Expression<Func<T, T2, T3, T4, T5, T6, bool>> exp)
        {
            if (isAnd)
                And(exp);
            return this;
        }

        public Expressionable<T, T2, T3, T4, T5, T6> Or(Expression<Func<T, T2, T3, T4, T5, T6, bool>> exp)
        {
            if (_exp == null)
                _exp = exp;
            else
                _exp = Expression.Lambda<Func<T, T2, T3, T4, T5, T6, bool>>(Expression.OrElse(_exp.Body, exp.Body), _exp.Parameters);
            return this;
        }

        public Expressionable<T, T2, T3, T4, T5, T6> OrIF(bool isOr, Expression<Func<T, T2, T3, T4, T5, T6, bool>> exp)
        {
            if (isOr)
                Or(exp);
            return this;
        }


        public Expression<Func<T, T2, T3, T4, T5, T6, bool>> ToExpression()
        {
            if (_exp == null)
                _exp = (it, t2, t3, t4, T5, t6) => true;
            return _exp;
        }
    }
    public class Expressionable
    {
        public static Expressionable<T> Create<T>() where T : class, new()
        {
            return new Expressionable<T>();
        }
        public static Expressionable<T, T2> Create<T, T2>() where T : class, new() where T2 : class, new()
        {
            return new Expressionable<T, T2>();
        }
        public static Expressionable<T, T2, T3> Create<T, T2, T3>() where T : class, new() where T2 : class, new() where T3 : class, new()
        {
            return new Expressionable<T, T2, T3>();
        }
        public static Expressionable<T, T2, T3, T4> Create<T, T2, T3, T4>() where T : class, new() where T2 : class, new() where T3 : class, new() where T4 : class, new()
        {
            return new Expressionable<T, T2, T3, T4>();
        }
        public static Expressionable<T, T2, T3, T4, T5> Create<T, T2, T3, T4, T5>() where T : class, new() where T2 : class, new() where T3 : class, new() where T4 : class, new() where T5 : class, new()
        {
            return new Expressionable<T, T2, T3, T4, T5>();
        }
        public static Expressionable<T, T2, T3, T4, T5, T6> Create<T, T2, T3, T4, T5, T6>() where T : class, new() where T2 : class, new() where T3 : class, new() where T4 : class, new() where T5 : class, new() where T6 : class, new()
        {
            return new Expressionable<T, T2, T3, T4, T5, T6>();
        }
    }
}
