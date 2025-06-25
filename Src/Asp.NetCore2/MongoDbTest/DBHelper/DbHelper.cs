using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDbTest 
{
    /// <summary>
    /// Helper class for database operations
    /// 数据库操作的辅助类
    /// </summary>
    public class DbHelper
    {
        public static string ConnectionString = "mongodb://root:123456@117.72.212.3:27017/testDB?authSource=admin";
        public static string SqlSugarConnectionString = "host=117.72.212.3;Port=27017;Database=testDB;Username= root;Password=123456;authSource=admin;replicaSet=";

        /// <summary>
        /// Get a new SqlSugarClient instance with specific configurations
        /// 获取具有特定配置的新 SqlSugarClient 实例
        /// </summary>
        /// <returns>SqlSugarClient instance</returns>
        public static SqlSugarClient GetNewDb()
        {
            //注册DLL防止找不到DLL
            InstanceFactory.CustomAssemblies = new System.Reflection.Assembly[] {
            typeof(SqlSugar.MongoDb.MongoDbProvider).Assembly };

            //创建DB
            var db = new SqlSugarClient(new ConnectionConfig()
            {
                IsAutoCloseConnection = true,
                DbType = DbType.MongoDb,
                ConnectionString = SqlSugarConnectionString,
                LanguageType = LanguageType.Default//Set language

            },
            it =>
            { 
                it.Aop.OnLogExecuting = (sql, para) =>
                {
                    Console.WriteLine(UtilMethods.GetNativeSql(sql, para));
                };
            });
            return db;
        }
    }
}