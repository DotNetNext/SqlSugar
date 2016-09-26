using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace SqlSugar
{
    internal partial class ResolveExpress
    {
        /// <summary>
        /// 拉姆达StartsWith函数处理
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="mce"></param>
        /// <param name="isTure"></param>
        /// <returns></returns>
        private string StartsWith(string methodName, MethodCallExpression mce, bool isTure)
        {
            MemberType leftType = MemberType.None;
            MemberType rightType = MemberType.None;
            var left = CreateSqlElements(mce.Object, ref leftType);
            var right = CreateSqlElements(mce.Arguments[0], ref rightType);
            var oldLeft = AddParas(ref left, right);
            return string.Format("({0} {1} LIKE @{2}+'%')", oldLeft,null, left);
        }

        /// <summary>
        /// 拉姆达EndWith函数处理
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="mce"></param>
        /// <param name="isTure"></param>
        /// <returns></returns>
        private string EndWith(string methodName, MethodCallExpression mce, bool isTure)
        {
            MemberType leftType = MemberType.None;
            MemberType rightType = MemberType.None;
            var left = CreateSqlElements(mce.Object, ref leftType);
            var right = CreateSqlElements(mce.Arguments[0], ref rightType);
            var oldLeft = AddParas(ref left, right);
            return string.Format("({0} {1} LIKE '%'+@{2})", oldLeft,null, left);
        }

        /// <summary>
        /// 拉姆达Contains函数处理
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="mce"></param>
        /// <param name="isTure"></param>
        /// <returns></returns>
        private string Contains(string methodName, MethodCallExpression mce, bool isTure)
        {
            MemberType leftType = MemberType.None;
            MemberType rightType = MemberType.None;
            var left = CreateSqlElements(mce.Object, ref leftType);
            var right = CreateSqlElements(mce.Arguments[0], ref rightType);
            var oldLeft = AddParas(ref left, right);
            return string.Format("({0} {1} LIKE '%'+@{2}+'%')", oldLeft, null, left);
        }

        /// <summary>
        /// 非空验证
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="mce"></param>
        /// <param name="isTure"></param>
        /// <returns></returns>
        private string IsNullOrEmpty(string methodName, MethodCallExpression mce, bool isTure)
        {
            MemberType leftType = MemberType.None;
            MemberType rightType = MemberType.None;
            var isConstant = mce.Arguments.First().NodeType == ExpressionType.Constant;
            var left = CreateSqlElements(mce.Object, ref leftType);
            var right = CreateSqlElements(mce.Arguments[0], ref rightType);
            if (right == "null")
            {
                right = "";
            }
            if (isConstant)
            {
                var oldLeft = AddParas(ref left, right);
                if (isTure)
                {
                    return string.Format("(@{0} is null OR @{0}='' )", left);
                }
                else
                {
                    return string.Format("(@{0} is not null AND @{0}<>'' )", left);
                }
            }
            else
            {
                if (isTure)
                {
                    if (rightType == MemberType.Key)
                    {
                        return string.Format("({0} is null OR {0}='' )", right.ToSqlFilter());
                    }
                    else
                    {
                        return string.Format("'({0}' is null OR '{0}'='' )", right.ToSqlFilter());
                    }
                }
                else
                {
                    if (rightType == MemberType.Key)
                    {
                        return string.Format("({0} is not null AND {0}<>'' )", right.ToSqlFilter());
                    }
                    else {

                        return string.Format("('{0}' is not null AND '{0}'<>'' )", right.ToSqlFilter());
                    }
                }

            }
        }

        /// <summary>
        /// 拉姆达函数处理
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="mce"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private string MethodTo(string methodName, MethodCallExpression mce, ref MemberType type)
        {
            string value = string.Empty;
            if (mce.Arguments.IsValuable())
            {
                value = CreateSqlElements(mce.Arguments.FirstOrDefault(), ref type);
            }
            else
            {
                value = MethodToString(methodName, mce, ref type); ;
            }
            if (methodName == "ToDateTime" || methodName == "ObjToDate")
            {
                return Convert.ToDateTime(value).ToString();
            }
            else if (methodName.StartsWith("ToInt"))
            {
                return Convert.ToInt32(value).ToString();
            }
            else if (methodName.StartsWith("Trim"))
            {
                return (value.ObjToString()).Trim();
            }
            else if (methodName.StartsWith("ObjTo"))
            {
                return value;
            }
            return value;
        }

        /// <summary>
        /// 拉姆达ToString函数处理
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="mce"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private string MethodToString(string methodName, MethodCallExpression mce, ref MemberType type)
        {
            return CreateSqlElements(mce.Object, ref type);
        }
    }
}
