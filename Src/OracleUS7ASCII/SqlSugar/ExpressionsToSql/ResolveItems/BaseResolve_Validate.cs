using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
namespace SqlSugar
{
    /// <summary>
    /// BaseResolve-Validate
    /// </summary>
    public partial class BaseResolve
    {

        private static bool IsSubToList(Expression item)
        {
            return ExpressionTool.GetMethodName(item).IsIn("ToList","First") && IsSubquery(item);
        }

        private static bool IsSubquery(Expression item)
        {
            var method = (item as MethodCallExpression);
            if (method == null)
                return false;
            if (method.Object == null)
                return false;
            return method.Object.Type.Name.StartsWith("Subquery");
        }

        private bool IsExtSqlFuncObj(Expression item)
        {
            return this.Context.SqlFuncServices != null && item is MethodCallExpression && this.Context.SqlFuncServices.Any(it => it.UniqueMethodName == ExpressionTool.GetMethodName(item));
        }
        private bool IsNullValue(ExpressionParameter parameter, object value)
        {
            return value == null
                    && !parameter.ValueIsNull
                    && parameter.BaseParameter != null
                    && parameter.BaseParameter.OperatorValue.IsIn("=", "<>")
                    && this.Context.ResolveType.IsIn(ResolveExpressType.WhereMultiple, ResolveExpressType.WhereSingle);
        }
        private static bool IsNotCaseExpression(Expression item)
        {
            if ((item as MethodCallExpression).Method.Name == "IIF")
            {
                return false;
            }
            else if ((item as MethodCallExpression).Method.Name == "IsNull")
            {
                return false;
            }
            else if ((item as MethodCallExpression).Method.Name == "End" && item.ToString().Contains("IF("))
            {
                return false;
            }
            else if ((item as MethodCallExpression).Method.Name == "AggregateMax")
            {
                return false;
            }
            else if ((item as MethodCallExpression).Method.Name == "AggregateMin")
            {
                return false;
            }
            else if ((item as MethodCallExpression).Method.Name == "AggregateSum")
            {
                return false;
            }
            else if ((item as MethodCallExpression).Method.Name == "ToBool")
            {
                return false;
            }
            else if ((item as MethodCallExpression).Method.Name == "ToBoolean")
            {
                return false;
            }
            else if ((item as MethodCallExpression).Method.Name == "Select" && item.ToString().Contains("Subqueryable()"))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private static bool IsBoolValue(Expression item)
        {
            return item.Type == UtilConstants.BoolType &&
                                   (item is MemberExpression) &&
                                   (item as MemberExpression).Expression != null &&
                                   (item as MemberExpression).Expression.Type == typeof(bool?) &&
                                    (item as MemberExpression).Member.Name == "Value";
        }
        protected static bool IsConvert(Expression item)
        {
            return item is UnaryExpression && item.NodeType == ExpressionType.Convert;
        }
        protected static bool IsNotMember(Expression item)
        {
            return item is UnaryExpression &&
                                     item.Type == UtilConstants.BoolType &&
                                    (item as UnaryExpression).NodeType == ExpressionType.Not &&
                                    (item as UnaryExpression).Operand is MemberExpression &&
                                   ((item as UnaryExpression).Operand as MemberExpression).Expression != null &&
                                   ((item as UnaryExpression).Operand as MemberExpression).Expression.NodeType == ExpressionType.Parameter;
        }
        protected static bool IsNotParameter(Expression item)
        {
            return item is UnaryExpression &&
                                     item.Type == UtilConstants.BoolType &&
                                    (item as UnaryExpression).NodeType == ExpressionType.Not &&
                                    (item as UnaryExpression).Operand is MemberExpression &&
                                   ((item as UnaryExpression).Operand as MemberExpression).Expression != null &&
                                   ((item as UnaryExpression).Operand as MemberExpression).Expression.NodeType == ExpressionType.MemberAccess;
        }
        protected bool IsSubMethod(MethodCallExpression express)
        {
            return SubTools.SubItemsConst.Any(it => express.Object != null && express.Object.Type.Name.StartsWith("Subqueryable`"));
        }
    }
}
