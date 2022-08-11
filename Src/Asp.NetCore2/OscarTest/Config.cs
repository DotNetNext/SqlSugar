using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    /// <summary>
    /// 配置好连接字符串，不需要建表
    /// </summary>
    public class Config
    {
        /// <summary>
        /// Account have permission to create database
        /// 用有建库权限的数据库账号
        /// </summary>
        public static string ConnectionString = "PORT=2003;DATABASE=osrdb;HOST=localhost;PASSWORD=szoscar55;USER ID=SYSDBA";
        /// <summary>
        /// Account have permission to create database
        /// 用有建库权限的数据库账号
        /// </summary>
        public static string ConnectionString2 = "PORT=2003;DATABASE=osrdb;HOST=localhost;PASSWORD=szoscar55;USER ID=SYSDBA";
        /// <summary>
        /// Account have permission to create database
        /// 用有建库权限的数据库账号
        /// </summary>
        public static string ConnectionString3 = "PORT=2003;DATABASE=osrdb;HOST=localhost;PASSWORD=szoscar55;USER ID=SYSDBA";
    }
}
