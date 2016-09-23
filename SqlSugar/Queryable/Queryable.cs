using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace SqlSugar
{

    /// <summary>
    /// ** 描述：Queryable拉姆达查询对象
    /// ** 创始时间：2015-7-13
    /// ** 修改时间：-
    /// ** 作者：sunkaixuan
    /// ** 使用说明：
    /// </summary>
    public class Queryable<T>
    {
        /// <summary>
        /// T的名称
        /// </summary>
        internal string TName { get { return typeof(T).Name; } }
        /// <summary>
        /// 实体类型
        /// </summary>
        internal Type Type { get { return typeof(T); } }
        /// <summary>
        /// 数据接口
        /// </summary>
        public SqlSugarClient DB = null;
        /// <summary>
        /// Where临时数据
        /// </summary>
        internal List<string> Where = new List<string>();
        /// <summary>
        /// Skip临时数据
        /// </summary>
        internal int? Skip { get; set; }
        /// <summary>
        /// Take临时数据
        /// </summary>
        internal int? Take { get; set; }
        /// <summary>
        /// Order临时数据
        /// </summary>
        internal string OrderBy { get; set; }
        /// <summary>
        /// Select临时数据
        /// </summary>
        internal string Select { get; set; }
        /// <summary>
        /// SqlParameter临时数据
        /// </summary>
        internal List<SqlParameter> Params = new List<SqlParameter>();
        /// <summary>
        /// 表名临时数据
        /// </summary>
        internal string TableName { get; set; }
        /// <summary>
        /// 分组查询临时数据
        /// </summary>
        internal string GroupBy { get; set; }
        /// <summary>
        /// 条件索引临时数据
        /// </summary>
        internal int WhereIndex = 1;
        /// <summary>
        /// 联表查询临时数据
        /// </summary>
        internal List<string> JoinTableValue = new List<string>();
    }
}
