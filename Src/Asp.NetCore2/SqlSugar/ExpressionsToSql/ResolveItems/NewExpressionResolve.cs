﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public class NewExpressionResolve : BaseResolve
    {
        public NewExpressionResolve(ExpressionParameter parameter) : base(parameter)
        {
            var expression = base.Expression as NewExpression;
            if (expression.Type.IsIn(UtilConstants.DateType,UtilConstants.GuidType))
            {
                NewValueType(parameter, expression);
                return;
            }
            switch (parameter.Context.ResolveType)
            {
                case ResolveExpressType.WhereSingle:
                    Check.ThrowNotSupportedException(expression.ToString());
                    break;
                case ResolveExpressType.WhereMultiple:
                    Check.ThrowNotSupportedException(expression.ToString());
                    break;
                case ResolveExpressType.SelectSingle:
                    Check.Exception(expression.Type == UtilConstants.DateType, "ThrowNotSupportedException {0} ", expression.ToString());
                    Select(expression, parameter, true);
                    break;
                case ResolveExpressType.SelectMultiple:
                    Check.Exception(expression.Type == UtilConstants.DateType, "ThrowNotSupportedException {0} ", expression.ToString());
                    Select(expression, parameter, false);
                    break;
                case ResolveExpressType.FieldSingle:
                    Check.ThrowNotSupportedException(expression.ToString());
                    break;
                case ResolveExpressType.FieldMultiple:
                case ResolveExpressType.ArrayMultiple:
                case ResolveExpressType.ArraySingle:
                    ArraySingle(expression);
                    break;
                case ResolveExpressType.Join:
                    Join(expression);
                    break;
                default:
                    break;
            }
        }

        private void Join(NewExpression expression)
        {
            base.Context.ResolveType = ResolveExpressType.WhereMultiple;
            int i = 0;
            foreach (var item in expression.Arguments)
            {
                if (item.Type != typeof(JoinType))
                {
                    base.Expression = item;
                    base.Start();
                }
                if (item.Type == typeof(JoinType))
                {
                    var joinValue = item.ObjToString();
                    if (joinValue.Contains("("))
                    {
                        joinValue = ExpressionTool.DynamicInvoke(item).ObjToString();
                    }
                    if (i > 0)
                    {
                        base.Context.Result.Append("," + joinValue + ",");
                    }
                    else
                    {
                        base.Context.Result.Append(joinValue + ",");
                    }
                    ++i;
                }
            }
        }

        private void ArraySingle(NewExpression expression)
        {
            foreach (var item in expression.Arguments)
            {
                if (IsDateValue(item))
                {
                    var value = GetNewExpressionValue(item);
                    base.Context.Result.Append(value);
                }
                else
                {
                    base.Expression = item;
                    base.Start();
                }
            }
        }

        private bool IsDateValue(Expression item)
        {
            var isMember = item is MemberExpression;
            if (isMember) 
            {
                var m = (item as MemberExpression);
                var isInt= m.Type == UtilConstants.IntType;
                if (m.Expression != null && isInt&& m.Expression is MemberExpression)
                {
                    var mm = (m.Expression as MemberExpression);
                    if (m.Member.Name.IsIn("Year", "Day", "Month")&&mm.Type==UtilConstants.DateType) 
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void NewValueType(ExpressionParameter parameter, NewExpression expression)
        {
            try
            {
                var value = ExpressionTool.DynamicInvoke(expression);
                var isSetTempData = parameter.CommonTempData.HasValue() && parameter.CommonTempData.Equals(CommonTempDataType.Result);
                if (isSetTempData)
                {
                    parameter.CommonTempData = value;
                }
                else
                {
                    AppendValue(parameter, parameter.IsLeft, value);
                }
            }
            catch (Exception ex)
            {
                Check.Exception(expression.Type == UtilConstants.DateType, "ThrowNotSupportedException {0} ", ex.ToString());
            }
        }

        private void Select(NewExpression expression, ExpressionParameter parameter, bool isSingle)
        {
            if (expression.Arguments != null)
            {
                int i = 0;
                foreach (var item in expression.Arguments)
                {

                    string memberName = expression.Members?[i]?.Name;
                    if (memberName == null&& expression.Members ==null && item is MemberExpression member) 
                    {
                        memberName = member.Member.Name;
                        this.Context.SugarContext.QueryBuilder.IsParameterizedConstructor = true;
                    }
                    if (this.Context?.SugarContext?.QueryBuilder?.AppendNavInfo?.MappingNavProperties?.ContainsKey(memberName) == true)
                    {
                        ++i;
                        continue;
                    }
                    ++i;
                    ResolveNewExpressions(parameter, item, memberName);
                }
            }
        }
    }
}

