using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest 
{
    public class UOneManyMany
    {

        public static void init()
        {

            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Student_001, School_001, Room_001, Desk_001>();
            db.DbMaintenance.TruncateTable<Student_001, School_001, Room_001, Desk_001>();

            db.Insertable(new Student_001() { sid = 1, Name = "北大jack", SchoolId = 1 }).ExecuteCommand();
            db.Insertable(new Student_001() { sid = 2, Name = "青华jack", SchoolId = 2 }).ExecuteCommand();

            db.Insertable(new School_001() { scid = 1, schname = "北大" }).ExecuteCommand();
            db.Insertable(new School_001() { scid = 2, schname = "青华" }).ExecuteCommand();


            db.Insertable(new Room_001() { roomId = 1, schoolId = 1, roomName = "北大01室" }).ExecuteCommand();
            db.Insertable(new Room_001() { roomId = 2, schoolId = 1, roomName = "北大02室" }).ExecuteCommand();
            db.Insertable(new Room_001() { roomId = 3, schoolId = 2, roomName = "青华03室" }).ExecuteCommand();
            db.Insertable(new Room_001() { roomId = 4, schoolId = 2, roomName = "青华04室" }).ExecuteCommand();

            db.Insertable(new Desk_001() { roomId = 1, deskid = 1, deskName = "北大01室_01" }).ExecuteCommand();
            db.Insertable(new Desk_001() { roomId = 2, deskid = 2, deskName = "北大02室_01" }).ExecuteCommand();
            db.Insertable(new Desk_001() { roomId = 3, deskid = 3, deskName = "青华03室_01" }).ExecuteCommand();
            db.Insertable(new Desk_001() { roomId = 4, deskid = 4, deskName = "青华04室_01" }).ExecuteCommand();


            var list = db.Queryable<Student_001>()
                         .Includes(x => x.school_001, x => x.rooms, x => x.desk)
                .Where(x => x.school_001.rooms.Any(z => z.desk.Any())).ToList();

            if (list.Count() != 2)
            {
                throw new Exception("unit error");
            }

            var list2 = db.Queryable<Student_001>()
                         .Includes(x => x.school_001, x => x.rooms)
            .Where(x => x.school_001.rooms.Any(z =>
            z.roomName == "北大01室" &&
            z.desk.Any())).ToList();


            if (list2.Count() != 1)
            {
                throw new Exception("unit error");
            }

            var list3 = db.Queryable<Student_001>()
                .Includes(x => x.school_001, x => x.rooms)
         .Where(x => x.school_001.rooms.Any(z =>
         z.roomName == "青华03室" &&
         z.desk.Any(c => c.deskName == "青华03室_01"))).ToList();

            if (list3.Count != 1)
            {
                throw new Exception("unit error");
            }

            var list4 = db.Queryable<Student_001>()
        .Where(x => x.school_001.rooms.Any(z =>
        z.roomName == "青华03室" &&
        z.desk.Any(c => c.deskName == "青华04室_01"))).ToList();


            if (list4.Count != 0)
            {
                throw new Exception("unit error");
            }
            db.DbMaintenance.TruncateTable<Student_001, School_001, Room_001, Desk_001>();
            db.InsertNav(list.First())
                .Include(x => x.school_001)
                .ThenInclude(x => x.rooms)
                .ThenInclude(x => x.desk).ExecuteCommand();
            db.InsertNav(list.Last())
              .Include(x => x.school_001)
              .ThenInclude(x => x.rooms)
              .ThenInclude(x => x.desk).ExecuteCommand();

            if (db.Queryable<Desk_001>().Count() != 4 || db.Queryable<Room_001>().Count() != 4
                || db.Queryable<School_001>().Count() != 2 || db.Queryable<Student_001>().Count() != 2) 
            {
                throw new Exception("unit error");
            }
        }

        public class Student_001 
        {
            [SqlSugar.SugarColumn(IsPrimaryKey =true)]
            public int sid { get; set; } 
            public string Name { get; set; }

            public int SchoolId { get; set; }
            [SqlSugar.Navigate(SqlSugar.NavigateType.OneToOne,nameof(SchoolId))]
            public School_001 school_001 { get; set; }  

        }

        public class School_001
        {
            [SqlSugar.SugarColumn(IsPrimaryKey = true)]
            public int scid { get; set; }
            public string schname { get; set; }
            [SqlSugar.Navigate(SqlSugar.NavigateType.OneToMany, nameof(Room_001.schoolId))]
            public List<Room_001> rooms { get; set; }
        }

        public class Room_001 
        {
            [SqlSugar.SugarColumn(IsPrimaryKey = true)]
            public int roomId { get; set; }
            public int schoolId { get; set; }
            public string roomName { get; set; }
            [SqlSugar.Navigate(SqlSugar.NavigateType.OneToMany, nameof(Desk_001.roomId))]
            public List<Desk_001> desk { get; set; }
        }

        public class Desk_001 
        {
            [SqlSugar.SugarColumn(IsPrimaryKey = true)]
            public int deskid { get; set; }
            public string deskName { get; set; }
            public int roomId { get; set; }
        }
    }
}
