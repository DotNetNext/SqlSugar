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
            string customParameter = GetCustomParameter(parameter, expression);
            if (customParameter == null)
            {
                DefaultConstant(parameter, expression);
            }
            else
            {
                CustomConstant(parameter, customParameter);
            }
        }

        private void CustomConstant(ExpressionParameter parameter, string customParameter)
        {
            var baseParameter = parameter.BaseParameter;
            var isSetTempData = baseParameter.CommonTempData.HasValue() && baseParameter.CommonTempData.Equals(CommonTempDataType.Result);
            if (isSetTempData)
            {
                baseParameter.CommonTempData = customParameter;
            }
            else
            {
                AppendMember(parameter, parameter.IsLeft, customParameter);
            }
        }

        private string GetCustomParameter(ExpressionParameter parameter, ConstantExpression expression)
        {
            string customParameter = null;
            if (parameter.OppsiteExpression != null)
            {
                var exp = ExpressionTool.RemoveConvert(parameter.OppsiteExpression);
                if (exp is MemberExpression)
                {
                    var member = (exp as MemberExpression);
                    var memberParent = member.Expression;
                    if (memberParent != null&& this.Context?.SugarContext?.Context!=null)
                    {
                        var entity = this.Context.SugarContext.Context.EntityMaintenance.GetEntityInfo(memberParent.Type);
                        var columnInfo = entity.Columns.FirstOrDefault(it => it.PropertyName == member.Member.Name);
                        if (columnInfo?.SqlParameterDbType is Type)
                        {
                            var type = columnInfo.SqlParameterDbType as Type;
                            var ParameterConverter = type.GetMethod("ParameterConverter").MakeGenericMethod(columnInfo.PropertyInfo.PropertyType);
                            var obj=Activator.CreateInstance(type);
                            var p = ParameterConverter.Invoke(obj, new object[] { expression.Value, 100 }) as SugarParameter;
                            customParameter = base.AppendParameter(p);

                        }
                    }
                }
            }
            return customParameter;
        }

        private void DefaultConstant(ExpressionParameter parameter, ConstantExpression expression)
        {
            var isLeft = parameter.IsLeft;
            object value = ExpressionTool.GetValue(expression.Value, this.Context);
            value = ConvetValue(parameter, expression, value);
            if (IsEnumString(value))
                value = ConvertEnum(value);
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

        private object ConvetValue(ExpressionParameter parameter, ConstantExpression expression, object value)
        {
            if (expression.Type == UtilConstants.IntType && parameter.OppsiteExpression != null &&ExpressionTool.IsUnConvertExpress(parameter.OppsiteExpression))
            {
                var exp = ExpressionTool.RemoveConvert(parameter.OppsiteExpression);
                if (exp.Type == typeof(char)&& value is int) {
                    value = Convert.ToChar(Convert.ToInt32(value));
                }
            }

            return value;
        }

        private object ConvertEnum(object value)
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

            return value;
        }

        private bool IsEnumString(object value)
        {
            return this.Context.TableEnumIsString == true
                                   && value != null
                                     && value.IsInt()
                                      && base.BaseParameter?.OppsiteExpression != null;
        }
    }
}
