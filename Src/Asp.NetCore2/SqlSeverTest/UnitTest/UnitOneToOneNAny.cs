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
           db.Queryable<Order>()
             .Where(b =>
                     $"{b.Name}^{b.Name}^{b.Name}^{b.Id}^{1}  ".Contains("a")).ToList();
            var sql = db.Queryable<Order>()
               .Where(b =>
                       $"{b.Name}^{b.Name}^{b.Name}^{b.Id}^{1}  ".Contains("a")).ToSqlString();
            if (!sql.Contains("(''+ CAST([Name] AS NVARCHAR(MAX))+'^'+ CAST([Name] AS NVARCHAR(MAX))+'^'+ CAST([Name] AS NVARCHAR(MAX))+'^'+ CAST([Id] AS NVARCHAR(MAX))+'^'+ CAST(' 1 ' AS NVARCHAR(MAX))+'' like")) 
            {
                throw new Exception("unit error");
            }
        }
    }
}
