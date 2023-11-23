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

        protected virtual SqlSugarClient GetContext()
        {
            SqlSugarClient result = null;
            var key = _configs.GetHashCode().ToString();
            StackTrace st = new StackTrace(true);
            var methods = st.GetFrames();
            var isAsync = UtilMethods.IsAnyAsyncMethod(methods);
            if (methods.Length>=0) 
            {
                foreach (var method in methods.Take(25)) 
                {
                    var refType = method.GetMethod()?.ReflectedType;
                    if (refType != null)
                    {
                        var getInterfaces = refType.Name.StartsWith("<") ? refType?.ReflectedType?.GetInterfaces() : refType?.GetInterfaces();
                        if (getInterfaces != null && getInterfaces.Any(it => it.Name.IsIn("IJob")))
                        {
                            key = $"{key}IJob";
                            break;
                        }
                    }
                }
            }
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
            return _configs.Select(it =>UtilMethods.CopyConfig(it)).ToList();
        }

    }
}
