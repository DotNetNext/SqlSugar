using SqlSugar;
using SqlSugar.DbConvert;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrmTest
{
    public partial class NewUnitTest
    {
        public static void Bulk()
        {
            var db = NewUnitTest.Db;
            db.DbMaintenance.DropTable<UnitBulkCopyaaa>();
            db.CodeFirst.InitTables<UnitBulkCopyaaa>();
            db.DbMaintenance.TruncateTable<UnitBulkCopyaaa>(); 
            db.Fastest<UnitBulkCopyaaa>().BulkCopy(new List<UnitBulkCopyaaa>()
            {
                new UnitBulkCopyaaa(){ a=DbType.GaussDB,Id=1 }
            });
            db.Fastest<UnitBulkCopyaaa>().BulkUpdate(new List<UnitBulkCopyaaa>()
            {
                new UnitBulkCopyaaa(){ a=DbType.GaussDB,Id=1 }
            });
            Console.WriteLine("用例跑完");
        }

    }
    public class UnitBulkCopyaaa 
    {
        [SugarColumn(IsPrimaryKey =true)]
        public int Id { get; set; }
        [SqlSugar.SugarColumn(ColumnDataType ="varchar(10)",SqlParameterDbType = typeof(EnumToStringConvert))]
        public DbType a { get; set; }
    }
    public class UnitTestoffset11
    {
        [SqlSugar.SugarColumn(IsNullable = true)]
        public DateTimeOffset? DateTimeOffset { get; set; }
    }
    public class UnitTable001
    {
        [SqlSugar.SugarColumn(IsPrimaryKey =true)]
        public int Id { get; set; }
        public string table { get; set; }
    }
    public class UnitIdentity111
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class UnitIdentity111111111
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class UnitIdentity1 
    {
        [SqlSugar.SugarColumn(IsPrimaryKey =true,IsIdentity =true)]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
