using SqlSugar;
using System;
using System.Collections.Generic;

namespace OrmTest
{
    internal class _a2_Sql
    {
        /// <summary>
        /// 初始化 SQL 操作的示例方法。
        /// Initializes example methods for SQL operations.
        /// </summary>
        internal static void Init()
        {
            // 获取新的数据库连接对象
            // Get a new database connection object
            var db = DbHelper.GetNewDb();

            // CodeFirst 初始化 ClassA 表
            // CodeFirst initializes the ClassA table
            db.CodeFirst.InitTables<ClassA>();
            db.Insertable(new ClassA() { Name = Guid.NewGuid().ToString("N") }).ExecuteCommand();

            // 1. 无参数查询 DataTable
            // 1. Query DataTable without parameters
            var dt1 = db.Ado.GetDataTable("SELECT * FROM Table_a2");

            // 2. 带参数查询 DataTable（简化用法）
            // 2. Query DataTable with parameters (simplified usage)
            var dt2 = db.Ado.GetDataTable("SELECT * FROM Table_a2 WHERE id=@id AND name LIKE @name",
                new { id = 1, name = "%Jack%" });

            // 3. 带参数查询 DataTable（复杂用法）
            // 3. Query DataTable with parameters (complex usage)
            var parameters = new List<SugarParameter>
            {
                new SugarParameter("@id", 1),
                new SugarParameter("@name", "%Jack%",System.Data.DbType.AnsiString)//DbType
            };
            var dt3 = db.Ado.GetDataTable("SELECT * FROM Table_a2 WHERE id=@id AND name LIKE @name", parameters);

            // 4. 带参数查询 DataTable（结合用法）
            // 4. Query DataTable with parameters (combined usage)
            var dynamicParameters = db.Ado.GetParameters(new { p = 1, p2 = "A" });
            var dt4 = db.Ado.GetDataTable("SELECT * FROM Table_a2 WHERE id=@p AND name=@p2", dynamicParameters);

            // 原生 SQL 使用实体进行查询
            // Native SQL query using an entity
            List<ClassA> entities = db.Ado.SqlQuery<ClassA>("SELECT * FROM Table_a2");
            List<ClassA> entities2 = db.Ado.SqlQuery<ClassA>("SELECT * FROM Table_a2 WHERE ID>@ID",new { ID=1});

            // 原生 SQL 使用匿名对象进行查询
            // Native SQL query using an anonymous object
            List<dynamic> anonymousObjects = db.Ado.SqlQuery<dynamic>("SELECT * FROM Table_a2");

            // 执行 SQL 命令（插入、更新、删除操作）
            // Execute SQL commands (insert, update, delete operations)
            db.Ado.ExecuteCommand("INSERT INTO Table_a2 (name) VALUES ( 'New Record')"); 
        }

        /// <summary>
        /// 示例实体类。
        /// Example entity class.
        /// </summary>
        [SugarTable("Table_a2")]
        public class ClassA
        {
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int Id { get; set; }

            public string Name { get; set; }
        }
    }
}