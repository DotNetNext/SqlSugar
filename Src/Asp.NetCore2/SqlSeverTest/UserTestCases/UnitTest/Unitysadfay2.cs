using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    internal class Unitysadfay2
    {
        public static void Init() 
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<ClassA, ClassB>();
            db.Updateable<ClassA>()
   .InnerJoin<ClassB>((m, e) => m.EmpId == e.EmpId)
   .Where((m,e)=>true)
   .SetColumns((m, e) => new ClassA()
   {
       EndDate = SqlFunc.IIF(e.CourseModel.Equals("DATE"), 
                                SqlFunc.ToDate(e.Deadline).Date,
                                SqlFunc.DateAdd(m.StartDate
                                    , e.Period ?? 0
                                    , DateType.Day).Date)

   })
   .ExecuteCommand();
        }
    }
    [SugarTable("UnitClasssdfy")]
    public class ClassA
    {
        public string EmpId { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime StartDate { get; set; }
    }

    [SugarTable("UnitClassBdfas2")]
    public class ClassB
    {
        public string EmpId { get; set; }
        public string CourseModel { get; set; }
        public DateTime? Deadline { get; set; }
        public int? Period { get; set; }
    }



   
}
