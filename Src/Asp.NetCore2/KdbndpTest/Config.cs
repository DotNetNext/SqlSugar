using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    /// <summary>
    /// Setting up the database name does not require you to create the database
    /// 设置好数据库名不需要你去手动建库
    /// </summary>
    public class Config
    {
        /// <summary>
        /// Account have permission to create database
        /// 用有建库权限的数据库账号
        /// </summary>
        public static string ConnectionString = "Server=127.0.0.1;Port=54321;UID=SYSTEM;PWD=system;database=SQLSUGAR4XTEST1";
        /// <summary>
        /// Account have permission to create database
        /// 用有建库权限的数据库账号
        /// </summary>
        public static string ConnectionString2 = "Server=127.0.0.1;Port=54321;UID=SYSTEM;PWD=system;database=SQLSUGAR4XTEST2";
        /// <summary>
        /// Account have permission to create database
        /// 用有建库权限的数据库账号
        /// </summary>
        public static string ConnectionString3 = "Server=127.0.0.1;Port=54321;UID=SYSTEM;PWD=system;database=SQLSUGAR4XTEST3";
    }
}
