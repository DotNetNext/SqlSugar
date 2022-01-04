using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar
{
    public class SubSelect : ISubOperation
    {
        public bool HasWhere
        {
            get; set;
        }

        public string Name
        {
            get
            {
                return "Select";
            }
        }

        public Expression Expression
        {
            get; set;
        }


        public int Sort
        {
            get
            {
                return 200;
            }
        }

        public ExpressionContext Context
        {
            get;set;
        }

        public string GetValue(Expression expression = null)
        {
            var exp = expression as MethodCallExpression;
            var entityType = (exp.Arguments[0] as LambdaExpression).Parameters[0].Type;
            if (this.Context.InitMappingInfo != null)
            {
                this.Context.InitMappingInfo(entityType);
                this.Context.RefreshMapping();
            }
            var result = "";
            if (this.Context.JoinIndex == 0)
                result = SubTools.GetMethodValue(this.Context, exp.Arguments[0], ResolveExpressType.FieldSingle);
            else
                result = SubTools.GetMethodValue(this.Context, exp.Arguments[0], ResolveExpressType.FieldMultiple);

            SetShortName(exp, result);

            return result;
        }

        public void SetShortName(MethodCallExpression exp, string result)
        {
            if (exp.Arguments[0] is LambdaExpression && result.IsContainsIn("+", "-"))
            {
                var parameters = (exp.Arguments[0] as LambdaExpression).Parameters;
                if (parameters != null && parameters.Count > 0)
                {
                    this.Context.CurrentShortName = this.Context.SqlTranslationLeft + parameters[0] + this.Context.SqlTranslationRight;
                }

            }
        }
        public void SetShortNameNext(MethodCallExpression exp, string result)
        {
            if (exp.Arguments.Count>1&&exp.Arguments[1] is LambdaExpression && result.IsContainsIn("+", "-"))
            {
                var parameters = (exp.Arguments[1] as LambdaExpression).Parameters;
                if (parameters != null && parameters.Count > 0)
                {
                    this.Context.CurrentShortName = this.Context.SqlTranslationLeft + parameters[0] + this.Context.SqlTranslationRight;
                }

            }
        }
    }
}
