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
        {
            var exp = expression as MethodCallExpression;
            if(IsAutoGeneric(exp)) return GetValueByAuto(exp);
            if (IsAutoSelect(exp)) return GetValueByAuto(exp);
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
            if (exp.Arguments.Any()&&exp.Arguments[0] is LambdaExpression)
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

        private string GetValueByAuto(MethodCallExpression exp)
        {
            var selectExp = exp.Arguments.FirstOrDefault();
            if (selectExp==null) 
            {
                var type = exp.Type.GenericTypeArguments[0];
                var parameter = Expression.Parameter(type, "it");

                // 构造返回值表达式
                var body = Expression.MemberInit(Expression.New(type));

                // 将返回值表达式作为lambda表达式的主体
                selectExp = Expression.Lambda(body, parameter);

            }
            var bodyExp=ExpressionTool.GetLambdaExpressionBody(selectExp);
            var newMemExp = (bodyExp as MemberInitExpression);
            var parameters = ExpressionTool.GetParameters(exp);
            InitType(exp);
            if (parameters.Any())
            {
                this.Context.CurrentShortName = this.Context.GetTranslationColumnName(parameters.FirstOrDefault().Name);
            }
            Check.ExceptionEasy(newMemExp == null, $"Subquery ToList(exp,true) expression {exp.ToString()} can only be it=>new class(){{Id = it.id}}", $"子查询ToList(exp,true)表达式{exp.ToString()}只能是it=>new class(){{ id=it.Id}}");
            var dic=ExpressionTool.GetMemberBindingItemList(newMemExp.Bindings);
            var db = this.Context.SugarContext.Context;
            var builder = this.Context.SugarContext.QueryBuilder.Builder;
            var columnInfos=db.EntityMaintenance.GetEntityInfo(bodyExp.Type);
            var autoColumns = columnInfos.Columns
                          .Where(it => !dic.ContainsKey(it.PropertyName))
                          .Where(it => it.IsIgnore == false)
                          .ToList();
            List<string> appendColumns = new List<string>();
            List<string> completeColumnColumns = new List<string>();
            foreach (var item in autoColumns)
            {

                foreach (var parameter in parameters)
                {
                    var parameterColumns = db.EntityMaintenance.GetEntityInfo(parameter.Type).Columns;
                    if (!completeColumnColumns.Any(it=>it.EqualCase(item.PropertyName))&& parameterColumns.Any(it=>it.PropertyName.EqualCase(item.PropertyName))) 
                    {
                        var completeColumn = parameterColumns.First(it => it.PropertyName == item.PropertyName);
                        var shortName = builder.GetTranslationColumnName(parameter.Name);
                        var columnName = builder.GetTranslationColumnName(completeColumn.DbColumnName);
                        var asName = builder.SqlTranslationLeft + item.PropertyName + builder.SqlTranslationRight; 
                        appendColumns.Add($"{shortName}.{columnName} as {asName}");
                        completeColumnColumns.Add(completeColumn.PropertyName);
                    }
                }
            }
            var copyContext = this.Context.GetCopyContextWithMapping();
            copyContext.Resolve(bodyExp, ResolveExpressType.SelectMultiple);
            var select = copyContext.Result.GetString();
            if (dic.Count > 0 && appendColumns.Count == 0)
            {
                return select + ",@sugarIndex as sugarIndex"; ;
            }
            else if (dic.Count > 0 && appendColumns.Count > 0) 
            {
                return select+","+string.Join(",",appendColumns) + ",@sugarIndex as sugarIndex"; ;
            }
            else 
            {
                return string.Join(",", appendColumns) + ",@sugarIndex as sugarIndex";
            }
        }
        private static bool IsAutoSelect(MethodCallExpression exp)
        {
            return exp.Arguments.Count == 2 && exp.Arguments.Last().Type == UtilConstants.BoolType;
        }
        private static bool IsAutoGeneric(MethodCallExpression exp)
        {
            return exp.Arguments.Count == 0&&exp.Method.GetGenericArguments().Count()==1;
        }
    }
}
