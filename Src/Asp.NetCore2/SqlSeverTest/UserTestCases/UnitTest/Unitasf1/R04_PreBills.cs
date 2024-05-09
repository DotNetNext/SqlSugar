
using System;
using System.Linq;
using System.Text;
using SqlSugar;


namespace SqlSeverTest.UserTestCases.UnitTest.Unitasf1
{
    ///<summary>
    ///预账单
    ///</summary>
    [SugarTable("R04_PreBills")]
    [Serializable]
    public partial class R04_PreBills
    {
        public R04_PreBills()
        {
            R04_BillAmount = Convert.ToDouble("0");
            R04_PaidAmount = Convert.ToDouble("0");
            R04_DiscountAmount = Convert.ToDouble("0");
            R04_IsRefund = Convert.ToByte("0");
            R04_Status = Convert.ToByte("0");
            R04_IsValid = Convert.ToByte("0");

        }
        /// <summary>
        /// Desc:预账单Id
        /// Default:
        /// Nullable:False
        /// </summary>

        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long R04_PreBillId { get; set; }
        /// <summary>
        /// Desc:预账单编号
        /// Default:
        /// Nullable:False
        /// </summary>

        public string R04_PreBillNo { get; set; }
        /// <summary>
        /// Desc:线索Id
        /// Default:
        /// Nullable:True
        /// </summary>

        public long? C01_ClueId { get; set; }
        /// <summary>
        /// Desc:客户产品Id
        /// Default:
        /// Nullable:True
        /// </summary>

        public long? C08_CustomerProductId { get; set; }
        /// <summary>
        /// Desc:客户Id
        /// Default:
        /// Nullable:True
        /// </summary>

        public long? C02_CustomerId { get; set; }
        /// <summary>
        /// Desc:客户名称
        /// Default:
        /// Nullable:True
        /// </summary>

        public string C02_CustomerName { get; set; }
        /// <summary>
        /// Desc:联系方式
        /// Default:
        /// Nullable:True
        /// </summary>

        public string C02_Contact { get; set; }
        /// <summary>
        /// Desc:招生来源
        /// Default:
        /// Nullable:True
        /// </summary>

        public long? C02_Origin { get; set; }
        /// <summary>
        /// Desc:项目Id
        /// Default:
        /// Nullable:True
        /// </summary>

        public long? P01_ProjectId { get; set; }
        /// <summary>
        /// Desc:项目名称
        /// Default:
        /// Nullable:True
        /// </summary>

        public string P01_ProjectName { get; set; }
        /// <summary>
        /// Desc:产品Id
        /// Default:
        /// Nullable:False
        /// </summary>

        public long P01_ProductId { get; set; }
        /// <summary>
        /// Desc:产品名称
        /// Default:
        /// Nullable:True
        /// </summary>

        public string P01_ProductName { get; set; }
        /// <summary>
        /// Desc:产品费用Id
        /// Default:
        /// Nullable:True
        /// </summary>

        public long? P03_ProductExpenseId { get; set; }
        /// <summary>
        /// Desc:产品费用类型 0通用 1专用
        /// Default:
        /// Nullable:True
        /// </summary>

        public byte? P03_ProductExpenseType { get; set; }
        /// <summary>
        /// Desc:费用科目Id
        /// Default:
        /// Nullable:True
        /// </summary>

        public long? S15_ExpenseId { get; set; }
        /// <summary>
        /// Desc:费用类型 0收 1支 2收&支
        /// Default:
        /// Nullable:True
        /// </summary>

        public byte? S15_Type { get; set; }
        /// <summary>
        /// Desc:费用名称
        /// Default:
        /// Nullable:True
        /// </summary>

        public string S15_ExpenseName { get; set; }
        /// <summary>
        /// Desc:费用别名
        /// Default:
        /// Nullable:True
        /// </summary>

        public string P03_ExpenseAlias { get; set; }
        /// <summary>
        /// Desc:代理Id
        /// Default:
        /// Nullable:True
        /// </summary>

