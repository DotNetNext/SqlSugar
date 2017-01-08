using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public class MemberNoExpressionResolve : BaseResolve
    {
        public MemberNoExpressionResolve(ExpressionParameter parameter) : base(parameter)
        {
            var expression = base.Expression as MemberExpression;
            var isLeft = parameter.IsLeft;
            var isSingle = parameter.Context.IsSingle;
            object value = null;
            var fieldInfo = expression.Member as System.Reflection.FieldInfo;
            var propertyInfo = expression.Member as System.Reflection.PropertyInfo;
            if (fieldInfo != null)
            {
                value = ExpressionTool.GetFiledValue(expression);
            }
            else if (propertyInfo != null)
            {
                value = ExpressionTool.GetPropertyValue(expression);
            }
            if (parameter.BaseParameter.BinaryExpressionInfoList != null)
            {
                parameter.BaseParameter.BinaryExpressionInfoList.Add(new KeyValuePair<string, BinaryExpressionInfo>(ExpressionConst.BinaryExpressionInfoListKey, new BinaryExpressionInfo()
                {
                    IsLeft = Convert.ToBoolean(isLeft),
                    Value = value,
                    ExpressionType = ExpressionConst.ConstantExpressionType
                }));
            }
            if (isLeft == null && base.Context.SqlWhere == null)
            {
                base.Context.SqlWhere = new StringBuilder();
                base.Context.SqlWhere.Append(value);
            }
        }
    }

}
