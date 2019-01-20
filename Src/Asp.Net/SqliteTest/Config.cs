using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public class Config
    {
        public static string GetCurrentProjectPath
        {

            get
            {
                return Environment.CurrentDirectory.Replace(@"\bin\Debug", "");
            }
        }
        public static string ConnectionString = @"DataSource=GetCurrentProjectPath\DataBase\SqlSugar4xTest.sqlite";
    }
}
