using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public class SubToList:ISubOperation
    {
        public bool HasWhere
        {
            get; set;
        }

        public string Name
        {
            get
            {
                return "ToList";
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
            get; set;
        }

        public string GetValue(Expression expression = null)
        {;
            var exp = expression as MethodCallExpression;
            InitType(exp);
            var type=expression.Type;
            if (type.FullName.IsCollectionsList()
                && exp.Arguments.Count == 0&& type.GenericTypeArguments.Length>0
                && this.Context.SugarContext!=null
                &&this.Context.SugarContext.QueryBuilder.IsSelectNoAll)
            {
                var entity = type.GenericTypeArguments[0];
                var columnNames=this.Context.SugarContext.Context.EntityMaintenance.GetEntityInfo(entity).Columns;
                var columnsString = string.Join(",", columnNames
                    .Where(it => it.IsIgnore == false)
                    .Where(it => it.DbColumnName.HasValue())
                    .Select(it => this.Context.GetTranslationColumnName(it.DbColumnName)));
                return $"{columnsString},@sugarIndex as sugarIndex";
            }
            else if (exp.Arguments.Count == 0)
            {
                return "*,@sugarIndex as sugarIndex";
            }
            var argExp = exp.Arguments[0];
            var parametres = (argExp as LambdaExpression).Parameters;
            if ((argExp as LambdaExpression).Body is UnaryExpression)
            {
                argExp = ((argExp as LambdaExpression).Body as UnaryExpression).Operand;
            }
            var argLambda = argExp as LambdaExpression;
            var copyContext = this.Context.GetCopyContextWithMapping();
            copyContext.Resolve(argLambda, ResolveExpressType.SelectMultiple);
            var select= copyContext.Result.GetString();
            this.Context.Parameters.AddRange(copyContext.Parameters);
            this.Context.Index = copyContext.Index;
            this.Context.ParameterIndex = copyContext.ParameterIndex;
            SetShortName(exp, null);
            return select+",@sugarIndex as sugarIndex";
        }

        private void InitType(MethodCallExpression exp)
        {
            if (exp.Arguments.Count > 0)
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
            if (exp.Arguments.Count > 1 && exp.Arguments[1] is LambdaExpression)
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
