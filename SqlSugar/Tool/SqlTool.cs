using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using System.Data.SqlClient;
using System.Linq.Expressions;


namespace SqlSugar
{
    /// <summary>
    /// ** 描述：工具函数
    /// ** 创始时间：2015-7-13
    /// ** 修改时间：-
    /// ** 作者：sunkaixuan
    /// ** 使用说明：
    /// </summary>
    internal class SqlTool
    {
        public static Type StringType = typeof(string);
        public static Type IntType = typeof(int);


        /// <summary>
        /// Reader转成List《T》
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="dr"></param>
        /// <param name="isClose"></param>
        /// <returns></returns>
        public static List<T> DataReaderToList<T>(Type type, IDataReader dr, bool isClose = true)
        {
            var cacheManager = CacheManager<IDataReaderEntityBuilder<T>>.GetInstance();
            string key = "DataReaderToList." + type.FullName;
            IDataReaderEntityBuilder<T> eblist = null;
            if (cacheManager.ContainsKey(key))
            {
                eblist = cacheManager[key];
            }
            else
            {
                eblist = IDataReaderEntityBuilder<T>.CreateBuilder(type, dr);
                cacheManager.Add(key, eblist, cacheManager.Day);
            }
            List<T> list = new List<T>();
            if (dr == null) return list;
            while (dr.Read())
            {
                list.Add(eblist.Build(dr));
            }
            if (isClose) { dr.Close(); dr.Dispose(); dr = null; }
            return list;
        }

        ///// <summary>
        ///// 将dataTable转成List<T>
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="dt"></param>
        ///// <returns></returns>
        //public static List<T> List<T>(DataTable dt)
        //{
        //    if (dt == null) return new List<T>();
        //    var list = new List<T>();
        //    Type type = typeof(T);

        //    string cachePropertiesKey = "db." + type.Name + ".GetProperties";
        //    var cachePropertiesManager = CacheManager<PropertyInfo[]>.GetInstance();
        //    PropertyInfo[] props = null;
        //    if (cachePropertiesManager.ContainsKey(cachePropertiesKey))
        //    {
        //        props = cachePropertiesManager[cachePropertiesKey];
        //    }
        //    else
        //    {
        //        props = type.GetProperties();
        //        cachePropertiesManager.Add(cachePropertiesKey, props, cachePropertiesManager.Day);
        //    }

        //    var plist = new List<PropertyInfo>(props);

        //    foreach (DataRow item in dt.Rows)
        //    {
        //        T s = System.Activator.CreateInstance<T>();
        //        for (int i = 0; i < dt.Columns.Count; i++)
        //        {
        //            PropertyInfo info = plist.Find(p => p.Name == dt.Columns[i].ColumnName);
        //            if (info != null)
        //            {
        //                if (!Convert.IsDBNull(item[i]))
        //                {
        //                    info.SetValue(s, item[i], null);
        //                }
        //            }
        //        }
        //        list.Add(s);
        //    }
        //    return list;
        //}

        /// <summary>
        /// 将数组转为 '1','2' 这种格式的字符串 用于 where id in(  )
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static string ToJoinSqlInVal(object[] array)
        {
            if (array == null || array.Length == 0)
            {
                return "''";
            }
            else
            {
                return string.Join(",", array.Where(c => c != null).Select(c => "'" + (c + "").Replace("'", "''") + "'"));//除止SQL注入
            }
        }

        public static SqlParameter[] GetParameters(object obj)
        {
            List<SqlParameter> listParams = new List<SqlParameter>();
            if (obj != null)
            {
                var type = obj.GetType();
                var propertiesObj = type.GetProperties();
                string replaceGuid = Guid.NewGuid().ToString();
                foreach (PropertyInfo r in propertiesObj)
                {
                    listParams.Add(new SqlParameter("@" + r.Name, r.GetValue(obj, null).ToString()));
                }
            }
            return listParams.ToArray();
        }

        public static Dictionary<string, string> GetObjectToDictionary(object obj)
        {

            Dictionary<string, string> reval = new Dictionary<string, string>();
            if (obj == null) return reval;
            var type = obj.GetType();
            var propertiesObj =type.GetProperties();
            string replaceGuid = Guid.NewGuid().ToString();
            foreach (PropertyInfo r in propertiesObj)
            {
                var val = r.GetValue(obj, null);
                reval.Add(r.Name, val == null ? "" : val.ToString());
            }

            return reval;
        }


