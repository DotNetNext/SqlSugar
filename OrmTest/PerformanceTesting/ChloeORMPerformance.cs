using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chloe;
using Chloe.SqlServer;
using OrmTest.Models;

namespace OrmTest.PerformanceTesting
{
    public class ChloeORMPerformance: PerformanceBase
    {
        public ChloeORMPerformance(int eachCount)
        {
            this.count = eachCount;
        }
        public void Select()
        {
            MsSqlContext db = new MsSqlContext(Config.ConnectionString);
            db.Query<Student>().Select(it => new ViewModelStudent2 { Name = it.Name, Student = it }).ToList();
            base.Execute("chloe", () =>
            {
                var test = db.Query<Student>().Select(it => new ViewModelStudent2 { Name = it.Name, Student = it }).ToList();
            });
            db.Dispose();
        }
    }
}
