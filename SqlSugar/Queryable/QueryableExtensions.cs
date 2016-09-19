using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.SqlClient;

namespace SqlSugar
{
    /// <summary>
    /// ** 描述：Queryable扩展函数
    /// ** 创始时间：2015-7-13
    /// ** 修改时间：-
    /// ** 作者：sunkaixuan
    /// ** 使用说明：
    /// </summary>
    public static class QueryableExtensions
    {
        /// <summary>
        /// 条件筛选
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="queryable">查询对象</param>
        /// <param name="expression">表达示条件</param>
        /// <returns>Queryable</returns>
        public static Queryable<T> Where<T>(this Queryable<T> queryable, Expression<Func<T, bool>> expression)
        {
            var type = queryable.Type;
            queryable.WhereIndex = queryable.WhereIndex + 100;
            ResolveExpress re = new ResolveExpress(queryable.WhereIndex);
            if (queryable.JoinTable.IsValuable()) {
                re.Type = ResolveExpressType.nT;
            }
            re.ResolveExpression(re, expression);
            queryable.Params.AddRange(re.Paras);
            queryable.Where.Add(re.SqlWhere);
            return queryable;
        }

        /// <summary>
        /// 条件筛选
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="queryable">查询对象</param>
        /// <param name="whereString">匿名参数(例如:new{id=1,name="张三"})</param>
        /// <returns>Queryable</returns>
        public static Queryable<T> Where<T>(this Queryable<T> queryable, string whereString, object whereObj = null)
        {
            var type = queryable.Type;
            string whereStr = string.Format(" AND {0} ", whereString);
            queryable.Where.Add(whereStr);
            if (whereObj != null)
                queryable.Params.AddRange(SqlSugarTool.GetParameters(whereObj));
            return queryable;
        }

        /// <summary>
        /// 条件筛选
        /// </summary>
        /// <typeparam name="T">表实体类型</typeparam>
        /// <typeparam name="T2">表实体类型</typeparam>
        /// <param name="queryable">查询对象</param>
        /// <param name="expression">表达示条件</param>
        /// <returns>Queryable</returns>
        public static Queryable<T> Where<T, T2>(this Queryable<T> queryable, Expression<Func<T, T2, object>> expression)
        {
            var type = queryable.Type;
            queryable.WhereIndex = queryable.WhereIndex + 100;
            ResolveExpress re = new ResolveExpress(queryable.WhereIndex);
            re.Type = ResolveExpressType.nT;
            re.ResolveExpression(re, expression);
            queryable.Params.AddRange(re.Paras);
            queryable.Where.Add(re.SqlWhere);
            return queryable;
        }

        /// <summary>
        /// 条件筛选
        /// </summary>
        /// <typeparam name="T">表实体类型</typeparam>
        /// <typeparam name="T2">表实体类型</typeparam>
        /// <typeparam name="T3">表实体类型</typeparam>
        /// <param name="queryable">查询对象</param>
        /// <param name="expression">表达示条件</param>
        /// <returns>Queryable</returns>
        public static Queryable<T> Where<T, T2, T3>(this Queryable<T> queryable, Expression<Func<T, T2, T3, object>> expression)
        {
            var type = queryable.Type;
            queryable.WhereIndex = queryable.WhereIndex + 100;
            ResolveExpress re = new ResolveExpress(queryable.WhereIndex);
            re.Type = ResolveExpressType.nT;
            re.ResolveExpression(re, expression);
            queryable.Params.AddRange(re.Paras);
            queryable.Where.Add(re.SqlWhere);
            return queryable;
        }

        /// <summary>
        /// 条件筛选
        /// </summary>
        /// <typeparam name="T">表实体类型</typeparam>
        /// <typeparam name="T2">表实体类型</typeparam>
        /// <typeparam name="T3">表实体类型</typeparam>
        /// <typeparam name="T4">表实体类型</typeparam>
        /// <param name="queryable">查询对象</param>
        /// <param name="expression">表达示条件</param>
        /// <returns>Queryable</returns>
        public static Queryable<T> Where<T, T2, T3, T4>(this Queryable<T> queryable, Expression<Func<T, T2, T3, T4, object>> expression)
        {
            var type = queryable.Type;
            queryable.WhereIndex = queryable.WhereIndex + 100;
            ResolveExpress re = new ResolveExpress(queryable.WhereIndex);
            re.Type = ResolveExpressType.nT;
            re.ResolveExpression(re, expression);
            queryable.Params.AddRange(re.Paras);
            queryable.Where.Add(re.SqlWhere);
            return queryable;
        }

