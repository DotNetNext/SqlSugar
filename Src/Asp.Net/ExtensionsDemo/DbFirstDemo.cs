using SqlSugar;
using SqlSugar.DbFirstExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtensionsDemo
{
    public class DbFirstDemo
    {
        public static void Init()
        {
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = Config.ConnectionString,
                DbType = DbType.SqlServer,
                IsAutoCloseConnection = true,
                ConfigureExternalServices = new ConfigureExternalServices()
                {
                    RazorService = new RazorService()
                }
            });
 
            db.DbFirst.UseRazorAnalysis(RazorFirst.DefaultRazorClassTemplate).CreateClassFile("c:\\Demo\\Razor\\");
        }
    }
}

