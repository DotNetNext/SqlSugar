using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest 
{
    public class UnitFilterasdfas
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<UnitOrderA01, UnitOrderB02>();
            db.QueryFilter.AddTableFilter<IDeleted>(it => it.IsDeleted == false);
            var list = db.Queryable<UnitOrderA01>()
                .LeftJoin<UnitOrderB02>((x, t) => x.ID == t.ID)
                .LeftJoin<UnitOrderB02>((x, t,t2) => x.ID == t2.ID)
                .ToList();

            var list2 = db.Queryable<UnitOrderA01, UnitOrderB02, UnitOrderB02>((x, t,t2) =>
                 new SqlSugar.JoinQueryInfos(SqlSugar.JoinType.Left, x.ID == t.ID, SqlSugar.JoinType.Left, x.ID == t2.ID)
                )
                .ToList();
              
        }
        public interface IDeleted
        {
            bool IsDeleted { get; set; }
        }
        public class UnitOrderA01 : IDeleted
        {
            public int ID { get; set; }
            public bool IsDeleted { get; set; }
        }
        public class UnitOrderB02 : IDeleted
        {
            public int ID { get; set; }
            public bool IsDeleted { get; set; }
        }
    }
}
