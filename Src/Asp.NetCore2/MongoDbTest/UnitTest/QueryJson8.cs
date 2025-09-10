using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using SqlSugar.MongoDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
using System.ComponentModel;
using MongoDb.Ado.data;
using MongoDB.Driver;

namespace MongoDbTest
{
    public class QueryJson8
    {
        internal static void Init()
        {
            var db = DbHelper.GetNewDb();
            db.CodeFirst.InitTables<Pub_ElectCurve>();
            db.DbMaintenance.TruncateTable<Pub_ElectCurve>(); 
            var val = new Pub_ElectCurve()
            {
                ElectDate = "2025",
                ElectName = "测试",
                ElectDayInfo = new List<Pub_ElectDayInfo>()
                {
                    new()
                    {
                        ElectDayDate = "工作日",
                        ElectDayVals = new double[] { 0, 1, 2, 3, 4, 5 }
                    }
                },

                ElectYearCurve = new Pub_ElectYearInfo[2]
                {
                    new Pub_ElectYearInfo()
                    {
                        ElectType = 0, ElectVal = 1.0
                    },
                    new Pub_ElectYearInfo()
                    {
                        ElectType = 0, ElectVal = 1.0

                    }
                }
            };
            var id = db.Insertable<Pub_ElectCurve>(
                val).ExecuteReturnPkList<string>().First();
            var val2 = new Pub_ElectCurve()
            {
                ElectDate = "2026",
                ElectName = "测试2",
                ElectDayInfo = new List<Pub_ElectDayInfo>()
                {
                    new()
                    {
                        ElectDayDate = "工作日",
                        ElectDayVals = new double[] { 0, 1, 2, 3, 4, 5 }
                    }
                },

                ElectYearCurve = new Pub_ElectYearInfo[2]
        {
                    new Pub_ElectYearInfo()
                    {
                        ElectType = 0, ElectVal = 1.0
                    },
                    new Pub_ElectYearInfo()
                    {
                        ElectType = 0, ElectVal = 1.0

                    }
        }
            };
            var id2 = db.Insertable<Pub_ElectCurve>(
                val2).ExecuteReturnPkList<string>().First();

            A aa = new A()
            {
                b = new()
                {
                    new Pub_ElectMonDInfos()
                    {
                        ElectMonDate = 1,
                        ElectMonVals = new double[] { 1, 2, 3, 4, 5 },
                        ElectEntId = id
                    }
                }
            }; 
            var yyy=db.Queryable<Pub_ElectCurve>().Where(it => aa.b.Any(x => x.ElectEntId == it.Id)).ToList();
            if (yyy.Count != 1 && yyy.First().ElectDate != "2025") Cases.ThrowUnitError();
           }
        }


        /// <summary>
        /// 电量曲线设置
        /// </summary>
        [SugarTable("pub_electcurve_setting")]
        public class Pub_ElectCurve : MongoDbBase
        {
            /// <summary>
            /// 年份
            /// </summary>
            public string ElectDate { get; set; }

            /// <summary>
            /// 曲线名称
            /// </summary>
            public string ElectName { get; set; }

            /// <summary>
            /// 年分月曲线
            /// </summary>
            [SugarColumn(IsJson = true)]
            public Pub_ElectYearInfo[] ElectYearCurve { get; set; } = new Pub_ElectYearInfo[2];

            /// <summary>
            /// 月分日曲线-按日期类型权重
            /// </summary>
            [SugarColumn(IsJson = true)]
            public List<Pub_ElectMonDInfo> ElectMonDInfo { get; set; } = new();

            /// <summary>
            /// 日分时曲线-按日期类型权重
            /// </summary>
            [SugarColumn(IsJson = true)]
            public List<Pub_ElectDayInfo> ElectDayInfo { get; set; } = new();

            /// <summary>
            /// 月分日曲线-按日期权重
            /// </summary>
            [SugarColumn(IsJson = true)]
            public List<Pub_ElectMonInfo> ElectMonInfo { get; set; } = new();

            /// <summary>
            /// 日分时曲线-按日期权重
            /// </summary>
            [SugarColumn(IsJson = true)]
            public List<Pub_ElectHouInfo> ElectHouInfo { get; set; } = new();
        }

