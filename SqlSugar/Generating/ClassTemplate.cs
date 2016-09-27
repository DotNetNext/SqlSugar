﻿using System;
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
        /// <param name="foreachStr"></param>
        /// <param name="classNameStr"></param>
        /// <param name="primaryKeyName"></param>
        /// <returns></returns>
        public static string Replace(string templateStr, string nameSpaceStr, string foreachStr, string classNameStr, List<string> primaryKeyName = null)
        {
            if (nameSpaceStr.IsNullOrEmpty())
            {
                nameSpaceStr = "System";
            }
            templateStr = templateStr.Replace("$foreach", foreachStr)
                .Replace("$namespace", nameSpaceStr)
                .Replace("$className", classNameStr);
            //处理主键
            if (primaryKeyName != null && primaryKeyName.Count > 0)
            {
                templateStr = templateStr.Replace("$primaryKeyName", primaryKeyName[0]);
                //处理特殊的主键取值
                for (int i = 0; i < primaryKeyName.Count; i++)
                {
                    templateStr = templateStr.Replace("$primaryKeyName_" + i + "", primaryKeyName[i]);
                }
            }
            return templateStr;
        }
        /// <summary>
        /// 字段模版
        /// </summary>
        public static string ItemTemplate = "        public {0}{3} {1} {2}";

        /// <summary>
        /// 生成的实体类模版
        /// </summary>
        public static string Template =
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
