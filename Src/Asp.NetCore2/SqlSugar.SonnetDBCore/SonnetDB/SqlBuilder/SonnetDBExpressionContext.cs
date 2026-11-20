using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SqlSugar.SonnetDB
{
    public class SonnetDBExpressionContext : ExpressionContext, ILambdaExpressions
    {
        public SqlSugarProvider Context { get; set; }

        public SonnetDBExpressionContext()
        {
            DbMehtods = new SonnetDBMethod();
        }

        public override string GetLimit() => " LIMIT 1 OFFSET 0 ";
        public override string SqlTranslationLeft => "\"";
        public override string SqlTranslationRight => "\"";
        public override string GetTranslationText(string name) => "\"" + name.Replace("\"", "\"\"") + "\"";
    }

    public class SonnetDBMethod : DefaultDbMethod, IDbMethods
    {
        public override string Contains(MethodCallExpressionModel model) =>
            $"({model.Args[0].MemberName} LIKE CONCAT('%', {model.Args[1].MemberName}, '%'))";

        public override string StartsWith(MethodCallExpressionModel model) =>
            $"({model.Args[0].MemberName} LIKE CONCAT({model.Args[1].MemberName}, '%'))";

        public override string EndsWith(MethodCallExpressionModel model) =>
            $"({model.Args[0].MemberName} LIKE CONCAT('%', {model.Args[1].MemberName}))";

        public override string True() => "TRUE";
        public override string False() => "FALSE";
        public override string TrueValue() => "TRUE";
        public override string FalseValue() => "FALSE";
        public override string EqualTrue(string fieldName) => "(" + fieldName + " = TRUE)";

        public override string ContainsArray(MethodCallExpressionModel model)
        {
            if (model.Args[0].MemberValue is IEnumerable enumerable)
            {
                var values = enumerable.Cast<object>().ToList();
                if (values.Count > 0 && values.All(value => value is bool))
                {
                    return "(" + model.Args[1].MemberName + " IN (" + string.Join(",", values.Select(value => (bool)value ? "TRUE" : "FALSE")) + "))";
                }
            }
            return base.ContainsArray(model);
        }

        public override string GetDate() => "CURRENT_UTC_DATETIME()";
        public override string UNIX_TIMESTAMP(MethodCallExpressionModel model) => $"TO_UNIX_SECONDS({model.Args[0].MemberName})";

        public override string DateValue(MethodCallExpressionModel model) =>
            $"DATE_PART('{GetDatePart(model.Args[1])}', {model.Args[0].MemberName})";

        public override string DateIsSameDay(MethodCallExpressionModel model) =>
            $"(DATE_ONLY({model.Args[0].MemberName}) = DATE_ONLY({model.Args[1].MemberName}))";

        public override string DateIsSameByType(MethodCallExpressionModel model)
        {
            var left = model.Args[0].MemberName;
            var right = model.Args[1].MemberName;
            var type = GetDateType(model.Args[2]);
            var parts = type switch
            {
                DateType.Year => new[] { "year" },
                DateType.Quarter => new[] { "year", "quarter" },
                DateType.Month => new[] { "year", "month" },
                DateType.Hour => new[] { "hour" },
                DateType.Minute => new[] { "hour", "minute" },
                DateType.Second => new[] { "hour", "minute", "second" },
                DateType.Millisecond => new[] { "hour", "minute", "second", "millisecond" },
                DateType.Weekday => new[] { "day_of_week" },
                _ => Array.Empty<string>()
            };
            var conditions = parts.Select(part => $"DATE_PART('{part}', {left}) = DATE_PART('{part}', {right})").ToList();
            if (type is DateType.Day or DateType.Hour or DateType.Minute or DateType.Second or DateType.Millisecond)
            {
                conditions.Insert(0, $"DATE_ONLY({left}) = DATE_ONLY({right})");
            }
            return "(" + string.Join(" AND ", conditions) + ")";
        }

        public override string DateAddDay(MethodCallExpressionModel model) =>
            $"DATE_ADD_DATETIME({model.Args[0].MemberName}, {model.Args[1].MemberName}, 'day')";

        public override string DateAddByType(MethodCallExpressionModel model)
        {
            var part = GetDatePart(model.Args[2]);
            var amount = model.Args[1].MemberName + "";
            if (part == "quarter")
            {
                part = "month";
                amount = "(" + amount + " * 3)";
            }
            return $"DATE_ADD_DATETIME({model.Args[0].MemberName}, {amount}, '{part}')";
        }

        public override string DateDiff(MethodCallExpressionModel model) =>
            $"DATE_DIFF('{GetDatePart(model.Args[0])}', {model.Args[1].MemberName}, {model.Args[2].MemberName})";

        public override string GetDateString(string dateValue, string formatString) =>
            $"DATE_FORMAT({dateValue}, '{formatString.Replace("'", "''")}')";

        public override string ToDate(MethodCallExpressionModel model) => Cast(model, "DATETIME");
        public override string ToDateShort(MethodCallExpressionModel model) => $"DATE_ONLY({model.Args[0].MemberName})";
        public override string ToInt32(MethodCallExpressionModel model) => Cast(model, "INT");
        public override string ToInt64(MethodCallExpressionModel model) => Cast(model, "INT");
        public override string ToDouble(MethodCallExpressionModel model) => Cast(model, "FLOAT");
        public override string ToDecimal(MethodCallExpressionModel model) => Cast(model, "DECIMAL");
        public override string ToBool(MethodCallExpressionModel model) => Cast(model, "BOOL");
        public override string ToTime(MethodCallExpressionModel model) => Cast(model, "TIME");
        public override string ToString(MethodCallExpressionModel model) => Cast(model, "STRING");
        public override string ToVarchar(MethodCallExpressionModel model) => Cast(model, "STRING");
        string IDbMethods.ToSingle(MethodCallExpressionModel model) => Cast(model, "FLOAT");

        public override string MergeString(params string[] strings) => "CONCAT(" + string.Join(",", strings) + ")";
        public override string IsNull(MethodCallExpressionModel model) => $"COALESCE({model.Args[0].MemberName}, {model.Args[1].MemberName})";
        public override string Trim(MethodCallExpressionModel model) => $"TRIM({model.Args[0].MemberName})";
        public override string TrimStart(MethodCallExpressionModel model) => "LTRIM(" + string.Join(",", model.Args.Select(argument => argument.MemberName)) + ")";
        public override string TrimEnd(MethodCallExpressionModel model) => "RTRIM(" + string.Join(",", model.Args.Select(argument => argument.MemberName)) + ")";
        public override string Length(MethodCallExpressionModel model) => $"LENGTH({model.Args[0].MemberName})";
        public override string CharIndex(MethodCallExpressionModel model) => $"(POSITION({model.Args[0].MemberName} IN {model.Args[1].MemberName}) - 1)";
        public override string CharIndexNew(MethodCallExpressionModel model) => $"POSITION({model.Args[1].MemberName} IN {model.Args[0].MemberName})";

        public override string GetStringJoinSelector(string result, string separator) =>
            $"STRING_AGG({result}, '{separator.Replace("'", "''")}')";

        public override string JsonField(MethodCallExpressionModel model) =>
            $"JSON_VALUE({model.Args[0].MemberName}, {GetJsonPath(model)})";

        public override string JsonIndex(MethodCallExpressionModel model)
        {
            var argument = model.Args[1];
            var path = argument.IsMember
                ? $"CONCAT('$[', {argument.MemberName}, ']')"
                : "'$[" + Convert.ToString(argument.MemberValue, CultureInfo.InvariantCulture) + "]'";
            if (!argument.IsMember) RemoveConsumedParameter(model, argument);
            return $"JSON_VALUE({model.Args[0].MemberName}, {path})";
        }

        public override string JsonContainsFieldName(MethodCallExpressionModel model) =>
            $"JSON_EXISTS({model.Args[0].MemberName}, {GetJsonPath(model)})";

        public override string JsonArrayLength(MethodCallExpressionModel model) =>
            $"JSON_ARRAY_LENGTH({model.Args[0].MemberName}, '$')";

        public override string JsonArrayAny(MethodCallExpressionModel model) =>
            $"JSON_CONTAINS({model.Args[0].MemberName}, '$', {model.Args[1].MemberName})";

        public override string JsonListObjectAny(MethodCallExpressionModel model) =>
            $"JSON_CONTAINS({model.Args[0].MemberName}, '$', JSON_OBJECT({model.Args[1].MemberName}, {model.Args[2].MemberName}))";

        public override string JsonParse(MethodCallExpressionModel model) => Cast(model, "JSON");

        private static string Cast(MethodCallExpressionModel model, string type) => $"CAST({model.Args[0].MemberName} AS {type})";

        private static DateType GetDateType(MethodCallExpressionArgs argument) =>
            (DateType)Enum.Parse(typeof(DateType), Convert.ToString(argument.MemberValue, CultureInfo.InvariantCulture)!, true);

        private static string GetDatePart(MethodCallExpressionArgs argument)
        {
            var type = GetDateType(argument);
            return type == DateType.Weekday ? "day_of_week" : type.ToString().ToLowerInvariant();
        }

        private static string GetJsonPath(MethodCallExpressionModel model)
        {
            var fields = model.Args.Skip(1).ToList();
            if (fields.All(argument => argument.MemberValue is string))
            {
                var first = (string)fields[0].MemberValue;
                var path = first.StartsWith("$", StringComparison.Ordinal) ? first : "$['" + first.Replace("'", "''") + "']";
                foreach (var field in fields.Skip(1)) path += "['" + ((string)field.MemberValue).Replace("'", "''") + "']";
                foreach (var field in fields) RemoveConsumedParameter(model, field);
                return "'" + path.Replace("'", "''") + "'";
            }
            return "CONCAT('$', " + string.Join(",", fields.Select(field => $"CONCAT('[''', REPLACE({field.MemberName}, '''', ''''''), ''']')")) + ")";
        }

        private static void RemoveConsumedParameter(MethodCallExpressionModel model, MethodCallExpressionArgs argument)
        {
            var name = Convert.ToString(argument.MemberName, CultureInfo.InvariantCulture);
            model.Parameters?.RemoveAll(parameter => string.Equals(parameter.ParameterName, name, StringComparison.OrdinalIgnoreCase));
        }
    }
}