        /// <summary>
        /// 电量曲线设置 - 年分月曲线
        /// </summary>
        public class Pub_ElectYearInfo
        {
            /// <summary>
            /// 类型 0:按日期类型权重 1:按日期权重
            /// </summary>
            public int ElectType { get; set; }

            /// <summary>
            /// 权重
            /// </summary>
            public double ElectVal { get; set; }
        }

        /// <summary>
        /// 电量曲线设置 - 月分日曲线-按日期类型权重
        /// </summary>
        public class Pub_ElectMonDInfo
        {
            /// <summary>
            /// 月份
            /// </summary>
            public int ElectMonDate { get; set; }

            /// <summary>
            /// 权重值:1-31
            /// </summary>
            [SugarColumn(IsJson = true)]
            public double[] ElectMonVals { get; set; }

            [BsonRepresentation(BsonType.ObjectId)]
            [SugarColumn(ColumnDataType = nameof(ObjectId))]
            public string ElectEntId { get; set; }
        }

        /// <summary>
        /// 电量曲线设置 - 日分时曲线-按日期类型权重
        /// </summary>
        public class Pub_ElectDayInfo
        {
            /// <summary>
            /// 日期类型
            /// </summary>
            public string ElectDayDate { get; set; }

            /// <summary>
            /// 权重值:0-23
            /// </summary>
            [SugarColumn(IsJson = true)]
            public double[] ElectDayVals { get; set; } = new double[24];
        }

        /// <summary>
        /// 电量曲线设置 - 月分日曲线-按日期权重
        /// </summary>
        public class Pub_ElectMonInfo
        {
            /// <summary>
            /// 月份
            /// </summary>
            public int ElectMonDate { get; set; }

            /// <summary>
            /// 工作日权重
            /// </summary>
            public double ElectWeekDay { get; set; }

            /// <summary>
            /// 周六权重
            /// </summary>
            public double ElectSaturday { get; set; }

            /// <summary>
            /// 周日权重
            /// </summary>
            public double ElectSunday { get; set; }

            /// <summary>
            /// 法定节假日权重
            /// </summary>
            public double ElectStaHolidays { get; set; }

            /// <summary>
            /// 调休节假日权重
            /// </summary>
            public double ElectPaidHolidays { get; set; }
        }

        /// <summary>
        /// 电量曲线设置 - 日分时曲线-按日期权重
        /// </summary>
        public class Pub_ElectHouInfo
        {
            /// <summary>
            /// 日期类型
            /// </summary>
            public PubDayType ElectHouType { get; set; }

            /// <summary>
            /// 日期类型名称
            /// </summary>
            public string ElectHouTypeName { get { return this.ElectHouType.ToString(); } set { } }

            /// <summary>
            /// 权重值:0-23
            /// </summary>
            [SugarColumn(IsJson = true)]
            public double[] ElectHouVals { get; set; } = new double[24];
        }

        /// <summary>
        /// 日类型
        /// </summary>
        public enum PubDayType
        {
            /// <summary>
            /// 工作日
            /// </summary>
            [Description("工作日")]
            WeekDay,

            /// <summary>
            /// 周六
            /// </summary>
            [Description("周六")]
            Saturday,

            /// <summary>
            /// 周日
            /// </summary>
            [Description("周日")]
            Sunday,

            /// <summary>
            /// 法定节假日
            /// </summary>
            [Description("法定节假日")]
            StaHolidays,

            /// <summary>
            /// 调休节假日
            /// </summary>
            [Description("调休节假日")]
            PaidHolidays
        }

        /// <summary>
        /// 电量曲线设置 - 月分日曲线-按日期类型权重
        /// </summary>
        public class Pub_ElectMonDInfos
        {
            /// <summary>
            /// 月份
            /// </summary>
            public int ElectMonDate { get; set; }

            /// <summary>
            /// 权重值:1-31
            /// </summary>
            [SugarColumn(IsJson = true)]
            public double[] ElectMonVals { get; set; }

            [BsonRepresentation(BsonType.ObjectId)]
            [SugarColumn(ColumnDataType = nameof(ObjectId))]
            public string ElectEntId { get; set; }
        }

        public class A
        {
            public List<Pub_ElectMonDInfos> b { get; set; }
    }

}
