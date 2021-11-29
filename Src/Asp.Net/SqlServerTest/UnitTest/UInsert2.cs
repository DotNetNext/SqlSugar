using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public partial class NewUnitTest
    {
        public static void Insert2()
        {
            var db = Db;
            db.CodeFirst.InitTables<UnitInsertMethod>();
            db.Insertable(new UnitInsertMethod() { Name = "1" }).CallEntityMethod(it=>it.Create()).ExecuteCommand();
            db.Insertable(new UnitInsertMethod() { Name = "2" }).CallEntityMethod(it => it.Create("admin")).ExecuteCommand();
            db.Updateable(new UnitInsertMethod() {Id=1, Name = "1" }).CallEntityMethod(it => it.Create()).ExecuteCommand();
            db.Updateable(new UnitInsertMethod() { Name = "1" }).CallEntityMethod(it => it.Create("admint")).ExecuteCommand();
            db.CodeFirst.InitTables<Unitsdafa111>();
            db.Insertable(new Unitsdafa111()).ExecuteCommand();
            db.Insertable(new Unitsdafa111() {Id=Guid.NewGuid(),Id2=Guid.NewGuid() }).ExecuteCommand();
            var list=db.Queryable<Unitsdafa111>().ToList();
        }
        public class Unitsdafa111
        {
            [SqlSugar.SugarColumn(IsNullable =true,ColumnDataType ="nvarchar(50)")]
            public Guid Id { get; set; }
            [SqlSugar.SugarColumn(IsNullable = true, ColumnDataType = "nvarchar(50)")]
            public Guid? Id2 { get; set; }
        }
        public class UnitInsertMethod
        {
            [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int Id { get; set; }
            public string Name { get; set; }
            public DateTime Time { get; set; }
            [SqlSugar.SugarColumn(IsNullable =true)]
            public string UserId { get; set; }

            public void Create()
            {
                this.Time = DateTime.Now;
                this.UserId = "1";
            }
            public void Create(string a)
            {
                this.Time = DateTime.Now;
                this.UserId = a;
            }
        }

    }
}
