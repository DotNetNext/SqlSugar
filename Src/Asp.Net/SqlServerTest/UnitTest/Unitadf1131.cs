using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    internal class Unitadf1131
    {

        public static void Init() 
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<PmtSetting>();
            db.CodeFirst.InitTables<BankTemplate>();
            db.DbMaintenance.TruncateTable<PmtSetting, BankTemplate>();
            db.Insertable(new PmtSetting() {
             TemplateName= TemplateName.T1_HSBC_CNY,
              PaymentApproach="a"
            }).ExecuteCommand();
            db.Insertable(new BankTemplate()
            {
                TemplateName = TemplateName.T1_HSBC_CNY,
              TemplateData=new byte[] { 1},
               TemplateFileName="a"
            }).ExecuteCommand();
            var list=db.Queryable<PmtSetting>().Includes(x => x.Template).ToList();
            var sql=db.Queryable<Unitadfaasdfasfa>()
                .Where(it => it.num == SqlFunc.Subqueryable<UnitadfaasdfasfaDTO>().GroupBy(s => s.num).Select(s => s.num))
                .ToSqlString();
            if (!sql.Contains("[num]  in")) { throw new Exception("unit error"); }

            db.CodeFirst.InitTables<Unitadfaasdfasfa, UnitadfaasdfasfaDTO>();
            var sql2 = db.Queryable<Unitadfaasdfasfa>()
              .Where(
                it => it.num == SqlFunc.Subqueryable<UnitadfaasdfasfaDTO>().GroupBy(s => s.num).Select(s => s.num)
               && it.num == SqlFunc.Subqueryable<UnitadfaasdfasfaDTO>().GroupBy(s => s.num).Select(s => s.num))
              .ToList();

            decimal tmp1 = 1110; 
            decimal tmp2 = 2220;
            var sql3 =   db.Updateable<TbRenWuEx>()

              .SetColumns(it => new TbRenWuEx() { WorkWgt = tmp2 + tmp1 })

              .Where(it => it.PoundNo == "")

              .ToSqlString();

            if (!sql3.Contains("( 2220 + 1110 )")) 
            {
                throw new Exception("unit error");
            }
        }
        /// <summary>

        /// </summary>

        [SugarTable("tb_renwuex2")]

        public class TbRenWuEx

        {

            /// <summary>

            /// </summary>

            [SugarColumn(ColumnName = "ID", IsPrimaryKey = true, IsIdentity = true)]

            public int Id { get; set; }



            /// <summary>

            /// </summary>

            [SugarColumn(ColumnName = "POUND_NO")]

            public string PoundNo { get; set; }



            /// <summary>

            /// </summary>

            [SugarColumn(ColumnName = "CARGO_NAM")]

            public string CargoNam { get; set; }



            /// <summary>

            ///

            /// </summary>

            [SugarColumn(ColumnName = "OUT_TOOL")]

            public string OutTool { get; set; }



            /// <summary>

            /// </summary>

            [SugarColumn(ColumnName = "BARGE_NO")]

            public string BargeNo { get; set; }



            /// <summary>

            /// </summary>

            [SugarColumn(ColumnName = "WORK_WGT")]

            public decimal? WorkWgt { get; set; }



            /// <summary>

            /// 是否完成

            /// </summary>

            [SugarColumn(ColumnName = "FinishFlag")]

            public byte? FinishFlag { get; set; }



            /// <summary>

            /// 记录时间

            /// </summary>

            [SugarColumn(ColumnName = "RecDate")]

            public DateTime? RecDate { get; set; }





        }
        public class Unitadfaasdfasfa 
        {
            public string Id { get; set; }
            public long num { get; set; }
        }

        public class UnitadfaasdfasfaDTO
        {
            public string Id { get; set; }
            public long? num { get; set; }
        }

        // 主表
        public class PmtSetting  
        {
    

            [SugarColumn(ColumnDataType = "int", ColumnName = "TemplateName", IsNullable = true)]
            public TemplateName TemplateName { get; set; }

            public string PaymentApproach { get; set; }
            [Navigate(NavigateType.OneToOne, nameof(TemplateName), nameof(BankTemplate.TemplateName))]
            public BankTemplate Template { get; set; } //子表对象
        }



        // 子表
        public class BankTemplate
        {
         
            [SugarColumn(ColumnDataType = "int", IsNullable = false, IsPrimaryKey = true)]
            public TemplateName TemplateName { get; set; }
            public string TemplateFileName { get; set; }
            public byte[] TemplateData { get; set; }
        }
        // 枚举
        public enum TemplateName
        {
            T1_HSBC_CNY = 1,
            T2_HSBC_NONCNY = 2,
            T3_BOC_PRIVATE = 3,
            T4_BOC_USD = 4,
            T5_BOC_EUR = 5,
            T6_BOC_OTHERS = 6,
            T7_BOC_UNIT = 7
        }
    }
}
