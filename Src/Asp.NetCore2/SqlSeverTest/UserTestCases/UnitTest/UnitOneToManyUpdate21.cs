using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrmTest
{
    public class UnitOneToMany1231123
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<UnitPerson011, UnitAddress011>();
            db.DbMaintenance.TruncateTable<UnitPerson011, UnitAddress011>();

            var address = new UnitAddress011
            {
                Street = "123 Main Street"
            };
            int addressId = db.Insertable(address).ExecuteReturnIdentity();

            // 创建 UnitPerson011 对象并插入记录
            var person = new UnitPerson011
            {
                Name = "John Doe",
                AddressId = addressId
            };
            int personId = db.Insertable(person).ExecuteReturnIdentity();

            var list = db.Queryable<UnitAddress011>().Includes(x => x.Persons).ToList();

            var list2=db.Queryable<UnitAddress011>().Includes(x=>x.Persons).First();
            list2.Persons.First().Name = "Update";
            list2.Persons.Add(new UnitPerson011() { 
             Name="jack",
              AddressId=addressId
            });
            db.UpdateNav(list2)
                .Include(x => x.Persons,new SqlSugar.UpdateNavOptions()
                { 
                     OneToManyInsertOrUpdate = true,
                }).ExecuteCommand();

            var list3 = db.Queryable<UnitAddress011>().Includes(x => x.Persons).First();
            if (list3.Persons.Count() != 2 || list3.Persons.First().Name != "Update" || list3.Persons.Last().Name != "jack") 
            {
                throw new Exception("unit error");
            }

            list3.Persons.Remove(list3.Persons.Last());
            list3.Persons.First().Name = "Update2";
            db.UpdateNav(list3)
                .Include(x => x.Persons, new SqlSugar.UpdateNavOptions()
                {
                    OneToManyInsertOrUpdate = true,
                }).ExecuteCommand();

           var list4 = db.Queryable<UnitAddress011>().Includes(x => x.Persons).First();
;            if (list4.Persons.Count() != 1 || list4.Persons.First().Name != "Update2" )
            {
                throw new Exception("unit error");
            }

            db.UpdateNav(list3)
                        .Include(x => x.Persons, new SqlSugar.UpdateNavOptions()
                        {
                            OneToManyInsertOrUpdate = true,//启用：子表插入或更新模式
                            IgnoreColumns =new string[] { "AddressId" }
                        }).ExecuteCommand();

        }
        [SqlSugar.SugarTable("UnitPerson0112s22a2")]
        public class UnitPerson011
        {
            [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int Id { get; set; }
            public string Name { get; set; }
            public int AddressId { get; set; }
        }
        [SqlSugar.SugarTable("UnitAddress0110112s22a2")]
        public class UnitAddress011
        {
            [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int Id { get; set; }
            public string Street { get; set; }
            [SqlSugar.Navigate(SqlSugar.NavigateType.OneToMany, nameof(UnitPerson011.AddressId))]
            public List<UnitPerson011> Persons { get; set; }
        }
    }

}
