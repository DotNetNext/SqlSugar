using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    internal class PostgreSQLInserttable<T> :InsertableProvider<T> where T : class, new()
    {
    }
    internal class PostgreSQLExpressionContext : ExpressionContext, ILambdaExpressions
    {
        public SqlSugarContext Context
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
