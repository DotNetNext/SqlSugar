using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
namespace SqlSugar
{
    public class UnaryExpressionResolve : BaseResolve
    {
        public UnaryExpressionResolve(ExpressionParameter parameter) : base(parameter)
        {
            var oldExpression = base.Expression;
            var expression = base.Expression as UnaryExpression;
            var baseParameter = parameter.BaseParameter;
            switch (this.Context.ResolveType)
            {
                case ResolveExpressType.WhereSingle:
                case ResolveExpressType.WhereMultiple:
                case ResolveExpressType.FieldSingle:
                case ResolveExpressType.FieldMultiple:
                case ResolveExpressType.SelectSingle:
                case ResolveExpressType.SelectMultiple:
                case ResolveExpressType.ArraySingle:
                case ResolveExpressType.ArrayMultiple:
                case ResolveExpressType.Update:
                    var nodeType = expression.NodeType;
                    base.Expression = expression.Operand;
                    var isMember = expression.Operand is MemberExpression;
                    var isConst = expression.Operand is ConstantExpression;
                    var offsetIsNull = (parameter.OppsiteExpression is ConstantExpression)
                                        &&(parameter.OppsiteExpression as ConstantExpression).Value==null
                                        &&ExpressionTool.IsComparisonOperator(expression.Operand);
                    if (isMember && offsetIsNull) {
                        Append(parameter, nodeType);
                    }
                    else if (baseParameter.CurrentExpression is NewArrayExpression)
                    {
                        Result(parameter, nodeType);
                    }
                    else if (baseParameter.OperatorValue == "=" &&IsNotMember(oldExpression))
                    {
                        AppendNotMember(parameter,nodeType);
                    }
                    else if (baseParameter.OperatorValue == "=" && IsNotParameter(oldExpression))
                    {
                        AppendNotParameter(parameter, nodeType);
                    }
                    else if (base.Expression is BinaryExpression || parameter.BaseExpression is BinaryExpression || baseParameter.CommonTempData.ObjToString() == CommonTempDataType.Append.ToString())
                    {
                        Append(parameter, nodeType);
                    }
                    else if (isMember)
                    {
                        MemberLogic(parameter, baseParameter, nodeType);
                    }
                    else if (isConst)
                    {
                        Result(parameter, nodeType);
                    }
                    else
                    {
                        Append(parameter, nodeType);
                    }
                    break;
                default:
                    break;
            }
        }

        private void MemberLogic(ExpressionParameter parameter, ExpressionParameter baseParameter, ExpressionType nodeType)
        {
            var memberExpression = (base.Expression as MemberExpression);
            var isLogicOperator = ExpressionTool.IsLogicOperator(baseParameter.OperatorValue) || baseParameter.OperatorValue.IsNullOrEmpty();
            var isHasValue = isLogicOperator && memberExpression.Member.Name == "HasValue" && memberExpression.Expression != null && memberExpression.NodeType == ExpressionType.MemberAccess;
            if (isHasValue)
            {
                var member = memberExpression.Expression as MemberExpression;
                parameter.CommonTempData = CommonTempDataType.Result;
                var isConst = member.Expression != null && member.Expression is ConstantExpression;
                if (isConst)
                {
                    var paramterValue = ExpressionTool.DynamicInvoke(member);
                    var paramterName= base.AppendParameter(paramterValue);
                    var result = this.Context.DbMehtods.HasValue(new MethodCallExpressionModel()
                    {
                        Args = new List<MethodCallExpressionArgs>() {
                        new MethodCallExpressionArgs() { IsMember=false, MemberName=paramterName, MemberValue=paramterValue } }
                    });
                    this.Context.Result.Append(result);
                }
                else
                {
                    this.Expression = isConst ? member.Expression : member;
                    this.Start();
                    var methodParamter = isConst ? new MethodCallExpressionArgs() { IsMember = false } : new MethodCallExpressionArgs() { IsMember = true, MemberName = parameter.CommonTempData, MemberValue = null };
                    var result = this.Context.DbMehtods.HasValue(new MethodCallExpressionModel()
                    {
                        Args = new List<MethodCallExpressionArgs>() {
                      methodParamter
                  }
                    });
                    this.Context.Result.Append(result);
                    parameter.CommonTempData = null;
                }
            }
            else if (memberExpression.Type == UtilConstants.BoolType && isLogicOperator)
            {
                Append(parameter, nodeType);
            }
            else
            {
                Result(parameter, nodeType);
            }
        }

        private void Result(ExpressionParameter parameter, ExpressionType nodeType)
        {
            BaseParameter.ChildExpression = base.Expression;
            parameter.CommonTempData = CommonTempDataType.Result;
            if (nodeType == ExpressionType.Not)
                AppendNot(parameter.CommonTempData);
            base.Start();
            parameter.BaseParameter.CommonTempData = parameter.CommonTempData;
            parameter.BaseParameter.ChildExpression = base.Expression;
            parameter.CommonTempData = null;
        }

        private void Append(ExpressionParameter parameter, ExpressionType nodeType)
        {
            BaseParameter.ChildExpression = base.Expression;
            this.IsLeft = parameter.IsLeft;
            parameter.CommonTempData = CommonTempDataType.Append;
            if (nodeType == ExpressionType.Not)
                AppendNot(parameter.CommonTempData);
            if (nodeType == ExpressionType.Negate)
                AppendNegate(parameter.CommonTempData);
            base.Start();
            parameter.BaseParameter.CommonTempData = parameter.CommonTempData;
            parameter.BaseParameter.ChildExpression = base.Expression;
            parameter.CommonTempData = null;
        }


        private void AppendNotMember(ExpressionParameter parameter, ExpressionType nodeType)
        {
            BaseParameter.ChildExpression = base.Expression;
            this.IsLeft = parameter.IsLeft;
            parameter.CommonTempData = CommonTempDataType.Result;
            base.Start();
            var result= this.Context.DbMehtods.IIF(new MethodCallExpressionModel()
            {
                Args = new List<MethodCallExpressionArgs>() {
                                  new MethodCallExpressionArgs(){ IsMember=true, MemberName=parameter.CommonTempData.ObjToString()+"=1" },
                                  new MethodCallExpressionArgs(){ IsMember=true,MemberName=AppendParameter(0)  },
                                  new MethodCallExpressionArgs(){ IsMember=true, MemberName=AppendParameter(1)  }
                           }
            });
            this.Context.Result.Append(result);
            parameter.BaseParameter.ChildExpression = base.Expression;
            parameter.CommonTempData = null;
        }


        private void AppendNotParameter(ExpressionParameter parameter, ExpressionType nodeType)
        {
            BaseParameter.ChildExpression = base.Expression;
            this.IsLeft = parameter.IsLeft;
            parameter.CommonTempData = CommonTempDataType.Result;
            base.Start();
            var result = this.Context.DbMehtods.IIF(new MethodCallExpressionModel()
            {
                Args = new List<MethodCallExpressionArgs>() {
                                  new MethodCallExpressionArgs(){ IsMember=true, MemberName=AppendParameter(parameter.CommonTempData)+"=1" },
                                  new MethodCallExpressionArgs(){ IsMember=true,MemberName=AppendParameter(0)  },
                                  new MethodCallExpressionArgs(){ IsMember=true, MemberName=AppendParameter(1)  }
                           }
            });
            this.Context.Result.Append(result);
            parameter.BaseParameter.ChildExpression = base.Expression;
            parameter.CommonTempData = null;
        }

    }
}
