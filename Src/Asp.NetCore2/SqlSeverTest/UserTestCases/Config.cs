using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSeverTest.UserTestCases
{
    /// <summary>
    /// Setting up the database name does not require you to create the database
    /// 只要设置IP、用户名和密码 自动建库和表
    /// </summary>
    public class Config
    {
        /// <summary>
        /// Account have permission to create database
        /// 用有建库权限的数据库账号
        /// </summary>
        public static string ConnectionString = "server=.;uid=sa;pwd=SqlSugar123!;database=SQLSUGAR4XTEST;Encrypt=True;TrustServerCertificate=True";
        /// <summary>
        /// Account have permission to create database
        /// 用有建库权限的数据库账号
        /// </summary>
        public static string ConnectionString2 = "server=.;uid=sa;pwd=SqlSugar123!;database=SQLSUGAR4XTEST2;Encrypt=True;TrustServerCertificate=True";
        /// <summary>
        /// Account have permission to create database
        /// 用有建库权限的数据库账号
        /// </summary>
        public static string ConnectionString3 = "server=.;uid=sa;pwd=SqlSugar123!;database=SQLSUGAR4XTEST3;Encrypt=True;TrustServerCertificate=True";
    }
}
