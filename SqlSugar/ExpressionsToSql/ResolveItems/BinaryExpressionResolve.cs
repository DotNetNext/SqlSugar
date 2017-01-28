using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
namespace SqlSugar
{
    public class BinaryExpressionResolve : BaseResolve
    {
        public BinaryExpressionResolve(ExpressionParameter parameter) : base(parameter)
        {
            if (parameter.BaseParameter.CommonTempData != null && parameter.BaseParameter.CommonTempData.Equals("simple"))
            {
                parameter.BaseParameter = parameter;
                new SimpleBinaryExpressionResolve(parameter);
                this.Context.Result.CurrentParameter = null;
            }
            else
            {
                parameter.BinaryTempData = new List<KeyValuePair<string, BinaryExpressionInfo>>();
                var expression = this.Expression as BinaryExpression;
                var operatorValue = ExpressionTool.GetOperator(expression.NodeType);
                var isComparisonOperator =
                                            expression.NodeType != ExpressionType.And &&
                                            expression.NodeType != ExpressionType.AndAlso &&
                                            expression.NodeType != ExpressionType.Or &&
                                            expression.NodeType != ExpressionType.OrElse;
                base.BaseExpression = expression;
                var leftExpression = expression.Left;
                var rightExpression = expression.Right;
                var leftIsBinary = leftExpression is BinaryExpression;
                var rightBinary = rightExpression is BinaryExpression;
                int i = 0;
                var lbrs = leftIsBinary && !rightBinary;
                var lsrb = !leftIsBinary && rightBinary;
                var lbrb = rightBinary && leftIsBinary;
                var lsbs = !leftIsBinary && !rightBinary;
                if (lbrs)
                {
                    base.Context.Result.Append("{" + i + "}");
                    ++i;
                }
                else if (lsrb)
                {
                    base.Context.Result.Append("{" + i + "}");
                }
                else if (lbrb)
                {
                    base.Context.Result.Append("{0}");
                    base.Context.Result.Append("{2}");
                    base.Context.Result.Append("{1}");
                }
                base.Expression = leftExpression;
                base.IsLeft = true;
                base.Start();
                base.IsLeft = false;
                base.Expression = rightExpression;
                base.Start();
                base.IsLeft = null;
                string leftString = null;
                if (!leftIsBinary)
                    leftString = GetLeftString(parameter);
                string rightString = null;
                if (!rightBinary)
                    rightString = GetRightString(parameter);
                string binarySql = null;
                if (lsbs)
                {
                    binarySql = string.Format(ExpressionConst.BinaryFormatString, leftString, operatorValue, rightString);
                }
                else if (lbrb)
                {
                    binarySql = operatorValue;
                }
                if (Context.Result.Contains(ExpressionConst.Format0))
                {
                    base.Context.Result.Replace(ExpressionConst.Format0, binarySql);
                }
                else
                {
                    base.Context.Result.Append(binarySql);
                }
                if (Context.Result.Contains(ExpressionConst.Format1))
                {
                    base.Context.Result.Replace(ExpressionConst.Format1, ExpressionConst.Format0);
                    base.Context.Result.Replace(ExpressionConst.Format2, ExpressionConst.Format1);
                }
            }
        }

        private string GetRightString(ExpressionParameter parameter)
        {
            var leftInfo = parameter.BinaryTempData.Single(it => it.Value.IsLeft).Value;
            var rightInfo = parameter.BinaryTempData.Single(it => !it.Value.IsLeft).Value;
            if (rightInfo.ExpressionType == ExpressionConst.ConstantExpressionType)
            {
                var sqlParameterKeyWord = parameter.Context.SqlParameterKeyWord;
                var reval = string.Format("{0}{1}{2}", sqlParameterKeyWord, leftInfo.Value, parameter.Context.Index + parameter.Index);
                if (parameter.Context.Parameters == null)
                {
                    parameter.Context.Parameters = new List<SugarParameter>();
                }
                parameter.Context.Parameters.Add(new SugarParameter(reval, rightInfo.Value));
                return reval;
            }
            return rightInfo.Value.ObjToString();
        }

        private string GetLeftString(ExpressionParameter parameter)
        {
            var leftInfo = parameter.BinaryTempData.Single(it => it.Value.IsLeft).Value;
            var rightInfo = parameter.BinaryTempData.Single(it => !it.Value.IsLeft).Value;
            if (leftInfo.ExpressionType == ExpressionConst.ConstantExpressionType)
            {
                var sqlParameterKeyWord = parameter.Context.SqlParameterKeyWord;
                var reval = string.Format("{0}{1}{2}", sqlParameterKeyWord, leftInfo.Value, parameter.Context.Index + parameter.Index);
                parameter.Context.Parameters.Add(new SugarParameter(reval, leftInfo.Value));
                return reval;
            }
            return leftInfo.Value.ObjToString();
        }
    }
}
