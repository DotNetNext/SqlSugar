using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
namespace OrmTest
{
    public class UCustom020
    {
        // See https://aka.ms/new-console-template for more information

        public static void Init() 
        {
            var db = NewUnitTest.Db;
            if(!db.DbMaintenance.IsAnyTable("js_crm_customer", false))
            {
                db.CodeFirst.InitTables<CustomerModel>();
            }
            //建表 
            if (!db.DbMaintenance.IsAnyTable("js_crm_customertypelevelnew", false))
            {
                db.CodeFirst.InitTables<CustomertypelevelNewModel>();
            }

            var param = new CustomerReq() { TabType = 3 };
 
                                                                                                                                                                                                                                                                       //用例代码 
            var result =  db.Queryable<CustomerModel>().Where(customer => customer.CustomerLevelId == SqlFunc.Subqueryable<CustomertypelevelNewModel>().Where(n => SqlFunc.Between(n.CustomerTypeNewId, 1, 2)).GroupBy(n => n.CustomerLevelId).Select(n => n.CustomerLevelId)).ToList();
            var result2 = db.Queryable<CustomerModel>()
                            .Where(customer => customer.CustomerLevelId ==
                            SqlFunc.Subqueryable<CustomertypelevelNewModel>()
                            .Where(n => n.RoamCusTypeNewId == 1)
                            .GroupBy(n => n.CustomerLevelId).Select(n => n.CustomerLevelId)).ToList();
        }

        public class CustomerReq
{
    public int? TabType { get; set; }
}

