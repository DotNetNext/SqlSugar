using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace SqlSugar
{
    /// <summary>
    /// 数据库配置
    /// </summary>
    public class DataBaseConfig
    {
        /// <summary>
        /// 手动配置
        /// </summary>
        public static bool IsManualConfiguration = false;
        /// <summary>
        /// key表名 value主键名
        /// </summary>
        public static List<KeyValue> PrimaryKeys = null;
        /// <summary>
        /// key表名 value自添列名
        /// </summary>
        public static List<KeyValue> IdentityKeys = null;
    }
}
