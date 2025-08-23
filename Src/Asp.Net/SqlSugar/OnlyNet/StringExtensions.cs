using System;
using System.Collections.Generic;
using System.Linq;
 using System.Text;
namespace SqlSugar 
{ 
    public static class StringExtensions
    {
        public static string Replace(this string source, string oldValue, string newValue, StringComparison comparison)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(oldValue))
                return source;

            var sb = new StringBuilder();
            int previousIndex = 0, index;

            while ((index = source.IndexOf(oldValue, previousIndex, comparison)) != -1)
            {
                sb.Append(source, previousIndex, index - previousIndex);
                sb.Append(newValue);
                previousIndex = index + oldValue.Length;
            }

            sb.Append(source, previousIndex, source.Length - previousIndex);
            return sb.ToString();
        }
    }
}