        /// <summary>
        /// 条件筛选
        /// </summary>
        /// <typeparam name="T">表实体类型</typeparam>
        /// <typeparam name="T2">表实体类型</typeparam>
        /// <typeparam name="T3">表实体类型</typeparam>
        /// <typeparam name="T4">表实体类型</typeparam>
        /// <typeparam name="T5">表实体类型</typeparam>
        /// <param name="queryable">查询对象</param>
        /// <param name="expression">表达示条件</param>
        /// <returns>Queryable</returns>
        public static Queryable<T> Where<T, T2, T3, T4, T5>(this Queryable<T> queryable, Expression<Func<T, T2, T3, T4, T5, object>> expression)
        {
            var type = queryable.Type;
            queryable.WhereIndex = queryable.WhereIndex + 100;
            ResolveExpress re = new ResolveExpress(queryable.WhereIndex);
            re.Type = ResolveExpressType.nT;
            re.ResolveExpression(re, expression);
            queryable.Params.AddRange(re.Paras);
            queryable.Where.Add(re.SqlWhere);
            return queryable;
        }


        /// <summary>
        /// 条件筛选 ( 例如：InFieldName 为 id, inValues 值为 new string[]{"1" ,"2"} 生成的SQL就是 id in('1','2')  )
        /// </summary>
        /// <typeparam name="T">表实体类型</typeparam>
        /// <typeparam name="FieldType">In的字段类型</typeparam>
        /// <param name="queryable">查询对象</param>
        /// <param name="InFieldName">In的字段名称</param>
        /// <param name="inValues">In的值的数组集合</param>
        /// <returns>Queryable</returns>
        public static Queryable<T> In<T, FieldType>(this Queryable<T> queryable, string InFieldName, params FieldType[] inValues)
        {
            var type = queryable.Type;
            queryable.WhereIndex = queryable.WhereIndex + 100;
            ResolveExpress re = new ResolveExpress(queryable.WhereIndex);
            queryable.Where.Add(string.Format(" AND {0} IN ({1})", InFieldName, inValues.ToJoinSqlInVal()));
            return queryable;
        }

        /// <summary>
        ///  条件筛选 ( 例如：expression 为 it=>it.id,  inValues值为 new string[]{"1" ,"2"} 生成的SQL就是  id in('1','2') )
        /// </summary>
        /// <typeparam name="T">表实体类型</typeparam>
        /// <typeparam name="FieldType">In的字段类型</typeparam>
        /// <param name="queryable">查询对象</param>
        /// <param name="expression">In的字段（例如：it=>it.id）</param>
        /// <param name="inValues">In的值的数组集合</param>
        /// <returns>Queryable</returns>
        public static Queryable<T> In<T, FieldType>(this Queryable<T> queryable, Expression<Func<T, object>> expression, params FieldType[] inValues)
        {

            ResolveExpress re = new ResolveExpress();
            var InFieldName = re.GetExpressionRightField(expression);
            return In<T, FieldType>(queryable, InFieldName, inValues);
        }

        /// <summary>
        ///  条件筛选 ( 例如：expression 为 it=>it.id,  inValues值为 new string[]{"1" ,"2"} 生成的SQL就是  id in('1','2') )
        /// </summary>
        /// <typeparam name="T">表实体类型</typeparam>
        /// <typeparam name="FieldType">In的字段类型</typeparam>
        /// <param name="queryable">查询对象</param>
        /// <param name="expression">In的字段（例如：it=>it.id）</param>
        /// <param name="inValues">In的值的集合</param>
        /// <returns>Queryable</returns>
        public static Queryable<T> In<T, FieldType>(this Queryable<T> queryable, Expression<Func<T, object>> expression, List<FieldType> inValues)
        {

            ResolveExpress re = new ResolveExpress();
            var InFieldName = re.GetExpressionRightField(expression);
            return In<T, FieldType>(queryable, InFieldName, inValues);
        }

        
        /// <summary>
        /// 条件筛选 ( 例如：InFieldName 为 id,  inValues值为 new string[]{"1" ,"2"} 生成的SQL就是  id in('1','2') )
        /// </summary>
        /// <typeparam name="T">表实体类型</typeparam>
        /// <typeparam name="FieldType">In的字段类型</typeparam>
        /// <param name="queryable">查询对象</param>
        /// <param name="InFieldName">In的字段名称</param>
        /// <param name="inValues">In的值的集合</param>
        /// <returns>Queryable</returns>
        public static Queryable<T> In<T, FieldType>(this Queryable<T> queryable, string InFieldName, List<FieldType> inValues)
        {
            return In<T, FieldType>(queryable, InFieldName, inValues.ToArray());
        }


