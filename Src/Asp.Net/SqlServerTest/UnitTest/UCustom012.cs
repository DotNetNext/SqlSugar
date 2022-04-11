using OrmTest.UnitTest.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public class UCustom012
    {

        public static void Init()
        {
            var db = NewUnitTest.Db;

            db.CodeFirst.InitTables<StudentA, RoomA, SchoolA, TeacherA>();
            db.DbMaintenance.TruncateTable<StudentA>();
            db.DbMaintenance.TruncateTable<RoomA>();
            db.DbMaintenance.TruncateTable<SchoolA>();
            db.DbMaintenance.TruncateTable<TeacherA>();
            db.Insertable(new RoomA() { RoomId = 1, RoomName = "北大001室", SchoolId = 1 }).ExecuteCommand();
            db.Insertable(new RoomA() { RoomId = 2, RoomName = "北大002室", SchoolId = 1 }).ExecuteCommand();
            db.Insertable(new RoomA() { RoomId = 3, RoomName = "北大003室", SchoolId = 1 }).ExecuteCommand();
            db.Insertable(new RoomA() { RoomId = 4, RoomName = "清华001厅", SchoolId = 2 }).ExecuteCommand();
            db.Insertable(new RoomA() { RoomId = 5, RoomName = "清华002厅", SchoolId = 2 }).ExecuteCommand();
            db.Insertable(new RoomA() { RoomId = 6, RoomName = "清华003厅", SchoolId = 2 }).ExecuteCommand();


            db.Insertable(new SchoolA() { SchoolId = 1, SchoolName = "北大" }).ExecuteCommand();
            db.Insertable(new SchoolA() { SchoolId = 2, SchoolName = "清华" }).ExecuteCommand();

            db.Insertable(new StudentA() { StudentId = 1, SchoolId = 1, Name = "北大jack" }).ExecuteCommand();
            db.Insertable(new StudentA() { StudentId = 2, SchoolId = 1, Name = "北大tom" }).ExecuteCommand();
            db.Insertable(new StudentA() { StudentId = 3, SchoolId = 2, Name = "清华jack" }).ExecuteCommand();
            db.Insertable(new StudentA() { StudentId = 4, SchoolId = 2, Name = "清华tom" }).ExecuteCommand();

            db.Insertable(new TeacherA() { SchoolId = 1, Id = 1, Name = "北大老师01" }).ExecuteCommand();
            db.Insertable(new TeacherA() { SchoolId = 1, Id = 2, Name = "北大老师02" }).ExecuteCommand();

            db.Insertable(new TeacherA() { SchoolId = 2, Id = 3, Name = "清华老师01" }).ExecuteCommand();
            db.Insertable(new TeacherA() { SchoolId = 2, Id = 4, Name = "清华老师02" }).ExecuteCommand();

            //先用Mapper导航映射查出第二层
            var list = db.Queryable<StudentA>().Mapper(x => x.SchoolA, x => x.SchoolId).ToList();

            //参数1 ：将第二层对象合并成一个集合  参数2：委托
            //说明：如果2级对象是集合用SelectMany
            db.ThenMapper(list.Select(it => it.SchoolA), sch =>
            {
                //参数1: room表关联字段  参数2: school表关联字段，  参数3: school当前记录
                sch.RoomList = db.Queryable<RoomA>().SetContext(room => room.SchoolId, () => sch.SchoolId, sch).ToList();

                sch.TeacherList = db.Queryable<TeacherA>().SetContext(teachera => teachera.SchoolId, () => sch.SchoolId, sch).ToList();
            });

            db.Queryable<StudentA>()
                .Includes(x => x.SchoolA, x => x.RoomList)//2个参数就是 then Include 
                .Includes(x => x.Books) 
                .ToList();


        }
        public class StudentA
        {
            [SugarColumn(IsPrimaryKey = true)]
            public int StudentId { get; set; }
            public string Name { get; set; }
            public int SchoolId { get; set; }
            [Navigat(nameof(SchoolId))]
            public SchoolA SchoolA { get; set; }
            [Navigat(nameof(BookA.studenId))]
            public List<BookA> Books { get; set; }
        }

        public class SchoolA
        {
            [SugarColumn(IsPrimaryKey = true)]
            public int SchoolId { get; set; }
            public string SchoolName { get; set; }
            [SugarColumn(IsIgnore = true)]
            public List<RoomA> RoomList { get; set; }
            [SugarColumn(IsIgnore = true)]
            public List<TeacherA> TeacherList { get; set; }
        }
        public class TeacherA
        {
            [SugarColumn(IsPrimaryKey = true)]
            public int Id { get; set; }
            public int SchoolId { get; set; }
            public string Name { get; set; }
        }
        public class RoomA
        {
            [SugarColumn(IsPrimaryKey = true)]
            public int RoomId { get; set; }
            public string RoomName { get; set; }
            public int SchoolId { get; set; }
        }
        public class BookA 
        { 
            public int studenId { get; set; }
        }

    }
}
