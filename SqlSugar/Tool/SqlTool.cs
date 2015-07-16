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
        /// 生成MappingTable的内部函数
        /// </summary>
        /// <param name="mappCount"></param>
        /// <returns></returns>
        public static string CreateMappingTable(int mappCount)
        {
            Func<int, string> GetMethodString = count =>
            {
                string methodString = (@"  
/// <summary>
/// 根据T1到TN生成关联查询
/// </summary>
/// <returns></returns>
public static Sqlable MappingTable<{0}>(this Sqlable sqlable{5})
           {1}
        {{
    if (sqlable.SqlableCurrentState != null)
                throw new Exception("".MappingTable只能在Sqlable后面使用,正确用法：Sqlable.MappingTable "");
            {2};
            sqlable.SqlableCurrentState = SqlableCurrentState.MappingTable;
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(""{3}"",{4});
            sqlable.Sql = sb;
            return sqlable;
        }}
            ");
                string tStr = string.Empty;
                string tStr2 = string.Empty;
                string tStr3 = string.Empty;
                string tStr4 = string.Empty;
                string tStr5 = string.Empty;
                string tStr6 = string.Empty;
                int j = 2;
                for (int i = 1; i <= count; i++)
                {

                    tStr += string.Format("T{0},", i);
                    tStr2 += string.Format("\n where T{0} : new() \n", i);
                    tStr3 += string.Format("\n    T{0} t{0} = new T{0}(); \n", i);
                    if (i != count)
                        tStr6 += string.Format(", string mappingFeild" + i);
                    if (i == 1)
                    {
                        tStr4 += @" FROM {1} t1 {0} \n   ";
                        tStr5 += "sqlable.IsNoLock.IsNoLock(),t" + i + ".GetType().Name,";

                    }
                    else
                    {

                        tStr4 += @"\n {" + (j) + "} JOIN {" + (j + 1) + "} t" + i + " {0} ON {" + (j + 2) + "} ";

                        tStr5 += "mappingFeild" + (i - 1) + ".IsLeft(),t" + (i) + ".GetType().Name, mappingFeild" + (i - 1) + ".Remove(),";
                        j = j + 3;
                    }

                }
                methodString = string.Format(methodString, tStr.TrimEnd(','), tStr2.TrimEnd(','), tStr3.TrimEnd(','), tStr4, tStr5.TrimEnd(','), tStr6);
                return methodString;
            };
            string reval = string.Empty;
            for (int i = 2; i < mappCount; i++)
            {
                reval += GetMethodString(i);
            }
            return reval;
        }


        /// <summary>
        /// Reader转成List<T>
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="dr"></param>
        /// <param name="isClose"></param>
        /// <returns></returns>
        public static List<T> DataReaderToList<T>(Type type,IDataReader dr, bool isClose = true)
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

        public static SqlParameter[] GetObjectToParameters(object obj)
        {
            List<SqlParameter> listParams = new List<SqlParameter>();
            if (obj != null)
            {
                var propertiesObj = obj.GetType().GetProperties();
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
            var propertiesObj = obj.GetType().GetProperties();
            string replaceGuid = Guid.NewGuid().ToString();
            foreach (PropertyInfo r in propertiesObj)
            {
                var val = r.GetValue(obj, null);
                reval.Add(r.Name, val == null ? "" : val.ToString());
            }

            return reval;
        }

        public static string GetWhereByExpression<T>(Expression<Func<T, bool>> expression)
        {
            string whereStr = string.Empty;
            if (expression.Body is BinaryExpression)
            {
                BinaryExpression be = ((BinaryExpression)expression.Body);
                whereStr = " and " + SqlTool.BinarExpressionProvider(be.Left, be.Right, be.NodeType);
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
        static string ExpressionRouter(Expression exp, bool isRight)
        {
            string sb = string.Empty;
            if (exp is BinaryExpression)
            {
                BinaryExpression be = ((BinaryExpression)exp);
                return BinarExpressionProvider(be.Left, be.Right, be.NodeType);
            }
            else if (exp is MemberExpression)
            {
                if (!isRight)
                {
                    MemberExpression me = ((MemberExpression)exp);
                    return me.Member.Name;
                }
                else
                {
                    return Expression.Lambda(exp).Compile().DynamicInvoke() + "";
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
                if (mce.Method.Name == "Like")
                    return string.Format("({0} like {1})", ExpressionRouter(mce.Arguments[0], false), ExpressionRouter(mce.Arguments[1], false));
                else if (mce.Method.Name == "NotLike")
                    return string.Format("({0} Not like {1})", ExpressionRouter(mce.Arguments[0], false), ExpressionRouter(mce.Arguments[1], false));
                else if (mce.Method.Name == "In")
                    return string.Format("{0} In ({1})", ExpressionRouter(mce.Arguments[0], false), ExpressionRouter(mce.Arguments[1], false));
                else if (mce.Method.Name == "NotIn")
                    return string.Format("{0} Not In ({1})", ExpressionRouter(mce.Arguments[0], false), ExpressionRouter(mce.Arguments[1], false));
            }
            else if (exp is ConstantExpression)
            {
                ConstantExpression ce = ((ConstantExpression)exp);
                if (ce.Value == null)
                    return "null";
                else if (ce.Value is ValueType)
                    return ce.Value.ToString();
                else if (ce.Value is string || ce.Value is DateTime || ce.Value is char)
                    return string.Format("{0}", ce.Value.ToString());
            }
            else if (exp is UnaryExpression)
            {
                UnaryExpression ue = ((UnaryExpression)exp);
                var mexp = (MemberExpression)ue.Operand;
                object value = (mexp.Expression as ConstantExpression).Value;
                string name = mexp.Member.Name;
                System.Reflection.FieldInfo info = value.GetType().GetField(name);
                object obj = info.GetValue(value);
                return obj + "";
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
