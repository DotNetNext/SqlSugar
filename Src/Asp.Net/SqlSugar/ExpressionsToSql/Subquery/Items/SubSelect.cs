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
            InitType(exp);
            var result = "";

            var oldIsSingle = this.Context.IsSingle;
            this.Context.IsSingle = false;
            result = SubTools.GetMethodValue(this.Context, exp.Arguments[0], ResolveExpressType.FieldMultiple);
            this.Context.IsSingle = oldIsSingle;

            SetShortName(exp, result);

            return result;
        }

        private void InitType(MethodCallExpression exp)
        {
            foreach (var arg in (exp.Arguments[0] as LambdaExpression).Parameters) 
            {
                if (this.Context.InitMappingInfo != null)
                {
                    this.Context.InitMappingInfo(arg.Type);
                    this.Context.RefreshMapping();
                }
            }
        }

        public void SetShortName(MethodCallExpression exp, string result)
        {
            if (exp.Arguments[0] is LambdaExpression)
            {
                var parameters = (exp.Arguments[0] as LambdaExpression).Parameters;
                if (parameters != null && parameters.Count > 0)
                {
                    this.Context.CurrentShortName = this.Context.GetTranslationColumnName(parameters[0].ObjToString());
                }

            }
        }
        public void SetShortNameNext(MethodCallExpression exp, string result)
        {
            if (exp.Arguments.Count>1&&exp.Arguments[1] is LambdaExpression )
            {
                var parameters = (exp.Arguments[1] as LambdaExpression).Parameters;
                if (parameters != null && parameters.Count > 0)
                {
                    this.Context.CurrentShortName = this.Context.GetTranslationColumnName(parameters[0].ObjToString());
                }

            }
        }
    }
}
