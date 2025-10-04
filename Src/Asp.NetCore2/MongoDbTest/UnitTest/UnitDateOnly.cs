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
    internal class UnitDateOnly
    {
        public static void Init()
        {
            var db = DbHelper.GetNewDb();
            db.DbMaintenance.TruncateTable<DateOnlyModel>();
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
        }
    } 

    public class DateOnlyModel : MongoDbBase 
    {
         public DateOnly DateOnly { get; set; } 
    }
}
