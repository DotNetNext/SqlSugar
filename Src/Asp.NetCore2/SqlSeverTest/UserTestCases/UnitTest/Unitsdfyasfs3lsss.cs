using SqlSugar;
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
            db.CodeFirst.InitTables<UserInfo001>(); 
            var x = new List<UserInfo001>() { new UserInfo001() }; 
            var userInfo = db.Queryable<UserInfo001>()
                .Where(t => x.Any(s => s.UserName == t.UserName && s.Context == t.Context))
                .ToList();

            var userInfo2 = db.Queryable<UserInfo001>()
             .Where(t => x.Any(s =>  t.UserName ==s.UserName && t.Context == s.Context))
             .ToList();
            var x2 = new List<UserInfo001>() { new UserInfo001() {  UserName="a"} };
            var userInfo3 = db.Queryable<UserInfo001>()
                .Where(t => x2.Any(s => s.UserName == t.UserName && s.Context == t.Context))
                .ToList();
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

        [SugarTable("Unitadfaysd22")]
        public class UserInfo001
        {
            /// <summary>
            /// User ID (Primary Key)
            /// 用户ID（主键）
            /// </summary>
            [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
            public int UserId { get; set; }

            /// <summary>
            /// User name
            /// 用户名
            /// </summary>
            [SugarColumn(Length = 50, IsNullable = false)]
            public string UserName { get; set; }

            /// <summary>
            /// User email
            /// 用户邮箱
            /// </summary>
            [SugarColumn(IsNullable = true)]
            public string Email { get; set; }


            /// <summary>
            /// Product price
            /// 产品价格
            /// </summary> 
            public decimal Price { get; set; }

            /// <summary>
            /// User context
            /// 用户内容
            /// </summary>
            [SugarColumn(ColumnDataType = StaticConfig.CodeFirst_BigString, IsNullable = true)]
            public string Context { get; set; }

            /// <summary>
            /// User registration date
            /// 用户注册日期
            /// </summary>
            [SugarColumn(IsNullable = true)]
            public DateTime? RegistrationDate { get; set; }
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
