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
        public ResolveExpressType Type = ResolveExpressType.OneT;
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
        private string CreateSqlElements(Expression exp, ref MemberType type, bool isTure, bool? isComparisonOperator=null)
        {
            //主入口
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
                throw new SqlSugarException(ExpBlockExpression + exp.ToString());
            }
            else if (exp is ConditionalExpression)
            {
                throw new SqlSugarException(ExpConditionalExpression + exp.ToString());
            }
            else if (exp is MethodCallExpression)
            {
                return MethodCallExpression(exp, ref type, isTure);
            }
            else if (exp is ConstantExpression)
            {
                return ConstantExpression(exp, ref type,isComparisonOperator);
            }
            else if (exp is MemberExpression)
            {
                return MemberExpression(ref exp, ref type,isComparisonOperator);
            }
            else if (exp is UnaryExpression)
            {
                return UnaryExpression(exp, ref type);
            }
            else if (exp != null && exp.NodeType.IsIn(ExpressionType.New, ExpressionType.NewArrayBounds, ExpressionType.NewArrayInit))
            {
                throw new SqlSugarException(ExpNew+ exp.ToString());
            }
            return null;
        }

        /// <summary>
        /// 将解析值赋给dynInv
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="me"></param>
        /// <param name="dynInv"></param>
        private static void SetMemberValueToDynInv(ref Expression exp, MemberExpression me, ref object dynInv)
        {
            var conExp = me.Expression as ConstantExpression;
            var fieldInfo = me.Member as System.Reflection.FieldInfo;
            if (conExp != null&&fieldInfo!=null)
            {
                dynInv = (fieldInfo).GetValue((me.Expression as ConstantExpression).Value);
                if (fieldInfo.FieldType.IsEnum)
                {
                    dynInv = Convert.ToInt64(Enum.ToObject(fieldInfo.FieldType, dynInv));
                }
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
                                    throw new SqlSugarException(string.Format(ExpNoSupportObjectOrAttr, dynInv.GetType().FullName, dynInv.GetType().FullName));
                                }
                            }
                            return;
                        }
                        else if (memberExpr.Member.MemberType == MemberTypes.Field)
                        {
                            FieldInfo field = (FieldInfo)memberExpr.Member;
                            dynInv = field.GetValue(memberExpr.Member);
                            if (dynInv != null && dynInv.GetType().IsClass&&dynInv.GetType()!=SqlSugarTool.StringType) {
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
                                    throw new SqlSugarException(string.Format(ExpNoSupportObjectOrAttr, dynInv.GetType().FullName, dynInv.GetType().FullName));
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
                if (constExpr == null) {
                    dynInv = ExpErrorUniqueKey;
                    return;
                }
                var objReference = constExpr.Value;

                // "ascend" back whence we came from and resolve object references along the way:
                while (memberInfos.Count > 0)  // or some other break condition
                {
                    var mi = memberInfos.Pop();
                    if (mi.MemberType == MemberTypes.Property)
                    {
                        var objProp= objReference.GetType().GetProperty(mi.Name);
                        if (objProp == null) {
                            dynInv = ExpErrorUniqueKey;
                            return;
                        }
                        objReference = objProp.GetValue(objReference, null);
                    }
                    else if (mi.MemberType == MemberTypes.Field)
                    {
                        var objField=objReference.GetType().GetField(mi.Name);
                        if (objField == null) {
                            dynInv = ExpErrorUniqueKey;
                            return;
                        }
                        objReference = objField.GetValue(objReference);
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
            if (Type != ResolveExpressType.OneT)
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
            if (Type != ResolveExpressType.OneT)
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
