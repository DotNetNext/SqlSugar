using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Linq;
namespace OrmTest
{
    internal class Unitadfasfa
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;

            db.Deleteable<Order>()

                    .AS("Order")

                    .In(

                        it => it.Id,

                        db.Queryable<Order>()

                       .AS("Order")

                            .Skip(11)

                            .OrderByDescending(it => it.Id)

                            .Select(it => it.Id)

                        )

                    .ExecuteCommand();
            var list = new string[] { "中文名称2", "中文名称4" };
            //如何存在删掉重新建新表
            if (db.DbMaintenance.IsAnyTable("Product", false))
            {
                db.DbMaintenance.DropTable<Manufacturer, Product, DefineManufacturerAuthentication, ManufacturerAuthentication>();
            }
            db.CodeFirst.InitTables<Manufacturer, Product, DefineManufacturerAuthentication, ManufacturerAuthentication>();
            db.Ado.ExecuteCommand(@"INSERT INTO [dbo].[Product] ([Id],  [ManufacturerId]) VALUES (1714179162048172032, 60732);
INSERT INTO [dbo].[Product] ([Id],  [ManufacturerId]) VALUES (1714179421117747200, 60731);
INSERT INTO [dbo].[Product] ([Id],  [ManufacturerId]) VALUES (1715183013903601664, 60732);
INSERT INTO [dbo].[Product] ([Id],  [ManufacturerId]) VALUES (1716346588932214784, 60733);
INSERT INTO [dbo].[Product] ([Id],  [ManufacturerId]) VALUES (1716598093790711808, 60731);
INSERT INTO [dbo].[Product] ([Id],  [ManufacturerId]) VALUES (1719258452808830976, 60732);
INSERT INTO [dbo].[Manufacturer] ([Id], [Name]) VALUES (60731, 60732);
INSERT INTO [dbo].[Manufacturer] ([Id], [Name]) VALUES (60732, 60732);
INSERT INTO [dbo].[Manufacturer] ([Id], [Name]) VALUES (60733, 60733);
INSERT INTO [dbo].[ManufacturerAuthentication] ([Id], [ManufacturerId],[AuthenticationId]) VALUES (1719001582437273600, 60731, 1708202032495726592);
INSERT INTO [dbo].[ManufacturerAuthentication] ([Id], [ManufacturerId],[AuthenticationId]) VALUES (1719003887857438720, 60731, 1708202076162625536);
INSERT INTO [dbo].[ManufacturerAuthentication] ([Id], [ManufacturerId],[AuthenticationId]) VALUES (1719003937211813888, 60732, 1708202076162625536);
INSERT INTO [dbo].[ManufacturerAuthentication] ([Id], [ManufacturerId],[AuthenticationId]) VALUES (1719006394939740160, 60732, 1708504951212347392);
INSERT INTO [dbo].[ManufacturerAuthentication] ([Id], [ManufacturerId],[AuthenticationId]) VALUES (1719009872995028992, 60732, 1708546354562863104);
INSERT INTO [dbo].[ManufacturerAuthentication] ([Id], [ManufacturerId],[AuthenticationId]) VALUES (1719019162610831360, 60733, 1708202032495726592);
INSERT INTO [dbo].[ManufacturerAuthentication] ([Id], [ManufacturerId],[AuthenticationId]) VALUES (1719019162648580096, 60733, 1708546354562867777);
INSERT INTO [dbo].[DefineManufacturerAuthentication] ([Id], [CnAuthentication]) VALUES (1708202032495726592, N'中文名称1');
INSERT INTO [dbo].[DefineManufacturerAuthentication] ([Id], [CnAuthentication]) VALUES (1708202076162625536, N'中文名称2');
INSERT INTO [dbo].[DefineManufacturerAuthentication] ([Id], [CnAuthentication]) VALUES (1708504951212347392, N'中文名称3');
INSERT INTO [dbo].[DefineManufacturerAuthentication] ([Id], [CnAuthentication]) VALUES (1708546354562863104, N'中文名称4');
INSERT INTO [dbo].[DefineManufacturerAuthentication] ([Id], [CnAuthentication])  VALUES (1708546354562866666, N'中文名称5');
INSERT INTO [dbo].[DefineManufacturerAuthentication] ([Id], [CnAuthentication]) VALUES (1708546354562867777, N'中文名称7');
INSERT INTO [dbo].[DefineManufacturerAuthentication] ([Id], [CnAuthentication]) VALUES (1708747701719535616, N'中文名称8');
");
            Test1(db, list);
            Test2(db, list);
            Test3(db, list);
            Test4(db, list);
        }

