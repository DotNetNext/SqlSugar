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
        public static void Filter()
        {
            var db = Db;
            db.QueryFilter.Add(new SqlSugar.TableFilterItem<UnitFilterClass1>(it=>it.id>0));

            var s1=db.Queryable<Order>().ToSql();
            if (s1.Key.Contains(">"))
            {
                throw new Exception("UnitFilter");
            }
            var s4 = db.Queryable<Order, OrderItem>((o, i) => i.OrderId == o.Id).Select("o.*").ToSql();
            if (s4.Key.Contains(">"))
            {
                throw new Exception("UnitFilter");
            }

            var s5 = db.Queryable<Order, OrderItem>((o, i) => new JoinQueryInfos(JoinType.Left, o.Id == i.OrderId))
                .Select("o.*").ToSql();
            if (s5.Key.Contains(">"))
            {
                throw new Exception("UnitFilter");
            }


            var s2 = db.Queryable<UnitFilterClass1>().ToSql();
            UValidate.Check(s2.Key, 
            @"SELECT [id],[name] FROM [UnitFilterClass1]  WHERE ( [id] > @id0 )", "UnitFilter");

            var s3= db.Queryable<UnitFilterClass1>().Where(it=>it.name!=null).ToSql();
            UValidate.Check(s3.Key,
            @"SELECT [id],[name] FROM [UnitFilterClass1]  WHERE ( [name] IS NOT NULL )  AND ( [id] > @id1 )", "UnitFilter");



            var s6 = db.Queryable<UnitFilterClass1, UnitFilterClass2>((o, i) => i.id == o.id).Select("o.*").ToSql();
            UValidate.Check(s6.Key, "SELECT o.* FROM [UnitFilterClass1] [o]  ,[UnitFilterClass2]  [i]  WHERE ( [i].[id] = [o].[id] )  AND ( [o].[id] > @id0 )", "UnitFilter");

            var s7 = db.Queryable<UnitFilterClass1, UnitFilterClass2>((o, i) => i.id == o.id).Where(o=>o.id==1).Select("o.*").ToSql();
            UValidate.Check(s7.Key, "SELECT o.* FROM [UnitFilterClass1] [o]  ,[UnitFilterClass2]  [i]  WHERE ( [i].[id] = [o].[id] )  AND ( [o].[id] = @id0 )  AND ( [o].[id] > @id1 )", "UnitFilter");

            var s8 = db.Queryable<UnitFilterClass2, UnitFilterClass1>((o, i) => i.id == o.id).Where(o => o.id == 1).Select("o.*").ToSql();
            UValidate.Check(s8.Key, "SELECT o.* FROM [UnitFilterClass2] [o]  ,[UnitFilterClass1]  [i]  WHERE ( [i].[id] = [o].[id] )  AND ( [o].[id] = @id0 )  AND ( [i].[id] > @id1 )", "UnitFilter");


            var s9 = db.Queryable<UnitFilterClass1, UnitFilterClass2>((o, i) => new JoinQueryInfos(JoinType.Left, o.id == i.id))
             .Select("o.*").ToSql();
            UValidate.Check(s9.Key, "SELECT o.* FROM [UnitFilterClass1] [o] Left JOIN [UnitFilterClass2] [i] ON ( [o].[id] = [i].[id] )   WHERE ( [o].[id] > @id0 )", "UnitFilter");


            var s10= db.Queryable<UnitFilterClass1, UnitFilterClass2>((o, i) => new JoinQueryInfos(JoinType.Left, o.id == i.id))
             .Where((o,i) =>i.id==0).Select("o.*").ToSql();
            UValidate.Check(s10.Key, "SELECT o.* FROM [UnitFilterClass1] [o] Left JOIN [UnitFilterClass2] [i] ON ( [o].[id] = [i].[id] )   WHERE ( [i].[id] = @id0 )  AND ( [o].[id] > @id1 )", "UnitFilter");

            var s11 = db.Queryable<UnitFilterClass2, UnitFilterClass1>((o, i) => new JoinQueryInfos(JoinType.Left, o.id == i.id))
            .Select("o.*").ToSql();
            UValidate.Check(s11.Key, "SELECT o.* FROM [UnitFilterClass2] [o] Left JOIN [UnitFilterClass1] [i] ON ( [o].[id] = [i].[id] )   WHERE ( [i].[id] > @id0 )", "UnitFilter");

            db.QueryFilter.Add(new SqlSugar.TableFilterItem<UnitFilterClass2>(it => it.id==0));

            var s12 = db.Queryable<UnitFilterClass2, UnitFilterClass1>((o, i) => new JoinQueryInfos(JoinType.Left, o.id == i.id))
            .Select("o.*").ToSql();
            UValidate.Check(s12.Key, "SELECT o.* FROM [UnitFilterClass2] [o] Left JOIN [UnitFilterClass1] [i] ON ( [o].[id] = [i].[id] )   WHERE ( [i].[id] > @id0 )  AND ( [o].[id] = @id1 )", "UnitFilter");

            var s13 = db.Queryable<UnitFilterClass2, UnitFilterClass1>((o, i) => new JoinQueryInfos(JoinType.Left, o.id == i.id))
            .Where(o=>o.name=="")
                .Select("o.*").ToSql();
            UValidate.Check(s13.Key, "SELECT o.* FROM [UnitFilterClass2] [o] Left JOIN [UnitFilterClass1] [i] ON ( [o].[id] = [i].[id] )   WHERE ( [o].[name] = @name0 )  AND ( [i].[id] > @id1 )  AND ( [o].[id] = @id2 )", "UnitFilter");

            db.QueryFilter
                .AddTableFilter<IDeleted>(it => it.Delete == true)
                .AddTableFilter<IOrg>(it => it.OrgId == 1);
            var sql = db.Queryable<FilterTest>().ClearFilter().ToSqlString();
            if (sql != "SELECT [Delete],[OrgId],[Name] FROM [FilterTest] ")
            {
                throw new Exception("unit error");
            }
            var sql2= db.Queryable<FilterTest>().ClearFilter<IOrg>().ToSqlString();
            if (sql2 != "SELECT [Delete],[OrgId],[Name] FROM [FilterTest]  WHERE ( [Delete] = 1 )") 
            {
                throw new Exception("unit error");
            }
            db.QueryFilter.Clear();
            //db.QueryFilter.AddTableFilter<Order>(x => x.Id==1);
            db.QueryFilter.AddTableFilter<IOrg>(x => x.OrgId == 1);
            var sql3 = db.Queryable< FilterTest, Order>((y, x) => x.Id == y.OrgId)
                .ToSqlString();
            var sql4=db.Queryable<Order, FilterTest, FilterTest>((x, y,z) => x.Id == y.OrgId)
                .ToSqlString();
            if (!sql4.Contains("AND ( [y].[OrgId] = 1 ) AND ( [z].[OrgId] = 1")) 
            {
                throw new Exception("unit error");
            }
            var sql5 = db.Queryable<FilterTest, FilterTest, FilterTest>((x, y, z) => x.OrgId == y.OrgId)
             .ToSqlString();
            if (!sql5.Contains("WHERE ( [x].[OrgId] = [y].[OrgId] )  AND ( [x].[OrgId] = 1 ) AND ( [y].[OrgId] = 1 ) AND ( [z].[OrgId] = 1 )"))
            {
                throw new Exception("unit error");
            }
            db.CodeFirst.InitTables<FilterTest>();
            db.Queryable<FilterTest, FilterTest, FilterTest>((x, y, z) => x.OrgId == y.OrgId)
                .Select("x.*").ToList();
            db.QueryFilter.Clear();
            db.Queryable<FilterTest, FilterTest, FilterTest>((x, y, z) => x.OrgId == y.OrgId)
              .Select("x.*").ToList();
        }
        public class FilterTest : IDeleted, IOrg
        {
            public bool Delete { get; set; }
            public int OrgId { get; set; }
            public string Name { get; set; }
        }
        public interface IDeleted 
        {
            public bool Delete { get; set; }
        }
        public interface IOrg
        {
            public int OrgId { get; set; }
        }
        public class UnitFilterClass1
        {
            public int id { get; set; }
            public string name { get; set; }
        }
        public class UnitFilterClass2
        {
            public int id { get; set; }
            public string name { get; set; }
        }
    }
}
