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
        private static bool IsTryDm = false;
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
                        " Newtonsoft.Json.dll 未安装或者版本冲突，按下步骤操作即可 1、从Nuget卸载所有项目的Newtonsoft.Json和SqlSugar，检查引用中是否还存在,存在直接删掉引用，然后Nuget重新安装即可");
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
                     " MySql.Data.dll  未安装或者版本冲突，按下面步骤操作即可 1、从Nuget卸载所有项目的MySql.Data.dll和SqlSugar，检查引用中是否还存在,存在直接删掉引用，然后Nuget重新安装即可 ，注意.NET 4.62版本以下用户只能用MySql.Data 6.x版本");
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
                     " Npgsql.dll 未安装或者版本冲突，按下面步骤操作即可 1、从Nuget卸载所有项目的Npgsql.dll和SqlSugar，检查引用中是否还存在,存在直接删掉引用，然后Nuget重新安装即可");
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
                     "需要引用ManagedDataAccess.dll 未安装或者版本冲突， 按下面步骤操作即可 1、从Nuget卸载所有项目的ManagedDataAccess.dll和SqlSugar，检查引用中是否还存在,存在直接删掉引用，然后Nuget重新安装即可");
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
                    "System.Data.SQLite.dll  未安装或者版本冲突，按下面步骤操作即可 1、从Nuget卸载所有项目的System.Data.SQLite.dll 和SqlSugar，检查引用中是否还存在,存在直接删掉引用，然后Nuget重新安装即可");
                    throw new Exception(message);
                }
            }
        }

        public static void TryKdbndb()
        {
            throw new Exception("Kdbndb只能在.NetCore版本下使用");
        }

        public static void TryDm()
        {
            if (!IsTryDm)
            {
                try
                {
                    DmProvider db = new DmProvider();
                    var conn = db.GetAdapter();
                    IsTryDm = true;
                }
                catch (Exception ex)
                {

                    throw new Exception("你需要引用DmProvider.dll，在https://github.com/sunkaixuan/SqlSugar 源码里面下载");
                }
            }
        }
    }
}
