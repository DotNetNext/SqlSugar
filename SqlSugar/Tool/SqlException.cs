using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    /// <summary>
    /// ** 描述：SqlSugar自定义异常
    /// ** 创始时间：2015-7-13
    /// ** 修改时间：-
    /// ** 作者：sunkaixuan
    /// ** 使用说明：
    /// </summary>
    public class SqlSugarException : Exception
    {
        /// <summary>
        /// SqlSugar异常
        /// </summary>
        /// <param name="message">错误信息</param>
        public SqlSugarException(string message)
            : base(message)
        {

        }
        /// <summary>
        /// SqlSugar异常
        /// </summary>
        /// <param name="message">错误信息</param>
        /// <param name="sql">ORM生成的SQL</param>
        public SqlSugarException(string message, string sql)
            : base(GetMessage(message, sql))
        {

        }
        /// <summary>
        /// SqlSugar异常
        /// </summary>
        /// <param name="message">错误信息</param>
        /// <param name="sql">ORM生成的SQL</param>
        /// <param name="pars">错误函数的参数</param>
        public SqlSugarException(string message, string sql, object pars)
            : base(GetMessage(message, sql, pars))
        {

        }
        /// <summary>
        /// SqlSugar异常
        /// </summary>
        /// <param name="message">错误信息</param>
        /// <param name="pars">错误函数的参数</param>
        public SqlSugarException(string message,object pars)
            : base(GetMessage(message,pars))
        {

        }

        private static string GetMessage(string message, object pars)
        {
            var parsStr = string.Empty; ;
            if (pars != null)
            {
                parsStr = JsonConverter.Serialize(pars);
            }
            var reval = GetLineMessage("错误信息", message) + GetLineMessage("函数参数", parsStr);
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
                var reval = GetLineMessage("错误信息         ", message) + GetLineMessage("ORM生成的Sql", sql) + GetLineMessage("函数参数        ", JsonConverter.Serialize(pars));
                return reval;
            }
        }


        private static string GetMessage(string message, string sql)
        {
            var reval = GetLineMessage("错误信息         ", message) + GetLineMessage("ORM生成的Sql", sql);
            return reval;
        }

        private static string GetLineMessage(string key, string value)
        {
            return string.Format("{0} ： 【{1}】\r\n", key, value);
        }
    }
}
