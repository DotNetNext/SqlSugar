using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    internal class UnitBytesEmpty
    {
        public static void Init() 
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Unitadfasfa2>();
            db.Insertable(new Unitadfasfa2() { bytes = new byte[] { } }).ExecuteCommand();
            var xxx=db.Queryable<Unitadfasfa2>().ToList();
            if (xxx.First().bytes.Length != 0) 
            {
                throw new Exception("unit error");
            }
        }
        public class Unitadfasfa2 
        {
            public byte[] bytes { get; set; }
        }
    }
}
