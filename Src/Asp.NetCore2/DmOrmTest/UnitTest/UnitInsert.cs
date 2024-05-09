using System;
using System.Collections.Generic;
using System.Text;

namespace OrmTest
{
    internal class UnitInsert
    {
        public static void Init() 
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Unitadfa>();
            db.Insertable(new Unitadfa()
            {
                Name = "A",
                 Date = DateTime.Now,
            }).ExecuteCommand();
            db.Insertable(new List<Unitadfa>() {
            new Unitadfa()
            {
                Name = "A",
                Date = DateTime.Now,
            },
            new Unitadfa()
            {
                Name = "A",
                Date = DateTime.Now,
            }}).ExecuteCommand();
            var list=db.Queryable<Unitadfa>().ToList();
            db.DbMaintenance.TruncateTable<Unitadfa>();
            db.Fastest<Unitadfa>().OffIdentity().BulkCopy(list);
          
            db.DbMaintenance.DropTable<Unitadfa>();

        }
    }
    public class Unitadfa 
    {
        [SqlSugar.SugarColumn(IsIdentity =true,IsPrimaryKey =true)]
        public int Id { get; set; }

        public string Name { get; set; }
        [SqlSugar.SugarColumn(ColumnDataType ="Date")]
        public DateTime Date { get; set; }
    }
}
