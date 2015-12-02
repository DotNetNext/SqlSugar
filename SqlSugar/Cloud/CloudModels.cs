using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace SqlSugar
{

    /// <summary>
    /// 云计算连接配置
    /// </summary>
    public class CloudConnectionConfig
    {
        /// <summary>
        /// 处理机率,值越大机率越高
        /// </summary>
        public int Rate { get; set; }
        /// <summary>
        /// 链接字符串名称
        /// </summary>
        public string ConnectionName { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class CloudSearch<T>
    {
        public List<T> TEntities { get; set; }
        public int Count { get; set; }
        public string ConnectionName { get; set; }
        public DataTable DataTable { get; set; }
    }
}
