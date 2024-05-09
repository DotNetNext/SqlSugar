using System.Collections.Generic;
using System.Linq.Expressions;

namespace TranslateTool
{
    /// <summary>
    /// 处理 Lambda 参数不一致问题
    /// </summary>
    internal sealed class ParameterReplaceExpressionVisitor : ExpressionVisitor
    {
        /// <summary>
        /// 参数表达式映射集合
        /// </summary>
        private readonly Dictionary<ParameterExpression, ParameterExpression> parameterExpressionSetter;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="parameterExpressionSetter">参数表达式映射集合</param>
        public ParameterReplaceExpressionVisitor(Dictionary<ParameterExpression, ParameterExpression> parameterExpressionSetter)
        {
            this.parameterExpressionSetter = parameterExpressionSetter ?? new Dictionary<ParameterExpression, ParameterExpression>();
        }

        /// <summary>
        /// 替换表达式参数
        /// </summary>
        /// <param name="parameterExpressionSetter">参数表达式映射集合</param>
        /// <param name="expression">表达式</param>
        /// <returns>新的表达式</returns>
        public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> parameterExpressionSetter, Expression expression)
        {
            return new ParameterReplaceExpressionVisitor(parameterExpressionSetter).Visit(expression);
        }

        /// <summary>
        /// 重写基类参数访问器
        /// </summary>
        /// <param name="parameterExpression"></param>
        /// <returns></returns>
        protected override Expression VisitParameter(ParameterExpression parameterExpression)
        {
            if (parameterExpressionSetter.TryGetValue(parameterExpression, out var replacement))
            {
                parameterExpression = replacement;
            }

            return base.VisitParameter(parameterExpression);
        }
    }
}