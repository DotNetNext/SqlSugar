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
        public static string ConnectionString = "PORT=5432;DATABASE=SqlSugar4xTest;HOST=localhost;PASSWORD=haosql;USER ID=postgres";
        /// <summary>
        /// Account have permission to create database
        /// 用有建库权限的数据库账号
        /// </summary>
        public static string ConnectionString2 = "PORT=5432;DATABASE=SqlSugar4xTest2;HOST=localhost;PASSWORD=haosql;USER ID=postgres";
        /// <summary>
        /// Account have permission to create database
        /// 用有建库权限的数据库账号
        /// </summary>
        public static string ConnectionString3 = "PORT=5432;DATABASE=SqlSugar4xTest3;HOST=localhost;PASSWORD=haosql;USER ID=postgres";
    }
}
