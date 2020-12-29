using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public class SubLeftJoin : ISubOperation
    {

        public bool HasWhere
        {
            get; set;
        }

        public string Name
        {
            get { return "LeftJoin"; }
        }

        public Expression Expression
        {
            get; set;
        }

        public int Sort
        {
            get
            {
                return 301;
            }
        }

        public ExpressionContext Context
        {
            get; set;
        }

        public string GetValue(Expression expression)
        {
            var exp = expression as MethodCallExpression;
            var argExp = exp.Arguments[0];
            var name =this.Context.GetTranslationColumnName((argExp as LambdaExpression).Parameters[0].Name);
            var parameter = (argExp as LambdaExpression).Parameters[1];
            Context.InitMappingInfo(parameter.Type);
            var tableName= Context.GetTranslationTableName(parameter.Type.Name, true);
            var joinString =string.Format(" {2} LEFT JOIN {1} {0} ",
                 this.Context.GetTranslationColumnName(parameter.Name), 
                 tableName,
                 this.Context.JoinIndex==0?name:"");
            var result = joinString+ "ON " + SubTools.GetMethodValue(Context, argExp, ResolveExpressType.WhereMultiple);
            //var selfParameterName = Context.GetTranslationColumnName((argExp as LambdaExpression).Parameters.First().Name) + UtilConstants.Dot;
            this.Context.JoinIndex++;

            //result = result.Replace(selfParameterName, SubTools.GetSubReplace(this.Context));
            return result;
        }
    }
}
