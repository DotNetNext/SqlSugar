using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SqlSugar
{
    public class SubFromTableWithAttr : ISubOperation
    {
        public bool HasWhere
        {
            get; set;
        }

        public string Name
        {
            get
            {
                return "SubqueryableWithAttr";
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
            this.Context.SubTableType = entityType;
            var name = entityType.Name;
            if (this.Context.InitMappingInfo != null)
            {
                this.Context.InitMappingInfo(entityType);
                this.Context.RefreshMapping();
            }
            var result = "FROM ";
            var tenant = entityType.GetCustomAttribute<TenantAttribute>();
            if (tenant != null)
            {
                result += $"{this.Context.SqlTranslationLeft}{tenant.configId}{this.Context.SqlTranslationRight}{UtilConstants.Dot}dbo{UtilConstants.Dot}";
            }
            result += this.Context.GetTranslationTableName(name, true);
            if (this.Context.SubQueryIndex > 0) {
                result += " subTableIndex"+this.Context.SubQueryIndex;
            }
            return result;
        }
    }
}
