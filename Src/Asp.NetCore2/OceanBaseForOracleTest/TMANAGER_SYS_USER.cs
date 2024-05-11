using SqlSugar;
using System;
using System.Collections.Generic;

namespace xTPLM.RFQ.Model.XMANAGER_APP
{
    ///<summary>
    ///用户表
    ///</summary>
    [SugarTable("TMANAGER_SYS_USER")]
    public partial class TMANAGER_SYS_USER
    {
        public TMANAGER_SYS_USER()
        {


        }
        /// <summary>
        /// Desc:主键
        /// Default:
        /// Nullable:False
        /// </summary>           
        [SugarColumn(IsNullable = false, IsPrimaryKey = true, OracleSequenceName = "TMANAGER_SYS_USER$SEQ", IsIdentity = true)]
        public int U_ID { get; set; }

        /// <summary>
        /// Desc:用户名
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, Length = 48)]
        public string U_NAME { get; set; }

        /// <summary>
        /// Desc:密码
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, Length = 48)]
        public string U_PWD { get; set; }

        /// <summary>
        /// Desc:性别
        /// Default:
        /// Nullable:False
        /// </summary>           
        [SugarColumn(IsNullable = false, Length = 1)]
        public string U_SEX { get; set; }

        /// <summary>
        /// Desc:邮件地址
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, Length = 100)]
        public string U_EMAIL { get; set; }

        /// <summary>
        /// Desc:手机号
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, Length = 20)]
        public string U_MOBILE { get; set; }

        /// <summary>
        /// Desc:备注
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, Length = 1000)]
        public string U_REMARK { get; set; }

        /// <summary>
        /// Desc:逻辑删除
        /// Default:
        /// Nullable:False
        /// </summary>           
        [SugarColumn(IsNullable = false, Length = 1)]
        public string DELETED { get; set; } = "0";

        /// <summary>
        /// Desc:更新人
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, Length = 32)]
        public string UPDATED_BY { get; set; }

        /// <summary>
        /// Desc:更新时间
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true)]
        public DateTime? UPDATED_TIME { get; set; }

        /// <summary>
        /// Desc:用户启用状态 1：启用,0：禁用
        /// Default:
        /// Nullable:False
        /// </summary>           
        [SugarColumn(IsNullable = false)]
        public int U_STATUS { get; set; }

        /// <summary>
        /// Desc:昵称
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, Length = 255)]
        public string U_NICKNAME { get; set; }

        /// <summary>
        /// Desc:部门ID
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true)]
        public int? U_DID { get; set; }

        /// <summary>
        /// Desc:职务
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true)]
        public int? U_POSITION { get; set; }

        /// <summary>
        /// Desc:是否是IR单点登录用户 0:不是，1:是
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true)]
        public int? IS_IRUSER { get; set; }

        /// <summary>
        /// IR用户
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string USER_CODE { get => this.IR_CODE; }

        /// <summary>
        /// 部门名称
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string D_NAME { get; set; }

        /// <summary>
        /// IR账户名
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string IR_CODE { get; set; }

    }
}
