using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    /// <summary>
    /// sql查询类
    /// </summary>
    public class Queryable<T>
    {
        public string Name
        {
            get
            {
                return typeof(T).Name;
            }
        }
        public Type Type
        {
            get
            {
                return typeof(T);
            }
        }

        public List<string> Where = new List<string>();
        public SqlSugarClient DB = null;
        public int? Skip { get; set; }
        public int? Take { get; set; }
        public string Order { get; set; }
    }
}
