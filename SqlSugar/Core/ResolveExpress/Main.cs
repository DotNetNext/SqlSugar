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
    /// ** 修改时间：2016-9-26
    /// ** 作者：sunkaixuan
    /// ** qq：610262374 
    /// ** 使用说明：使用请注名作者
    /// </summary>
    internal partial class ResolveExpress
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
        private SqlSugarClient DB;


        /// <summary>
        /// 解析表达式
        /// </summary>
        /// <param name="re">当前解析对象</param>
        /// <param name="exp">要解析的表达式</param>
        /// <param name="db">数据库访问对象</param>
        public void ResolveExpression(ResolveExpress re, Expression exp, SqlSugarClient db)
        {
            DB = db;
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
            //解析表达式
            this.SqlWhere = string.Format(" AND {0} ", re.CreateSqlElements(exp, ref type,true));
            //还原bool值
            foreach (var item in ConstantBoolDictionary)
            {
                if (this.SqlWhere.IsValuable())
                {
                    this.SqlWhere = this.SqlWhere.Replace(item.Key.ToString(), item.ConditionalValue);
                }
            }

        }

        /// <summary>
        /// 递归解析表达式路由计算
        /// </summary>
        /// <returns></returns>
        private string CreateSqlElements(Expression exp, ref MemberType type, bool isTure)
        {
            if (exp is LambdaExpression)
            {
                return LambdaExpression(exp);
            }
            else if (exp is BinaryExpression)
            {
                return BinaryExpression(exp);

            }
            else if (exp is BlockExpression)
            {

            }
            else if (exp is ConditionalExpression)
            {

            }
            else if (exp is MethodCallExpression)
            {
                return MethodCallExpression(exp, ref type, isTure);

            }
            else if (exp is ConstantExpression)
            {
                return ConstantExpression(exp, ref type);
            }
            else if (exp is MemberExpression)
            {
                return MemberExpression(ref exp, ref type);
            }
            else if (exp is UnaryExpression)
            {
                return UnaryExpression(exp, ref type);
            }
            else if (exp != null && exp.NodeType.IsIn(ExpressionType.New, ExpressionType.NewArrayBounds, ExpressionType.NewArrayInit))
            {
                throw new SqlSugarException("拉姆达表达式内不支持new对象，请提取变量后在赋值，错误信息" + exp.ToString());
            }
            return null;
        }

        private string UnaryExpression(Expression exp, ref MemberType type)
        {
            UnaryExpression ue = ((UnaryExpression)exp);
            var mex = ue.Operand;
            var cse = CreateSqlElements(mex, ref type, false);
            if (type == MemberType.None && ue.NodeType.ToString() == "Not")
            {
                cse = " NOT " + cse;
            }
            return cse;
        }

        private string MemberExpression(ref Expression exp, ref MemberType type)
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
            MemberType leftType = MemberType.None;
            MemberType rightType = MemberType.None;
            var leftIsDateTime = expression.Left.Type.ToString().Contains("System.DateTime");
            var rightIsDateTime = expression.Right.Type.ToString().Contains("System.DateTime");
            var left = CreateSqlElements(expression.Left, ref leftType, true);
            var right = CreateSqlElements(expression.Right, ref rightType, true);
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
            return CreateSqlElements(expression, ref EleType, true);
        }

        private static void SetMemberValueToDynInv(ref Expression exp, MemberExpression me, ref object dynInv)
        {
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
                    if (memberExpr.Expression == null)
                    {
                        if (memberExpr.Member.MemberType == MemberTypes.Property)
                        {
                            PropertyInfo pro = (PropertyInfo)memberExpr.Member;
                            dynInv = pro.GetValue(memberExpr.Member, null);
                            if (dynInv != null && dynInv.GetType().IsClass)
                            {
                                var fieldName = me.Member.Name;
                                var proInfo = dynInv.GetType().GetProperty(fieldName);
                                if (proInfo != null)
                                {
                                    dynInv = proInfo.GetValue(dynInv, null);
                                }
                                var fieInfo = dynInv.GetType().GetField(fieldName);
                                if (fieInfo != null)
                                {
                                    dynInv = fieInfo.GetValue(dynInv);
                                }
                                if (fieInfo == null && proInfo == null)
                                {
                                    throw new SqlSugarException("拉姆达解析不支持" + dynInv.GetType().FullName + "对象，或该" + dynInv.GetType().FullName + "的属性。");
                                }
                            }
                            return;
                        }
                        else if (memberExpr.Member.MemberType == MemberTypes.Field)
                        {
                            FieldInfo field = (FieldInfo)memberExpr.Member;
                            dynInv = field.GetValue(memberExpr.Member);
                            if (dynInv != null && dynInv.GetType().IsClass) {
                                var fieldName = me.Member.Name;
                                var proInfo = dynInv.GetType().GetProperty(fieldName);
                                if (proInfo != null) {
                                    dynInv=proInfo.GetValue(dynInv,null);
                                }
                                var fieInfo = dynInv.GetType().GetField(fieldName);
                                if (fieInfo != null) {
                                    dynInv = fieInfo.GetValue(dynInv);
                                }
                                if (fieInfo == null && proInfo == null)
                                {
                                    throw new SqlSugarException("拉姆达解析不支持" + dynInv.GetType().FullName+ "对象，或该"+ dynInv.GetType().FullName+"的属性。");
                                }
                            }
                            return;
                        }

                    }
                    if (memberExpr.Expression == null)
                    {
                        dynInv = ExpErrorUniqueKey;
                        return;
                    }
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
                this.Paras.Add(new SqlParameter(SqlSugarTool.ParSymbol + left, DBNull.Value));
            }
            else
            {
                this.Paras.Add(new SqlParameter(SqlSugarTool.ParSymbol + left, right));
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
                this.Paras.Add(new SqlParameter(SqlSugarTool.ParSymbol + right, DBNull.Value));
            }
            else
            {
                this.Paras.Add(new SqlParameter(SqlSugarTool.ParSymbol + right, left));
            }
            return oldRight;
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
                    throw new SqlSugarException(string.Format(OperatorError + expressiontype));
            }
        }

    }
}
