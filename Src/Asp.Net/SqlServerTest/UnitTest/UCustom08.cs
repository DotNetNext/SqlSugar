using OrmTest.UnitTest.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public class UCustom08
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Unitasfa1>();
            db.CodeFirst.InitTables<Unitasfa1Item>();
         db.Queryable<Unitasfa1>().Where(new List<IConditionalModel>
            {
                new ConditionalModel(){
                 ConditionalType=ConditionalType.In,
                   FieldName="id",
                   FieldValue="null"
                }
            }).ToList();
            db.Insertable(new Unitasfa1() { Id = Guid.NewGuid() }).ExecuteCommand();
            db.Insertable(new Unitasfa1() { Id = Guid.NewGuid(),ItemId=Guid.NewGuid() }).ExecuteCommand();
            var list = db.Queryable<Unitasfa1>().Mapper(x => x.items, x => x.ItemId).ToList();
        }
        public class Unitasfa1
        {
            [SugarColumn(IsPrimaryKey = true)]
            public Guid Id { get; set; }
            [SugarColumn(IsNullable = true)]
            public Guid? ItemId { get; set; }

            [SugarColumn(IsIgnore = true)]
            public List<Unitasfa1Item> items { get; set; }
        }

        public class Unitasfa1Item
        {
            [SugarColumn(IsPrimaryKey = true)]
            public Guid Id { get; set; }
        }
    }
}
