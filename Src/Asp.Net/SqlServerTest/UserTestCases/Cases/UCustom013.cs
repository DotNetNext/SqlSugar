using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public class UCustom013
    {

        public static void Init()
        {
            var db = new SqlSugarScope(new SqlSugar.ConnectionConfig()
            {
                ConnectionString = Config.ConnectionString,
                DbType = DbType.SqlServer,
                IsAutoCloseConnection = true
            });
            db.CodeFirst.InitTables<UnitBoola1>();
            db.Insertable(new UnitBoola1() { a = true }).ExecuteCommand();
            db.Queryable<Order>()
                .Select(x => new
                {
                    x1 =(bool?) SqlFunc.Subqueryable<UnitBoola1>().Select(it => it.a)
                }).ToList();

        }
        public class UnitBoola1 
        {
            public bool a { get; set; }
        }
       
    }
}
