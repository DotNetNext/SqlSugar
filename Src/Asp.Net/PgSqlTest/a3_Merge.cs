using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    internal class _a3_Merge
    {
        // 中文备注：初始化方法（用于大数据处理）
        // English Comment: Initialization method (for big data processing)
        internal static void Init()
        {
            var db = DbHelper.GetNewDb();
            //建表
            //Create table 
            db.CodeFirst.InitTables<Order>();
            var list = new List<Order>() { new Order() { Name = "jack" } };

            // 中文备注：执行插入或更新操作
            // English Comment: Perform insert or update operation
            db.Storageable(list).ExecuteCommand();

            // 中文备注：分页执行插入或更新操作，每页1000条记录
            // English Comment: Perform insert or update operation with paging, 1000 records per page
            db.Storageable(list).PageSize(1000).ExecuteCommand();

            // 中文备注：带异常处理的分页插入或更新操作，每页1000条记录
            // English Comment: Perform insert or update operation with exception handling and paging, 1000 records per page
            db.Storageable(list).PageSize(1000, exrows => { }).ExecuteCommand();

            // 中文备注：使用Fastest方式批量合并数据（用于大数据处理）
            // English Comment: Merge data using Fastest method (for big data processing)
            db.Fastest<Order>().BulkMerge(list);

            // 中文备注：分页使用Fastest方式批量合并数据，每页100000条记录（用于大数据处理）
            // English Comment: Merge data using Fastest method with paging, 100000 records per page (for big data processing)
            db.Fastest<Order>().PageSize(100000).BulkMerge(list);
        }

        [SqlSugar.SugarTable("Order_a3")]
        public class Order
        {
            [SqlSugar.SugarColumn(IsPrimaryKey =true)]
            public long Id { get; set; }
            public string Name { get; set; }
            // 其他属性
        }
    }
}