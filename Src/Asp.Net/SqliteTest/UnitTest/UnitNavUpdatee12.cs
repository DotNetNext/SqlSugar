using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Test.Model;
namespace OrmTest
{
    public class UnitNavUpdatee12
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Student, School, Playground, Room, Book>();
            db.CodeFirst.InitTables<OtherThingTwo, OtherThingOne>();
            Demo1(db);
            Demo2(db);
        }

        private static void Demo1(SqlSugarClient db)
        {
            //清空初始化数据
            db.DbMaintenance.TruncateTable<Student, School, Playground, Room, Book>();
            db.DbMaintenance.TruncateTable<OtherThingOne, OtherThingTwo>();

            //插入测试数据
            db.InsertNav(new Student()
            {
                Age = "11",
                Name = "a",
                Books = new List<Book>() { new Book() { Name = "book" } },
                Schools = new List<School>() {
                   new School(){
                       Name="学校",
                       Playgrounds=new List<Playground>()
                       {
                           new Playground(){  Name="学校GR"}

                       } ,
                       Rooms=new List<Room>(){
                             new Room() {  Name= "学校ROOM" }
                       },
                        OtherThingOnes=new List<OtherThingOne>()
                        {
                          new OtherThingOne(){ Name="学校One" }
                        },
                        OtherThingTwos=new List<OtherThingTwo>()
                        {
                          new OtherThingTwo(){ Name="学校Two" }
                        }


                    }


                 }

            })
            .Include(s => s.Books)
            .Include(s => s.Schools).ThenInclude(sc => sc.Rooms)
            .Include(s => s.Schools).ThenInclude(sc => sc.Playgrounds)
            .Include(s => s.Schools).ThenInclude(sc => sc.OtherThingOnes)
            .Include(s => s.Schools).ThenInclude(sc => sc.OtherThingTwos)
            .ExecuteCommand();

            var data = db.Queryable<Student>()
                .Includes(s => s.Books)
                .Includes(s => s.Schools, s => s.Rooms)
                .Includes(s => s.Schools, s => s.Playgrounds)
                .Includes(s => s.Schools, s => s.OtherThingOnes)
                .Includes(s => s.Schools, s => s.OtherThingTwos)
                .ToList();

            data[0].Schools[0].OtherThingOnes[0].Name = "updateOne";

            if (data.Count != 1 || data.First().Schools.Count != 1 || data.First().Schools.First().Rooms.Count() != 1 || data.First().Schools.First().Playgrounds.Count() != 1)
            {
                throw new Exception("unit error");
            }

            //更新数据
            db.UpdateNav(data)
            .Include(s => s.Schools).ThenInclude(sc => sc.Rooms)
           .Include(s => s.Schools).ThenInclude(sc => sc.Playgrounds)
           .Include(s => s.Schools).ThenInclude(sc => sc.OtherThingOnes)
           .Include(s => s.Schools).ThenInclude(sc => sc.OtherThingTwos)
           .Include(s => s.Books)
           .ExecuteCommand();

            var data2 = db.Queryable<Student>()
                .Includes(s => s.Books)
                .Includes(s => s.Schools, s => s.Rooms)
                .Includes(s => s.Schools, s => s.Playgrounds)
                .Includes(s => s.Schools, s => s.OtherThingOnes)
                .Includes(s => s.Schools, s => s.OtherThingTwos)
                .ToList();
            if (data2.Count != 1 || data2.First().Schools.Count != 1 || data2.First().Schools.First().Rooms.Count() != 1 || data2.First().Schools.First().Playgrounds.Count() != 1)
            {
                throw new Exception("unit error");
            }
        }
        private static void Demo2(SqlSugarClient db)
        {
            db.DbMaintenance.TruncateTable<Student, School, Playground, Room, Book>();
            db.InsertNav(new Student()
            {
                Age = "11",
                Name = "a",
                Books = new List<Book>() { new Book() { Name = "book" } },
                Schools = new List<School>() {
                   new School(){
                       Name="学校",
                       Playgrounds=new List<Playground>()
                       {
                           new Playground(){  Name="学校GR"}

                       } ,
                       Rooms=new List<Room>(){
                             new Room() {  Name= "学校ROOM" } } }

                 }

            })

            //   .Include(s => s.Schools)
            .Include(s => s.Schools).ThenInclude(sc => sc.Rooms)
            .Include(s => s.Schools).ThenInclude(sc => sc.Playgrounds)
            .Include(s => s.Books)
            .ExecuteCommand();
            var data = db.Queryable<Student>()
                .Includes(s => s.Books)
                .Includes(s => s.Schools, s => s.Rooms)
                .Includes(s => s.Schools, s => s.Playgrounds)
                .ToList();
            if (data.Count != 1 || data.First().Schools.Count != 1 || data.First().Schools.First().Rooms.Count() != 1 || data.First().Schools.First().Playgrounds.Count() != 1)
            {
                throw new Exception("unit error");
            }
            foreach (var item in data)
            {
                item.Name = "st" + item.Name;
                foreach (var s in item.Schools)
                {
                    s.Name = "shool" + s.Name;
                    foreach (var si in s.Rooms)
                    {
                        si.Name += "1";
                    }
                    foreach (var si in s.Playgrounds)
                    {
                        si.Name += "1";
                    }
                }

            }
            db.UpdateNav(data)
             .Include(s => s.Schools).ThenInclude(sc => sc.Rooms)
            .Include(s => s.Schools).ThenInclude(sc => sc.Playgrounds)
            .Include(s => s.Books)
            .ExecuteCommand();

            var data2 = db.Queryable<Student>()
                //.Includes(s => s.Books)
                .Includes(s => s.Schools, s => s.Rooms)
                .Includes(s => s.Schools, s => s.Playgrounds)
                .ToList();
            if (data2.Count != 1 || data2.First().Schools.Count != 1 || data2.First().Schools.First().Rooms.Count() != 1 || data2.First().Schools.First().Playgrounds.Count() != 1)
            {
                throw new Exception("unit error");
            }

        }
    }
}
