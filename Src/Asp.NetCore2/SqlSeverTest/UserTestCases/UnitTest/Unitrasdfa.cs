using OrmTest;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrmTest
{
    internal class Unitrasdfa
    {
        public static void Init() 
        {
            var db = NewUnitTest.Db;
            db.CurrentConnectionConfig.ConfigId = "a";
            var sql= db.QueryableWithAttr<FlowTemplateEntity>()
                .InnerJoin<FlowTemplateEntity>((t, tj) => t.Id == tj.tempId)
                .ToSqlString();


            var test =db.Queryable<PRT_BMS_SA_WEBCON_DOC_V>().Where(a =>  a.SALESID == (long)SqlFunc.Subqueryable<PRT_BMS_SA_WEBCON_DTL_VP>().GroupBy(z => z.SALESID).Select(z => z.SALESID)).Select(a => new { a.SALESID, a.SACONNO }).Clone()
             .ToSqlString();
        }
    }
    [SugarTable("FLOW_TEMPLATE")]
    [Tenant("a")]
    public class FlowTemplateEntity  
    {
        public object tempId { get; set; }

        /// <summary>
        /// 流程编码.
        /// </summary>
        [SugarColumn(ColumnName = "F_ENCODE")]
        public string EnCode { get; set; }

        /// <summary>
        /// 流程名称.
        /// </summary>
        [SugarColumn(ColumnName = "F_FULLNAME")]
        public string FullName { get; set; }

        /// <summary>
        /// 流程类型（0：发起流程，1：功能流程）.
        /// </summary>
        [SugarColumn(ColumnName = "F_TYPE")]
        public int? Type { get; set; }

        /// <summary>
        /// 流程分类.
        /// </summary>
        [SugarColumn(ColumnName = "F_CATEGORY")]
        public string Category { get; set; }

        /// <summary>
        /// 图标.
        /// </summary>
        [SugarColumn(ColumnName = "F_ICON")]
        public string Icon { get; set; }

        /// <summary>
        /// 图标背景色.
        /// </summary>
        [SugarColumn(ColumnName = "F_ICONBACKGROUND")]
        public string IconBackground { get; set; }

        /// <summary>
        /// 描述.
        /// </summary>
        [SugarColumn(ColumnName = "F_DESCRIPTION")]
        public string Description { get; set; }

        /// <summary>
        /// 排序码.
        /// </summary>
        [SugarColumn(ColumnName = "F_SORTCODE")]
        public long? SortCode { get; set; }
        public object Id { get;   set; }
    }



    [SugarTable("FLOW_TEMPLATEJSON")]
    [Tenant("a")]
    public class FlowTemplateJsonEntity  
    {
        /// <summary>
        /// 流程编码.
        /// </summary>
        [SugarColumn(ColumnName = "F_TEMPLATEID")]
        public string TemplateId { get; set; }

        /// <summary>
        /// 可见类型.
        /// </summary>
        [SugarColumn(ColumnName = "F_VISIBLETYPE")]
        public int? VisibleType { get; set; }

        /// <summary>
        /// 流程版本.
        /// </summary>
        [SugarColumn(ColumnName = "F_VERSION")]
        public string Version { get; set; }

        /// <summary>
        /// 流程模板.
        /// </summary>
        [SugarColumn(ColumnName = "F_FLOWTEMPLATEJSON")]
        public string FlowTemplateJson { get; set; }
    }
}
