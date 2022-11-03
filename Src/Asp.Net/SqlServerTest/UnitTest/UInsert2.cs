using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
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
            db.CodeFirst.InitTables<UnitafaXX1>();
            Db.Insertable(new List<UnitafaXX1>() { new UnitafaXX1 { } }).IgnoreColumns(z => z.dt2).CallEntityMethod(z => z.Test01()).ExecuteCommand();
            try
            {
                Db.Insertable(new List<UnitafaXX1>() { new UnitafaXX1 {  Id=1, dt2=DateTime.Now,dt = DateTime.Now } }).IgnoreColumns(false, true).UseParameter().ExecuteCommand();
                throw new Exception("ok");
            }
            catch (Exception ex)
            {
                if (ex.Message == "ok")
                {
                    throw new Exception("unit error"); 
                }
            }
            var list2=db.Insertable(new List<Order22>() {
            new Order22()
            {
                Name = "a",
                CreateTime = DateTime.Now,
                CustomId = 1,
                Price = 1
            },new Order22()
            {
                Name = "a",
                CreateTime = DateTime.Now,
                CustomId = 1,
                Price = 1
            }
            }).AS("Order").ExecuteReturnPkList<long>();

            var list3 = db.Insertable(new List<Order>() {
            new Order ()
            {
                Name = "a",
                CreateTime = DateTime.Now,
                CustomId = 1,
                Price = 1
            },new Order()
            {
                Name = "a",
                CreateTime = DateTime.Now,
                CustomId = 1,
                Price = 1
            }
            }).AS("Order").ExecuteReturnPkList<int>();
        }

        public class Order22
        {
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public long Id { get; set; }
            /// <summary>
            /// 姓名
            /// </summary>
            public string Name { get; set; }
            public decimal Price { get; set; }
            [SugarColumn(IsNullable = true)]
            public DateTime CreateTime { get; set; }
            [SugarColumn(IsNullable = true)]
            public int CustomId { get; set; }
            [SugarColumn(IsIgnore = true)]
            public List<OrderItem> Items { get; set; }
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

    public class UnitafaXX1
    {
        [SqlSugar.SugarColumn(IsPrimaryKey =true,IsIdentity = true)]
        public int Id { get; set; }

        public DateTime dt { get; set; }
        [SqlSugar.SugarColumn(IsNullable =true)]
        public DateTime dt2 { get; set; }

        internal void Test01()
        {
            dt = DateTime.Now;
        }
    }
}
