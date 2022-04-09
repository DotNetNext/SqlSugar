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

            db.CodeFirst.InitTables<StudentA, RoomA, SchoolA>();
            db.DbMaintenance.TruncateTable<StudentA>();
            db.DbMaintenance.TruncateTable<RoomA>();
            db.DbMaintenance.TruncateTable<SchoolA>();

            db.Insertable(new RoomA() { RoomId = 1, RoomName = "北大001室", SchoolId = 1 }).ExecuteCommand();
            db.Insertable(new RoomA() { RoomId = 2, RoomName = "北大002室", SchoolId = 1 }).ExecuteCommand();
            db.Insertable(new RoomA() { RoomId = 3, RoomName = "北大003室", SchoolId = 1 }).ExecuteCommand();
            db.Insertable(new RoomA() { RoomId = 4, RoomName = "清华001厅", SchoolId = 2 }).ExecuteCommand();
            db.Insertable(new RoomA() { RoomId = 5, RoomName = "清华002厅", SchoolId = 2 }).ExecuteCommand();
            db.Insertable(new RoomA() { RoomId = 6, RoomName = "清华003厅", SchoolId = 2 }).ExecuteCommand();


            db.Insertable(new SchoolA() {  SchoolId=1, SchoolName="北大"  }).ExecuteCommand();
            db.Insertable(new SchoolA() { SchoolId = 2, SchoolName = "清华" }).ExecuteCommand();

            db.Insertable(new StudentA() { StudentId=1, SchoolId=1, Name= "北大jack" }).ExecuteCommand();
            db.Insertable(new StudentA() { StudentId = 2, SchoolId = 1, Name = "北大tom" }).ExecuteCommand();
            db.Insertable(new StudentA() { StudentId = 3, SchoolId = 2, Name = "清华jack" }).ExecuteCommand();
            db.Insertable(new StudentA() { StudentId = 4, SchoolId = 2, Name = "清华tom" }).ExecuteCommand();

            var list=db.Queryable<StudentA>().Mapper(x => x.SchoolA, x => x.SchoolId).ToList();

            db.ThenMapper(list.Select(it => it.SchoolA), sch =>
            {
                sch.RoomList = db.Queryable<RoomA>().SetContext(room => room.SchoolId , () => sch.SchoolId, sch).ToList();
            });


        }
        public class StudentA
        {
            [SugarColumn(IsPrimaryKey = true)]
            public int StudentId { get; set; }
            public string Name { get; set; }
            public int SchoolId { get; set; }
            [SugarColumn(IsIgnore = true)]
            public SchoolA SchoolA { get; set; }
        }

        public class SchoolA
        {
            [SugarColumn(IsPrimaryKey = true)]
            public int SchoolId { get; set; }
            public string SchoolName { get; set; }
            [SugarColumn(IsIgnore = true)]
            public List<RoomA> RoomList { get; set; }
        }

        public class RoomA
        {
            [SugarColumn(IsPrimaryKey = true)]
            public int RoomId { get; set; }
            public string RoomName { get; set; }
            public int SchoolId { get; set; }
        }

    }
}
