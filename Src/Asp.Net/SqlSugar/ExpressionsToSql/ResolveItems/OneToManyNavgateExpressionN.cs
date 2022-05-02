using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    internal class OneToManyNavgateExpressionN
    {
        private SqlSugarProvider context;
        public OneToManyNavgateExpressionN(SqlSugarProvider context, MethodCallExpressionResolve methodCallExpressionResolve)
        {
            this.context = context;

        }

        internal bool IsNavgate(Expression expression)
        {
            var result = false;
            var exp = expression;
            if (exp is UnaryExpression) 
            {
                exp = (exp as UnaryExpression).Operand;
            }
            if (exp is MethodCallExpression) 
            {
                var memberExp=exp as MethodCallExpression;
                if (memberExp.Method.Name.IsIn("Any","Count") &&  memberExp.Arguments.Count>0 && memberExp.Arguments[0] is MemberExpression ) 
                {
                    result = ValidateNav(result, memberExp.Arguments[0] as MemberExpression, memberExp.Arguments[0]);
                    if (memberExp.Arguments.Count > 1)
                    {
                    }
                }
            }
            return result;
        }

        private string GetWhereSql(MethodCallExpression memberExp)
        {
           
            return null;
        }

        private bool ValidateNav(bool result, MemberExpression memberExp, Expression childExpression)
        {
            if (childExpression != null && childExpression is MemberExpression)
            {
                result = ValidateIsJoinMember(result,memberExp, childExpression);
            }

            return result;
        }
        List<ExpressionItems> items;
        private bool ValidateIsJoinMember(bool result, MemberExpression memberExp, Expression childExpression)
        {
            if (childExpression != null && childExpression is MemberExpression)
            {
                var oldChildExpression = childExpression;
                var child2Expression = (childExpression as MemberExpression).Expression;
                if (child2Expression == null || (child2Expression is ConstantExpression))
                {
                    return false;
                }
                items = new List<ExpressionItems>();
                items.Add(new ExpressionItems() { Type = 1, Expression = memberExp, ParentEntityInfo = this.context.EntityMaintenance.GetEntityInfo(oldChildExpression.Type) });
                items.Add(new ExpressionItems() { Type = 2, Expression = oldChildExpression, ThisEntityInfo = this.context.EntityMaintenance.GetEntityInfo(oldChildExpression.Type), ParentEntityInfo = this.context.EntityMaintenance.GetEntityInfo(child2Expression.Type) });
                if (items.Any(it => it.Type == 2 && it.Nav == null))
                {
                    return false;
                }
                while (child2Expression != null)
                {
                    if (IsClass(child2Expression))
                    {
                        items.Add(new ExpressionItems() { Type = 2, Expression = child2Expression, ThisEntityInfo = this.context.EntityMaintenance.GetEntityInfo(child2Expression.Type), ParentEntityInfo = this.context.EntityMaintenance.GetEntityInfo(GetMemberExpression(child2Expression).Type) });
                        child2Expression = GetMemberExpression(child2Expression);

                    }
                    else if (IsParameter(child2Expression))
                    {
                        shorName = child2Expression.ToString();
                        entityInfo = this.context.EntityMaintenance.GetEntityInfo(child2Expression.Type);
                        break;
                    }
                    else
                    {
                        break;
                    }
                }
                if (!items.Any(it => it.Type == 2 && it.Nav == null))
                {
                    return true;
                }
            }
            return result;
        }

        internal object GetSql()
        {
            throw new NotImplementedException();
        }
    }
}
