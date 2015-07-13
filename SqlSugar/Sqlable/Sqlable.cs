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
        /// <summary>
        /// sql
        /// </summary>
        public StringBuilder Sql { get; set; }
        /// <summary>
        /// 当前状态
        /// </summary>
        public MappingCurrentState? MappingCurrentState { get; set; }
        /// <summary>
        /// 是否允许脏读
        /// </summary>
        public bool IsNoLock { get; set; }
    }
}
