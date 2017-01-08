using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
namespace SqlSugar
{
    public class ConstantExpressionResolve : BaseResolve
    {
        public ConstantExpressionResolve(ExpressionParameter parameter) : base(parameter)
        {
            var expression = base.Expression as ConstantExpression;
            var isLeft = parameter.IsLeft;
            var isSingle = parameter.Context.IsSingle;
            object value = expression.Value;
            if (parameter.BaseParameter.BinaryExpressionInfoList != null)
            {
                parameter.BaseParameter.BinaryExpressionInfoList.Add(new KeyValuePair<string, BinaryExpressionInfo>(ExpressionConst.BinaryExpressionInfoListKey, new BinaryExpressionInfo()
                {
                    IsLeft = Convert.ToBoolean(isLeft),
                    Value = value,
                    ExpressionType = expression.GetType()
                }));
            }
            if (isLeft == null && base.Context.SqlWhere == null)
            {
                base.Context.SqlWhere = new StringBuilder();
                base.Context.SqlWhere.Append(value);
            }
        }

        //private object GetValue(MemberInfo member, Expression expression)
        //{
        //    var memberInfos = new Stack<MemberInfo>();
        //    var fieldInfo = member as System.Reflection.FieldInfo;
        //    object dynInv = null;
        //    // "descend" toward's the root object reference:
        //    while (expression is MemberExpression)
        //    {
        //        var memberExpr = expression as MemberExpression;
        //        memberInfos.Push(memberExpr.Member);
        //        if (memberExpr.Expression == null)
        //        {
        //            if (memberExpr.Member.MemberType == MemberTypes.Property)
        //            {
        //                PropertyInfo pro = (PropertyInfo)memberExpr.Member;
        //                dynInv = pro.GetValue(memberExpr.Member, null);
        //                if (dynInv != null && dynInv.GetType().IsClass)
        //                {
        //                    var fieldName = memberExpr.Member.Name;
        //                    var proInfo = dynInv.GetType().GetProperty(fieldName);
        //                    if (proInfo != null)
        //                    {
        //                        dynInv = proInfo.GetValue(dynInv, null);
        //                    }
        //                    var fieInfo = dynInv.GetType().GetField(fieldName);
        //                    if (fieInfo != null)
        //                    {
        //                        dynInv = fieInfo.GetValue(dynInv);
        //                    }
        //                    if (fieInfo == null && proInfo == null)
        //                    {

        //                    }
        //                }
        //            }
        //            else if (memberExpr.Member.MemberType == MemberTypes.Field)
        //            {
        //                FieldInfo field = (FieldInfo)memberExpr.Member;
        //                dynInv = field.GetValue(memberExpr.Member);
        //                if (dynInv != null && dynInv.GetType().IsClass && dynInv.GetType() != ExpressionConst.StringType)
        //                {
        //                    var fieldName = memberExpr.Member.Name;
        //                    var proInfo = dynInv.GetType().GetProperty(fieldName);
        //                    if (proInfo != null)
        //                    {
        //                        dynInv = proInfo.GetValue(dynInv, null);
        //                    }
        //                    var fieInfo = dynInv.GetType().GetField(fieldName);
        //                    if (fieInfo != null)
        //                    {
        //                        dynInv = fieInfo.GetValue(dynInv);
        //                    }
        //                    if (fieInfo == null && proInfo == null)
        //                    {

        //                    }
        //                }
        //            }
        //        }
        //        if (memberExpr.Expression == null)
        //        {

        //        }
        //        expression = memberExpr.Expression;
        //    }

        //    // fetch the root object reference:
        //    var constExpr = expression as ConstantExpression;
        //    if (constExpr == null)
        //    {

        //    }
        //    var objReference = constExpr.Value;

        //    // "ascend" back whence we came from and resolve object references along the way:
        //    while (memberInfos.Count > 0)  // or some other break condition
        //    {
        //        var mi = memberInfos.Pop();
        //        if (mi.MemberType == MemberTypes.Property)
        //        {
        //            var objProp = objReference.GetType().GetProperty(mi.Name);
        //            if (objProp == null)
        //            {

        //            }
        //            objReference = objProp.GetValue(objReference, null);
        //        }
        //        else if (mi.MemberType == MemberTypes.Field)
        //        {
        //            var objField = objReference.GetType().GetField(mi.Name);
        //            if (objField == null)
        //            {

        //            }
        //            objReference = objField.GetValue(objReference);
        //        }
        //    }
        //    return dynInv;
        //}
    }
}
