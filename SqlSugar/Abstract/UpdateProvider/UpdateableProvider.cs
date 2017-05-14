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

        public IInsertable<T> IgnoreColumns(Expression<Func<T, object[]>> columns)
        {
            throw new NotImplementedException();
        }

        public IInsertable<T> Update(T InsertObj)
        {
            throw new NotImplementedException();
        }

        public IInsertable<T> UpdateColumns(Expression<Func<T, object[]>> columns)
        {
            throw new NotImplementedException();
        }

        public IInsertable<T> UpdateRange(List<T> InsertObjs)
        {
            throw new NotImplementedException();
        }

        public IInsertable<T> Where(bool isUpdateNull)
        {
            throw new NotImplementedException();
        }

        public IInsertable<T> With(string lockString)
        {
            throw new NotImplementedException();
        }

        internal void Init()
        {
            throw new NotImplementedException();
        }
    }
}
