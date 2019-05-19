using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
namespace OrmTest
{
    public partial class NewUnitTest
    {
        public static void QueryableAsync()
        {
            RefAsync<int> total = 0;
            Task t=Db.Queryable<Order>().ToPageListAsync(1, 2, total);
            t.Wait();
        }
    }
}
