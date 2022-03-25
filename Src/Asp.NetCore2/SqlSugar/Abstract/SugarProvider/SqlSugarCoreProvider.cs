using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    /// <summary>
    /// Partial SqlSugarScope
    /// </summary>
    public partial class SqlSugarScope : ISqlSugarClient, ITenant
    {

        private List<ConnectionConfig> _configs;
        private Action<SqlSugarClient> _configAction;

        private SqlSugarClient GetContext()
        {
            SqlSugarClient result = null;
            var key = _configs.GetHashCode().ToString();
            StackTrace st = new StackTrace(true);
            var methods = st.GetFrames();
            var isAsync = UtilMethods.IsAnyAsyncMethod(methods);
            if (isAsync)
            {
                result = GetAsyncContext(key);
            }
            else
            {
                result = GetThreadContext(key);
            }
            return result;
        }
        private SqlSugarClient GetAsyncContext(string key)
        {
            SqlSugarClient result = CallContextAsync<SqlSugarClient>.GetData(key);
            if (result == null)
            {
                List<ConnectionConfig> configList = GetCopyConfigs();
                CallContextAsync<SqlSugarClient>.SetData(key, new SqlSugarClient(configList));
                result = CallContextAsync<SqlSugarClient>.GetData(key);
                if (this._configAction != null)
                {
                    this._configAction(result);
                }
            }

            return result;
        }
        private SqlSugarClient GetThreadContext(string key)
        {
            SqlSugarClient result = CallContextThread<SqlSugarClient>.GetData(key);
            if (result == null)
            {
                List<ConnectionConfig> configList = GetCopyConfigs();
                CallContextThread<SqlSugarClient>.SetData(key, new SqlSugarClient(configList));
                result = CallContextThread<SqlSugarClient>.GetData(key);
                if (this._configAction != null)
                {
                    this._configAction(result);
                }
            }
            return result;
        }
        private List<ConnectionConfig> GetCopyConfigs()
        {
            return _configs.Select(it => new ConnectionConfig()
            {
                AopEvents = it.AopEvents,
                ConfigId = it.ConfigId,
                ConfigureExternalServices = it.ConfigureExternalServices,
                ConnectionString = it.ConnectionString,
                DbType = it.DbType,
                IndexSuffix = it.IndexSuffix,
                InitKeyType = it.InitKeyType,
                IsAutoCloseConnection = it.IsAutoCloseConnection,
                LanguageType = it.LanguageType,
                MoreSettings = it.MoreSettings == null ? null : new ConnMoreSettings()
                {
                    DefaultCacheDurationInSeconds = it.MoreSettings.DefaultCacheDurationInSeconds,
                    DisableNvarchar = it.MoreSettings.DisableNvarchar,
                    PgSqlIsAutoToLower = it.MoreSettings.PgSqlIsAutoToLower,
                    IsAutoRemoveDataCache = it.MoreSettings.IsAutoRemoveDataCache,
                    IsWithNoLockQuery = it.MoreSettings.IsWithNoLockQuery,
                    TableEnumIsString=it.MoreSettings.TableEnumIsString
                },
                SlaveConnectionConfigs = it.SlaveConnectionConfigs
            }).ToList();
        }
    }
}
