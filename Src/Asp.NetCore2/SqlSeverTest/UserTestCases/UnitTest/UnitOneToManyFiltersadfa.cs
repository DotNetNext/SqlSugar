using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public class UnitOneToManyFiltersadfa
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<UnitPerson011, UnitAddress011>();
            db.DbMaintenance.TruncateTable<UnitPerson011, UnitAddress011>();

            var address = new UnitAddress011
            {
                Street = "222"
            };
            int addressId = db.Insertable(address).ExecuteReturnIdentity();

            // 创建 UnitPerson011 对象并插入记录
            var person = new UnitPerson011
            {
                Name = "John Doe",
                AddressId = addressId
            };
            int personId = db.Insertable(person).ExecuteReturnIdentity();

            db.QueryFilter.AddTableFilter<IAddressId>(x => x.AddressId ==1);

            var list = db.Queryable<UnitAddress011>().Includes(x => x.Persons).ToList();
            db.CurrentConnectionConfig.MoreSettings = new SqlSugar.ConnMoreSettings
            {
                 IsAutoDeleteQueryFilter=true
            };
             db.UpdateNav(list).Include(x=>x.Persons).ExecuteCommand(); 
        }
        [SqlSugar.SugarTable("UnitPerson011xxx11")]
        public class UnitPerson011: IAddressId
        {
            [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int Id { get; set; }
            public string Name { get; set; }
            public int AddressId { get; set; }
        }
        [SqlSugar.SugarTable("UnitPerson011yyy1")]
        public class UnitAddress011
        {
            [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int Id { get; set; }
            public string Street { get; set; }
            [SqlSugar.Navigate(SqlSugar.NavigateType.OneToMany, nameof(UnitPerson011.AddressId))]
            public List<UnitPerson011> Persons { get; set; }
        }
        public interface IAddressId
        {
            public int AddressId { get; set; }
        }
    }
}
