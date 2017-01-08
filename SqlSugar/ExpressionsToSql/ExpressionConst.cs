using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    internal class ExpressionConst
    {
        public const string BinaryExpressionInfoListKey = "BinaryExpressionInfoListKey";
        public const string BinaryFormatString = " ( {0} {1} {2} ) ";
        public const string Format0 = "{0}";
        public const string Format1 = "{1}";
        internal static string GetThrowMessage(string enMessage, string cnMessage, params string[] args)
        {
            List<string> formatArgs = new List<string>() { enMessage, cnMessage };
            formatArgs.AddRange(args);
            return string.Format("\r\n English Message : {0}\r\n Chinese Message : {1}", formatArgs.ToArray());
        }
    }
    internal partial class ErrorMessage


    {
        internal static string OperatorError
        {
            get
            {
                return ExpressionConst.GetThrowMessage("拉姆达解析出错：不支持{0}此种运算符查找！",
                                       "Lambda parsing error: {0} does not support the operator to find!");
            }
        }
        internal static string ExpFileldError
        {
            get
            {
                return ExpressionConst.GetThrowMessage("Expression format error, correct format: it=>it.fieldName",
                                       "表达示格式错误，正确格式： it=>it.fieldName");
            }
        }

    }
}
