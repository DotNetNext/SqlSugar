using System;
using SqlSugar;
namespace NugetTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var db = new SqlSugarClient(new ConnectionConfig() {
                 ConnectionString=Config.ConnectionString,
                 IsAutoCloseConnection=true,
                 DbType=DbType.Oracle
            });
            var list = db.Ado.GetInt("select 1");
            Console.WriteLine("Hello World!");
        }
        public class Config
        {
            public static string ConnectionString = "Database=SqlSugar4xTest;Data Source=127.0.0.1;User Id=root;Password=root;pooling=false;CharSet=utf8;port=3306";
        }
    }
}
