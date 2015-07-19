using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    /// <summary>
    ///Queryable是多表查询基类，基于拥有大量查询扩展函数
    /// </summary>
    public class Sqlable
    {
        /// <summary>
        /// 数据接口
        /// </summary>
        public SqlSugarClient DB = null;
        /// <summary>
        /// sql临时数据
        /// </summary>
        public StringBuilder Sql { get; set; }
        /// <summary>
        /// Where临时数据
        /// </summary>
        public List<string> Where = new List<string>();
        /// <summary>
        /// OrderBy临时数据
        /// </summary>
        public string OrderBy { get; set; }
        /// <summary>
        /// GroupBy临时数据
        /// </summary>
        public string GroupBy { get; set; }
    }
}
