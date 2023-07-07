using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest.UnitTest
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
