using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{


    /// <summary>
    /// Sqlable执行状态
    /// </summary>
    public enum SqlableCurrentState
    {
        MappingTable = 0,
        Where = 1,
        Select = 2,
        SingleTable = 3,
        Table = 4
    }

    /// <summary>
    /// join类型
    /// </summary>
    public enum JoinType
    {
        INNER = 0,
        LEFT = 1,
        RIGHT = 2
    }
}
