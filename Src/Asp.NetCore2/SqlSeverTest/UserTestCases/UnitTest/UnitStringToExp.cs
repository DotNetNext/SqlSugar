﻿using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Linq.Dynamic.Core.CustomTypeProviders;
using System.Net;
using System.Text;
using static OrmTest.UnitOneToManyFiltersadfa;

namespace OrmTest
{
    internal class UnitStringToExp
    {
        public static void Init()
        {
            Test01();
            Test02();
            Test03();
            Test04();
        }

        private static void Test04()
        {
            var db = NewUnitTest.Db;
            var userInfo = db.Queryable<Order>()
            .Select("it",
             new List<string>()
             { "it.Id as userId",
                    " {0} as id",
                   " it.Name as Name" }
            , 10)
            .ToList();
            var userInfo2 = db.Queryable<Order>()
             .Select("it",
              new List<string>()
              { "it.Id as userId",
                    " {0} as id",
                   " it.Name as Name" }
             , 10)
             .ToPageList(1, 2);
            int c = 0;
            var userInfo3 = db.Queryable<Order>()
                   .Select("it",
                    new List<string>()
                    { "it.Id as userId",
                                    " {0} as id",
                                   " it.Name as Name" }
                   , 10)
                   .ToPageList(1, 2, ref c);

            var userInfo4 = db.Queryable<Order>()
             .Select("it",
              new List<string>()
              { "it.Id as userId",
                                    " {0} as id",
                                   " it.Name as Name" }
             , 10)
             .ToListAsync().GetAwaiter().GetResult();

            var userInfo5 = db.Queryable<Order>()
               .Select("it",
                new List<string>()
                { "it.Id as userId",
                                            " {0} as id",
                                           " it.Name as Name" }
               , 10)
               .First();

            var userInfo6 = db.Queryable<Order>()
               .In(1)
               .Select("it",
                new List<string>()
                { "it.Id as userId",
                                                    " {0} as id",
                                                   " it.Name as Name" }
               , 10)
               .Single();


            var userInfo7 = db.Queryable<Order>()
               .Select("it",
                new List<string>()
                { "it.Id as userId",
                                            " {0} as id",
                                           " it.Name as Name" }
               , 10)
               .FirstAsync().GetAwaiter().GetResult();

            var userInfo8 = db.Queryable<Order>()
               .In(1)
               .Select("it",
                new List<string>()
                { "it.Id as userId",
                                                    " {0} as id",
                                                   " it.Name as Name" }
               , 10)
               .SingleAsync().GetAwaiter().GetResult();


            RefAsync<int> c2 = 0;
            var userInfo9 = db.Queryable<Order>()
                   .Select("it",
                    new List<string>()
                    { "it.Id as userId",
                                    " {0} as id",
                                   " it.Name as Name" }
                   , 10)
                   .ToPageListAsync(1, 2,   c2).GetAwaiter().GetResult();
        }

        private static void Test03()
        {
            var db = NewUnitTest.Db;
            db.StorageableByObject(new Order() { Id = 1, Name = "jack" }).ExecuteCommand();
            var xx = db.StorageableByObject(new Order() { Id = 1, Name = "jack" }).ToStorage();
            xx.AsInsertable.ExecuteCommand();
            xx.AsUpdateable.ExecuteCommand();
        }

