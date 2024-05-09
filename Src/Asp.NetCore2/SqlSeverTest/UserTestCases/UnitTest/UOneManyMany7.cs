using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace OrmTest
{
    public class UOneManyMany7
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
            db.Insertable(new Student_004() { sid = Guid.NewGuid(), StudentId = id1, Name = "北大jack", SchoolId = id1 }).ExecuteCommand();
            db.Insertable(new Student_004() { sid = Guid.NewGuid(), StudentId = id2, Name = "青华jack", SchoolId = id2 }).ExecuteCommand();

            var scid = Guid.NewGuid();
            var scid2 = Guid.NewGuid();
            db.Insertable(new School_004() { scid =scid, scid2 = id1, schname = "北大" }).ExecuteCommand();
            db.Insertable(new School_004() { scid = scid2, scid2 = id2, schname = "青华" }).ExecuteCommand();
            db.Insertable(new Book_004() { StudentId = id1, Name = "数学001" }).ExecuteCommand();
            db.Insertable(new Book_004() { StudentId = id2, Name = "语文002" }).ExecuteCommand();


            db.Insertable(new Room_004() { roomId = id1, schoolId = scid2, roomName = "北大01室" }).ExecuteCommand();
            db.Insertable(new Room_004() { roomId = id2, schoolId = scid2, roomName = "北大02室" }).ExecuteCommand();
            db.Insertable(new Room_004() { roomId = id3, schoolId = scid, roomName = "青华03室" }).ExecuteCommand();
            db.Insertable(new Room_004() { roomId = id4, schoolId = scid, roomName = "青华04室" }).ExecuteCommand();

            db.Insertable(new Desk_004() { roomId = id1, deskid = id1, deskName = "北大01室_01" }).ExecuteCommand();
            db.Insertable(new Desk_004() { roomId = id2, deskid = id2, deskName = "北大02室_01" }).ExecuteCommand();
            db.Insertable(new Desk_004() { roomId = id3, deskid = id3, deskName = "青华03室_01" }).ExecuteCommand();
            db.Insertable(new Desk_004() { roomId = id4, deskid = id4, deskName = "青华04室_01" }).ExecuteCommand();


            var list = db.Queryable<Student_004>()
                         .Includes(x => x.school_001, x => x.rooms)
                         .Includes(x => x.books)
                .ToList();


            var list2 = db.Queryable<Student_004>()
                         .Includes(x => x.school_001, x => x.rooms)
                         .Includes(x => x.books)
                         .Select(x => new Student_004DTO
                         {
                             SchoolId = x.SchoolId,
                             books = x.books,
                             school_001 = x.school_001
                         }, true)
                .ToList();

            if (!list2.First().school_001.rooms.Any()) { throw new Exception("unit error"); }

            var list3 = db.Queryable<Student_004>()
             .Includes(x => x.school_001, x => x.rooms)
               .Includes(x => x.books)
                .Select(x => new
                {
                    SchoolId = x.SchoolId,
                    x2 = x.books ,
                    x = x.school_001,
                    school_001 = x.school_001
                })
            .ToList();

            if (!list3.First().school_001.rooms.Any()) { throw new Exception("unit error"); }

            var list4 = db.Queryable<Student_004>()
              .Includes(x => x.school_001, x => x.rooms)
              .Includes(x => x.books) 
              .LeftJoin<Student_004>((x,y)=>x.StudentId==y.StudentId)
              .Select((x) => new
              {
                  SchoolId = x.SchoolId,
                  x2 = x.books,
                  x = x.school_001,
                  school_001 = x.school_001
              })
              .ToList();

            if (!list4.First().school_001.rooms.Any()) { throw new Exception("unit error"); }

            var list5= db.Queryable<Student_004>()
           .Includes(x => x.school_001, x => x.rooms)
           .Includes(x => x.books)
           .LeftJoin<Order>((x, y) => true)
           .Select((x,y) => new Student_004DTO
           {
               SchoolId = x.SchoolId,
               books = x.books,
               school_001 = x.school_001,
               Name=y.Name
           })
           .ToList();



            var list6 = db.Queryable<Student_004>()
           .Includes(x => x.school_001, x => x.rooms)
           .Includes(x => x.books)
           .LeftJoin<Order>((x, y) => true)
           .Select((x, y) => new 
           {
               SchoolId = x.SchoolId,
               books = x.books.Select(it=>new Order() {  Name=it.Name } ).ToList(),
               bookFirst = x.books.Select(it => new Order() { Name = it.Name }  ).FirstOrDefault(),
               school_001 = x.school_001,
               Name = y.Name
           })
           .ToList();

        }


        public class Student_004DTO
        {

            public Guid sid { get; set; }
            public Guid StudentId { get; set; }
            public string Name { get; set; }

            public Guid SchoolId { get; set; }

            public School_004 school_001 { get; set; }

            public List<Book_004> books { get; set; }

        }

        [SugarTable("Student_005")]
        public class Student_004
        {
            [SqlSugar.SugarColumn(IsPrimaryKey = true)]
            public Guid sid { get; set; }
            public Guid StudentId { get; set; }
            public string Name { get; set; }

            public Guid SchoolId { get; set; }
            [SqlSugar.Navigate(SqlSugar.NavigateType.OneToOne, nameof(SchoolId), nameof(School_004.scid2))]
            public School_004 school_001 { get; set; }
            [SqlSugar.Navigate(SqlSugar.NavigateType.OneToMany, nameof(Book_004.StudentId), nameof(StudentId))]
            public List<Book_004> books { get; set; }

        }

        [SugarTable("Book_005")]
        public class Book_004
        {
            [SqlSugar.SugarColumn(IsPrimaryKey = true)]
            public Guid bookid { get; set; }
            public string Name { get; set; }
            public Guid StudentId { get; set; }
        }

        [SugarTable("School_005")]
        public class School_004
        {
            [SqlSugar.SugarColumn(IsPrimaryKey = true)]
            public Guid scid { get; set; }
            public Guid scid2 { get; set; }
            public string schname { get; set; }
            [SqlSugar.Navigate(SqlSugar.NavigateType.OneToMany, nameof(Room_004.schoolId))]
            public List<Room_004> rooms { get; set; }
        }

        [SugarTable("Room_005")]
        public class Room_004
        {
            [SqlSugar.SugarColumn(IsPrimaryKey = true)]
            public Guid roomId { get; set; }
            public Guid schoolId { get; set; }
            public string roomName { get; set; }
            [SqlSugar.Navigate(SqlSugar.NavigateType.OneToMany, nameof(Desk_004.roomId))]
            public List<Desk_004> desk { get; set; }
        }
        [SugarTable("Desk_005")]
        public class Desk_004
        {
            [SqlSugar.SugarColumn(IsPrimaryKey = true)]
            public Guid deskid { get; set; }
            public string deskName { get; set; }
            public Guid roomId { get; set; }
        }
    }
}
