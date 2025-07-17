using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDbTest 
{
    internal class Unitdafasdys
    {
        internal static void Init()
        {
            var db = DbHelper.GetNewDb();
            db.CodeFirst.InitTables<Unitadfasdfa, Unitadfasdfa2>();
            db.Insertable(new Unitadfasdfa() { Id = 1 }).ExecuteCommand();
            db.Insertable(new Unitadfasdfa2() { Id = 1 }).ExecuteCommand();
            var list=db.Queryable<Unitadfasdfa3>().ToList();
        }
        public class Unitadfasdfa 
        {
            public int Id { get; set; }
        }
        [SqlSugar.SugarTable("Unitadfasdfa")]
        public class Unitadfasdfa2
        {
            public double Id { get; set; }
        }
        [SqlSugar.SugarTable("Unitadfasdfa")]
        public class Unitadfasdfa3
        {
            public SqlSugar.DbType Id { get; set; }
        }

    }
}
