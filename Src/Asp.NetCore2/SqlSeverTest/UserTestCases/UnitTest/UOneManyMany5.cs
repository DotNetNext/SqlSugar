using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest 
{
    public class UOneManyMany5
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
            db.Insertable(new Student_004() { sid=Guid.NewGuid(), StudentId = id1, Name = "北大jack", SchoolId = id1 }).ExecuteCommand();
            db.Insertable(new Student_004() { sid=Guid.NewGuid(), StudentId = id2, Name = "青华jack", SchoolId = id2 }).ExecuteCommand();

            db.Insertable(new School_004() {  scid=Guid.NewGuid(), scid2 = id1, schname = "北大" }).ExecuteCommand();
            db.Insertable(new School_004() { scid = Guid.NewGuid(),scid2 = id2, schname = "青华" }).ExecuteCommand();
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
                         .Includes(x => x.school_001 )
                         .Includes(x=>x.books)
                .ToList();

           
            db.DbMaintenance.TruncateTable<Student_004, School_004, Room_004, Desk_004,Book_004>();
            foreach (var item in list) 
            {
                item.sid = Guid.Empty;
                item.SchoolId = Guid.Empty;
                item.school_001.scid = Guid.Empty;
                foreach (var z in item.books) 
                {
                    z.bookid = Guid.Empty;
                    z.StudentId = Guid.Empty;
                }
            }
            db.InsertNav(list.First())
                .Include(x=>x.books)
                .Include(x=>x.school_001)
                .ExecuteCommand();

            db.InsertNav(list.Last())
              .Include(x=>x.books)
              .Include(x=>x.school_001)
              .ExecuteCommand();

            db.QueryFilter.AddTableFilter<Book_004>(x => x.StudentId == Guid.NewGuid());
            db.Queryable<Student_004>()

                .ToList(it => new {
                    list = it.books.Count(),
                    list2 = it.books.Count(),
                });

        }

        [SugarTable("Student_005")]
        public class Student_004 
        {
            [SqlSugar.SugarColumn(IsPrimaryKey =true)]
            public Guid sid { get; set; } 
            public Guid StudentId { get; set; }
            public string Name { get; set; }

            public Guid SchoolId { get; set; }
            [SqlSugar.Navigate(SqlSugar.NavigateType.OneToOne,nameof(SchoolId),nameof(School_004.scid2))]
            public School_004 school_001 { get; set; }
            [SqlSugar.Navigate(SqlSugar.NavigateType.OneToMany, nameof(Book_004.StudentId),nameof(StudentId))]
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
            [SqlSugar.SugarColumn(IsPrimaryKey = true )]
            public Guid scid { get; set; }
            public Guid scid2 { get; set; }
            public string schname { get; set; }
            [SqlSugar.Navigate(SqlSugar.NavigateType.OneToMany, nameof(Room_004.schoolId))]
            public List<Room_004> rooms { get; set; }
        }

        [SugarTable("Room_005")]
        public class Room_004 
        {
            [SqlSugar.SugarColumn(IsPrimaryKey = true )]
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
