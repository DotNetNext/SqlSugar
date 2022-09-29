using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    /// <summary>
    /// 配置连接字符串后，需要手动建库,表可以自动创建
    /// </summary>
    public class Config
    {
        /// <summary>
        /// Account have permission to create database
        /// 用有建库权限的数据库账号
        /// </summary>
        public static string ConnectionString = "Server=116.242.92.57:5511;User Id=S;PWD=S0718;DATABASE=DAMENG";
        /// <summary>
        /// Account have permission to create database
        /// 用有建库权限的数据库账号
        /// </summary>
        public static string ConnectionString2 = ConnectionString;
        /// <summary>
        /// Account have permission to create database
        /// 用有建库权限的数据库账号
        /// </summary>
        public static string ConnectionString3 = ConnectionString;
    }
}
