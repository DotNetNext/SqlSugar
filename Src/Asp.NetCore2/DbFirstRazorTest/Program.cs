using RazorEngine;
using RazorEngine.Templating;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DbFirstRazorTest
{
    class Program
    {
        static void Main(string[] args)
        {

            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = "server=.;uid=sa;pwd=sasa;database=SQLSUGAR4XTEST",
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
    public class RazorService : IRazorService
    {
        public List<KeyValuePair<string, string>> GetClassStringList(string razorTemplate, List<RazorTableInfo> model)
        {
            if (model != null && model.Any())
            {
                var result = new List<KeyValuePair<string, string>>();
                foreach (var item in model)
                {
                    try
                    {
                        item.ClassName = item.DbTableName;//格式化类名
                        string key = "RazorService.GetClassStringList" + razorTemplate.Length;
                        var classString = Engine.Razor.RunCompile(razorTemplate, key, item.GetType(), item);
                        result.Add(new KeyValuePair<string, string>(item.ClassName, classString));
                    }
                    catch (Exception ex)
                    {
                        new Exception(item.DbTableName + " error ." + ex.Message);
                    }
                }
                return result;
            }
            else
            {
                return new List<KeyValuePair<string, string>>();
            }
        }
    }
}
