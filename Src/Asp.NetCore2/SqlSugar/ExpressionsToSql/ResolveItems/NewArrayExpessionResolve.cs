using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public class NewArrayExpessionResolve : BaseResolve
    {
        public NewArrayExpessionResolve(ExpressionParameter parameter) : base(parameter)
        {
            var expression = (NewArrayExpression)base.Expression;
            switch (base.Context.ResolveType)
            {
                case ResolveExpressType.WhereSingle:
                case ResolveExpressType.WhereMultiple:
                case ResolveExpressType.SelectSingle:
                case ResolveExpressType.SelectMultiple:
                case ResolveExpressType.FieldSingle:
                case ResolveExpressType.FieldMultiple:
                    try
                    {
                       var value = ExpressionTool.DynamicInvoke(expression);
                        var isLeft = parameter.IsLeft;
                        var baseParameter = parameter.BaseParameter;
                        var isSetTempData = baseParameter.CommonTempData.HasValue() && baseParameter.CommonTempData.Equals(CommonTempDataType.Result);
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
                    }
                    catch (Exception)
                    {
                        Check.ThrowNotSupportedException("NewArrayExpression");
                    }
                    break;
                case ResolveExpressType.ArraySingle:
                    foreach (var item in expression.Expressions)
                    {
                        base.Expression = item;
                        base.Start();
                    }
                    break;
                case ResolveExpressType.Join:
                    base.Context.ResolveType = ResolveExpressType.WhereMultiple;
                    int i = 0;
                    foreach (var item in expression.Expressions)
                    {
                        if (item is UnaryExpression)
                        {
                            base.Expression = item;
                            base.Start();
                            if (parameter.CommonTempData is JoinType)
                            {
                                if (i > 0)
                                {
                                    base.Context.Result.Append("," + parameter.CommonTempData.ObjToString().Replace(",",UtilConstants.ReplaceCommaKey) + ",");
                                }
                                else
                                {
                                    base.Context.Result.Append(parameter.CommonTempData.ObjToString().Replace(",", UtilConstants.ReplaceCommaKey) + ",");
                                }
                                ++i;
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
