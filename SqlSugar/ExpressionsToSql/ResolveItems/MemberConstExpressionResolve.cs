using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public class MemberConstExpressionResolve : BaseResolve
    {
        public MemberConstExpressionResolve(ExpressionParameter parameter) : base(parameter)
        {
            var expression = base.Expression as MemberExpression;
            var isLeft = parameter.IsLeft;
            var isSingle = parameter.Context.IsSingle;
            object value = GetValue(expression.Member, expression);
            if (parameter.BaseParameter.BinaryExpressionInfoList != null)
            {
                parameter.BaseParameter.BinaryExpressionInfoList.Add(new KeyValuePair<string, BinaryExpressionInfo>(ExpressionConst.BinaryExpressionInfoListKey, new BinaryExpressionInfo()
                {
                    IsLeft = Convert.ToBoolean(isLeft),
                    Value = value,
                    ExpressionType = expression.Expression.GetType()
                }));
            }
            if (isLeft == null && base.Context.SqlWhere == null)
            {
                base.Context.SqlWhere = new StringBuilder();
                base.Context.SqlWhere.Append(value);
            }
        }
        private object GetValue(MemberInfo member, Expression expression)
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
            if (constExpr == null)
            {

            }
            var objReference = constExpr.Value;

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

        private static object GetFiledValue(MemberExpression memberExpr)
        {
            object reval;
            {
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
            }
            return reval;
        }

        private static object GetPropertyValue(MemberExpression memberExpr)
        {
            object reval;
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
    }
}
