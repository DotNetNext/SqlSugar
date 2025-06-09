using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{

    public class Unitsdfasyss
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;

            db.QueryableByObject(typeof(Order)).AS("Order").ToList();
            db.QueryableByObject(typeof(Order)).AS("\"public\".Order").ToList();

            db.CodeFirst.InitTables<Unitdfsas>();
            db.Insertable(new Unitdfsas() { GridX = new double[] { 1 } }).ExecuteCommand();
            db.Fastest<Unitdfsas>().BulkCopy(new List<Unitdfsas>(){ new Unitdfsas() { GridX = new double[] { 1 } } });
            var list=db.Queryable<Unitdfsas>().ToList();
        }
        public class Unitdfsas 
        {
            [SugarColumn(ColumnDataType = "double precision []", IsArray = true)]

            public double[] GridX { get; set; }
        }
    }


}
