
using System;
using System.Linq;
using System.Text;
using SqlSugar;


namespace SqlSeverTest.UserTestCases.UnitTest.Unitasf1
{
    ///<summary>
    ///应收账单
    ///</summary>
    [SugarTable("R01_ReceivableBills")]
    [Serializable]
    public partial class R01_ReceivableBills
    {
        public R01_ReceivableBills()
        {
            R01_BillAmount = Convert.ToDouble("0");
            R01_PaidAmount = Convert.ToDouble("0");
            R01_DiscountAmount = Convert.ToDouble("0");
            R01_IsRefund = Convert.ToByte("0");
            R01_Status = Convert.ToByte("0");
            R01_IsValid = Convert.ToByte("0");

        }
        /// <summary>
        /// Desc:应收账单Id
        /// Default:
        /// Nullable:False
        /// </summary>

        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long R01_ReceivableBillId { get; set; }
        /// <summary>
        /// Desc:应收账单编号
        /// Default:
        /// Nullable:False
        /// </summary>

        public string R01_ReceivableBillNo { get; set; }
        /// <summary>
        /// Desc:预账单Id
        /// Default:
        /// Nullable:False
        /// </summary>

        public long R04_PreBillId { get; set; }
        /// <summary>
        /// Desc:预账单编号
        /// Default:
        /// Nullable:True
        /// </summary>

        public string R04_PreBillNo { get; set; }
        /// <summary>
        /// Desc:客户产品Id
        /// Default:
        /// Nullable:False
        /// </summary>

        public long C08_CustomerProductId { get; set; }
        /// <summary>
        /// Desc:客户Id
        /// Default:
        /// Nullable:False
        /// </summary>

        public long C02_CustomerId { get; set; }
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

        public double R01_BillAmount { get; set; }
        /// <summary>
        /// Desc:已付金额 累加
        /// Default:0
        /// Nullable:False
        /// </summary>

        public double R01_PaidAmount { get; set; }
        /// <summary>
        /// Desc:优惠金额 累加
        /// Default:0
        /// Nullable:False
        /// </summary>

        public double R01_DiscountAmount { get; set; }
        /// <summary>
        /// Desc:计划缴费时间
        /// Default:
        /// Nullable:True
        /// </summary>

        public DateTime? R01_PlanPayTime { get; set; }
        /// <summary>
        /// Desc:应收责任人Id
        /// Default:
        /// Nullable:True
        /// </summary>

        public long? R01_BillResponsibleId { get; set; }
        /// <summary>
        /// Desc:是否退费 0否 1是
        /// Default:0
        /// Nullable:False
        /// </summary>

        public byte R01_IsRefund { get; set; }
        /// <summary>
        /// Desc:账单状态 0待完成 1未完成 2完成
        /// Default:0
        /// Nullable:False
        /// </summary>

        public byte R01_Status { get; set; }
        /// <summary>
        /// Desc:备注
        /// Default:
        /// Nullable:True
        /// </summary>

        public string R01_Remark { get; set; }
        /// <summary>
        /// Desc:是否有效 0有效 1无效
        /// Default:0
        /// Nullable:False
        /// </summary>

        public byte R01_IsValid { get; set; }
        /// <summary>
        /// Desc:创建者Id
        /// Default:
        /// Nullable:False
        /// </summary>

        public long R01_CreateId { get; set; }
        /// <summary>
        /// Desc:创建者名称
        /// Default:
        /// Nullable:False
        /// </summary>

        public string R01_CreateBy { get; set; }
        /// <summary>
        /// Desc:创建日期
        /// Default:
        /// Nullable:False
        /// </summary>

        public DateTime R01_CreateTime { get; set; }
        /// <summary>
        /// Desc:更新者Id
        /// Default:
        /// Nullable:True
        /// </summary>

        public long? R01_ModifyId { get; set; }
        /// <summary>
        /// Desc:更新者名称
        /// Default:
        /// Nullable:True
        /// </summary>

        public string R01_ModifyBy { get; set; }
        /// <summary>
        /// Desc:更新日期
        /// Default:
        /// Nullable:True
        /// </summary>

        public DateTime? R01_ModifyTime { get; set; }
        /// <summary>
        /// Desc:删除者Id
        /// Default:
        /// Nullable:True
        /// </summary>

        public long? R01_DeleteId { get; set; }
        /// <summary>
        /// Desc:删除者名称
        /// Default:
        /// Nullable:True
        /// </summary>

        public string R01_DeleteBy { get; set; }
        /// <summary>
        /// Desc:删除日期
        /// Default:
        /// Nullable:True
        /// </summary>

        public DateTime? R01_DeleteTime { get; set; }
    }
}
