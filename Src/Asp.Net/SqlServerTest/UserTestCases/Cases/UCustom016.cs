using OrmTest.UnitTest.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public class UCustom016
    {

        public static void Init()
        {
            var db = NewUnitTest.Db;

 

            db.CodeFirst.InitTables<A1, B1, ABMapping1>();
            db.DbMaintenance.TruncateTable<A1>();
            db.DbMaintenance.TruncateTable<B1>();
            db.DbMaintenance.TruncateTable<ABMapping1>();
            db.Insertable(new A1() { Id = Guid.NewGuid(), Name = "a1" }).ExecuteCommand();
           
            db.Insertable(new B1() { Id = Guid.NewGuid(), Name = "b1" }).ExecuteCommand();
            db.Insertable(new B1() { Id = Guid.NewGuid(), Name = "b2" }).ExecuteCommand();
            db.Insertable(new ABMapping1() { AId = Guid.NewGuid(), BId = Guid.NewGuid() }).ExecuteCommand();
            db.Queryable<A1>().Includes(X => X.BList).ToList();
        }
        [SugarTable("ABMapping1Guid")]
        public class ABMapping1
        {
            [SugarColumn(IsPrimaryKey = true )]
            public Guid AId { get; set; }
            [SugarColumn(IsPrimaryKey = true)]
            public Guid BId { get; set; }
        }
        [SugarTable("A1Guid")]
        public class A1
        {
            [SugarColumn(IsPrimaryKey = true  )]
            public Guid Id { get; set; }
            public string Name { get; set; }
            [Navigate(typeof(ABMapping1),nameof(ABMapping1.AId),nameof(ABMapping1.BId))]
            public List<B1> BList { get; set; }
        }
        [SugarTable("B1Guid")]
        public class B1
        {
            [SugarColumn(IsPrimaryKey = true )]
            public Guid Id { get; set; }
            public string Name { get; set; }
            [Navigate(typeof(ABMapping1), nameof(ABMapping1.BId), nameof(ABMapping1.AId))]
            public List<A1> AList { get; set; }
        }
 

    }
}
