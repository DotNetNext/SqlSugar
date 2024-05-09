using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest 
{
    public class UOneManyMany3
    {

        public static void init()
        {

            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Student_003, School_003, Room_003, Desk_003>();
            db.DbMaintenance.TruncateTable<Student_003, School_003, Room_003, Desk_003>();

            db.Insertable(new Student_003() { sid = 1, Name = "北大jack", SchoolId = 1 }).ExecuteCommand();
            db.Insertable(new Student_003() { sid = 2, Name = "青华jack", SchoolId = 2 }).ExecuteCommand();

            db.Insertable(new School_003() { scid = 1, schname = "北大" }).ExecuteCommand();
            db.Insertable(new School_003() { scid = 2, schname = "青华" }).ExecuteCommand();


            db.Insertable(new Room_003() { roomId = 1, schoolId = 1, roomName = "北大01室" }).ExecuteCommand();
            db.Insertable(new Room_003() { roomId = 2, schoolId = 1, roomName = "北大02室" }).ExecuteCommand();
            db.Insertable(new Room_003() { roomId = 3, schoolId = 2, roomName = "青华03室" }).ExecuteCommand();
            db.Insertable(new Room_003() { roomId = 4, schoolId = 2, roomName = "青华04室" }).ExecuteCommand();

            db.Insertable(new Desk_003() { roomId = 1, deskid = 1, deskName = "北大01室_01" }).ExecuteCommand();
            db.Insertable(new Desk_003() { roomId = 2, deskid = 2, deskName = "北大02室_01" }).ExecuteCommand();
            db.Insertable(new Desk_003() { roomId = 3, deskid = 3, deskName = "青华03室_01" }).ExecuteCommand();
            db.Insertable(new Desk_003() { roomId = 4, deskid = 4, deskName = "青华04室_01" }).ExecuteCommand();


            var list = db.Queryable<Student_003>()
                         .Includes(x => x.school_001, x => x.rooms, x => x.desk)
                .Where(x => x.school_001.rooms.Any(z => z.desk.Any())).ToList();

            if (list.Count() != 2)
            {
                throw new Exception("unit error");
            }

            var list2 = db.Queryable<Student_003>()
                         .Includes(x => x.school_001, x => x.rooms)
            .Where(x => x.school_001.rooms.Any(z =>
            z.roomName == "北大01室" &&
            z.desk.Any())).ToList();


            if (list2.Count() != 1)
            {
                throw new Exception("unit error");
            }

            var list3 = db.Queryable<Student_003>()
                .Includes(x => x.school_001, x => x.rooms)
         .Where(x => x.school_001.rooms.Any(z =>
         z.roomName == "青华03室" &&
         z.desk.Any(c => c.deskName == "青华03室_01"))).ToList();

            if (list3.Count != 1)
            {
                throw new Exception("unit error");
            }

            var list4 = db.Queryable<Student_003>()
        .Where(x => x.school_001.rooms.Any(z =>
        z.roomName == "青华03室" &&
        z.desk.Any(c => c.deskName == "青华04室_01"))).ToList();


            if (list4.Count != 0)
            {
                throw new Exception("unit error");
            }
            db.DbMaintenance.TruncateTable<Student_003, School_003, Room_003, Desk_003>();
            foreach (var item in list) 
            {
                item.sid = 0;
                item.SchoolId = 0;
                item.school_001.scid = 0;
                foreach (var r in item.school_001.rooms)
                {
                    r.roomId = 0;
                    r.schoolId = 0;
                    foreach (var z in r.desk)
                    {
                        z.deskid = 0;
                        z.roomId = 0;
                    }
                }
            }
            db.InsertNav(list.First())
                .Include(x => x.school_001)
                .ThenInclude(x => x.rooms)
                .ThenInclude(x => x.desk).ExecuteCommand();
            db.InsertNav(list.Last())
              .Include(x => x.school_001)
              .ThenInclude(x => x.rooms)
              .ThenInclude(x => x.desk).ExecuteCommand();

            if (db.Queryable<Desk_003>().Count() != 4 || db.Queryable<Room_003>().Count() != 4
                || db.Queryable<School_003>().Count() != 2 || db.Queryable<Student_003>().Count() != 2) 
            {
                throw new Exception("unit error");
            }

            db.DbMaintenance.TruncateTable<Student_003, School_003, Room_003, Desk_003>();

            db.InsertNav(list.First().school_001)
             .Include(x => x.rooms)
             .ThenInclude(x => x.desk).ExecuteCommand();
            db.InsertNav(list.Last().school_001)
              .Include(x => x.rooms)
              .ThenInclude(x => x.desk).ExecuteCommand();

            if (db.Queryable<Desk_003>().Count() != 4 || db.Queryable<Room_003>().Count() != 4
               || db.Queryable<School_003>().Count() != 2 || db.Queryable<Student_003>().Count() != 0)
            {
                throw new Exception("unit error");
            }

            db.QueryFilter.AddTableFilter<Room_003>(x => x.roomName == "a");
            var listxx= db.Queryable<School_003>()
            .Includes(x => x.rooms).ToList();
            var listyyy=  db.Queryable<School_003>()
                .ClearFilter<Room_003>()
          .Includes(x => x.rooms).ToList();
            if (listxx.First().rooms.Count() != 0 || listyyy.First().rooms.Count() == 0) 
            {
                throw new Exception("unit error");
            }
            //db.QueryFilter.AddTableFilter<Room_003>(x => x.roomName == "a");
            var sql1 = db.Queryable<School_003>()
            .Where(x => x.rooms.Any()).ToSql();
            var sql12 = db.Queryable<School_003>()
                .ClearFilter<Room_003>()
           .Where(x => x.rooms.Any()).ToSql();
            if (sql1.Key == sql12.Key) 
            {
                throw new Exception("unit error");
            }
            db.Queryable<School_003>()
        .Where(x => x.rooms.Any()).ToList();
            db.Queryable<School_003>()
                .ClearFilter<Room_003>()
           .Where(x => x.rooms.Any()).ToList();
        }

        public class Student_003 
        {
            [SqlSugar.SugarColumn(IsPrimaryKey =true)]
            public long sid { get; set; } 
            public string Name { get; set; }

            public long SchoolId { get; set; }
            [SqlSugar.Navigate(SqlSugar.NavigateType.OneToOne,nameof(SchoolId))]
            public School_003 school_001 { get; set; }  

        }

        public class School_003
        {
            [SqlSugar.SugarColumn(IsPrimaryKey = true )]
            public long scid { get; set; }
            public string schname { get; set; }
            [SqlSugar.Navigate(SqlSugar.NavigateType.OneToMany, nameof(Room_003.schoolId))]
            public List<Room_003> rooms { get; set; }
        }

        public class Room_003 
        {
            [SqlSugar.SugarColumn(IsPrimaryKey = true )]
            public long roomId { get; set; }
            public long schoolId { get; set; }
            public string roomName { get; set; }
            [SqlSugar.Navigate(SqlSugar.NavigateType.OneToMany, nameof(Desk_003.roomId))]
            public List<Desk_003> desk { get; set; }
        }

        public class Desk_003 
        {
            [SqlSugar.SugarColumn(IsPrimaryKey = true)]
            public long deskid { get; set; }
            public string deskName { get; set; }
            public long roomId { get; set; }
        }
    }
}
