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
            parameter.BinaryExpressionInfoList =new List<KeyValuePair<string, BinaryExpressionInfo>>();
            var expression = this.Expression as BinaryExpression;
            var operatorValue = ExpTool.GetOperator(expression.NodeType);
            var isComparisonOperator =
                                        expression.NodeType != ExpressionType.And &&
                                        expression.NodeType != ExpressionType.AndAlso &&
                                        expression.NodeType != ExpressionType.Or &&
                                        expression.NodeType != ExpressionType.OrElse;
            base.BaseExpression = expression;
            base.IsLeft = true;
            base.Expression = expression.Left;
            base.Start();
            base.IsLeft = false;
            base.Expression = expression.Right;
            base.Start();
            base.IsLeft = null;
            string leftString = GetLeftString(parameter);
            string rightString = GetRightString(parameter);
            string binarySql =string.Format(ExpConst.BinaryFormatString,leftString,operatorValue,rightString);
            string sqlWhereString = base.SqlWhere.ToString();
            if (base.SqlWhere == null) {
                base.SqlWhere = new StringBuilder();
            }
            if (sqlWhereString.Contains(ExpConst.Format0))
            {
                base.SqlWhere.Replace(ExpConst.Format0, sqlWhereString);
            }
            else
            {
                base.SqlWhere.Append(binarySql);
            }
            if (sqlWhereString.Contains(ExpConst.Format1))
            {
                base.SqlWhere.Replace(ExpConst.Format1, ExpConst.Format0);
            }
        }

        private string GetRightString(ExpressionParameter parameter)
        {
            var info=parameter.BinaryExpressionInfoList.Single(it => it.Value.IsLeft).Value;
            return info.Value.ObjToString();
        }

        private string GetLeftString(ExpressionParameter parameter)
        {
            var info = parameter.BinaryExpressionInfoList.Single(it => !it.Value.IsLeft).Value;
            return info.Value.ObjToString();
        }
    }
}
