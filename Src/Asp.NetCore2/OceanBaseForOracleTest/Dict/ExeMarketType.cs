namespace xTPLM.RFQ.Common
{
    /// <summary>
    /// 执行市场
    /// </summary>
    public static class ExeMarketType
    {
        private static Dictionary<string, string> _exeMarketDict;

        /// <summary>
        /// 上交所固定收益平台
        /// </summary>
        public const string CN_SH_FIX = "XSHG_FIX";

        /// <summary>
        /// 上交所新债券交易平台
        /// </summary>
        public const string CN_SH_NEWBOND = "XSHG_NEWBOND";

        /// <summary>
        /// 上交所竞价系统
        /// </summary>
        public const string CN_SH_NORMAL = "XSHG_NORMAL";

        /// <summary>
        /// 上交所综合业务平台
        /// </summary>
        public const string CN_SH_LARGE = "XSHG_LARGE";

        /// <summary>
        /// 深交所综合协议交易平台
        /// </summary>
        public const string CN_SZ_LARGE = "XSHE_LARGE";

        /// <summary>
        /// 深交所竞价系统
        /// </summary>
        public const string CN_SZ_NORMAL = "XSHE_NORMAL";

        /// <summary>
        /// 深交所固定收益平台
        /// </summary>
        public const string CN_SZ_FIX = "XSHE_FIX";

        /// <summary>
        /// 银行间场内
        /// </summary>
        public const string CN_BD_IN = "X_CNBD_IN";

        /// <summary>
        /// 北交所交易支持平台
        /// </summary>
        public const string X_BSE_NORMAL = "X_BSE_NORMAL";

        /// <summary>
        /// 北交所固定收益平台
        /// </summary>
        public const string X_BSE_FIX = "X_BSE_FIX";

        /// <summary>
        /// 执行市场字典
        /// </summary>
        public static Dictionary<string, string> ExeMarketDict
        {
            get
            {
                if (_exeMarketDict == null)
                {
                    _exeMarketDict = new Dictionary<string, string>();

                    _exeMarketDict.Add(CN_SH_FIX, "上交所固定收益平台");
                    _exeMarketDict.Add(CN_SH_NEWBOND, "上交所新债券交易平台");
                    _exeMarketDict.Add(CN_SH_NORMAL, "上交所竞价系统");
                    _exeMarketDict.Add(CN_SH_LARGE, "上交所综合业务平台");

                    _exeMarketDict.Add(CN_SZ_LARGE, "深交所综合协议交易平台");
                    _exeMarketDict.Add(CN_SZ_NORMAL, "深交所竞价系统");
                    _exeMarketDict.Add(CN_SZ_FIX, "深交所固定收益平台");

                    _exeMarketDict.Add(CN_BD_IN, "银行间场内");

                    _exeMarketDict.Add(X_BSE_FIX, "北交所固定收益平台");
                    _exeMarketDict.Add(X_BSE_NORMAL, "北交所交易支持平台");
                }
                return _exeMarketDict.ToDictionary(m => m.Key, m => m.Value);
            }
        }

        /// <summary>
        /// 获取描述
        /// </summary>
        /// <returns></returns>
        public static string GetDescription(string ExeMarket)
        {
            if (!string.IsNullOrWhiteSpace(ExeMarket) && ExeMarketDict.TryGetValue(ExeMarket, out string value))
            {
                return value;
            }
            return string.Empty;
        }
    }
}
