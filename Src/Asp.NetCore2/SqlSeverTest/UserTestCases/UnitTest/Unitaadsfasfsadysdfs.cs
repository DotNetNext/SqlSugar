using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrmTest
{
    public class Unitdfasdfasfysa
    {
       public static void Init()
        {
            var db = NewUnitTest.Db;
            var clinicId = "1111";
            db.CodeFirst.InitTables<RecipeEntity, RecipeDetailEntity, UsageEntity>();
            var rpDetailIds = new List<string>()
            {
                "111", "22",
            };
            var isAutoReg = true;
            // 1. 这种方式，直接写isAutoReg会报错
            /* 生成sql：
             * SELECT  TOP 1 1 FROM [CT_Recipe] [rp] WITH(NOLOCK)  Inner JOIN [CT_RecipeDetail] [det] WITH(NOLOCK) ON (( [det].[RecipeId] = [rp].[RecipeId] ) AND ( [rp].[Status] = 2 ))  Inner JOIN [Sys_Usage] [u] WITH(NOLOCK) ON ( [u].[UsageId] = [det].[UsageRouteId] )   WHERE (( [rp].[ClinicId] = N'1111' ) AND  ([det].[RecipeDetId] IN ('111','22')) )  AND (((( [u].[Category] = 2 ) OR ( [u].[Category] = 3 )) OR ( [u].[Category] = 4 )) OR (( 1 AND( [det].[IsSkinTest] = 1 )) AND ( [det].[SkinTestSign] = 3 )))
             */
            try
            {
                var bb = db.Queryable<RecipeEntity, RecipeDetailEntity, UsageEntity>((rp, det, u) => new object[]
                   {
                                                JoinType.Inner, det.RecipeId == rp.RecipeId && rp.Status == 2,
                                                JoinType.Inner, u.UsageId == det.UsageRouteId,
                   })
                    //.Where((rp, det, u) => rp.ClinicId == clinicId && rpDetailIds.Contains(det.RecipeDetId))
                    .Where((rp, det, u) => u.Category == 2 ||
                    u.Category == 3 || u.Category == 4
                    || (isAutoReg && det.IsSkinTest == 1 && det.SkinTestSign == 3))
                    .Any();
            }
            catch (Exception)
            {

                throw;
            }
            // 2. 写成isAutoReg == true 是可以正常生成 1=1条件
            /*生成sql：
             * SELECT  TOP 1 1 FROM [CT_Recipe] [rp] WITH(NOLOCK)  Inner JOIN [CT_RecipeDetail] [det] WITH(NOLOCK) ON (( [det].[RecipeId] = [rp].[RecipeId] ) AND ( [rp].[Status] = 2 ))  Inner JOIN [Sys_Usage] [u] WITH(NOLOCK) ON ( [u].[UsageId] = [det].[UsageRouteId] )   WHERE (( [rp].[ClinicId] = N'1111' ) AND  ([det].[RecipeDetId] IN ('111','22')) )  AND (((( [u].[Category] = 2 ) OR ( [u].[Category] = 3 )) OR ( [u].[Category] = 4 )) OR ((( 1 = 1 ) AND ( [det].[IsSkinTest] = 1 )) AND ( [det].[SkinTestSign] = 3 )))
             */
            try
            {
                var bb = db.Queryable<RecipeEntity, RecipeDetailEntity, UsageEntity>((rp, det, u) => new object[]
                   {
                                                JoinType.Inner, det.RecipeId == rp.RecipeId && rp.Status == 2,
                                                JoinType.Inner, u.UsageId == det.UsageRouteId,
                   }).Where((rp, det, u) => rp.ClinicId == clinicId && rpDetailIds.Contains(det.RecipeDetId))
                    .Where((rp, det, u) => u.Category == 2 ||
                    u.Category == 3 || u.Category == 4
                    || (isAutoReg == true && det.IsSkinTest == 1 && det.SkinTestSign == 3))
                    .Any();
            }
            catch (Exception)
            {
                throw;
            }
            // 3. 写成isAutoReg == false 是可以正常生成 1=0条件
            /*生成sql：
             * SELECT  TOP 1 1 FROM [CT_Recipe] [rp] WITH(NOLOCK)  Inner JOIN [CT_RecipeDetail] [det] WITH(NOLOCK) ON (( [det].[RecipeId] = [rp].[RecipeId] ) AND ( [rp].[Status] = 2 ))  Inner JOIN [Sys_Usage] [u] WITH(NOLOCK) ON ( [u].[UsageId] = [det].[UsageRouteId] )   WHERE (( [rp].[ClinicId] = N'1111' ) AND  ([det].[RecipeDetId] IN ('111','22')) )  AND (((( [u].[Category] = 2 ) OR ( [u].[Category] = 3 )) OR ( [u].[Category] = 4 )) OR ((( 1 = 0 ) AND ( [det].[IsSkinTest] = 1 )) AND ( [det].[SkinTestSign] = 3 )))
             */
            try
            {
                var bb = db.Queryable<RecipeEntity, RecipeDetailEntity, UsageEntity>((rp, det, u) => new object[]
                   {
                                                JoinType.Inner, det.RecipeId == rp.RecipeId && rp.Status == 2,
                                                JoinType.Inner, u.UsageId == det.UsageRouteId,
                   }).Where((rp, det, u) => rp.ClinicId == clinicId && rpDetailIds.Contains(det.RecipeDetId))
                    .Where((rp, det, u) => u.Category == 2 ||
                    u.Category == 3 || u.Category == 4
                    || (isAutoReg == false && det.IsSkinTest == 1 && det.SkinTestSign == 3))
                    .Any();
            }
            catch (Exception)
            {
                throw;
            } 
        }
        /// <summary>
        /// 门诊处方明细
        /// </summary>
        [SugarTable("UnitdfaaCT_RecipeDetail")]
        public class RecipeDetailEntity
        {
            /// <summary>
            /// 处方明细编号
            /// </summary>
            [SugarColumn(IsPrimaryKey = true)]
            public string RecipeDetId { get; set; }

            /// <summary>
            /// 处方编号
            /// </summary>
            public string RecipeId { get; set; }

            /// <summary>
            /// 处方内项目序号
            /// </summary>
            public int Sort { get; set; }

            /// <summary>
            /// 处方取整方式 1.按次取整  2.按天取整  3.按实际发  4.医嘱取整  9.不自动计算
            /// </summary>
            public int? SplitType { get; set; }

            /// <summary>
            /// 项目编号
            /// </summary>
            public string ItemId { get; set; }

            /// <summary>
            /// 项目名称
            /// </summary>
            public string ItemName { get; set; }

            /// <summary>
            /// 项目规格
            /// </summary>
            public string ItemSpec { get; set; }

            /// <summary>
            /// 项目类型1.药品  2.项目 3项目组合
            /// </summary>
            public int ItemType { get; set; }

            /// <summary>
            /// 组号（不分组，默认为0）
            /// </summary>
            public int GroupNo { get; set; }

            /// <summary>
            /// 组显示标志（开始、中间、结束，不分组默认为空）
            /// </summary>
            public string GroupTag { get; set; }

            /// <summary>
            /// 单次用量
            /// </summary>
            public decimal SingleDose { get; set; }

            /// <summary>
            /// 剂量系数
            /// </summary>
            public decimal DoseRate { get; set; }

            /// <summary>
            /// 剂量单位
            /// </summary>
            public string DoseUnit { get; set; }

            /// <summary>
            /// 项目数量，最小单位
            /// </summary>
            public decimal MiniUnitCount { get; set; }

            /// <summary>
            /// 项目单位、最小单位
            /// </summary>
            public string MiniUnitName { get; set; }

            /// <summary>
            /// 门诊开药数量
            /// </summary>
            public decimal ClinicUnitCount { get; set; }

            /// <summary>
            /// 门诊开药单位
            /// </summary>
            public string ClinicUnitName { get; set; }

            /// <summary>
            /// 门诊开药单位系数
            /// </summary>
            public int ClinicUnitRate { get; set; }

            /// <summary>
            /// 频次编号
            /// </summary>
            public string FreqId { get; set; }

            /// <summary>
            /// 频次代码
            /// </summary>
            public string FreqCode { get; set; }

            /// <summary>
            /// 频次名称
            /// </summary>
            public string FreqName { get; set; }

            /// <summary>
            /// 执行次数
            /// </summary>
            public int? FreqTimes { get; set; }

            /// <summary>
            /// 执行时间点
            /// </summary>
            public string FreqExecPoints { get; set; }

            /// <summary>
            /// 药品给药途径编号
            /// </summary>
            public string UsageRouteId { get; set; }

            /// <summary>
            /// 药品给药途径名称
            /// </summary>
            public string UsageRouteName { get; set; }

            /// <summary>
            /// 执行天数(默认1)
            /// </summary>
            public int Days { get; set; }

            /// <summary>
            /// 每分钟滴速
            /// </summary>
            public decimal? DroppingSpeedPerMinute { get; set; }

            /// <summary>
            /// 记速单位（类似d/min或者ml/h）
            /// </summary>
            public string SpeedRecordUnit { get; set; }

            /// <summary>
            /// 已经执行次数(记录输液、注射等单据打印次数)
            /// </summary>
            public int? ExecutedTimes { get; set; }

            /// <summary>
            /// 最后一次执行时间
            /// </summary>
            public DateTime? LastExecutedTime { get; set; }

            /// <summary>
            /// 是否皮试药品
            /// </summary>
            public int IsSkinTest { get; set; }

            /// <summary>
            /// 皮试标注
            /// </summary>
            public int? SkinTestSign { get; set; }

            /// <summary>
            /// 是否抗菌药物(0否1是)
            /// </summary>
            public int? IsAntibacterials { get; set; }

            /// <summary>
            /// 中医处方-煎煮法（打碎、先煎、后下等）字典:2004
            /// </summary>
            public string DecoctionWayId { get; set; }

            /// <summary>
            /// 中医处方-煎煮法名称 字典:2004
            /// </summary>
            public string DecoctionWayName { get; set; }

            /// <summary>
            /// 执行科室编号
            /// </summary>
            public string ExecDeptId { get; set; }

            /// <summary>
            /// 执行科室名称
            /// </summary>
            public string ExecDeptName { get; set; }

            /// <summary>
            /// 已退数量
            /// </summary>
            public decimal? ReturnClinicUnitCount { get; set; }

            /// <summary>
            /// 嘱托
            /// </summary>
            public string Memo { get; set; }

            /// <summary>
            /// 美康药品处方明细唯一标识
            /// </summary>
            public string MKIndex { get; set; }

            /// <summary>
            /// 中公网明细信息编号
            /// </summary>
            public string ZgwDetailID { get; set; }

            /// <summary>
            /// 代煎费用关联草药处方编号
            /// </summary>
            public string RelHerbalRecipeId { get; set; }

            /// <summary>
            /// 是否医保限制用药(0否1是)
            /// </summary>
            public int? IsMedUseLimit { get; set; }

            /// <summary>
            /// 医保限制审批标志(枚举：MedUseApprFlagEnum)
            /// </summary>
            public int? MedUseApprFlag { get; set; }

            /// <summary>
            /// 确费时间
            /// </summary>
            public DateTime? ConfirmFeeTime { get; set; }

            /// <summary>
            /// 确费科室编号
            /// </summary>
            public string ConfirmFeeDeptId { get; set; }

            /// <summary>
            /// 确费科室名称
            /// </summary>
            public string ConfirmFeeDeptName { get; set; }

            /// <summary>
            /// 确费员工编号
            /// </summary>
            public string ConfirmFeeEmployeeId { get; set; }

            /// <summary>
            /// 确费员工姓名
            /// </summary>
            public string ConfirmFeeEmployeeName { get; set; }

            /// <summary>
            /// 手术描述
            /// </summary>
            public string OperationMemo { get; set; }

            /// <summary>
            /// 手术申请编号
            /// </summary>
            public string ApplyId { get; set; }

            /// <summary>
            /// 是否特殊费用
            /// </summary>
            public int? IsSpecifiedPrice { get; set; }

            /// <summary>
            /// 数据标签编号(前端生成为值)
            /// </summary>
            public string TagId { get; set; }

            /// <summary>
            /// 是否可以编辑(枚举:BoolStatusEnum 0否1是)
            /// </summary>
            public int? IsCanEdit { get; set; }

            /// <summary>
            /// 剂量代码
            /// </summary>
            public string DoseCode { get; set; }

            /// <summary>
            /// 给药科室编号
            /// </summary>
            public string GiveDrugDeptId { get; set; }

            /// <summary>
            /// 给药科室名称
            /// </summary>
            public string GiveDrugDeptName { get; set; }

            /// <summary>
            /// 门诊输液是否打印
            /// </summary>
            public int? InfusionIsPrint { get; set; }

            /// <summary>
            /// 小包装包数
            /// </summary>
            public int? SmallPackageCount { get; set; }

            /// <summary>
            /// 小包装取整后剂量代码
            /// </summary>
            public string SmallPackageRoundingDoseCode { get; set; }

            /// <summary>
            /// 排序2
            /// </summary>
            public int? Sort2 { get; set; }

            /// <summary>
            /// 医生确认保存标志
            /// </summary>
            public int? DoctorConfirmSaved { get; set; }

            /// <summary>
            /// 配伍禁忌药品
            /// </summary>
            public string CompatibleTabooDrugs { get; set; }
        }
        /// <summary>
        /// 门诊处方
        /// </summary>
        [SugarTable("UnitdfaaCT_Recipe")]
        public class RecipeEntity
        {
            /// <summary>
            /// 处方编号
            /// </summary>
            [SugarColumn(IsPrimaryKey = true)]
            public string RecipeId { get; set; }

            /// <summary>
            /// 门诊号
            /// </summary>
            public string ClinicId { get; set; }

            /// <summary>
            /// 接诊编号
            /// </summary>
            public string ReceiveId { get; set; }

            /// <summary>
            /// 患者ID
            /// </summary>
            public string PatientId { get; set; }

            /// <summary>
            /// 患者姓名
            /// </summary>
            public string PatientName { get; set; }

            /// <summary>
            /// 数据类型   1.西成药 2.中草药 3.检查项目 5.检验项目 6.诊疗项目 99.门诊划价
            /// </summary>
            public int DataType { get; set; }

            /// <summary>
            /// 处方类型 字典:2001
            /// </summary>
            public string RecipeType { get; set; }

            /// <summary>
            /// 特殊处方标记(1急诊处方2儿科处方)
            /// </summary>
            public int? SpecialRecipeSign { get; set; }

            /// <summary>
            /// 处方类型名称 字典:2001
            /// </summary>
            public string RecipeTypeName { get; set; }

            /// <summary>
            /// 中药-总剂数
            /// </summary>
            public int? TotalPairCount { get; set; }

            /// <summary>
            /// 中药-每日剂数（默认1）
            /// </summary>
            public int? DayPairCount { get; set; }

            /// <summary>
            /// 草药处方剂型编号
            /// </summary>
            public string DosageFormId { get; set; }

            /// <summary>
            /// 草药处方剂型名称
            /// </summary>
            public string DosageFormName { get; set; }

            /// <summary>
            /// 中药类别(1饮片2颗粒剂3小包装)
            /// </summary>
            public int? CMDrugCategory { get; set; }

            /// <summary>
            /// 中药-采用剂型编号 字典:2002
            /// </summary>
            public string UseDoseFormId { get; set; }

            /// <summary>
            /// 中药-采用剂型名称 字典:2002
            /// </summary>
            public string UseDoseFormName { get; set; }

            /// <summary>
            /// 中药-用法编号
            /// </summary>
            public string UsageId { get; set; }

            /// <summary>
            /// 中药-用法名称
            /// </summary>
            public string UsageName { get; set; }

            /// <summary>
            /// 状态  1.已录入 2.已提交
            /// </summary>
            public int Status { get; set; }

            /// <summary>
            /// 开方医生的科室编号
            /// </summary>
            public string DrDeptId { get; set; }

            /// <summary>
            /// 开方医生的科室名称
            /// </summary>
            public string DrDeptName { get; set; }

            /// <summary>
            /// 开药医生编号
            /// </summary>
            public string DrEmployeeId { get; set; }

            /// <summary>
            /// 开药医生姓名
            /// </summary>
            public string DrEmployeeName { get; set; }

            /// <summary>
            /// 机构编号
            /// </summary>
            public string InputOrgId { get; set; }

            /// <summary>
            /// 录入科室编号
            /// </summary>
            public string InputDeptId { get; set; }

            /// <summary>
            /// 录入科室名称
            /// </summary>
            public string InputDeptName { get; set; }

            /// <summary>
            /// 录入职工编号
            /// </summary>
            public string InputEmployeeId { get; set; }

            /// <summary>
            /// 录入职工姓名
            /// </summary>
            public string InputEmployeeName { get; set; }

            /// <summary>
            /// 录入时间
            /// </summary>
            public DateTime InputTime { get; set; }

            /// <summary>
            /// 是否需要审核处方(0否1是)
            /// </summary>
            public int IsNeedAudit { get; set; }

            /// <summary>
            /// 处方诊断编码
            /// </summary>
            public string DiagCode { get; set; }

            /// <summary>
            /// 处方诊断名称
            /// </summary>
            public string DiagName { get; set; }

            /// <summary>
            /// 批量挂号划价结算记录编号
            /// </summary>
            public string BatchId { get; set; }

            /// <summary>
            /// 注意事项
            /// </summary>
            public string Memo { get; set; }

            /// <summary>
            /// 是否打印(0否1是)
            /// </summary>
            public int? IsPrint { get; set; }

            /// <summary>
            /// 精麻毒处方代办人
            /// </summary>
            public string SpecialRecipeAgent { get; set; }

            /// <summary>
            /// 精麻毒处方代办人身份证
            /// </summary>
            public string SpecialRecipeAgentIdCard { get; set; }

            /// <summary>
            /// 中公网主信息编号
            /// </summary>
            public string ZgwMainID { get; set; }

            /// <summary>
            /// 每日用量
            /// </summary>
            public decimal? DecoctingDosage { get; set; }

            /// <summary>
            /// 代煎剂数
            /// </summary>
            public int? DecoctPairCount { get; set; }

            /// <summary>
            /// 体检系统结算单号
            /// </summary>
            public string PEISSettlementId { get; set; }

            /// <summary>
            /// 处方名称
            /// </summary>
            public string RecipeName { get; set; }

            /// <summary>
            /// 是否外送处方
            /// </summary>
            public int? IsDeliveryRecipe { get; set; }

            /// <summary>
            /// 开始时间
            /// </summary>
            public DateTime? BeginTime { get; set; }

            /// <summary>
            /// 草药类型编号
            /// </summary>
            public string HerbalTypeId { get; set; }

            /// <summary>
            /// 草药类型名称
            /// </summary>
            public string HerbalTypeName { get; set; }

            /// <summary>
            /// 数据标签编号(前端生成为值)
            /// </summary>
            public string TagId { get; set; }

            /// <summary>
            /// 费别编号(关联费别表)
            /// </summary>
            public string SettlementTypeId { get; set; }

            /// <summary>
            /// 费别名称
            /// </summary>
            public string SettlementTypeName { get; set; }

            /// <summary>
            /// 病种编码（字典：012011）
            /// </summary>
            public string DiseId { get; set; }

            /// <summary>
            /// 病种名称（字典：012011）
            /// </summary>
            public string DiseName { get; set; }

            /// <summary>
            /// 公费医疗-支付单位编号（字典：001040）
            /// </summary>
            public string PayCompanyId { get; set; }

            /// <summary>
            /// 公费医疗-支付单位名称（字典：001040）
            /// </summary>
            public string PayCompanyName { get; set; }

            /// <summary>
            /// 审核状态
            /// </summary>
            public int AuditStatus { get; set; }

            /// <summary>
            /// 审核时间
            /// </summary>
            public DateTime? AuditTime { get; set; }

            /// <summary>
            /// 审核结果
            /// </summary>
            public string AuditResult { get; set; }

            /// <summary>
            /// 中草药对应药房规则编号
            /// </summary>
            public string RuleId { get; set; }

            /// <summary>
            /// 中草药对应药房处方类型
            /// </summary>
            public string PrescriptionType { get; set; }

            /// <summary>
            /// 是否签到(0否1是)
            /// </summary>
            public int IsSignIn { get; set; }

            /// <summary>
            /// 签到标识
            /// </summary>
            public string SignInLogo { get; set; }

            /// <summary>
            /// 药品拿药序号
            /// </summary>
            public string DrugSort { get; set; }

            /// <summary>
            /// 延长用药时间原因说明
            /// </summary>
            public string ExtendedMedicationDescription { get; set; }

            /// <summary>
            /// 每日用量单位编号
            /// </summary>
            public string DecoctingDosageUnitId { get; set; }

            /// <summary>
            /// 每日用量单位名称
            /// </summary>
            public string DecoctingDosageUnitName { get; set; }

            /// <summary>
            /// 是否协定方
            /// </summary>
            public int? IsAgreedParty { get; set; }

            /// <summary>
            /// 特殊药品每日序号
            /// </summary>
            public string SpecialDrugDailySerialNum { get; set; }

            /// <summary>
            /// 是否是科研处方
            /// </summary>
            public int? IsResearchRecipe { get; set; }

            /// <summary>
            /// 协定方是否可以编辑（枚举：BoolStatusEnum）
            /// </summary>
            public int? AgreedPartyIsCanEdit { get; set; }

            /// <summary>
            /// 优惠类型编号
            /// </summary>
            public string DiscountTypeId { get; set; }

            /// <summary>
            /// 优惠类型名称
            /// </summary>
            public string DiscountTypeName { get; set; }

            /// <summary>
            /// 是否双通道处方
            /// </summary>
            public int? IsDualChannelRecipe { get; set; }

            /// <summary>
            /// 电子处方流转状态 ElectronicPrescriptionStatusEnum
            /// </summary>
            public int? ElectronicPrescriptionStatus { get; set; }

            /// <summary>
            /// 处方追溯码
            /// </summary>
            public string MedTraceCode { get; set; }

            /// <summary>
            /// 医保处方编号
            /// </summary>
            public string MedRecipeNo { get; set; }

            /// <summary>
            /// 预核验操作人ID
            /// </summary>
            public string PreVerificationEmployeeId { get; set; }

            /// <summary>
            /// 预核验操作人姓名
            /// </summary>
            public string PreVerificationEmployeeName { get; set; }

            /// <summary>
            /// 预核验时间
            /// </summary>
            public DateTime? PreVerificationTime { get; set; }

            /// <summary>
            /// 签名操作人
            /// </summary>
            public string SignEmployeeId { get; set; }

            /// <summary>
            /// 签名操作人姓名
            /// </summary>
            public string SignEmployeeName { get; set; }

            /// <summary>
            /// 签名时间
            /// </summary>
            public DateTime? SignTime { get; set; }

            /// <summary>
            /// 上传操作人
            /// </summary>
            public string UploadEmployeeId { get; set; }

            /// <summary>
            /// 上传操作人姓名
            /// </summary>
            public string UploadEmployeeName { get; set; }

            /// <summary>
            /// 上传时间
            /// </summary>
            public DateTime? UploadTime { get; set; }

            /// <summary>
            /// 撤销上传操作人
            /// </summary>
            public string UnUploadEmployeeId { get; set; }

            /// <summary>
            /// 撤销上传操作人姓名
            /// </summary>
            public string UnUploadEmployeeName { get; set; }

            /// <summary>
            /// 撤销上传时间
            /// </summary>
            public DateTime? UnUploadTime { get; set; }

            /// <summary>
            /// 补录时间
            /// </summary>
            public DateTime? SupplementTime { get; set; }
        }    /// <summary>
             /// 药品用法表
             /// </summary>
        [SugarTable("UnitdfaaSys_Usage")]
        public class UsageEntity
        {
            /// <summary>
            /// 用法编号
            /// </summary>
            [SugarColumn(IsPrimaryKey = true)]
            public string UsageId { get; set; }

            /// <summary>
            /// 用法编码
            /// </summary>
            public string UsageCode { get; set; }

            /// <summary>
            /// 用法名称
            /// </summary>
            public string UsageName { get; set; }

            /// <summary>
            /// 允许处方类型(1西医2中医)，多个逗号分隔
            /// </summary>
            public string AllowUseRecipeTypes { get; set; }

            /// <summary>
            /// 分类（1口服2输液3注射4治疗5外用9其它）
            /// </summary>
            public int Category { get; set; }

            /// <summary>
            /// 机构编号(root系统内置)
            /// </summary>
            public string OrgId { get; set; }

            /// <summary>
            /// 排序
            /// </summary>
            public int Sort { get; set; }

            /// <summary>
            /// 拼音
            /// </summary>
            public string SpellCode { get; set; }

            /// <summary>
            /// 五笔
            /// </summary>
            public string StrokeCode { get; set; }

            /// <summary>
            /// 状态(1：启用；2：停用)
            /// </summary>
            public int Status { get; set; }

            /// <summary>
            /// 是否删除（0否1是）
            /// </summary>
            public int IsDelete { get; set; }

            /// <summary>
            /// 备注
            /// </summary>
            public string Memo { get; set; }

            /// <summary>
            /// 记速方式（类似滴速或者流速）
            /// </summary>
            public string SpeedRecordType { get; set; }

            /// <summary>
            /// 记速单位（类似d/min或者ml/h）
            /// </summary>
            public string SpeedRecordUnits { get; set; }

            /// <summary>
            /// 是否必填
            /// </summary>
            public int? IsRequired { get; set; }

            /// <summary>
            /// 适用范围
            /// </summary>
            public string ScopeOfApplication { get; set; }
        }
    }
}
