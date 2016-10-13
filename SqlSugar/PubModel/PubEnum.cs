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
        /// <summary>
        /// 等值连接
        /// </summary>
        INNER = 0,
        /// <summary>
        /// 左外连
        /// </summary>
        LEFT = 1,
        /// <summary>
        /// 右外连
        /// </summary>
        RIGHT = 2
    }
    /// <summary>
    /// Apply类型
    /// </summary>
    public enum ApplyType
    {
        /// <summary>
        /// 笛卡尔积
        /// </summary>
        CORSS = 1,
        /// <summary>
        /// 外连
        /// </summary>
        OUTRE = 2
    }
    /// <summary>
    /// 排序类型
    /// </summary>
    public enum OrderByType
    {
        /// <summary>
        /// 升序
        /// </summary>
        asc = 0,
        /// <summary>
        /// 降序
        /// </summary>
        desc = 1
    }
    /// <summary>
    /// 分页类型
    /// </summary>
    public enum PageModel
    {
        /// <summary>
        /// 05分页
        /// </summary>
        RowNumber = 0,
        /// <summary>
        /// 12分页
        /// </summary>
        Offset = 1
    }
    /// <summary>
    /// 解析类型
    /// </summary>
    public enum ResolveExpressType
    {
        /// <summary>
        /// 单个T
        /// </summary>
        oneT = 0,
        /// <summary>
        /// 多个T
        /// </summary>
        nT = 1,

    }
}
