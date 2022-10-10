using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public class UnitUpdateNavOneToOne
    {
        public static void Init()
        {
            Console.WriteLine("");
            Console.WriteLine("#### Suber Start ####");
            SqlSugarClient db = NewUnitTest.Db;
            db.Aop.OnLogExecuted = (s, p) =>
            {
                Console.WriteLine(s);
                Console.WriteLine(string.Join(",", p?.Select(it => it.ParameterName + ":" + it.Value)));
            };


            Console.WriteLine();


            var productStation = db.Queryable<ProductStationEntity>().Where(t => t.KeyID == "d0678f52-b6f9-41f8-a49a-be850fe49498")
                .Includes(t => t.Data)
                .First();



            Console.WriteLine();
            ////强制分表类型
            var result = db.UpdateNav(productStation).Include(t => t.Data).ExecuteCommand();


            Console.WriteLine("#### suber end ####");
        }
        /// <summary>
        /// 工作站数据保存方式
        ///</summary>
        [SugarTable("TS_PRODUCTSTATION", TableDescription = "工作站数据保存方式")]
        public partial class ProductStationEntity : CommonEntity
        {
            #region 实体成员
            /// <summary>
            /// 记录ID
            ///</summary>
            [SugarColumn(ColumnName = "c_ProductRecordID", ColumnDataType = "varchar(50)", ColumnDescription = "产品ID")]
            public string ProductRecordID { get; set; }
            /// <summary>
            /// 班次ID
            ///</summary>
            [SugarColumn(ColumnName = "c_ClassID", ColumnDataType = "varchar(50)", ColumnDescription = "班次ID")]
            public string ClassID { get; set; }
            /// <summary>
            /// 产线ID
            ///</summary>
            [SugarColumn(ColumnName = "c_LineID", ColumnDataType = "varchar(50)", ColumnDescription = "产线ID")]
            public string LineID { get; set; }
            /// <summary>
            /// 工作站ID
            ///</summary>
            [SugarColumn(ColumnName = "c_BaseStationID", ColumnDataType = "varchar(50)", ColumnDescription = "工作站ID")]
            public string BaseStationID { get; set; }
            /// <summary>
            /// 子工作站编号（0表示当前工作站，不区分子工位，从1开始编号）
            /// </summary>
            [SugarColumn(ColumnName = "c_ChildNumber", ColumnDataType = "int", ColumnDescription = "子工作站编号")]
            public int ChildNumber { get; set; }
            /// <summary>
            /// 结果代码
            ///</summary>
            [SugarColumn(ColumnName = "c_ResultCode", ColumnDataType = "int", ColumnDescription = "结果代码")]
            public int ResultCode { get; set; }
            /// <summary>
            /// 判定结果
            ///</summary>
            [SugarColumn(ColumnName = "c_Result", ColumnDataType = "bit", ColumnDescription = "判定结果")]
            public bool Result { get; set; }
            /// <summary>
            /// 托盘过程码
            ///</summary>
            [SugarColumn(ColumnName = "c_TrayProcessCode", ColumnDataType = "varchar(50)", ColumnDescription = "托盘过程码")]
            public string TrayProcessCode { get; set; }
            /// <summary>
            /// 获取数据方式
            ///</summary>
            [SugarColumn(ColumnName = "c_GetDataType", ColumnDataType = "int", ColumnDescription = "获取数据方式")]
            public int GetDataType { get; set; }
            /// <summary>
            /// 当前配方记录ID
            ///</summary>
            [SugarColumn(ColumnName = "c_RecipeRecordID", ColumnDataType = "varchar(50)", ColumnDescription = "当前配方记录ID")]
            public string RecipeRecordID { get; set; }
            /// <summary>
            /// 工作站数据
            ///</summary>
            [SugarColumn(ColumnName = "ProductData", IsIgnore = true)]
            [Navigate(NavigateType.OneToOne, nameof(KeyID), nameof(ProductDataEntity.ProductStationID))]
            public ProductDataEntity Data { get; set; }
            #endregion


        }
        /// <summary>
        ///产品数据记录
        ///</summary>
        [SugarTable("TS_PRODUCTDATA", TableDescription = "产品数据记录表")]
        public partial class ProductDataEntity : CommonEntity
        {
            /// <summary>
            /// 构造函数
            /// </summary>
            public ProductDataEntity()
            {
                this.KeyID = Guid.NewGuid().ToString();
                this.CreatorTime = DateTime.Now;
            }
            public ProductDataEntity(string productStationID)
            {
                this.ProductStationID = productStationID;
                this.LastModifyTime = DateTime.Now;
            }
            #region 实体成员
            /// <summary>
            /// 记录工作站ID
            ///</summary>
            [SugarColumn(ColumnName = "c_ProductStationID", ColumnDataType = "varchar(50)", ColumnDescription = "记录工作站ID")]
            public string ProductStationID { get; set; }
            #endregion
            #region 参数表
            /// <summary>
            /// c_column0
            ///</summary>
            [SugarColumn(ColumnName = "c_column0")]
            public decimal? Column0 { get; set; }
            /// <summary>
            /// c_column1
            ///</summary>
            [SugarColumn(ColumnName = "c_column1")]
            public decimal? Column1 { get; set; }
            /// <summary>
            /// c_column2
            ///</summary>
            [SugarColumn(ColumnName = "c_column2")]
            public decimal? Column2 { get; set; }
            /// <summary>
            /// c_column3
            ///</summary>
            [SugarColumn(ColumnName = "c_column3")]
            public decimal? Column3 { get; set; }
            /// <summary>
            /// c_column4
            ///</summary>
            [SugarColumn(ColumnName = "c_column4")]
            public decimal? Column4 { get; set; }
            /// <summary>
            /// c_column5
            ///</summary>
            [SugarColumn(ColumnName = "c_column5")]
            public decimal? Column5 { get; set; }
            /// <summary>
            /// c_column6
            ///</summary>
            [SugarColumn(ColumnName = "c_column6")]
            public decimal? Column6 { get; set; }
            /// <summary>
            /// c_column7
            ///</summary>
            [SugarColumn(ColumnName = "c_column7")]
            public decimal? Column7 { get; set; }
            /// <summary>
            /// c_column8
            ///</summary>
            [SugarColumn(ColumnName = "c_column8")]
            public decimal? Column8 { get; set; } /// <summary>
                                                  /// c_column0
                                                  ///</summary>
            [SugarColumn(ColumnName = "c_column9")]
            public decimal? Column9 { get; set; } /// <summary>
                                                  /// c_column0
                                                  ///</summary>
            [SugarColumn(ColumnName = "c_column10")]
            public decimal? Column10 { get; set; } /// <summary>
                                                   /// c_column0
                                                   ///</summary>
            [SugarColumn(ColumnName = "c_column11")]
            public decimal? Column11 { get; set; } /// <summary>
                                                   /// c_column0
                                                   ///</summary>
            [SugarColumn(ColumnName = "c_column12")]
            public decimal? Column12 { get; set; } /// <summary>
                                                   /// c_column0
                                                   ///</summary>
            [SugarColumn(ColumnName = "c_column13")]
            public decimal? Column13 { get; set; } /// <summary>
                                                   /// c_column0
                                                   ///</summary>
            [SugarColumn(ColumnName = "c_column14")]
            public decimal? Column14 { get; set; } /// <summary>
                                                   /// c_column0
                                                   ///</summary>
            [SugarColumn(ColumnName = "c_column15")]
            public decimal? Column15 { get; set; } /// <summary>
                                                   /// c_column0
                                                   ///</summary>
            [SugarColumn(ColumnName = "c_column16")]
            public decimal? Column16 { get; set; } /// <summary>
                                                   /// c_column0
                                                   ///</summary>
            [SugarColumn(ColumnName = "c_column17")]
            public decimal? Column17 { get; set; } /// <summary>
                                                   /// c_column0
                                                   ///</summary>
            [SugarColumn(ColumnName = "c_column18")]
            public decimal? Column18 { get; set; } /// <summary>
                                                   /// c_column0
                                                   ///</summary>
            [SugarColumn(ColumnName = "c_column19")]
            public decimal? Column19 { get; set; } /// <summary>
                                                   /// c_column0
                                                   ///</summary>
            [SugarColumn(ColumnName = "c_column20")]
            public decimal? Column20 { get; set; } /// <summary>
                                                   /// c_column0
                                                   ///</summary>
            [SugarColumn(ColumnName = "c_column21")]
            public decimal? Column21 { get; set; }
            #endregion

            #region 扩展操作



            #endregion
        }

        /// <summary>
        /// 通用属性
        /// </summary>
        public abstract class CommonEntity
        {
            /// <summary>
            /// 主键
            /// </summary>
            [SugarColumn(IsPrimaryKey = true, ColumnName = "F_Id", ColumnDataType = "varchar(50)", ColumnDescription = "主键")]
            public string KeyID { get; set; }
            /// <summary>
            /// 创建人编号
            ///</summary>
            [SugarColumn(ColumnName = "F_CreatorUserId", ColumnDataType = "varchar(50)", ColumnDescription = "创建人编号", IsOnlyIgnoreUpdate = true)]
            public string CreatorUserId { get; set; }
            /// <summary>
            /// 创建时间
            ///</summary>
            [SugarColumn(ColumnName = "F_CreatorTime", ColumnDataType = "datetime", ColumnDescription = "创建时间", IsOnlyIgnoreUpdate = true)]
            public DateTime? CreatorTime { get; set; }
            /// <summary>
            /// 修改人
            ///</summary>
            [SugarColumn(ColumnName = "F_LastModifyUserId", ColumnDataType = "varchar(50)", ColumnDescription = "修改人编号", IsOnlyIgnoreInsert = true)]
            public string LastModifyUserId { get; set; }
            /// <summary>
            /// 修改时间
            ///</summary>
            [SugarColumn(ColumnName = "F_LastModifyTime", ColumnDataType = "datetime", ColumnDescription = "修改时间", IsOnlyIgnoreInsert = true)]
            public DateTime? LastModifyTime { get; set; }
            /// <summary>
            /// 删除时间
            ///</summary>
            [SugarColumn(ColumnName = "F_DeleteTime", ColumnDataType = "datetime", ColumnDescription = "删除时间", IsOnlyIgnoreInsert = true)]
            public DateTime? DeleteTime { get; set; }
            /// <summary>
            /// 删除人员ID
            ///</summary>
            [SugarColumn(ColumnName = "F_DeleteUserId", ColumnDataType = "varchar(50)", ColumnDescription = "删除人员ID", IsOnlyIgnoreInsert = true)]
            public string DeleteUserId { get; set; }

        }
    }
}
