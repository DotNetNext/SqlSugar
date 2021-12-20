using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public partial class NewUnitTest
    {
        public static void CodeFirst()
        {
            if (Db.DbMaintenance.IsAnyTable("UnitCodeTest1", false))
                Db.DbMaintenance.DropTable("UnitCodeTest1");
            Db.CodeFirst.InitTables<UnitCodeTest1>();
            Db.CodeFirst.InitTables<UnitCodeTest22>();
            Db.Insertable(new UnitCodeTest22()).ExecuteCommand();
            Db.DbMaintenance.TruncateTable<UnitCodeTest22>();
            Db.CodeFirst.InitTables<UnitCodeTest3>();
            Db.Insertable(new UnitCodeTest22() {  id=1}).ExecuteCommand();
            Db.CodeFirst.InitTables<UnitTimeSpan2>();
            Db.Insertable(new UnitTimeSpan2()
            {
                 Id=new TimeSpan(),
                  id2=new TimeSpan(11,2,1)
            }).ExecuteCommand();
            var x= Db.Queryable<UnitTimeSpan2>().ToList();
            Db.CodeFirst.InitTables<UnitDateOfTime22>();
            Db.Insertable(new UnitDateOfTime22() { DateTimeOffset1 = DateTimeOffset.Now }).ExecuteCommand();
            Db.Insertable(new List<UnitDateOfTime22> { new UnitDateOfTime22() { DateTimeOffset1 = DateTimeOffset.Now }, new UnitDateOfTime22() { DateTimeOffset1 = DateTimeOffset.Now } }).ExecuteCommand();
            var list2 = Db.Queryable<UnitDateOfTime22>().ToList();

            Db.CodeFirst.InitTables<UnitTestPk>();
            Db.CodeFirst.InitTables<UnitTestPk>();
            Db.CodeFirst.InitTables<UnitTestPk>();
        }

        public class UnitTestPk 
        {
            [SqlSugar.SugarColumn(IsPrimaryKey =true,IsIdentity =true)]
            public int id { get; set; }
            [SqlSugar.SugarColumn(IsPrimaryKey = true )]
            public long id2 { get; set; }
            [SqlSugar.SugarColumn(IsPrimaryKey = true )]
            public Guid id3 { get; set; }

            public string Name { get; set; }
        
        }

        [SqlSugar.SugarTable("UnitDateOfTime221")]
        public class UnitDateOfTime22
        {
            public DateTimeOffset DateTimeOffset1 { get; set; }
        }
        public class UnitTimeSpan2
        {
            [SqlSugar.SugarColumn(ColumnDataType ="time")]
            public TimeSpan Id { get; set; }
            public TimeSpan id2 { get; set; }
        }
        public class UnitCodeTest22 {
             [SqlSugar.SugarColumn(IsNullable =true)]
             public decimal? id { get; set; }
        }
        [SqlSugar.SugarTable("UnitCodeTest22")]
        public class UnitCodeTest3
        {
            [SqlSugar.SugarColumn(IsNullable = false)]
            public int id { get; set; }
        }

        public class UnitCodeTest1
        {
            [SqlSugar.SugarColumn(IndexGroupNameList = new string[] { "group1" })]
            public int Id { get; set; }
            [SqlSugar.SugarColumn(DefaultValue= "now()", IndexGroupNameList =new string[] {"group1" } )]
            public DateTime? CreateDate { get; set; }
        }
    }
}
