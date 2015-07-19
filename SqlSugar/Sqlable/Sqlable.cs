using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    /// <summary>
    /// sql创建类
    /// </summary>
    public class Sqlable
    {
        public SqlSugarClient DB = null;
        /// <summary>
        /// sql
        /// </summary>
        public StringBuilder Sql { get; set; }

        /// <summary>
        /// 是否允许脏读
        /// </summary>
        public bool IsNoLock { get; set; }

        public List<string> Where = new List<string>();
        public string OrderBy { get; set; }
        public string GroupBy { get; set; }
    }
}
