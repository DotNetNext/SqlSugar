using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{

    /// <summary>
    /// join类型
    /// </summary>
    public enum JoinType
    {
        INNER = 0,
        LEFT = 1,
        RIGHT = 2
    }
    /// <summary>
    /// Apply类型
    /// </summary>
    public enum ApplyType
    {
        CORSS = 1,
        OUTRE = 2
    }
    /// <summary>
    /// 排序类型
    /// </summary>
    public enum OrderByType
    {
        asc = 0,
        desc = 1
    }
    /// <summary>
    /// 分页类型
    /// </summary>
    public enum PageModel
    {
        RowNumber = 0,
        Offset = 1
    }
    /// <summary>
    /// 解析类型
    /// </summary>
    public enum ResolveExpressType
    {
        /// <summary>
        /// 参数和值
        /// </summary>
        oneT = 0,
        /// <summary>
        /// 只要字段名和运算符
        /// </summary>
        nT = 1,
 
    }
}
