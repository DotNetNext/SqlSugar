using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
namespace OrmTest
{
    public class ORdER
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true,ColumnName ="id")]
        public int Id1 { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        [SugarColumn(ColumnName = "Name")]
        public string Name1 { get; set; }
        [SugarColumn(ColumnName = "Price")]
        public decimal Price1 { get; set; }
        [SugarColumn(IsNullable = true,ColumnName = "CreateTime")]
        public DateTime CreateTime1 { get; set; }
        [SugarColumn(IsNullable = true, ColumnName = "CustomId")]
        public int CustomId1 { get; set; }
        [SugarColumn(IsIgnore = true)]
        public List<OrderItem> Items { get; set; }
    }
    public class UCustom24
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;
            db.DbMaintenance.TruncateTable<ORdER>();
            db.Insertable(new ORdER() { Id1 = 1, Name1 = "jack", CreateTime1 = DateTime.Now, CustomId1 = 1 }).ExecuteCommand();
            var test1 = db.Queryable<ORdER>()
                 .ToList(z => new
                 {
                     name1 = new { z.Id1, z.Name1, ZId = 100 }
                 }).First();

            if (test1.name1.Id1 != 1 || test1.name1.Name1 != "jack" || test1.name1.ZId != 100)
            {
                throw new Exception("unit error");
            }

            var test2 = db.Queryable<ORdER>()
           .ToList(z => new
           {
               name1 = new { z.Id1, z.Name1, ZId = z.Id1.ToString() }
           }).First();

            if (test2.name1.Id1 != 1 || test2.name1.Name1 != "jack" || test2.name1.ZId != "1")
            {
                throw new Exception("unit error");
            }

            var test3 = db.Queryable<ORdER>()
            .Take(2)
             .Select(i => new TestDTO
             {
                 SubOne = new TestSubDTO { NameOne = "a1", NameTwo = i.Id1.ToString() },
                 //   SubTwo = new TestSubDTO { NameOne = i.Name, NameTwo = i.Name }
             })
             .First();

            if (test3.SubOne.NameOne != "a1" || test3.SubOne.NameTwo != "1")
            {
                throw new Exception("unit error");
            }

            var test4 = db.Queryable<ORdER>()
            .Take(2)
             .Select(i => new TestDTO
             {
                 SubOne = new TestSubDTO { NameOne = "a1", NameTwo = i.Name1 },
                 //SubTwo = new TestSubDTO { NameOne = i.Name, NameTwo = i.Name }
             })
            .ToList().First();
            if (test4.SubOne.NameOne != "a1" || test4.SubOne.NameTwo != "jack")
            {
                throw new Exception("unit error");
            }

            var test5 = db.Queryable<ORdER>()
           .Take(2)
            .Select(i => new TestDTO
            {
                SubOne = new TestSubDTO { NameOne = i.Name1, NameTwo = i.Name1 },
                //SubTwo = new TestSubDTO { NameOne = i.Name, NameTwo = i.Name }
            })
           .ToList().First();
            if (test5.SubOne.NameOne != "jack" || test5.SubOne.NameTwo != "jack")
            {
                throw new Exception("unit error");
            }

            var test6 = db.Queryable<ORdER>()
            .Take(2)
             .Select(i => new TestDTO
             {
                 SubOne = new TestSubDTO { NameOne = i.Name1+"1", NameTwo = i.Name1+"2" },
                 SubTwo = new TestSubDTO { NameOne = i.Name1+"3", NameTwo = i.Name1+"4" }
             })
            .ToList().First();
            if (test6.SubOne.NameOne != "jack1" || test6.SubOne.NameTwo != "jack2"||
                test6.SubTwo.NameOne != "jack3" || test6.SubTwo.NameTwo != "jack4")
            {
                throw new Exception("unit error");
            }
            var p = new Order() { };
            var sqlobj=db.Updateable<Order>().SetColumns(x => x.Name == p.Name)
                .Where(x => x.Id == 1).ToSql();

            if (!sqlobj.Key.Contains("=")) 
            {
                throw new Exception("unit error");
            }

            var sqlobj2 = db.Updateable<Order>().SetColumns(x => x.Name == null)
             .Where(x => x.Id == 1).ToSql();

            if (!sqlobj2.Key.Contains("="))
            {
                throw new Exception("unit error");
            }
        }

        public class TestDTO
        {
            public TestSubDTO SubOne { get; set; }

            public TestSubDTO SubTwo { get; set; }
        }

        public class TestSubDTO
        {
            public string NameOne { get; set; }

            public string NameTwo { get; set; }
        }
    }
}
