using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
namespace SqlSugar
{
    public class MemberExpressionResolve : BaseResolve
    {
        public MemberExpressionResolve(ExpressionParameter parameter) : base(parameter)
        {
            var expression = base.Expression as MemberExpression;
            var isLeft = parameter.IsLeft;
            var isSingle = parameter.Context.IsSingle;
            string field = string.Empty;
            field = isSingle ? expression.Member.Name : expression.Member.ToString();
            base.IsFinished = true;
            base.SqlWhere += string.Format(" {0} ", field);
            if (isLeft == true)
            {
                base.SqlWhere += ExpTool.GetOperator(parameter.BaseExpression.NodeType);
            }
            else if (isLeft == false)
            {

            }
            else
            {

            }
        }
    }
}
