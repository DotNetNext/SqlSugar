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
        }
    }


}
