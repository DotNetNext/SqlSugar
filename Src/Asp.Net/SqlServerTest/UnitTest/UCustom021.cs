using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
namespace OrmTest 
{
    public class UCustom021
    {
        public static void Inti() {
            var db = NewUnitTest.Db;
            db.CodeFirst.SetStringDefaultLength(20).InitTables<UnitStudent111>();
            db.CodeFirst.SetStringDefaultLength(20).InitTables<UnitExam>();

            var id = db.Insertable(new UnitStudent111() { Name = "小a" }).ExecuteReturnIdentity();
            db.Insertable(new UnitExam
            {
                StudentId = id,
                Number = 1,
                Time = DateTime.Now
            }).ExecuteCommand();

            var result2 = db.Queryable<UnitStudent111>()
             .Includes(e => e.Exams.Where(s => s.Id >e.Id).ToList()).ToList();



        }
    }

    public class UnitStudent111
    {
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public int Id { get; set; }
        public string Name { get; set; }
        [Navigate(NavigateType.OneToMany, nameof(UnitExam.StudentId))]//BookA表中的studenId
        public List<UnitExam> Exams { get; set; }

    }
    public class UnitExam
    {
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int Number { get; set; }
        public DateTime Time { get; set; }

    }
}
