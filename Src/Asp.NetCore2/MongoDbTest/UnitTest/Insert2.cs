using SqlSugar.MongoDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MongoDbTest 
{
    public class Insert2
    {
        internal static void Init()
        {
            var db = DbHelper.GetNewDb();
            db.CodeFirst.InitTables<Student>();
            db.DbMaintenance.TruncateTable<Student>();
             
            //Get primary key
            var data = new Student() { Age = 1, Name = "11", SchoolId = "111" };
            db.Insertable(data).ExecuteCommandIdentityIntoEntity();
            var data2=db.Queryable<Student>().ToList(); 

        }
        [SqlSugar.SugarTable("UnitStudentadsfe2s3z1")]
        public class Student : MongoDbBase
        {
            public string Name { get; set; }

            public string SchoolId { get; set; }

            public int Age { get; set; }
            [SqlSugar.SugarColumn(InsertServerTime =true)]
            public DateTime CreateDateTime { get; set; }
        }
    }
}
