using System;
using System.Collections.Generic;
namespace SqlSugar
{
    public interface IClient
    {

        bool Delete<T, FiledType>(params FiledType[] whereIn);
        bool Delete<T>(System.Linq.Expressions.Expression<Func<T, bool>> expression);
        bool FalseDelete<T, FiledType>(string field, params FiledType[] whereIn);
        bool FalseDelete<T>(string field, System.Linq.Expressions.Expression<Func<T, bool>> expression);
        object Insert<T>(T entity, bool isIdentity = true) where T : class;
        List<object> InsertRange<T>(System.Collections.Generic.List<T> entities, bool isIdentity = true) where T : class;
        void RemoveAllCache();
        bool Update<T, FiledType>(object rowObj, params FiledType[] whereIn) where T : class;
        bool Update<T>(object rowObj, System.Linq.Expressions.Expression<Func<T, bool>> expression) where T : class;
    }
}
