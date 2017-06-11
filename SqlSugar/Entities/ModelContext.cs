using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public class ModelContext
    {
        internal SqlSugarClient Context { get; set; }
        public ISugarQueryable<T> CreateMapping<T>()where T:class,new()
        {
            return Context.Queryable<T>();
        }
    }
}
