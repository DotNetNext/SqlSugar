using System.Collections.Generic;
using System.Linq;

namespace SqlSugar
{
    public class VastbaseV5Builder : PostgreSQLBuilder
    {
        private VastbaseV5Mode Mode => VastbaseV5Mode.Require(this.Context);

        public override string SqlTranslationLeft =>
            Mode?.SqlTranslationLeft ?? base.SqlTranslationLeft;

        public override string SqlTranslationRight =>
            Mode?.SqlTranslationRight ?? base.SqlTranslationRight;

        public override string GetTranslationColumnName(string propertyName)
        {
            var mode = Mode;
            if (mode != null && mode.IsBabelfish)
                return mode.MSSQLBuilder.GetTranslationColumnName(propertyName?.TrimEnd());
            return base.GetTranslationColumnName(propertyName);
        }

        public override string GetTranslationColumnName(string entityName, string propertyName)
        {
            var mode = Mode;
            if (mode != null && mode.IsBabelfish)
                return mode.MSSQLBuilder.GetTranslationColumnName(entityName?.TrimEnd(), propertyName?.TrimEnd());
            return base.GetTranslationColumnName(entityName, propertyName);
        }

        public override string GetTranslationTableName(string name)
        {
            var mode = Mode;
            if (mode != null && mode.IsBabelfish)
            {
                var result = mode.MSSQLBuilder.GetTranslationTableName(name?.TrimEnd());
                if (result != null)
                {
                    var left = mode.SqlTranslationLeft;
                    var right = mode.SqlTranslationRight;
                    if (left != null && right != null && result.StartsWith(left) && result.EndsWith(right))
                        result = left + result.Substring(left.Length, result.Length - left.Length - right.Length).TrimEnd() + right;
                }
                return result;
            }
            return base.GetTranslationTableName(name);
        }

        public override string GetUnionFomatSql(string sql)
        {
            var mode = Mode;
            if (mode != null && mode.IsBabelfish)
                return mode.MSSQLBuilder.GetUnionFomatSql(sql);
            return base.GetUnionFomatSql(sql);
        }

        public override KeyValuePair<string, SugarParameter[]> ConditionalModelToSql(List<IConditionalModel> models, int beginIndex = 0)
        {
            var mode = Mode;
            if (mode != null && mode.IsBabelfish)
            {
                foreach (var model in models)
                {
                    if (model is ConditionalModel cm
                        && string.IsNullOrEmpty(cm.CSharpTypeName)
                        && !string.IsNullOrEmpty(cm.FieldValue))
                    {
                        if (cm.ConditionalType == ConditionalType.NoEqual && cm.FieldValue.Contains(","))
                            cm.ConditionalType = ConditionalType.NotIn;

                        if (cm.ConditionalType == ConditionalType.In || cm.ConditionalType == ConditionalType.NotIn)
                        {
                            var parts = cm.FieldValue.Split(',');
                            if (parts.Length > 0 && parts.All(p => int.TryParse(p.Trim(), out _)))
                                cm.CSharpTypeName = "int";
                            else if (parts.Length > 0 && parts.All(p => long.TryParse(p.Trim(), out _)))
                                cm.CSharpTypeName = "long";
                            else if (parts.Length > 0 && parts.All(p => decimal.TryParse(p.Trim(), out _)))
                                cm.CSharpTypeName = "decimal";
                        }
                    }
                }
            }
            return base.ConditionalModelToSql(models, beginIndex);
        }
    }

    public class VastbaseV5DeleteBuilder : PostgreSQLDeleteBuilder
    {
    }
}