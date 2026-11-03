using System;
using System.Text.Json;

namespace SqlSugar.SonnetDB
{
    public class SonnetDBDbFirst : DbFirstProvider
    {
        protected override string GetPropertyTypeName(DbColumnInfo column)
        {
            if (column.DataType.Equals("TIME", StringComparison.OrdinalIgnoreCase))
            {
                return column.IsNullable ? "TimeOnly?" : "TimeOnly";
            }

            // The shared DbFirst template recognizes JSON attributes by this prefix.
            if (column.DataType.Equals("JSON", StringComparison.OrdinalIgnoreCase))
            {
                column.DataType = "json";
            }

            return base.GetPropertyTypeName(column);
        }

        protected override string GetPropertyTypeConvert(DbColumnInfo column)
        {
            var value = column.DefaultValue.Trim();
            if (value.Equals("NULL", StringComparison.OrdinalIgnoreCase))
            {
                return "null";
            }
            // Native metadata quotes the function identifier, e.g. "CURRENT_DATETIME"().
            var function = value.EndsWith("()", StringComparison.Ordinal)
                ? value.Substring(0, value.Length - 2).Trim('"')
                : value;
            if (column.DataType.Equals("DATETIME", StringComparison.OrdinalIgnoreCase) &&
                function.Equals("CURRENT_UTC_DATETIME", StringComparison.OrdinalIgnoreCase))
            {
                return "DateTime.UtcNow";
            }
            if (column.DataType.Equals("DATETIME", StringComparison.OrdinalIgnoreCase) &&
                (function.Equals("CURRENT_DATETIME", StringComparison.OrdinalIgnoreCase) ||
                 function.Equals("CURRENT_TIMESTAMP", StringComparison.OrdinalIgnoreCase) || value == "DateTime.Now"))
            {
                return "DateTime.Now";
            }
            if (value.Length >= 2 && value[0] == '\'' && value[value.Length - 1] == '\'')
            {
                value = value.Substring(1, value.Length - 2).Replace("''", "'");
            }

            var literal = JsonSerializer.Serialize(value);
            const string invariant = "System.Globalization.CultureInfo.InvariantCulture";
            return column.DataType.ToUpperInvariant() switch
            {
                "INT" => $"long.Parse({literal}, {invariant})",
                "FLOAT" => $"double.Parse({literal}, {invariant})",
                "DECIMAL" or "NUMERIC" => $"decimal.Parse({literal}, {invariant})",
                "BOOL" when value == "1" => "true",
                "BOOL" when value == "0" => "false",
                "BOOL" => $"bool.Parse({literal})",
                "STRING" or "JSON" => literal,
                "DATETIME" => $"DateTime.Parse({literal}, {invariant}, System.Globalization.DateTimeStyles.RoundtripKind)",
                "TIME" => $"TimeOnly.Parse({literal}, {invariant})",
                _ => base.GetPropertyTypeConvert(column)
            };
        }
    }
}
