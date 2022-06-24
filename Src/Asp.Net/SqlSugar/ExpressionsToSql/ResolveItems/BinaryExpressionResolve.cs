using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
            var operatorValue = parameter.OperatorValue = ExpressionTool.GetOperator(expression.NodeType);

            if (IsGroupSubquery(expression.Right,operatorValue))
            {
                if (ExpressionTool.IsUnConvertExpress(expression.Right))
                {
                    InSubGroupByConvertExpress(expression);
                }
                else
                {
                    InSubGroupBy(expression, operatorValue=="<>"?"NOT":"");
                }
                return;
            }

            var isEqual = expression.NodeType == ExpressionType.Equal;
            var isComparisonOperator = ExpressionTool.IsComparisonOperator(expression);
            base.ExactExpression = expression;
            var leftExpression = expression.Left;
            var rightExpression = expression.Right;
            var leftIsBinary = leftExpression is BinaryExpression;
            var rightBinary = rightExpression is BinaryExpression;
            var lbrs = leftIsBinary && !rightBinary;
            var lsrb = !leftIsBinary && rightBinary;
            var lbrb = rightBinary && leftIsBinary;
            var lsbs = !leftIsBinary && !rightBinary;
            var isAppend = !base.Context.Result.Contains(ExpressionConst.FormatSymbol);
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
            if (rightExpression is UnaryExpression&& (rightExpression as UnaryExpression).NodeType == ExpressionType.Convert)
            {
                rightExpression = (rightExpression as UnaryExpression).Operand;
            }
            parameter.LeftExpression = leftExpression;
            parameter.RightExpression = rightExpression;
            base.Expression = leftExpression;
            base.IsLeft = true;
            base.Start();
            if (leftExpression is UnaryExpression && leftExpression.Type == UtilConstants.BoolType && !this.Context.Result.Contains(ExpressionConst.ExpressionReplace))
            {
                this.Context.Result.AppendFormat(" {0} ", ExpressionTool.GetOperator(expression.NodeType));
            }
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
            var leftSql = GetNewExpressionValue(expression.Left);
            var rightExpression = expression.Right as MethodCallExpression;
            var selector = GetNewExpressionValue(rightExpression.Arguments[0]);
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
            base.Context.Result.Append($" {leftSql} {not} in ({rightSql}) ");
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
            var methodString = method.ToString();
            if (methodString.IndexOf("GroupBy(")<=0) 
            {
                return false;
            }
            if (Regex.Matches(methodString, @"Subqueryable\(").Count!=1)
            {
                return false;
            }
            return true;
        }
    }
}
