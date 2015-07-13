using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    
 
    /// <summary>
    /// 映射表操作的当前状态
    /// </summary>
    public enum MappingCurrentState
    {
        MappingTable = 0,
        Where = 1,
        Select = 2,
        SingleTable = 3
    }
}
