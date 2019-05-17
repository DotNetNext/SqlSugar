using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public class ModelContext
    {
        [SugarColumn(IsIgnore = true)]
        [JsonIgnore]
        public SqlSugarProvider Context { get; set; }
        public ISugarQueryable<T> CreateMapping<T>() where T : class, new()
        {
            Check.ArgumentNullException(Context, "Please use Sqlugar.ModelContext");
            using (Context)
            {
                return Context.Queryable<T>();
            }
        }
    }
}
