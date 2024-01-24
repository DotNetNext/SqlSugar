using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    internal class Unitadfsafas
    {
        public static void Init() 
        {
            var Db = NewUnitTest.Db;
            Db.CurrentConnectionConfig.MoreSettings = new ConnMoreSettings()
            {

                 MaxParameterNameLength=30
            };
            Db.CodeFirst.InitTables<LIS6_REQ_INFO>();
            List<string> allIds = new List<string> { "1", "2", "3", "4" };
         
            List<LIS6_REQ_INFO> multipleRegistDatas = Db.Queryable<LIS6_REQ_INFO>().AS("XH_DATA.LIS6_REQ_INFO")
               .Where(o => allIds.Any(a => o.REQUISITION_ID.StartsWith(a)))
               .ToList();
        }
        [Table("LIS6_REQ_INFO")]
        public partial class LIS6_REQ_INFO
        {
            [Key]
            [StringLength(20)]
            [SugarColumn(IsPrimaryKey = true, SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string REQUISITION_ID { get; set; } = null!;
            [Column(TypeName = "DATE")]
            public DateTime GENERATE_DATE { get; set; }
            [StringLength(20)]
            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string INTERFACE_ID { get; set; } = null!;
            [StringLength(20)]
            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string HOSPITAL_REQID { get; set; } = null!;
            [StringLength(20)]
            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? HOSPITAL_COLLECTID { get; set; }
            [StringLength(20)]
            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? AREA_REQID { get; set; }
            [StringLength(20)]
            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? AREA_COLLECTID { get; set; }
            [StringLength(20)]
            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? REQ_ID { get; set; }
            [StringLength(20)]
            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? LAB_ID { get; set; }
            [StringLength(20)]
            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? PGROUP_ID { get; set; }
            [StringLength(20)]
            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? REQ_SOURCE { get; set; }
            [StringLength(20)]
            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? INSPECT_TYPE { get; set; }
            [StringLength(20)]
            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? PATIENT_TYPE { get; set; }
            [StringLength(50)]
            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? PAT_ID { get; set; }
            [StringLength(50)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? PATIENT_ID { get; set; }
            [StringLength(50)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? OUTPATIENT_ID { get; set; }
            [StringLength(50)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? INPATIENT_ID { get; set; }
            [StringLength(50)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? INVOICE_ID { get; set; }
            [StringLength(10)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? VISIT_NUM { get; set; }
            [StringLength(50)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? CHARGE_TYPE { get; set; }
            [StringLength(100)]
            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? PATIENT_NAME { get; set; }
            [StringLength(80)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? PATIENT_ENAME { get; set; }
            [StringLength(50)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? PATIENT_SNAME { get; set; }
            [StringLength(2)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? PATIENT_SEX { get; set; }
            [StringLength(20)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? PATIENT_AGE { get; set; }
            [Column(TypeName = "NUMBER")]
            public decimal? AGE_SAVE { get; set; }
            [Column(TypeName = "DATE")]
            public DateTime? PATIENT_BIRTHDAY { get; set; }
            [StringLength(10)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? PATIENT_HEIGHT { get; set; }
            [StringLength(10)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? PATIENT_WEIGHT { get; set; }
            [StringLength(50)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? PATIENT_NATION { get; set; }
            [StringLength(50)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? PATIENT_NATIONALITY { get; set; }
            [StringLength(10)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? CARD_TYPE { get; set; }
            [StringLength(80)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? ID_CARD { get; set; }
            [StringLength(80)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? MOBILE_NO { get; set; }
            [StringLength(500)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? PATIENT_ADDRESS { get; set; }
            [StringLength(20)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? BLOODTYPE_RH { get; set; }
            [StringLength(20)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? BLOODTYPE_ABO { get; set; }
            [StringLength(20)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? RH_PHENOTYPE { get; set; }
            [StringLength(50)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? PATIENT_GROUP { get; set; }
            [StringLength(50)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? PATIENT_DEPT { get; set; }
            [StringLength(50)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? PATIENT_DEPT_NAME { get; set; }
            [StringLength(50)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? PATIENT_WARD { get; set; }
            [StringLength(50)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? PATIENT_WARD_NAME { get; set; }
            [StringLength(20)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? PATIENT_BED { get; set; }
            [StringLength(50)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? PATIENT_UNIT { get; set; }
            [StringLength(100)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? CLINICAL_DIAGNOSES { get; set; }
            [StringLength(500)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? CLINICAL_DIAGNOSES_NAME { get; set; }
            [StringLength(20)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? SAMPLECLASS_NO { get; set; }
            [StringLength(50)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? SAMPLE_CLASS { get; set; }
            [StringLength(50)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? SAMPLE_CLASS_NAME { get; set; }
            [StringLength(50)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? SAMPLE_STATUS { get; set; }
            [StringLength(50)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? SAMPLE_STATUS_NAME { get; set; }
            [StringLength(50)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? SAMPLING_POSITION { get; set; }
            [StringLength(50)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? SAMPLING_POSITION_NAME { get; set; }
            [StringLength(50)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? REQUISITION_DEPT { get; set; }
            [Column(TypeName = "DATE")]
            public DateTime? REQUISITION_TIME { get; set; }
            [StringLength(50)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? REQUISITION_PERSON { get; set; }
            [StringLength(50)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? REQ_MOBILE_NO { get; set; }
            [StringLength(50)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? REQ_PRINT_DEPT { get; set; }
            [Column(TypeName = "DATE")]
            public DateTime? REQ_PRINT_TIME { get; set; }
            [StringLength(50)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? REQ_PRINT_PERSON { get; set; }
            [StringLength(50)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? EXECUTE_DEPT { get; set; }
            [Column(TypeName = "DATE")]
            public DateTime? EXECUTE_TIME { get; set; }
            [StringLength(50)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? EXECUTE_PERSON { get; set; }
            [Column(TypeName = "DATE")]
            public DateTime? GENERATION_TIME { get; set; }
            [Column(TypeName = "DATE")]
            public DateTime? SAMPLING_TIME { get; set; }
            [StringLength(50)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? SAMPLING_PERSON { get; set; }
            [Column(TypeName = "DATE")]
            public DateTime? CLASSIFY_TIME { get; set; }
            [StringLength(50)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? CLASSIFY_PERSON { get; set; }
            [Column(TypeName = "DATE")]
            public DateTime? GATHER_TIME { get; set; }
            [StringLength(50)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? GATHER_PERSON { get; set; }
            [Column(TypeName = "DATE")]
            public DateTime? SENDOUT_TIME { get; set; }
            [StringLength(50)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? SENDOUT_PERSON { get; set; }
            [Column(TypeName = "DATE")]
            public DateTime? SENDIN_TIME { get; set; }
            [StringLength(50)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? SENDIN_PERSON { get; set; }
            [Column(TypeName = "DATE")]
            public DateTime? INCEPT_TIME { get; set; }
            [StringLength(50)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? INCEPT_PERSON { get; set; }
            [Column(TypeName = "NUMBER")]
            public decimal? MIX_PAT_NUM { get; set; }
            [Column(TypeName = "DATE")]
            public DateTime? NUMING_TIME { get; set; }
            [StringLength(50)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? NUMING_PERSON { get; set; }
            [StringLength(30)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? SAMPLE_NUMBER { get; set; }
            [StringLength(50)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? INSPECTION_ID { get; set; }
            [Column(TypeName = "DATE")]
            public DateTime? REQ_ARCHIVED_TIME { get; set; }
            [StringLength(50)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? REQ_ARCHIVED_PERSON { get; set; }
            [Column(TypeName = "DATE")]
            public DateTime? REQ_DESTROY_TIME { get; set; }
            [StringLength(50)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? REQ_DESTROY_PERSON { get; set; }
            [StringLength(50)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? LASTEST_OPER { get; set; }
            [StringLength(50)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? LASTEST_LOCATION { get; set; }
            [StringLength(20)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? LASTEST_UNIT_ID { get; set; }
            [StringLength(200)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? REQ_ENTRUST { get; set; }
            [Column(TypeName = "DATE")]
            public DateTime? REQ_EXEC_TIME { get; set; }
            [StringLength(20)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? REQUISITION_STATE_BAK { get; set; }
            [StringLength(20)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? REQUISITION_STATE { get; set; }
            [StringLength(50)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? REQ_MAIN_ID { get; set; }
            [StringLength(500)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? ITEM_NAME_UNNUM { get; set; }
            [StringLength(500)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? UNQUALIFIED_CAUSE { get; set; }
            [StringLength(10)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? UNQUALIFIED_SIGN { get; set; }
            //[StringLength(50)]
            //[Unicode(false)]
            //[SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            //public string? REQ_ID_OLD { get; set; }
            [StringLength(20)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? CHARGE_STATE { get; set; }
            [Column(TypeName = "NUMBER")]
            public decimal? REQ_CHARGE { get; set; }
            [StringLength(500)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? TEST_ORDER_NAME { get; set; }
            [StringLength(20)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? COMBO_ITEM { get; set; }
            [StringLength(10)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? ITEM_TYPE { get; set; }
            [StringLength(50)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? FIRST_RPERSON { get; set; }
            [Column(TypeName = "DATE")]
            public DateTime? FIRST_RTIME { get; set; }
            [StringLength(50)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? LAST_MPERSON { get; set; }
            [Column(TypeName = "DATE")]
            public DateTime? LAST_MTIME { get; set; }
            [StringLength(500)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? REMARK { get; set; }
            [StringLength(20)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? COLLECT_ID { get; set; }
            //[StringLength(20)]
            //[Unicode(false)]
            //[SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            //public string? QUEUE_NUMBER { get; set; }
            [StringLength(20)]

            [SugarColumn(SqlParameterDbType = System.Data.DbType.AnsiString)]
            public string? REQ_USE { get; set; }
            [Column(TypeName = "DATE")]
            public DateTime? CHARGE_TIME { get; set; }
            /// <summary>
            /// 信息来源
            /// </summary>
            public string? INFO_SOURCE { get; set; }
        }
    }
}
