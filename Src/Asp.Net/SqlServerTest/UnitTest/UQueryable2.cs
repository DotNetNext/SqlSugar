using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public partial class NewUnitTest
    {
        public static void Queryable2()
        {
            var list4 = Db.Queryable<ABMapping>()
                            .Mapper(it => it.A, it => it.AId)
                            .Where(it => it.A.Name == "a")
                            .ToList();


            var list5 = Db.Queryable<ABMapping>()
                   .Mapper(it => it.A, it => it.AId, it => it.A.Id)
                   .Where(it => it.A.Name == "a")
                   .ToList();


            var list3 = Db.Queryable<Order>()
                .Mapper(it => it.Items, it => it.Items.First().OrderId)
                .Where(it => it.Items.Count() > 0)
                .ToList();

            var list6 = Db.Queryable<Order>()
               .Mapper(it => it.Items, it => it.Items.First().OrderId)
               .Where(it => it.Items.Any())
               .ToList();
            var list7 = Db.Queryable<Order>()
               .Mapper(it => it.Items, it => it.Items.First().OrderId)
               .Where(it => it.Items.Any(y => y.ItemId == 1))
               .ToList();

            var sql = Db.Queryable<Order>().AS("[order]").ToList();

            var sql1 = Db.Queryable<Order, OrderItem, Custom>((o, i, c) => new JoinQueryInfos(
             JoinType.Left, o.Id == i.OrderId,
             JoinType.Left, c.Id == o.CustomId
            ))
            .AS("[aa]")
            .AS<OrderItem>("[xx]")
            .AS<Custom>("[yy]")
           .Select<ViewOrder>().ToSql().Key;
            if (!sql1.Contains("[aa]") || !sql1.Contains("[xx]") || !sql1.Contains("[yy]"))
            {
                throw new Exception("unit queryable2 ");
            }
            var sql2 = Db.Queryable<OrderItem>().AS("[zz]").ToSql().Key;
            if (sql2 != "SELECT [ItemId],[OrderId],[Price],[CreateTime] FROM [zz] ")
            {
                throw new Exception("unit queryable2 ");
            }
            Db.Queryable<Order, OrderItem, Custom>((o, i, c) => new JoinQueryInfos(
             JoinType.Left, o.Id == i.OrderId,
             JoinType.Left, c.Id == o.CustomId
            ))
            .AS("[order]")
            .AS<OrderItem>("[orderdetail]")
            .AS<Custom>("[custom]")
           .Select<ViewOrder>().ToList();

            Db.Queryable<object>().AS("[order]").Select("*").ToList();

            var qu1 = Db.Queryable<Order>().Select(it => new
            {
                id = it.Id
            }).MergeTable().Select<Order>();
            var qu2 = Db.Queryable<Order>().Select(it => new
            {
                id = it.Id,
                name = it.Name
            }).MergeTable().Select<Order>();
            var list = Db.Queryable(qu1, qu2, JoinType.Left, (x, y) => x.Id == y.Id).Select((x, y) => new
            {
                id1 = x.Id,
                name = y.Name
            }).ToList();

            var qu3 = Db.Queryable<Order>().Select(it => new
            {
                id = it.Id,
                name = it.Name
            }).MergeTable()
            .Where(it => 2 > it.id).Select(it => new Order()
            {
                Id = SqlFunc.IIF(2 > it.id, 1, 2)
            }).ToList();


            var qu4 = Db.Queryable<Order>().OrderBy(it => it.Id + it.Id).ToList();

            var list11 = Db.Queryable<A>()

            .ToList();

            var list8 = Db.Queryable<A>()
            .Mapper<A, B, ABMapping>(it => ManyToMany.Config(it.AId, it.BId))
            .ToList();

            Db.CodeFirst.InitTables<ABMap, TableA, TableB>();
            Db.DbMaintenance.TruncateTable("TableA");
            Db.DbMaintenance.TruncateTable("Tableb");
            Db.DbMaintenance.TruncateTable("ABMap");
            Db.Insertable(new TableA() { id = 1, Name = "A1" }).ExecuteCommand();
            Db.Insertable(new TableA() { id = 2, Name = "A1" }).ExecuteCommand();
            Db.Insertable(new TableB() { id = 1, Name = "B1" }).ExecuteCommand();
            Db.Insertable(new TableB() { id = 2, Name = "B2" }).ExecuteCommand();
            Db.Insertable(new ABMap() { Aid = 1, Bid = 1 }).ExecuteCommand();
            Db.Insertable(new ABMap() { Aid = 1, Bid = 2 }).ExecuteCommand();
            Db.Insertable(new ABMap() { Aid = 2, Bid = 1 }).ExecuteCommand();
            Db.Insertable(new ABMap() { Aid = 2, Bid = 2 }).ExecuteCommand();
            var list9 = Db.Queryable<TableA>()
                .Mapper<TableA, TableB, ABMap>(it => ManyToMany.Config(it.Aid, it.Bid)).ToList();

            var _db = Db;
            int[] communities = { 1, 2, 3, 4, 5 };

            _db.QueryFilter.Add(new TableFilterItem<Dat_WorkBill>(it => communities.Contains(it.CommunityID ?? 0)));

            Db.CodeFirst.InitTables<Dat_WorkBill, Base_Community>();


            var rlt = _db.Queryable(_db.Queryable<Dat_WorkBill>(), _db.Queryable<Base_Community>(),

                JoinType.Left, (bill, com) => bill.CommunityID == com.CommunityID)

                .Where((bill, com) => com.IsEnable == 1)
          

                .Select((bill, com) => new

                {

                    ID = bill.WorkBillID,

                    Name = com.CommunityName

                })

                .ToList();


            var sql12 = Db.Queryable<Order, OrderItem, Custom>((o, i, c) => true)
         .AS("[aa]")
         .AS<OrderItem>("[xx]")
         .AS<Custom>("[yy]").ToSql();



            var list12 = Db.Queryable<Order>()
           .Select(it => new
           {
               name = it.Name,
               customName = SqlFunc.MappingColumn(default(string), " (select top 1 id from [Order] ) ")
           }).ToList();

            var p1 = "1";
            var p2 = "2";
          var list13 = Db.Queryable<Order>()
            .Select(it => new
            {
               name = it.Name,
               customName = SqlFunc.MappingColumn(default(string), $" (select top 1 id from [Order] where id={p1} or id={p2} ) ")
            }).ToList();
       

            _db.QueryFilter.Clear();



        }

        public class Dat_WorkBill
        {
            public int? CommunityID { get; set; }
            public string WorkBillID { get; set; }
        }
        public class Base_Community
        {
            public int? CommunityID { get; set; }
            public int IsEnable { get; set; }
            public string CommunityName { get; set; }
        }

        public class ABMap
        {
            public int Aid { get; set; }
            public int Bid { get; set; }
        }

        public class TableA
        {

            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int id { get; set; }
            public string Name { get; set; }
            [SugarColumn(IsIgnore = true)]
            public List<TableB> Childlist { get; set; }
        }


        public class TableB
        {
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int id { get; set; }
            public string Name { get; set; }
        }
    }
}
