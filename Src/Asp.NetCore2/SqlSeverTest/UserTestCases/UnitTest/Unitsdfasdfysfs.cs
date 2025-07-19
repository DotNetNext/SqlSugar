using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSeverTest 
{
    using OrmTest;
    using SqlSugar;
    using System;
    using System.Collections.Generic;

    public class DummyModel
    {
        public string FieldA { get; set; }
        public string FieldB { get; set; }
        public string FieldC { get; set; }
        public string FieldD { get; set; }
    }

    public class SourceEntity
    {
        public string FieldA { get; set; }
        public string FieldB { get; set; }
        public string FieldC { get; set; }
        public string FieldD { get; set; }
    }

   public  class Unitdasfyasdfa
    {
       public static void Init()
        {
            var db = NewUnitTest.Db;
            int groupLevel = 2;
            db.CodeFirst.InitTables<SourceEntity>();
            var queryAble = db.Queryable<SourceEntity>();

            var result = queryAble
                .GroupBy((a) => new
                {
                    a.FieldA,
                    a.FieldB,
                    a.FieldC,
                    FieldD = SqlFunc.IIF(groupLevel > 1, a.FieldD, "NA")
                })
                .Select(a => new DummyModel
                {
                    FieldA = a.FieldA,
                    FieldB = a.FieldB,
                    FieldC = a.FieldC,
                    FieldD = SqlFunc.IIF(groupLevel > 1, a.FieldD, "NA")
                })
                .ToList();
        }
    }

}
