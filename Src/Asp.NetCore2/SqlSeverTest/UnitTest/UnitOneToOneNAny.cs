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
                .Where(it => it.EmpLicense.EmpInformation.EmpDepartmentJobs.Any())
                .ToList();

            var sql = db.Queryable<Order>()
               .Where(b =>
                       $"{b.Name}^{b.Name}^{b.Name}^{b.Id}^{1}  ".Contains("a")).ToSqlString();
            if (!sql.Contains("'[Name]^[Name]^[Name]^[Id]^ 1 ' like")) 
            {
                throw new Exception("unit error");
            }
        }
    }
}
