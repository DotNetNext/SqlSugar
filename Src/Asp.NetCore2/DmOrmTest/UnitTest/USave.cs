using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public partial class NewUnitTest
    {
        public static void Save()
        {
            Db.CodeFirst.InitTables<UnitSysMenu>();
            Db.DbMaintenance.TruncateTable<UnitSysMenu>();
            Db.Saveable<UnitSysMenu>(new UnitSysMenu() { ID="aa", ButtonList="", CreateName="a", CreateTime=DateTime.Now, ImageUrl="", IsDel=true, MenuCode="a", NavigateUrl="a", UpdateName="", Remark="", UpdateTime=DateTime.Now }).ExecuteReturnEntity();
        }
    }
    public class BaseEntity 
    {
        /// <summary>
        /// ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, ColumnDescription = "ID", Length = 32)]
        public string ID { get; set; }
        /// <summary>
        /// 删除标记
        /// </summary>
        [ SugarColumn(ColumnDescription = "删除标记")]
        public bool IsDel { get; set; }
        /// <summary>
        /// 排序字段
        /// </summary>
        [SugarColumn(ColumnDescription = "排序字段", IsIdentity = true)]
        public int SortIndex { get; set; }
        /// <summary>
        /// 添加操作员
        /// </summary>
        [ SugarColumn(ColumnDescription = "添加操作员", Length = 10, IsNullable = true)]
        public string CreateName { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        [ SugarColumn(ColumnDescription = "添加时间", IsNullable = true)]
        public DateTime? CreateTime { get; set; }
        /// <summary>
        /// 修改操作员
        /// </summary>
        [ SugarColumn(ColumnDescription = "修改操作员", Length = 10, IsNullable = true)]
        public string UpdateName { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        [ SugarColumn(ColumnDescription = "修改时间", IsNullable = true)]
        public DateTime? UpdateTime { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn(ColumnDescription = "备注", Length = 150, IsNullable = true)]
        public string Remark { get; set; }
    }
    public class UnitSysMenu : BaseEntity
    {
        public UnitSysMenu()
        {
        }
        /// <summary>
        /// 菜单图标
        /// </summary>
        [SqlSugar.SugarColumn(Length = 150, ColumnDescription = "菜单图标")]

        public string ImageUrl { get; set; }
        /// <summary>
        /// 菜单编码
        /// </summary>
        [SqlSugar.SugarColumn(Length = 50, ColumnDescription = "菜单编码")]
        public string MenuCode { get; set; }
        /// <summary>
        /// 菜单地址
        /// </summary>
        [ SqlSugar.SugarColumn(Length = 200, IsNullable = true, ColumnDescription = "菜单地址")]
        public string NavigateUrl { get; set; }
        /// <summary>
        /// 菜单功能
        /// </summary>
        [SqlSugar.SugarColumn(IsIgnore = true)]
        public string ButtonList { get; set; }

        /// <summary>
        /// 排序字段
        /// </summary>
        [SugarColumn(ColumnDescription = "排序字段", IsIdentity = true)]
        public int SortIndex { get; set; }
    }
}
