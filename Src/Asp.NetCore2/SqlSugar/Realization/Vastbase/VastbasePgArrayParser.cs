using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SqlSugar
{
    /// <summary>
    /// PG 数组字面量文本 → 目标数组类型（Babelfish 兼容模式(TDS) 下数组列降级为 text 的读回路径）。
    /// 写侧对应 VastbaseV5Provider 的参数序列化（FormatPgArrayLiteral，'{1,2}' 形态）。
    /// 仅当值为 string 且目标为数组时调用（PG 模式驱动返回原生数组，不走此路径）；
    /// 数值转换用 InvariantCulture。
    /// </summary>
    public static class VastbaseV5PgArrayParser
    {
        public static object Parse(string literal, Type elementType)
        {
            if (literal == null) return null;
            var content = literal.Trim();
            if (content.Length >= 2 && content[0] == '{' && content[content.Length - 1] == '}')
                content = content.Substring(1, content.Length - 2);
            if (content.Length == 0)
                return Array.CreateInstance(elementType, 0);
            // 逗号分割（引号内逗号/转义原样保留）
            var items = new List<string>();
            var sb = new StringBuilder();
            bool inQuote = false;
            for (int i = 0; i < content.Length; i++)
            {
                char c = content[i];
                if (c == '"')
                {
                    inQuote = !inQuote;
                    sb.Append(c);
                }
                else if (c == ',' && !inQuote)
                {
                    items.Add(sb.ToString());
                    sb.Length = 0;
                }
                else
                {
                    sb.Append(c);
                }
            }
            items.Add(sb.ToString());
            var elementUnder = UtilMethods.GetUnderType(elementType);
            var result = Array.CreateInstance(elementType, items.Count);
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i].Trim();
                if (item.Length == 0 || item == "NULL")
                {
                    result.SetValue(null, i);
                    continue;
                }
                if (item.Length >= 2 && item[0] == '"' && item[item.Length - 1] == '"')
                    item = item.Substring(1, item.Length - 2).Replace("\\\\", "\\").Replace("\\\"", "\"");
                if (elementUnder == UtilConstants.StringType)
                {
                    result.SetValue(item, i);
                }
                else if (elementUnder == UtilConstants.GuidType)
                {
                    result.SetValue(new Guid(item), i);
                }
                else if (elementUnder.IsEnum())
                {
                    result.SetValue(Enum.Parse(elementUnder, item), i);
                }
                else
                {
                    result.SetValue(Convert.ChangeType(item, elementUnder, CultureInfo.InvariantCulture), i);
                }
            }
            return result;
        }
    }
}
