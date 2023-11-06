using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
namespace OrmTest
{
 
    public class UCustom016
    {
        public static void Init() 
        {
            var db = NewUnitTest.Db;
            var list0 = new TestEntity
            {
                Id = 1,
                Data1 = null,
                Data2 = null
            };
            var list1 = new List<TestEntity>
                {
                    new TestEntity
                    {
                        Id = 2,
                        Data1 = null,
                        Data2 = null
                    },
                };
            var list2 = new List<TestEntity>
            {
                new TestEntity
                {
                    Id = 3,
                    Data1 = null,
                    Data2 = null
                },
                new TestEntity
                {
                    Id = 4,
                    Data1 = null,
                    Data2 = null
                },
            };
            db.CodeFirst.InitTables<TestEntity>();
            db.DbMaintenance.TruncateTable<TestEntity>();
            db.Insertable(list0).ExecuteCommand();
            db.Insertable(list1).ExecuteCommand ();
            db.Insertable(list2).ExecuteCommand();
            var list3 = db.Queryable<TestEntity>().ToList();
            if (list3.First().Data1 != null) 
            {
                throw new Exception("unit error");
            }
        }
        public enum TestEnum
        {
            A = 1,
            B = 2,
            C = 3,
            D = 4,
        }
        [SugarTable("UnitNullenum1")]
        public class TestEntity
        {
            [SugarColumn(IsPrimaryKey = true)]
            public long Id { get; set; }
            [SugarColumn(IsNullable =true)]
            public TestEnum? Data1 { get; set; }
            [SugarColumn(IsNullable = true)]
            public int? Data2 { get; set; }
        }
    }
}
