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
        //数据库配置
        const SqlSugar.DbType dbType = SqlSugar.DbType.SqlServer;
        const string connectionString = "server=.;uid=sa;pwd=@jhl85661501;database=SqlSugar4XTest";
 
        static void Main(string[] args)
        {
            /***连接数据库***/
            var db = GetDB();


            /***生成实体***/

            //配置参数
            string classProjectName = "Entity";//实体类项目名称
            string classPath = "DbModels";//生成的目录
            string classNamespace = "MyTest";//实体命名空间
            var classDirectory = Methods.GetSlnPath + "\\" + classProjectName + "\\" + classPath.TrimStart('\\');
            //执行生成
            GenerationClass(classProjectName, classPath, classNamespace, classDirectory);
            Print("实体创建成功");





            /***生成DbContext***/

            //配置参数
            var contextProjectName = "BLL";//DbContext所在项目
            var contextPath = "DbCore";//dbcontext存储目录
            var savePath = Methods.GetSlnPath + "\\" + contextProjectName + "\\" + contextPath + "\\DbContext.cs";//具体文件名
            var tables = db.DbMaintenance.GetTableInfoList().Select(it => it.Name).ToList();
            //执行生成
            GenerationDContext(contextProjectName, contextPath, savePath, tables);
            Print("DbContext创建成功");

            



            /***生成BLL***/

            //配置参数
            var bllProjectName2 = "BLL";//具体项目
            var bllPath2 = "BaseBLL";//文件目录
            var savePath2 = Methods.GetSlnPath + "\\" + bllProjectName2 + "\\" + bllPath2;//保存目录
            var tables2 = db.DbMaintenance.GetTableInfoList().Select(it => it.Name).ToList();
            //执行生成
            GenerationBLL(bllProjectName2,bllPath2,savePath2, tables2);
            Print("BLL创建成功");





            /***修改解决方案***/
            UpdateCsproj();
            Print("项目解决方案修改成功");

            //如何使用创建好的业务类（注意 SchoolManager 不能是静态的）
            //SchoolManager sm = new SchoolManager();
            //sm.GetList();
            //sm.StudentDb.AsQueryable().Where(it => it.Id == 1).ToList();
            //sm.Db.Queryable<Student>().ToList();

        }

 
      /// <summary>
        /// 生成BLL
        /// </summary>
        private static void GenerationBLL(string bllProjectName, string bllPath, string savePath, List<string>tables)
        {
            var templatePath = Methods.GetCurrentProjectPath + "\\Template\\Bll.txt";//bll模版地址
            //下面代码不动
            Methods.CreateBLL(templatePath, savePath, tables);
            AddTask(bllProjectName, bllPath);
        }



        /// <summary>
        /// 生成DbContext
        /// </summary>
        private static void GenerationDContext(string contextProjectName, string contextPath, string savePath,List<string> tables)
        {
            var templatePath = Methods.GetCurrentProjectPath + "\\Template\\DbContext.txt";//dbcontexts模版文件
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
        private static void GenerationClass(string classProjectName,string classPath,string classNamespace,string classDirectory)
        {
            //连接数据库
            var db = GetDB();
            //下面代码不动
            db.DbFirst.IsCreateAttribute().CreateClassFile(classDirectory, classNamespace);
            AddTask(classProjectName,classPath);
        }

        #region 辅助方法
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
        private static void Print(string message)
        {
            Console.WriteLine("");
            Console.WriteLine(message);
            Console.WriteLine("");
        }

        private static void AddTask(string bllProjectName, string bllPath)
        {
            var task = new Task(() =>
            {
                Methods.AddCsproj(bllPath, bllProjectName);
            });
            CsprojList.Add(task);
        }
        static List<Task> CsprojList = new List<Task>();
        static SqlSugar.SqlSugarClient GetDB()
        {

            return new SqlSugar.SqlSugarClient(new SqlSugar.ConnectionConfig()
            {
                DbType = dbType,
                ConnectionString = connectionString,
                IsAutoCloseConnection = true,
                InitKeyType = SqlSugar.InitKeyType.Attribute
            });
        }
        #endregion
    }
}
