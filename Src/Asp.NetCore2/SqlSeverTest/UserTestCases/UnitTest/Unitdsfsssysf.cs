using SQLitePCL;
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
    }
}
