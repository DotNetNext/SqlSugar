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
        public static string ConnectionString = "server=localhost;Database=SqlSugar4xTest;Uid=root;Pwd=haosql";
        /// <summary>
        /// Account have permission to create database
        /// 用有建库权限的数据库账号
        /// </summary>
        public static string ConnectionString2 = "server=localhost;Database=SqlSugar4xTest2;Uid=root;Pwd=haosql";
        /// <summary>
        /// Account have permission to create database
        /// 用有建库权限的数据库账号
        /// </summary>
        public static string ConnectionString3 = "server=localhost;Database=SqlSugar4xTest3;Uid=root;Pwd=haosql";


        /***注意：如果报错：指字关键词不在字典中这说明需要更新MYSQL.DATA驱动到最新，不报错就不需要更新***/
    }
}
