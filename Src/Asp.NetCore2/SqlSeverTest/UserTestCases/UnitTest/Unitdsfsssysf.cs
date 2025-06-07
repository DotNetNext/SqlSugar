﻿using SQLitePCL;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public class Unitdsfsssysf
    {
        public static void Init() 
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Products>();
            db.DbMaintenance.TruncateTable<Products>();
            db.Insertable(new Products() { BusinessSliceID = 1, Id = 1, ItemName = "a" }).ExecuteCommand();

            var cargolaneDbs = db.Queryable<Products>()
                .Select(it => new
                {
                    name=it.ItemName,
                    newit=it,
                })
                .ToList();
            if (cargolaneDbs.FirstOrDefault().newit.ItemName != "a") 
            {
                throw new Exception("unit error");
            }
            var cargolaneDbs2 = db.Queryable<Products>()
             .Select(it => new
             {
                 name = it.ItemName,
                 it,
             })
             .ToList();
            if (cargolaneDbs2.FirstOrDefault().it.ItemName != "a")
            {
                throw new Exception("unit error");
            }
            var cargolaneDbs3 = db.Queryable<Products>()
           .Select(it => new DTOProducts
           { 
               it=it,
           })
           .ToList();
            var names = new string[] { "aa", "bb" };
            var list=db.Queryable<Order>().Where(it => names.Any(s => it.Name.Contains(s)))
                .ToList();
            var sql = db.Queryable<Order>().Where(it => names.Any(s => it.Name.Contains(s))).ToSqlString();
            if (sql.Trim() != "SELECT [Id],[Name],[Price],[CreateTime],[CustomId] FROM [Order] [it]  WHERE  (  ([it].[Name] like '%'+ N'aa' +'%') OR ([it].[Name] like '%'+ N'bb' +'%')  ) ".Trim()) 
            {
                throw new Exception("unit error");
            }
            var sql2 = db.Queryable<OrderVarchar>().Where(it => names.Any(s => it.Name.Contains(s))).ToSqlString();
            if (sql2.Trim() != "SELECT [Id],[Name],[Price],[CreateTime],[CustomId] FROM [Order] [it]  WHERE  (  ([it].[Name] like '%'+ 'aa' +'%') OR ([it].[Name] like '%'+ 'bb' +'%')  ) ".Trim())
            {
                throw new Exception("unit error");
            }
            var ids = new List<int>() { 1, 2 };
            var list3 = db.Queryable<Order>().Where(it => ids.Any(s => it.Id==s))
            .ToList();

            db.CodeFirst.InitTables<CargoLanes>();
            db.Insertable(new CargoLanes() { ProductId = 1 }).ExecuteCommand();
            var cargolaneDbs4 = db.Queryable<CargoLanes>()
                .Select(it => new  
                {
                    it = it,
                })
                .ToList();
        }

        [SugarTable("prdev_cargolanes")]
        public class CargoLanes
        {
            /// <summary>
            /// 货道 Id
            /// </summary>
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int Id { get; set; }

            /// <summary>
            /// 产品 Id
            /// </summary>
            public int ProductId { get; set; }

            /// <summary>
            /// 产品信息
            /// </summary>
            [Navigate(NavigateType.OneToOne, nameof(ProductId), nameof(Products.Id))]
            public Products? ProductInfo { get; set; }
        }
        [SugarTable("Unitpsroducsfdsatsfd")]
        public class Products
        {
            /// <summary>
            /// 产品 Id
            /// </summary>
            [SugarColumn(IsPrimaryKey = true)]
            public int Id { get; set; }

            /// <summary>
            /// 物品名称
            /// </summary>
            public string ItemName { get; set; }


            /// <summary>
            /// 关联商品id
            /// </summary>
            public int BusinessSliceID { get; set; }

            /// <summary>
            /// 关联商品数据
            /// </summary>
            [Navigate(NavigateType.OneToOne, nameof(BusinessSliceID), nameof(Id))]
            public Products? SliceProduct { get; set; }
        }
        [SugarTable("Order")]
        public class OrderVarchar
        {
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int Id { get; set; }
            [SugarColumn(SqlParameterDbType =System.Data.DbType.AnsiString)]
            public string Name { get; set; }
            public decimal Price { get; set; }
            [SugarColumn(IsNullable = true)]
            public DateTime CreateTime { get; set; }
            [SugarColumn(IsNullable = true)]
            public int CustomId { get; set; }
            [SugarColumn(IsIgnore = true)]
            public List<OrderItem> Items { get; set; }
        }
    }

    public class DTOProducts
    {
        public string name { get; set; }
        public Unitdsfsssysf.Products it { get;   set; }
    }
}
