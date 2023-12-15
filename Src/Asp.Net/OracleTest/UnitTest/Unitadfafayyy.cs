using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    internal class Unitadfafayyy
    {
        public static void Init() 
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Unitadfzadsfa>();
            db.Insertable(new Unitadfzadsfa() { time = TimeSpan.FromDays(1) }).ExecuteCommand();
            var list=db.Queryable<Unitadfzadsfa>().ToList();
        }
        public class Unitadfzadsfa 
        {
            [SqlSugar.SugarColumn(ColumnDataType = "interval day to second")]
            public TimeSpan time { get; set; }
        }
    }
}
