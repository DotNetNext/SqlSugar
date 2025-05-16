using System;
using System.Collections.Generic;
using ERP1.Entity;
using SqlSugar;

namespace ERP1.Entity
{
    [Serializable]
    [SugarTable("t_Address")]
    public class AddressEntity
    {
        #region # 数据库字段 #

        /// <summary>地址Id</summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = false)]
        public string AddressId { set; get; }

        /// <summary>RefId(订单收货地址、订单发货单收货地址、客户收货地址)</summary>
        public string RefId { set; get; }


        /// <summary>姓名</summary>
        public string Name { set; get; }

        /// <summary>联系电话</summary>
        public string Telephone { set; get; }

        /// <summary>电子邮箱</summary>
        public string Email { set; get; }

        /// <summary>国家/地区</summary>
        public string Country { set; get; }

        /// <summary>省/州</summary>
        public string State { set; get; }

        /// <summary>城市</summary>
        public string City { set; get; }

        /// <summary>地址</summary>
        public string Address { set; get; }

        /// <summary>邮编</summary>
        public string ZipCode { set; get; }

        /// <summary>创建时间</summary>
        public DateTime AddTime { set; get; }

        #endregion

        /// <summary>国家/地区(中文名称)</summary>
        [SugarColumn(IsIgnore = true)]
        public string Country_NameCn { set; get; }

        /// <summary>省/州（中文名称）/summary>
        [SugarColumn(IsIgnore = true)]
        public string State_NameCn { set; get; }

        /// <summary>省/州（英文名称）/summary>
        [SugarColumn(IsIgnore = true)]
        public string State_NameEn { set; get; }
    }
}
