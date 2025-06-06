using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public class Unitdfdaysss
    {

        public static void Init() 
        {
            var db = NewUnitTest.Db;

            var sql1 =
                        db.Queryable<OrderJoinItemTemp>()
                        .Select(p => new StoreBrandOther
                        {
                            BrandName = string.IsNullOrEmpty(
                              SqlFunc.Subqueryable<Brand>().Where(n => n.Id == p.BrandId).Select(n => n.Name)
                          ) ? "n" : "a"
                        }).ToSqlString();

            if (!sql1.Contains(" [OrderJoinItemTemp] [p]")) 
            {
                throw new Exception("unit test");
            }

                 var sql2 =  
                         db.Queryable<OrderJoinItemTemp>()
                         .Select(p => new StoreBrandOther
                         {
                             BrandName = string.IsNullOrEmpty(
                                 SqlFunc.Subqueryable<Brand>().Where(n => n.Id == p.BrandId).Select(n => n.Name)
                             )
                                 ? "aa"
                             : SqlFunc
                                     .Subqueryable<Brand>()
                                     .Where(n => n.Id == p.BrandId)
                                     .Select(n => n.Name),
                             CategoryName = string.IsNullOrEmpty(
                                 SqlFunc
                                     .Subqueryable<Category>()
                                     .Where(n => n.Id == p.ThreeLevelCategory)
                                     .Select(n => n.Name)
                             )
                                 ? "为空三级分类"
                                 : SqlFunc
                                     .Subqueryable<Category>()
                                     .Where(n => n.Id == p.ThreeLevelCategory)
                                     .Select(n => n.Name),
                             StoreId = p.StoreId,
                             OrderItemId = SqlFunc.ToInt64(p.FoodId),
                             Total = SqlFunc.ToDecimal(p.ItemPrice * p.ItemCount),
                             CityId = p.CityId,
                         })
                         .ToSqlString();

            if (!sql2.Contains(" [OrderJoinItemTemp] [p]"))
            {
                throw new Exception("unit test");
            }
        }
    }
    ///<summary>
    ///
    ///</summary>
    public partial class Category
    {
        public Category()
        {


        }
        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>           
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>           
        public string Name { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>           
        public int Level { get; set; }

        /// <summary>
        /// Desc:父级Id，指向本表
        /// Default:
        /// Nullable:True
        /// </summary>           
        public int? ParentId { get; set; }

        /// <summary>
        /// Desc:天猫热卖指数
        /// Default:
        /// Nullable:True
        /// </summary>           
        public int? TmallIndex { get; set; }

        /// <summary>
        /// Desc:美团热卖指数
        /// Default:
        /// Nullable:True
        /// </summary>           
        public int? MtIndex { get; set; }

        /// <summary>
        /// Desc:饿了么热卖指数
        /// Default:
        /// Nullable:True
        /// </summary>           
        public int? EleIndex { get; set; }

        /// <summary>
        /// Desc:京东热卖指数
        /// Default:
        /// Nullable:True
        /// </summary>           
        public int? JdIndex { get; set; }

        /// <summary>
        /// Desc:
        /// Default:DateTime.Now
        /// Nullable:False
        /// </summary>           
        public DateTime Created { get; set; }

        /// <summary>
        /// Desc:
        /// Default:DateTime.Now
        /// Nullable:False
        /// </summary>           
        public DateTime Updated { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public int? CreatorId { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public int? UpdaterId { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public bool? IsDelete { get; set; }

    }
    ///<summary>
    ///
    ///</summary>
    public partial class Brand
    {
        public Brand()
        {


        }
        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>           
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>           
        public string Letter { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>           
        public string Name { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string? Desc { get; set; }

        /// <summary>
        /// Desc:
        /// Default:0
        /// Nullable:False
        /// </summary>           
        public int Order { get; set; }

        /// <summary>
        /// Desc:
        /// Default:1
        /// Nullable:False
        /// </summary>           
        public bool Published { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string? Website { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string? Origin { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string? Company { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>           
        public byte[] Version { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string? Letter2 { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string? Letter3 { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public long? MTId { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public long? EleId { get; set; }

        /// <summary>
        /// Desc:废弃
        /// Default:
        /// Nullable:True
        /// </summary>           
        public int? TmallHotSellIndex { get; set; }

        /// <summary>
        /// Desc:废弃
        /// Default:
        /// Nullable:True
        /// </summary>           
        public int? MtHotSellIndex { get; set; }

        /// <summary>
        /// Desc:废弃
        /// Default:
        /// Nullable:True
        /// </summary>           
        public int? EleHotSellIndex { get; set; }

        /// <summary>
        /// Desc:废弃
        /// Default:
        /// Nullable:True
        /// </summary>           
        public int? JDHotSellIndex { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public int? UpdateUserId { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public bool? IsDelete { get; set; }

        /// <summary>
        /// Desc:logo图片地址
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string? LogoUrl { get; set; }

        /// <summary>
        /// Desc:是否重点品牌
        /// Default:0
        /// Nullable:True
        /// </summary>           
        public bool? IsTop { get; set; }

        /// <summary>
        /// Desc:是否自有品牌
        /// Default:0
        /// Nullable:True
        /// </summary>           
        public bool? IsPrivate { get; set; }

    }
    public class StoreBrandOther
    {
        /// <summary>
        /// 店铺ID
        /// </summary>
        public int StoreId { get; set; }

        /// <summary>
        ///品牌名称
        /// </summary>
        public  string BrandName { get; set; }

        /// <summary>
        ///三级分类名称
        /// </summary>
        public  string CategoryName { get; set; }

        /// <summary>
        ///订单明细ID
        /// </summary>
        public  long OrderItemId { get; set; }

        /// <summary>
        ///单价*数量
        /// </summary>
        public  decimal Total { get; set; }
        /// <summary>
        /// 城市ID
        /// </summary>
        public  long? CityId { get; set; }
    }
    ///<summary>
    ///
    ///</summary>
    public partial class OrderJoinItemTemp
    {
        public OrderJoinItemTemp() { }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>
        public long? OrderId { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>
        public int StoreId { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>
        [Column(TypeName = "decimal(9,2)")]
        public decimal Totals { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>
        public int? FoodId { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>
        public string? FoodName { get; set; }
        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>
        public int? BrandId { get; set; }
        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>
        public int? ThreeLevelCategory { get; set; }
        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>
        public bool? IsFullRefund { get; set; }
        [Column(TypeName = "decimal(9,2)")]
        public decimal? ItemPrice { get; set; }
        public int? ItemCount { get; set; }
        public long? CityId { get; set; }
        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>
        public DateTime Created { get; set; }
    }
}
