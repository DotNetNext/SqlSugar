using SqlSugar;
using SqlSugarSelect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public partial class NewUnitTest
    {

        public static void Json()
        {
            Db.CodeFirst.InitTables<UnitJsonTest>();
            Db.CodeFirst.InitTables<UnitJsonTest2>();
            Db.DbMaintenance.TruncateTable<UnitJsonTest>();
            Db.Insertable(new UnitJsonTest() { Order = new Order { Id = 1, Name = "order1" } }).ExecuteCommand();
            var list = Db.Queryable<UnitJsonTest>().ToList();
            var json = Db.Queryable<UnitJsonTest>().Select(it => SqlFunc.JsonField(it.Order, "Name")).ToList();
            UValidate.Check("order1", list.First().Order.Name, "Json");
            Db.Updateable(new UnitJsonTest() { Id = 1, Order = new Order { Id = 2, Name = "order2" } }).ExecuteCommand();
            list = Db.Queryable<UnitJsonTest>().ToList();
            UValidate.Check("order2", list.First().Order.Name, "Json");
            var list2 = Db.Queryable<UnitJsonTest>().ToList();
            var x = new Order() { Name = "a" };
            Db.Updateable<UnitJsonTest2>()
                 .SetColumns(it => it.Name == "a")
                 .Where(it => it.Id == 1)
                 .ExecuteCommand();
            var list3 = Db.Queryable<UnitJsonTest>().Select(it => new
            {
                x = it
            }).ToList();
            if (list3[0].x == null)
            {
                throw new Exception("unit error");
            }
            var db = Db;
            db.CodeFirst.SetStringDefaultLength(200).InitTables(typeof(SqlSugarSelect.TestModel1));
            db.CodeFirst.SetStringDefaultLength(200).InitTables(typeof(SqlSugarSelect.TestModel2));

            #region 加入数据
            var isadd = !db.Queryable<SqlSugarSelect.TestModel1>().Any();
            if (isadd)
            {
                db.Insertable(new SqlSugarSelect.TestModel1
                {
                    Ids = new Guid[] { Guid.NewGuid() },
                    Titlt = "123"
                }).ExecuteCommand();
                db.Insertable(new SqlSugarSelect.TestModel2
                {
                    Pid = 1
                }).ExecuteCommand();
            }
            #endregion
            #region 实际搜索代码，Bug所在处
            var rv = db.Queryable<SqlSugarSelect.TestModel2>()
                 .LeftJoin<SqlSugarSelect.TestModel1>((a, b) => a.Pid == b.Id)
                 .Select((a, b) => new { a, b }).ToList();
            #endregion

            db.CodeFirst.SetStringDefaultLength(2000).InitTables<UnitJsonTestadsga1>();
            db.Insertable(new UnitJsonTestadsga1() { os = new List<Order>() }).ExecuteCommand();
            db.Insertable(new UnitJsonTestadsga1() { os = new List<Order>() { new Order() { CreateTime = DateTime.Now } } }).ExecuteCommand();
            var list10 = db.Queryable<UnitJsonTestadsga1>().Select(it => new { it }).ToList();

            var jsonDb = Db;
            jsonDb.CurrentConnectionConfig.ConfigureExternalServices = new SqlSugar.ConfigureExternalServices()
            {
                EntityService = (c, p) =>
                {
                    p.IsJson = true;
                }
            };
            var list11 = jsonDb.Queryable<UNITJSONTESTADSGA1>().Select(it => new { it }).ToList();
            if (list11.FirstOrDefault().it.os == null) 
            {
                throw new Exception("unit test");
            }

            var list12 = jsonDb.Queryable<UNITJSONTESTADSGA1>().Select(it =>  it ).ToList();
            if (!list12.Any(z=>z.os.Count>0))
            {
                throw new Exception("unit test");
            }

            var list13=db.Queryable<object>().AS("UnitTestModel1").Select<ViewTestModel1>().ToList();
            if (list13.First().Ids.Count() == 0) 
            {
                throw new Exception("unit test");
            }
        }
    }
    public class UnitJsonTestadsga1
    {
        [SqlSugar.SugarColumn(Length =2000,IsJson =true)]
        public List<Order>  os{get;set;}
    }
    public class UNITJSONTESTADSGA1
    {
        [SqlSugar.SugarColumn(Length = 2000)]
        public List<Order> os { get; set; }
    }
    public class UnitJsonTest2
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        [SqlSugar.SugarColumn(IsJson = true)]
        public Order Order { get; set; }
        [SqlSugar.SugarColumn(IsNullable =true)]
        public string Name { get; set; }
    }
    public class UnitJsonTest
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        [SqlSugar.SugarColumn(ColumnDataType = "varchar(max)", IsJson = true)]
        public Order Order { get; set; }
    }
}
