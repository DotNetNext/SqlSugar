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
        }
    }
}
