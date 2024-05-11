using System.ComponentModel;

namespace xTPLM.RFQ.Common
{
    /// <summary>
    /// 类型 欧式、美式、百慕大式
    /// </summary>
    public enum ExerciseTypeEnum
    {
        /// <summary>
        /// 美式
        /// </summary>
        [Description("美式")]
        American = 0,

        /// <summary>
        /// 百慕大式
        /// </summary>
        [Description("百慕大式")]
        Bermudan = 1,

        /// <summary>
        /// 欧式
        /// </summary>
        [Description("欧式")]
        European = 2,

        /// <summary>
        /// 不行权
        /// </summary>
        [Description("不行权")]
        None = 3,
    }
}
