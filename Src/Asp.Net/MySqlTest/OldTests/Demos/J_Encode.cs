using OrmTest.Demo;
using OrmTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest.Demo
{
    public class Encode : DemoBase
    {
        public static void Init()
        {
            var db = GetInstance();
            db.Utilities.RemoveCacheAll();
           

            var id=db.Insertable(new List<StudentEncode>() {
                new StudentEncode(){Name="✨✨✨👉🏻2"  },
                new StudentEncode(){ Name="✨✨✨👉🏻" }
            } ).ExecuteReturnIdentity();

            var entity = db.Queryable<StudentEncode>().OrderBy(it=>it.Id,SqlSugar.OrderByType.Desc).ToList();
            entity.First().Name = "✨update✨✨👉🏻";
            db.Updateable(entity.First()).ExecuteCommand();
            entity = db.Queryable<StudentEncode>().OrderBy(it => it.Id, SqlSugar.OrderByType.Desc).ToList();
  
        }
    }
    [SqlSugar.SugarTable("student")]
    public class StudentEncode {
        public int Id { get; set; }
        [SqlSugar.SugarColumn(IsTranscoding =true)]
        public string Name { get; set; }
    }
}
