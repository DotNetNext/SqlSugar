using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SqlSugar
{
    internal class ExpressionErrorMessage
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
