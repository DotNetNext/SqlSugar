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
            var isLeft = this.IsLeft;
            this.IsLeft = null;
            var isSingle = base.Context.IsSingle;
            string field = string.Empty;
            field = isSingle ? expression.Member.Name : expression.Member.ToString();
            base.IsFinished = true;
            if (IsLeft == true)
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
