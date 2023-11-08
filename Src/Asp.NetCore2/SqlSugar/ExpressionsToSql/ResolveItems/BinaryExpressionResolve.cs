using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace SqlSugar
{
    public class BinaryExpressionResolve : BaseResolve
    {
        public BinaryExpressionResolve(ExpressionParameter parameter) : base(parameter)
        {
            switch (parameter.Context.ResolveType)
            {
                case ResolveExpressType.FieldSingle:
                case ResolveExpressType.FieldMultiple:
                    var sql = base.GetNewExpressionValue(this.Expression);
                    this.Context.Result.Append(sql);
                    break;
                default:
                    Other(parameter);
                    break;
            }
        }

        private void Other(ExpressionParameter parameter)
        {
            var expression = this.Expression as BinaryExpression;
            if (expression.NodeType == ExpressionType.ArrayIndex)
            {
                var parameterName = AppendParameter(ExpressionTool.DynamicInvoke(expression));
                if (this.BaseParameter?.IsLeft==true)
                {
                    base.Context.Result.Append($" {BaseParameter.BaseParameter.OperatorValue} {parameterName} ");
                }
                else
                {
                    base.Context.Result.Append($" {parameterName} ");
                }
                return;
            }
            var operatorValue = parameter.OperatorValue = ExpressionTool.GetOperator(expression.NodeType);
            var isSubGroup = IsGroupSubquery(expression.Right, operatorValue);
            if (isSubGroup)
            {
                SubGroup(expression, operatorValue);
            }
            else if (IsJoinString(expression, operatorValue))
            {
                JoinString(parameter, expression);
            }
            else if (IsUpdateJson(parameter, expression, operatorValue))
            {
                parameter.CommonTempData = "IsJson=true";
                DefaultBinary(parameter, expression, operatorValue);
                parameter.CommonTempData = null;
            }
            else if (IsUpdateArray(parameter, expression, operatorValue))
            {
                parameter.CommonTempData = "IsArray=true";
                DefaultBinary(parameter, expression, operatorValue);
                parameter.CommonTempData = null;
            }
            else if (IsBinaryGroup(operatorValue,expression)) 
            {
                var isComparisonOperator = ExpressionTool.IsComparisonOperator(expression);
                var getLeft=GetNewExpressionValue(expression.Left);
                var getRight = GetNewExpressionValue(expression.Right);
                base.Context.Result.Append($"( {getLeft} {operatorValue} {getRight} )");
            }
            else
            {
                DefaultBinary(parameter, expression, operatorValue);
            }
        }

        private bool IsBinaryGroup(string operatorValue, BinaryExpression expression)
        {
            var left = ExpressionTool.RemoveConvert(expression.Left);
            var right = ExpressionTool.RemoveConvert(expression.Right);
            if (operatorValue?.IsIn("AND","OR")==true&&left is BinaryExpression&& right is BinaryExpression) 
            {

                var leftChild = ExpressionTool.RemoveConvert((left as BinaryExpression).Right);
                var rightChild = ExpressionTool.RemoveConvert((right as BinaryExpression).Right);
                var isLeftSelect = ExpressionTool.GetMethodName(leftChild) == "Select"|| leftChild is BinaryExpression;
                var isRightSelect = ExpressionTool.GetMethodName(rightChild) == "Select" || rightChild is BinaryExpression;
                var isLeftGroup = ExpressionTool.ContainsMethodName(left as BinaryExpression, "Group");
                var isRightGroup = ExpressionTool.ContainsMethodName(right as BinaryExpression, "Group");
                if ( 
                     (isLeftSelect && isLeftGroup)
                     || 
                     (isRightSelect && isRightGroup) 
                 ) 
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsUpdateArray(ExpressionParameter parameter, BinaryExpression expression, string operatorValue)
        {
            var isOk = parameter.Context.ResolveType == ResolveExpressType.WhereSingle && operatorValue == "=" && (expression.Left is MemberExpression) && expression.Left.Type.IsClass();
            if (isOk && this.Context.SugarContext != null)
            {
                var member = (expression.Left as MemberExpression);
                if (member.Expression != null)
                {
                    var entity = this.Context.SugarContext.Context.EntityMaintenance.GetEntityInfo(member.Expression.Type);
                    var jsonColumn = entity.Columns.FirstOrDefault(it => it.IsArray && it.PropertyName == ExpressionTool.GetMemberName(expression.Left));
                    if (jsonColumn != null)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool IsUpdateJson(ExpressionParameter parameter,BinaryExpression expression, string operatorValue)
        {
            var isOk= parameter.Context.ResolveType==ResolveExpressType.WhereSingle&&operatorValue == "=" && (expression.Left is MemberExpression) && expression.Left.Type.IsClass();
            if (isOk&&this.Context.SugarContext != null) 
            {
                var member = (expression.Left as MemberExpression);
                if (member.Expression != null)
                {
                    var entity = this.Context.SugarContext.Context.EntityMaintenance.GetEntityInfo(member.Expression.Type);
                    var jsonColumn = entity.Columns.FirstOrDefault(it => it.IsJson && it.PropertyName == ExpressionTool.GetMemberName(expression.Left));
                    if (jsonColumn != null)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void JoinString(ExpressionParameter parameter, BinaryExpression expression)
        {
            var leftString = GetNewExpressionValue(expression.Left);
            var RightString = GetNewExpressionValue(expression.Right);
            var joinString = this.Context.DbMehtods.MergeString(leftString, RightString);
            if (this.Context.Result.Contains(ExpressionConst.FormatSymbol))
            {
                base.Context.Result.Replace("{0}", $" {joinString} ");
                base.Context.Result.Append(" " + ExpressionConst.ExpressionReplace + parameter.BaseParameter.Index + " ");
            }
            else
            {
                base.Context.Result.Append($" {joinString} ");
            }
        }

   

        private void DefaultBinary(ExpressionParameter parameter, BinaryExpression expression, string operatorValue)
        {
            var isEqual = expression.NodeType == ExpressionType.Equal;
            var isComparisonOperator = ExpressionTool.IsComparisonOperator(expression);
            base.ExactExpression = expression;
            var leftExpression = expression.Left;
            var rightExpression = expression.Right;
            if (operatorValue == "="&& ExpressionTool.RemoveConvert(leftExpression) is ConstantExpression) 
            {
                 leftExpression = expression.Right;
                 rightExpression = expression.Left;
            }
            if (RightIsHasValue(leftExpression, rightExpression,ExpressionTool.IsLogicOperator(expression)))
            {
                Expression trueValue = Expression.Constant(true);
                rightExpression = ExpressionBuilderHelper.CreateExpression(rightExpression, trueValue, ExpressionType.Equal);
            }
            var leftIsBinary = leftExpression is BinaryExpression;
            var rightBinary = rightExpression is BinaryExpression;
            var lbrs = leftIsBinary && !rightBinary;
            var lsrb = !leftIsBinary && rightBinary;
            var lbrb = rightBinary && leftIsBinary;
            var lsbs = !leftIsBinary && !rightBinary;
            var isAppend = !base.Context.Result.Contains(ExpressionConst.FormatSymbol);
            ConvertExpression(ref leftExpression, ref rightExpression, isAppend);
            parameter.LeftExpression = leftExpression;
            parameter.RightExpression = rightExpression;
            Left(expression, leftExpression);
            Right(parameter, operatorValue, isEqual, rightExpression, lsbs);
        }

        private void SubGroup(BinaryExpression expression, string operatorValue)
        {
            if (ExpressionTool.IsUnConvertExpress(expression.Right))
            {
                InSubGroupByConvertExpress(expression);
            }
            else
            {
                InSubGroupBy(expression, operatorValue == "<>" ? "NOT" : "");
            }
        }

        private void ConvertExpression(ref Expression leftExpression, ref Expression rightExpression, bool isAppend)
        {
            if (isAppend)
            {
                base.Context.Result.Append(ExpressionConst.LeftParenthesis);
                base.Context.Result.Append(ExpressionConst.FormatSymbol);
            }
            else
            {
                base.Context.Result.Replace(ExpressionConst.FormatSymbol, ExpressionConst.LeftParenthesis + ExpressionConst.FormatSymbol);
            }
            if (leftExpression is UnaryExpression && (leftExpression as UnaryExpression).Operand is UnaryExpression && (leftExpression as UnaryExpression).NodeType == ExpressionType.Convert)
            {
                leftExpression = (leftExpression as UnaryExpression).Operand;
            }
            if (leftExpression is UnaryExpression && (leftExpression as UnaryExpression).Operand.Type == UtilConstants.BoolType && (leftExpression as UnaryExpression).NodeType == ExpressionType.Convert && rightExpression.Type == UtilConstants.BoolTypeNull)
            {
                leftExpression = (leftExpression as UnaryExpression).Operand;
            }
            if (rightExpression is UnaryExpression && (rightExpression as UnaryExpression).NodeType == ExpressionType.Convert)
            {
                rightExpression = (rightExpression as UnaryExpression).Operand;
            }
        }

        private void Right(ExpressionParameter parameter, string operatorValue, bool isEqual, Expression rightExpression, bool lsbs)
        {
            base.IsLeft = false;
            base.Expression = rightExpression;
            base.Start();
            base.IsLeft = null;
            if (lsbs && parameter.ValueIsNull)
            {
                base.Context.Result.Replace(ExpressionConst.ExpressionReplace + parameter.Index, isEqual ? "IS" : "IS NOT");
                base.Context.Result.Replace(ExpressionConst.ExpressionReplace + (parameter.Index + 1), isEqual ? "IS" : "IS NOT");
            }
            else
            {
                base.Context.Result.Replace(ExpressionConst.ExpressionReplace + parameter.Index, operatorValue);
                base.Context.Result.Replace(ExpressionConst.ExpressionReplace + (parameter.Index + 1), operatorValue);
            }
            base.Context.Result.Append(ExpressionConst.RightParenthesis);
            if (parameter.BaseExpression is BinaryExpression && parameter.IsLeft == true)
            {
                base.Context.Result.Append(" " + ExpressionConst.ExpressionReplace + parameter.BaseParameter.Index + " ");
            }
        }

        private void Left(BinaryExpression expression, Expression leftExpression)
        {
            base.Expression = leftExpression;
            base.IsLeft = true;
            base.Start();
            if (leftExpression is UnaryExpression && leftExpression.Type == UtilConstants.BoolType && !this.Context.Result.Contains(ExpressionConst.ExpressionReplace))
            {
                this.Context.Result.AppendFormat(" {0} ", ExpressionTool.GetOperator(expression.NodeType));
            }
            else if (leftExpression is UnaryExpression && ExpressionTool.RemoveConvert(leftExpression) is BinaryExpression && !this.Context.Result.Contains(ExpressionConst.ExpressionReplace))
            {
                this.Context.Result.AppendFormat(" {0} ", ExpressionTool.GetOperator(expression.NodeType));
            }
        }

        private void InSubGroupByConvertExpress(BinaryExpression expression)
        {
            var leftSql = GetNewExpressionValue(expression.Left);
            var rightExpression = (expression.Right as UnaryExpression).Operand as MethodCallExpression;
            var selector = GetNewExpressionValue(rightExpression.Arguments[0]);
            var rightSql = GetNewExpressionValue(rightExpression.Object).Replace("SELECT FROM", $"SELECT {selector} FROM");
            if (this.Context.IsSingle && this.Context.SingleTableNameSubqueryShortName == null)
            {
                var leftExp = expression.Left;
                if (leftExp is UnaryExpression)
                {
                    leftExp = (leftExp as UnaryExpression).Operand;
                }
                var p = (leftExp as MemberExpression);
                this.Context.SingleTableNameSubqueryShortName = p.Expression.ToString();
            }
            base.Context.Result.Append($" {leftSql} in ({rightSql}) ");
        }
        private void InSubGroupBy(BinaryExpression expression,string not)
        {
            var leftSql = GetNewExpressionValue(ExpressionTool.RemoveConvert(expression.Left));
            var rightExpression = expression.Right as MethodCallExpression;
            if (rightExpression.Arguments[0] is LambdaExpression) 
            {
                if ((rightExpression.Arguments[0] as LambdaExpression).Parameters?.Count > 0) 
                {
                    foreach (var item in (rightExpression.Arguments[0] as LambdaExpression).Parameters)
                    {
                        if (this.Context.InitMappingInfo != null)
                        {
                            this.Context.InitMappingInfo(item.Type);
                            this.Context.RefreshMapping();
                        }
                    }
                }
            }
            var selector = "";
            bool hasMethodCallWithName = ExpressionTool.ContainsMethodName(expression, "Join");
            if (hasMethodCallWithName)
            {
                selector = GetNewExpressionValue(rightExpression.Arguments[0], ResolveExpressType.WhereMultiple);
            }
            else
            {
                selector = GetNewExpressionValue(rightExpression.Arguments[0]);
            }
            var selectorExp = rightExpression.Arguments[0];
            if (hasMethodCallWithName==false&&selector.Contains(".") && selectorExp is LambdaExpression) 
            {
                var selectorExpLam = (selectorExp as LambdaExpression);
                var name=(selectorExpLam.Parameters[0] as ParameterExpression).Name;
                selector= selector.Replace(this.Context.GetTranslationColumnName(name)+ ".", "");
            }
            var rightSql = GetNewExpressionValue(rightExpression.Object).Replace("SELECT FROM", $"SELECT {selector} FROM");
            if (this.Context.IsSingle&&this.Context.SingleTableNameSubqueryShortName==null)
            {
                var leftExp = expression.Left;
                if (leftExp is UnaryExpression) 
                {
                    leftExp = (leftExp as UnaryExpression).Operand;
                }
                var p = (leftExp as MemberExpression);
                this.Context.SingleTableNameSubqueryShortName=p.Expression.ToString();
            }
            if (UtilMethods.IsParentheses(rightSql+""))
            {
                base.Context.Result.Append($" {leftSql} {not} in {rightSql} ");
            }
            else
            {
                base.Context.Result.Append($" {leftSql} {not} in ({rightSql}) ");
            }
        }

        private bool IsGroupSubquery(Expression rightExpression, string operatorValue)
        {
            if (operatorValue != "="&& operatorValue != "<>")
            {
                return false;
            }
            if (rightExpression == null)
            {
                return false;
            }
            if (ExpressionTool.IsUnConvertExpress(rightExpression))
            {
                rightExpression = (rightExpression as UnaryExpression).Operand;
            }
            if ((rightExpression is MethodCallExpression) == false)
            {
                return false;
            }
            var method = (rightExpression as MethodCallExpression);
            if (method.Method.Name != "Select")
            {
                return false;
            }
            var topMethods =ExpressionTool.GetTopLevelMethodCalls(method);
            if (!topMethods.Contains("Subqueryable")) 
            {
                return false;
            }
            if (!topMethods.Contains("GroupBy"))
            {
                return false;
            } 
            return true;
        }
        private static bool IsJoinString(BinaryExpression expression, string operatorValue)
        {
            return operatorValue == "+" 
                && expression.Right.Type == UtilConstants.StringType
                && expression.Left.Type==UtilConstants.StringType;
        }
        private static bool RightIsHasValue(Expression leftExpression, Expression rightExpression,bool isLogic)
        {
            return isLogic&&
                leftExpression.Type == UtilConstants.BoolType && 
                rightExpression.Type == UtilConstants.BoolType && 
                rightExpression is MethodCallExpression && 
                (rightExpression as MethodCallExpression).Method.Name == "HasValue";
        }

    }
}
