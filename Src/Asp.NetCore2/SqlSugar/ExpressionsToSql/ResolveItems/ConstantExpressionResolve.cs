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
            object value = ExpressionTool.GetValue(expression.Value,this.Context);
            if (this.Context.TableEnumIsString == true
                       && value != null
                         && value.IsInt()
                          && base.BaseParameter?.OppsiteExpression != null)
            {
                if (base.BaseParameter?.OppsiteExpression is UnaryExpression)
                {
                    var oppsiteExpression = base.BaseParameter?.OppsiteExpression as UnaryExpression;
                    var oppsiteValue = oppsiteExpression.Operand;
                    if (oppsiteValue.Type.IsEnum())
                    {
                        value = UtilMethods.ChangeType2(value, oppsiteValue.Type).ToString();
                    }
                }
            }
            var baseParameter = parameter.BaseParameter;
            baseParameter.ChildExpression = expression;
            var isSetTempData = baseParameter.CommonTempData.HasValue() && baseParameter.CommonTempData.Equals(CommonTempDataType.Result);
            switch (parameter.Context.ResolveType)
            {
                case ResolveExpressType.SelectSingle:
                case ResolveExpressType.Update:
                case ResolveExpressType.SelectMultiple:
                    baseParameter.CommonTempData = value;
                    break;
                case ResolveExpressType.WhereSingle:
                case ResolveExpressType.WhereMultiple:
                    if (isSetTempData)
                    {
                        baseParameter.CommonTempData = value;
                    }
                    else
                    {
                        var parentIsBinary = parameter.BaseParameter.CurrentExpression is BinaryExpression;
                        var parentIsRoot = parameter.BaseParameter.CurrentExpression is LambdaExpression;
                        var isBool = value != null && value.GetType() == UtilConstants.BoolType;
                        if (parentIsRoot && isBool)
                        {
                            this.Context.Result.Append(value.ObjToBool() ? this.Context.DbMehtods.True() : this.Context.DbMehtods.False());
                            break;
                        }
                        if (parentIsBinary && isBool)
                        {
                            var isLogicOperator =
                               parameter.BaseExpression.NodeType == ExpressionType.And ||
                               parameter.BaseExpression.NodeType == ExpressionType.AndAlso ||
                               parameter.BaseExpression.NodeType == ExpressionType.Or ||
                               parameter.BaseExpression.NodeType == ExpressionType.OrElse;
                            if (isLogicOperator)
                            {
                                AppendMember(parameter, isLeft, (value.ObjToBool() ? this.Context.DbMehtods.True() : this.Context.DbMehtods.False()));
                                break;
                            }
                        }
                        if (value == null && parentIsBinary)
                        {
                            parameter.BaseParameter.ValueIsNull = true;
                            value = this.Context.DbMehtods.Null();
                        }
                        AppendValue(parameter, isLeft, value);
                    }
                    break;
                case ResolveExpressType.FieldSingle:
                case ResolveExpressType.FieldMultiple:
                default:
                    break;
            }
        }
    }
}
