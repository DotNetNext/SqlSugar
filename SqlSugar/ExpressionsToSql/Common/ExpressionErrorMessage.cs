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
                return ExpressionConst.GetThrowMessage("Lambda parsing error: {0} does not support the operator to find!","拉姆达解析出错：不支持{0}此种运算符查找！");
            }
        }
        internal static string ExpFileldError
        {
            get
            {
                return ExpressionConst.GetThrowMessage("Expression format error, correct format: it=>it.fieldName","表达示格式错误，正确格式： it=>it.fieldName");
            }
        }

        internal static string MethodError
        {
            get
            {
                return ExpressionConst.GetThrowMessage("Expression parsing does not support the current function {0}. There are many functions available in the SqlFunc class, for example, it=>SqlFunc.HasValue(it.Id)", "拉姆达解析不支持当前函数{0}，SqlFunc这个类里面有大量函数可用,也许有你想要的，例如： it=>SqlFunc.HasValue(it.Id)");
            }
        }
    }
}
