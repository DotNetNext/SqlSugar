using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    /// <summary>
    /// SQL安全异常处理
    /// </summary>
    public class SqlSecurityException : Exception
    {
        public SqlSecurityException(string message)
            : base(message)
        {

        }
    }
}
