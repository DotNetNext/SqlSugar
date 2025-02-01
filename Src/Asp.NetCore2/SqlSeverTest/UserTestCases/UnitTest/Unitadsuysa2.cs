using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    internal class Unitdfaatsd2
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;
            db.CurrentConnectionConfig.ConfigureExternalServices = new ConfigureExternalServices()
            {
                EntityService = (type, entity) =>
                {
                    if (entity.PropertyInfo.PropertyType == typeof(MyValueObject3)) 
                    {
                        entity.IsOwnsOne = true;
                    }
                }
            };
            var cols=db.EntityMaintenance.GetEntityInfo<UnitMyEntity3>();
            db.CodeFirst.InitTables<UnitMyEntity3>();
        }
        [SugarTable("unityyyzzz")]
        public class UnitMyEntity3
        {
            public Guid Id { get; init; }
             
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
