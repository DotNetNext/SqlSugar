using System;
using System.Linq;

namespace SqlSugar.IoTDB
{
    public sealed class IoTDBBuilder : SqlBuilderProvider
    {
        public override string SqlTranslationLeft => string.Empty;
        public override string SqlTranslationRight => string.Empty;
        public override string SqlDateNow => "now()";
        public override string FullSqlDateNow => "select now()";

        public override string GetTranslationColumnName(string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName)) return propertyName;
            if (propertyName.Contains("(")) return propertyName;
            var names = propertyName.Split('.');
            var translated = names.Select((name, index) =>
            {
                if (index == 0 && names.Length > 1 && !IsIdentifier(name)) return name;
                return this.Context.CurrentConnectionConfig.DbType == DbType.IoTDB
                    ? Options.GetMeasurementName(name)
                    : name;
            });
            return string.Join(".", translated);
        }

        public override string GetTranslationColumnName(string entityName, string propertyName)
        {
            var mapping = this.Context.MappingColumns.FirstOrDefault(it =>
                it.EntityName.Equals(entityName, StringComparison.OrdinalIgnoreCase) &&
                it.PropertyName.Equals(propertyName, StringComparison.OrdinalIgnoreCase));
            return Options.GetMeasurementName(mapping?.DbColumnName ?? propertyName);
        }

        public override string GetTranslationTableName(string name)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Contains("(") || name.Contains(" ")) return name;
            var mapping = this.Context.MappingTables.FirstOrDefault(it => it.EntityName.Equals(name, StringComparison.OrdinalIgnoreCase));
            return Options.GetDevicePath(mapping?.DbTableName ?? name);
        }

        public override string GetNoTranslationColumnName(string name)
        {
            if (string.IsNullOrEmpty(name)) return name;
            var beforeAssignment = name.Split('=').First().Trim();
            return beforeAssignment.Trim('`', '"');
        }

        private IoTDBConnectionOptions Options => IoTDBConnectionOptions.Parse(this.Context.CurrentConnectionConfig.ConnectionString);

        private static bool IsIdentifier(string name)
        {
            return name.All(ch => char.IsLetterOrDigit(ch) || ch == '_');
        }
    }
}
