using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace SqlSugar
{
    /// <summary>
    ///  性能稍差作废了
    /// </summary>
    public class ResovleExpress_Old
    {
        /// <summary>
        /// 根据Expression获取SQL WHERE 字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static string GetWhereByExpression<T>(Expression<Func<T, bool>> expression)
        {
            string whereStr = string.Empty;
            if (expression.Body is BinaryExpression)
            {
                BinaryExpression be = ((BinaryExpression)expression.Body);
                whereStr = " and " + BinarExpressionProvider(be.Left, be.Right, be.NodeType);
            }
            else
            {
                whereStr = " and " + ExpressionRouter(expression.Body, false);
            }
            return whereStr;
        }
        /// <summary>
        /// 据Expression获取SQL WHERE 字符串
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string BinarExpressionProvider(Expression left, Expression right, ExpressionType type)
        {
            string sb = "(";
            //先处理左边
            sb += ExpressionRouter(left, false);
            sb += ExpressionTypeCast(type);
            //再处理右边
            string tmpStr = ExpressionRouter(right, true);
            if (tmpStr == "null")
            {
                if (sb.EndsWith(" ="))
                    sb = sb.Substring(0, sb.Length - 2) + " is null";
                else if (sb.EndsWith("<>"))
                    sb = sb.Substring(0, sb.Length - 2) + " is not null";
            }
            else
                sb += tmpStr;
            return sb += ")";
        }
        /// <summary>
        /// 表达式路由计算
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="isNot"></param>
        /// <param name="isSingleQuotation"></param>
        /// <returns></returns>
        protected static string ExpressionRouter(Expression exp, bool isNot = false, bool isSingleQuotation = true)
        {

            string sb = string.Empty;
            if (exp is BinaryExpression)
            {
                BinaryExpression be = ((BinaryExpression)exp);
                var left = BinarExpressionProvider(be.Left, be.Right, be.NodeType);
                return left;

            }
            else if (exp is MemberExpression)
            {
                MemberExpression me = ((MemberExpression)exp);
                if (me.Expression == null || me.Expression.NodeType.ToString() != "Parameter")
                {
                    if (isSingleQuotation)
                    {
                        return (Expression.Lambda(exp).Compile().DynamicInvoke().ToString()).ToSqlValue();
                    }
                    else
                    {
                        return Expression.Lambda(exp).Compile().DynamicInvoke().ToString().ToSqlFilter();
                    }
                }
                else
                {
                    return me.Member.Name;
                }

            }
            else if (exp is NewArrayExpression)
            {
                NewArrayExpression ae = ((NewArrayExpression)exp);
                StringBuilder tmpstr = new StringBuilder();
                foreach (Expression ex in ae.Expressions)
                {
                    tmpstr.Append(ExpressionRouter(ex, false, isSingleQuotation));
                    tmpstr.Append(",");
                }
                return tmpstr.ToString(0, tmpstr.Length - 1);
            }
            else if (exp is MethodCallExpression)
            {
                MethodCallExpression mce = (MethodCallExpression)exp;
                string methodName = mce.Method.Name;
                if (methodName == "Contains")
                {
                    return string.Format("({0} {2} LIKE '%{1}%')", ExpressionRouter((mce.Object as MemberExpression), false), ExpressionRouter(mce.Arguments[0], false, false), isNot == true ? "  NOT " : null);
                }
                else if (methodName == "StartsWith")
                {
                    return string.Format("({0} {2} LIKE '{1}%')", ExpressionRouter((mce.Object as MemberExpression), false), ExpressionRouter(mce.Arguments[0], false, false), isNot == true ? "  NOT " : null);
                }
                else if (methodName == "EndWith")
                {
                    return string.Format("({0} {2} LIKE '%{1}')", ExpressionRouter((mce.Object as MemberExpression), false), ExpressionRouter(mce.Arguments[0], false, false), isNot == true ? "  NOT " : null);
                }
                else if (methodName == "ToString")
                {
                    return ExpressionRouter((mce.Object), false, isSingleQuotation);
                }
                else if (methodName.StartsWith("ToDateTime"))
                {
                    if (mce.Object != null)
                    {
                        return ExpressionRouter((mce.Object), false, isSingleQuotation);
                    }
                    else if (mce.Arguments.Count == 1)
                    {
                        if (isSingleQuotation)
                        {
                            return (Convert.ToDateTime(ExpressionRouter(mce.Arguments[0], false, false))).ToString().ToSqlValue();
                        }
                        else
                        {
                            return Convert.ToDateTime(ExpressionRouter(mce.Arguments[0], false, false)).ToString();
                        }
                    }
                }
                else if (methodName.StartsWith("To"))
                {
                    if (mce.Object != null)
                    {
                        return ExpressionRouter((mce.Object), false, isSingleQuotation);
                    }
                    else if (mce.Arguments.Count == 1)
                    {
                        return ExpressionRouter(mce.Arguments[0], false, isSingleQuotation);
                    }
                }

                throw new Exception(string.Format("目前不支支：{0}函数", methodName));
            }
            else if (exp is ConstantExpression)
            {
                ConstantExpression ce = ((ConstantExpression)exp);
                if (ce.Value == null)
                    return "null";
                else if (ce.Value is Boolean)
                {
                    return Convert.ToBoolean(ce.Value) ? "1=1" : "1<>1";
                }
                else if (ce.Value is ValueType)
                {
                    if (isSingleQuotation)
                    {
                        return (ce.Value.ToString()).ToSqlValue();
                    }
                    else
                    {
                        return ce.Value.ToString().ToSqlFilter();
                    }
                }
                else if (ce.Value is string || ce.Value is DateTime || ce.Value is char)
                {
                    if (isSingleQuotation)
                    {
                        return (ce.Value.ToString()).ToSqlValue();
                    }
                    else
                    {
                        return ce.Value.ToString().ToSqlFilter();
                    }
                }
            }
            else if (exp is UnaryExpression)
            {
                UnaryExpression ue = ((UnaryExpression)exp);
                var mex = ue.Operand;
                return ExpressionRouter(mex, true, isSingleQuotation);

            }
            return null;
        }
        /// <summary>
        /// 获取表达示类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        static string ExpressionTypeCast(ExpressionType type)
        {
            switch (type)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    return " AND ";
                case ExpressionType.Equal:
                    return " =";
                case ExpressionType.GreaterThan:
                    return " >";
                case ExpressionType.GreaterThanOrEqual:
                    return ">=";
                case ExpressionType.LessThan:
                    return "<";
                case ExpressionType.LessThanOrEqual:
                    return "<=";
                case ExpressionType.NotEqual:
                    return "<>";
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    return " Or ";
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                    return "+";
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    return "-";
                case ExpressionType.Divide:
                    return "/";
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                    return "*";
                default:
                    return null;
            }
        }
    }
}
