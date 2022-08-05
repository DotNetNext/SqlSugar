using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
namespace OrmTest 
{
    public  class UnitSub
    {
        public static void Init() 
        {
            var db = NewUnitTest.Db;
            var sql=db.Queryable<Order>()
                .Select(it => new
                {
                     id= SqlFunc.Subqueryable<SubEntity>().Sum(o => o.Id)
                }).ToSql();
            if (!sql.Key.Contains("id_1")) 
            {
                throw new Exception("unit error");
            }
        }
        public class SubEntity 
        {
            [SqlSugar.SugarColumn(ColumnName ="id_1")]
            public decimal Id { get; set; }    
        }
    }
}
