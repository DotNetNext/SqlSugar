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
    /// docker快速创建测试容器：
    /// docker run -p 5432:5432 -eGAUSS_USER=gauss  -eGAUSS_PASSWORD=Gauss666 --name OpenGaussTest lsqtzj/openeuler_open_gauss:latest
    /// 等到容器提示 ==> START Service SUCCESSFUL ... 
    /// 删除容器
    /// docker stop OpenGaussTest 
    /// docker rm OpenGaussTest
    /// </summary>
    public class Config
    {
        /// <summary>
        /// Account have permission to create database
        /// 用有建库权限的数据库账号
        /// </summary>
        public static string ConnectionString = "PORT=5432;DATABASE=SqlSugar4xTest;HOST=localhost;PASSWORD=Gauss666;USER ID=gauss";
        /// <summary>
        /// Account have permission to create database
        /// 用有建库权限的数据库账号
        /// </summary>
        public static string ConnectionString2 = "PORT=5432;DATABASE=SqlSugar4xTest2;HOST=localhost;PASSWORD=Gauss666;USER ID=gauss";
        /// <summary>
        /// Account have permission to create database
        /// 用有建库权限的数据库账号
        /// </summary>
        public static string ConnectionString3 = "PORT=5432;DATABASE=SqlSugar4xTest3;HOST=localhost;PASSWORD=Gauss666;USER ID=gauss";
    }
}