        public long? A01_AgentId { get; set; }
        /// <summary>
        /// Desc:是否代收代付 0否 1是
        /// Default:
        /// Nullable:True
        /// </summary>

        public byte? P03_IsAgentPay { get; set; }
        /// <summary>
        /// Desc:协议
        /// Default:
        /// Nullable:True
        /// </summary>

        public string P03_Agreement { get; set; }
        /// <summary>
        /// Desc:协议开始时间
        /// Default:
        /// Nullable:True
        /// </summary>

        public DateTime? P03_AgreementStartTime { get; set; }
        /// <summary>
        /// Desc:协议结束时间
        /// Default:
        /// Nullable:True
        /// </summary>

        public DateTime? P03_AgreementEndTime { get; set; }
        /// <summary>
        /// Desc:账单金额
        /// Default:0
        /// Nullable:False
        /// </summary>

        public double R04_BillAmount { get; set; }
        /// <summary>
        /// Desc:已付金额 累加
        /// Default:0
        /// Nullable:False
        /// </summary>

        public double R04_PaidAmount { get; set; }
        /// <summary>
        /// Desc:优惠金额 累加
        /// Default:0
        /// Nullable:False
        /// </summary>

        public double R04_DiscountAmount { get; set; }
        /// <summary>
        /// Desc:计划缴费时间
        /// Default:
        /// Nullable:True
        /// </summary>

        public DateTime? R04_PlanPayTime { get; set; }
        /// <summary>
        /// Desc:应收责任人
        /// Default:
        /// Nullable:True
        /// </summary>

        public long? R04_BillResponsibleId { get; set; }
        /// <summary>
        /// Desc:是否退费 0否 1是
        /// Default:0
        /// Nullable:False
        /// </summary>

        public byte R04_IsRefund { get; set; }
        /// <summary>
        /// Desc:账单状态 0待完成 1未完成 2完成
        /// Default:0
        /// Nullable:False
        /// </summary>

        public byte R04_Status { get; set; }
        /// <summary>
        /// Desc:操作类型 0新增 1删除 2编辑
        /// Default:
        /// Nullable:True
        /// </summary>

        public byte? R04_OperationType { get; set; }
        /// <summary>
        /// Desc:备注
        /// Default:
        /// Nullable:True
        /// </summary>

        public string R04_Remark { get; set; }
        /// <summary>
        /// Desc:是否有效 0有效 1无效
        /// Default:0
        /// Nullable:False
        /// </summary>

        public byte R04_IsValid { get; set; }
        /// <summary>
        /// Desc:创建者Id
        /// Default:
        /// Nullable:False
        /// </summary>

        public long R04_CreateId { get; set; }
        /// <summary>
        /// Desc:创建者名称
        /// Default:
        /// Nullable:False
        /// </summary>

        public string R04_CreateBy { get; set; }
        /// <summary>
        /// Desc:创建日期
        /// Default:
        /// Nullable:False
        /// </summary>

        public DateTime R04_CreateTime { get; set; }
        /// <summary>
        /// Desc:更新者Id
        /// Default:
        /// Nullable:True
        /// </summary>

        public long? R04_ModifyId { get; set; }
        /// <summary>
        /// Desc:更新者名称
        /// Default:
        /// Nullable:True
        /// </summary>

        public string R04_ModifyBy { get; set; }
        /// <summary>
        /// Desc:更新日期
        /// Default:
        /// Nullable:True
        /// </summary>

        public DateTime? R04_ModifyTime { get; set; }
        /// <summary>
        /// Desc:删除者Id
        /// Default:
        /// Nullable:True
        /// </summary>

        public long? R04_DeleteId { get; set; }
        /// <summary>
        /// Desc:删除者名称
        /// Default:
        /// Nullable:True
        /// </summary>

        public string R04_DeleteBy { get; set; }
        /// <summary>
        /// Desc:删除日期
        /// Default:
        /// Nullable:True
        /// </summary>

        public DateTime? R04_DeleteTime { get; set; }
    }
}
