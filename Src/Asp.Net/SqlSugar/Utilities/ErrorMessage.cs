using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
namespace SqlSugar
{
    internal static partial class ErrorMessage
    {
        internal static string ObjNotExist
        {
            get
            {
                return GetThrowMessage("{0} does not exist.",
                                       "{0}不存在。");
            }
        }
        internal static string EntityMappingError
        {
            get
            {
                return GetThrowMessage("Entity mapping error.{0}",
                                       "实体与表映射出错。{0}");
            }
        }

        public static string NotSupportedDictionary
        {
            get
            {
                return GetThrowMessage("This type of Dictionary is not supported for the time being. You can try Dictionary<string, string>, or contact the author!!",
                                       "暂时不支持该类型的Dictionary 你可以试试 Dictionary<string ,string>或者联系作者！！");
            }
        }

        public static string NotSupportedArray
        {
            get
            {
                return GetThrowMessage("This type of Array is not supported for the time being. You can try object[] or contact the author!!",
                                       "暂时不支持该类型的Array 你可以试试 object[] 或者联系作者！！");
            }
        }

        internal static string GetThrowMessage(string enMessage, string cnMessage)
        {
            if (CultureInfo.CurrentCulture.Name.StartsWith("zh"))
                return cnMessage;
            else
                return enMessage;
        }
    }
}
