using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    /// <summary>
    ///  CPU X86
    ///  CPU X86
    /// </summary>
    public class Config
    {
        public static string GetCurrentProjectPath
        {

            get
            {
                return Environment.CurrentDirectory;
            }
        }
        /// <summary>
        /// Account have permission to create database
        /// 用有建库权限的数据库账号
        /// </summary>
        public static string ConnectionString =  "Provider=Microsoft.ACE.OleDB.16.0;Data Source="+GetCurrentProjectPath+"\\test.accdb";
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
