using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SqlSugar
{
    internal class SqlValueHelper
    {
        public  static bool IsSqlValue(string valueString)
        {
            return Regex.IsMatch(valueString, @"^\{\w{1,10}\}\:");
        }
        public static string GetType(string valueString)
        {
            return Regex.Match(valueString, @"^\{(\w+)\}\:").Groups[1].Value;
        }
        public static string GetValue(string valueString)
        {
            return Regex.Replace(valueString, @"^\{\w{1,10}\}\:", "");
        }

    }
}
