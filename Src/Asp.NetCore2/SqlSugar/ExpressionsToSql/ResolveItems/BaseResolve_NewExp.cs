using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
namespace SqlSugar
{
    /// <summary>
    ///BaseResolve New Expression
    /// </summary>
    public partial class BaseResolve
    {
        public string GetNewExpressionValue(Expression item)
        {
            var newContext = this.Context.GetCopyContextWithMapping();
            newContext.SugarContext = this.Context.SugarContext;
            newContext.Resolve(item, this.Context.IsJoin ? ResolveExpressType.WhereMultiple : ResolveExpressType.WhereSingle);
            this.Context.Index = newContext.Index;
            this.Context.ParameterIndex = newContext.ParameterIndex;
            if (newContext.Parameters.HasValue())
            {
                foreach (var p in newContext.Parameters)
                {
                    if (!this.Context.Parameters.Any(it => it.ParameterName == p.ParameterName))
                    {
                        this.Context.Parameters.Add(p);
                    }
                }
            }
            if (this.Context.SingleTableNameSubqueryShortName == "Subqueryable()")
            {
                this.Context.SingleTableNameSubqueryShortName = newContext.SingleTableNameSubqueryShortName;
            }
            else if (newContext.SingleTableNameSubqueryShortName!=null&& newContext.Result !=null && newContext.Result.Contains(this.Context.SqlTranslationLeft+ newContext.SingleTableNameSubqueryShortName+ this.Context.SqlTranslationRight))
            {
                this.Context.SingleTableNameSubqueryShortName = newContext.SingleTableNameSubqueryShortName;
            }
            return newContext.Result.GetResultString();
        }

        public string GetNewExpressionValue(Expression item, ResolveExpressType type)
        {
            var newContext = this.Context.GetCopyContextWithMapping();
            newContext.SugarContext = this.Context.SugarContext;
            newContext.Resolve(item, type);
            this.Context.Index = newContext.Index;
            this.Context.ParameterIndex = newContext.ParameterIndex;
            if (newContext.Parameters.HasValue())
            {
                this.Context.Parameters.AddRange(newContext.Parameters);
            }
            return newContext.Result.GetResultString();
        }

        protected void ResolveNewExpressions(ExpressionParameter parameter, Expression item, string asName)
        {
            if (item is ConstantExpression)
            {
                ResolveConst(parameter, item, asName);
            }
            else if ((item is MemberExpression) && ((MemberExpression)item).Expression == null)
            {
                ResolveMember(parameter, item, asName);
            }
            else if ((item is MemberExpression) && ((MemberExpression)item).Expression.NodeType == ExpressionType.Constant)
            {
                ResolveMemberConst(parameter, item, asName);
            }
            else if (item is MemberExpression)
            {
                ResolveMemberOther(parameter, item, asName);
            }
            else if (item is UnaryExpression && ((UnaryExpression)item).Operand is MemberExpression)
            {
                ResolveUnaryExpMem(parameter, item, asName);
            }
            else if (item is UnaryExpression && ((UnaryExpression)item).Operand is ConstantExpression)
            {
                ResolveUnaryExpConst(parameter, item, asName);
            }
            else if (ExpressionTool.RemoveConvert(item) is BinaryExpression)
            {
                ResolveBinary(item, asName);
            }
            else if (item.Type.IsClass())
            {
                asName = ResolveClass(parameter, item, asName);
            }
            else if (item.Type == UtilConstants.BoolType && item is MethodCallExpression && IsNotCaseExpression(item))
            {
                ResloveBoolMethod(parameter, item, asName);
            }
            else if (item.NodeType == ExpressionType.Not
                && (item as UnaryExpression).Operand is MethodCallExpression
                && ((item as UnaryExpression).Operand as MethodCallExpression).Method.Name.IsIn("IsNullOrEmpty", "IsNullOrWhiteSpace"))
            {
                ResloveNot(parameter, item, asName);
            }
            else if (item is MethodCallExpression && (item as MethodCallExpression).Method.Name.IsIn("Count", "Any") && !item.ToString().StartsWith("Subqueryable"))
            {
                ResloveCountAny(parameter, item, asName);
            }
            else if (item is MethodCallExpression || item is UnaryExpression || item is ConditionalExpression || item.NodeType == ExpressionType.Coalesce)
            {
                ResloveOtherMUC(parameter, item, asName);
            }
            else
            {
                Check.ThrowNotSupportedException(item.GetType().Name);
            }
        }

     }
}