        /// <summary>
        /// 客户信息
        ///</summary>
        [SugarTable("js_crm_customer")]
        public class CustomerModel
        {
            /// <summary>
            /// 系统编号 
            ///</summary>
            [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
            public string Id { get; set; }
            /// <summary>
            /// 客户编号 
            ///</summary>
            [SugarColumn(ColumnName = "Number")]
            public string Number { get; set; }
            /// <summary>
            /// 密码 
            ///</summary>
            [SugarColumn(ColumnName = "Password")]
            public string Password { get; set; }
            /// <summary>
            /// 加密方式 
            ///</summary>
            [SugarColumn(ColumnName = "PasswordFormat")]
            public int? PasswordFormat { get; set; }
            /// <summary>
            /// 加密密匙 
            ///</summary>
            [SugarColumn(ColumnName = "PasswordSalt")]
            public string PasswordSalt { get; set; }
            /// <summary>
            /// 客户姓名 
            ///</summary>
            [SugarColumn(ColumnName = "Name")]
            public string Name { get; set; }
            /// <summary>
            /// 性别(0=男；1=女;2=保密;) 
            ///</summary>
            [SugarColumn(ColumnName = "Gender")]
            public int? Gender { get; set; }
            /// <summary>
            /// 区县编号 
            ///</summary>
            [SugarColumn(ColumnName = "RegionId")]
            public string RegionId { get; set; }
            /// <summary>
            /// 详细地址 
            ///</summary>
            [SugarColumn(ColumnName = "Address")]
            public string Address { get; set; }
            /// <summary>
            /// 联系方式1 
            ///</summary>
            [SugarColumn(ColumnName = "CellPhone")]
            public string CellPhone { get; set; }
            /// <summary>
            /// 联系方式2 
            ///</summary>
            [SugarColumn(ColumnName = "CellPhone2")]
            public string CellPhone2 { get; set; }
            /// <summary>
            /// 联系方式3 
            ///</summary>
            [SugarColumn(ColumnName = "CellPhone3")]
            public string CellPhone3 { get; set; }
            /// <summary>
            /// 创建日期 
            ///</summary>
            [SugarColumn(ColumnName = "CreateDate")]
            public DateTime? CreateDate { get; set; }
            /// <summary>
            /// 创建人 
            ///</summary>
            [SugarColumn(ColumnName = "CeateUserId")]
            public string CeateUserId { get; set; }
            /// <summary>
            /// 消费金额 
            ///</summary>
            [SugarColumn(ColumnName = "CostTotal")]
            public decimal? CostTotal { get; set; }
            /// <summary>
            /// 意向产品(多产品ID用逗号隔开) 
            ///</summary>
            [SugarColumn(ColumnName = "LikeProduct")]
            public string LikeProduct { get; set; }
            /// <summary>
            /// 归属人 
            ///</summary>
            [SugarColumn(ColumnName = "ManageUserId")]
            public string ManageUserId { get; set; }
            /// <summary>
            /// 跟进次数 
            ///</summary>
            [SugarColumn(ColumnName = "FollowupTimes")]
            public int? FollowupTimes { get; set; }
            /// <summary>
            /// 成交次数 
            ///</summary>
            [SugarColumn(ColumnName = "Volamount")]
            public int? Volamount { get; set; }
            /// <summary>
            /// 客户等级 
            ///</summary>
            [SugarColumn(ColumnName = "Grade")]
            public int? Grade { get; set; }
            /// <summary>
            /// 上级编号 
            ///</summary>
            [SugarColumn(ColumnName = "CustomerParentId")]
            public string CustomerParentId { get; set; }
            /// <summary>
            /// 客户状态 
            ///</summary>
            [SugarColumn(ColumnName = "CustomerStatus")]
            public int? CustomerStatus { get; set; }
            /// <summary>
            /// 客户来源编号 
            ///</summary>
            [SugarColumn(ColumnName = "CustomerSourceId")]
            public string CustomerSourceId { get; set; }
            /// <summary>
            /// 备注 
            ///</summary>
            [SugarColumn(ColumnName = "Remark")]
            public string Remark { get; set; }
            /// <summary>
            /// 是否可用 
            ///</summary>
            [SugarColumn(ColumnName = "IsEnabled")]
            public int? IsEnabled { get; set; }
            /// <summary>
            ///  
            ///</summary>
            [SugarColumn(ColumnName = "WxId")]
            public string WxId { get; set; }
            /// <summary>
            ///  
            ///</summary>
            [SugarColumn(ColumnName = "HurTime")]
            public DateTime? HurTime { get; set; }
            /// <summary>
            ///  
            ///</summary>
            [SugarColumn(ColumnName = "RegionPaht")]
            public string RegionPaht { get; set; }
            /// <summary>
            /// 分配时间 
            ///</summary>
            [SugarColumn(ColumnName = "Ftime")]
            public DateTime? Ftime { get; set; }
            /// <summary>
            /// 预约下次沟通时间 
            ///</summary>
            [SugarColumn(ColumnName = "Gtime")]
            public DateTime? Gtime { get; set; }
            /// <summary>
            ///  
            ///</summary>
            [SugarColumn(ColumnName = "LikeProduct2")]
            public string LikeProduct2 { get; set; }
            /// <summary>
            /// 最后一次访问时间 
            ///</summary>
            [SugarColumn(ColumnName = "lastvisitdate")]
            public DateTime? Lastvisitdate { get; set; }
            /// <summary>
            /// 客户等级 
            ///</summary>
            [SugarColumn(ColumnName = "CustomerLevel")]
            public string CustomerLevel { get; set; }
            /// <summary>
            /// 推广链接 
            ///</summary>
            [SugarColumn(ColumnName = "PromotionUrl")]
            public string PromotionUrl { get; set; }
            /// <summary>
            /// 年龄 
            ///</summary>
            [SugarColumn(ColumnName = "Age")]
            public int? Age { get; set; }
            /// <summary>
            /// 年龄段 
            ///</summary>
            [SugarColumn(ColumnName = "AgeSection")]
            public int? AgeSection { get; set; }
            /// <summary>
            /// 体重 
            ///</summary>
            [SugarColumn(ColumnName = "Weight")]
            public decimal? Weight { get; set; }
            /// <summary>
            /// 身高 
            ///</summary>
            [SugarColumn(ColumnName = "Height")]
            public decimal? Height { get; set; }
            /// <summary>
            /// 号码归属地 
            ///</summary>
            [SugarColumn(ColumnName = "Location")]
            public string Location { get; set; }
            /// <summary>
            /// 生日 
            ///</summary>
            [SugarColumn(ColumnName = "Birthday")]
            public DateTime? Birthday { get; set; }
            /// <summary>
            /// 其他疾病 
            ///</summary>
            [SugarColumn(ColumnName = "OtherDisease")]
            public string OtherDisease { get; set; }
            /// <summary>
            /// 过敏 如果为空或长度等于0则选中无 
            ///</summary>
            [SugarColumn(ColumnName = "Allergy")]
            public string Allergy { get; set; }
            /// <summary>
            /// 运动习惯 0 常常 1 偶尔 2 较少 
            ///</summary>
            [SugarColumn(ColumnName = "SportHabit")]
            public int? SportHabit { get; set; }
            /// <summary>
            /// 饮食习惯 
            ///</summary>
            [SugarColumn(ColumnName = "DietaryHabit")]
            public string DietaryHabit { get; set; }
            /// <summary>
            /// 婚姻状况 0 未婚 1 已婚 
            ///</summary>
            [SugarColumn(ColumnName = "MaritalStatus")]
            public int? MaritalStatus { get; set; }
            /// <summary>
            /// 睡眠状况 0 较好 1 一般 2 不好 
            ///</summary>
            [SugarColumn(ColumnName = "SleepStatus")]
            public int? SleepStatus { get; set; }
            /// <summary>
            /// 需求，以 | 分割 
            ///</summary>
            [SugarColumn(ColumnName = "Need")]
            public string Need { get; set; }
            /// <summary>
            /// 订单数量 
            ///</summary>
            [SugarColumn(ColumnName = "ordercount")]
            public int? Ordercount { get; set; }
            /// <summary>
            /// 最后沟通时间 
            ///</summary>
            [SugarColumn(ColumnName = "communicationdate")]
            public DateTime? Communicationdate { get; set; }
            /// <summary>
            /// 已减体重 
            ///</summary>
            [SugarColumn(ColumnName = "ReduceWeight")]
            public decimal? ReduceWeight { get; set; }
            /// <summary>
            /// 最后下单时间 
            ///</summary>
            [SugarColumn(ColumnName = "lastorderdate")]
            public DateTime? Lastorderdate { get; set; }
            /// <summary>
            /// 脂玫乐编号 
            ///</summary>
            [SugarColumn(ColumnName = "ZmlId")]
            public string ZmlId { get; set; }
            /// <summary>
            /// 脂玫乐绑定码 
            ///</summary>
            [SugarColumn(ColumnName = "ZmlInviteCode")]
            public string ZmlInviteCode { get; set; }
            /// <summary>
            /// 广告客户来源计划编号 
            ///</summary>
            [SugarColumn(ColumnName = "PlanId")]
            public string PlanId { get; set; }
            /// <summary>
            /// 互联网医院ID 
            ///</summary>
            [SugarColumn(ColumnName = "HospitalId")]
            public string HospitalId { get; set; }
            /// <summary>
            /// 广告客户来源账户编号 
            ///</summary>
            [SugarColumn(ColumnName = "AccountId")]
            public long? AccountId { get; set; }
            /// <summary>
            /// 是否通过电话取得联系(通话记录中有呼入或呼出记录) 
            ///</summary>
            [SugarColumn(ColumnName = "IsContact")]
            public int? IsContact { get; set; }
            /// <summary>
            /// 真实创建时间（入库时间） 
            ///</summary>
            [SugarColumn(ColumnName = "RealCreateDate")]
            public DateTime? RealCreateDate { get; set; }
            /// <summary>
            /// 是否有投诉记录 
            ///</summary>
            [SugarColumn(ColumnName = "IsComplaint")]
            public int? IsComplaint { get; set; }
            /// <summary>
            /// 0:未锁定 1：已锁定  
            ///</summary>
            [SugarColumn(ColumnName = "IsLock")]
            public int? IsLock { get; set; }
            /// <summary>
            /// 部门分类编号 
            ///</summary>
            [SugarColumn(ColumnName = "DeptClassifyId")]
            public int? DeptClassifyId { get; set; }
            /// <summary>
            /// 复购进线时间 
            ///</summary>
            [SugarColumn(ColumnName = "RePurchaseLeadTime")]
            public DateTime? RePurchaseLeadTime { get; set; }
            /// <summary>
            /// 蜕变日记开启服务 
            ///</summary>
            [SugarColumn(ColumnName = "IsOpenDiary")]
            public int? IsOpenDiary { get; set; }
            /// <summary>
            /// 蜕变日记用户开启 
            ///</summary>
            [SugarColumn(ColumnName = "IsUserOpenDiary")]
            public int? IsUserOpenDiary { get; set; }
            /// <summary>
            /// 客户标记 
            ///</summary>
            [SugarColumn(ColumnName = "CustomerFlag")]
            public string CustomerFlag { get; set; }
            /// <summary>
            /// 最后编辑时间 
            ///</summary>
            [SugarColumn(ColumnName = "LastModifyTime")]
            public DateTime? LastModifyTime { get; set; }
            /// <summary>
            /// 进线类型  0 员工创建 1 未接来电  2 其他系统导入  3 excel导入  4 飞鱼广告进线 5 快手广告进线 
            ///</summary>
            [SugarColumn(ColumnName = "InlineType")]
            public int? InlineType { get; set; }
            /// <summary>
            /// 微信号 
            ///</summary>
            [SugarColumn(ColumnName = "wechatnumber")]
            public string Wechatnumber { get; set; }
            /// <summary>
            /// 客户阶段 
            ///</summary>
            [SugarColumn(ColumnName = "CustomerLevelId")]
            public int? CustomerLevelId { get; set; }



        }

