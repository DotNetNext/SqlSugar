 
using SqlSugar;

namespace OrmTest
{
    /// <summary>
    /// 终端平台配置信息表
    /// </summary>
    public class TB_ClientConfig  
    {
        /// <summary>
        /// 终端平台配置信息表
        /// </summary>
        public TB_ClientConfig()
        {
        }

        /// <summary>
        /// 终端平台配置信息表
        /// </summary>
        /// <param name="setid">是否给ID赋值</param>
        public TB_ClientConfig(bool setid)
        {
            
        }

        /// <summary>
        /// 主键 雪花ID. 平台唯一Key
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int64 ID { get; set; }

        /// <summary>
        /// 平台名称. 如: 友玩安卓APP
        /// </summary>
        public System.String Name { get; set; }  

       
        /// <summary>
        /// 微信开放平台的AppID
        /// </summary>
        public System.String  OpenWechatAppID { get; set; }

        /// <summary>
        /// 微信开放平台的AppSecret
        /// </summary>
        public System.String  OpenWechatAppSecret { get; set; }

        /// <summary>
        /// 微信支付的商户号
        /// </summary>
        public System.String  WechatPayMchID { get; set; }

        /// <summary>
        /// 微信支付的API签名Key
        /// </summary>
        public System.String  WechatPayApiKey { get; set; }

        /// <summary>
        /// 微信支付V3版本的API签名Key
        /// </summary>
        public System.String  WechatPayApiKeyV3 { get; set; }

        /// <summary>
        /// 微信支付是否开启
        /// </summary>
        public System.Boolean WechatPaymentOpen { get; set; }

        /// <summary>
        /// 微信提现是否开启
        /// </summary>
        public System.Boolean WechatWithdrawOpen { get; set; }

        /// <summary>
        /// 微信小程序原始ID
        /// </summary>
        public System.String  WechatMiniOriginalID { get; set; }

        /// <summary>
        /// 支付宝支付的AppID
        /// </summary>
        public System.String  AlipayAppID { get; set; }

        /// <summary>
        /// 支付宝支付的私钥
        /// </summary>
        public System.String  AlipayPrivateKey { get; set; }

        /// <summary>
        /// 支付宝支付的公钥
        /// </summary>
        public System.String  AlipayPublicKey { get; set; }

        /// <summary>
        /// 支付宝支付是否开启
        /// </summary>
        public System.Boolean AlipayPaymentOpen { get; set; }

        /// <summary>
        /// 支付宝提现是否开启
        /// </summary>
        public System.Boolean AlipayWithdrawOpen { get; set; }

        /// <summary>
        /// 是否有效
        /// </summary>
        public System.Boolean IsValid { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public System.Int64 CreateDateTime { get; set; }

        /// <summary>
        /// 创建后台用户ID
        /// </summary>
        public System.Int64 CreateAdminUserID { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public System.Int64? ModifyDateTime { get; set; }

        /// <summary>
        /// 最后修改的后台用户ID
        /// </summary>
        public System.Int64? ModifyAdminUserID { get; set; }

        /// <summary>
        /// 扩展信息, 一般为JSON格式
        /// </summary>
        public System.String  Extension { get; set; }
    }
}