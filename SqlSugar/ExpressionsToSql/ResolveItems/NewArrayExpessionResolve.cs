using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public class NewArrayExpessionResolve : BaseResolve
    {
        public NewArrayExpessionResolve(ExpressionParameter parameter) : base(parameter)
        {
            var expression = (NewArrayExpression)base.Expression;
            switch (base.Context.ResolveType)
            {
                case ResolveExpressType.WhereSingle:
                case ResolveExpressType.WhereMultiple:
                case ResolveExpressType.SelectSingle:
                case ResolveExpressType.SelectMultiple:
                case ResolveExpressType.FieldSingle:
                case ResolveExpressType.FieldMultiple:
                    Check.ThrowNotSupportedException("NewArrayExpression");
                    break;
                case ResolveExpressType.Join:
                    base.Context.ResolveType = ResolveExpressType.WhereMultiple;
                    foreach (var item in expression.Expressions)
                    {
                        var value = ((ConstantExpression)item).Value;
                        var isJoinType = value is JoinType;
                        //string value = "";
                        //base.AppendValue();
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
