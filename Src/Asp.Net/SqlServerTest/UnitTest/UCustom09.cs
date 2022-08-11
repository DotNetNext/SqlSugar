using OrmTest.UnitTest.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public class UCustom09
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;
            var list=db.Queryable<Order>()
               .GroupBy(x => new { CreateTime = x.CreateTime.ToString("yyyy-MM"),x.Name })
                .Select(x => new { CreateTime =  x.CreateTime.ToString("yyyy-MM") }).ToList();
            db.CodeFirst.InitTables<Unitadfaint21>();
            db.Insertable(new Unitadfaint21() { id = 1 , id2=22 }).ExecuteCommand();
            var list2 = db.Queryable<Unitadfaint21>().ToList();
            
            var list3=db.Queryable<Order>()
              // .Where(it=>SqlFunc.ToString(it.Id>0?1:0)=="")
               .Select(it => new
             {
                 count = SqlFunc.AggregateSum(it.Id < 20 ? 1 : 0)
                 //count  = SqlFunc.AggregateSum(SqlFunc.IIF(b.客户确认状态 < 20, 1, 0))
             }).ToList();
          
            db.Queryable<Tree>().LeftJoin<Tree>((t,y)=>t.Id==y.Id).Where(t=>1==1).Select(t=>t).ToParentList(it => it.ParentId, "0");
            db.Queryable<Order>() .Select((a) => new
                           {
                               ID = a.Id,
                               真实名称 = !string.IsNullOrEmpty(a.Name) ? a.Name : a.Name
                           }).ToList();
            db.Queryable<Order>().Select(x => new
            {
                // c=SqlFunc.Subqueryable<Order>().Where(z=>z.Id==x.Id).Count(),
                x1 = !string.IsNullOrEmpty(x.Name)
            }).ToList();
        }

        public class Unitadfaint21
        {
            [SugarColumn(ColumnDataType ="int")]
            public long? id { get; set; }
            [SugarColumn(ColumnDataType = "int")]
            public long id2 { get; set; }
        }
        public class Unitasfa1sadfa
        {
            [SugarColumn(IsPrimaryKey = true)]
            public Guid Id { get; set; }
            [SugarColumn(IsNullable = true)]
            public bool ItemId { get; set; }
 
        }

     
    }
}