        /// <summary>
        /// 
        /// </summary>
        [SugarTable("js_crm_customertypelevelnew")]
        public class CustomertypelevelNewModel
        {
            /// <summary>
            /// 客户分类等级Id 
            ///</summary>
            [SugarColumn(ColumnName = "CustomerLevelId", IsPrimaryKey = true, IsIdentity = true)]
            public int CustomerLevelId { get; set; }
            /// <summary>
            /// 客户分类等级名称 
            ///</summary>
            [SugarColumn(ColumnName = "CustomerLevelName")]
            public string CustomerLevelName { get; set; }
            /// <summary>
            /// 新分类Id 
            ///</summary>
            [SugarColumn(ColumnName = "CustomerTypeNewId")]
            public int? CustomerTypeNewId { get; set; }
            /// <summary>
            /// 排序 
            ///</summary>
            [SugarColumn(ColumnName = "LevelSort")]
            public int? LevelSort { get; set; }
            /// <summary>
            /// 自动流转勾选 1 是 0否 
            ///</summary>
            [SugarColumn(ColumnName = "RoamEnable")]
            public bool? RoamEnable { get; set; }
            /// <summary>
            /// 自动流转到客户分类Id 
            ///</summary>
            [SugarColumn(ColumnName = "RoamCusTypeNewId")]
            public int? RoamCusTypeNewId { get; set; }
            /// <summary>
            /// 自动流转到客户分类明细Id 
            ///</summary>
            [SugarColumn(ColumnName = "RoamCusTypeLevelId")]
            public int? RoamCusTypeLevelId { get; set; }
            /// <summary>
            /// 限制条件 1.不限制 2.需要审核 3.订单签收 
            ///</summary>
            [SugarColumn(ColumnName = "Restrictions")]
            public int? Restrictions { get; set; }
            /// <summary>
            /// 是否删除 0否 1是 
            ///</summary>
            [SugarColumn(ColumnName = "IsDelete")]
            public bool? IsDelete { get; set; }
            /// <summary>
            /// 对应crm等级名称 
            ///</summary>
            [SugarColumn(ColumnName = "CrmCustomerTypeName")]
            public string CrmCustomerTypeName { get; set; }

            /// <summary>
            /// 创建日期 
            ///</summary>
            [SugarColumn(ColumnName = "CreateData")]
            public DateTime? CreateData { get; set; }
            /// <summary>
            /// 创建人员工Id 
            ///</summary>
            [SugarColumn(ColumnName = "CreateUserId")]
            public string CreateUserId { get; set; }
        }
    }
}
