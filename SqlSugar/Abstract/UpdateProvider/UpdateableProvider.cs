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
            throw new NotImplementedException();
        }

        public IUpdateable<T> IgnoreColumns(Func<string, bool> ignoreColumMethod)
        {
            throw new NotImplementedException();
        }

        public IUpdateable<T> IgnoreColumns(Expression<Func<T, object>> columns)
        {
            throw new NotImplementedException();
        }

        public IUpdateable<T> ReSetValue(Func<T, bool> setValueExpression)
        {
            throw new NotImplementedException();
        }

        public KeyValuePair<string, List<SugarParameter>> ToSql()
        {
            throw new NotImplementedException();
        }

        public IUpdateable<T> Update(T InsertObj)
        {
            throw new NotImplementedException();
        }

        public IUpdateable<T> UpdateColumns(Expression<Func<T, object>> columns)
        {
            throw new NotImplementedException();
        }

        public IUpdateable<T> UpdateRange(List<T> InsertObjs)
        {
            throw new NotImplementedException();
        }

        public IUpdateable<T> Where(bool isUpdateNull)
        {
            throw new NotImplementedException();
        }

        public IUpdateable<T> With(string lockString)
        {
            throw new NotImplementedException();
        }

        internal void Init()
        {
            throw new NotImplementedException();
        }
    }
}
