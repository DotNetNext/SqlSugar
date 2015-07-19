using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    /// <summary>
    /// ** 描述：逻辑出错抛出异常
    /// ** 创始时间：2015-7-19
    /// ** 修改时间：-
    /// ** 作者：网络
    /// ** 修改人：sunkaixuan
    /// ** 使用说明：
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
