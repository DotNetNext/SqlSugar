using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar
{
    public class SubFromTable : ISubOperation
    {
        public bool HasWhere
        {
            get; set;
        }

        public string Name
        {
            get
            {
                return "Subqueryable";
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
                return 300;
            }
        }

        public ExpressionContext Context
        {
            get;set;
        }

        public string GetValue(Expression expression)
        {
            var exp = expression as MethodCallExpression;
            var resType = exp.Method.ReturnType;
            var entityType = resType.GetGenericArguments().First();
            var name = entityType.Name;
            if (this.Context.InitMappingInfo != null)
            {
                this.Context.InitMappingInfo(entityType);
                this.Context.RefreshMapping();
            }
            var result= "FROM "+this.Context.GetTranslationTableName(name,true);
            if (this.Context.SubQueryIndex > 0) {
                result += " subTableIndex"+this.Context.SubQueryIndex;
            }
            return result;
        }
    }
}
