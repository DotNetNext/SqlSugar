using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrmTest
{
    public class OneToManyInString
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<UnitPerson011, UnitAddress011>();
            db.DbMaintenance.TruncateTable<UnitPerson011, UnitAddress011>();

            var address = new UnitAddress011
            {
                Id= "我是,中国,人",
                Street = "123 Main Street"
            };
            int addressId = db.Insertable(address).ExecuteReturnIdentity();
            var address2 = new UnitAddress011
            {
                Street = "123 Main Street",
                Id= "我不,是猫"
            };
            int addressId2 = db.Insertable(address2).ExecuteReturnIdentity();
            // 创建 UnitPerson011 对象并插入记录
            var person = new UnitPerson011
            {
                Id="a",
                Name = "John Doe",
                AddressId = "我是,中国,人"
            };
            int personId = db.Insertable(person).ExecuteReturnIdentity();

            var list = db.Queryable<UnitAddress011>().Includes(x => x.Persons).ToList();
            if (list.Last().Persons.Count() == 0) 
            {
                throw new Exception("unit error");
            }
        }
        [SqlSugar.SugarTable("UnitPerson011asfaa2d")]
        public class UnitPerson011
        {
            [SqlSugar.SugarColumn(IsPrimaryKey = true )]
            public string Id { get; set; }
            public string Name { get; set; }
            public string AddressId { get; set; }
        }
        [SqlSugar.SugarTable("UnitAddress01111asfaa2d")]
        public class UnitAddress011
        {
            [SqlSugar.SugarColumn(IsPrimaryKey = true )]
            public string Id { get; set; }
            public string Street { get; set; }
            [SqlSugar.Navigate(SqlSugar.NavigateType.OneToMany, nameof(UnitPerson011.AddressId))]
            public List<UnitPerson011> Persons { get; set; }
        }
    }
}
