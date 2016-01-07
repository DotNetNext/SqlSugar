using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace SqlSugar
{
    /// <summary>
    /// ** 描述：Sqlable扩展函数
    /// ** 创始时间：2015-7-13
    /// ** 修改时间：-
    /// ** 作者：sunkaixuan
    /// ** 使用说明：
    /// </summary>
    public static partial class SqlableExtensions
    {
        /// <summary>
        /// Form
        /// </summary>
        /// <param name="sqlable"></param>
        /// <param name="modelObj">表名</param>
        /// <param name="shortName">表名简写</param>
        /// <returns></returns>
        public static Sqlable Form(this Sqlable sqlable, object tableName, string shortName)
        {
            sqlable.Sql = new StringBuilder();
            sqlable.Sql.AppendFormat(" FROM {0} {1} {2} ", tableName, shortName, sqlable.DB.IsNoLock.GetLockString());
            return sqlable;
        }
        /// <summary>
        /// Form
        /// </summary>
        /// <param name="sqlable"></param>
        /// <param name="modelObj">表名</param>
        /// <param name="shortName">表名简写</param>
        /// <returns></returns>
        public static Sqlable Form<T>(this Sqlable sqlable,string shortName)
        {
            sqlable.Sql = new StringBuilder();
            sqlable.Sql.AppendFormat(" FROM {0} {1} {2} ", typeof(T).Name, shortName, sqlable.DB.IsNoLock.GetLockString());
            return sqlable;
        }

        /// <summary>
        /// Join
        /// </summary>
        /// <param name="sqlable"></param>
        /// <param name="leftFiled">join左边连接字段</param>
        /// <param name="RightFiled">join右边连接字段</param>
        /// <param name="type">join类型</param>
        /// <returns></returns>
        public static Sqlable Join(this Sqlable sqlable, object tableName, string shortName, string leftFiled, string RightFiled, JoinType type)
        {
            Check.ArgumentNullException(sqlable.Sql, "语法错误，正确用法：sqlable.Form(“table”).Join");
            sqlable.Sql.AppendFormat(" {0} JOIN {1} {2}  {3} ON  {4} = {5} ", type.ToString(), tableName, shortName, sqlable.DB.IsNoLock.GetLockString(), leftFiled, RightFiled);
            return sqlable;
        }
        /// <summary>
        /// Join
        /// </summary>
        /// <param name="sqlable"></param>
        /// <param name="leftFiled">join左边连接字段</param>
        /// <param name="RightFiled">join右边连接字段</param>
        /// <param name="type">join类型</param>
        /// <returns></returns>
        public static Sqlable Join<T>(this Sqlable sqlable,string shortName, string leftFiled, string RightFiled, JoinType type)
        {
            Check.ArgumentNullException(sqlable.Sql, "语法错误，正确用法：sqlable.Form(“table”).Join");
            sqlable.Sql.AppendFormat(" {0} JOIN {1} {2}  {3} ON  {4} = {5} ", type.ToString(), typeof(T).Name, shortName, sqlable.DB.IsNoLock.GetLockString(), leftFiled, RightFiled);
            return sqlable;
        }

        /// <summary>
        /// Where
        /// </summary>
        /// <param name="sqlable"></param>
        /// <param name="where">查询条件、开头无需写 AND或者WHERE</param>
        /// <returns></returns>
        public static Sqlable Where(this Sqlable sqlable, string where)
        {
            if (where.IsValuable())
            {
                sqlable.Where.Add(string.Format(" AND {0} ", where));
            }
            return sqlable;
        }

        /// <summary>
        /// OrderBy
        /// </summary>
        /// <param name="sqlable"></param>
        /// <param name="orderBy">排序字段，可以多个</param>
        /// <returns></returns>
        public static Sqlable OrderBy(this Sqlable sqlable, string orderBy)
        {
            sqlable.OrderBy = orderBy;
            return sqlable;
        }

        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="sqlable"></param>
        /// <param name="applySql">apply主体内容</param>
        /// <param name="shotName">apply简写</param>
        /// <param name="type">Apply类型</param>
        /// <returns></returns>
        public static Sqlable Apply(this Sqlable sqlable, string applySql, string shotName, ApplyType type)
        {
            Check.ArgumentNullException(sqlable.Sql, "语法错误，正确用法：sqlable.Form(“table”).Join");
            sqlable.Sql.AppendFormat(" {0} APPLY ({1}) {2}} ", type.ToString(), applySql, shotName);
            return sqlable;
        }

        /// <summary>
        /// GroupBy
        /// </summary>
        /// <param name="sqlable"></param>
        /// <param name="groupBy">GroupBy字段，可以多个</param>
        /// <returns></returns>
        public static Sqlable GroupBy(this Sqlable sqlable, string groupBy)
        {
            sqlable.GroupBy = groupBy;
            return sqlable;
        }

        /// <summary>
        /// 设置查询列执行查询，并且将结果集转成List《T》
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlable"></param>
        /// <param name="fileds">查询列</param>
        /// <param name="whereObj">SQL参数,例如:new{id=1,name="张三"}</param>
        /// <returns></returns>
        public static List<T> SelectToList<T>(this Sqlable sqlable, string fileds, object whereObj = null) where T : class
        {
            StringBuilder sbSql = new StringBuilder(sqlable.Sql.ToString());
            try
            {
                Check.ArgumentNullException(sqlable.Sql, "语法错误，SelectToSql必需要在.Form后面使用");
                sbSql.Insert(0, string.Format("SELECT {0} ", fileds));
                sbSql.Append(" WHERE 1=1").Append(string.Join(" ", sqlable.Where));
                sbSql.Append(sqlable.OrderBy);
                sbSql.Append(sqlable.GroupBy);
                var sqlParams = SqlSugarTool.GetParameters(whereObj);
                var reval = SqlSugarTool.DataReaderToList<T>(typeof(T), sqlable.DB.GetReader(sbSql.ToString(), sqlParams),fileds);
                return reval;
            }
            catch (Exception ex)
            {
                Check.Exception(true, "sql:{0} \r\n message:{1}", sbSql.ToString(), ex.Message);
                throw;
            }
            finally
            {
                sqlable = null;
                sbSql = null;
            }
        }


        /// <summary>
        /// 设置查询列执行查询，并且将结果集转成DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlable"></param>
        /// <param name="fileds">查询列</param>
        /// <param name="whereObj">SQL参数,例如:new{id=1,name="张三"}</param>
        /// <returns></returns>
        public static DataTable SelectToDataTable(this Sqlable sqlable, string fileds, object whereObj = null) 
        {
            StringBuilder sbSql = new StringBuilder(sqlable.Sql.ToString());
            try
            {
                Check.ArgumentNullException(sqlable.Sql, "语法错误，SelectToSql必需要在.Form后面使用");
                sbSql.Insert(0, string.Format("SELECT {0} ", fileds));
                sbSql.Append(" WHERE 1=1").Append(string.Join(" ", sqlable.Where));
                sbSql.Append(sqlable.OrderBy);
                sbSql.Append(sqlable.GroupBy);
                var sqlParams = SqlSugarTool.GetParameters(whereObj);
                var reval = sqlable.DB.GetDataTable(sbSql.ToString(), sqlParams);
                return reval;
            }
            catch (Exception ex)
            {
                Check.Exception(true, "sql:{0} \r\n message:{1}", sbSql.ToString(), ex.Message);
                throw;
            }
            finally
            {
                sqlable = null;
                sbSql = null;
            }
        }

        /// <summary>
        /// 生成查询结果对应的实体类字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlable"></param>
        /// <param name="fileds">查询列</param>
        /// <param name="whereObj">SQL参数,例如:new{id=1,name="张三"}</param>
        /// <returns></returns>
        public static string ToClass(this Sqlable sqlable, string fileds, object whereObj = null)
        {
            var cg = new ClassGenerating();
            var dt = SelectToDataTable(sqlable, fileds, whereObj);
            return cg.DataTableToClass(dt, "TableName");
        }

        /// <summary>
        /// 反回记录数
        /// </summary>
        /// <param name="sqlable"></param>
        /// <returns></returns>
        public static int Count(this Sqlable sqlable,object whereObj=null)
        {
            StringBuilder sbSql = new StringBuilder(sqlable.Sql.ToString());
            try
            {
                Check.ArgumentNullException(sqlable.Sql, "语法错误，Count必需要在.Form后面使用");
                sbSql.Insert(0, string.Format("SELECT COUNT(1) "));
                sbSql.Append(" WHERE 1=1").Append(string.Join(" ", sqlable.Where));
                sbSql.Append(sqlable.OrderBy);
                sbSql.Append(sqlable.GroupBy);
                var sqlParams = SqlSugarTool.GetParameters(whereObj);
                return sqlable.DB.GetInt(sbSql.ToString(), sqlParams);
            }
            catch (Exception ex)
            {
                Check.Exception(true, "sql:{0} \r\n message:{1}", sbSql.ToString(), ex.Message);
                throw;
            }
            finally
            {
                sqlable = null;
                sbSql = null;
            }
        }

        /// <summary>
        /// 设置查询列和分页参数执行查询，并且将结果集转成List《T》
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlable"></param>
        /// <param name="fileds">查询列</param>
        /// <param name="orderByFiled">Order By字段，可以多个</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页显示数量</param>
        /// <param name="whereObj">SQL参数,例如:new{id=1,name="张三"}</param>
        /// <returns></returns>
        public static List<T> SelectToPageList<T>(this Sqlable sqlable, string fileds, string orderByFiled, int pageIndex, int pageSize, object whereObj = null) where T : class
        {
            StringBuilder sbSql = new StringBuilder(sqlable.Sql.ToString());
            try
            {
                if (pageIndex == 0) pageIndex = 1;
                Check.ArgumentNullException(sqlable.Sql, "语法错误，SelectToSql必需要在.Form后面使用");
                sbSql.Insert(0, string.Format("SELECT {0},row_index=ROW_NUMBER() OVER(ORDER BY {1} )", fileds, orderByFiled));
                sbSql.Append(" WHERE 1=1 ").Append(string.Join(" ", sqlable.Where));
                sbSql.Append(sqlable.OrderBy);
                sbSql.Append(sqlable.GroupBy);
                int skip = (pageIndex - 1) * pageSize + 1;
                int take = pageSize;
                sbSql.Insert(0, "SELECT * FROM ( ");
                sbSql.AppendFormat(") t WHERE  t.row_index BETWEEN {0}  AND {1}   ", skip, skip + take - 1);
                var sqlParams = SqlSugarTool.GetParameters(whereObj);
                var reval = SqlSugarTool.DataReaderToList<T>(typeof(T), sqlable.DB.GetReader(sbSql.ToString(), sqlParams),fileds);
                return reval;
            }
            catch (Exception ex)
            {
                Check.Exception(true, "sql:{0} \r\n message:{1}", sbSql.ToString(), ex.Message);
                throw;
            }
            finally
            {
                sbSql = null;
                sqlable = null;
            }
        }




    }
}
