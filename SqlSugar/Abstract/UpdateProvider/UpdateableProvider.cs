using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public class UpdateableProvider<T> : IUpdateable<T>
    {
        public SqlSugarClient Context { get; internal set; }
        public EntityInfo EntityInfo { get; internal set; }
        public ISqlBuilder SqlBuilder { get; internal set; }
        public UpdateBuilder UpdateBuilder { get; internal set; }
        public object[] UpdateObjs { get; internal set; }

        public int ExecuteCommand()
        {
            return 0;
        }

        public IUpdateable<T> IgnoreColumns(Func<string, bool> ignoreColumMethod)
        {
            return this;
        }

        public IUpdateable<T> IgnoreColumns(Expression<Func<T, object>> columns)
        {
            return this;
        }

        public IUpdateable<T> ReSetValue(Expression<Func<T, bool>> setValueExpression)
        {
            var expResult=UpdateBuilder.GetExpressionValue(setValueExpression, ResolveExpressType.WhereSingle);

            return this;
        }

        public KeyValuePair<string, List<SugarParameter>> ToSql()
        {
            return new KeyValuePair<string, List<SugarParameter>>();
        }

        public IUpdateable<T> Update(T InsertObj)
        {
            return this;
        }

        public IUpdateable<T> UpdateColumns(Expression<Func<T, object>> columns)
        {
            return this;
        }

        public IUpdateable<T> UpdateRange(List<T> InsertObjs)
        {
            return this;
        }

        public IUpdateable<T> Where(bool isUpdateNull)
        {
            return this;
        }

        public IUpdateable<T> With(string lockString)
        {
            return this;
        }

        internal void Init()
        {
            return this;
        }
    }
}
