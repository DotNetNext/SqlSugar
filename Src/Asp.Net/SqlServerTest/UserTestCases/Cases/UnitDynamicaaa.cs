using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest 
{
    public class UnitDynamicaaa
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<UnitPerson011, UnitAddress011>();
            db.DbMaintenance.TruncateTable<UnitPerson011, UnitAddress011>();

            for (int i = 0; i < 200; i++)
            {
                AddData(db, "a"+i,"name"+i); 
            }

            var list = db.Queryable<UnitAddress011>()
                .Includes(x =>
                  x.Persons.MappingField(y => y.AddressId, () => x.Id).ToList()//可以多字段匹配 MappingField(..).MappingField(..).Tolist()
                ).ToList();
            if (list[150].Persons.Count() != 2) 
            {
                throw new Exception("unit error");
            }
        }

        private static void AddData(SqlSugarClient db,string street,string name)
        {
            var address = new UnitAddress011
            {
                Street = street
            };
            int addressId = db.Insertable(address).ExecuteReturnIdentity();

            for (int i = 0; i < 2; i++)
            {
                // 创建 UnitPerson011 对象并插入记录
                var person = new UnitPerson011
                {
                    Name = name,
                    AddressId = addressId
                };
                int personId = db.Insertable(person).ExecuteReturnIdentity(); 
            }
        }
    }
    [SugarTable("UnitPerson011xxx")]
    public class UnitPerson011
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public string Name { get; set; }
        public int AddressId { get; set; }
        [SqlSugar.Navigate(SqlSugar.NavigateType.OneToOne, nameof(AddressId))]
        public UnitAddress011 Address { get; set; }
    }
    [SugarTable("UnitAddress011sss")]
    public class UnitAddress011
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public string Street { get; set; }
        [SqlSugar.Navigate(SqlSugar.NavigateType.Dynamic, null)]
        public List<UnitPerson011> Persons { get; set; }
    }
}
