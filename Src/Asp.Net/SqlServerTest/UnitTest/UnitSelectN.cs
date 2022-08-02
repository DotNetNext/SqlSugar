using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest 
{
    internal class UnitSelectN
    {
        public static void Init()
        {
            var db =  NewUnitTest.Db;
            db.CodeFirst.InitTables<A, AA, B, BB>();
            db.DbMaintenance.TruncateTable<A>();
            db.DbMaintenance.TruncateTable<AA>();
            db.DbMaintenance.TruncateTable<B>();
            db.DbMaintenance.TruncateTable<BB>();
            db.Insertable(new B() { BId = 1, AId = 1, BName = "a" }).ExecuteCommand();
            var data=db.Queryable<B>()
                .LeftJoin<A>((b, a) => b.AId == a.AId)
                .LeftJoin<AA>((b, a, aa) => a.AId == aa.AId)
                .LeftJoin<BB>((b, a, aa, bb) => bb.BId == b.BId)
                .Select((b, a, aa, bb) => new
                {
                    B =new B() {  BId=b.BId, BName=b.BName, AId=a.AId}
                })
                .ToList();
            if (data.First().B.BId != 1 && data.First().B.BName != "a") 
            {
                throw new Exception("unit error");
            }
            var data2 = db.Queryable<B>()
            .LeftJoin<A>((b, a) => b.AId == a.AId)
            .LeftJoin<AA>((b, a, aa) => a.AId == aa.AId)
            .LeftJoin<BB>((b, a, aa, bb) => bb.BId == b.BId)
            .Select((b, a, aa, bb) => new
            {
                B = new  { BId = b.BId, BName = b.BName, AId = a.AId }
            })
            .ToList();
            if (data2.First().B.BId != 1 && data2.First().B.BName != "a")
            {
                throw new Exception("unit error");
            }
            var data3 = db.Queryable<B>()
                .LeftJoin<A>((b, a) => b.AId == a.AId)
                .LeftJoin<AA>((b, a, aa) => a.AId == aa.AId)
                .LeftJoin<BB>((b, a, aa, bb) => bb.BId == b.BId)
                .Select((b, a, aa, bb) => new
                {
                    b=b.BId,
                    A=new ADTO() { AId=a.AId },
                    B = new  BDTO{ BId = b.BId, BName = b.BName, AId = a.AId }
                })
                .ToList();
            if (data3.First().B.BId != 1 && data3.First().B.BName != "a"&& data3.First().b!=1)
            {
                throw new Exception("unit error");
            }

        }
        [SqlSugar.SugarTable("unita1")]
        public class A
        {
            public int AId { get; set; }
            public string AName { get; set; }
        }
        [SqlSugar.SugarTable("unitb1")]
        public class B
        {
            public int AId { get; set; }
            public int BId { get; set; }
            public string BName { get; set; }
        }
        [SqlSugar.SugarTable("unitaa1")]
        public class AA
        {
            public int AId { get; set; }
            public int AAId { get; set; }
            public string AAName { get; set; }
        }
        [SqlSugar.SugarTable("unitbb1")]
        public class BB
        {
            public int BId { get; set; }
            public int BBId { get; set; }
            public string BBName { get; set; }
        }

        public class ADTO
        {
            public int AId { get; set; }
            public int AAId { get; set; }
            public string AName { get; set; }
            public string AAName { get; set; }
        }

        public class BDTO
        {
            public int AId { get; set; }
            public string AName { get; set; }
            public int AAId { get; set; }
            public string AAName { get; set; }
            public int BId { get; set; }
            public int BBId { get; set; }
            public string BName { get; set; }
            public string BBName { get; set; }
        }
    }
}
