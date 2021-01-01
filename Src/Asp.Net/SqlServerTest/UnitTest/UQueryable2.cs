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
                            .Where(it => it.A.Name == "a")
                            .ToList();


            var list5 = Db.Queryable<ABMapping>()
                   .Mapper(it => it.A, it => it.AId, it => it.A.Id)
                   .Where(it => it.A.Name == "a")
                   .ToList();

        }
    }
}
