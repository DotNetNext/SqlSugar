using MySqlConnector.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    internal class UnitEnumTest
    {
        public static void Init() 
        {
            var db = NewUnitTest.Db;

            db.CodeFirst.InitTables<UnitadfadfaaEnum>();
            var s = new List<UnitadfadfaaEnum>() {   
                 new UnitadfadfaaEnum(){ Id=1, DbType=SqlSugar.DbType.Sqlite}
            };
            db.Queryable<UnitadfadfaaEnum>()
                .Where(it => s.Any(s => s.Id == it.Id && s.DbType == it.DbType))
                .ToList();
        }

        public class UnitadfadfaaEnum 
        {
            [SqlSugar.SugarColumn(IsPrimaryKey =true,IsIdentity =true)]
            public int Id { get; set; }
            public SqlSugar.DbType DbType { get; set; }
        }
    }
}
