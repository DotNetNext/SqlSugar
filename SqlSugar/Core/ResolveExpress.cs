using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Reflection;
using System.Text.RegularExpressions;

namespace SqlSugar
{
    /// <summary>
    /// ** 描述：拉姆达解析类
    /// ** 创始时间：2015-7-20
    /// ** 修改时间：-
    /// ** 作者：sunkaixuan
    /// ** qq：610262374 
    /// ** 使用说明：使用请注名作者
    /// </summary>
    internal class ResolveExpress
    {
        /// <summary>
        /// 解析拉姆达
        /// </summary>
        /// <param name="sameIndex">区分相同参数名的索引号</param>
        public ResolveExpress(int sameIndex = 1)
        {
            this.SameIndex = sameIndex;
        }

        public string SqlWhere = null;
        public ResolveExpressType Type = ResolveExpressType.oneT;
        public List<SqlParameter> Paras = new List<SqlParameter>();
        private int SameIndex = 1;

        /// <summary>
        /// 获取拉姆达表达式的字段值
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public string GetExpressionRightField(Expression exp)
        {
            LambdaExpression lambda = exp as LambdaExpression;
            if (lambda.Body.NodeType.IsIn(ExpressionType.Convert))
            {
                var memberExpr =
                      ((UnaryExpression)lambda.Body).Operand as MemberExpression;
                return memberExpr.Member.Name;
            }
            else if (lambda.Body.NodeType.IsIn(ExpressionType.MemberAccess))
            {
                return (lambda.Body as MemberExpression).Member.Name;
            }
            else
            {
                Check.Exception(true, "不是有效拉姆达格式" + exp.ToString());
                return null;
            }
        }

        /// <summary>
        /// 获取拉姆达表达式的字段值多个T模式
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public string GetExpressionRightFieldByNT(Expression exp)
        {
            LambdaExpression lambda = exp as LambdaExpression;
            if (lambda.Body.NodeType.IsIn(ExpressionType.Convert))
            {
                var memberExpr =
                      ((UnaryExpression)lambda.Body).Operand as MemberExpression;
                return memberExpr.ToString();
            }
            else if (lambda.Body.NodeType.IsIn(ExpressionType.MemberAccess))
            {
                return lambda.Body.ToString();
            }
            else
            {
                Check.Exception(true, "不是有效拉姆达格式" + exp.ToString());
                return null;
            }
        }

        /// <summary>
        /// 解析表达式
        /// </summary>
        /// <param name="re">当前解析对象</param>
        /// <param name="exp">要解析的表达式</param>
        public void ResolveExpression(ResolveExpress re, Expression exp)
        {
            //初始化表达式
            Init(re, exp);

            //设置PageSize
            foreach (var par in Paras)
            {
                SqlSugarTool.SetParSize(par);
            }
        }

        /// <summary>
        /// 初始化表达式
        /// </summary>
        /// <param name="re"></param>
        /// <param name="exp"></param>
        private void Init(ResolveExpress re, Expression exp)
        {
            ResolveExpress.MemberType type = ResolveExpress.MemberType.None;
            var expStr = exp.ToString();
            var isNotBool = !expStr.Contains("True") && !expStr.Contains("False");
            var isContainsNot = expStr.Contains("Not");
            if (isContainsNot && expStr.IsMatch(@" => Not\(.+?\)") && !expStr.Contains("Contains"))
            {
                this.SqlWhere = string.Format(" AND {0}=0 or {0} is null ", Regex.Match(expStr, @" => Not\(.+\.(.+?)\)").Groups[1].Value);
            }
            else if (isNotBool)
            {
                //解析表达式
                this.SqlWhere = string.Format(" AND {0} ", re.CreateSqlElements(exp, ref type));
            }
            else
            {
                var isTrue = Regex.IsMatch(expStr, @"\=\> True$");
                var isFalse = Regex.IsMatch(expStr, @"\=\> False$");
                if (isFalse)
                {
                    this.SqlWhere = string.Format(" AND 1<>1 ");
                }
                else if (isTrue)
                {

                }
                else
                {
                    //解析表达式
                    this.SqlWhere = string.Format(" AND {0} ", re.CreateSqlElements(exp, ref type));
                }
            }
        }

