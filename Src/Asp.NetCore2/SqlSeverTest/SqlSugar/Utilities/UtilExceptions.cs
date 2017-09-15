using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
namespace SqlSugar
{
    public class UtilExceptions : Exception
    {
        public UtilExceptions(string message)
            : base(message){}

        public UtilExceptions(SqlSugarClient context,string message, string sql)
            : base(GetMessage(context, message, sql)) {}

        public UtilExceptions(SqlSugarClient context, string message, string sql, object pars)
            : base(GetMessage(context,message, sql, pars)){}

        public UtilExceptions(SqlSugarClient context, string message, object pars)
            : base(GetMessage(context,message, pars)){}

        private static string GetMessage(SqlSugarClient context, string message, object pars)
        {
            var parsStr = string.Empty; ;
            if (pars != null)
            {
                parsStr = context.Utilities.SerializeObject(pars);
            }
            var reval = GetLineMessage("message", message) + GetLineMessage("function", parsStr);
            return reval;

        }
        private static string GetMessage(SqlSugarClient context, string message, string sql, object pars)
        {
            if (pars == null)
            {
                return GetMessage(context,message, sql);
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
            return string.Format("{0} ： '{1}' \r\n", key, value);
        }
    }
}
