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
            db.CodeFirst.InitTables<BookA>();
            db.DbMaintenance.TruncateTable<StudentA>();
            db.DbMaintenance.TruncateTable<RoomA>();
            db.DbMaintenance.TruncateTable<SchoolA>();
            db.DbMaintenance.TruncateTable<TeacherA>();
            db.DbMaintenance.TruncateTable<BookA>();
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


            db.Insertable(new BookA() { BookId=1, Name="java" , studenId=1 }).ExecuteCommand();
            db.Insertable(new BookA() { BookId = 2, Name = "c#2", studenId = 2 }).ExecuteCommand();
            db.Insertable(new BookA() { BookId = 3, Name = "c#1", studenId = 2 }).ExecuteCommand();
            db.Insertable(new BookA() { BookId = 4, Name = "php", studenId = 3 }).ExecuteCommand();
            db.Insertable(new BookA() { BookId = 5, Name = "js", studenId = 4 }).ExecuteCommand();

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
     

            var list2=db.Queryable<StudentA>()
                .Includes(x => x.SchoolA, x => x.RoomList)//2个参数就是 then Include 
                .Includes(x => x.Books) 
                .ToList();
            db.CodeFirst.InitTables<A1, B1, ABMapping1>();
            db.DbMaintenance.TruncateTable<A1>();
            db.DbMaintenance.TruncateTable<B1>();
            db.DbMaintenance.TruncateTable<ABMapping1>();
            db.Insertable(new A1() { Id = 1, Name = "a1" }).ExecuteCommand();
            db.Insertable(new A1() { Id = 2, Name = "a2" }).ExecuteCommand();
            db.Insertable(new B1() { Id = 1, Name = "b1" }).ExecuteCommand();
            db.Insertable(new B1() { Id = 2, Name = "b2" }).ExecuteCommand();
            db.Insertable(new ABMapping1() {  AId=1,BId=1 }).ExecuteCommand();
            db.Insertable(new ABMapping1() { AId = 2, BId = 2 }).ExecuteCommand();
            var list3= db.Queryable<A1>().Includes(x => x.BList).ToList();
        }

        public class ABMapping1
        {
            [SugarColumn(IsPrimaryKey = true )]
            public int AId { get; set; }
            public int BId { get; set; }
        }
        public class A1
        {
            [SugarColumn(IsPrimaryKey = true  )]
            public int Id { get; set; }
            public string Name { get; set; }
            [Navigat(typeof(ABMapping1),nameof(ABMapping1.AId),nameof(ABMapping1.BId))]
            public List<B1> BList { get; set; }
        }
        public class B1
        {
            [SugarColumn(IsPrimaryKey = true )]
            public int Id { get; set; }
            public string Name { get; set; }
            [Navigat(typeof(ABMapping1), nameof(ABMapping1.BId), nameof(ABMapping1.AId))]
            public List<A1> BList { get; set; }
        }

        public class StudentA
        {
            [SugarColumn(IsPrimaryKey = true)]
            public int StudentId { get; set; }
            public string Name { get; set; }
            public int SchoolId { get; set; }
            [Navigat(NavigatType.OneToOne, nameof(SchoolId))]
            public SchoolA SchoolA { get; set; }
            [Navigat(NavigatType.OneToMany, nameof(BookA.studenId))]
            public List<BookA> Books { get; set; }

        }
        public class SchoolA
        {
            [SugarColumn(IsPrimaryKey = true)]
            public int SchoolId { get; set; }
            public string SchoolName { get; set; }
            [Navigat(NavigatType.OneToMany,nameof(RoomA.SchoolId))]
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
            [SugarColumn(IsPrimaryKey = true)]
            public int BookId { get; set; }

            public string Name { get; set; }
            public int studenId { get; set; }
        }

    }
}
