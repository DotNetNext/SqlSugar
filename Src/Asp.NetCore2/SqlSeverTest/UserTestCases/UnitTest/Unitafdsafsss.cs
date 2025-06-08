using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OrmTest.Unitaadfas1;

namespace OrmTest
{
    public class Unitafdsafsss
    {
        public static void Init() 
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Test001>();
            db.DbMaintenance.TruncateTable<Test001>();
            db.Insertable(new Test001() { ID = 1, Num = 2+"" }).ExecuteCommand();
            var iiii = db.Queryable<Unitafdsafsss.Test001>()
             .Select((c) =>
                 new Unitafdsafsss.TestDTO { A = c }
                 )
             .ToList();
            if (iiii.First().A == null) 
            {
                throw new Exception("unit error");
            }
            var iiii2 = db.Queryable<Unitafdsafsss.Test001>()
               .Select((A) =>
                   new Unitafdsafsss.TestDTO { A = A }
               )
           .ToList();
            if (iiii2.First().A == null)
            {
                throw new Exception("unit error");
            }
            var iiii3 = db.Queryable<Unitafdsafsss.Test001>()
             .Select((A) =>
               new Unitafdsafsss.TestDTO2 {Id=A.ID,A = A }
             )
            .ToList();
            if (iiii3.First().A == null|| iiii3.First().Id==0)
            {
                throw new Exception("unit error");
            }
        }
        [SugarTable("Test002asdfasss")]
        public class Test001
        {
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int ID { get; set; }
            public string Num { get; set; }

            public int Test002ID { get; set; }

            [Navigate(NavigateType.ManyToOne, nameof(Test002ID))]
            public Test002 Test002lass { get; set; }
        }

        [SugarTable("Test002asdfa")]
        public class Test002
        {
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int ID { get; set; }
            public DateTime Date { get; set; }

            public int Test002ID { get; set; }

            [Navigate(NavigateType.ManyToOne, nameof(Test002ID))]
            public Test002 Test002Class { get; set; }//A 要么注释掉这句
        }

        public class TestDTO
        {
            public Test001 A { get; set; }
            public Test002? B { get; set; }//B 要么注释掉这句， 可空类型，应该可以不用吧

        }
        public class TestDTO2
        {
            public int Id { get; set; }
            public Test001 A { get; set; }
            public Test002? B { get; set; }//B 要么注释掉这句， 可空类型，应该可以不用吧

        }
    }

}
