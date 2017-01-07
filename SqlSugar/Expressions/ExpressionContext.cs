using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
namespace SqlSugar
{
    public class ExpressionContext : ExpResolveAccessory
    {
        private ResolveExpressType type { get; set; }
        private Expression expression { get; set; }
        private bool IsSingle { get { return this.type == ResolveExpressType.Single; } }

        public ExpressionContext(Expression expression, ResolveExpressType type)
        {
            this.type = type;
            this.expression = expression;
        }

        public string GetFiledName()
        {
            string reval = null;
            LambdaExpression lambda = this.expression as LambdaExpression;
            var isConvet = lambda.Body.NodeType.IsIn(ExpressionType.Convert);
            var isMember = lambda.Body.NodeType.IsIn(ExpressionType.MemberAccess);
            if (!isConvet && !isMember)
                throw new SqlSugarException(ErrorMessage.ExpFileldError);
            try
            {
                if (isConvet && IsSingle)
                {
                    var memberExpr = ((UnaryExpression)lambda.Body).Operand as MemberExpression;
                    reval = memberExpr.Member.Name;
                }
                else if (!isConvet && IsSingle)//isMember
                {
                    reval = (lambda.Body as MemberExpression).Member.Name;
                }
                else if (isConvet && !IsSingle)
                {
                    var memberExpr = ((UnaryExpression)lambda.Body).Operand as MemberExpression;
                    reval = memberExpr.ToString();
                }
                else if (!isConvet && !IsSingle)
                {
                    reval = lambda.Body.ToString();
                }
            }
            catch
            {
                throw new SqlSugarException(ErrorMessage.ExpFileldError);
            }
            return reval;
        }

        public string GetWhere()
        {
            BaseResolve resolve = new BaseResolve(this.expression);
            resolve.Context = this;
            resolve.Start();
            return resolve.SqlWhere;
        }

        public string GetSelect() { return ""; }

        public List<SqlParameter> Parameters
        {
            get
            {
                return PubMethod.IsNullReturnNew(base._Parameters);
            }
            set
            {
                base._Parameters = value;
            }
        }
    }
}
