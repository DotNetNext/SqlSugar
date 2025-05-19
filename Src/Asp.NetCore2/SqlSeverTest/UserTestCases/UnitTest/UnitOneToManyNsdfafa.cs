using OrmTest;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
namespace OrmTest
{
    internal class UnitOneToManyNsdfafa
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<BaseProcess, ProcessPlanPackage, ProcessPlanPackageEntry, ProcessPlanPackageEntry, ProcessPlan
                 >();
            db.CodeFirst.InitTables<ProcessPlanEntry>();
        

            Demo1(db);
            Demo2(db);
        }

        private static void Demo2(SqlSugarClient db )
        {
           
            var PlanPackage2 = db.Queryable<ProcessPlanPackage>()
           .Where(p => p.processPlanPackageEntries.Any(z =>
           z.processPlan.Entries.Any(c =>
              c.OrgId == 11 &&
                          c.Process.Type == ProcessEnum.混色 &&
                       
                          c.Pass == 0)))
           .SingleAsync(p => p.Code == "a").GetAwaiter().GetResult();
        }
        private static void Demo1(SqlSugarClient db)
        {
            var sql = db.Queryable<ProcessPlanPackage>()
            .Where(p => p.processPlanPackageEntries.Any(z =>
            z.processPlan.Entries.Any(c =>

                           c.Process.Type == ProcessEnum.混色 &&
                           c.OrgId == 11 &&
                           c.Pass == 0)))
            .ToSqlString();
            if (!sql.Contains("SELECT [Type] FROM [BaseProcess]  WHERE  [ProcessPlanEntry1].[ProcessId]=[Id]"))
            {
                throw new Exception("unit error");
            }
            var PlanPackage2 = db.Queryable<ProcessPlanPackage>()
           .Where(p => p.processPlanPackageEntries.Any(z =>
           z.processPlan.Entries.Any(c =>

                          c.Process.Type == ProcessEnum.混色 &&
                          c.OrgId == 11 &&
                          c.Pass == 0)))
           .SingleAsync(p => p.Code == "a").GetAwaiter().GetResult();
        }

        /// <summary>
        /// 作业表
        /// </summary>
        [SugarTable(null, "作业表")] 
        public class BaseProcess : EntityBase
        {
            /// <summary>
            /// 名称
            /// </summary>
            [SugarColumn(ColumnDescription = "名称", Length = 50)]
            [Required, MaxLength(50)]
            public string Name { get; set; }

            /// <summary>
            /// 编码
            /// </summary>
            [SugarColumn(ColumnDescription = "编码", Length = 50)]
            [Required, MaxLength(50)]
            public string Code { get; set; }

            /// <summary>
            /// 班组Id
            /// </summary>
            [SugarColumn(ColumnDescription = "班组Id")]
            [Required]
            public long SysOrgId { get; set; }

            /// <summary>
            /// 图标
            /// </summary>
            [SugarColumn(ColumnDescription = "图标", Length = 50)]
            [MaxLength(50)]
            public string Icon { get; set; }

            /// <summary>
            /// 工序类别
            /// </summary>
            [SugarColumn(ColumnDescription = "工序类别")]
            public ProcessEnum Type { get; set; }
        }
        /// <summary>
        /// 工序类型枚举（多次报工领料区分）
        /// </summary>
        [Description("工序类型枚举（多次报工领料区分）")]
        public enum ProcessEnum
        {
            /// <summary>
            /// 普通
            /// </summary>
            [Description("普通")]
            普通 = 0,

            /// <summary>
            /// 混色
            /// </summary>
            [Description("混色")]
            混色 = 1,

            /// <summary>
            /// 分把
            /// </summary>
            [Description("分把")]
            分把 = 2,
            /// <summary>
            /// 包装
            /// </summary>
            [Description("包装")]
            包装 = 3,
            /// <summary>
            /// PU
            /// </summary>
            [Description("PU")]
            PU = 4,
        }
        /// <summary>
        /// 工序计划包
        /// </summary>
        [SugarTable(null, "工序计划包")]
        public class ProcessPlanPackage : EntityBase
        {
            /// <summary>
            /// 编码
            /// </summary>
            [SugarColumn(ColumnDescription = "编码", Length = 50)]
            [Required]
            public string Code { get; set; }

            /// <summary>
            /// 名称
            /// </summary>
            [SugarColumn(ColumnDescription = "名称", Length = 50)]
            public string Name { get; set; }

            ///// <summary>
            ///// 汇报重量
            ///// </summary>
            //[SugarColumn(ColumnDescription = "汇报重量")]
            //public decimal Num { get; set; }

            /// <summary>
            /// 工序计划包明细
            /// </summary>
            [Navigate(NavigateType.OneToMany, nameof(ProcessPlanPackageEntry.PlanPackageId))]
            public List<ProcessPlanPackageEntry> processPlanPackageEntries { get; set; }
        }

        /// <summary>
        /// 工序计划包明细
        /// </summary>
        [SugarTable(null, "工序计划包明细")]
        public class ProcessPlanPackageEntry : EntityBase
        {
            /// <summary>
            /// 工序计划包Id
            /// </summary>
            [SugarColumn(ColumnDescription = "工序计划包Id")]
            [Required]
            public long PlanPackageId { get; set; }

            /// <summary>
            /// 计划Id
            /// </summary>
            [SugarColumn(ColumnDescription = "计划Id")]
            [Required]
            public long PlanId { get; set; }

            /// <summary>
            /// 工序计划包明细
            /// </summary>
            [Navigate(NavigateType.OneToOne, nameof(PlanId))]
            public ProcessPlan processPlan { get; set; }

        }

        /// <summary>
        /// 工序计划表
        /// </summary>
        [SugarTable(null, "工序计划表")]
        public class ProcessPlan : EntityBase
        {
            /// <summary>
            /// 单据编号
            /// </summary>
            [SugarColumn(ColumnDescription = "单据编号", Length = 50)]
            [Required, MaxLength(50)]
            public string Code { get; set; }

            /// <summary>
            /// 物料Id
            /// </summary>
            [SugarColumn(ColumnDescription = "物料Id")]
            [Required]
            public long MaterialId { get; set; }


            /// <summary>
            /// erp工艺路线Id
            /// </summary>
            [SugarColumn(ColumnDescription = "erp工艺路线Id")]
            [Required]
            public long ProcessChainId { get; set; }


            /// <summary>
            /// 生产订单编号
            /// </summary>
            [SugarColumn(ColumnDescription = "生产订单编号", Length = 50)]
            [Required, MaxLength(50)]
            public string OrderCode { get; set; }

            /// <summary>
            /// 销售订单编号
            /// </summary>
            [SugarColumn(ColumnDescription = "销售订单编号", Length = 300)]
            [Required, MaxLength(300)]
            public string SaleCode { get; set; }

            /// <summary>
            /// 生产订单行号
            /// </summary>
            [SugarColumn(ColumnDescription = "生产订单行号")]
            [Required]
            public int OrderEntrySeq { get; set; }

            /// <summary>
            /// 数量
            /// </summary>
            [SugarColumn(ColumnDescription = "数量")]
            [Required]
            public decimal Num { get; set; }

            /// <summary>
            /// ERP未换算数量
            /// </summary>
            [SugarColumn(ColumnDescription = "ERP未换算数量")]
            [Required]
            public decimal ERPNum { get; set; }

            /// <summary>
            /// 计划开工时间
            /// </summary>
            [SugarColumn(ColumnDescription = "计划开工时间")]
            [Required]
            public DateTime StartDate { get; set; }

            /// <summary>
            /// 计划完工时间
            /// </summary>
            [SugarColumn(ColumnDescription = "计划完工时间")]
            [Required]
            public DateTime EndDate { get; set; }

            /// <summary>
            /// 工序计划工序列表
            /// </summary>
            [Navigate(NavigateType.OneToMany, nameof(ProcessPlanEntry.ProcessPlanId))]
            public List<ProcessPlanEntry> Entries { get; set; }
            /// <summary>
            /// 生产订单Id
            /// </summary>
            /// 
            [SugarColumn(ColumnDescription = "生产订单Id")]
            [Required]
            public long OrderId { get; set; }
            /// <summary>
            /// 是否成品订单
            /// </summary>
            /// 
            [SugarColumn(ColumnDescription = "是否成品订单")]
            [Required]
            public bool IsFinishedProduct { get; set; }
            /// <summary>
            /// 生产订单明细Id
            /// </summary>
            /// 
            [SugarColumn(ColumnDescription = "生产订单明细Id")]
            [Required]
            public long OrderEntryId { get; set; }

            /// <summary>
            /// 生产车间Id
            /// </summary>
            /// 
            [SugarColumn(ColumnDescription = "生产车间Id")]
            [Required]
            public long OrgId { get; set; }

            /// <summary>
            /// 进度
            /// </summary>
            [SugarColumn(IsIgnore = true)]
            public decimal Progress { get; set; }
        }
        /// <summary>
        /// 工序计划工序表
        /// </summary>
        [SugarTable(null, "工序计划工序表")]
        public class ProcessPlanEntry : EntityBase
        {
            /// <summary>
            /// 工序序列Id
            /// </summary>
            [SugarColumn(ColumnDescription = "工序序列Id")]
            [Required]
            public long SeqId { get; set; }
            /// <summary>
            /// 工序计划Id
            /// </summary>
            [SugarColumn(ColumnDescription = "工序计划Id")]
            [Required]
            public long ProcessPlanId { get; set; }

 
            /// <summary>
            /// ERP工序序号
            /// </summary>
            [SugarColumn(ColumnDescription = "ERP工序序号")]
            [Required, MaxLength(50)]
            public int ErpProcessNo { get; set; }

            /// <summary>
            /// ERP工序计划明细Id
            /// </summary>
            [SugarColumn(ColumnDescription = "ERP工序计划明细Id")]
            [Required]
            public long ErpId { get; set; }
 

            /// <summary>
            /// 作业Id
            /// </summary>
            [SugarColumn(ColumnDescription = "作业Id")]
            [Required]
            public long ProcessId { get; set; }
 
 

            /// <summary>
            /// 作业
            /// </summary>
            [Navigate(NavigateType.OneToOne, nameof(ProcessId))]
            public BaseProcess Process { get; set; }
            public int OrgId { get;   set; }
            public int Pass { get;   set; }
        }

        public class EntityBase
        {
            [SugarColumn(IsPrimaryKey =true)]
            public int Id { get; set; }
        }

    }

}
