using SugarCodeGeneration.Codes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace SugarCodeGeneration
{
    class Program
    {
        private const SqlSugar.DbType sqlServer = SqlSugar.DbType.SqlServer;
        private const string projectName = "SugarCodeGeneration";
        private const string classPath= "Models"; 
        private const string classNamespace = "MyTest";
        private const string connectionString = "server=.;uid=sa;pwd=@jhl85661501;database=SqlSugar4XTest";

        static void Main(string[] args)
        {
            //Generation model
            SqlSugar.SqlSugarClient db = new SqlSugar.SqlSugarClient(new SqlSugar.ConnectionConfig() {
                DbType = sqlServer,
                ConnectionString = connectionString,
                IsAutoCloseConnection = true
            });
            var classDirectory = Methods.GetSlnPath +"\\"+projectName+"\\"+ classPath.TrimStart('\\');
           
            //if all then remove .Where
            db.DbFirst.Where("Student","School").CreateClassFile(classDirectory, classNamespace);

            Methods.AddCsproj(classPath, projectName);

            //Generation  DbContext
        }
    }
}
