namespace xTPLM.RFQ.Common.Dict
{
    /// <summary>
    /// IR的结算方式
    /// </summary>
    public static class IrSetType
    {
        /// <summary>
        /// 见券付款
        /// </summary>
        public const string PaymentAfterDelivery = "PAD";

        /// <summary>
        /// 见款付券
        /// </summary>
        public const string DeliveryAfterPayment = "DAP";

        /// <summary>
        /// 券款对付
        /// </summary>
        public const string DeliveryVersusPayment = "DVP";

        /// <summary>
        /// 纯券过户
        /// </summary>
        public const string FreeOfPayment = "FOP";

        /// <summary>
        /// 券券对付
        /// </summary>
        public const string BondVersusBond = "BVB";

        /// <summary>
        /// 券费对付
        /// </summary>
        public const string BondVersusPayment = "BVP";

        /// <summary>
        /// 返券付费解券
        /// </summary>
        public const string BondAfterBondFree = "BVBF";

        //TT6789 chengdengliang 2011-06-17 净额券款对付结算方式

        /// <summary>
        /// 净额券款对付
        /// </summary>
        public const string NetDeliveryVersusPayment = "NDVP";

        //2013-10-17 成登亮

        /// <summary>
        /// 双边清算
        /// </summary>
        public const string BOTH = "BOTH";

        /// <summary>
        /// 主动扣款 2014-11-13 沈何凯 大宗商品远期 扣款方式
        /// </summary>
        public const string ActiveDebit = "AD";

        /// <summary>
        /// 被动扣款 2014-11-13 沈何凯 大宗商品远期 扣款方式
        /// </summary>
        public const string PassiveDebit = "PD";

        /// <summary>
        /// 交易所--担保交收(2017-07-24 蔡旦旭 TT2550)
        /// </summary>
        public const string SecureSettle = "SecureSet";

        /// <summary>
        /// 暂时不用这个枚举交易所--非担保交收(非RTGS)(2017-07-24 蔡旦旭 TT2550)
        /// 2018-08-13 周志斌 TT9787交易所质押式回购结算方式修改，报价回购需要支持非担保交收(NGGS)
        /// </summary>
        public const string UnSecureSettleNGGS = "UnSecureSetNGGS";

        /// <summary>
        /// 交易所--非担保交收(2017-07-24 蔡旦旭 TT2550)
        /// </summary>
        public const string UnSecureSettleRTGS = "UnSecureSetRTGS";


        /// <summary>
        /// 2021-08-13 王继博 对手账户结算
        /// </summary>
        public const string CounterPartyAccountSettle = "CPAS";


        /// <summary>
        /// 2021-08-13 王继博 直接划付
        /// </summary>
        public const string DirectTransfer = "DIRECT";

        /// <summary>
        /// 其他
        /// 2022-03-04 冯彦谕 P008XIR-31852 江海-债券认购，增加结算方式：“其他”
        /// 2022-03-04 冯彦谕 P008XIR-32198 开源证券-债券认购界面结算类型增加线下划款
        /// </summary>
        public const string OTHER = "OTHER";


    }
}
