using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public interface IInsertable<T>
    {
        int ExecuteCommand();
        int ExecuteReutrnIdentity();
        IInsertable<T> With(string lockString);
        IInsertable<T> InsertColumns(Expression<Func<T, object[]>> columns);
        IInsertable<T> IgnoreColumns(Expression<Func<T, object[]>> columns);
        IInsertable<T> Where(bool isInsertNull);
        KeyValuePair<string, List<SugarParameter>> ToSql();
        EntityInfo EntityInfo { get; set; }
    }
}
