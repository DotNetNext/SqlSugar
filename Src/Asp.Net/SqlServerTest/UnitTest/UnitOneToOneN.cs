using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
namespace OrmTest
{
    public class UnitOneToOneN
    {
        public static void Init() 
        {
            var db = NewUnitTest.Db;


            //建表 

            if (!db.DbMaintenance.IsAnyTable("StudentA", false))

            {

                db.CodeFirst.InitTables<StudentA>();

            }

            if (!db.DbMaintenance.IsAnyTable("SchoolA", false))

            {

                db.CodeFirst.InitTables<SchoolA>();

            }

            if (!db.DbMaintenance.IsAnyTable("RoomA", false))

            {

                db.CodeFirst.InitTables<RoomA>();

            }



            //用例代码 

            var result = db.Queryable<StudentA>()

                .Select(student => student.School.Room.RoomId)

                .ToSqlString();//用例代码

            if (!result.Contains("student.[SchoolNo] = SchoolA0.[SchoolNo]")) 
            {
                throw new Exception("unit error");
            }
        }

        public class StudentA

        {

            [SugarColumn(IsPrimaryKey = true)]

            public int StudentId { get; set; }

            public int SchoolNo { get; set; }

            [Navigate(NavigateType.OneToOne, nameof(SchoolNo), nameof(SchoolA.SchoolNo))]

            public SchoolA School { get; set; }



        }

        public class SchoolA

        {

            [SugarColumn(IsPrimaryKey = true)]

            public int SchoolId { get; set; }

            public int SchoolNo { get; set; }

            public int RoomId { get; set; }

            [Navigate(NavigateType.OneToOne, nameof(RoomId))]

            public RoomA Room { get; set; }

        }

        public class RoomA

        {

            [SugarColumn(IsPrimaryKey = true)]

            public int RoomId { get; set; }

        }
    }
}
