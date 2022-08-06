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
           db.CodeFirst.InitTables<SubEntity>();
           db.Queryable<Order>()
              .Select(it => new
              {
                  id = SqlFunc.Subqueryable<SubEntity>()
                  .OrderBy(o => o.Id)
                   .OrderByDesc(o => o.name)
                         .Select(o=>o.Id)
              }).ToList();
            db.Queryable<Order>()
              .Select(it => new
              {
                  id = SqlFunc.Subqueryable<SubEntity>()
                  .OrderByDesc(o => o.Id)
                   .OrderByDesc(o => o.name)
                         .Select(o => o.Id)
              }).ToList();
            db.Queryable<Order>()
              .Select(it => new
              {
                  id = SqlFunc.Subqueryable<SubEntity>()
                  .OrderBy(o => o.name)
                   .OrderBy(o => o.Id)
                         .Select(o => o.Id)
              }).ToList();
            db.Queryable<Order>()
            .Select(it => new
            {
                id = SqlFunc.Subqueryable<SubEntity>()
                .OrderBy(o => o.Id) 
                .Select(o => o.Id)
            }).ToList();
            db.Queryable<Order>()
            .Select(it => new
            {
                id = SqlFunc.Subqueryable<SubEntity>()
                .OrderByDesc(o => o.Id)
                .Select(o => o.Id)
            }).ToList();
            db.DbMaintenance.DropTable("UnitSubEntity");
        }
        [SugarTable("UnitSubEntity")]
        public class SubEntity 
        {
            [SqlSugar.SugarColumn(ColumnName ="id_1")]
            public decimal Id { get; set; }  
            public string name { get; set; }
        }
    }
}