        /// <summary>
        /// 排序
        /// </summary>
        /// <typeparam name="T">表实体类型</typeparam>
        /// <param name="queryable">查询对象</param>
        /// <param name="orderFileds">排序字符串（例如：id asc,name desc) </param>
        /// <returns>Queryable</returns>
        public static Queryable<T> OrderBy<T>(this Queryable<T> queryable, string orderFileds)
        {
            queryable.OrderBy = orderFileds.ToSuperSqlFilter();
            return queryable;
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <typeparam name="T">表实体类型</typeparam>
        /// <param name="queryable">查询对象</param>
        /// <param name="expression">排序字段 it=>it.fieldName </param>
        /// <param name="type">排序类型</param>
        /// <returns>Queryable</returns>
        public static Queryable<T> OrderBy<T>(this Queryable<T> queryable, Expression<Func<T, object>> expression, OrderByType type = OrderByType.asc)
        {
            ResolveExpress re = new ResolveExpress();
            var field = re.GetExpressionRightField(expression);
            if (queryable.JoinTable.IsValuable()) {
                 field = re.GetExpressionRightFieldByNT(expression);
            }
            var pre = queryable.OrderBy.IsValuable() ? "," : "";
            queryable.OrderBy += pre + field + " " + type.ToString().ToUpper();
            return queryable;
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <typeparam name="T">表实体类型</typeparam>
        /// <typeparam name="T2">表实体类型</typeparam>
        /// <param name="queryable">查询对象</param>
        /// <param name="expression">例如 (s1,s2)=>s1.id,相当于 order by s1.id</param>
        /// <param name="type">排序类型</param>
        /// <returns>Queryable</returns>
        public static Queryable<T> OrderBy<T, T2>(this Queryable<T> queryable, Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.asc)
        {
            ResolveExpress re = new ResolveExpress();
            var field = re.GetExpressionRightFieldByNT(expression);
            var pre = queryable.OrderBy.IsValuable() ? "," : "";
            queryable.OrderBy += pre + field + " " + type.ToString().ToUpper();
            return queryable;
        }


        /// <summary>
        /// 分组
        /// </summary>
        /// <typeparam name="T">表实体类型</typeparam>
        /// <param name="queryable">查询对象</param>
        /// <param name="expression">分组字段 (例如：it=>it.fieldName)</param>
        /// <returns>Queryable</returns>
        public static Queryable<T> GroupBy<T>(this Queryable<T> queryable, Expression<Func<T, object>> expression)
        {
            ResolveExpress re = new ResolveExpress();
            var field = re.GetExpressionRightField(expression);
            var pre = queryable.GroupBy.IsValuable() ? "," : "";
            queryable.GroupBy += pre + field;
            return queryable;
        }

        /// <summary>
        /// 分组
        /// </summary>
        /// <typeparam name="T">表实体类型</typeparam>
        /// <param name="queryable">查询对象</param>
        /// <param name="groupFileds">分组字段 (例如：id,name) </param>
        /// <returns>Queryable</returns>
        public static Queryable<T> GroupBy<T>(this Queryable<T> queryable, string groupFileds)
        {
            queryable.GroupBy = groupFileds.ToSuperSqlFilter();
            return queryable;
        }


        /// <summary>
        ///  跳过序列中指定数量的元素，然后返回剩余的元素。
        /// </summary>
        /// <typeparam name="T">表实体类型</typeparam>
        /// <param name="queryable">查询对象</param>
        /// <param name="index">指定数量的索引</param>
        /// <returns>Queryable</returns>
        public static Queryable<T> Skip<T>(this Queryable<T> queryable, int index)
        {
            if (queryable.OrderBy.IsNullOrEmpty())
            {
                throw new Exception(".Skip必需使用.Order排序");
            }
            queryable.Skip = index;
            return queryable;
        }

        /// <summary>
        /// 从起始点向后取指定条数的数据
        /// </summary>
        /// <typeparam name="T">表实体类型</typeparam>
        /// <param name="queryable">查询对象</param>
        /// <param name="num">指定条数</param>
        /// <returns>Queryable</returns>
        public static Queryable<T> Take<T>(this Queryable<T> queryable, int num)
        {
            if (queryable.OrderBy.IsNullOrEmpty())
            {
                throw new Exception(".Take必需使用.OrderBy排序");
            }
            queryable.Take = num;
            return queryable;
        }


        /// <summary>
        ///  返回序列的唯一元素；如果该序列并非恰好包含一个元素，则会引发异常。
        /// </summary>
        /// <typeparam name="T">表实体类型</typeparam>
        /// <param name="queryable">查询对象</param>
        /// <returns>T</returns>
        public static T Single<T>(this  Queryable<T> queryable)
        {
            if (queryable.OrderBy.IsNullOrEmpty())
            {
                queryable.OrderBy = "GETDATE()";
            }
            queryable.Skip(0);
            queryable.Take(1);
            return queryable.ToList().Single();
        }

        /// <summary>
        ///  返回序列的唯一元素；如果该序列为空返回Default(T),序列超过1条返则抛出异常。
        /// </summary>
        /// <typeparam name="T">表实体类型</typeparam>
        /// <param name="queryable">查询对象</param>
        /// <returns>T</returns>
        public static T SingleOrDefault<T>(this  Queryable<T> queryable)
        {
            if (queryable.OrderBy.IsNullOrEmpty())
            {
                queryable.OrderBy = "GETDATE()";
            }
            queryable.Skip(0);
            queryable.Take(1);
            var reval = queryable.ToList();
            if (reval == null || reval.Count == 0)
            {
                return default(T);
            }
            return reval.Single();
        }

        /// <summary>
        ///  返回序列的唯一元素；如果该序列并非恰好包含一个元素，则会引发异常。
        /// </summary>
        /// <typeparam name="T">表实体类型</typeparam>
        /// <param name="queryable">查询对象</param>
        /// <param name="expression">表达示条件</param>
        /// <returns>T</returns>
        public static T Single<T>(this  Queryable<T> queryable, Expression<Func<T, bool>> expression)
        {
            var type = queryable.Type;
            queryable.WhereIndex = queryable.WhereIndex + 100;
            ResolveExpress re = new ResolveExpress(queryable.WhereIndex);
            re.ResolveExpression(re, expression);
            queryable.Where.Add(re.SqlWhere);
            queryable.Params.AddRange(re.Paras);
            return queryable.ToList().Single();
        }

        /// <summary>
        ///  返回序列的唯一元素；如果该序列并非恰好包含一个元素，否则返回null。
        /// </summary>
        /// <typeparam name="T">表实体类型</typeparam>
        /// <param name="queryable">查询对象</param>
        /// <param name="expression">表达示条件</param>
        /// <returns>T</returns>
        public static T SingleOrDefault<T>(this  Queryable<T> queryable, Expression<Func<T, bool>> expression)
        {
            var type = queryable.Type;
            queryable.WhereIndex = queryable.WhereIndex + 100;
            ResolveExpress re = new ResolveExpress(queryable.WhereIndex);
            re.ResolveExpression(re, expression);
            queryable.Where.Add(re.SqlWhere);
            queryable.Params.AddRange(re.Paras);
            return queryable.ToList().SingleOrDefault();
        }

        /// <summary>
        ///  返回序列中的第一个元素。
        /// </summary>
        /// <typeparam name="T">表实体类型</typeparam>
        /// <param name="queryable">查询对象</param>
        /// <returns>T</returns>
        public static T First<T>(this  Queryable<T> queryable)
        {
            if (queryable.OrderBy.IsNullOrEmpty())
            {
                queryable.OrderBy = "GETDATE()";
            }
            queryable.Skip(0);
            queryable.Take(1);
            var reval = queryable.ToList();
            return reval.First();
        }

        /// <summary>
        ///   返回序列中的第一个元素,如果序列为NULL返回default(T)
        /// </summary>
        /// <typeparam name="T">表实体类型</typeparam>
        /// <param name="queryable">查询对象</param>
        /// <returns>T</returns>
        public static T FirstOrDefault<T>(this  Queryable<T> queryable)
        {
            if (queryable.OrderBy.IsNullOrEmpty())
            {
                queryable.OrderBy = "GETDATE()";
            }
            queryable.Skip(0);
            queryable.Take(1);
            var reval = queryable.ToList();
            if (reval == null || reval.Count == 0)
            {
                return default(T);
            }
            return reval.First();
        }

        /// <summary>
        /// 返回序列中的第一个元素。
        /// </summary>
        /// <typeparam name="T">表实体类型</typeparam>
        /// <param name="queryable">查询对象</param>
        /// <param name="expression">表达示条件</param>
        /// <returns>T</returns>
        public static T First<T>(this  Queryable<T> queryable, Expression<Func<T, bool>> expression)
        {

            var type = queryable.Type;
            queryable.WhereIndex = queryable.WhereIndex + 100;
            ResolveExpress re = new ResolveExpress(queryable.WhereIndex);
            re.ResolveExpression(re, expression);
            queryable.Where.Add(re.SqlWhere);
            queryable.Params.AddRange(re.Paras);
            return First<T>(queryable);
        }

        /// <summary>
        /// 返回序列中的第一个元素,如果序列为NULL返回default(T)
        /// </summary>
        /// <typeparam name="T">表实体类型</typeparam>
        /// <param name="queryable">查询对象</param>
        /// <param name="expression">表达示条件</param>
        /// <returns>T</returns>
        public static T FirstOrDefault<T>(this  Queryable<T> queryable, Expression<Func<T, bool>> expression)
        {

            var type = queryable.Type;
            queryable.WhereIndex = queryable.WhereIndex + 100;
            ResolveExpress re = new ResolveExpress(queryable.WhereIndex);
            re.ResolveExpression(re, expression);
            queryable.Where.Add(re.SqlWhere);
            queryable.Params.AddRange(re.Paras);
            return FirstOrDefault<T>(queryable);
        }


        /// <summary>
        ///  确定序列是否包含任何元素。
        /// </summary>
        /// <typeparam name="T">表实体类型</typeparam>
        /// <param name="queryable">查询对象</param>
        /// <param name="expression">表达示条件</param>
        /// <returns>count>0返回true</returns>
        public static bool Any<T>(this  Queryable<T> queryable, Expression<Func<T, bool>> expression)
        {
            var type = queryable.Type;
            queryable.WhereIndex = queryable.WhereIndex + 100;
            ResolveExpress re = new ResolveExpress(queryable.WhereIndex);
            re.ResolveExpression(re, expression);
            queryable.Where.Add(re.SqlWhere);
            queryable.Params.AddRange(re.Paras);
            return queryable.Count() > 0;
        }

        /// <summary>
        ///  确定序列是否包含任何元素
        /// </summary>
        /// <typeparam name="T">表实体类型</typeparam>
        /// <param name="queryable">查询对象</param>
        /// <returns>count>0返回true</returns>
        public static bool Any<T>(this  Queryable<T> queryable)
        {
            return queryable.Count() > 0;
        }

        /// <summary>
        /// 将源数据对象转换到新对象中
        /// </summary>
        /// <typeparam name="T">原数据实体类型</typeparam>
        /// <typeparam name="T2">原数据实体类型</typeparam>
        /// <typeparam name="TResult">返回值的新实体类型</typeparam>
        /// <param name="queryable">查询对象</param>
        /// <param name="expression">给新实体赋值的表达示</param>
        /// <returns>Queryable</returns>
        public static Queryable<TResult> Select<T, T2, TResult>(this Queryable<T> queryable, Expression<Func<T, T2, TResult>> expression)
        {
            var expStr = expression.ToString();
            var type = typeof(T);
            Queryable<TResult> reval = new Queryable<TResult>()
            {
                DB = queryable.DB,
                OrderBy = queryable.OrderBy,
                Params = queryable.Params,
                Skip = queryable.Skip,
                Take = queryable.Take,
                Where = queryable.Where,
                TableName = type.Name,
                GroupBy = queryable.GroupBy,
                JoinTable=queryable.JoinTable
            };
            reval.Select = Regex.Match(expStr, @"(?<=\{).*?(?=\})").Value;
            if (reval.Select.IsNullOrEmpty())
            {
                reval.Select = Regex.Match(expStr, @"c =>.*?\((.+)\)").Groups[1].Value;
            }
            //reval.Select = Regex.Replace(reval.Select, @"(?<=\=).*?\.", "");
            return reval;
        }

        /// <summary>
        /// 将源数据对象转换到新对象中
        /// </summary>
        /// <typeparam name="T">原数据实体类型</typeparam>
        /// <typeparam name="T2">原数据实体类型</typeparam>
        /// <typeparam name="T3">原数据实体类型</typeparam>
        /// <typeparam name="TResult">返回值的新实体类型</typeparam>
        /// <param name="queryable">查询对象</param>
        /// <param name="expression">给新实体赋值的表达示</param>
        /// <returns>Queryable</returns>
        public static Queryable<TResult> Select<T, T2 ,T3, TResult>(this Queryable<T> queryable, Expression<Func<T, T2,T3, TResult>> expression)
        {
            var expStr = expression.ToString();
            var type = typeof(T);
            Queryable<TResult> reval = new Queryable<TResult>()
            {
                DB = queryable.DB,
                OrderBy = queryable.OrderBy,
                Params = queryable.Params,
                Skip = queryable.Skip,
                Take = queryable.Take,
                Where = queryable.Where,
                TableName = type.Name,
                GroupBy = queryable.GroupBy,
                JoinTable = queryable.JoinTable
            };
            reval.Select = Regex.Match(expStr, @"(?<=\{).*?(?=\})").Value;
            if (reval.Select.IsNullOrEmpty())
            {
                reval.Select = Regex.Match(expStr, @"c =>.*?\((.+)\)").Groups[1].Value;
            }
            //reval.Select = Regex.Replace(reval.Select, @"(?<=\=).*?\.", "");
            return reval;
        }

        /// <summary>
        /// 将源数据对象转换到新对象中
        /// </summary>
        /// <typeparam name="T">原数据实体类型</typeparam>
        /// <typeparam name="T2">原数据实体类型</typeparam>
        /// <typeparam name="T3">原数据实体类型</typeparam>
        /// <typeparam name="T4">原数据实体类型</typeparam>
        /// <typeparam name="TResult">返回值的新实体类型</typeparam>
        /// <param name="queryable">查询对象</param>
        /// <param name="expression">给新实体赋值的表达示</param>
        /// <returns>Queryable</returns>
        public static Queryable<TResult> Select<T, T2, T3,T4, TResult>(this Queryable<T> queryable, Expression<Func<T, T2, T3,T4, TResult>> expression)
        {
            var expStr = expression.ToString();
            var type = typeof(T);
            Queryable<TResult> reval = new Queryable<TResult>()
            {
                DB = queryable.DB,
                OrderBy = queryable.OrderBy,
                Params = queryable.Params,
                Skip = queryable.Skip,
                Take = queryable.Take,
                Where = queryable.Where,
                TableName = type.Name,
                GroupBy = queryable.GroupBy,
                JoinTable = queryable.JoinTable
            };
            reval.Select = Regex.Match(expStr, @"(?<=\{).*?(?=\})").Value;
            if (reval.Select.IsNullOrEmpty())
            {
                reval.Select = Regex.Match(expStr, @"c =>.*?\((.+)\)").Groups[1].Value;
            }
            //reval.Select = Regex.Replace(reval.Select, @"(?<=\=).*?\.", "");
            return reval;
        }

        /// <summary>
        /// 将源数据对象转换到新对象中
        /// </summary>
        /// <typeparam name="T">原数据实体类型</typeparam>
        /// <typeparam name="T2">原数据实体类型</typeparam>
        /// <typeparam name="T3">原数据实体类型</typeparam>
        /// <typeparam name="T4">原数据实体类型</typeparam>
        /// <typeparam name="T5">原数据实体类型</typeparam>
        /// <typeparam name="TResult">返回值的新实体类型</typeparam>
        /// <param name="queryable">查询对象</param>
        /// <param name="expression">给新实体赋值的表达示</param>
        /// <returns>Queryable</returns>
        public static Queryable<TResult> Select<T, T2, T3, T4,T5, TResult>(this Queryable<T> queryable, Expression<Func<T, T2, T3, T4,T5, TResult>> expression)
        {
            var expStr = expression.ToString();
            var type = typeof(T);
            Queryable<TResult> reval = new Queryable<TResult>()
            {
                DB = queryable.DB,
                OrderBy = queryable.OrderBy,
                Params = queryable.Params,
                Skip = queryable.Skip,
                Take = queryable.Take,
                Where = queryable.Where,
                TableName = type.Name,
                GroupBy = queryable.GroupBy,
                JoinTable = queryable.JoinTable
            };
            reval.Select = Regex.Match(expStr, @"(?<=\{).*?(?=\})").Value;
            if (reval.Select.IsNullOrEmpty())
            {
                reval.Select = Regex.Match(expStr, @"c =>.*?\((.+)\)").Groups[1].Value;
            }
            //reval.Select = Regex.Replace(reval.Select, @"(?<=\=).*?\.", "");
            return reval;
        }

        /// <summary>
        /// 将源数据对象转换到新对象中
        /// </summary>
        /// <typeparam name="TSource">原数据实体类型</typeparam>
        /// <typeparam name="TResult">返回值的新实体类型</typeparam>
        /// <param name="queryable">查询对象</param>
        /// <param name="expression">给新实体赋值的表达示</param>
        /// <returns>Queryable</returns>
        public static Queryable<TResult> Select<TSource, TResult>(this Queryable<TSource> queryable, Expression<Func<TSource, TResult>> expression)
        {
            var type = typeof(TSource);
            var expStr = expression.ToString();
            Queryable<TResult> reval = new Queryable<TResult>()
            {
                DB = queryable.DB,
                OrderBy = queryable.OrderBy,
                Params = queryable.Params,
                Skip = queryable.Skip,
                Take = queryable.Take,
                Where = queryable.Where,
                TableName = type.Name,
                GroupBy = queryable.GroupBy
            };
            reval.Select = Regex.Match(expStr, @"(?<=\{).*?(?=\})").Value;
            if (reval.Select.IsNullOrEmpty())
            {
                reval.Select = Regex.Match(expStr, @"c =>.*?\((.+)\)").Groups[1].Value;
            }
            if (queryable.JoinTable.IsValuable() == false)
            {
                reval.Select = Regex.Replace(reval.Select, @"(?<=\=).*?\.", "");
            }
            else {
                reval.JoinTable = queryable.JoinTable;
            }
            return reval;
        }

        /// <summary>
        /// 将源数据对象转换到新对象中
        /// </summary>
        /// <typeparam name="TSource">原数据实体类型</typeparam>
        /// <typeparam name="TResult">返回值的新实体类型</typeparam>
        /// <param name="queryable">查询对象</param>
        /// <param name="select">查询字符串（例如 id,name）</param>
        /// <returns></returns>
        public static Queryable<TResult> Select<TSource, TResult>(this Queryable<TSource> queryable, string select)
        {
            var type = typeof(TSource);
            Queryable<TResult> reval = new Queryable<TResult>()
            {
                DB = queryable.DB,
                OrderBy = queryable.OrderBy,
                Params = queryable.Params,
                Skip = queryable.Skip,
                Take = queryable.Take,
                Where = queryable.Where,
                TableName = type.Name,
                GroupBy = queryable.GroupBy,
                Select = select
            };
            if (queryable.JoinTable.IsValuable())
            {
                reval.JoinTable = queryable.JoinTable;
            }
            return reval;
        }

        /// <summary>
        /// 将select的字段映射到T对象
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="queryable">查询对象</param>
        /// <param name="select">查询字符串（例如 id,name）</param>
        /// <returns></returns>
        public static Queryable<T> Select<T>(this Queryable<T> queryable, string select)
        {
            queryable.Select = select;
            return queryable;
        }


        /// <summary>
        /// 获取序列总记录数
        /// </summary>
        /// <param name="queryable">查询对象</param>
        /// <returns>int</returns>
        public static int Count<T>(this Queryable<T> queryable)
        {
            StringBuilder sbSql = new StringBuilder();
            string joinInfo = string.Join(" ", queryable.JoinTable);
            string withNoLock = queryable.DB.IsNoLock ? "WITH(NOLOCK)" : null;
            var tableName = queryable.TName;
            if (queryable.TableName.IsValuable())
            {
                tableName = queryable.TableName;
            }
            sbSql.AppendFormat("SELECT COUNT({3})  FROM [{0}] {1} "+joinInfo+" WHERE 1=1 {2} {4} ", tableName, withNoLock, string.Join("", queryable.Where), "1", queryable.GroupBy.GetGroupBy());
            var count = queryable.DB.GetInt(sbSql.ToString(), queryable.Params.ToArray());
            return count;
        }


        /// <summary>
        /// 获取最大值
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="maxField">列</param>
        /// <returns></returns>
        public static TResult Max<TSource, TResult>(this Queryable<TSource> queryable, string maxField)
        {
            StringBuilder sbSql = new StringBuilder();
            string withNoLock = queryable.DB.IsNoLock ? "WITH(NOLOCK)" : null;
            sbSql.AppendFormat("SELECT MAX({3})  FROM [{0}] {1} WHERE 1=1 {2} {4} ", queryable.TName, withNoLock, string.Join("", queryable.Where), maxField, queryable.GroupBy.GetGroupBy());
            var objValue = queryable.DB.GetScalar(sbSql.ToString(), queryable.Params.ToArray());
            var reval = Convert.ChangeType(objValue, typeof(TResult));
            return (TResult)reval;
        }

        /// <summary>
        /// 获取最大值
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="maxField">列</param>
        /// <returns></returns>
        public static object Max<T>(this Queryable<T> queryable, Expression<Func<T, object>> expression)
        {

            ResolveExpress re = new ResolveExpress();
            var minField = re.GetExpressionRightField(expression);
            return Max<T, object>(queryable, minField);
        }

        /// <summary>
        /// 获取最小值
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="minField">列</param>
        /// <returns></returns>
        public static TResult Min<TSource, TResult>(this Queryable<TSource> queryable, string minField)
        {
            StringBuilder sbSql = new StringBuilder();
            string withNoLock = queryable.DB.IsNoLock ? "WITH(NOLOCK)" : null;
            sbSql.AppendFormat("SELECT MIN({3})  FROM [{0}] {1} WHERE 1=1 {2} {4} ", queryable.TName, withNoLock, string.Join("", queryable.Where), minField, queryable.GroupBy.GetGroupBy());
            var objValue = queryable.DB.GetScalar(sbSql.ToString(), queryable.Params.ToArray());
            var reval = Convert.ChangeType(objValue, typeof(TResult));
            return (TResult)reval;
        }

        /// <summary>
        /// 获取最小值
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="minField">列</param>
        /// <returns></returns>
        public static object Min<T>(this Queryable<T> queryable, Expression<Func<T, object>> expression)
        {
            ResolveExpress re = new ResolveExpress();
            var minField = re.GetExpressionRightField(expression);
            return Min<T, object>(queryable, minField);
        }


        /// <summary>
        /// 将Queryable转换为List《T》集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <returns></returns>
        public static List<T> ToList<T>(this Queryable<T> queryable)
        {
            StringBuilder sbSql = SqlSugarTool.GetQueryableSql<T>(queryable);
            var reader = queryable.DB.GetReader(sbSql.ToString(), queryable.Params.ToArray());
            var reval = SqlSugarTool.DataReaderToList<T>(typeof(T), reader, queryable.Select.GetSelectFiles());
            queryable = null;
            sbSql = null;
            return reval;


        }

        /// <summary>
        /// 将Queryable转换为Json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <returns></returns>
        public static string ToJson<T>(this Queryable<T> queryable)
        {
            return JsonConverter.DataTableToJson(ToDataTable<T>(queryable), queryable.DB.SerializerDateFormat);
        }
        /// <summary>
        /// 返回Sql和SqlParameter []
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <returns></returns>
        public static KeyValuePair<string, string[]> ToSql<T>(this Queryable<T> queryable)
        {
            var sql=SqlSugarTool.GetQueryableSql<T>(queryable).ToString();
            var pars = queryable.Params.ToArray();
            var reval = new Dictionary<string, string[]>();
            reval.Add(sql, pars.Select(it =>it.ParameterName+":"+it.Value).ToArray());
            return reval.First();
        }

        /// <summary>
        /// 将Queryable转换为Dynamic
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <returns></returns>
        public static dynamic ToDynamic<T>(this Queryable<T> queryable)
        {
            return JsonConverter.ConvertJson(ToJson<T>(queryable));
        }

        /// <summary>
        /// 将Queryable转换为DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(this Queryable<T> queryable)
        {
            StringBuilder sbSql = SqlSugarTool.GetQueryableSql<T>(queryable);
            var dataTable = queryable.DB.GetDataTable(sbSql.ToString(), queryable.Params.ToArray());
            queryable = null;
            sbSql = null;
            return dataTable;
        }

        /// <summary>
        /// 将Queryable转换为分页后的List&lt;T&gt;集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">每页显示数量</param>
        /// <returns></returns>
        public static List<T> ToPageList<T>(this Queryable<T> queryable, int pageIndex, int pageSize)
        {
            if (queryable.OrderBy.IsNullOrEmpty())
            {
                throw new Exception("分页必需使用.Order排序");
            }
            if (pageIndex == 0)
                pageIndex = 1;
            queryable.Skip = (pageIndex - 1) * pageSize;
            queryable.Take = pageSize;
            return queryable.ToList();
        }

        /// <summary>
        /// 联表查询
        /// </summary>
        /// <typeparam name="T">第一个表的对象</typeparam>
        /// <typeparam name="T2">联接的表对象</typeparam>
        /// <param name="queryable"></param>
        /// <param name="expression">表达示</param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Queryable<T> JoinTable<T, T2>(this Queryable<T> queryable, Expression<Func<T, T2, object>> expression, JoinType type = JoinType.LEFT)
        {

            ResolveExpress re = new ResolveExpress();
            queryable.WhereIndex = queryable.WhereIndex + 100;
            re.Type = ResolveExpressType.nT;
            var exLeftStr = Regex.Match(expression.ToString(), @"\((.+?)\).+").Groups[1].Value;
            var exLeftArray = exLeftStr.Split(',');
            var shortName1 = exLeftArray.First();
            var shortName2 = exLeftArray.Last();
            re.ResolveExpression(re, expression);
            string joinTableName = type.ToString();
            string joinStr = string.Format(" {0} JOIN {1} {2} ON {3}  ",
                /*0*/queryable.JoinTable.Count == 0 ? (" " + shortName1 + " " + joinTableName) : joinTableName.ToString(),
                /*1*/typeof(T2).Name,
                /*2*/shortName2,
                /*3*/re.SqlWhere.Trim().TrimStart('A').TrimStart('N').TrimStart('D')
                );
            queryable.JoinTable.Add(joinStr);
            queryable.Params.AddRange(re.Paras);
            return queryable;
        }


        /// <summary>
        /// 联表查询
        /// </summary>
        /// <typeparam name="T">第一个表的对象</typeparam>
        /// <typeparam name="T2">联接表的对象</typeparam>
        /// <param name="queryable"></param>
        /// <param name="expression">条件表达示</param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Queryable<T> JoinTable<T, T2,T3>(this Queryable<T> queryable, Expression<Func<T, T2,T3, object>> expression, JoinType type = JoinType.LEFT)
        {

            ResolveExpress re = new ResolveExpress();
            queryable.WhereIndex = queryable.WhereIndex + 100;
            re.Type = ResolveExpressType.nT;
            var exLeftStr = Regex.Match(expression.ToString(), @"\((.+?)\).+").Groups[1].Value;
            var exLeftArray = exLeftStr.Split(',');
            var shortName1 = exLeftArray[1];
            var shortName2 = exLeftArray[2];
            re.ResolveExpression(re, expression);
            string joinTableName = type.ToString();
            string joinStr = string.Format(" {0} JOIN {1} {2} ON {3}  ",
                /*0*/queryable.JoinTable.Count == 0 ? (" " + shortName1 + " " + joinTableName) : joinTableName.ToString(),
                /*1*/typeof(T3).Name,
                /*2*/shortName2,
                /*3*/re.SqlWhere.Trim().TrimStart('A').TrimStart('N').TrimStart('D')
                );
            queryable.JoinTable.Add(joinStr);
            queryable.Params.AddRange(re.Paras);
            return queryable;
        }

        /// <summary>
        /// 联表查询根据字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="tableName">表名（可是以表或也可以是SQL语句加括号）</param>
        /// <param name="shortName">表名简写</param>
        /// <param name="onWhere">on后面的条件</param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Queryable<T> JoinTable<T>(this Queryable<T> queryable, string tableName,string shortName,string onWhere,object whereObj, JoinType type = JoinType.LEFT)
        {

            queryable.WhereIndex = queryable.WhereIndex + 100;;
            string joinType = type.ToString();
            string joinStr = string.Format(" {0} JOIN {1} {2} ON {3}  ",
                /*0*/joinType,
                /*1*/tableName,
                /*2*/shortName,
                /*3*/onWhere
                );
            queryable.JoinTable.Add(joinStr);
            if (whereObj != null)
            {
                queryable.Params.AddRange(SqlSugarTool.GetParameters(whereObj));
            }
            return queryable;
        }

    }
}
