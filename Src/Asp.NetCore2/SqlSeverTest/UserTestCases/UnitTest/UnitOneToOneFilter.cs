using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    internal class UnitOneToOneFilter
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<UnitPerson011dd1231dd3, UnitAddre222s0xx11>();
            db.DbMaintenance.TruncateTable<UnitPerson011dd1231dd3, UnitAddre222s0xx11>();

            var address = new UnitAddre222s0xx11
            {
                Street = "123 Main Street"
            };
            int addressId = db.Insertable(address).ExecuteReturnIdentity();

            // 创建 UnitPerson011 对象并插入记录
            var person = new UnitPerson011dd1231dd3
            {
                Name = "John Doe",
                AddressId = addressId
            };
            int personId = db.Insertable(person).ExecuteReturnIdentity();

            db.QueryFilter.AddTableFilter<DelId>(x => x.IsDel==true);
            var list = db.Queryable<UnitPerson011dd1231dd3>().Includes(x => x.Address)
                .OrderBy(it=>it.Address.Id).ToList();

        }

        public class UnitPerson011dd1231dd3: DelId
        {
            [SqlSugar.SugarColumn(IsPrimaryKey = true )]
            public int Id { get; set; }
      
            public string Name { get; set; }
            public int AddressId { get; set; }
            [SqlSugar.Navigate(SqlSugar.NavigateType.OneToOne, nameof(AddressId))]
            public UnitAddre222s0xx11 Address { get; set; }
            public bool IsDel { get; set; }
        }

        public class UnitAddre222s0xx11: DelId
        {
            [SqlSugar.SugarColumn(IsPrimaryKey = true )]
            public int Id { get; set; }
            [SqlSugar.SugarColumn(IsPrimaryKey = true)]
            public int Id2 { get; set; }
            public string Street { get; set; }
            public bool IsDel { get; set; }
        }
        public interface DelId 
        {
            public bool IsDel { get; set; }
        }
    }
}
