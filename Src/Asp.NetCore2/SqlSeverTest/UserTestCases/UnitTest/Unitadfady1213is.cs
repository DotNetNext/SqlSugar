using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net.Mime;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SqlSugar;

namespace OrmTest;

public static class Unitdafaaaaa
{
    public static void Init() 
    {
        Main22().GetAwaiter().GetResult();
    }
    public static async Task Main22()
    {
        await Task.CompletedTask;
        var db = new SqlSugarScope(new SqlSugar.ConnectionConfig()
        {
            ConnectionString = $@"DataSource={Environment.CurrentDirectory}\test22.db",
            DbType = DbType.Sqlite,
            IsAutoCloseConnection = true,
        });

        db.Aop.OnLogExecuting = (s, parameters) =>
            Console.WriteLine(UtilMethods.GetSqlString(DbType.Sqlite, s, parameters));

        db.DbMaintenance.CreateDatabase();

        //建表 
        db.CodeFirst.InitTables<Orders>();
        db.CodeFirst.InitTables<MaterialStocks>();
        db.CodeFirst.InitTables<ItemInfoProperty>();
        db.CodeFirst.InitTables<ItemInfos>();

        db.DbMaintenance.TruncateTable<Orders>();
        db.DbMaintenance.TruncateTable<MaterialStocks>();
        db.DbMaintenance.TruncateTable<ItemInfoProperty>();
        db.DbMaintenance.TruncateTable<ItemInfos>();

        var item = new ItemInfos { Id = 1 };
        var property = new ItemInfoProperty { StockId = 100, ItemId = 1 ,Color="a"};
        var stock = new MaterialStocks { Id = 100, ItemId = 1, StockId = 100 };
        var order = new Orders { Id = 1, StockId = 100 };

        await db.Insertable(item).ExecuteCommandAsync();
        await db.Insertable(property).ExecuteCommandAsync();
        await db.Insertable(stock).ExecuteCommandAsync();
        await db.Insertable(order).ExecuteCommandAsync();

        var result = await db.Queryable<Orders>()
           .LeftJoin<ItemInfoProperty>((s, ip) => s.StockId == ip.StockId)
           .LeftJoin<ItemInfos>((s, ip, i) => ip.ItemId == i.Id)
           .Select((s, ip, i) => new View()
           {
               ItemInfoProperty = ip,
           }, true)
           .ToListAsync();
        var result2 = await db.Queryable<Orders>()
              .LeftJoin<ItemInfoProperty>((s, ip) => s.StockId == ip.StockId) 
              .Select((s, ip) => new View()
              {
                  ItemInfoProperty = ip,
              }, true)
            .ToListAsync();
    }

