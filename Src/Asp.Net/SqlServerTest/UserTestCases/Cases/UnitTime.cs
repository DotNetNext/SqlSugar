using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest 
{
    public class UnitTime
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;
            var xx=db.Queryable<Test>().Where(it => (it.a == DateTime.Now && it.a == DateTime.Now) || (it.a == DateTime.Now && it.a == DateTime.Now)).ToSql();
            foreach (var item in xx.Value)
            {
                Console.WriteLine(item.DbType);
            }
        }
        public class Test 
        {
            [SqlSugar.SugarColumn(SqlParameterDbType =System.Data.DbType.Date)]
            public DateTime a { get; set; }
        }
    }
}
