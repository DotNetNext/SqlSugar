using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NugetTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = "server=.;uid=sa;pwd=haosql;database=SqlSugar4XTest",
                IsAutoCloseConnection = true,
                DbType = DbType.SqlServer
            });
            InstanceFactory.RemoveCache();
            var list = db.Ado.GetInt("select 1");
            Console.WriteLine("Hello World!");
        }
    }
}
