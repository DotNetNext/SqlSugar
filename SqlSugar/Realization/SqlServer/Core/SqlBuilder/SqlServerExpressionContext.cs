using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public partial class SqlServerExpressionContext : ExpressionContext, ILambdaExpressions
    {
        public SqlSugarClient Context { get; set; }
        public SqlServerExpressionContext()
        {
            base.DbMehtods = new SqlServerMethod();
        }

    }
    public partial class SqlServerMethod : DefaultDbMethod, IDbMethods
    {

    }
}
