using System;
using System.Collections.Generic;
using SqlSugar;

namespace ERP1.Entity
{
    [Serializable]
    [SugarTable("t_OrderDeliveryLogHis")]
    public class OrderDeliveryLogHisEntity
    {
        #region # 数据库字段 #

        /// <summary>Id</summary>
        public string Id { set; get; }

        /// <summary>发货单号</summary>
        public string DeliveryNo { set; get; }

        /// <summary>物流单号</summary>
        public string LogisticeNo { set; get; }

        /// <summary>创建人Id</summary>
        public string? AdderId { set; get; }

        /// <summary>创建时间</summary>
        public DateTime AddTime { set; get; }

        #endregion
    }
}
