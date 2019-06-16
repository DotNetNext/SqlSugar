using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
namespace SqlSugar
{
    public class ExpressionTool
    {
        public static string GetOperator(ExpressionType expressiontype)
        {
            switch (expressiontype)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    return "AND";
                case ExpressionType.Equal:
                    return "=";
                case ExpressionType.GreaterThan:
                    return ">";
                case ExpressionType.GreaterThanOrEqual:
                    return ">=";
                case ExpressionType.LessThan:
                    return "<";
                case ExpressionType.LessThanOrEqual:
                    return "<=";
                case ExpressionType.NotEqual:
                    return "<>";
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    return "OR";
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                    return "+";
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    return "-";
                case ExpressionType.Divide:
                    return "/";
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                    return "*";
                case ExpressionType.Modulo:
                    return "%";
                case ExpressionType.Coalesce:
                    throw new Exception("Expression no support ?? ,Use SqlFunc.IsNull");
                default:
                    Check.ThrowNotSupportedException(string.Format(ErrorMessage.OperatorError, expressiontype.ToString()));
                    return null;
            }
        }

        public static object GetValue(object value)
        {
            if (value == null) return value;
            var type = value.GetType();
            if (type.IsEnum()&& type != typeof(DateType)&& type!=typeof(JoinType)&&type!=typeof(OrderByType)) return Convert.ToInt64(value);
            else
                return value;
        }

        public static bool IsLogicOperator(string operatorValue)
        {
            return operatorValue == "&&" || operatorValue == "||"||operatorValue == "AND" || operatorValue == "OR";
        }

        public static bool IsLogicOperator(Expression expression)
        {
            return expression.NodeType == ExpressionType.And ||
                                        expression.NodeType == ExpressionType.AndAlso ||
                                        expression.NodeType == ExpressionType.Or ||
                                        expression.NodeType == ExpressionType.OrElse;
        }
        public static bool IsComparisonOperator(Expression expression)
        {
            return expression.NodeType != ExpressionType.And &&
                                        expression.NodeType != ExpressionType.AndAlso &&
                                        expression.NodeType != ExpressionType.Or &&
                                        expression.NodeType != ExpressionType.OrElse;
        }
        public static object GetMemberValue(MemberInfo member, Expression expression)
        {
            var rootExpression = expression as MemberExpression;
            var memberInfos = new Stack<MemberInfo>();
            var fieldInfo = member as System.Reflection.FieldInfo;
            object reval = null;
            MemberExpression memberExpr = null;
            while (expression is MemberExpression)
            {
                memberExpr = expression as MemberExpression;
                memberInfos.Push(memberExpr.Member);
                if (memberExpr.Expression == null)
                {
                    var isProperty = memberExpr.Member.MemberType == MemberTypes.Property;
                    var isField = memberExpr.Member.MemberType == MemberTypes.Field;
                    if (isProperty)
                    {
                        try
                        {
                            reval = GetPropertyValue(memberExpr);
                        }
                        catch 
                        {
                            reval = null;
                        }
                    }
                    else if (isField)
                    {
                        reval = GetFiledValue(memberExpr);
                    }
                }
                if (memberExpr.Expression == null)
                {

                }
                expression = memberExpr.Expression;
            }
            // fetch the root object reference:
            var constExpr = expression as ConstantExpression;
            if (constExpr == null) {
                return DynamicInvoke(rootExpression);
            }
            object objReference = constExpr.Value;
            // "ascend" back whence we came from and resolve object references along the way:
            while (memberInfos.Count > 0)  // or some other break condition
            {
                var mi = memberInfos.Pop();
                if (mi.MemberType == MemberTypes.Property)
                {
                    var objProp = objReference.GetType().GetProperty(mi.Name);
                    if (objProp == null)
                    {
                        objReference = DynamicInvoke(expression, rootExpression==null?memberExpr: rootExpression);
                    }
                    else
                    {
                        objReference = objProp.GetValue(objReference, null);
                    }
                }
                else if (mi.MemberType == MemberTypes.Field)
                {
                    var objField = objReference.GetType().GetField(mi.Name);
                    if (objField == null)
                    {
                        objReference = DynamicInvoke(expression, rootExpression==null?memberExpr: rootExpression);
                    }
                    else
                    {
                        objReference = objField.GetValue(objReference);
                    }
                }
            }
            reval = objReference;
            return reval;
        }

