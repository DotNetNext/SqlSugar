using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace SqlSugar
{

    /// <summary>
    /// ** 描述：Queryable是单表查询基类，基于拥有大量查询扩展函数
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
        public string TName { get { return typeof(T).Name; } }
        /// <summary>
        /// 实体类型
        /// </summary>
        public Type Type { get { return typeof(T); }  }
        /// <summary>
        /// 数据接口
        /// </summary>
        public SqlSugarClient DB = null;
        /// <summary>
        /// Where临时数据
        /// </summary>
        public List<string> Where = new List<string>();
        /// <summary>
        /// Skip临时数据
        /// </summary>
        public int? Skip { get; set; }
        /// <summary>
        /// Take临时数据
        /// </summary>
        public int? Take { get; set; }
        /// <summary>
        /// Order临时数据
        /// </summary>
        public string OrderBy { get; set; }
        /// <summary>
        /// Select临时数据
        /// </summary>
        public string Select { get; set; }
        /// <summary>
        /// SqlParameter临时数据
        /// </summary>
        public List<SqlParameter> Params = new List<SqlParameter>();
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 分组查询
        /// </summary>
        public string GroupBy { get; set; }
    }
}
