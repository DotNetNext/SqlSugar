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
    public class SqlServerExpressionContext : ExpressionContext
    {
        public SqlServerExpressionContext(Expression expression, ResolveExpressType resolveType) : base(expression, resolveType)
        {
            base.DbMehtods = new SqlServerMethod();
        }
    }
    public class SqlServerMethod : IDbMethods
    {
        public string IsNullOrEmpty(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            if (parameter.IsMember)
            {
                return string.Format("( {0}='' OR {0} IS NULL )", parameter.Value);
            }
            else
            {
                return null;
            }
        }
    }
}
