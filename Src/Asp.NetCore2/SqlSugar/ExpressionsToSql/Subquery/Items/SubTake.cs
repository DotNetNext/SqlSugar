using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar
{
    public class SubTake : ISubOperation
    {
        public bool HasWhere
        {
            get; set;
        }

        public ExpressionContext Context
        {
            get; set;
        }

        public Expression Expression
        {
            get; set;
        }

        public string Name
        {
            get
            {
                return "Take";
            }
        }

        public int Sort
        {
            get
            {
                if (this.Context is SqlServerExpressionContext || this.Context.GetType().Name.Contains("Access"))
                {
                    return 150;
                }
                else if (this.Context is OracleExpressionContext)
                {

                    return 401;
                }
                else
                {
                    return 490;
                }
            }
        }


        public string GetValue(Expression expression)
        {
            var numExp = (expression as MethodCallExpression).Arguments[0];
            var num =1;
            if (ExpressionTool.GetParameters(numExp).Any()) 
            { 
                var copyContext = this.Context.GetCopyContextWithMapping();
                copyContext.IsSingle = false;
                copyContext.Resolve(numExp, ResolveExpressType.WhereMultiple);
                copyContext.Result.GetString();
            }
            else 
            {
                num = ExpressionTool.DynamicInvoke(numExp).ObjToInt();
            }
            var take = (expression as MethodCallExpression); 
            if (this.Context is SqlServerExpressionContext || this.Context.GetType().Name.Contains("Access"))
            {
                return "TOP " + num;
            }
            else if (this.Context is OracleExpressionContext)
            {
                return (HasWhere ? "AND" : "WHERE") + " ROWNUM<=" + num;
            }
            else if (this.Context is PostgreSQLExpressionContext || this.Context?.SugarContext?.Context?.CurrentConnectionConfig?.MoreSettings?.DatabaseModel == DbType.PostgreSQL)
            {
                return "limit " + num;
            }
            else if (this.Context.GetLimit() != null)
            {
                if (this?.Context?.Case?.HasWhere == true)
                {
                    this.Context.Case.HasWhere = this.HasWhere;
                }
                return this.Context.GetLimit();
            }
            else
            {
                return "limit " + num;
            }
        }
    }
}