using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OrmTest.UOneManyMany4;

namespace OrmTest 
{
    internal class UnitUpdateNav
    {
        public static void Init()
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
            db.Insertable(new Book_004() { StudentId = id1, Name = "数学001" }).ExecuteCommand();
            db.Insertable(new Book_004() { StudentId = id2, Name = "语文002" }).ExecuteCommand();


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
                         .Includes(x => x.books)
                .Where(x => x.school_001.rooms.Any(z => z.desk.Any())).ToList();

            if (list.Count() != 2)
            {
                throw new Exception("unit error");
            }
            list.First().books = new List<Book_004>();
              db.UpdateNav(list)
                .Include(x=>x.books).ExecuteCommand();

            var list2 = db.Queryable<Student_004>()
              .Includes(x => x.books).ToList();

            if (list2.First().books.Count > 0) 
            {
                throw new Exception("unit error");
            }
            if (list2.Last().books.Count == 0)
            {
                throw new Exception("unit error");
            }
        }
    }
}
