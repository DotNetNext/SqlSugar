using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    internal class UnitBool
    {
        public int num { get; set; }
        public static void Init()
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<UnitADFA>();
            var p = new UnitBool();
            db.Updateable<UnitADFA>()
                .SetColumns(it => new UnitADFA()
                {
                    a = p.num > 1
                })
                .Where(it=>it.id==1)
                .ExecuteCommand();

            db.Updateable<UnitADFA>()
                  .SetColumns(it => new UnitADFA()
                  {
                      a = it.id > 1
                  })
                  .Where(it => it.id == 1)
                  .ExecuteCommand();
        }
        public class UnitADFA
        {
            public int id { get; set; }
            public bool a { get; set; }
        }
    }
}
