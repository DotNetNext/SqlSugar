using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest 
{
    public class UOneManyMany4
    {

        public static void init()
        {

            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Student_004, School_004, Room_004, Desk_004, Book_004>();
            db.DbMaintenance.TruncateTable<Student_004, School_004, Room_004, Desk_004, Book_004>();

            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();
            var id3 = Guid.NewGuid();
            var id4 = Guid.NewGuid();
            db.Insertable(new Student_004() { sid = id1, Name = "北大jack", SchoolId = id1 }).ExecuteCommand();
            db.Insertable(new Student_004() { sid = id2, Name = "青华jack", SchoolId = id2 }).ExecuteCommand();

            db.Insertable(new School_004() { scid = id1, schname = "北大" }).ExecuteCommand();
            db.Insertable(new School_004() { scid = id2, schname = "青华" }).ExecuteCommand();
            db.Insertable(new Book_004() { StudentId = id1,  Name = "数学001" }).ExecuteCommand();
            db.Insertable(new Book_004() { StudentId = id2,  Name = "语文002" }).ExecuteCommand();


            db.Insertable(new Room_004() { roomId = id1, schoolId = id1, roomName = "北大01室" }).ExecuteCommand();
            db.Insertable(new Room_004() { roomId = id2, schoolId = id1, roomName = "北大02室" }).ExecuteCommand();
            db.Insertable(new Room_004() { roomId = id3, schoolId = id2, roomName = "青华03室" }).ExecuteCommand();
            db.Insertable(new Room_004() { roomId = id4, schoolId = id2, roomName = "青华04室" }).ExecuteCommand();

            db.Insertable(new Desk_004() { roomId = id1, deskid = id1, deskName = "北大01室_01" }).ExecuteCommand();
            db.Insertable(new Desk_004() { roomId = id2, deskid = id2, deskName = "北大02室_01" }).ExecuteCommand();
            db.Insertable(new Desk_004() { roomId = id3, deskid = id3, deskName = "青华03室_01" }).ExecuteCommand();
            db.Insertable(new Desk_004() { roomId = id4, deskid = id4, deskName = "青华04室_01" }).ExecuteCommand();


            var list = db.Queryable<Student_004>()
                         .Includes(x => x.school_001, x => x.rooms, x => x.desk)
                         .Includes(x=>x.books)
                .Where(x => x.school_001.rooms.Any(z => z.desk.Any())).ToList();

            if (list.Count() != 2)
            {
                throw new Exception("unit error");
            }

            var list2 = db.Queryable<Student_004>()
                         .Includes(x => x.school_001, x => x.rooms)
            .Where(x => x.school_001.rooms.Any(z =>
            z.roomName == "北大01室" &&
            z.desk.Any())).ToList();


            if (list2.Count() != 1)
            {
                throw new Exception("unit error");
            }

            var list3 = db.Queryable<Student_004>()
                .Includes(x => x.school_001, x => x.rooms)
         .Where(x => x.school_001.rooms.Any(z =>
         z.roomName == "青华03室" &&
         z.desk.Any(c => c.deskName == "青华03室_01"))).ToList();

            if (list3.Count != 1)
            {
                throw new Exception("unit error");
            }

            var list4 = db.Queryable<Student_004>()
        .Where(x => x.school_001.rooms.Any(z =>
        z.roomName == "青华03室" &&
        z.desk.Any(c => c.deskName == "青华04室_01"))).ToList();


            if (list4.Count != 0)
            {
                throw new Exception("unit error");
            }
            db.DbMaintenance.TruncateTable<Student_004, School_004, Room_004, Desk_004,Book_004>();
            foreach (var item in list) 
            {
                item.sid = Guid.Empty;
                item.SchoolId = Guid.Empty;
                item.school_001.scid = Guid.Empty;
                foreach (var r in item.school_001.rooms)
                {
                    r.roomId = Guid.Empty;
                    r.schoolId = Guid.Empty;
                    foreach (var z in r.desk)
                    {
                        z.deskid = Guid.Empty;
                        z.roomId = Guid.Empty;
                        
                    }
                }
                foreach (var z in item.books) 
                {
                    z.bookid = Guid.Empty;
                    z.StudentId = Guid.Empty;
                }
            }
            db.InsertNav(list.First())

                .Include(x => x.school_001)
                .ThenInclude(x => x.rooms)
                .ThenInclude(x => x.desk)

                .Include(x=>x.books).ExecuteCommand();

            db.InsertNav(list.Last())

              .Include(x => x.school_001)
              .ThenInclude(x => x.rooms)
              .ThenInclude(x => x.desk)

              .Include(x=>x.books).ExecuteCommand();

            if (db.Queryable<Desk_004>().Count() != 4 || db.Queryable<Room_004>().Count() != 4
                || db.Queryable<School_004>().Count() != 2 || db.Queryable<Student_004>().Count() != 2) 
            {
                throw new Exception("unit error");
            }

            db.DbMaintenance.TruncateTable<Student_004, School_004, Room_004, Desk_004,Book_004>();

            db.InsertNav(list.First().school_001)
             .Include(x => x.rooms)
             .ThenInclude(x => x.desk).ExecuteCommand();
            db.InsertNav(list.Last().school_001)
              .Include(x => x.rooms)
              .ThenInclude(x => x.desk).ExecuteCommand(); 
            if (db.Queryable<Desk_004>().Count() != 4 || db.Queryable<Room_004>().Count() != 4
               || db.Queryable<School_004>().Count() != 2 || db.Queryable<Student_004>().Count() != 0)
            {
                throw new Exception("unit error");
            }
        }

        public class Student_004 
        {
            [SqlSugar.SugarColumn(IsPrimaryKey =true)]
            public Guid sid { get; set; } 
            public string Name { get; set; }

            public Guid SchoolId { get; set; }
            [SqlSugar.Navigate(SqlSugar.NavigateType.OneToOne,nameof(SchoolId))]
            public School_004 school_001 { get; set; }
            [SqlSugar.Navigate(SqlSugar.NavigateType.OneToMany, nameof(Book_004.StudentId))]
            public List<Book_004> books { get; set; }

        }


        public class Book_004 
        {
            [SqlSugar.SugarColumn(IsPrimaryKey = true)]
            public Guid bookid { get; set; }
            public string Name { get; set; }
            public Guid StudentId { get; set; }
        }

        public class School_004
        {
            [SqlSugar.SugarColumn(IsPrimaryKey = true )]
            public Guid scid { get; set; }
            public string schname { get; set; }
            [SqlSugar.Navigate(SqlSugar.NavigateType.OneToMany, nameof(Room_004.schoolId))]
            public List<Room_004> rooms { get; set; }
        }

        public class Room_004 
        {
            [SqlSugar.SugarColumn(IsPrimaryKey = true )]
            public Guid roomId { get; set; }
            public Guid schoolId { get; set; }
            public string roomName { get; set; }
            [SqlSugar.Navigate(SqlSugar.NavigateType.OneToMany, nameof(Desk_004.roomId))]
            public List<Desk_004> desk { get; set; }
        }

        public class Desk_004 
        {
            [SqlSugar.SugarColumn(IsPrimaryKey = true)]
            public Guid deskid { get; set; }
            public string deskName { get; set; }
            public Guid roomId { get; set; }
        }
    }
}
