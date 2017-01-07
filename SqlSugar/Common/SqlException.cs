using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
namespace SqlSugar
{
    public class SqlSugarException : Exception
    {
        public SqlSugarException(string message)
            : base(message){}

        public SqlSugarException(string message, string sql)
            : base(GetMessage(message, sql)) {}

        public SqlSugarException(string message, string sql, object pars)
            : base(GetMessage(message, sql, pars)){}

        public SqlSugarException(string message, object pars)
            : base(GetMessage(message, pars)){}

        private static string GetMessage(string message, object pars)
        {
            var parsStr = string.Empty; ;
            if (pars != null)
            {
                parsStr = JsonConvert.SerializeObject(pars);
            }
            var reval = GetLineMessage("message", message) + GetLineMessage("function", parsStr);
            return reval;

        }

        private static string GetMessage(string message, string sql, object pars)
        {
            if (pars == null)
            {
                return GetMessage(message, sql);
            }
            else
            {
                var reval = GetLineMessage("message         ", message) + GetLineMessage("ORM Sql", sql) + GetLineMessage("函数参数        ", JsonConvert.SerializeObject(pars));
                return reval;
            }
        }

        private static string GetMessage(string message, string sql)
        {
            var reval = GetLineMessage("message         ", message) + GetLineMessage("ORM Sql", sql);
            return reval;
        }

        private static string GetLineMessage(string key, string value)
        {
            return string.Format("{0} ： 【{1}】\r\n", key, value);
        }
    }
}
