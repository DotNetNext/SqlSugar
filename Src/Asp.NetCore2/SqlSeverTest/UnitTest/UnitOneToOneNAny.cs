using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrmTest
{
    internal class UnitOneToOneNAny
    {
        public static void Init() 
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<EmpLicenseLogOff, EmpInformation, EmpDepartmentJob,EmpLicense>();
            db.Queryable<EmpLicenseLogOff>()
                .Where(it => it.EmpLicense.EmpInformation.EmpDepartmentJobs.Any(z => z.DepId == 1))
                .ToList();
        }
    }
}
