using MongoDB.Bson;
using SqlSugar.MongoDb;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace MongoDbTest 
{
    internal class UnitDateOnly
    {
        public static void Init()
        {
            var db = DbHelper.GetNewDb();
            db.DbMaintenance.TruncateTable<DateOnlyModel, DateOnlyModel2>();
            var dt = DateOnly.FromDateTime(Convert.ToDateTime("2022-01-01"));
            var dt2 = DateOnly.FromDateTime(Convert.ToDateTime("2022-11-01"));
            db.Insertable(new DateOnlyModel()
            {
                DateOnly=dt

            }).ExecuteCommand();
            db.Insertable(new DateOnlyModel()
            {
                DateOnly = DateOnly.FromDateTime(Convert.ToDateTime("2022-12-01"))

            }).ExecuteCommand();
            var list = db.Queryable<DateOnlyModel>().ToList();

            var list2 = db.Queryable<DateOnlyModel>().Where(it=>it.DateOnly==dt).ToList();
            var list3 = db.Queryable<DateOnlyModel>().Where(it => it.DateOnly == dt2).ToList();
            if (list2.Count != 1 || list3.Count !=0 || list.Count != 2) Cases.ThrowUnitError();

            db.Insertable(new DateOnlyModel2() { DateOnly=dt }).ExecuteCommand();
            var list4=db.Queryable<DateOnlyModel2>().ToList();
        }
    } 

    public class DateOnlyModel : MongoDbBase 
    {
         public DateOnly DateOnly { get; set; } 
    }
    public class DateOnlyModel2 : MongoDbBase
    {
        public DateOnly DateOnly { get; set; }

        [SugarColumn(IsJson = true)]
        public WeatherDataInfo[] DataInfo { get; set; } = new WeatherDataInfo[24];
    }

    public class WeatherDataInfo
    {
        /// <summary>
        /// // 时间 
        /// </summary>
        public string Time { get; set; }

        /// <summary>
        /// // 天气状况 
        /// </summary>
        [Description("天气状况")]
        public string WeatherCondition { get; set; }

        /// <summary>
        /// // 温度 
        /// </summary>
        [Description("温度")]
        public double Temperature { get; set; }

        /// <summary>
        /// // 降雨几率 
        /// </summary>
        [Description("降雨几率")]
        public double Precipitation { get; set; }

        /// <summary>
        /// 风速
        /// </summary>
        [Description("风速")]
        public int Wind { get; set; }

        /// <summary>
        /// 风向
        /// </summary>
        [Description("风向")]
        public string WindName { get; set; }

        /// <summary>
        /// 体感温度
        /// </summary>
        [Description("体感温度")]
        public int FeelsLike { get; set; }

        /// <summary>
        /// 湿度
        /// </summary>
        [Description("湿度")]
        public double Humidity { get; set; }

        /// <summary>
        /// 紫外线指数
        /// </summary>
        [Description("紫外线指数")]
        public int UVIndex { get; set; }

        /// <summary>
        /// 云量
        /// </summary>
        [Description("云量")]
        public double CloudCover { get; set; }

        /// <summary>
        /// 降雨量
        /// </summary>
        [Description("降雨量")]
        public double Rainfall { get; set; }
    }
}
