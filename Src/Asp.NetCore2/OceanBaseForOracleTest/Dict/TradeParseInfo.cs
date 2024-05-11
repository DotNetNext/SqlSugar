namespace xTPLM.RFQ.Common
{
    /// <summary>
    /// 交易解析配置
    /// </summary>
    public static class TradeParseInfo
    {
        private static Dictionary<string, string> _tradeParseInfoDict;

        /// <summary>
        /// 文本解析对应交易员
        /// </summary>
        public const string PARSETRADER = "PARSETRADER";

        /// <summary>
        /// 文本解析对应交易对手
        /// </summary>
        public const string PARSEPARTY = "PARSEPARTY";

        /// <summary>
        /// 用户对应配置交易员
        /// </summary>
        public const string USERTRADE = "USERTRADE";

        /// <summary>
        /// 内政
        /// </summary>
        public const string INTSECU = "INTSECU";

        /// <summary>
        /// 执行员
        /// </summary>
        public const string EXECUTOR = "EXECUTOR";

        /// <summary>
        /// 执行市场字典
        /// </summary>
        public static Dictionary<string, string> TradeParseInfoDict
        {
            get
            {
                if (_tradeParseInfoDict == null)
                {
                    _tradeParseInfoDict = new Dictionary<string, string>();
                    _tradeParseInfoDict.Add(PARSETRADER, "文本解析对应交易员");
                    _tradeParseInfoDict.Add(PARSEPARTY, "文本解析对应交易对手");
                    _tradeParseInfoDict.Add(USERTRADE, "用户对应配置交易员");
                    _tradeParseInfoDict.Add(INTSECU, "内政");
                    _tradeParseInfoDict.Add(EXECUTOR, "执行员");
                }
                return _tradeParseInfoDict.ToDictionary(m => m.Key, m => m.Value);
            }
        }

        /// <summary>
        /// 获取描述
        /// </summary>
        /// <returns></returns>
        public static string GetDescription(string TradeParseInfo)
        {
            if (!string.IsNullOrWhiteSpace(TradeParseInfo) && TradeParseInfoDict.TryGetValue(TradeParseInfo, out string value))
            {
                return value;
            }
            return string.Empty;
        }

    }
}
