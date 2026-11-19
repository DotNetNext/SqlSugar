using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SqlSugar.IoTDB
{
    public static class IoTDBParameterBinder
    {
        public static string Bind(string sql, IEnumerable<KeyValuePair<string, object>> parameters)
        {
            if (string.IsNullOrEmpty(sql) || parameters == null)
                return sql;

            var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var parameter in parameters)
            {
                var name = parameter.Key?.TrimStart('@', ':', '?', '$');
                if (!string.IsNullOrEmpty(name))
                    values[name] = ToSqlLiteral(parameter.Value);
            }

            var result = new StringBuilder(sql.Length);
            var state = LexState.Normal;
            for (var i = 0; i < sql.Length; i++)
            {
                var current = sql[i];
                var next = i + 1 < sql.Length ? sql[i + 1] : '\0';
                if (state == LexState.SingleQuote)
                {
                    result.Append(current);
                    if (current == '\'' && next == '\'') result.Append(sql[++i]);
                    else if (current == '\'') state = LexState.Normal;
                    continue;
                }
                if (state == LexState.DoubleQuote || state == LexState.Backtick)
                {
                    var delimiter = state == LexState.DoubleQuote ? '"' : '`';
                    result.Append(current);
                    if (current == delimiter && next == delimiter) result.Append(sql[++i]);
                    else if (current == delimiter) state = LexState.Normal;
                    continue;
                }
                if (state == LexState.LineComment)
                {
                    result.Append(current);
                    if (current == '\n') state = LexState.Normal;
                    continue;
                }
                if (state == LexState.BlockComment)
                {
                    result.Append(current);
                    if (current == '*' && next == '/')
                    {
                        result.Append(sql[++i]);
                        state = LexState.Normal;
                    }
                    continue;
                }

                if (current == '\'') { state = LexState.SingleQuote; result.Append(current); continue; }
                if (current == '"') { state = LexState.DoubleQuote; result.Append(current); continue; }
                if (current == '`') { state = LexState.Backtick; result.Append(current); continue; }
                if (current == '-' && next == '-') { state = LexState.LineComment; result.Append(current).Append(sql[++i]); continue; }
                if (current == '/' && next == '*') { state = LexState.BlockComment; result.Append(current).Append(sql[++i]); continue; }

                if (current == '@' || current == ':' || current == '?' || current == '$')
                {
                    var end = i + 1;
                    while (end < sql.Length && (char.IsLetterOrDigit(sql[end]) || sql[end] == '_')) end++;
                    if (end > i + 1 && values.TryGetValue(sql.Substring(i + 1, end - i - 1), out var literal))
                    {
                        result.Append(literal);
                        i = end - 1;
                        continue;
                    }
                }
                result.Append(current);
            }
            return result.ToString();
        }

        public static string ToSqlLiteral(object value)
        {
            if (value == null || value == DBNull.Value) return "NULL";
            if (value is bool boolean) return boolean ? "true" : "false";
            if (value is DateTime dateTime) return ToUnixTimeMilliseconds(dateTime).ToString(CultureInfo.InvariantCulture);
            if (value is DateTimeOffset dateTimeOffset) return dateTimeOffset.ToUnixTimeMilliseconds().ToString(CultureInfo.InvariantCulture);
            if (value is byte[] bytes) return "X'" + BitConverter.ToString(bytes).Replace("-", string.Empty) + "'";
            if (value.GetType().IsEnum) return Convert.ToInt64(value, CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture);

            switch (Type.GetTypeCode(value.GetType()))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Decimal:
                case TypeCode.Single:
                case TypeCode.Double:
                    return Convert.ToString(value, CultureInfo.InvariantCulture);
                default:
                    return "'" + Convert.ToString(value, CultureInfo.InvariantCulture).Replace("'", "''") + "'";
            }
        }

        internal static long ToUnixTimeMilliseconds(DateTime value)
        {
            if (value.Kind == DateTimeKind.Unspecified)
                value = DateTime.SpecifyKind(value, DateTimeKind.Utc);
            return new DateTimeOffset(value.ToUniversalTime()).ToUnixTimeMilliseconds();
        }

        private enum LexState { Normal, SingleQuote, DoubleQuote, Backtick, LineComment, BlockComment }
    }
}
