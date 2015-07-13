using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    /// <summary>
    /// sql查询类
    /// </summary>
    public class Queryable<T> where T : new()
    {
        public SqlHelper SqlHelper { get; set; }
    }
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

    public enum MappingCurrentState
    {
        MappingTable = 0,
        Where = 1,
        Select = 2,
        SingleTable = 3
    }
}
