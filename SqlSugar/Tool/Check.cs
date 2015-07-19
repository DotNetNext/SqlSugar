using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    /// <summary>
    /// 异常处理类
    /// </summary>
    public class Check
    {
        public static void ArgumentNullException(object checkObj, string message)
        {
            if (checkObj == null)
                throw new ArgumentNullException(message);
        }
        public static void Exception(bool isException, string message, params string[] args)
        {
            if (isException)
                throw new Exception(string.Format(message, args));
        }
    }

    
}
