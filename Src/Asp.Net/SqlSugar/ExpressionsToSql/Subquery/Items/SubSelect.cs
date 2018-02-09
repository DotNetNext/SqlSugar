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
            return SubTools.GetMethodValue(this.Context, exp.Arguments[0],ResolveExpressType.FieldSingle);
        }
    }
}
