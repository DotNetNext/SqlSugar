using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Data.SqlClient;

namespace SqlSugar
{
    public class ResolveExpress
    {
        public Dictionary<string, object> Argument;
        public string SqlWhere;
        public SqlParameter[] Paras;

        /// <summary>
        /// 递归解析表达式路由计算
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="isNot"></param>
        /// <param name="isSingleQuotation"></param>
        /// <returns></returns>
        protected static string ExpressionRouter(Expression exp)
        {
            string sb = string.Empty;
            if (exp is MemberExpression)
            {
                MemberExpression me = ((MemberExpression)exp);
                if (me.Expression == null || me.Expression.NodeType.ToString() != "Parameter")
                {
                    return (Expression.Lambda(exp).Compile().DynamicInvoke().ToString());
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
                    tmpstr.Append(ExpressionRouter(ex));
                    tmpstr.Append(",");
                }
                return tmpstr.ToString(0, tmpstr.Length - 1);
            }
            else if (exp is MethodCallExpression)
            {
                MethodCallExpression mce = (MethodCallExpression)exp;
                string methodName = mce.Method.Name;
                if (methodName == "ToString")
                {
                    return ExpressionRouter((mce.Object));
                }
                else if (methodName.StartsWith("ToDateTime"))
                {
                    if (mce.Object != null)
                    {
                        return ExpressionRouter((mce.Object));
                    }
                    else if (mce.Arguments.Count == 1)
                    {
                        return (Convert.ToDateTime(ExpressionRouter(mce.Arguments[0]))).ToString();
                    }
                }
                else if (methodName.StartsWith("To"))
                {
                    if (mce.Object != null)
                    {
                        return ExpressionRouter((mce.Object));
                    }
                    else if (mce.Arguments.Count == 1)
                    {
                        return ExpressionRouter(mce.Arguments[0]);
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
                    return ce.Value.ToString();
                }
                else if (ce.Value is string || ce.Value is DateTime || ce.Value is char)
                {
                    return ce.Value.ToString();
                }
            }
            else if (exp is UnaryExpression)
            {
                UnaryExpression ue = ((UnaryExpression)exp);
                var mex = ue.Operand;
                return ExpressionRouter(mex);

            }
            return null;
        }

        /// <summary>
        /// 解析lamdba，生成Sql查询条件
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public void ResolveExpression(Expression expression)
        {
            this.Argument = new Dictionary<string, object>();
            this.SqlWhere = " AND " + Resolve(expression);
            this.Paras = Argument.Select(x => new SqlParameter(x.Key, x.Value)).ToArray();
        }

        private string Resolve(Expression expression)
        {
            if (expression is LambdaExpression)
            {
                LambdaExpression lambda = expression as LambdaExpression;
                expression = lambda.Body;
                return Resolve(expression);
            }
            if (expression is BinaryExpression)
            {
                BinaryExpression binary = expression as BinaryExpression;
                //解析x=>x.Name=="123" x.Age==123这类
                if (binary.Left is MemberExpression && binary.Right is ConstantExpression)
                    return ResolveFunc(binary.Left, binary.Right, binary.NodeType);
                //解析x=>x.Name.Contains("xxx")==false这类的
                if (binary.Left is MethodCallExpression && binary.Right is ConstantExpression)
                {
                    bool isTrue = Convert.ToBoolean((binary.Right as ConstantExpression).Value);
                    return ResolveLinqToObject(binary.Left, isTrue, binary.NodeType);
                }
                //解析x=>x.Date==DateTime.Now这种
                if (binary.Left is MemberExpression && binary.Right is MemberExpression)
                {
                    LambdaExpression lambda = Expression.Lambda(binary.Right);
                    Delegate fn = lambda.Compile();
                    ConstantExpression value = Expression.Constant(fn.DynamicInvoke(null), binary.Right.Type);
                    return ResolveFunc(binary.Left, value, binary.NodeType);
                }
                if (binary.Left is MemberExpression && binary.Right is MethodCallExpression) {
                    return ResolveFunc(binary.Left, binary.Right, binary.NodeType);
                }

            }
            if (expression is UnaryExpression)
            {
                UnaryExpression unary = expression as UnaryExpression;
                //解析!x=>x.Name.Contains("xxx")或!array.Contains(x.Name)这类
                if (unary.Operand is MethodCallExpression)
                    return ResolveLinqToObject(unary.Operand, false);
                //解析x=>!x.isDeletion这样的 
                if (unary.Operand is MemberExpression && unary.NodeType == ExpressionType.Not)
                {
                    ConstantExpression constant = Expression.Constant(false);
                    return ResolveFunc(unary.Operand, constant, ExpressionType.Equal);
                }
            }
            //解析x=>x.isDeletion这样的 
            if (expression is MemberExpression && expression.NodeType == ExpressionType.MemberAccess)
            {
                MemberExpression member = expression as MemberExpression;
                ConstantExpression constant = Expression.Constant(true);
                return ResolveFunc(member, constant, ExpressionType.Equal);
            }
            //x=>x.Name.Contains("xxx")或array.Contains(x.Name)这类
            if (expression is MethodCallExpression)
            {
                MethodCallExpression methodcall = expression as MethodCallExpression;
                return ResolveLinqToObject(methodcall, true);
            }
            var body = expression as BinaryExpression;
            if (body == null)
                throw new Exception("无法解析" + expression);
            var Operator = GetOperator(body.NodeType);
            var Left = Resolve(body.Left);
            var Right = Resolve(body.Right);
            string Result = string.Format("({0} {1} {2})", Left, Operator, Right);
            return Result;
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
                    return "and";
                case ExpressionType.AndAlso:
                    return "and";
                case ExpressionType.Or:
                    return "or";
                case ExpressionType.OrElse:
                    return "or";
                case ExpressionType.Equal:
                    return "=";
                case ExpressionType.NotEqual:
                    return "<>";
                case ExpressionType.LessThan:
                    return "<";
                case ExpressionType.LessThanOrEqual:
                    return "<=";
                case ExpressionType.GreaterThan:
                    return ">";
                case ExpressionType.GreaterThanOrEqual:
                    return ">=";
                default:
                    throw new Exception(string.Format("不支持{0}此种运算符查找！" + expressiontype));
            }
        }


