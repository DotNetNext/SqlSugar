using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace SqlSugar
{
    //局部类 解析字段名
    internal partial class ResolveExpress
    {
        /// <summary>
        /// 获取拉姆达表达式的字段值
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="db">数据库访问对象</param>
        /// <returns></returns>
        public string GetExpressionRightField(Expression exp, SqlSugarClient db)
        {
            DB = db;
            string reval = "";
            LambdaExpression lambda = exp as LambdaExpression;
            var isConvet = lambda.Body.NodeType.IsIn(ExpressionType.Convert);
            var isMember = lambda.Body.NodeType.IsIn(ExpressionType.MemberAccess);
            if (!isConvet && !isMember)
            {
                throw new SqlSugarException(FileldErrorMessage);
            }
            try
            {
                if (isConvet)
                {
                    var memberExpr =((UnaryExpression)lambda.Body).Operand as MemberExpression;
                    reval= memberExpr.Member.Name;
                }
                else//isMember
                {
                    reval= (lambda.Body as MemberExpression).Member.Name;
                }
            }
            catch (Exception)
            {
                throw new SqlSugarException(FileldErrorMessage);
            }
            if (DB != null && DB.IsEnableAttributeMapping && DB._mappingColumns.IsValuable())
            {
                if (DB._mappingColumns.Any(it => it.Key == reval))
                {
                    var dbName = DB._mappingColumns.Single(it => it.Key == reval).Value;
                    return dbName;
                }
            }
            return reval;
        }

        /// <summary>
        /// 获取拉姆达表达式的字段值多个T模式
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="db">数据库访问对象</param>
        /// <returns></returns>
        public string GetExpressionRightFieldByNT(Expression exp, SqlSugarClient db)
        {
            DB = db;
            string reval = "";
            LambdaExpression lambda = exp as LambdaExpression;
            var isConvet = lambda.Body.NodeType.IsIn(ExpressionType.Convert);
            var isMember = lambda.Body.NodeType.IsIn(ExpressionType.MemberAccess);
            if (!isConvet && !isMember)
            {
                throw new SqlSugarException(FileldErrorMessage);
            }
            try
            {
                if (isConvet)
                {
                    var memberExpr = ((UnaryExpression)lambda.Body).Operand as MemberExpression;
                    reval= memberExpr.ToString();
                }
                else//isMember
                {
                    reval= lambda.Body.ToString();
                }
            }
            catch (Exception)
            {
                 throw new SqlSugarException(FileldErrorMessage);
            }
            if (DB != null && DB.IsEnableAttributeMapping && DB._mappingColumns.IsValuable())
            {
                if (DB._mappingColumns.Any(it => reval.EndsWith("."+it.Key)))
                {
                    var preName = reval.Split('.').First();
                    var dbName = DB._mappingColumns.Single(it => reval.EndsWith("." + it.Key)).Value;
                    return preName+"."+dbName;
                }
            }
            return reval;
        }
    }
}
