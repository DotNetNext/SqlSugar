namespace xTPLM.RFQ.Common.Enum
{
    /// <summary>
    /// 交易日期类型
    /// </summary>
    public enum TradeDateMode
    {
        /// <summary>
        /// 今天
        /// </summary>
        Today = 0,

        /// <summary>
        /// 明天
        /// </summary>
        Tomorrow = 1,

        /// <summary>
        /// 后天
        /// </summary>
        DayAfterTomorrow = 2,

        /// <summary>
        /// 周几
        /// </summary>
        Weeks = 3,

        /// <summary>
        /// 指定日期
        /// </summary>
        Specified = 4
    }
}
