
using SqlSugar;

namespace xTPLM.RFQ.Model.XRFQ_APP
{
    ///<summary>
    ///计划任务表
    ///</summary>
    [SugarTable("TRFQ_SYS_TASK")]
    public partial class TRFQ_SYS_TASK
    {
        public TRFQ_SYS_TASK()
        {


        }
        /// <summary>
        /// Desc:主键
        /// Default:
        /// Nullable:False
        /// </summary>           
        [SugarColumn(IsNullable = false, IsPrimaryKey = true, OracleSequenceName = "TRFQ_TASK$SEQ", IsIdentity = true)]
        public int ID { get; set; }

        /// <summary>
        /// Desc:任务名称
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, Length = 128)]
        public string TASKNAME { get; set; }

        /// <summary>
        /// Desc:执行ID
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true)]
        public int? JOB_ID { get; set; }

        /// <summary>
        /// Desc:任务配置
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, Length = 1024)]
        public string TASK_CONFIG { get; set; }

        /// <summary>
        /// Desc:状态1:启用，0:禁用
        /// Default:
        /// Nullable:False
        /// </summary>           
        [SugarColumn(IsNullable = false)]
        public int STATUS { get; set; }

        /// <summary>
        /// Desc:上一次执行时间
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true)]
        public DateTime? PREVIOUS_FIRE_TIME { get; set; }

        /// <summary>
        /// Desc:下一次执行时间
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true)]
        public DateTime? NEXT_FIRE_TIME { get; set; }

        /// <summary>
        /// Desc:已执行次数
        /// Default:
        /// Nullable:False
        /// </summary>           
        [SugarColumn(IsNullable = false)]
        public int COUNT { get; set; }

        /// <summary>
        /// Desc:最后一次修改人
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, Length = 32)]
        public string UPDATE_BY { get; set; }

        /// <summary>
        /// Desc:最后一次修改时间
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true)]
        public DateTime? UPDATE_TIME { get; set; }

        /// <summary>
        /// 工作名称
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string JOBNAME { get; set; }

        /// <summary>
        /// 配置说明
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string TASK_CONFIG_DESCRIPTION { get; set; }

    }
}