        private string ResolveFunc(Expression left, Expression right, ExpressionType expressiontype)
        {
            var Name = ExpressionRouter(left);
            if (left.NodeType.ToString() != "MemberAccess")
            {
                Name = Name.ToSqlValue();
            }
            var Value = ExpressionRouter(right);
            if (right.NodeType.ToString() != "MemberAccess")
            {
                Value = Value.ToSqlValue();
            }
            var Operator = GetOperator(expressiontype);
            string CompName = SetArgument(Name, Value.ToString());
            string Result = string.Format("({0} {1} {2})", Name, Operator, CompName);
            return Result;
        }

        private string ResolveLinqToObject(Expression expression, bool isTrue, ExpressionType? expressiontype = null)
        {
            var methodCall = expression as MethodCallExpression;
            var methodName = methodCall.Method.Name;
            switch (methodName)//这里其实还可以改成反射调用，不用写switch
            {
                case "Contains":
                    if (methodCall.Object != null)
                        return Like(methodCall, isTrue);
                    return In(methodCall, isTrue);
                case "Count":
                    return Len(methodCall, isTrue, expressiontype.Value);
                case "LongCount":
                    return Len(methodCall, isTrue, expressiontype.Value);
                case "ToString":
                    return ToString(methodCall);
                default:
                    return ExpressionRouter(methodCall);
            }
        }



        private string SetArgument(string name, string value)
        {
            name = "@" + name;
            string temp = name;
            while (Argument.ContainsKey(temp))
            {
                int code = Guid.NewGuid().GetHashCode();
                if (code < 0)
                    code *= -1;
                temp = name + code;
            }
            Argument[temp] = value;
            return temp;
        }

        private string ToString(MethodCallExpression expression)
        {
            return ExpressionRouter(expression);
        }

        private string In(MethodCallExpression expression, bool isTrue)
        {
            var Argument1 = (expression.Arguments[0] as MemberExpression).Expression as ConstantExpression;
            var Argument2 = expression.Arguments[1] as MemberExpression;
            var Field_Array = Argument1.Value.GetType().GetFields().First();
            object[] Array = Field_Array.GetValue(Argument1.Value) as object[];
            List<string> SetInPara = new List<string>();
            for (int i = 0; i < Array.Length; i++)
            {
                string Name_para = "InParameter" + i;
                string Value = Array[i].ToString();
                string Key = SetArgument(Name_para, Value);
                SetInPara.Add(Key);
            }
            string Name = Argument2.Member.Name;
            string Operator = Convert.ToBoolean(isTrue) ? "in" : " not in";
            string CompName = string.Join(",", SetInPara);
            string Result = string.Format("{0} {1} ({2})", Name, Operator, CompName);
            return Result;
        }

        private string Like(MethodCallExpression expression, bool isTrue)
        {
            object Temp_Vale = string.Empty;
            if (expression.Arguments[0] is ConstantExpression)
            {
                Temp_Vale = (expression.Arguments[0] as ConstantExpression).Value;
            }
            else
            {
                Temp_Vale = ExpressionRouter(expression.Arguments[0]);
            }
            string Value = string.Format("%{0}%", Temp_Vale);
            string Name = (expression.Object as MemberExpression).Member.Name;
            string CompName = SetArgument(Name, Value);
            string Result = string.Format("{0} {1} LIKE {2}", Name, isTrue ? "" : "NOT", CompName);
            return Result;
        }

        private string Len(MethodCallExpression expression, object value, ExpressionType expressiontype)
        {
            object Name = (expression.Arguments[0] as MemberExpression).Member.Name;
            string Operator = GetOperator(expressiontype);
            string CompName = SetArgument(Name.ToString(), value.ToString());
            string Result = string.Format("LEN({0}){1}{2}", Name, Operator, CompName);
            return Result;
        }

    }
}
