using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    internal class PostgreSQLExpressionContext : ExpressionContext, ILambdaExpressions
    {
        public SqlSugarClient Context
        {
            get
            {
                var message = ErrorMessage.GetThrowMessage(
             "SqlSugar PostGreSQL only support.NET CORE",
             "SqlSugar使用 PostGreSQL只支持.NET CORE");
                throw new Exception(message);
            }

            set
            {
                var message = ErrorMessage.GetThrowMessage(
                     "SqlSugar PostGreSQL only support.NET CORE",
                     "SqlSugar使用 PostGreSQL只支持.NET CORE");
                throw new Exception(message);
            }
        }
    }
}
