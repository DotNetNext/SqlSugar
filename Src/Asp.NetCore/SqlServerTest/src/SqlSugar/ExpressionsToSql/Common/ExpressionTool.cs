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
                default:
                    Check.ThrowNotSupportedException(string.Format(ExpressionErrorMessage.OperatorError, expressiontype.ToString()));
                    return null;
            }
        }

        public static object GetMemberValue(MemberInfo member, Expression expression)
        {
            var memberInfos = new Stack<MemberInfo>();
            var fieldInfo = member as System.Reflection.FieldInfo;
            object reval = null;
            // "descend" toward's the root object reference:
            while (expression is MemberExpression)
            {
                var memberExpr = expression as MemberExpression;
                memberInfos.Push(memberExpr.Member);
                if (memberExpr.Expression == null)
                {
                    var isProperty = memberExpr.Member.MemberType == MemberTypes.Property;
                    var isField = memberExpr.Member.MemberType == MemberTypes.Field;
                    if (isProperty)
                    {
                        reval = GetPropertyValue(memberExpr);
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

                    }
                    objReference = objProp.GetValue(objReference, null);
                }
                else if (mi.MemberType == MemberTypes.Field)
                {
                    var objField = objReference.GetType().GetField(mi.Name);
                    if (objField == null)
                    {

                    }
                    objReference = objField.GetValue(objReference);
                }
            }
            reval = objReference;
            return reval;
        }

        public static object GetFiledValue(MemberExpression memberExpr)
        {
            object reval = null;
            FieldInfo field = (FieldInfo)memberExpr.Member;
            reval = field.GetValue(memberExpr.Member);
            if (reval != null && reval.GetType().IsClass && reval.GetType() != ExpressionConst.StringType)
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

                }
            }
            return reval;
        }

        public static object GetPropertyValue(MemberExpression memberExpr)
        {
            object reval = null;
            PropertyInfo pro = (PropertyInfo)memberExpr.Member;
            reval = pro.GetValue(memberExpr.Member, null);
            if (reval != null && reval.GetType().IsClass && reval.GetType() != ExpressionConst.StringType)
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

                }
            }
            return reval;
        }

        public static object DynamicInvoke(MemberExpression expression)
        {
            object value = Expression.Lambda(expression).Compile().DynamicInvoke();
            if (value != null && value.GetType().IsClass && value.GetType() != ExpressionConst.StringType)
            {
                value = Expression.Lambda(expression).Compile().DynamicInvoke();
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
            return type.IsClass && type!=ExpressionConst.StringType;
        }

        public static bool IsValueType(Type type)
        {
            return !IsEntity(type);
        }
    }
}
