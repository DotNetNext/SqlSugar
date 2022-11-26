using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
namespace SqlSugar
{
    public partial class BaseResolve
    {
        #region Property
        protected Expression Expression { get; set; }
        protected Expression ExactExpression { get; set; }
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
            this.Expression = parameter.CurrentExpression;
            this.Context = parameter.Context;
            this.BaseParameter = parameter;
        }

        public BaseResolve Start()
        {
            Context.Index++;
            Expression expression = this.Expression;
            ExpressionParameter parameter = new ExpressionParameter()
            {
                Context = this.Context,
                CurrentExpression = expression,
                IsLeft = this.IsLeft,
                BaseExpression = this.ExactExpression,
                BaseParameter = this.BaseParameter,
                Index = Context.Index
            };
            if (expression is LambdaExpression)
            {
                return new LambdaExpressionResolve(parameter);
            }
            else if (expression is BinaryExpression && expression.NodeType == ExpressionType.Coalesce)
            {
                return new CoalesceResolveItems(parameter);
            }
            else if (expression is BinaryExpression)
            {
                return new BinaryExpressionResolve(parameter);
            }
            else if (expression is BlockExpression)
            {
                Check.ThrowNotSupportedException("BlockExpression");
            }
            else if (expression is ConditionalExpression)
            {
                return new ConditionalExpressionResolve(parameter);
            }
            else if (expression is MethodCallExpression)
            {
                return new MethodCallExpressionResolve(parameter);
            }
            else if (expression is MemberExpression && ((MemberExpression)expression).Expression == null)
            {
                return new MemberNoExpressionResolve(parameter);
            }
            else if (expression is MemberExpression && ((MemberExpression)expression).Expression.NodeType == ExpressionType.Constant)
            {
                return new MemberConstExpressionResolve(parameter);
            }
            else if (expression is MemberExpression && ((MemberExpression)expression).Expression.NodeType == ExpressionType.New)
            {
                return new MemberNewExpressionResolve(parameter);
            }
            else if (expression is ConstantExpression)
            {
                return new ConstantExpressionResolve(parameter);
            }
            else if (expression is MemberExpression)
            {
                return new MemberExpressionResolve(parameter);
            }
            else if (expression is UnaryExpression)
            {
                return new UnaryExpressionResolve(parameter);
            }
            else if (expression is MemberInitExpression)
            {
                return new MemberInitExpressionResolve(parameter);
            }
            else if (expression is NewExpression)
            {
                return new NewExpressionResolve(parameter);
            }
            else if (expression is NewArrayExpression)
            {
                return new NewArrayExpessionResolve(parameter);
            }
            else if (expression is ParameterExpression)
            {
                return new TypeParameterExpressionReolve(parameter);
            }
            else if (expression != null && expression.NodeType.IsIn(ExpressionType.NewArrayBounds))
            {
                Check.ThrowNotSupportedException("ExpressionType.NewArrayBounds");
            }
            return null;
        }

        #endregion

        #region Dictionary
        private Dictionary<string, string> GetMappingColumns(Expression currentExpression)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            if (currentExpression == null)
            {
                return result;
            }
            List<Type> types = new List<Type>();
            int i = 0;
            if (currentExpression is NewExpression)
            {
                i = (currentExpression as NewExpression).Arguments.Count;
                foreach (var item in (currentExpression as NewExpression).Arguments)
                {
                    if (item.Type.IsClass())
                    {
                        types.Add(item.Type);
                    }
                }
            }
            else if (currentExpression is MemberInitExpression)
            {
                i = (currentExpression as MemberInitExpression).Bindings.Count;
                foreach (var item in (currentExpression as MemberInitExpression).Bindings)
                {
                    MemberAssignment memberAssignment = (MemberAssignment)item;
                    if (memberAssignment.Expression.Type.IsClass())
                    {
                        types.Add(memberAssignment.Expression.Type);
                    }
                }
            }
            if (types.Count == i && (types.Count == types.Distinct().Count()))
            {
                return result;
            }
            var array = currentExpression.ToString().Split(',');
            foreach (var item in array)
            {
                var itemArray = item.Split('=').ToArray();
                var last = itemArray.Last().Trim().Split('.').First().TrimEnd(')').TrimEnd('}');
                var first = itemArray.First().Trim();
                if (first.Contains("{"))
                {
                    first = first.Split('{').Last().Trim();
                }
                if (first.Contains("("))
                {
                    first = first.Split('(').Last().Trim();
                }
                if (!result.ContainsKey(first))
                {
                    result.Add(first, last);
                }
                else
                {
                    //future
                }
            }
            return result; ;
        }
        protected static Dictionary<string, string> MethodMapping = new Dictionary<string, string>() {
            { "ToString","ToString"},
            { "ToInt32","ToInt32"},
            { "ToInt16","ToInt32"},
            { "ToInt64","ToInt64"},
            { "ToDecimal","ToDecimal"},
            { "ToDateTime","ToDate"},
            { "ToBoolean","ToBool"},
            { "ToDouble","ToDouble"},
            { "Length","Length"},
            { "Replace","Replace"},
            { "Contains","Contains"},
            { "ContainsArray","ContainsArray"},
            { "EndsWith","EndsWith"},
            { "StartsWith","StartsWith"},
            { "HasValue","HasValue"},
            { "Trim","Trim"},
            { "Equals","Equals"},
            { "ToLower","ToLower"},
            { "ToUpper","ToUpper"},
            { "Substring","Substring"},
            { "DateAdd","DateAdd"}
        };
        protected static Dictionary<string, DateType> MethodTimeMapping = new Dictionary<string, DateType>() {
            { "AddYears",DateType.Year},
            { "AddMonths",DateType.Month},
            { "AddDays",DateType.Day},
            { "AddHours",DateType.Hour},
            { "AddMinutes",DateType.Minute},
            { "AddSeconds",DateType.Second},
            { "AddMilliseconds",DateType.Millisecond}
        };
        #endregion
    }
}
