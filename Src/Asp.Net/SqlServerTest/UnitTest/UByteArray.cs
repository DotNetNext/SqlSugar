using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest 
{
    public class UByteArray
    {
        public static void Init() 
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<UnitBytePk>();
            db.DbMaintenance.TruncateTable<UnitBytePk>();
            db.Insertable(new UnitBytePk()
            {
                 id=new byte[] { 1,2,3},
                 name="a"
            }).ExecuteCommand();
            db.Insertable(new UnitBytePk()
            {
                id = new byte[] { 2, 2, 3 },
                name = "a2"
            }).ExecuteCommand();
            var x=db.Storageable(new UnitBytePk()
            {
                id =new byte []{ 1, 2, 3 },
                name = "aaa"
            }).ToStorage();
            var count=x.AsUpdateable.ExecuteCommand();
            SqlSugar.Check.Exception(count == 3, "unit error");

        }
        public class UnitBytePk 
        {
            [SqlSugar.SugarColumn(IsPrimaryKey =true,ColumnDataType ="binary(20)")]
            public byte[] id { get; set; }
            public string name { get; set; }
        }
    }
}
