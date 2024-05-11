namespace xTPLM.RFQ.Common.Dict
{
    /// <summary>
    /// 托管市场
    /// </summary>
    public static class HostMarketType
    {
        private static Dictionary<string, string> _hostMarketTypeDict;

        /// <summary>
        /// 上交所
        /// </summary>
        public const string CN_SH = "XSHG";

        /// <summary>
        /// 深交所
        /// </summary>
        public const string CN_SZ = "XSHE";

        /// <summary>
        /// 中债登
        /// </summary>
        public const string CN_BD_ZZD = "X_CNBD_ZZD";

        /// <summary>
        /// 清算所
        /// </summary>
        public const string CN_BD_QSS = "X_CNBD_QSS";

        /// <summary>
        /// 中金所
        /// </summary>
        public const string CN_FFEX = "X_CNFFEX";

        /// <summary>
        /// 其他
        /// </summary>
        public const string NONE = "NONE";

        /// <summary>
        /// 金交所
        /// </summary>
        public const string CN_SGEX = "SGEX";

        /// <summary>
        /// 场外市场托管场所
        /// </summary>
        public const string OTHER = "OTHER";

        /// <summary>
        /// 机构间市场托管场所
        /// </summary>
        public const string X_INTER = "X_INTER";

        /// <summary>
        /// 自由贸易区托管场所
        /// </summary>
        public const string X_FTZ = "X_FTZ";

        /// <summary>
        /// 票交所托管场所
        /// </summary>
        public const string X_SHCPE = "X_SHCPE";

        /// <summary>
        /// 港交所
        /// </summary>
        public const string XHKG = "XHKG";

        /// <summary>
        /// 新三板托管市场
        /// </summary>
        public const string X_NEEQ = "X_NEEQ";

        /// <summary>
        /// 银登中心
        /// </summary>
        public const string X_CBDC = "X_CBDC";

        /// <summary>
        /// 外汇市场
        /// </summary>
        public const string X_FX = "X_FX";

        /// <summary>
        /// 欧清市场
        /// </summary>
        public const string CMMT = "CMMT";

        /// <summary>
        /// 执行市场字典
        /// </summary>
        public static Dictionary<string, string> HostMarketTypeDict
        {
            get
            {
                if (_hostMarketTypeDict == null)
                {
                    _hostMarketTypeDict = new Dictionary<string, string>();
                    _hostMarketTypeDict.TryAdd(CN_SH, "上交所");
                    _hostMarketTypeDict.TryAdd(CN_SZ, "深交所");
                    _hostMarketTypeDict.TryAdd(CN_BD_ZZD, "中债登");
                    _hostMarketTypeDict.TryAdd(CN_BD_QSS, "清算所");
                    _hostMarketTypeDict.TryAdd(CN_FFEX, "中金所");
                    _hostMarketTypeDict.TryAdd(CN_SGEX, "金交所");
                    _hostMarketTypeDict.TryAdd(OTHER, "场外市场");
                    _hostMarketTypeDict.TryAdd(X_CBDC, "银登中心");
                }
                return _hostMarketTypeDict.ToDictionary(m => m.Key, m => m.Value);
            }
        }

        /// <summary>
        /// 获取描述
        /// </summary>
        /// <returns></returns>
        public static string GetDescription(string type)
        {
            if (!string.IsNullOrWhiteSpace(type) && HostMarketTypeDict.TryGetValue(type, out string value))
            {
                return value;
            }
            return string.Empty;
        }
    }
}
