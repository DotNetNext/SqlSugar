using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public partial class NewUnitTest
    {
        public static void Queryable2()
        {
           var list4 = Db.Queryable<ABMapping>()
                           .Mapper(it => it.A, it => it.AId)
                           .Mapper(it => it.B, it => it.BId)
                           .Where(it=>it.A.Name=="A")
                           .Where(it=>it.B.Id==1)
                           .Select(it=>new {
                               id=it.B.Id
                           })
                           .ToList();

        }
    }
}
