using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
}
