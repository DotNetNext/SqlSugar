namespace xTPLM.RFQ.Common
{
    /// <summary>
    /// 交易市场
    /// </summary>
    public static class MarketType
    {
        private static Dictionary<string, string> _marketTypeDict;

        /// <summary>
        /// 上交所
        /// </summary>
        public const string CN_SH = "XSHG";

        /// <summary>
        /// 银行间
        /// </summary>
        public const string CN_BD = "X_CNBD";

        /// <summary>
        /// 深交所
        /// </summary>
        public const string CN_SZ = "XSHE";

        /// <summary>
        /// 北交所
        /// </summary>
        public const string X_BSE = "X_BSE";

        /// <summary>
        /// 执行市场字典
        /// </summary>
        public static Dictionary<string, string> MarketTypeDict
        {
            get
            {
                if (_marketTypeDict == null)
                {
                    _marketTypeDict = new Dictionary<string, string>();
                    _marketTypeDict.Add(CN_SH, "上交所");
                    _marketTypeDict.Add(CN_BD, "银行间");
                    _marketTypeDict.Add(CN_SZ, "深交所");
                    _marketTypeDict.Add(X_BSE, "北交所");
                }
                return _marketTypeDict.ToDictionary(m => m.Key, m => m.Value);
            }
        }

        /// <summary>
        /// 获取描述
        /// </summary>
        /// <returns></returns>
        public static string GetDescription(string MarketType)
        {
            if (!string.IsNullOrWhiteSpace(MarketType) && MarketTypeDict.TryGetValue(MarketType, out string value))
            {
                return value;
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取市场后缀
        /// </summary>
        /// <param name="MarketType"></param>
        /// <returns></returns>
        public static string GetMaketSuffix(string MarketType)
        {
            var result = "";
            switch (MarketType)
            {
                case CN_SH:
                    result = ".SH";
                    break;
                case CN_BD:
                    result = ".IB";
                    break;
                case CN_SZ:
                    result = ".SZ";
                    break;
                case X_BSE:
                    result = ".BJ";
                    break;
                default:
                    break;
            }
            return result;
        }
    }
}
