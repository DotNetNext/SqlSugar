using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace OrmTest
{
    public partial class NewUnitTest
    {
        public class Unit4ASDF
        {
            [SqlSugar.SugarColumn(ColumnDataType = " bigint(20)", IsNullable = true)]
            public long? Id { get; set; }
            [SqlSugar.SugarColumn(ColumnDataType = " bigint(20)")]
            public long Id2 { get; set; }
        }
        public static void Insert()
        {
            Db.CodeFirst.InitTables<Unit4ASDF>();
            Db.Insertable(new List<Unit4ASDF>() {
                 new Unit4ASDF() { Id=null, Id2=1 },
                   new Unit4ASDF() { Id=2, Id2=1 }}).UseMySql().ExecuteBulkCopy();

            var list = Db.Queryable<Unit4ASDF>().ToList();

            Db.CodeFirst.InitTables<testdb>();
            Db.DbMaintenance.TruncateTable("testdb");
            var list1 = new List<testdb>();



            for (int i = 0; i < 10; i++)

            {

                var id = i.ToString();

                list1.Add(new testdb

                {

                    id = id,

                });

                Console.WriteLine(id + " Length：" + id.Length);

            }



            Db.Insertable(list1).UseMySql().ExecuteBulkCopy();



            var queryList = Db.Queryable<testdb>().ToList();



            foreach (var item in queryList)

            {

                if (item.id.Length != 1)

                {

                    throw new Exception("blue copy");

                }

            }

            Db.CodeFirst.InitTables<Testdbbool>();
            Db.DbMaintenance.TruncateTable("Testdbbool");
            Db.Insertable(new Testdbbool() { isok = true }).UseMySql().ExecuteBulkCopy();
            Db.Fastest<Testdbbool>().BulkCopy(new List<Testdbbool>() { new Testdbbool() { isok = true }, new Testdbbool() { isok = false } });
            var list2= Db.Queryable<Testdbbool>().ToList();

            if (!list2.Any(it => it.isok == false)) 
            {
                throw new Exception("blue copy");
            }

            Db.CodeFirst.InitTables<MicroBlog>();

            var x= Db.Storageable(new List<MicroBlog>()
            {
                new MicroBlog(){ Mid="1" },
                new MicroBlog(){ Mid="2" }
            })
             .SplitInsert(it=>!it.Any())
             .WhereColumns(it=>it.Mid).ToStorage();

            x.AsInsertable.ExecuteCommand();

            Db.CodeFirst.InitTables<UnitFastest001>();
            Db.DbMaintenance.TruncateTable<UnitFastest001>();
            var fastList = new List<UnitFastest001>()
            {
                new UnitFastest001 (){  Id=Guid.NewGuid()+"", Remark=@"aa
raa" }
            };
            Db.Fastest<UnitFastest001>().BulkCopy(fastList);
            var searchList = Db.Queryable<UnitFastest001>().ToList();
            if (searchList.Count != 1) 
            {
                throw new Exception("unit error");
            }
        }

        public class UnitFastest001 
        {
            public string Id { get; set; }
            [SqlSugar.SugarColumn(Length =1000)]
            public string Remark { get; set; }
        }
        public class MicroBlog
        {
            //[SugarColumn(IsPrimaryKey = true, IsIdentity = true)]//如果是主键，此处必须指定，否则会引发InSingle(id)方法异常。
            public string Mid { get; set; }
            //public int id { get; set; }
            [SqlSugar.SugarColumn(IsNullable = true)]
            public string uid { get; set; }
            [SqlSugar.SugarColumn(IsNullable = true)]
            public string nick { get; set; }
            [SqlSugar.SugarColumn(IsNullable = true)]
            public DateTime SendTime { get; set; }
            [SqlSugar.SugarColumn(IsNullable = true)]
            public string content { get; set; }
            [SqlSugar.SugarColumn(IsNullable = true)]
            public string ForwardHtml { get; set; }
            [SqlSugar.SugarColumn(IsNullable =true)]
            public string MediaHtml { get; set; }
            //public DateTime CreatedAt { get; set; }
        }
        public class testdb

        {

            public string id { get; set; }

        }

        public class Testdbbool
        {
            [SqlSugar.SugarColumn(IsPrimaryKey = true,IsIdentity =true)]
            public int id { get; set; }
            public bool isok { get; set; }
        }
    }
}
