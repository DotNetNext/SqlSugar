using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace SqlSugar
{
    public class Check
    {
        public static void ThrowNotSupportedException(string message,Exception ex=null)
        {
            message = message.IsNullOrEmpty() ? new NotSupportedException().Message : message;
            if(ex == null)
            {
                throw new SqlSugarException("SqlSugarException.NotSupportedException：" + message);
            }
            else
            {
                throw new SqlSugarException("SqlSugarException.NotSupportedException：" + message, ex);
            }
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
        public static void Exception(bool isException, Exception ex, string message, params string[] args)
        {
            if (isException)
                throw new SqlSugarException(string.Format(message, args), ex);
        }
        public static void ExceptionEasy(string enMessage, string cnMessage,Exception ex = null)
        {
            if(ex == null)
            {
                throw new SqlSugarException(ErrorMessage.GetThrowMessage(enMessage, cnMessage));
            }
            else
            {
                throw new SqlSugarException(ErrorMessage.GetThrowMessage(enMessage, cnMessage),ex);
            }
        }
        public static void ExceptionEasy(bool isException, string enMessage, string cnMessage)
        {
            if (isException)
                throw new SqlSugarException(ErrorMessage.GetThrowMessage(enMessage, cnMessage));
        }
    }
}
