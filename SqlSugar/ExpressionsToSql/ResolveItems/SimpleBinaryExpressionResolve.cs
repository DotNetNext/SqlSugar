using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
namespace SqlSugar
{
    public class SimpleBinaryExpressionResolve : BaseResolve
    {
        public SimpleBinaryExpressionResolve(ExpressionParameter parameter) : base(parameter)
        {
            var expression = base.Expression as  BinaryExpression;
            switch (parameter.Context.ResolveType)
            {
                case ResolveExpressType.SelectSingle:
                case ResolveExpressType.SelectMultiple:
                case ResolveExpressType.Update:

                    base.Expression = expression.Right;
                    base.Start();
                    var rightValue = parameter.CommonTempData;
                    base.Expression = expression.Left;
                    base.Start();
                    var leftValue = parameter.Context.Result.CurrentParameter.CommonTempData;
                    var operatorValue = ExpressionTool.GetOperator(expression.NodeType);
                    this.Context.Result.CurrentParameter = null;
                    this.Context.Result.AppendFormat(ExpressionConst.BinaryFormatString, leftValue, operatorValue, rightValue.ObjToString());
                    break;
                case ResolveExpressType.WhereSingle:
                case ResolveExpressType.WhereMultiple:
                case ResolveExpressType.FieldSingle:
                case ResolveExpressType.FieldMultiple:
                default:
       
                    break;
            }
        }
    }
}
