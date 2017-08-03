using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public partial class OracleExpressionContext : ExpressionContext, ILambdaExpressions
    {
        public SqlSugarClient Context { get; set; }
        public OracleExpressionContext()
        {
            base.DbMehtods = new OracleMethod();
            base.Result.IsUpper = true;
        }
        public override string GetTranslationTableName(string entityName, bool isMapping = true)
        {
            Check.ArgumentNullException(entityName, string.Format(ErrorMessage.ObjNotExist, "Table Name"));
            if (IsTranslationText(entityName)) return entityName;
            if (isMapping && this.MappingTables.IsValuable())
            {
                if (entityName.Contains("."))
                {
                    var columnInfo = entityName.Split('.');
                    var mappingInfo = this.MappingTables.FirstOrDefault(it => it.EntityName.Equals(columnInfo.Last(), StringComparison.CurrentCultureIgnoreCase));
                    if (mappingInfo != null)
                    {
                        columnInfo[columnInfo.Length - 1] = mappingInfo.EntityName;
                    }
                    return string.Join(".", columnInfo.Select(it => GetTranslationText(it)));
                }
                else
                {
                    var mappingInfo = this.MappingTables.FirstOrDefault(it => it.EntityName.Equals(entityName, StringComparison.CurrentCultureIgnoreCase));
                    return "\"" + (mappingInfo == null ? entityName : mappingInfo.EntityName) + "\"";
                }
            }
            else
            {
                if (entityName.Contains("."))
                {
                    return string.Join(".", entityName.Split('.').Select(it => GetTranslationText(it)));
                }
                else
                {
                    return GetTranslationText(entityName);
                }
            }
        }
        public override bool IsTranslationText(string name)
        {
            return name.Contains("\"") && name.Contains("\"");
        }
        public override string GetTranslationText(string name)
        {
            return "\"" + name + "\"";
        }

    }
    public partial class OracleMethod : DefaultDbMethod, IDbMethods
    {

    }
}
