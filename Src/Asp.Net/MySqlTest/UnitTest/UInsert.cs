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
            [SqlSugar.SugarColumn(ColumnDataType = " bigint(20)",IsNullable =true)]
            public long? Id { get; set; }
            [SqlSugar.SugarColumn(ColumnDataType = " bigint(20)" )]
            public long Id2 { get; set; }
        }
        public static void Insert()
        {
            Db.CodeFirst.InitTables<Unit4ASDF>();
            Db.Insertable(new List<Unit4ASDF>() {
                 new Unit4ASDF() { Id=null, Id2=1 },
                   new Unit4ASDF() { Id=2, Id2=1 }}).UseMySql().ExecuteBlueCopy();

            var list = Db.Queryable<Unit4ASDF>().ToList();
        }
    }
}
