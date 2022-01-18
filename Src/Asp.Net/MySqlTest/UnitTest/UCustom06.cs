using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest 
{
    public class UCustom06
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;

            db.CodeFirst.InitTables<Unit06>();
            db.Insertable(new Unit06() { Company = "1", Name = "2", Work = "3" }).ExecuteCommand();
            var list = db.Queryable<Unit06>().Select(a => new UnitPeople
            {
                Name = a.Name,
                Job = new UnitJobClass { Company = a.Company, Work = a.Work }
            }
            ).ToList();
            Check.Exception(list.First().Job.Company != "1", "unit error");

        }
        public class Unit06 
        {
            public string Name { get; set; }
            public string Company { get; set; }
            public string Work { get; set; }
        }
        public class UnitPeople
        {
            public string Name { get; set; }
            public UnitJobClass Job { get; set; }
        }

        public class UnitJobClass
        {
            public string Company { get; set; }
            public string Work { get; set; }
        }
    }
}
