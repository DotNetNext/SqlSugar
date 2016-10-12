using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    //局部类：解析用到的实体
    internal partial class ResolveExpress
    {
        /// <summary>
        /// 拉姆达成员类型
        /// </summary>
        public enum MemberType
        {
            None = 0,
            Key = 1,
            Value = 2
        }
        /// <summary>
        /// 用来处理bool类型的实体
        /// </summary>
        public class ExpressBoolModel
        {
            /// <summary>
            /// 唯一标识
            /// </summary>
            public Guid Key { get; set; }
            /// <summary>
            /// 数据类型
            /// </summary>
            public Type Type { get; set; }
            /// <summary>
            /// 原始值
            /// </summary>
            public string OldValue { get; set; }
            /// <summary>
            /// 处事后的值
            /// </summary>
            public string NewValue
            {
                get
                {
                    if (Type == SqlSugarTool.BoolType)
                    {
                        return Convert.ToBoolean(OldValue) ? "1" : "0";
                    }
                    else
                    {
                        return OldValue.ToString();
                    }
                }
            }
            /// <summary>
            /// 处理后的运算对象
            /// </summary>
            public string ConditionalValue
            {
                get
                {
                    return Convert.ToBoolean(OldValue) ? "(1=1)" : "(1=2)";
                }
            }
        }
    }
}
