using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    internal class b10_GetConfigValueTest
    {
        public static void Init()
        {
            // Get a new database instance
            // 获取新的数据库实例
            var db = DbHelper.GetNewDb();

            // Create the database if it doesn't exist
            // 如果数据库不存在，则创建数据库
            db.DbMaintenance.CreateDatabase();

            db.CodeFirst.InitTables<EavEntityType>();
            db.CodeFirst.InitTables<SysDictType>();
            db.CodeFirst.InitTables<SysDictData>();

            // 清空表
            db.DbMaintenance.TruncateTable<EavEntityType>();
            db.DbMaintenance.TruncateTable<SysDictType>();
            db.DbMaintenance.TruncateTable<SysDictData>();

            // 插入数据
            // 字典类型
            db.Insertable(new SysDictType() { DictName = "系统是否", DictType = "sys_yes_no" }).ExecuteCommand();//用例代码    

            // 字典值
            db.Insertable(new SysDictData() { DictLabel = "是", DictValue = "Y", DictType = "sys_yes_no" }).ExecuteCommand();//用例代码
            db.Insertable(new SysDictData() { DictLabel = "否", DictValue = "N", DictType = "sys_yes_no" }).ExecuteCommand();//用例代码


            // 实体类型
            db.Insertable(new EavEntityType() { EntityTypeName = "人员", EntityTypeCode = "Human", IsActive = true }).ExecuteCommand();//用例代码

            // 配置
            var types = db.Queryable<SysDictType>()
                .Select(it => it.DictType)
                .ToList();

            //上面有耗时操作写在Any上面，保证程序启动后只执行一次
            if (!db.ConfigQuery.Any())
            {
                foreach (var type in types)
                {
                    //db.ConfigQuery.SetTable<SysDictData>(it => SqlFunc.ToString(it.DictValue), it => it.DictLabel, type, it => it.DictType == type);
                    db.ConfigQuery.SetTable<SysDictData>(it => it.DictValue.ToString(), it => it.DictLabel, type, it => it.DictType == type);
                }
            }

            //  查询
            var total = 0;
            var result = db.Queryable<EavEntityType>().Select((it) => new
            {
                it,
                IsActiveLabel = (it.IsActive ? "Y" : "N").GetConfigValue<SysDictData>("sys_yes_no"),
            }, true)
            .ToPageList(1, 100000, ref total);
        }
    }

    [SugarTable("sys_dict_type", "字典类型表")]
    [SugarIndex("index_dict_type", nameof(DictType), OrderByType.Asc, true)]
    public class SysDictType
    {
        /// <summary>
        /// 字典名称
        /// </summary>
        [SugarColumn(Length = 100, ExtendedAttribute = ProteryConstant.NOTNULL)]
        public string DictName { get; set; }
        /// <summary>
        /// 字典类型
        /// </summary>
        [SugarColumn(Length = 100, ExtendedAttribute = ProteryConstant.NOTNULL)]
        public string DictType { get; set; }
    }
    [SugarTable("sys_dict_data", "字典数据表")]
    public class SysDictData
    {
        /// <summary>
        /// 字典标签
        /// </summary>
        [SugarColumn(Length = 100, ExtendedAttribute = ProteryConstant.NOTNULL)]
        public string DictLabel { get; set; }
        /// <summary>
        /// 字典键值
        /// </summary>
        [SugarColumn(Length = 100, ExtendedAttribute = ProteryConstant.NOTNULL)]
        public string DictValue { get; set; }
        /// <summary>
        /// 字典类型
        /// </summary>
        [SugarColumn(Length = 100, ExtendedAttribute = ProteryConstant.NOTNULL)]
        public string DictType { get; set; }
    }
    /// <summary>
    /// 实体类型
    /// </summary>
    [SugarTable("eav_entity_type")]
    public class EavEntityType
    {
        /// <summary>
        /// 主键 主键，自增ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true, ColumnName = "entity_type_id")]
        public int? EntityTypeId { get; set; }

        /// <summary>
        /// 代码 实体类型代码（如 product, user）
        /// </summary>
        [SugarColumn(ColumnName = "entity_type_code")]
        public string EntityTypeCode { get; set; }

        /// <summary>
        /// 名称 实体类型名称（如 产品, 用户））
        /// </summary>
        [SugarColumn(ColumnName = "entity_type_name")]
        public string EntityTypeName { get; set; }

        /// <summary>
        /// 启用 是否启用该实体类型
        /// </summary>
        [SugarColumn(ColumnName = "is_active")]
        public bool IsActive { get; set; }

    }
    public enum ProteryConstant
    {
        NOTNULL = 0
    }

}