
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public class UIncludesadfadfaf
    {
        public static void Init()
        {
            Console.WriteLine("");
            Console.WriteLine("#### SqlQueryable Start ####");
            var db = NewUnitTest.Db;
            //by sql
            //db.CodeFirst.InitTables<ProductEntity>();
            //db.CodeFirst.InitTables<ProductPartSNEntity>();
            var list3 = db.Queryable<ProductEntity>().Where(t => t.ProductPartSNs.Any(n => n.PartSN.SN == "WEB2022052614050")).ToList();

            //db.InitMappingInfo<ProductPartSNEntity>();
            var list4 = db.Queryable<ProductEntity>().Where(t => t.ProductPartSNs.Any(n => n.PartSNID == "WEB2022052614050")).ToList();

            Console.WriteLine("#### SqlQueryable End ####");
        }
    }
    [Tenant("MesConfig")]
    [SugarTable("TB_PRODUCT", TableDescription = "产品信息")]
    public partial class ProductEntity
    {
        /// <summary>
        /// 产品信息(构建函数)
        /// </summary>
        public ProductEntity()
        {
            this.KeyID = Guid.NewGuid().ToString();
            this.CreatorTime = DateTime.Now;
            this.CurrentIndex = 0;
            this.EnabledMark = true;
        }
        #region 实体成员
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, ColumnName = "F_Id")]
        public string KeyID { get; set; }
        /// <summary>
        /// 创建人编号
        ///</summary>
        [SugarColumn(ColumnName = "F_CREATORUSERID")]
        public string CreatorUserId { get; set; }
        /// <summary>
        /// 创建时间
        ///</summary>
        [SugarColumn(ColumnName = "F_CREATORTIME")]
        public DateTime? CreatorTime { get; set; }
        /// <summary>
        /// 修改人
        ///</summary>
        [SugarColumn(ColumnName = "F_LASTMODIFYUSERID")]
        public String LastModifyUserId { get; set; }
        /// <summary>
        /// 修改时间
        ///</summary>
        [SugarColumn(ColumnName = "F_LASTMODIFYTIME")]
        public DateTime? LastModifyTime { get; set; }
        /// <summary>
        /// 删除时间
        ///</summary>
        [SugarColumn(ColumnName = "F_DELETETIME")]
        public DateTime? DeleteTime { get; set; }
        /// <summary>
        /// 删除人员ID
        ///</summary>
        [SugarColumn(ColumnName = "F_DELETEUSERID")]
        public string DeleteUserId { get; set; }
        /// <summary>
        /// 产品关联产品信息
        ///</summary>
        [SugarColumn(ColumnName = "ProductPartSNs", IsIgnore = true)]
        [Navigate(NavigateType.OneToMany, nameof(ProductPartSNEntity.ProductID))]
        public List<ProductPartSNEntity> ProductPartSNs { get; set; }
        /// <summary>
        /// 条码显示
        ///</summary>
        [SugarColumn(ColumnName = "C_SNSHOW")]
        public string SnShow { get; set; }
        /// <summary>
        /// 条码个数
        ///</summary>
        [SugarColumn(ColumnName = "C_SNCOUNT")]
        public int? SnCount { get; set; }
        /// <summary>
        /// 产品型号ID
        ///</summary>
        [SugarColumn(ColumnName = "C_PRODUCTTYPEID")]
        public string ProductTypeID { get; set; }
        ///// <summary>
        ///// 产品类型信息
        /////</summary>
        //[SugarColumn(ColumnName = "ProductType", IsIgnore = true)]
        //[Navigate(NavigateType.OneToOne, nameof(ProductTypeID))]
        //public ProductTypeEntity ProductType { get; set; }
        /// <summary>
        /// 当前在进行第几次存储
        ///</summary>
        [SugarColumn(ColumnName = "C_CURRENTINDEX")]
        public int? CurrentIndex { get; set; }
        /// <summary>
        /// 是否可用
        ///</summary>
        [SugarColumn(ColumnName = "ENABLEDMARK")]
        public bool? EnabledMark { get; set; }
        /// <summary>
        /// 返修工单ID
        ///</summary>
        [SugarColumn(ColumnName = "C_REPAIRWORKID")]
        public string RepairWorkID { get; set; }

        #endregion

    }

    /// <summary>
    /// 产品配置条码表
    ///</summary>
    [Tenant("MesConfig")]
    [SugarTable("TC_PRODUCTPARTSN", TableDescription = "产品配置条码表")]
    public partial class ProductPartSNEntity
    {
        #region 实体成员
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, ColumnName = "F_Id")]
        public string KeyID { get; set; }
        /// <summary>
        /// 创建人编号
        ///</summary>
        [SugarColumn(ColumnName = "F_CREATORUSERID")]
        public string CreatorUserId { get; set; }
        /// <summary>
        /// 创建时间
        ///</summary>
        [SugarColumn(ColumnName = "F_CREATORTIME")]
        public DateTime? CreatorTime { get; set; }
        /// <summary>
        /// 修改人
        ///</summary>
        [SugarColumn(ColumnName = "F_LASTMODIFYUSERID")]
        public String LastModifyUserId { get; set; }
        /// <summary>
        /// 修改时间
        ///</summary>
        [SugarColumn(ColumnName = "F_LASTMODIFYTIME")]
        public DateTime? LastModifyTime { get; set; }
        /// <summary>
        /// 删除时间
        ///</summary>
        [SugarColumn(ColumnName = "F_DELETETIME")]
        public DateTime? DeleteTime { get; set; }
        /// <summary>
        /// 删除人员ID
        ///</summary>
        [SugarColumn(ColumnName = "F_DELETEUSERID")]
        public string DeleteUserId { get; set; }
        /// <summary>
        /// 产品ID
        ///</summary>
        [SugarColumn(ColumnName = "C_PRODUCTID")]
        public string ProductID { get; set; }
        /// <summary>
        /// 配件条码ID
        ///</summary>
        [SugarColumn(ColumnName = "C_PARTSNID")]
        public string PartSNID { get; set; }
        /// <summary>
        /// 配件条码ID
        ///</summary>
        [SugarColumn(ColumnName = "PARTSN", IsIgnore = true)]
        [Navigate(NavigateType.OneToOne, nameof(PartSNID))]
        public PartSNEntity PartSN { get; set; }
        /// <summary>
        /// 配件类型
        ///</summary>
        [SugarColumn(ColumnName = "C_ISMAINSN")]
        public PartSNType IsMainSN { get; set; }
        #endregion

    }

    /// <summary>
    /// 构成类型
    /// </summary>
    public enum PartSNType
    {
        /// <summary>
        /// 配件码
        /// </summary>
        PartSN = 0,
        /// <summary>
        /// 主码
        /// </summary>
        MainSN = 1,
        /// <summary>
        /// 产品码
        /// </summary>
        ProductSN = 2,
    }

    /// <summary>
    /// 产品条码信息
    /// </summary>
    [Tenant("MesConfig")]
    [SugarTable("TB_PARTSN", TableDescription = "配件条码信息")]
    public partial class PartSNEntity
    {

        public PartSNEntity()
        {
            this.KeyID = Guid.NewGuid().ToString();
            this.CreatorTime = DateTime.Now;
        }
        #region 实体成员
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, ColumnName = "F_Id")]
        public string KeyID { get; set; }
        /// <summary>
        /// 创建人编号
        ///</summary>
        [SugarColumn(ColumnName = "F_CREATORUSERID")]
        public string CreatorUserId { get; set; }
        /// <summary>
        /// 创建时间
        ///</summary>
        [SugarColumn(ColumnName = "F_CREATORTIME")]
        public DateTime? CreatorTime { get; set; }
        /// <summary>
        /// 修改人
        ///</summary>
        [SugarColumn(ColumnName = "F_LASTMODIFYUSERID")]
        public String LastModifyUserId { get; set; }
        /// <summary>
        /// 修改时间
        ///</summary>
        [SugarColumn(ColumnName = "F_LASTMODIFYTIME")]
        public DateTime? LastModifyTime { get; set; }
        /// <summary>
        /// 删除时间
        ///</summary>
        [SugarColumn(ColumnName = "F_DELETETIME")]
        public DateTime? DeleteTime { get; set; }
        /// <summary>
        /// 删除人员ID
        ///</summary>
        [SugarColumn(ColumnName = "F_DELETEUSERID")]
        public string DeleteUserId { get; set; }
        /// <summary>
        /// 配件ID
        ///</summary>
        [SugarColumn(ColumnName = "C_PARTID")]
        public String PartID { get; set; }
        /// <summary>
        /// 配件
        ///</summary>
        [SugarColumn(ColumnName = "PART", IsIgnore = true)]
        [Navigate(NavigateType.OneToOne, nameof(PartID))]
        public PartEntity Part { get; set; }
        /// <summary>
        /// 条码
        ///</summary>
        [SugarColumn(ColumnName = "C_SN")]
        public string SN { get; set; }
        /// <summary>
        /// 使用次数统计
        /// </summary>
        [SugarColumn(ColumnName = "C_USECOUNT")]
        public int UseCount { get; set; }
        /// <summary>
        /// 是否可用
        ///</summary>
        [SugarColumn(ColumnName = "ENABLEDMARK")]
        public bool? EnabledMark { get; set; } = true;

        #endregion

        #region 扩展操作

        #endregion

    }

    /// <summary>
    /// 配件信息
    ///</summary>
    [Tenant("MesConfig")]
    [SugarTable("TB_PART", TableDescription = "配件表")]
    public partial class PartEntity
    {
        #region 实体成员
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, ColumnName = "F_Id")]
        public string KeyID { get; set; }
        /// <summary>
        /// 创建人编号
        ///</summary>
        [SugarColumn(ColumnName = "F_CREATORUSERID")]
        public string CreatorUserId { get; set; }
        /// <summary>
        /// 创建时间
        ///</summary>
        [SugarColumn(ColumnName = "F_CREATORTIME")]
        public DateTime? CreatorTime { get; set; }
        /// <summary>
        /// 修改人
        ///</summary>
        [SugarColumn(ColumnName = "F_LASTMODIFYUSERID")]
        public String LastModifyUserId { get; set; }
        /// <summary>
        /// 修改时间
        ///</summary>
        [SugarColumn(ColumnName = "F_LASTMODIFYTIME")]
        public DateTime? LastModifyTime { get; set; }
        /// <summary>
        /// 删除时间
        ///</summary>
        [SugarColumn(ColumnName = "F_DELETETIME")]
        public DateTime? DeleteTime { get; set; }
        /// <summary>
        /// 删除人员ID
        ///</summary>
        [SugarColumn(ColumnName = "F_DELETEUSERID")]
        public string DeleteUserId { get; set; }
        /// <summary>
        /// 配件名称
        ///</summary>
        [SugarColumn(ColumnName = "C_PARTNAME")]
        public string PartName { get; set; }
        /// <summary>
        /// 注释
        ///</summary>
        [SugarColumn(ColumnName = "C_DESC")]
        public string Desc { get; set; }
        /// <summary>
        /// 是否可用
        ///</summary>
        [SugarColumn(ColumnName = "ENABLEDMARK")]
        public bool? EnabledMark { get; set; }

        #endregion
    }
}
