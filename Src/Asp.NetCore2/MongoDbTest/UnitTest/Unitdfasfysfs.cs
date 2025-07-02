using MongoDB.Bson;
using SqlSugar.MongoDb;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDbTest 
{
    internal class Unitdfasfysfs
    {
        public static void Init() 
        {
            var db = DbHelper.GetNewDb();
            db.Insertable(new VehicleTracksEO()
            {
                carId= ObjectId.GenerateNewId() + "",
                orderId =ObjectId.GenerateNewId()+"",
                latlngPoints=new List<Point>() { 
                 new Point(){ lat=1, lng=1, timestamp=11, vehicleNo="aa" }
                }

            }).ExecuteCommand();
            var list=db.Queryable<VehicleTracksEO>().ToList();
        }
    }
    [SugarTable("unitadsfasfdsys")]

    public class VehicleTracksEO : MongoDbBase

    {

        //外键需要设置ObjectId类型不然存储会的是string

        [SugarColumn(ColumnDataType = nameof(ObjectId))]

        public string orderId { get; set; } = string.Empty;



        [SugarColumn(ColumnDataType = nameof(ObjectId))]

        public string carId { get; set; } = string.Empty;



        [SugarColumn(IsJson = true)]

        public List<Point> latlngPoints { get; set; } =new List<Point>() { };

    }



    public class Point

    {

        public string? vehicleNo { get; set; } = string.Empty;



        public double lat { get; set; }



        public double lng { get; set; }



        public long timestamp { get; set; }

    }
}
