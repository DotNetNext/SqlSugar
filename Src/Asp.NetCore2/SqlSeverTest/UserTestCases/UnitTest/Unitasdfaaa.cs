using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    internal class Unitasxdfaaa
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<UnitMyEntity3>();
            UnitMyEntity3 entity = new()
            {
                Id = Guid.NewGuid(),
                ValueObject = new MyValueObject3
                {
                    VOId = Guid.NewGuid(),
                    Name = "Name"
                }
            };

             db.Insertable(entity).ExecuteCommand();
             db.Updateable(entity).Where(it=>it.Id==Guid.NewGuid()).ExecuteCommand();
        }

        public class UnitMyEntity3
        {
            public Guid Id { get; init; }

            [SugarColumn(IsOwnsOne = true)]
            public MyValueObject3 ValueObject { get; set; }
        }

        public class MyValueObject3
        {
            [SugarColumn(ColumnName = "VALUEOBJECTID")]
            public Guid VOId { get; set; }

            [SugarColumn(ColumnName = "VALUEOBJECTNAME")]
            public string Name { get; set; }
        }
    }
}