        private static void Test1(SqlSugarClient db, string[] list)
        {
            var list2 = db.Queryable<Manufacturer>()
                 .Where(it => it.Authentications.Any(s => list.Contains(s.Authentication.CnAuthentication)))
                 .ToList();

            var list3 = db.Queryable<Manufacturer>().Includes(x =>x.Authentications, it => it.Authentication)
                .ToList();

            var list4 = list3.Where(it => it.Authentications.Any(s => list.Contains(s.Authentication.CnAuthentication)))
              .ToList();

            if (list2.Count != list4.Count) 
            {
                throw new Exception("unit error");
            }
        }
        private static void Test2(SqlSugarClient db, string[] list)
        {
            var list2 = db.Queryable<Product>()
                 .Where(it => it.Manufacturer.Authentications.Any(s => list.Contains(s.Authentication.CnAuthentication)))
                 .ToList();

            var list3 = db.Queryable<Product>().Includes(x => x.Manufacturer, it => it.Authentications, it => it.Authentication)
                .ToList();

            var list4 = list3.Where(it => it.Manufacturer.Authentications.Any(s => list.Contains(s.Authentication.CnAuthentication)))
              .ToList();

            if (list2.Count != list4.Count) 
            {
                throw new Exception("unit error");
            }
        }
        private static void Test3(SqlSugarClient db, string[] list)
        {
            var list2 = db.Queryable<Product>()
                 .Where(it => it.Manufacturer.Authentications.Any(s => list.Contains(s.Authentication.CnAuthentication)))
                 .ToList();

            var list3 = db.Queryable<Product>().Includes(x => x.Manufacturer, it => it.Authentications, it => it.Authentication)
                .ToList();

            var list4 = list3.Where(it => it.Manufacturer.Authentications.Any(s => list.Contains(s.Authentication.CnAuthentication)))
              .ToList();
        }
        private static void Test4(SqlSugarClient db, string[] list)
        {
            var list2 = db.Queryable<Product>()
                 .Where(it => it.Manufacturer.Authentications.Any(s =>  s.Authentication.CnAuthentication!=""))
                 .ToList();

            var list3 = db.Queryable<Product>().Includes(x => x.Manufacturer, it => it.Authentications, it => it.Authentication)
                .ToList();

            var list4 = list3.Where(it => it.Manufacturer.Authentications.Any(s => s.Authentication.CnAuthentication != ""))
              .ToList();

            if (list2.Count != list4.Count) 
            {
                throw new Exception("unit error");
            }
        }

        /// <summary>
        /// 商品 和供应商多对一
        /// </summary>
        public class Product
        {
            [SugarColumn(ColumnDescription = "Id", IsPrimaryKey = true)]
            public long Id { get; set; }
            /// <summary>
            ///   50个字段...
            /// </summary>
            /// <summary>
            /// 供应商Id
            /// </summary>
            [SugarColumn(ColumnDescription = "供应商Id")]
            public long? ManufacturerId { get; set; }
            /// <summary>
            /// 供应商
            /// </summary>
            [SugarColumn(IsIgnore = true)]
            [Navigate(NavigateType.OneToOne, nameof(ManufacturerId))]
            public Manufacturer? Manufacturer { get; set; }
        }
        /// <summary>
        /// 和供应商一对一
        /// </summary>
        public class Manufacturer
        {
            [SugarColumn(ColumnDescription = "Id", IsPrimaryKey = true)]
            public long Id { get; set; }
            /// <summary>
            /// 供应商名称
            /// </summary>
            [SugarColumn(ColumnDescription = "供应商名称", Length = 64)]
            public string? Name { get; set; }

            /// <summary>
            /// 认证
            /// </summary>
            [SugarColumn(IsIgnore = true)]
            [Navigate(NavigateType.OneToMany, nameof(ManufacturerAuthentication.ManufacturerId))]
            public List<ManufacturerAuthentication>? Authentications { get; set; }
        }
        /// <summary>
        /// 和供应商认证一对多
        /// </summary>
        public class ManufacturerAuthentication
        {
            [SugarColumn(ColumnDescription = "Id", IsPrimaryKey = true)]
            public long Id { get; set; }

            /// <summary>
            /// 供应商Id
            /// </summary>
            [Required]
            [SugarColumn(ColumnDescription = "供应商Id")]
            public long? ManufacturerId { get; set; }
            /// <summary>
            /// 供应商
            /// </summary>
            [SugarColumn(IsIgnore = true)]
            [Navigate(NavigateType.OneToOne, nameof(ManufacturerId))]
            public Manufacturer? Manufacturer { get; set; }
            /// <summary>
            /// 认证Id
            /// </summary>
            [SugarColumn(ColumnDescription = "认证Id")]
            public long? AuthenticationId { get; set; }
            /// <summary>
            /// 认证
            /// </summary>
            [SugarColumn(IsIgnore = true)]
            [Navigate(NavigateType.OneToOne, nameof(AuthenticationId))]
            public DefineManufacturerAuthentication? Authentication { get; set; }

        }
        /// <summary>
        /// 和认证一对一
        /// </summary>
        public class DefineManufacturerAuthentication
        {
            [SugarColumn(ColumnDescription = "Id", IsPrimaryKey = true)]
            public long Id { get; set; }
            /// <summary>
            /// 中文名称
            /// </summary>
            [SugarColumn(ColumnDescription = "中文名称", Length = 64)]
            [MaxLength(64)]
            public string? CnAuthentication { get; set; }

        }
    }
}
