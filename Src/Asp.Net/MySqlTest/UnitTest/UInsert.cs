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
                   new Unit4ASDF() { Id=2, Id2=1 }}).UseMySql().ExecuteBlueCopy();

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



            Db.Insertable(list1).UseMySql().ExecuteBlueCopy();



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
            Db.Insertable(new Testdbbool() { isok=true }).UseMySql().ExecuteBlueCopy();
            Db.Insertable(new Testdbbool() { isok = false }).UseMySql().ExecuteBlueCopy();
            var x=Db.Queryable<Testdbbool>().ToList();
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
