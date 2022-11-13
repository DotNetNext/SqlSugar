using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest 
{
    public class UnitByteArray
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;
            var b=new byte[1024];
            b[0] = 1;
            b[1] = 2;
            b[1023] = 2;
            if (db.DbMaintenance.IsAnyTable("UnitBytes001231",false)) 
            {
                db.DbMaintenance.DropTable<UnitBytes001231>();
            }
            db.CodeFirst.InitTables<UnitBytes001231>();
            db.DbMaintenance.TruncateTable<UnitBytes001231>();
            db.Insertable(new UnitBytes001231() { array = b }).ExecuteCommand();
            var list4 = db.Queryable<UnitBytes001231>().First().array;
            var old = string.Join(",", b);
            var newstr = string.Join(",", list4);
            if (old != newstr) 
            {
                throw new Exception("unit error");
            }
            Console.WriteLine("#### CodeFirst end ####");
        }
    }
    public class UnitBytes001231
    {
        public byte[] array { get; set; }
    }
 
}
