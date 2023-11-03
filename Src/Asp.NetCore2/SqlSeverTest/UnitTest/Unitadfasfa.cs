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
            var list=new string[] {"a","b" };
            //如何存在删掉重新建新表
            if (db.DbMaintenance.IsAnyTable("Product", false)) 
            {
                db.DbMaintenance.DropTable<Manufacturer, Product, DefineManufacturerAuthentication, ManufacturerAuthentication>();
            }
            db.CodeFirst.InitTables<Manufacturer, Product,DefineManufacturerAuthentication, ManufacturerAuthentication >();
            db.Queryable<Manufacturer>()
                .Where(it => it.Authentications.Any(s => list.Contains(s.Authentication.CnAuthentication)))
                .ToList();
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
