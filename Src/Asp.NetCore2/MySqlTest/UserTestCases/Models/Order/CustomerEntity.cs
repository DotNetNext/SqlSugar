using System;
using System.Collections.Generic;
using ERP1.Entity;
using SqlSugar;

namespace ERP1.Entity
{
    [Serializable]
    [SugarTable("t_Customer")]
    public class CustomerEntity: TeamBasePart
    {
        #region # 数据库字段 #
        /// <summary>客户Id</summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = false)]
        public string CustomerId { set; get; }

        /// <summary>姓名</summary>
        public string Name { set; get; }

        /// <summary>联系电话</summary>
        public string Telephone { set; get; }

        /// <summary>电子邮箱</summary>
        public string Email { set; get; }

        /// <summary>联系方式</summary>
        public string? Concat { set; get; }

        /// <summary>国家/地区</summary>
        public string Country { set; get; }

        /// <summary>客户等级</summary>
        public string Level { get; set; }

        /// <summary>备注</summary>
        public string? Remark { set; get; }

        /// <summary>交易笔数</summary>
        public Int32 TransNum { set; get; }

        /// <summary>交易金额</summary>
        public Decimal TransAmt { set; get; }

        /// <summary>最后交易时间</summary>
        public DateTime? LastTransTime { set; get; }

        /// <summary>创建时间</summary>
        public DateTime AddTime { set; get; }
        #endregion

        /// <summary>国家/地区(中文名称)</summary>
        [SugarColumn(IsIgnore = true)]
        public string Country_NameCn { set; get; }

        /// <summary>等级名称</summary>
        [SugarColumn(IsIgnore = true)]
        public string LevelName { set; get; }

        /// <summary>是否黑名单</summary>
        [SugarColumn(IsIgnore = true)]
        public bool IsBlack { set; get; }

        /// <summary>黑名单类型</summary>
        [SugarColumn(IsIgnore = true)]
        public List<string> BlackTypes { set; get; }

        /// <summary>收货地址</summary>
        [SugarColumn(IsIgnore = true)]
        [Navigate(NavigateType.OneToMany, nameof(AddressEntity.RefId))]
        public List<AddressEntity> CustomerAddressEntitys { get; set; }
    }
}
