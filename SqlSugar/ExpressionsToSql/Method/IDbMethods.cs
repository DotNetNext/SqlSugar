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
    public interface IDbMethods
    {
        string IsNullOrEmpty(MethodCallExpressionModel model);
        object ToLower(MethodCallExpressionModel model);
        object ToUpper(MethodCallExpressionModel model);
        object Trim(MethodCallExpressionModel model);
    }
}
