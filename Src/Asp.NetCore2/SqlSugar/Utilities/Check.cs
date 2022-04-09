using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace SqlSugar
{
    public class Check
    {
        public static void ThrowNotSupportedException(string message)
        {
            message = message.IsNullOrEmpty() ? new NotSupportedException().Message : message;
            throw new SqlSugarException("SqlSugarException.NotSupportedException：" + message);
        }

        public static void ArgumentNullException(object checkObj, string message)
        {
            if (checkObj == null)
                throw new SqlSugarException("SqlSugarException.ArgumentNullException：" + message);
        }

        public static void ArgumentNullException(object [] checkObj, string message)
        {
            if (checkObj == null|| checkObj.Length==0)
                throw new SqlSugarException("SqlSugarException.ArgumentNullException：" + message);
        }

        public static void Exception(bool isException, string message, params string[] args)
        {
            if (isException)
                throw new SqlSugarException(string.Format(message, args));
        }
        public static void ExceptionEasy(string enMessage, string cnMessage)
        {
            throw new SqlSugarException(ErrorMessage.GetThrowMessage(enMessage, cnMessage));
        }
        public static void ExceptionEasy(bool isException, string enMessage, string cnMessage)
        {
            if (isException)
                throw new SqlSugarException(ErrorMessage.GetThrowMessage(enMessage, cnMessage));
        }
    }
}
