
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Linq;

namespace OrmTest
{
    public class UInsertNav0adsf
    {

        public  static void  Init()
        {
           
            var context = NewUnitTest.Db;
            context.Aop.DataExecuting = (oldValue, entityInfo) =>
            {
                if (entityInfo.PropertyName == "Iden" && entityInfo.OperationType == DataFilterType.InsertByObject)
                {
                    entityInfo.SetValue(Guid.NewGuid().ToString("N").ToUpper());
                }
            };
        
            //建表 
            if (!context.DbMaintenance.IsAnyTable("DELIVERY_INFO_1", false))
            {
                context.CodeFirst.InitTables<Delivery>();
            }
            //建表 
            if (!context.DbMaintenance.IsAnyTable("DELIVERY_DETAIL_1", false))
            {
                context.CodeFirst.InitTables<DeliveryDetail>();
            }
            //建表 
            if (!context.DbMaintenance.IsAnyTable("DELIVERY_DETAIL_ITEM_1", false))
            {
                context.CodeFirst.InitTables<DeliveryDetailItem>();
            }
            context.DbMaintenance.TruncateTable<Delivery, DeliveryDetail, DeliveryDetailItem>();

            Delivery delivery = new Delivery
            {
                DeliveryNo = "1",
                DeliveryDetails = new List<DeliveryDetail> {
                    new DeliveryDetail() { DeliveryDetailItems= new List<DeliveryDetailItem>{ new DeliveryDetailItem {FieldId="00",FieldValue="00" } } }
                }
            };
              context.InsertNav(delivery).Include(s => s.DeliveryDetails).ThenInclude(s => s.DeliveryDetailItems).ExecuteCommand();
            var result = context.Queryable<Delivery>().Where(s => s.DeliveryNo == "1").Includes(s => s.DeliveryDetails, dd => dd.DeliveryDetailItems).First();
            context.UpdateNav(result).Include(s => s.DeliveryDetails).ThenInclude(s => s.DeliveryDetailItems).ExecuteCommand();
            var result2 = context.Queryable<Delivery>().Where(s => s.DeliveryNo == "1").Includes(s => s.DeliveryDetails, dd => dd.DeliveryDetailItems).First();
            if (!result.DeliveryDetails.Any(z => z.DeliveryDetailItems.Any())) 
            {
                throw new Exception("unit error");
            }
            if (!result2.DeliveryDetails.Any(z => z.DeliveryDetailItems.Any()))
            {
                throw new Exception("unit error");
            }
        }


        ///<summary>
        ///
        ///</summary>
        [SugarTable("DELIVERY_INFO_1")]
        public partial class Delivery : Base
        {
            /// <summary>
            /// 送货单号
            /// </summary>
            [SugarColumn(ColumnName = "DELIVERY_NO")]
            public string DeliveryNo { get; set; }

            [SugarColumn(IsIgnore = true)]
            [Navigate(NavigateType.OneToMany, nameof(DeliveryDetail.DeliveryNo), nameof(DeliveryNo))]
            public List<DeliveryDetail> DeliveryDetails { get; set; }

        }


        ///<summary>
        ///
        ///</summary>
        [SugarTable("DELIVERY_DETAIL_1")]
        public partial class DeliveryDetail : Base
        {
            public DeliveryDetail()
            {
            }

            /// <summary>
            /// 送货单号
            /// </summary>           
            [SugarColumn(ColumnName = "DELIVERY_NO")]
            public string DeliveryNo { get; set; }


            [SugarColumn(IsIgnore = true)]
            [Navigate(NavigateType.OneToMany, nameof(DeliveryDetailItem.DeliveryDetailIden))]
            public List<DeliveryDetailItem> DeliveryDetailItems { get; set; }

        }
        [SugarTable("DELIVERY_DETAIL_ITEM_1")]
        public partial class DeliveryDetailItem : Base
        {
            /// <summary>
            /// 送货单明细IDEN
            /// </summary>
            [SugarColumn(ColumnName = "DELIVERY_DETAIL_IDEN")]
            public string DeliveryDetailIden { get; set; }

            /// <summary>
            /// 字段编号
            /// </summary>
            [SugarColumn(ColumnName = "FIELD_ID")]
            public string FieldId { get; set; }

            /// <summary>
            /// 字段值
            /// </summary>
            [SugarColumn(ColumnName = "FIELD_VALUE")]
            public string FieldValue { get; set; }
        }

        public class Base
        {
            /// <summary>
            /// 主键
            /// </summary>
            [SugarColumn(IsPrimaryKey = true, ColumnName = "IDEN", ColumnDescription = "主键", ColumnDataType = "VARCHAR(50)")]
            public string Iden { get; set; }
        }
    }
}
