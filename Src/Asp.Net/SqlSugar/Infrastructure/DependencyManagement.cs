using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
namespace SqlSugar
{
    internal class DependencyManagement
    {
        private static bool IsTryJsonNet = false;
        private static bool IsTryMySqlData = false;
        private static bool IsTrySqlite = false;
        private static bool IsTryOracle = false;
        private static bool IsTryPgSql = false;
        public static void TryJsonNet()
        {
            if (!IsTryJsonNet)
            {
                try
                {
                    new SerializeService().SerializeObject(new { });
                    IsTryJsonNet = true;
                }
                catch
                {
                    var message = ErrorMessage.GetThrowMessage(
                        " SqlSugar Some functions are used in newtonsoft ,Nuget references Newtonsoft.Json 9.0.0.1 + .",
                        " SqlSugar 部分功能用到Newtonsoft.Json.dll，需要在Nuget上安装 Newtonsoft.Json 9.0.0.1及以上版本。如果有版本兼容问题请先删除原有引用（注意：所有项目类库）,全部重新从NUGET下载,如果还不明白，请查看详细教程 http://www.codeisbug.com/Doc/8/1154");
                    throw new Exception(message);
                }
            }
        }
        public static void TryMySqlData()
        {
            if (!IsTryMySqlData)
            {
                try
                {
                    MySqlProvider db = new MySqlProvider();
                    var conn = db.GetAdapter();
                    IsTryMySqlData = true;
                }
                catch
                {
                    var message = ErrorMessage.GetThrowMessage(
                     "You need to refer to MySql.Data.dll",
                     "需要引用MySql.Data.dll，请在Nuget安装最新稳定版本,如果有版本兼容问题请先删除原有引用");
                    throw new Exception(message);
                }
            }
        }

        public static void TryPostgreSQL()
        {
            if (!IsTryPgSql)
            {
                try
                {
                    PostgreSQLProvider db = new PostgreSQLProvider();
                    var conn = db.GetAdapter();
                    IsTryPgSql = true;
                }
                catch
                {
                    var message = ErrorMessage.GetThrowMessage(
                     "You need to refer to Npgsql.dll",
                     "需要引用Npgsql.dll，请在Nuget安装最新稳定版本,如果有版本兼容问题请先删除原有引用");
                    throw new Exception(message);
                }
            }
        }

        internal static void TryOracle()
        {
            if (!IsTryOracle)
            {
                try
                {
                    OracleProvider db = new OracleProvider();
                    var conn = db.GetAdapter();
                    IsTryOracle = true;
                }
                catch
                {
                    var message = ErrorMessage.GetThrowMessage(
                     "You need to refer to Oracle.ManagedDataAccess.dll",
                     "需要引用ManagedDataAccess.dll，请在Nuget安装最新稳定版本,如果有版本兼容问题请先删除原有引用");
                    throw new Exception(message);
                }
            }
        }

        public static void TrySqlite()
        {
            if (!IsTrySqlite)
            {
                try
                {
                    SqliteProvider db = new SqliteProvider();
                    var conn = db.GetAdapter();
                    IsTrySqlite = true;
                }
                catch (Exception ex)
                {
                    var message = ErrorMessage.GetThrowMessage(
                     "You need to refer to System.Data.SQLite.dll." + ex.Message,
                    "你需要引用System.Data.SQLite.dll,如果有版本兼容问题请先删除原有引用");
                    throw new Exception(message);
                }
            }
        }
    }
}
