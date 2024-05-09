using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Linq;

namespace OrmTest
{
    internal class UnitCustom020
    {

        public static void Init() 
        {
            var db = NewUnitTest.Db;
            //建表 
            if (!db.DbMaintenance.IsAnyTable("Kc_ygzlmx", false))
            {
                db.CodeFirst.InitTables<Kc_ygzlmx>();
            }

            //用例代码 
            //var result = db.Insertable(new TM.Kcgl.Kc_ygzlmx() { Pkid = "EE2C5971978248498AE378DD86518096", Gzlbh = "B198E22CA25842DB81D1500637DDF147", Jsbh = "JS00C49ACC45C244AD9E582A9C144936", Zc = 1, Jxcs = 1, Jxss = 2 }).ExecuteCommand();//用例代码
            //       result = db.Insertable(new TM.Kcgl.Kc_ygzlmx() { Pkid = "EE2C5971978248498AE378DD86518096", Gzlbh = "B198E22CA25842DB81D1500637DDF147", Jsbh = "JS00C49ACC45C244AD9E582A9C144936", Zc = 2, Jxcs = 2, Jxss = 4 }).ExecuteCommand();//用例代码

            // 修改表
            Kc_ygzlmx mx = new  Kc_ygzlmx();
            mx.Gzlbh = "B198E22CA25842DB81D1500637DDF147";
            mx.Zc = 1;
            mx.Jxcs = 10;
            mx.Jxss = 10;
            db.Updateable(mx)
                .UpdateColumns(new string[] { "Jxcs", "Jxss" })
  .WhereColumns(new string[] { "Gzlbh", "Zc" })
            .ExecuteCommand();
        }

        /// <summary>
        /// 月工作量明细Kc_ygzlmx
        /// </summary>
        public class Kc_ygzlmx
        {
            #region 构造函数

            public Kc_ygzlmx()
            {
            }

            #endregion

            #region 属性

            /// <summary>
            /// 序号
            /// </summary>
            [StringLength(32)]
            [Display(Name = "序号")]
            [Required(ErrorMessage = "序号是必填项")]
            [SugarColumn(IsPrimaryKey = true)]
            public string Pkid { get; set; }

            /// <summary>
            /// 工作量编号
            /// </summary>
            [StringLength(32)]
            [Display(Name = "工作量编号")]
            [Required(ErrorMessage = "工作量编号是必填项")]
            public string Gzlbh { get; set; }

            /// <summary>
            /// 教师编号
            /// </summary>
            [StringLength(32)]
            [Display(Name = "教师编号")]
            [Required(ErrorMessage = "教师编号是必填项")]
            public string Jsbh { get; set; }

            /// <summary>
            /// 周次
            /// </summary>
            [Display(Name = "周次")]
            [Required(ErrorMessage = "周次是必填项")]
            public int Zc { get; set; } = 0;

            /// <summary>
            /// 本周教学次数
            /// </summary>
            [Display(Name = "本周教学次数")]
            [Required(ErrorMessage = "本周教学次数是必填项")]
            public int Jxcs { get; set; } = 0;

            /// <summary>
            /// 本周教学时数
            /// </summary>
            [Display(Name = "本周教学时数")]
            [Required(ErrorMessage = "本周教学时数是必填项")]
            public int Jxss { get; set; } = 0;

            #endregion
        }
    }
}
