 using SqlSugar;
namespace DorisTest
{

    /// <summary>
    /// 日志记录表
    ///</summary>
    [SugarTable("LogEntity111")]
    public class LogEntity
    {
        /// <summary>
        /// 主键
        ///</summary>
        [SugarColumn(IsPrimaryKey = true)]
        public long Id { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string? Url { get; set; }

        /// <summary>
        /// 方法
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string? Method { get; set; }

        /// <summary>
        /// 数据-请求体
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string? Data { get; set; }

        /// <summary>
        /// 数据-返回(包括异常信息)
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string? RsData { get; set; }

        /// <summary>
        /// 是否成功
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public bool? IsSuccess { get; set; }

        /// <summary>
        /// Header
        /// </summary>
        [SugarColumn(IsNullable = true, IsJson = true )]
        public Dictionary<string, string> Header { get; set; }

        /// <summary>
        /// Token
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string? Token { get; set; }

        /// <summary>
        /// 客户端ip
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string ClientHost { get; set; }

        /// <summary>
        ///     应用标识
        /// </summary> 
        [SugarColumn(IsNullable = true)]
        public string? AppId { get; set; }

        /// <summary>
        /// 应用的Ip
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string? AppHost { get; set; }

        /// <summary>
        /// 持续时长
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int? RunTime { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime /*long*/ CreateTime { get; set; }

        public LogEntity()
        { 
            CreateTime = DateTime.Now;
            //CreateTime = DateTime.Now.ToFileTimeUtc();
        }
    }
}