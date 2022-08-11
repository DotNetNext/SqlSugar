using System;

using System.Collections.Generic;

using System.Linq;

using System.Reflection;

using SqlSugar;

namespace OrmTest

{

   public  class UnitManyToMay1231

    {

        public static void Init()

        {

            var db = new SqlSugarScope(new SqlSugar.ConnectionConfig()

            {

                ConnectionString = Config.ConnectionString,

                DbType = DbType.SqlServer,

                IsAutoCloseConnection = true

            });

 

            //用例代码 

            var result = db.Queryable<StudentA__1>()

                .Where(student => student.School.Rooms.Any())

                .ToSqlString();//用例代码

            if (result != "SELECT [StudentId],[SchoolNo] FROM [StudentA__1] student  WHERE  EXISTS( ( SELECT 1 FROM [SchoolA__1] SchoolA__10 Inner JOIN [SchoolAndRoomA__1] SchoolAndRoomA__1_1 ON  SchoolAndRoomA__1_1.[SchoolId]=SchoolA__10.[SchoolId]  Inner JOIN [RoomA__1] RoomA__11 ON  RoomA__11.[RoomId]=SchoolAndRoomA__1_1.[RoomId]   WHERE  student.[SchoolNo] = SchoolA__10.[SchoolNo]   ) ) ")
            {
                throw new Exception("unit error");
            }
 

        }

        //建类

        public class StudentA__1

        {

            [SugarColumn(IsPrimaryKey = true)]

            public int StudentId { get; set; }

            public int SchoolNo { get; set; }

            [Navigate(NavigateType.OneToOne, nameof(SchoolNo), nameof(SchoolA__1.SchoolNo))]

            public SchoolA__1 School { get; set; }



        }

        public class SchoolA__1

        {

            [SugarColumn(IsPrimaryKey = true)]

            public int SchoolId { get; set; }

            public int SchoolNo { get; set; }

            public int RoomId { get; set; }

            [Navigate(typeof(SchoolAndRoomA__1), nameof(SchoolAndRoomA__1.SchoolId), nameof(SchoolAndRoomA__1.RoomId))]

            public List<RoomA__1> Rooms { get; set; }

        }

        public class SchoolAndRoomA__1

        {

            [SugarColumn(IsPrimaryKey = true)]

            public int SchoolId { get; set; }

            [SugarColumn(IsPrimaryKey = true)]

            public int RoomId { get; set; }

        }

        public class RoomA__1

        {

            [SugarColumn(IsPrimaryKey = true)]

            public int RoomId { get; set; }

        }

    }

}