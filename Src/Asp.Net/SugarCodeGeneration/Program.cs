using SugarCodeGeneration.Codes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace SugarCodeGeneration
{
    class Program
    {

        const SqlSugar.DbType dbType = SqlSugar.DbType.SqlServer;
        const string connectionString = "server=.;uid=sa;pwd=@jhl85661501;database=SqlSugar4XTest";
        static List<Task> CsprojList = new List<Task>();
        static SqlSugar.SqlSugarClient GetDB()
        {

            return new SqlSugar.SqlSugarClient(new SqlSugar.ConnectionConfig()
            {
                DbType = dbType,
                ConnectionString = connectionString,
                IsAutoCloseConnection = true,
                InitKeyType=SqlSugar.InitKeyType.Attribute
            });
        }
        static void Main(string[] args)
        {
            //生成实体
            GenerationClass();
            Console.WriteLine("");
            Console.WriteLine("实体创建成功");
            Console.WriteLine("");

            //生成DbContext
            GenerationDContext();
            Console.WriteLine("");
            Console.WriteLine("DbContext创建成功");
            Console.WriteLine("");

            //生成BLL类
            GenerationBLL();
            Console.WriteLine("");
            Console.WriteLine("Bll创建成功");
            Console.WriteLine("");

            //修改解决方案
            UpdateCsproj();
            Console.WriteLine("项目解决方案修改成功");
            Console.ReadKey();

            //test bll
             
        }


        /// <summary>
        /// 生成BLL
        /// </summary>
        private static void GenerationBLL()
        {
            var db = GetDB();

            //配置参数
            var templatePath = Methods.GetCurrentProjectPath + "\\Template\\Bll.txt";//bll模版地址
            var bllProjectName = "SugarCodeGeneration";//具体项目
            var bllPath = "BLL";//文件目录
            var savePath = Methods.GetSlnPath + "\\" + bllProjectName + "\\" + bllPath;//保存目录
            var tables = db.DbMaintenance.GetTableInfoList().Where(it => it.Name == "Student" || it.Name == "School").Select(it => it.Name).ToList();

            //下面代码不动
            Methods.CreateBLL(templatePath, savePath, tables);
            AddTask(bllProjectName, bllPath);
        }



        /// <summary>
        /// 生成DbContext
        /// </summary>
        private static void GenerationDContext()
        {
            var db = GetDB();


            //配置参数
            var templatePath = Methods.GetCurrentProjectPath + "\\Template\\DbContext.txt";//dbcontexts模版文件
            var contextProjectName = "SugarCodeGeneration";//DbContext所在项目
            var contextPath = "DbContext";//dbcontext存储目录
            var savePath = Methods.GetSlnPath + "\\" + contextProjectName + "\\" + contextPath+"\\DbContext.cs";//具体文件名
            var tables = db.DbMaintenance.GetTableInfoList().Where(it => it.Name == "Student" || it.Name == "School").Select(it => it.Name).ToList();

            //下面代码不动
            var model = new DbContextParameter{
                ConnectionString = connectionString,
                DbType = dbType,
                Tables = tables
            };


            Methods.CreateDbContext(templatePath,savePath,model);
            AddTask(contextProjectName,contextPath);
        }

        /// <summary>
        /// 生成实体类
        /// </summary>
        private static void GenerationClass()
        {

            string classProjectName = "SugarCodeGeneration";//实体类项目名称
            string classPath = "Models";//生成的目录
            string classNamespace = "MyTest";//实体命名空间
            var db = GetDB();
            var classDirectory = Methods.GetSlnPath + "\\" + classProjectName + "\\" + classPath.TrimStart('\\');

            //如果生成全部可以把Where去掉
            db.DbFirst.Where("Student", "School").IsCreateAttribute().CreateClassFile(classDirectory, classNamespace);

            AddTask(classProjectName,classPath);
        }

        /// <summary>
        ///  修改解决方案
        /// </summary>
        private static void UpdateCsproj()
        {
            foreach (var item in CsprojList)
            {
                item.Start();
                item.Wait();
            }
        }

        private static void AddTask(string bllProjectName, string bllPath)
        {
            var task = new Task(() =>
            {
                Methods.AddCsproj(bllPath, bllProjectName);
            });
            CsprojList.Add(task);
        }
    }
}
