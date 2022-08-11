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
        public static string GetCurrentProjectPath
        {

            get
            {
                return Environment.CurrentDirectory.Replace(@"\bin\Debug", "");
            }
        }
        public static string ConnectionString = @"DataSource="+ GetCurrentProjectPath + @"\DataBase\SqlSugar4xTest.sqlite";
        public static string ConnectionString2 = @"DataSource=" + GetCurrentProjectPath + @"\DataBase\SqlSugar4xTest2.sqlite";
        public static string ConnectionString3 = @"DataSource=" + GetCurrentProjectPath + @"\DataBase\SqlSugar4xTest3.sqlite";
    }
}
