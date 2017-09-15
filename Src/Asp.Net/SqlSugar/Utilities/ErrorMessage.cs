using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace SqlSugar
{
    internal static partial class ErrorMessage
    {
        internal static string FilterError
        {
            get
            {
                return GetThrowMessage("The query parameter does not allow the existence of special combination, for example: %+number+anyword+% Or 0x+anyword+0",
                                       "参数不能允许存在特殊组合，例如 :% + 数字 + 任意字符 + % 或者 0x + 任意字符 + 0");
            }
        }

        internal static string EntityNamespaceError
        {
            get
            {
                return GetThrowMessage("ConnectionConfig.EntityNamespace cannot be null.",
                                       "ConnectionConfig.EntityNamespace 不能为null。");
            }
        }

        internal static string ConnectionConfigIsNull
        {
            get
            {
                return GetThrowMessage("CurrentConnectionConfig and CurrentConnectionConfig attributes can't be null",
                                       "CurrentConnectionConfig和它的属性不能为null。");
            }
        }
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

        internal static string GetThrowMessage(string enMessage, string cnMessage, params string[] args)
        {
            List<string> formatArgs = new List<string>() { enMessage, cnMessage };
            formatArgs.AddRange(args);
            return string.Format(@"English Message : {0}
Chinese Message : {1}", formatArgs.ToArray());
        }
    }
}
