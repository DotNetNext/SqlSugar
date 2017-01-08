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
            var operatorValue = ExpressionTool.GetOperator(expression.NodeType);
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
            string binarySql =string.Format(ExpressionConst.BinaryFormatString,leftString,operatorValue,rightString);
            string sqlWhereString = base.SqlWhere.ToString();
            if (base.SqlWhere == null) {
                base.SqlWhere = new StringBuilder();
            }
            if (sqlWhereString.Contains(ExpressionConst.Format0))
            {
                base.SqlWhere.Replace(ExpressionConst.Format0, sqlWhereString);
            }
            else
            {
                base.SqlWhere.Append(binarySql);
            }
            if (sqlWhereString.Contains(ExpressionConst.Format1))
            {
                base.SqlWhere.Replace(ExpressionConst.Format1, ExpressionConst.Format0);
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
