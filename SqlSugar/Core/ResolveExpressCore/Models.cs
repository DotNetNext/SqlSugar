using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
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
        /// 表达式常量用来处理bool的实体
        /// </summary>
        public class ExpressConstantBoolModel
        {

            public Guid Key { get; set; }

            public Type Type { get; set; }

            public string OldValue { get; set; }

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
            public string ConditionalValue
            {
                get
                {
                    return Convert.ToBoolean(OldValue) ? "1=1" : "1=2";
                }
            }
        }
    }
}
