using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
namespace SqlSugar
{
    public class MemberNewExpressionResolve : BaseResolve
    {
        public MemberNewExpressionResolve(ExpressionParameter parameter) : base(parameter)
        {
            var expression = base.Expression as MemberExpression;
            var isLeft = parameter.IsLeft;
            object value = null;
            value = ExpressionTool.DynamicInvoke(expression);
            if (parameter.BaseParameter.BinaryTempData != null)
            {
                parameter.BaseParameter.BinaryTempData.Add(new KeyValuePair<string, BinaryExpressionInfo>(ExpressionConst.BinaryExpressionInfoListKey, new BinaryExpressionInfo()
                {
                    IsLeft = Convert.ToBoolean(isLeft),
                    Value = value,
                    ExpressionType = ExpressionConst.ConstantExpressionType
                }));
            }
        }
    }
}