        /// <summary>
        /// 递归解析表达式路由计算
        /// </summary>
        /// <returns></returns>
        private string CreateSqlElements(Expression exp, ref MemberType type, bool isTure = true)
        {
            if (exp is LambdaExpression)
            {
                LambdaExpression lambda = exp as LambdaExpression;
                var expression = lambda.Body;
                MemberType EleType = MemberType.None;
                return CreateSqlElements(expression, ref EleType);
            }
            else if (exp is BinaryExpression)
            {
                var expression = exp as BinaryExpression;
                MemberType leftType = MemberType.None;
                MemberType rightType = MemberType.None;
                var leftIsDateTime = expression.Left.Type.ToString().Contains("System.DateTime");
                var rightIsDateTime = expression.Right.Type.ToString().Contains("System.DateTime");
                var left = CreateSqlElements(expression.Left, ref leftType);
                var right = CreateSqlElements(expression.Right, ref rightType);
                var oper = GetOperator(expression.NodeType);
                var isKeyOperValue = leftType == MemberType.Key && rightType == MemberType.Value;
                var isValueOperKey = rightType == MemberType.Key && leftType == MemberType.Value;
                #region 处理 null
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
                    return string.Format(" ({0} {1} @{2}) ", oldLeft, oper, left);
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
                    return string.Format("( @{0} {1} {2} )", right, oper, oldRight);
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
            else if (exp is BlockExpression)
            {

            }
            else if (exp is ConditionalExpression)
            {

            }
            else if (exp is MethodCallExpression)
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
                else
                {
                    type = MemberType.Value;
                    return MethodTo(methodName, mce, ref type);
                }

            }
            else if (exp is ConstantExpression)
            {
                type = MemberType.Value;
                ConstantExpression ce = ((ConstantExpression)exp);
                if (ce.Value == null)
                    return "null";
                else
                {
                    return ce.Value.ToString();
                }
            }
            else if (exp is MemberExpression)
            {
                MemberExpression me = ((MemberExpression)exp);
                if (me.Expression == null || me.Expression.NodeType.ToString() != "Parameter")
                {
                    type = MemberType.Value;
                    object dynInv = null;
                    try
                    {
                        // var dynInv = Expression.Lambda(exp).Compile().DynamicInvoke();原始写法性能极慢，下面写法性能提高了几十倍
                        // var dynInv= Expression.Lambda(me.Expression as ConstantExpression).Compile().DynamicInvoke();
                        var conExp = me.Expression as ConstantExpression;
                        if (conExp != null)
                        {
                            dynInv = (me.Member as System.Reflection.FieldInfo).GetValue((me.Expression as ConstantExpression).Value);
                        }
                        else
                        {

                            var memberInfos = new Stack<MemberInfo>();

                            // "descend" toward's the root object reference:
                            while (exp is MemberExpression)
                            {
                                var memberExpr = exp as MemberExpression;
                                memberInfos.Push(memberExpr.Member);
                                exp = memberExpr.Expression;
                            }

                            // fetch the root object reference:
                            var constExpr = exp as ConstantExpression;
                            var objReference = constExpr.Value;

                            // "ascend" back whence we came from and resolve object references along the way:
                            while (memberInfos.Count > 0)  // or some other break condition
                            {
                                var mi = memberInfos.Pop();
                                if (mi.MemberType == MemberTypes.Property)
                                {
                                    objReference = objReference.GetType()
                                                               .GetProperty(mi.Name)
                                                               .GetValue(objReference, null);
                                }
                                else if (mi.MemberType == MemberTypes.Field)
                                {
                                    objReference = objReference.GetType()
                                                               .GetField(mi.Name)
                                                               .GetValue(objReference);
                                }
                            }
                            dynInv = objReference;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (me.ToString() == "DateTime.Now")
                        {
                            return DateTime.Now.ToString();
                        }
                        Check.Exception(true, "错误信息:{0} \r\n message:{1}", "拉姆达解析出错,参数可支持的函数有 Trim 、Contains 、ObjToXXX、 Convert.ToXXX、Contains、StartsWith和StartsEnd。 ", ex.Message);
                    }

                    if (dynInv == null) return null;
                    else
                        return dynInv.ToString();
                }
                else
                {
                    if (Type == ResolveExpressType.nT)
                    {
                        type = MemberType.Key;
                        return exp.ToString();
                    }

                    string name = me.Member.Name;
                    type = MemberType.Key;
                    return name;
                }
            }
            else if (exp is UnaryExpression)
            {
                UnaryExpression ue = ((UnaryExpression)exp);
                var mex = ue.Operand;
                return CreateSqlElements(mex, ref type, false);
            }
            return null;
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

        /// <summary>
        /// 添加参数
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        private string AddParas(ref string left, object right)
        {
            string oldLeft = left;
            left = left + SameIndex;
            SameIndex++;
            if (Type != ResolveExpressType.oneT)
            {
                left = left.Replace(".", "_");
            }
            if (right == null)
            {
                this.Paras.Add(new SqlParameter("@" + left, DBNull.Value));
            }
            else
            {
                this.Paras.Add(new SqlParameter("@" + left, right));
            }
            return oldLeft;
        }

        /// <summary>
        /// 添加参数并返回右边值
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        private string AddParasReturnRight(object left, ref string right)
        {
            string oldRight = right;
            right = right + SameIndex;
            SameIndex++;
            if (Type != ResolveExpressType.oneT)
            {
                right = right.Replace(".", "_");
            }
            if (left == null)
            {
                this.Paras.Add(new SqlParameter("@" + right, DBNull.Value));
            }
            else
            {
                this.Paras.Add(new SqlParameter("@" + right, left));
            }
            return oldRight;
        }

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
            return string.Format("({0} {1} LIKE @{2}+'%')", oldLeft, isTure == false ? "  NOT " : null, left);
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
            return string.Format("({0} {1} LIKE '%'+@{2})", oldLeft, isTure == false ? "  NOT " : null, left);
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
            return string.Format("({0} {1} LIKE '%'+@{2}+'%')", oldLeft, isTure == false ? "  NOT " : null, left);
        }


        /// <summary>
        /// 根据条件生成对应的sql查询操作符
        /// </summary>
        /// <param name="expressiontype"></param>
        /// <returns></returns>
        private string GetOperator(ExpressionType expressiontype)
        {
            switch (expressiontype)
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
                    return " OR ";
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
                    throw new SqlSugarException(string.Format("拉姆达解析出错：不支持{0}此种运算符查找！" + expressiontype));
            }
        }

        /// <summary>
        /// 拉姆达成员类型
        /// </summary>
        public enum MemberType
        {
            None = 0,
            Key = 1,
            Value = 2
        }
    }
}
