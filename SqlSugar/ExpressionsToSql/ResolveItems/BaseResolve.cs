using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
namespace SqlSugar
{
    public class BaseResolve
    {
        protected Expression Expression { get; set; }
        protected Expression BaseExpression { get; set; }
        public ExpressionContext Context { get; set; }
        public bool? IsLeft { get; set; }
        public int ContentIndex { get { return this.Context.Index; } }
        public int Index { get; set; }
        public ExpressionParameter BaseParameter { get; set; }

        private BaseResolve()
        {

        }
        public BaseResolve(ExpressionParameter parameter)
        {
            this.Expression = parameter.Expression;
            this.Context = parameter.Context;
            this.BaseParameter = parameter;
        }

        public BaseResolve Start()
        {
            Context.Index++;
            Expression exp = this.Expression;
            ExpressionParameter parameter = new ExpressionParameter()
            {
                Context = this.Context,
                Expression = exp,
                IsLeft = this.IsLeft,
                BaseExpression = this.BaseExpression,
                BaseParameter = this.BaseParameter,
                Index = Context.Index
            };
            if (exp is LambdaExpression)
            {
                return new LambdaExpressionResolve(parameter);
            }
            else if (exp is BinaryExpression)
            {
                return new BinaryExpressionResolve(parameter);
            }
            else if (exp is BlockExpression)
            {
                Check.ThrowNotSupportedException("BlockExpression");
            }
            else if (exp is ConditionalExpression)
            {
                Check.ThrowNotSupportedException("ConditionalExpression");
            }
            else if (exp is MethodCallExpression)
            {
                return new MethodCallExpressionResolve(parameter);
            }
            else if (exp is MemberExpression && ((MemberExpression)exp).Expression == null)
            {
                return new MemberNoExpressionResolve(parameter);
            }
            else if (exp is MemberExpression && ((MemberExpression)exp).Expression.NodeType == ExpressionType.Constant)
            {
                return new MemberConstExpressionResolve(parameter);
            }
            else if (exp is MemberExpression && ((MemberExpression)exp).Expression.NodeType == ExpressionType.New)
            {
                return new MemberNewExpressionResolve(parameter);
            }
            else if (exp is ConstantExpression)
            {
                return new ConstantExpressionResolve(parameter);
            }
            else if (exp is MemberExpression)
            {
                return new MemberExpressionResolve(parameter);
            }
            else if (exp is UnaryExpression)
            {
                return new UnaryExpressionResolve(parameter);
            }
            else if (exp is MemberInitExpression)
            {
                return new MemberInitExpressionResolve(parameter);
            }
            else if (exp is NewExpression)
            {
                return new NewExpressionResolve(parameter);
            }
            else if (exp != null && exp.NodeType.IsIn(ExpressionType.NewArrayBounds, ExpressionType.NewArrayInit))
            {
                Check.ThrowNotSupportedException("ExpressionType.NewArrayBounds and ExpressionType.NewArrayInit");
            }
            return null;
        }

        protected void AppendValue(ExpressionParameter parameter, bool? isLeft, object value)
        {
            if (parameter.BaseExpression is BinaryExpression)
            {
                var otherExpression = isLeft == true ? parameter.BaseParameter.RightExpression : parameter.BaseParameter.LeftExpression;
                if (otherExpression is MemberExpression)
                {
                    string appendValue = Context.SqlParameterKeyWord
                        + ((MemberExpression)otherExpression).Member.Name
                        + Context.ParameterIndex;
                    this.Context.Parameters.Add(new SugarParameter(appendValue, value));
                    Context.ParameterIndex++;
                    appendValue = string.Format(" {0} ", appendValue);
                    if (isLeft == true)
                    {
                        appendValue += ExpressionConst.Format1 + parameter.BaseParameter.Index;
                    }
                    if (this.Context.Result.Contains(ExpressionConst.Format0))
                    {
                        this.Context.Result.Replace(ExpressionConst.Format0, appendValue);
                    }
                    else
                    {
                        this.Context.Result.Append(appendValue);
                    }
                }
                else 
                {
                    var appendValue =this.Context.SqlParameterKeyWord+ExpressionConst.CONST +Context.ParameterIndex;
                    Context.ParameterIndex++;
                    this.Context.Parameters.Add(new SugarParameter(appendValue,value));
                    appendValue = string.Format(" {0} ", appendValue);
                    if (isLeft == true)
                    {
                        appendValue += ExpressionConst.Format1 + parameter.BaseParameter.Index;
                    }
                    if (this.Context.Result.Contains(ExpressionConst.Format0))
                    {
                        this.Context.Result.Replace(ExpressionConst.Format0, appendValue);
                    }
                    else
                    {
                        this.Context.Result.Append(appendValue);
                    }
                }
            }
        }
        protected void ActionLeft(ExpressionParameter parameter, bool? isLeft)
        {
            if (isLeft == true)
            {
                this.Context.Result.Append(" " + ExpressionConst.Format1 + parameter.BaseParameter.Index);
            }
        }
    }
}
