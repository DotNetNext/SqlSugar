using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public class ClassTemplate
    {
        /// <summary>
        ///替换模版
        /// </summary>
        /// <param name="templateStr"></param>
        /// <param name="nameSpaceStr"></param>
        /// <param name="usingStr"></param>
        /// <param name="foreachStr"></param>
        /// <returns></returns>
        public static string Replace(string templateStr, string nameSpaceStr, string foreachStr, string classNameStr)
        {
            if (nameSpaceStr.IsNullOrEmpty()) {
                nameSpaceStr = "System";
            }
            return templateStr
                .Replace("$foreach", foreachStr)
                .Replace("$namespace", nameSpaceStr)
                .Replace("$className",classNameStr);

        }
        /// <summary>
        /// 字段模版
        /// </summary>
        public static string  ItemTemplate = "        public {0}{3} {1} {2}";

        /// <summary>
        /// 生成的实体类模版
        /// </summary>
        public static string  Template =
@"using System;
using System.Linq;
using System.Text;

namespace $namespace
{
    public class $className
    {
        $foreach
    }
}
";
    }
}
