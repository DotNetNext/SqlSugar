using SqlSeverTest.UserTestCases.UnitTest.Unitasf1;
using SqlSugar; 
using System; 
using System.Collections.Generic; 
using System.Linq; 
using System.Text; 
using System.Threading.Tasks; 

namespace OrmTest
{
    internal class UnitGridSave2
    {
        public static void Init()
        {

    

            // Get a new database connection
            // 获取一个新的数据库连接
            SqlSugarClient db = NewUnitTest.Db;
            UpdateSetColumns(db);
            // Initialize tables using CodeFirst
            // 使用 CodeFirst 初始化表
            db.CodeFirst.InitTables<Student>();

            // Clear table data
            // 清空表数据
            db.DbMaintenance.TruncateTable<Student>();

            // Insert two student records
            // 插入两条学生记录
            db.Insertable(new List<Student>() {
               new Student() {Name= "jack",CreateTime=DateTime.Now},
               new Student() {Name= "tom",CreateTime=DateTime.Now}
            }).ExecuteReturnIdentity();

            // Query all student records
            // 查询所有学生记录
            List<Student> getAll = db.Queryable<Student>().ToList();



            // Enable entity tracking for the list 'getAll'
            // 启用对列表 'getAll' 的实体跟踪
            db.Tracking(getAll);




            // Remove the first record
            // 移除第一条记录
            getAll.RemoveAt(0);

            // Modify the name of the last record
            // 修改最后一条记录的姓名
            getAll[getAll.Count - 1].Name += "_Update";

            // Add a new record
            // 添加新记录
            getAll.Add(new Student { Name = "NewRecord" });
            // Add a new record
            // 添加新记录
            getAll.Add(new Student { Name = "NewRecord" });


            // Execute GridSave operation
            // 执行 GridSave 操作
            db.GridSave(getAll).ExecuteCommand();

            // Query all students again
            // 再次查询所有学生
            var list = db.Queryable<Student>().ToList();

            db.CodeFirst.InitTables<MarkerEntity>();
            db.Context
                .Deleteable<MarkerEntity>(a => a.MarkTime.AddDays(a.KeepDays) < DateTime.Now)
                .ExecuteCommandAsync().GetAwaiter().GetResult();

            db.CodeFirst.InitTables<R04_PreBills, R01_ReceivableBills>();
            //更新 预应收账单
            var result = db.Updateable<R04_PreBills>()
                   .SetColumns(R04 => R04.R04_PaidAmount == SqlFunc.Subqueryable<R01_ReceivableBills>().Where(R01 => R01.R04_PreBillId == R04.R04_PreBillId).Select(R01 => R01.R01_PaidAmount))
                   .SetColumns(R04 => R04.R04_DiscountAmount == SqlFunc.Subqueryable<R01_ReceivableBills>().Where(R01 => R01.R04_PreBillId == R04.R04_PreBillId).Select(R01 => R01.R01_DiscountAmount))
                   .SetColumns(R04 => R04.R04_Status == SqlFunc.Subqueryable<R01_ReceivableBills>().Where(R01 => R01.R04_PreBillId == R04.R04_PreBillId).Select(R01 => (byte)R01.R01_Status))

                   .Where(R04 => R04.R04_PreBillId == 1)
                   .ExecuteCommandAsync().GetAwaiter().GetResult();

        }

        private static void UpdateSetColumns(SqlSugarClient db)
        {
            db.CodeFirst.InitTables<ProductSku2Entity>();
            var item = new ProductSku2Entity()
            {
                SalePrice2 = 11,
                SalePrice3 = 33,
                PurchasePrice = 4
            };
            db.Updateable<ProductSku2Entity>()
           .Where(p => p.SkuId == "a")
             .SetColumns(p => p.BarCode == item.BarCode)
             .SetColumns(p => p.SalePrice == item.SalePrice)
             .SetColumns(p => p.SalePrice2 == item.SalePrice2)
             .SetColumns(p => p.SalePrice3 == item.SalePrice3) 
             .ExecuteCommand();
        }

        /////////实体代码

        [SugarTable("ProductSku", TableDescription = "商品存货信息")]
        public partial class ProductSku2Entity  

        {

            /// <summary>

            /// 

            /// </summary>

            [SugarColumn(IsPrimaryKey = true, IsNullable = false, ColumnDescription = "", Length = 200)]

            public string SkuId { get; set; }





            /// <summary>

            /// 基本单位条码

            /// </summary>

            [SugarColumn(IsNullable = true, ColumnDescription = "条码", Length = 50)]

            public string  BarCode { get; set; }



            /// <summary>

            /// 基本销售价

            /// </summary>

            [SugarColumn(IsNullable = true, ColumnDescription = "基本价", Length = 18, DecimalDigits = 6)]

            public decimal  SalePrice { get; set; }



            /// <summary>

            /// 基本销售价2

            /// </summary>

            [SugarColumn(IsNullable = true, ColumnDescription = "售价2", Length = 18, DecimalDigits = 6)]

            public decimal? SalePrice2 { get; set; }



            /// <summary>

            /// 基本销售价3

            /// </summary>

            [SugarColumn(IsNullable = true, ColumnDescription = "售价3", Length = 18, DecimalDigits = 6)]

            public decimal? SalePrice3 { get; set; }



            /// <summary>

            ///  采购价

            /// </summary>

            [SugarColumn(IsNullable = true, ColumnDescription = "采购价", Length = 18, DecimalDigits = 6)]

            public decimal? PurchasePrice { get; set; }

        }

        [SugarTable("Marker")]

        public class MarkerEntity

        {

            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]

            public int Id { get; set; }

            public DateTime MarkTime { get; set; }

            public string Tag { get; set; }

            public string Value { get; set; }

            public int KeepDays { get; set; } = 7;

        }
        // Define the entity class 定义实体类
        [SugarTable("UnitSaveTablea5")]
        public class Student
        {
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int Id { get; set; }
            public string Name { get; set; }
            public DateTime CreateTime { get; set; }
        }
    }
}