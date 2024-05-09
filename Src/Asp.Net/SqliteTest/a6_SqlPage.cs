using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    internal class _a6_SqlPage
    {
        /// <summary>
        /// <summary>
        /// Initializes an example method for SQL paging operations.
        /// 初始化 SQL 分页操作的示例方法。
        /// </summary>
        internal static void Init()
        {
            // Get a new database connection object
            // 获取新的数据库连接对象
            var db = DbHelper.GetNewDb();

            // CodeFirst initializes the ClassA table
            // CodeFirst 初始化 ClassA 表
            db.CodeFirst.InitTables<ClassA>();
            for (int i = 0; i < 16; i++)
            {
                db.Insertable(new ClassA() { Name = Guid.NewGuid().ToString("N") }).ExecuteCommand();
            }


            // Query data using paging and get the total count
            // 使用分页查询数据，并获取总记录数
            int count = 0; 
            var list = db.SqlQueryable<ClassA>("select * from Table_a6").ToPageList(1, 5, ref count);

             

            // Asynchronously query data using paging and get the total count
            // 使用异步方式分页查询数据，并获取总记录数
            RefAsync<int> countAsync = 0;
            var listAsync = db.SqlQueryable<ClassA>("select * from Table_a6").ToPageListAsync(1, 5, countAsync).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Example entity class.
        /// 示例实体类。
        /// </summary>
        [SugarTable("Table_a6")]
        public class ClassA
        {
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int Id { get; set; }

            public string Name { get; set; }
        }
    }
}