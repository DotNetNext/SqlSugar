using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;

namespace OrmTest
{
    public class UNavTest
    {

        public static void Init()
        {
            var context = NewUnitTest.Db;
            //  context.QueryFilter.Add(new TableFilterItem<DeliveryDetail>(z=>z.IsDelete==1 , true));
            context.QueryFilter.Add(new TableFilterItem<DeliveryDetailItem>(z => z.IsDelete == 11, true));

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
            context.Aop.DataExecuting = (oldValue, entityInfo) =>
            {
                if (entityInfo.PropertyName == "Iden" && entityInfo.OperationType == DataFilterType.InsertByObject)
                {
                    entityInfo.SetValue(Guid.NewGuid().ToString("N").ToUpper());
                }
                if (entityInfo.PropertyName == "IsDelete" && entityInfo.OperationType == DataFilterType.InsertByObject)
                {
                    entityInfo.SetValue(0);
                }
            };
            DeliveryDetail deliveryDetail = new DeliveryDetail
            {
                DeliveryNo = "11",

                deliveryDetailItem = new DeliveryDetailItem { FieldId = "00", FieldValue = "00" }
            };

            context.InsertNav(deliveryDetail).Include(s => s.deliveryDetailItem).ExecuteCommand();
            var items = context.Queryable<DeliveryDetailItem>().ToList();
            items.ForEach(s => s.IsDelete = 1);
            context.Updateable(items).ExecuteCommand();
            var result = context.Queryable<DeliveryDetail>().Where(s => s.DeliveryNo == "11").Includes(dd => dd.deliveryDetailItem).First();
            if (result.deliveryDetailItem != null)
            {
                throw new Exception("unit error");
            }

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
            [Navigate(NavigateType.OneToOne, nameof(Iden), nameof(DeliveryDetailItem.DeliveryDetailIden))]
            public DeliveryDetailItem deliveryDetailItem { get; set; }

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
            /// <summary>
            /// 是否被删除
            /// </summary>
            [SugarColumn(ColumnName = "IS_DELETE", ColumnDescription = "是否被删除")]
            public int IsDelete { get; set; }
        }
    }
}