        public static PropertyInfo[] GetGetPropertiesByCache(Type type, string cachePropertiesKey, CacheManager<PropertyInfo[]> cachePropertiesManager)
        {
            PropertyInfo[] props = null;
            if (cachePropertiesManager.ContainsKey(cachePropertiesKey))
            {
                props = cachePropertiesManager[cachePropertiesKey];
            }
            else
            {
                props = type.GetProperties();
                cachePropertiesManager.Add(cachePropertiesKey, props, cachePropertiesManager.Day);
            }
            return props;
        }

        public static string GetWhereByExpression<T>(Expression<Func<T, bool>> expression)
        {
            string whereStr = string.Empty;
            if (expression.Body is BinaryExpression)
            {
                BinaryExpression be = ((BinaryExpression)expression.Body);
                whereStr = " and " + SqlTool.BinarExpressionProvider(be.Left, be.Right, be.NodeType);
            }
            else
            {
                whereStr = " and " + ExpressionRouter(expression.Body, false);
            }
            return whereStr;
        }
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
                sb += "'" + tmpStr + "'";
            return sb += ")";
        }
        //表达式路由计算 
        protected static string ExpressionRouter(Expression exp, bool isRight, bool isNot = false)
        {

            string sb = string.Empty;
            if (exp is BinaryExpression)
            {
                BinaryExpression be = ((BinaryExpression)exp);
                var left = BinarExpressionProvider(be.Left, be.Right, be.NodeType);

            }
            else if (exp is MemberExpression)
            {
                MemberExpression me = ((MemberExpression)exp);
                if (isRight)
                {
                    return Expression.Lambda(exp).Compile().DynamicInvoke() + "";
                }
                else
                {
                    if (me.Expression != null)
                    {
                        return me.Member.Name;
                    }
                    else
                    {
                        return string.Format("'{0}'", Expression.Lambda(exp).Compile().DynamicInvoke());
                    }
                }
            }
            else if (exp is NewArrayExpression)
            {
                NewArrayExpression ae = ((NewArrayExpression)exp);
                StringBuilder tmpstr = new StringBuilder();
                foreach (Expression ex in ae.Expressions)
                {
                    tmpstr.Append(ExpressionRouter(ex, false));
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
                    return string.Format("({0} {2} LIKE '%{1}%')", ExpressionRouter((mce.Object as MemberExpression), false), ExpressionRouter(mce.Arguments[0], false), isNot == true ? "  NOT " : null);
                }
                else if (methodName == "StartsWith")
                {
                    return string.Format("({0} {2} LIKE '{1}%')", ExpressionRouter((mce.Object as MemberExpression), false), ExpressionRouter(mce.Arguments[0], false), isNot == true ? "  NOT " : null);
                }
                else if (methodName == "EndWith")
                {
                    return string.Format("({0} {2} LIKE '%{1}')", ExpressionRouter((mce.Object as MemberExpression), false), ExpressionRouter(mce.Arguments[0], false), isNot == true ? "  NOT " : null);
                }
                else if (methodName == "ToString")
                {
                    return mce.Object.ToString();
                }
                else if (methodName.StartsWith("ToDateTime"))
                {
                    if (mce.Object != null)
                    {
                        return mce.Object.ToString();
                    }
                    else if (mce.Arguments.Count == 1)
                    {
                        return Convert.ToDateTime(ExpressionRouter(mce.Arguments[0], false)).ToString();
                    }
                }
                else if (methodName.StartsWith("To"))
                {
                    if (mce.Object != null)
                    {
                        return mce.Object.ToString();
                    }
                    else if (mce.Arguments.Count == 1)
                    {
                        return ExpressionRouter(mce.Arguments[0], false);
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
                    return ce.Value.ToString();
                else if (ce.Value is string || ce.Value is DateTime || ce.Value is char)
                    return string.Format("{0}", ce.Value.ToString());
            }
            else if (exp is UnaryExpression)
            {
                UnaryExpression ue = ((UnaryExpression)exp);
                var mex = ue.Operand;
                return ExpressionRouter(mex, false, true);

            }
            return null;
        }
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
