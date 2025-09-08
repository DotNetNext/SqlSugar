using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OrmTest.Unitsdfyasfs3lsss;

namespace OrmTest
{
    internal class Unitsdfyasfs3lsss
    { 
        public static void Init() 
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<OrderEntity>();

            var list=db.Queryable<OrderEntity>()
                .Select(o => new
                { 
                    Minute = o.OrderStatus == (int)OrderStatusEnum.WaitPay
                    ?
                     (o.CreateTime.AddMinutes(1) - DateTime.Now).TotalMinutes
                      :
                      0,
                    // 其他字段...
                }).ToList(); ;
        }
        [SqlSugar.SugarTable("unitsdfas0002113")]
        public class OrderEntity
        {
            public int Minute { get; set; }
            public int OrderStatus { get; set; }
            public DateTime CreateTime { get; set; }
            // 其他可能需要的属性
            public object OrderStatusObject { get; set; } // 如果OrderStatus可能是object类型
        }

        // 订单状态枚举
        public enum OrderStatusEnum
        {
            WaitPay = 1,    // 等待支付
            Paid = 2,       // 已支付
            Shipped = 3,    // 已发货
            Completed = 4,  // 已完成
            Cancelled = 5   // 已取消
        }

        // 配置选项类（用于依赖注入）
        public class CancelPayOptions
        {
            public int DelayTime { get; set; } // 延迟时间（分钟）
        }
    }
}