        public static object GetFiledValue(MemberExpression memberExpr)
        {
            if (!(memberExpr.Member is FieldInfo))
            {
                return DynamicInvoke(memberExpr);
            }
            object reval = null;
            FieldInfo field = (FieldInfo)memberExpr.Member;
            Check.Exception(field.IsPrivate, string.Format(" Field \"{0}\" can't be private ", field.Name));
            reval = field.GetValue(memberExpr.Member);
            if (reval != null && reval.GetType().IsClass() && reval.GetType() != UtilConstants.StringType)
            {
                var fieldName = memberExpr.Member.Name;
                var proInfo = reval.GetType().GetProperty(fieldName);
                if (proInfo != null)
                {
                    reval = proInfo.GetValue(reval, null);
                }
                var fieInfo = reval.GetType().GetField(fieldName);
                if (fieInfo != null)
                {
                    reval = fieInfo.GetValue(reval);
                }
                if (fieInfo == null && proInfo == null)
                {
                    Check.Exception(field.IsPrivate, string.Format(" Field \"{0}\" can't be private ", field.Name));
                }
            }
            return reval;
        }


        public static bool IsConstExpression(MemberExpression memberExpr)
        {
            var result = false;
            while (memberExpr!=null&&memberExpr.Expression != null)
            {
                var isConst = memberExpr.Expression is ConstantExpression;
                if (isConst)
                {
                    result = true;
                    break;
                }
                memberExpr = memberExpr.Expression as MemberExpression;
            }
            return result;
        }

        public static object GetPropertyValue(MemberExpression memberExpr)
        {
            if (!(memberExpr.Member is PropertyInfo))
            {
                return DynamicInvoke(memberExpr);
            }
            object reval = null;
            PropertyInfo pro = (PropertyInfo)memberExpr.Member;
            reval = pro.GetValue(memberExpr.Member, null);
            if (reval != null && reval.GetType().IsClass() && reval.GetType() != UtilConstants.StringType)
            {
                var fieldName = memberExpr.Member.Name;
                var proInfo = reval.GetType().GetProperty(fieldName);
                if (proInfo != null)
                {
                    reval = proInfo.GetValue(reval, null);
                }
                var fieInfo = reval.GetType().GetField(fieldName);
                if (fieInfo != null)
                {
                    reval = fieInfo.GetValue(reval);
                }
                if (fieInfo == null && proInfo == null)
                {
                    Check.Exception(true, string.Format(" Property \"{0}\" can't be private ", pro.Name));
                }
            }
            return reval;
        }

        public static object DynamicInvoke(Expression expression,MemberExpression memberExpression=null)
        {
            object value = Expression.Lambda(expression).Compile().DynamicInvoke();
            if (value != null && value.GetType().IsClass() && value.GetType() != UtilConstants.StringType&& memberExpression!=null)
            {
                value = Expression.Lambda(memberExpression).Compile().DynamicInvoke();
            }

            return value;
        }

        public static Type GetPropertyOrFieldType(MemberInfo propertyOrField)
        {
            if (propertyOrField.MemberType == MemberTypes.Property)
                return ((PropertyInfo)propertyOrField).PropertyType;
            if (propertyOrField.MemberType == MemberTypes.Field)
                return ((FieldInfo)propertyOrField).FieldType;
            throw new NotSupportedException();
        }

        public static bool IsEntity(Type type)
        {
            return type.IsClass() && type != UtilConstants.StringType;
        }

        public static bool IsValueType(Type type)
        {
            return !IsEntity(type);
        }

        public static bool IsUnConvertExpress(Expression item)
        {
            return item is UnaryExpression && item.NodeType == ExpressionType.Convert;
        }
    }
}