        private static void Test01()
        {
            //Clear
            StaticConfig.DynamicExpressionParserType = null;
            StaticConfig.DynamicExpressionParsingConfig = null;

            //程序启动时配置 5.1.4.106
            StaticConfig.DynamicExpressionParserType = typeof(DynamicExpressionParser);
            var db = NewUnitTest.Db;
            var exp = DynamicCoreHelper.GetWhere<Order>("it", $"it=>it.Name== {"jack"}");
            var getAll22 = db.Queryable<Order>().Where(exp).ToList();


        }
        private static void Test02()
        {

            var db = NewUnitTest.Db;
            //Test Data
            db.CodeFirst.InitTables<UnitPerson011, UnitAddress011>();
            db.DbMaintenance.TruncateTable<UnitPerson011, UnitAddress011>();

            var address = new UnitAddress011
            {
                Street = "123 Main Street"
            };
            int addressId = db.Insertable(address).ExecuteReturnIdentity();

            // 创建 UnitPerson011 对象并插入记录
            var person = new UnitPerson011
            {
                Name = "John Doe",
                AddressId = addressId
            };
            int personId = db.Insertable(person).ExecuteReturnIdentity();

            var list = db.Queryable<UnitPerson011>().Includes(x => x.Address).ToList();

            //Clear
            StaticConfig.DynamicExpressionParserType = null;
            StaticConfig.DynamicExpressionParsingConfig = null;

            //程序启动时配置 5.1.4.106
            StaticConfig.DynamicExpressionParserType = typeof(DynamicExpressionParser);
            StaticConfig.DynamicExpressionParsingConfig = new ParsingConfig()
            {
                CustomTypeProvider = new SqlSugarTypeProvider() 
            };
            var exp = DynamicCoreHelper.GetWhere<UnitPerson011>("it", $"it=>it.Address.Street={"a"}");
            var list2=db.Queryable<UnitPerson011>().Where(exp).ToList();
             
            //导航属性动态
            var list3 = db.Queryable<UnitPerson011>().Where("it", $"SqlFunc.Exists(it.Address.Id)").ToList();
            //普通条件动态
            var list4 = db.Queryable<UnitPerson011>().Where("it", $"it.Name={"a"}").ToList();
            //动态类+动态条件
            var list5=db.QueryableByObject(typeof(UnitPerson011)).Where("it", $"it.Address.Id=={1}").ToList();

            //动态排序
            var list55 = db.Queryable<UnitPerson011>().Where("it", $"it.Name={"a"}")
                .OrderBy("it", $"it=>it.Address.Street").ToList();

            var list6 = db.Queryable<UnitPerson011>() 
                .LeftJoin<Order>((it, y) => it.Id == y.Id)
                .Where("it", $"SqlFunc.Exists(it.Address.Id)")
                .OrderBy((it, y) => it.Id)
                .ToList();
            //Select实现返回导航属性
            var list66 = db.Queryable<UnitPerson011>()
                 .Includes(it => it.Address)
            .Select(it => new
                 {
                     addid=it.Address.Id,
                     Address =it.Address
                 }).ToList();
            var list666 = db.Queryable<UnitPerson011>()
                     .Includes(it=>it.Address)
                     .Select("it",
                      new List<string>()
                      {  
                            " it.Address.Id as addid" ,
                            "it.Address as Address"
                          }
                       )
                     .ToList();
            var list66666 = db.Queryable<UnitPerson011>()
                   .Includes(it => it.Address)
                   .Select(it=>new
                   {
                       addid= it.Address.Id,
                       Address=it.Address 
                   })
                   .ToList();
            var list6666 = db.Queryable<UnitPerson011>()
                       .Includes(it => it.Address)
                       .Select("it",
                        new List<string>()
                        {
                               " new(it.Address.Id) as addid" ,
                                "it.Address as Address"
                            }
                         )
                       .ToList();
            var xxx = DynamicCoreHelper.GetMember(typeof(UnitPerson011), typeof(int), "it", $"it.Address.Id ");
            var list7= db.Queryable<UnitPerson011>().Select<int>("it", $"it.Address.Id ",typeof(UnitPerson011), typeof(int)).ToList();
            var list8 = db.Queryable<Order>().Select("it", $"new(it.Id as Id, it.Name)", typeof(Order)).ToList();

            //ok  x name
            var list9= db.QueryableByObject(typeof(UnitPerson011))
                .Select("x", $"x=>new(x.Id as Id)", typeof(Order)).ToList();
            //ok x name
            var list91 = db.QueryableByObject(typeof(UnitPerson011))
            .Select("x", $"x=>new(x.Id as Id)", typeof(Order)).ToList();

            //error y name
            var list9111 = db.QueryableByObject(typeof(UnitPerson011))
             .Select("y", $"y=>new(y.Id as Id )", typeof(Order)).ToList();

            var xxx2=DynamicCoreHelper.GetMember(
                DynamicParameters.Create("o",typeof(Order),"u", typeof(UnitPerson011)) , 
                typeof(Order),
                $"new( o.Name as Name, u.Address.Id as Id)");

            var xxx3= DynamicCoreHelper.GetMember(
                DynamicParameters.Create("o", typeof(Order), "u", typeof(UnitPerson011)) , 
                typeof(Order), $"new( o.Name as Name, u.Address.Id as Id)");


            var xxx4 = DynamicCoreHelper.GetWhere(
              DynamicParameters.Create("z", typeof(Order), "u", typeof(UnitPerson011)),  $"z.Id == u.Address.Id ");

            var shortNames = DynamicParameters.Create("x", typeof(Order), "u", typeof(OrderItem) ,"u2", typeof(OrderItem));

            var xxxx4=db.QueryableByObject(typeof(Order), "x")
                .AddJoinInfo(typeof(OrderItem), DynamicParameters.Create("x", typeof(Order), "u", typeof(OrderItem)), $"x.Id==u.OrderId", JoinType.Left)
                .AddJoinInfo(typeof(OrderItem), DynamicParameters.Create("x", typeof(Order), "u", typeof(OrderItem), "u2", typeof(OrderItem)), $"x.Id==u2.OrderId", JoinType.Left)
                .Where(shortNames, $" x.Id == u.OrderId")
                .Select(shortNames, $"new (x.Name as Name,u.OrderId as Id)",typeof(ViewOrder))
                .ToList();

        }
        public class SqlSugarTypeProvider : DefaultDynamicLinqCustomTypeProvider
        {
            public override HashSet<Type> GetCustomTypes()
            {
                var customTypes = base.GetCustomTypes();
                customTypes.Add(typeof(SqlFunc));
                return customTypes;
            }
        }
        [SugarTable("UnitPerson0afdaa1x1g1")]
        public class UnitPerson011
        {
            [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int Id { get; set; }
            public string Name { get; set; }
            public int AddressId { get; set; }
            [SqlSugar.Navigate(SqlSugar.NavigateType.OneToOne, nameof(AddressId))]
            public UnitAddress011 Address { get; set; }
        }
        [SugarTable("UnitAddress011a11fasfa")]
        public class UnitAddress011
        {
            [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int Id { get; set; }
            public string Street { get; set; }
        }
    }
}
 