    public class MaterialStocks
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = false, IsNullable = false)]
        public long Id { get; set; }

        /// <summary>
        /// 仓库
        /// </summary>
        [SugarColumn(DefaultValue = "0")]
        public long WarehouseId { get; set; }

        /// <summary>
        /// 库存Id
        /// </summary>
        [SugarColumn(DefaultValue = "0")]
        public long StockId { get; set; }

        /// <summary>
        /// 物料属性
        /// </summary>
        [Navigate(NavigateType.OneToOne, nameof(StockId))]
        public ItemInfoProperty ItemProperty { get; set; }

        [SugarColumn(DefaultValue = "0")]
        public long ProductId { get; set; }

        [SugarColumn(DefaultValue = "0")]
        public long ItemId { get; set; }

        [Navigate(NavigateType.OneToOne, nameof(ItemId))]
        public ItemInfos ItemInfo { get; set; }

        /// <summary>
        /// 账面库存
        /// </summary>
        [SugarColumn(Length = 18, DecimalDigits = 4, DefaultValue = "0")]
        public decimal StockQty { get; set; }

        /// <summary>
        /// 可用库存
        /// </summary>
        [SugarColumn(Length = 18, DecimalDigits = 4, DefaultValue = "0")]
        public decimal AliveQty { get; set; }

        /// <summary>
        /// 锁定库存
        /// </summary>
        [SugarColumn(Length = 18, DecimalDigits = 4, DefaultValue = "0")]
        public decimal LockQty { get; set; }
    }


    /// <summary>
    /// 物料属性
    /// </summary>
    public class ItemInfoProperty
    {
        /// <summary>
        /// 库存Id
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = false, IsNullable = false)]
        public long StockId { get; set; }

        /// <summary>
        /// 物料Id
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public long ItemId { get; set; }

        [Navigate(NavigateType.OneToOne, nameof(ItemId))]
        public ItemInfos ItemInfo { get; set; }

        /// <summary>
        /// 助记码
        /// </summary>
        [SugarColumn(Length = 50, IsNullable = true)]
        public string Code { get; set; }

        /// <summary>
        /// 颜色
        /// </summary>
        [SugarColumn(Length = 50, IsNullable = false)]
        public string Color { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [SugarColumn(DefaultValue = "1")]
        public bool Enabled { get; set; } = true;

        [JsonIgnore]
        [Navigate(NavigateType.OneToMany, nameof(MaterialStocks.StockId), nameof(StockId))]
        public List<MaterialStocks> Stock { get; set; }
    }

    /// <summary>
    /// 物料档案
    /// </summary>
    public class ItemInfos
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = false, IsNullable = false)]
        public long Id { get; set; }

        /// <summary>
        /// 物料分类
        /// </summary>
        [SugarColumn(DefaultValue = "0")]
        public long CategoryId { get; set; }

        /// <summary>
        /// 默认仓库
        /// </summary>
        [SugarColumn(DefaultValue = "0")]
        public long WarehouseId { get; set; }

        /// <summary>
        /// 模具编码
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string MouldCode { get; set; }

        /// <summary>
        /// 物料编码
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string MaterialCode { get; set; }

        /// <summary>
        /// 物料名称
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string MaterialName { get; set; }

        /// <summary>
        /// 零件单位
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string MaterialUnit { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        [SugarColumn(IsNullable = true, DefaultValue = "0", Length = 18, DecimalDigits = 6)]
        public decimal MaterialPrice { get; set; }

        /// <summary>
        /// 单重（g）
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string MaterialWeight { get; set; }

        /// <summary>
        /// 物料描述
        /// </summary>
        [SugarColumn(ColumnDataType = "NVARCHAR(255)", IsNullable = true)]
        public string MaterialSpec { get; set; }

        /// <summary>
        /// 出模数量
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string MoldRate { get; set; }

        /// <summary>
        /// 图片（支持多张图片，分号隔开）
        /// </summary>
        [SugarColumn(Length = 2000, IsNullable = true)]
        public string ProductImages { get; set; }

        /// <summary>
        /// 默认供应商
        /// </summary>
        [SugarColumn(DefaultValue = "0")]
        public long SupplierId { get; set; }

        /// <summary>
        /// 是否通用件
        /// </summary>
        [SugarColumn(DefaultValue = "0")]
        public bool IsCommon { get; set; }

        /// <summary>
        /// 是否纸箱
        /// </summary>
        [SugarColumn(DefaultValue = "0")]
        public bool IsPaperBox { get; set; }

        /// <summary>
        /// 物料属性
        /// </summary>
        [Navigate(NavigateType.OneToMany, nameof(ItemInfoProperty.ItemId))]
        public List<ItemInfoProperty> Properties { get; set; }

        [Navigate(NavigateType.OneToOne, nameof(Id), nameof(MaterialStocks.ItemId))]
        public MaterialStocks Stock { get; set; }
    }
    [SugarTable("unitOrdersadsfaf")]
    public class Orders
    {
        [SugarColumn(IsPrimaryKey = true)]
        public long Id { get; set; }

        public long ItemId { get; set; }

        public long StockId { get; set; }

        [Navigate(NavigateType.OneToOne, nameof(StockId))]
        public ItemInfoProperty ItemProperty { get; set; }
    }

    public class View
    {
        public ItemInfoProperty ItemInfoProperty { get; set; }
    }
}