using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace SqlSugar
{
    //局部类：拉姆达解析分类处理
    internal partial class ResolveExpress
    {

        private string UnaryExpression(Expression exp, ref MemberType type)
        {
            UnaryExpression ue = ((UnaryExpression)exp);
            var mex = ue.Operand;
            bool? isComparisonOperator = null;
            var isNot = ue.NodeType==ExpressionType.Not;
            if (mex.NodeType == ExpressionType.MemberAccess && isNot)
            {
                isComparisonOperator = false;
            }
            var cse = CreateSqlElements(mex, ref type, false,isComparisonOperator);
            if (type == MemberType.None && isNot)
            {
                cse = " NOT " + cse;
            }
            return cse;
        }

        private string MemberExpression(ref Expression exp, ref MemberType type, bool? isComparisonOperator)
        {

            MemberExpression me = ((MemberExpression)exp);
            if (me.Expression == null || me.Expression.NodeType.ToString() != "Parameter")
            {
                type = MemberType.Value;
                object dynInv = null;
                // var dynInv = Expression.Lambda(exp).Compile().DynamicInvoke();原始写法性能极慢，下面写法性能提高了几十倍
                // var dynInv= Expression.Lambda(me.Expression as ConstantExpression).Compile().DynamicInvoke();
                SetMemberValueToDynInv(ref exp, me, ref dynInv);
                if (dynInv == ExpErrorUniqueKey)//特殊情况走原始写法
                {
                    dynInv = Expression.Lambda(exp).Compile().DynamicInvoke();
                    if (dynInv != null && dynInv.GetType().IsClass)
                    {
                        dynInv = Expression.Lambda(me).Compile().DynamicInvoke();
                    }
                }
                if (dynInv == null) return null;
                else
                    return dynInv.ToString();
            }
            else if (isComparisonOperator == false)
            {
                return "(1=1)";
            }
            else
            {
                if (Type == ResolveExpressType.nT)
                {
                    type = MemberType.Key;
                    var dbName = exp.ToString();
                    if (DB != null && DB.IsEnableAttributeMapping && DB._mappingColumns.IsValuable())
                    {
                        var preName = dbName.Split('.').First();
                        if (DB._mappingColumns.Any(it => it.Key == dbName.Split('.').Last()))
                        {
                            dbName = preName + "." + DB._mappingColumns.Single(it => dbName.EndsWith("." + it.Key)).Value;
                        }

                    }
                    return dbName;
                }

                string name = me.Member.Name;
                type = MemberType.Key;
                if (DB != null && DB.IsEnableAttributeMapping && DB._mappingColumns.IsValuable())
                {
                    if (DB._mappingColumns.Any(it => it.Key == name))
                    {
                        var dbName = DB._mappingColumns.Single(it => it.Key == name).Value;
                        return dbName;
                    }

                }
                return name;
            }
        }

        private static string ConstantExpression(Expression exp, ref MemberType type)
        {
            type = MemberType.Value;
            ConstantExpression ce = ((ConstantExpression)exp);
            if (ce.Value == null)
                return "null";
            else if (ce.Value.ToString().IsIn("True", "False"))//是bool值
            {
                var ceType = ce.Value.GetType();
                var ceValue = ce.Value.ToString();
                var ceNewValue = ConstantBoolDictionary.Single(it => it.Type == ceType && it.OldValue.ToLower() == ceValue.ToLower());
                return ceNewValue.Key.ToString();
            }
            else
            {
                return ce.Value.ToString();
            }
        }

        private string MethodCallExpression(Expression exp, ref MemberType type, bool isTure)
        {
            MethodCallExpression mce = (MethodCallExpression)exp;
            string methodName = mce.Method.Name;
            if (methodName == "Contains")
            {
                return Contains(methodName, mce, isTure);
            }
            else if (methodName == "StartsWith")
            {
                return StartsWith(methodName, mce, isTure);
            }
            else if (methodName == "EndsWith")
            {
                return EndWith(methodName, mce, isTure);
            }
            else if (methodName == "ToString")
            {
                type = MemberType.Value;
                return MethodToString(methodName, mce, ref type);
            }
            else if (methodName == "IsNullOrEmpty")
            {
                type = MemberType.Value;
                return IsNullOrEmpty(methodName, mce, isTure);
            }
            else if (methodName == "Equals")
            {
                return Equals(methodName, mce);
            }
            else
            {
                type = MemberType.Value;
                return MethodTo(methodName, mce, ref type);
            }
        }

        private string BinaryExpression(Expression exp)
        {
            var expression = exp as BinaryExpression;
            var isComparisonOperator =
            expression.NodeType != ExpressionType.And &&
            expression.NodeType != ExpressionType.AndAlso &&
            expression.NodeType != ExpressionType.Or &&
            expression.NodeType != ExpressionType.OrElse;
            MemberType leftType = MemberType.None;
            MemberType rightType = MemberType.None;
            var leftIsDateTime = expression.Left.Type.ToString().Contains("System.DateTime");
            var rightIsDateTime = expression.Right.Type.ToString().Contains("System.DateTime");
            var left = CreateSqlElements(expression.Left, ref leftType, true, isComparisonOperator);
            var right = CreateSqlElements(expression.Right, ref rightType, true, isComparisonOperator);
            var oper = GetOperator(expression.NodeType);
            var isKeyOperValue = leftType == MemberType.Key && rightType == MemberType.Value;
            var isValueOperKey = rightType == MemberType.Key && leftType == MemberType.Value;
            #region 处理 null

            if (isKeyOperValue && right.IsGuid() && ConstantBoolDictionary.Any(it => it.Key.ToString() == right))
            {
                right = ConstantBoolDictionary.Single(it => it.Key.ToString() == right).NewValue;
            }
            if (isValueOperKey && ConstantBoolDictionary.Any(it => it.Key.ToString() == left))
            {
                left = ConstantBoolDictionary.Single(it => it.Key.ToString() == left).NewValue;
            }

            if (isKeyOperValue & (right == "null" || right == null) && oper.Trim() == "=")
            {
                var oldLeft = AddParas(ref left, right);
                return string.Format(" ({0} is null ) ", oldLeft);
            }
            else if (isKeyOperValue & (right == "null" || right == null) && oper.Trim() == "<>")
            {
                var oldLeft = AddParas(ref left, right);
                return string.Format(" ({0} is not null ) ", oldLeft);
            }
            else if (isValueOperKey & (left == "null" || left == null) && oper.Trim() == "=")
            {
                return string.Format(" ({0} is null ) ", right);
            }
            else if (isValueOperKey & (left == "null" || left == null) && oper.Trim() == "<>")
            {
                return string.Format(" ({0} is not null ) ", right);
            }
            #endregion
            else if (isKeyOperValue)
            {
                object parValue = null;
                if (leftIsDateTime && right != null && right.IsDate())
                {
                    parValue = Convert.ToDateTime(right);
                }
                else
                {
                    parValue = right;
                }
                var oldLeft = AddParas(ref left, parValue);
                return string.Format(" ({0} {1} " + SqlSugarTool.ParSymbol + "{2}) ", oldLeft, oper, left);
            }
            else if (isValueOperKey)
            {
                object parValue = null;
                if (rightIsDateTime && left != null && left.IsDate())
                {
                    parValue = Convert.ToDateTime(left);
                }
                else
                {
                    parValue = left;
                }
                var oldRight = AddParasReturnRight(parValue, ref  right);
                return string.Format("( " + SqlSugarTool.ParSymbol + "{0} {1} {2} )", right, oper, oldRight);
            }
            else if (leftType == MemberType.Value && rightType == MemberType.Value)
            {
                return string.Format("( '{0}' {1} '{2}' )", left, oper, right);
            }
            else
            {
                return string.Format("( {0} {1} {2} )", left, oper, right);
            }
        }

        private string LambdaExpression(Expression exp)
        {
            LambdaExpression lambda = exp as LambdaExpression;
            var expression = lambda.Body;
            MemberType EleType = MemberType.None;
            if (expression.NodeType == ExpressionType.MemberAccess) {
                return "(1=1)";
            }
            return CreateSqlElements(expression, ref EleType, true);
        }
    }
}
