using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Text;
namespace SqlSugar
{
    internal class DependencyManagement
    {
        private static bool IsTryJsonNet = false;
        private static bool IsTryMySqlData = false;
        private static bool IsTrySqlite = false;
        public static void TryJsonNet()
        {
            if (!IsTryJsonNet)
            {
                try
                {
                    var settings =JsonConvert.DefaultSettings;
                    IsTryJsonNet = true;
                }
                catch (Exception ex)
                {
                   var message= ErrorMessage.GetThrowMessage(
                       " Some functions are used in newtonsoft ,Nuget references newtonsoft.json, 9.0.0.1 + ."+ex.Message,
                       " 部分功能用到newtonsoft，Nuget上面下载安装 newtonsoft.json, 9.0.0.1及以上版本，如果有版本兼容问题请先删除原有引用，详细错误："+ex.Message);
                    Check.ThrowNotSupportedException(message);
                }
            }
        }

        public static void TryMySqlData()
        {
            if (!IsTryMySqlData)
            {
                try
                {
                    using (MySqlConnection mySql = new MySqlConnection()){}
                    IsTryMySqlData = true;
                }
                catch (Exception ex)
                {
                    var message = ErrorMessage.GetThrowMessage(
                     "You need to refer to MySql.Data.dll"+ex.Message,
                     "需要MySql.Data.dll请在Nuget安装最新版本，如果有版本兼容问题请先删除原有引用，详细错误：" + ex.Message);
                    Check.ThrowNotSupportedException(message);
                }
            }
        }

        public static void TrySqlite()
        {
            if (!IsTrySqlite)
            {
                try
                {
                    using (SQLiteConnection mySql = new SQLiteConnection()) { }
                    IsTrySqlite = true;
                }
                catch (Exception ex)
                {
                    var message = ErrorMessage.GetThrowMessage(
                     ex.Message,
                    "需要Sqlite相关的驱动，如果有版本兼容问题请先删除原有引用，如果有版本兼容问题请先删除原有引用，详细错误：" + ex.Message);
                    Check.ThrowNotSupportedException(message);
                }
            }
        }
    }
}
