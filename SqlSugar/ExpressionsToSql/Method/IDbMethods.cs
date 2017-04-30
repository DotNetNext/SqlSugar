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
        string ToLower(MethodCallExpressionModel model);
        string ToUpper(MethodCallExpressionModel model);
        string Trim(MethodCallExpressionModel model);
        string Contains(MethodCallExpressionModel model);
        string Equals(MethodCallExpressionModel model);
        string DateIsSameDay(MethodCallExpressionModel model);
        string DateIsSameByType(MethodCallExpressionModel model);
        string DateAddByType(MethodCallExpressionModel model);
        string DateValue(MethodCallExpressionModel model);
        string DateAddDay(MethodCallExpressionModel model);
        string Between(MethodCallExpressionModel model);
        string StartsWith(MethodCallExpressionModel model);
        string EndsWith(MethodCallExpressionModel model);
    }
}
