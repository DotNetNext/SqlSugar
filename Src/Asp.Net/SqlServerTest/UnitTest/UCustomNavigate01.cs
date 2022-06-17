
using System.Configuration;
using SqlSugar;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System;

namespace OrmTest
{

    public class UCustomNavigate01
    {


        public static void Init()
        {
            SqlSugarClient db = NewUnitTest.Db;
            //建表 
            if (!db.DbMaintenance.IsAnyTable("VIEW_EQ_MODEL_INFO_1", false))
            {
                db.CodeFirst.InitTables<ViewEqModel>();
          
            }
            db.DbMaintenance.TruncateTable<ViewEqModel>();
            //建表 
            if (!db.DbMaintenance.IsAnyTable("VIEW_EQ_INFO_1", false))
            {
                db.CodeFirst.InitTables<ViewEqInfo>();
         
            }
            db.DbMaintenance.TruncateTable<ViewEqInfo>();
            //建表 
            if (!db.DbMaintenance.IsAnyTable("EQ_EDC_ITEM_CONFIG_1", false))
            {
                db.CodeFirst.InitTables<EqEdcItemConfig>();
  
            }
            db.DbMaintenance.TruncateTable<EqEdcItemConfig>();
            //建表 
            if (!db.DbMaintenance.IsAnyTable("EQ_ALARM_CONFIG_1", false))
            {
                db.CodeFirst.InitTables<EqAlarmConfig>();

            }
            db.DbMaintenance.TruncateTable<EqAlarmConfig>();
            db.Insertable(new ViewEqInfo() { Iden = "eq01", ModelIden = "eqModel01" }).ExecuteCommand();
            db.Insertable(new ViewEqInfo() { Iden = "eq02", ModelIden = "eqModel01" }).ExecuteCommand();
            db.Insertable(new ViewEqModel() { ModelIden = "eqModel01" }).ExecuteCommand();
            db.Insertable(new EqEdcItemConfig() { Iden = "eqEdc01", ModelIden = "eqModel01" }).ExecuteCommand();
            db.Insertable(new EqEdcItemConfig() { Iden = "eqEdc02", ModelIden = "eqModel01" }).ExecuteCommand();
            db.Insertable(new EqAlarmConfig() { Iden = "eqAlarm01", ModelIden = "eqModel01" }).ExecuteCommand();
            db.Insertable(new EqAlarmConfig() { Iden = "eqAlarm02", ModelIden = "eqModel01" }).ExecuteCommand();

            var s1 = db.Queryable<ViewEqModel>()
                .Includes(a => a.EqEdcItemList)
                //.Includes(a => a.EqAlarmList)
                .ToList();

            var s2 = db.Queryable<ViewEqInfo>()
                .Includes(s => s.eqModel, a => a.EqEdcItemList)
             //   .Includes(s => s.eqModel, a => a.EqAlarmList)
                .ToList();

            if (s1.First().EqEdcItemList.Count() != s2.First().eqModel.EqEdcItemList.Count()) 
            {
                throw new Exception("unit error");
            }

        }

        [SugarTable("VIEW_EQ_MODEL_INFO_1")]
        public partial class ViewEqModel
        {
            public ViewEqModel()
            {
            }

            /// <summary>
            /// 
            /// </summary>           
            [SugarColumn(IsPrimaryKey = true, ColumnName = "MODEL_IDEN")]
            public string ModelIden { get; set; }


      
            [Navigate(NavigateType.OneToMany, nameof(EqEdcItemConfig.ModelIden))]
            public List<EqEdcItemConfig> EqEdcItemList { get; set; }


 
            [Navigate(NavigateType.OneToMany, nameof(EqAlarmConfig.ModelIden))]
            public List<EqAlarmConfig> EqAlarmList { get; set; }
        }

        ///<summary>
        ///
        ///</summary>
        [SugarTable("VIEW_EQ_INFO_1")]
        public partial class ViewEqInfo
        {
            public ViewEqInfo()
            {
            }
            /// <summary>
            /// 主键
            /// </summary>           
            [SugarColumn(IsPrimaryKey = true, ColumnName = "IDEN")]
            public string Iden { get; set; }


            /// <summary>
            /// 设备型号
            /// </summary>           
            [SugarColumn(ColumnName = "MODEL_IDEN")]
            public string ModelIden { get; set; }

            [SugarColumn(IsIgnore = true)]
            [Navigate(NavigateType.OneToOne, nameof(ModelIden))]
            public ViewEqModel eqModel { get; set; }
        }

        ///<summary>
        ///
        ///</summary>
        [SugarTable("EQ_EDC_ITEM_CONFIG_1")]
        public partial class EqEdcItemConfig
        {
            public EqEdcItemConfig()
            {
            }
            /// <summary>
            /// 主键
            /// </summary>           
            [SugarColumn(IsPrimaryKey = true, ColumnName = "IDEN")]
            public string Iden { get; set; }

            /// <summary>
            /// 设备型号
            /// </summary>           
            [SugarColumn(ColumnName = "MODEL_IDEN")]
            public string ModelIden { get; set; }


        }
        ///
        ///</summary>
        [SugarTable("EQ_ALARM_CONFIG_1")]
        public partial class EqAlarmConfig
        {
            public EqAlarmConfig()
            {
            }
            /// <summary>
            /// 主键
            /// </summary>           
            [SugarColumn(IsPrimaryKey = true, ColumnName = "IDEN")]
            public string Iden { get; set; }

            /// <summary>
            /// 设备型号
            /// </summary>           
            [SugarColumn(ColumnName = "MODEL_IDEN")]
            public string ModelIden { get; set; }

        }

    }

}
